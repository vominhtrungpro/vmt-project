using Confluent.Kafka;
using Nest;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.Character;
using vmt_project.models.OpenAI;
using vmt_project.services.Contracts;
using Newtonsoft.Json;
using vmt_project.models.Request.Authentication;
using vmt_project.models.Request.Campaign;
using NetCore.Infrastructure.Common.Models;
using vmt_project.common.Constants;
using StackExchange.Redis;
using System.Security.Policy;


namespace vmt_project.services.OpenAI
{
    public class ChatOpenAIService : IChatOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = Environment.GetEnvironmentVariable("ApiKey");
        private readonly string assistantId = Environment.GetEnvironmentVariable("AssistantId");
        private readonly ICharacterService _characterService;
        private readonly IAuthenticationService _authenticationService;

        public ChatOpenAIService(HttpClient httpClient, ICharacterService characterService, IAuthenticationService authenticationService)
        {
            _httpClient = httpClient;
            _characterService = characterService;
            _authenticationService = authenticationService;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        }
        public async Task<FunctionCallingResponse> FunctionCalling(string text)
        {
            var msgs = new List<Message>
            {
                new Message
                {
                    role = "user",
                    content = text
                }
            };

            var tools = InitTool();


            var requestBody = new
            {
                model = "gpt-4o",
                messages = msgs,
                tools = tools,
                tool_choice = "auto"
            };

            string responseBody = await ChatCompletion(requestBody);
            FunctionCallingResponse responseData = JsonConvert.DeserializeObject<FunctionCallingResponse>(responseBody);
            var responseMessage = responseData.choices[0].message;
            var tool_calls = responseMessage.tool_calls;
            if (tool_calls != null)
            {
                msgs.Add(responseMessage);

                foreach (var toolCall in tool_calls)
                {
                    var functionName = toolCall.function.name;
                    var funcResponse = await GetResponseFromToolCall(toolCall);
                    msgs.Add(new Message
                    {
                        tool_call_id = toolCall.id,
                        role = "tool",
                        name = functionName,
                        content = funcResponse,
                    });
                }

                var requestBody2 = new
                {
                    model = "gpt-4o",
                    messages = msgs,
                };

                string responseBody2 = await ChatCompletion(requestBody2);
                FunctionCallingResponse responseData2 = JsonConvert.DeserializeObject<FunctionCallingResponse>(responseBody2);
                return responseData2;
            }
            else
            {
                return responseData;
            }
        }
        private async Task<string> ChatCompletion(object requestBody)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(requestBody, settings);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);


            response.EnsureSuccessStatusCode();


            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        public async Task<MessgeResponse> AssistantChat(string msg,string threadId)
        {
            var modifyResponse = await ModifyAssistant();
            string responseBody = "";
            if (threadId != null)
            {
                var createMsgResponse = await CreateMessage(msg, threadId);
                responseBody = await CreateRun(threadId);
            }
            else
            {
                responseBody = await CreateThreadAndRun(msg);
            }
            
            var runId = "";
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                if (doc.RootElement.TryGetProperty("thread_id", out JsonElement threadIdElement))
                {
                    threadId = threadIdElement.GetString();
                }
                if (doc.RootElement.TryGetProperty("id", out JsonElement runIdElement))
                {
                    runId = runIdElement.GetString();
                }
            }

            while (true)
            {
                string runUrl = $"https://api.openai.com/v1/threads/{threadId}/runs/{runId}";
                HttpResponseMessage runResponse = await _httpClient.GetAsync(runUrl);

                runResponse.EnsureSuccessStatusCode();
                string runResponseBody = await runResponse.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(runResponseBody);
                string status = (string)jsonObject["status"];
                if (status == "completed")
                {
                    break;
                }
                if (status == "requires_action")
                {
                    string toolCallsJson = jsonObject["required_action"]["submit_tool_outputs"]["tool_calls"].ToString(Formatting.Indented);

                    List<ToolCall> toolCalls = JsonConvert.DeserializeObject<List<ToolCall>>(toolCallsJson);
                    foreach (var toolCall in toolCalls)
                    {
                        var functionResponse = await GetResponseFromToolCall(toolCall);
                        var outputResponse = await SubmitToolOutputsToRun(toolCall.id, functionResponse, threadId, runId);
                    }
                    
                }
            }

            string msgUrl = $"https://api.openai.com/v1/threads/{threadId}/messages";
            HttpResponseMessage messageResponse = await _httpClient.GetAsync(msgUrl);

            messageResponse.EnsureSuccessStatusCode();

            string messageResponseBody = await messageResponse.Content.ReadAsStringAsync();
            MessgeResponse msgResponse = JsonConvert.DeserializeObject<MessgeResponse>(messageResponseBody);

            return msgResponse;
        }
        private async Task<string> SubmitToolOutputsToRun(string toolCallId, string output, string threadId, string runId)
        {
            var request = new
            {
                tool_outputs = new[]
                {
                    new {
                        tool_call_id = toolCallId,
                        output = output,
                    }
                }
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(request, settings);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://api.openai.com/v1/threads/{threadId}/runs/{runId}/submit_tool_outputs";
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);


            response.EnsureSuccessStatusCode();


            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private async Task<string> GetAllCharacter()
        {
            var characters = await _characterService.List();

            return JsonConvert.SerializeObject(characters);
        }
        private async Task<string> CreateACharacter(string name)
        {
            var character = new CreateCharacterRequest { Name = name };
            var result = await _characterService.Create(character);
            return JsonConvert.SerializeObject(result);
        }
        private async Task<string> RegisterAccount(string email, string username)
        {
            var register = new RegisterRequest { Email = email, UserName = username };
            var result = await _authenticationService.Register(register);
            return JsonConvert.SerializeObject(result);
        }
        private async Task<string> CreateCampaign(CreateCampaignRequest request)
        {
            string url = "https://app-simplyblast-api-qa-sea.azurewebsites.net/api/Campaign";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

                var jwtToken = await GetJwtToken();

                var json = JsonConvert.SerializeObject(request);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                httpRequest.Headers.Add("Authorization", "Bearer "+jwtToken);

                HttpResponseMessage response = await client.SendAsync(httpRequest);

                var content = await response.Content.ReadAsStringAsync();

                var apiResult = JsonConvert.DeserializeObject<AppApiResult>(content);

                return JsonConvert.SerializeObject(apiResult);
            }

        }
        private async Task<string> GetTemplates()
        {
            string url = "https://app-simplyblast-api-qa-sea.azurewebsites.net/api/template";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

                var jwtToken = await GetJwtToken();

                httpRequest.Headers.Add("Authorization", "Bearer " + jwtToken);

                HttpResponseMessage response = await client.SendAsync(httpRequest);

                var content = await response.Content.ReadAsStringAsync();

                var apiResult = JsonConvert.DeserializeObject<AppApiResult>(content);

                return JsonConvert.SerializeObject(apiResult);
            }
        }
        private async Task<string> GetJwtToken()
        {
            string url = "https://app-simplyblast-api-qa-sea.azurewebsites.net/api/auth/login";
            string accessToken = "";
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

                var request = new
                {
                    email = "tenantadmin2@yopmail.com",
                    password = "Password@123"
                };
                var json = JsonConvert.SerializeObject(request);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(httpRequest);

                var content = await response.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(content);
                JsonElement root = document.RootElement;

                if (root.TryGetProperty("data", out JsonElement dataElement) &&
                    dataElement.TryGetProperty("accessToken", out JsonElement accessTokenElement))
                {
                    accessToken = accessTokenElement.GetString();
                    Console.WriteLine($"Access Token: {accessToken}");
                }


                return accessToken;
            }
        }
        private List<Tool> InitTool()
        {
            var tools = new List<Tool>();
            tools.Add(new Tool() 
            {
                type = "function",
                function = new Function() 
                {
                    name = "get_all_character",
                    description = "Get the name of all character"
                }
            });
            tools.Add(new Tool()
            {
                type = "function",
                function = new Function()
                {
                    name = "get_templates",
                    description = "Get all template data"
                }
            });
            tools.Add(new Tool()
            {
                type = "function",
                function = new Function()
                {
                    name = "create_a_character",
                    description = "Create a new character",
                    parameters = new Parameters()
                    {
                        type = "object",
                        properties = new Dictionary<string, object>()
                        {
                            {
                                "name", new Dictionary<string, string>
                                {
                                    { "type", "string" },
                                    { "description", "Name of the character." }
                                }
                            },
                        },
                        required = new List<string> { "name" }
                    }
                }
            });
            tools.Add(new Tool()
            {
                type = "function",
                function = new Function()
                {
                    name = "register_account",
                    description = "Create or register a new account",
                    parameters = new Parameters()
                    {
                        type = "object",
                        properties = new Dictionary<string, object>()
                        {
                            {
                                "username", new Dictionary<string, string>
                                {
                                    { "type", "string" },
                                    { "description", "Username of account." }
                                }
                            },
                            {
                               "email", new Dictionary<string, string>
                                {
                                    { "type", "string" },
                                    { "description", "Email of account." }
                                }
                            },
                        },
                        required = new List<string> { "username", "email" }
                    }
                }
            });
            tools.Add(new Tool()
            {
                type = "function",
                function = new Function()
                {
                    name = "create_campaign",
                    description = "Create a new campaign",
                    parameters = new Parameters()
                    {
                        type = "object",
                        properties = new Dictionary<string, object>()
                        {
                            {
                                "Name", new Dictionary<string, string>
                                {
                                    { "type", "string" },
                                    { "description", "Name of campaign." }
                                }
                            },
                            {
                               "IsEnableTwoWay", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of campaign can enable two way, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "IsBypassUnsubBlock", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of campaign can bypass unsub block, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "RecipientIsUnconfirmed", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of campaign recipient is unconfirmed, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "RecipientIsSubscribe", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of campaign recipient is subscribe, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "RecipientIsUnsubscribe", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of campaign recipient is unsubscribe, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "RecipientTagFilters", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "A list of tag guid separated by a comma, example: '1817631c-aa37-438d-82ed-f941e45d257a','1817631c-aa37-438d-82ed-f941e45d257f,'1817631c-aa37-438d-82ed-f941e45d257d''" }
                               }
                            },
                            {
                               "MessageId", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "A guid of message example: 'db291242-f120-45d3-b8ac-e1b749e1448b'" }
                               }
                            },
                            {
                               "MessageIsInitialize", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Is boolean of message an initialize message, example: 'True' or 'False'." }
                               }
                            },
                            {
                               "MessageName", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Name of message." }
                               }
                            },
                            {
                               "MessageContent", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Content of message." }
                               }
                            },
                            {
                               "MessageTemplateId", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "A guid of message template example: 'be291a03-7f9b-4b97-983a-57e40212c4b1' or you can get templateId from datas with get_templates function" }
                               }
                            },
                            {
                               "MessageBroadcastSchedule", new Dictionary<string, string>
                               {
                                    { "type", "string" },
                                    { "description", "Schedule of campaign broadcast require UTC time, example: '2024-07-30T02:27:29.2318442'" }
                               }
                            },
                        },
                        required = new List<string> 
                        { 
                            "Name", 
                            "IsEnableTwoWay", 
                            "IsBypassUnsubBlock",
                            "RecipientIsUnconfirmed", 
                            "RecipientIsSubscribe", 
                            "RecipientIsUnsubscribe", 
                            "RecipientTagFilters", 
                            "MessageId", 
                            "MessageIsInitialize", 
                            "MessageName", 
                            "MessageContent", 
                            "MessageTemplateId", 
                            "MessageBroadcastSchedule" 
                        }
                    }
                }
            });
            return tools;

        }
        private async Task<string> GetResponseFromToolCall(ToolCall toolCall)
        {
            var functionName = toolCall.function.name;
            var functionArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(toolCall.function.arguments);
            var funcResponse = "";
            if (functionName == "get_all_character")
            {
                funcResponse = await GetAllCharacter();
            }
            if (functionName == "create_a_character")
            {
                var name = functionArgs["name"]?.ToString();
                funcResponse = await CreateACharacter(name);
            }
            if (functionName == "register_account")
            {
                var email = functionArgs["email"]?.ToString();
                var userName = functionArgs["username"]?.ToString();
                funcResponse = await RegisterAccount(email, userName);
            }
            if (functionName == "get_templates")
            {
                funcResponse = await GetTemplates();
            }
            if (functionName == "create_campaign")
            {
                var request = new CreateCampaignRequest()
                {
                    Name = functionArgs["Name"]?.ToString(),
                    IsEnableTwoWay = Convert.ToBoolean(functionArgs["IsEnableTwoWay"]?.ToString()),
                    IsBypassUnsubBlock = Convert.ToBoolean(functionArgs["IsBypassUnsubBlock"]?.ToString()),
                    Status = common.Enums.CampaignStatus.Pending,
                    EmailNotify = null,
                    RecipientRequest = new CampaignRecipientRequest()
                    {
                        IsUnconfirmed = Convert.ToBoolean(functionArgs["RecipientIsUnconfirmed"]?.ToString()),
                        IsSubscribe = Convert.ToBoolean(functionArgs["RecipientIsSubscribe"]?.ToString()),
                        IsUnsubscribe = Convert.ToBoolean(functionArgs["RecipientIsUnsubscribe"]?.ToString()),
                        TagFilters = new List<string>(functionArgs["RecipientTagFilters"]?.ToString().Split(','))
                    },
                    MessageRequests = new List<CampaignMessageRequest>()
                    {
                        new CampaignMessageRequest()
                        { 
                            Id = Guid.Parse(functionArgs["MessageId"]?.ToString()),
                            IsInitialize = Convert.ToBoolean(functionArgs["MessageIsInitialize"]?.ToString()),
                            Name = functionArgs["MessageName"]?.ToString(),
                            Content = functionArgs["MessageContent"]?.ToString(),
                            TemplateId = Guid.Parse(functionArgs["MessageTemplateId"]?.ToString()),
                            BroadcastSchedule = DateTime.Parse(functionArgs["MessageBroadcastSchedule"]?.ToString()),
                        }
                    },
                };
                funcResponse = await CreateCampaign(request);
            }
            return funcResponse;
        }
        private async Task<string> CreateThreadAndRun(string msg)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var request = new
            {
                assistant_id = assistantId,
                thread = new
                {
                    messages = new List<Message>()
                    {
                        new Message()
                        {
                            role = "user",
                            content = msg
                        }
                    }
                }
            };
            string json = JsonConvert.SerializeObject(request, settings);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/threads/runs", content);


            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        private async Task<string> CreateMessage(string msg,string threadId)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var request = new
            {
                role = "user",
                content = msg
            };
            string json = JsonConvert.SerializeObject(request, settings);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var msgUrl = $"https://api.openai.com/v1/threads/{threadId}/messages";

            HttpResponseMessage response = await _httpClient.PostAsync(msgUrl, content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        private async Task<string> CreateRun(string threadId)
        {
            var requestPayload = new
            {
                assistant_id = assistantId
            };

            string jsonPayload = JsonConvert.SerializeObject(requestPayload);

            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var requestUrl = $"https://api.openai.com/v1/threads/{threadId}/runs";

            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        private async Task<string> ModifyAssistant()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var tools = InitTool();
            var requestPayload = new
            {
                tools = tools,
                model = "gpt-4o"
            };

            string jsonPayload = JsonConvert.SerializeObject(requestPayload,settings);

            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var requestUrl = $"https://api.openai.com/v1/assistants/{assistantId}";

            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }

}
