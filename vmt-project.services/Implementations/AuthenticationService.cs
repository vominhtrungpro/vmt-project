
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NetCore.Infrastructure.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Model.Config;
using vmt_project.models.Request.Authentication;
using vmt_project.models.Request.Email;
using vmt_project.models.Response.Authentication;
using vmt_project.services.Contracts;
using static vmt_project.common.Constants.ResponseMessages;

namespace vmt_project.services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfigurationService _configurationService;
        private readonly JwtConfig _jwtConfig;
        public AuthenticationService(UserManager<User> userManager, IEmailService emailService,IConfigurationService configurationService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configurationService = configurationService;
            _jwtConfig = _configurationService.GetJwtConfig();
        }
        public async Task<AppActionResultData<LoginResponse>> Login(LoginRequest request)
        {
            var result = new AppActionResultData<LoginResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return result.BuildError(ERROR_LOGIN_USER_NOT_FOUND);
                }
                var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isValid)
                {
                    return result.BuildError(ERROR_LOGIN_WRONG_PASSWORD);
                }
                var token = new LoginResponse()
                {
                    AccessToken = await CreateAccessTokenAsync(user),
                    RefreshToken = await CreateRefreshTokenAsync(user)
                };
                result.BuildResult(token);
                return result;
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message);
            }
        }
        public async Task<AppActionResultData<LoginResponse>> LoginWithGooogle(LoginWithGoogleRequest request)
        {
            var result = new AppActionResultData<LoginResponse>();
            string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.Token);

                var response = await httpClient.GetAsync(userInfoEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        GoogleUserInfoResponse userInfo = JsonConvert.DeserializeObject<GoogleUserInfoResponse>(jsonResponse);
                        var user = await _userManager.FindByEmailAsync(userInfo.Email);
                        if (user == null)
                        {
                            var password = GenerateRandomPassword(null);
                            var userEntity = new User()
                            {
                                Email = userInfo.Email,
                                UserName = userInfo.Email,
                            };
                            var addUserResult = await _userManager.CreateAsync(userEntity, password);
                            if (addUserResult.Succeeded)
                            {
                                var sendEmailRequest = new SendEmailRequest()
                                {
                                    To = userInfo.Email,
                                    Subject = "Register",
                                    Body = password
                                };
                                await _emailService.SendEmail(sendEmailRequest);
                                user = userEntity;
                            } else
                            {
                                return result.BuildError(ERROR_REGISTER_USER);
                            }
                        }
                        var token = new LoginResponse()
                        {
                            AccessToken = await CreateAccessTokenAsync(user),
                            RefreshToken = await CreateRefreshTokenAsync(user)
                        };
                        result.BuildResult(token);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return result.BuildError(ex.Message);
                    }
                }
                else
                {
                    return result.BuildError(ERROR_LOGIN_GOOGLE_USER);
                }
            }
        }
        public async Task<AppActionResultData<IdentityResult>> Register(RegisterRequest request)
        {
            var result = new AppActionResultData<IdentityResult>();
            try
            {
                // check if email exist
                var checkEmailExist = _userManager.FindByEmailAsync(request.Email);
                if (checkEmailExist.Result != null)
                {
                    return result.BuildError(ERROR_REGISTER_EMAIL_EXIST);
                }
                // check if username exist
                var checkUserNameExist = _userManager.FindByNameAsync(request.UserName);
                if (checkUserNameExist.Result != null)
                {
                    return result.BuildError(ERROR_REGISTER_USERNAME_EXIST);
                }
                // generate random password
                var password = GenerateRandomPassword(null);

                // init user from request
                var user = new User()
                {
                    Email = request.Email,
                    UserName = request.UserName,
                };

                var addUserResult = await _userManager.CreateAsync(user, password);
                if (addUserResult.Succeeded)
                {
                    var sendEmailRequest = new SendEmailRequest()
                    {
                        To = request.Email,
                        Subject = "Register",
                        Body = password
                    };
                    await _emailService.SendEmail(sendEmailRequest);

                    return result.BuildResult(addUserResult, SUCCESS_REGISTER_USER);
                }
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message);
            }
            return result.BuildError(ERROR_REGISTER_USER);
        }
        private static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 10,
                RequiredUniqueChars = 5,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        private async Task<string> CreateAccessTokenAsync(User user)
        {
            var claims = await GetClaims(user);
            return GenerateAccessToken(claims);
        }
        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserName", user.UserName),
                new Claim("UserId", user.Id),
                new Claim("Email", user.Email),
                new Claim("tokenExpireTime", DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiresIn).Ticks.ToString()),
                new Claim("refreshExpireTime", DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenExpiresIn).Ticks.ToString()),
            };
            if (user.UserInfo != null)
            {
                claims.Add(new Claim("FirstName", user.UserInfo.FirstName ?? string.Empty));
                claims.Add(new Claim("LastName", user.UserInfo.LastName ?? string.Empty));
                claims.Add(new Claim("AvatarUrl", user.UserInfo.AvatarUrl ?? string.Empty));
            }

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("Role", role));
            }
            return claims;
        }
        private string GenerateAccessToken(List<Claim> claims)
        {
            var signingCredentials = GetSigningCredentials();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.AccessTokenExpiresIn),
                signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        public async Task<string> CreateRefreshTokenAsync(User user)
        {
            var claims = await GetClaims(user);
            var refreshToken = GenerateRefreshToken(claims);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenExpiresIn);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return refreshToken;
            }

            return string.Empty;
        }
        private string GenerateRefreshToken(List<Claim> claims)
        {
            var signingCredentials = GetSigningCredentials();
            var tokenOptions = GenerateRefreshTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        private JwtSecurityToken GenerateRefreshTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.RefreshTokenExpiresIn).AddDays(1),
                signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
    }
}
