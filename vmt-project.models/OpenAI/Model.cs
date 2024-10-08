﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace vmt_project.models.OpenAI
{
    public class Model
    {
    }
    public class ChatCompletion
    {
        public string? id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        public long created { get; set; }
        public string? model { get; set; }
        public string? systemfingerprint { get; set; }
        public List<Choice>? choices { get; set; }
        public Usage? usage { get; set; }
    }
    public class FunctionCallingResponse
    {
        public string? id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        public long created { get; set; }

        public string? model { get; set; }

        public Choice[]? choices { get; set; }

        public Usage? usage { get; set; }
    }
    public class MessgeResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; }
        
        public List<MessageData> data { get; set; }
        public string first_id { get; set; }
        public string last_id { get; set; }
        public bool has_more { get; set; }
    }
    public class MessageData
    {
        public string id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }
        public long created_at { get; set; }
        public object assistant_id { get; set; }
        public string thread_id { get; set; }

        public object run_id { get; set; }

        public string role { get; set; }

        public List<Content> content { get; set; }

        public List<object> attachments { get; set; }

        public Metadata metadata { get; set; }
    }
    public class Content
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }
    }
    public class Metadata
    {
    }


    public class Text
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("annotations")]
        public List<object> Annotations { get; set; }
    }


    public class Choice
    {
        public int index { get; set; }

        public Message? message { get; set; }

        public object? logprobs { get; set; }
        public string? finish_reason { get; set; }
    }

    public class Message
    {
        public string? role { get; set; }
        public object? content { get; set; }
        public ToolCall[]? tool_calls { get; set; }
        public string? tool_call_id { get; set; }
        public string? name { get; set; }
    }

    public class ToolCall
    {
        public string? id { get; set; }

        public string? type { get; set; }

        public Function? function { get; set; }
    }

    public class Function
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public Parameters? parameters { get; set; }
        public string? arguments { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }

        public int completion_tokens { get; set; }

        public int total_tokens { get; set; }
    }
    public class Tool
    {
        public string? type { get; set; }
        public Function? function { get; set; }
    }
    public class ToolFunction
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public Parameters? parameters { get; set; }
    }
    public class Parameters
    {
        public string? type { get; set; }
        public Dictionary<string, object>? properties { get; set; }
        public List<string>? required { get; set; }
    }
}
