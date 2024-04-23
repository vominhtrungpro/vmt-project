using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.common.Constants
{
    public class ResponseMessages
    {
        #region Success Messages
        public const string SUCCESS_CREATE_CHARACTER = "Character created!";
        public const string SUCCESS_REGISTER_USER = "Register success!";
        public const string SUCCESS_LOGIN_USER = "Login success!";
        #endregion

        #region Error messages
        public const string ERROR_REGISTER_EMAIL_EXIST = "Email exist!";
        public const string ERROR_REGISTER_PHONE_EXIST = "Phone exist!";
        public const string ERROR_REGISTER_USERNAME_EXIST = "Username exist!";
        public const string ERROR_REGISTER_USER = "Register error!";
        public const string ERROR_LOGIN_USER = "Login error!";
        public const string ERROR_LOGIN_USER_NOT_FOUND = "Username not found!";
        public const string ERROR_LOGIN_WRONG_PASSWORD = "Wrong password!";
        #endregion
    }
}
