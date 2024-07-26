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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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

    }

}
