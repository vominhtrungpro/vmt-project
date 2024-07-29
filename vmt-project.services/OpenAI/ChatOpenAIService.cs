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


namespace vmt_project.services.OpenAI
{
    public class ChatOpenAIService : IChatOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = Environment.GetEnvironmentVariable("ApiKey");
        private readonly string assistantId = Environment.GetEnvironmentVariable("AssistantId");
        private readonly ICharacterService _characterService;

        public ChatOpenAIService(HttpClient httpClient, ICharacterService characterService)
        {
            _httpClient = httpClient;
            _characterService = characterService;

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
