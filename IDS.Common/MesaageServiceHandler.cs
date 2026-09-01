using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
//using System.Text.Json.Serialization;

namespace IDS.Common
{
    public delegate IdsResult<E> MesaageServiceHandler<E>(MessageContent<E> messageContent);
    public class MessageContent<T> {
        [JsonProperty("type")]
       public MessageType Type { set; get; }
        [JsonProperty("headers")]
        public Dictionary<string, string>? Headers { set; get; }
        [JsonProperty("message")]
        public string? Message { set; get; }
        [JsonProperty("data")]
        public T ?Data;
    }
    [System.Text.Json.Serialization.JsonConverter(typeof(StringEnumConverter))]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageType {
        debug,
        fatch, 
        error,
        warn,
        info
    }

    public class IdsMessageHandler<T> {
        public static event MesaageServiceHandler<T> OnSendMessage;
        public static async Task SendMessage(MessageContent<T> messageContent) {
            if (OnSendMessage != null) {
             await  Task.Run(() =>
                {
                    OnSendMessage?.Invoke(messageContent);
                });
            }
        }
        public static async Task ErrorMessage(string mrssage) {
            MessageContent<T> messageContent = new MessageContent<T>
            {
                Type = MessageType.error,
                Message = mrssage
            };
            await SendMessage(messageContent);
        }
    }
}
