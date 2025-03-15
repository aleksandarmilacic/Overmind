
using Microsoft.Extensions.Configuration;
using Overmind.Models;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Overmind.Services
{
    public class OpenAIAssistantService
    {
        private readonly string _apiKey;
        private static readonly string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private static readonly string Model = "gpt-4o";

        public OpenAIAssistantService(IConfiguration configuration)
        {
            _apiKey = configuration["OPENAI_API_KEY"]
                      ?? throw new InvalidOperationException("Missing API Key in User Secrets.");
        }


        public async Task<string> AnalyzeGameScreenshotAsync(string base64Image, string gameName)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var payload = new
            {
                model = Model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = $"Today is {timestamp}. You are an expert game analyst specifically for '{gameName}'. " +
                                 "Analyze this screenshot carefully, including UI elements, text, icons, resources, technology options, and strategic information clearly visible on the screen. " +
                                 "Provide structured strategic advice strictly following this format:\n" +
                                 "- 📌 **Strategic Priority**:\n" +
                                 "- ⚔️ **Military Advice**:\n" +
                                 "- 🏗 **Economic Advice**:\n" +
                                 "- 🔬 **Technology Recommendations**:\n" +
                                 "- 🏛 **Diplomatic Strategy**:\n" +
                                 "- 🎭 **Miscellaneous Tips**:\n\n" +
                                 "Do NOT state you can't see the image. If text is clearly visible, analyze and use it for detailed strategic recommendations."
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = $"data:image/png;base64,{base64Image}"
                                }
                            }
                        }
                    }
                },
                max_tokens = 800,
                temperature = 0.5
            };

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await client.PostAsJsonAsync(ApiUrl, payload);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return openAiResponse?.Choices?[0]?.Message?.Content ?? "No response from OpenAI.";
            }
            catch (Exception ex)
            {
                return $"Error calling OpenAI API: {ex.Message}";
            }
        }

    }
}