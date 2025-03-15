using System.Text.Json.Serialization;

namespace Overmind.Models
{
    public class OpenAiResponse
    {
        [JsonPropertyName("choices")]
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; }
    }

    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
