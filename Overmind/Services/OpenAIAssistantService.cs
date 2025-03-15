
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
        private static readonly string Model = "gpt-4";

        public OpenAIAssistantService(IConfiguration configuration)
        {
            _apiKey = configuration["OPENAI_API_KEY"]
                      ?? throw new InvalidOperationException("Missing API Key in User Secrets.");
        }


        public async Task<string> AnalyzeGameScreenshotAsync(string base64Image, string gameName)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            string prompt = $"Today is {timestamp}. You are an AI strategy advisor for the game '{gameName}'. " +
                                  "Analyze the current game state extracted from the UI and provide expert advice on what actions the player should take next.\n\n" +
                                  "Extracted Game Data is on the image. Analyze the whole image in depth.\n" +
                                  "**Your response format must be structured as follows:**\n" +
                                  "- 📌 **Strategic Priority**: (Main focus area)\n" +
                                  "- ⚔️ **Military Advice**: (If applicable, fleet positioning, unit builds, etc.)\n" +
                                  "- 🏗 **Economic Advice**: (Resource optimization, building strategies, etc.)\n" +
                                  "- 🔬 **Technology Recommendations**: (Optimal research priorities)\n" +
                                  "- 🏛 **Diplomatic Strategy**: (Rivalries, alliances, trade deals, etc.)\n" +
                                  "- 🎭 **Miscellaneous Tips**: (Anything else that could be useful)\n\n" +
                                  "Ensure the advice is **specific to the extracted game state** and avoids generic suggestions.";


            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new
            {
                model = Model,
                messages = new object[]
                {
                    new { role = "system", content = "You are a highly intelligent AI specializing in game strategy." },
                    new { role = "user", content = prompt },
                    new { type = "image_url", image_url = $"data:image/png;base64,{base64Image}" }
                },
                max_tokens = 800,
                temperature = 0.8
            };

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(ApiUrl, payload);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<OpenAiResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                string rawResponse = responseJson?.Choices?[0]?.Message?.Content ?? "No response from ChatGPT.";

                return rawResponse;
            }
            catch (Exception ex)
            {
                return $"Error calling ChatGPT API: {ex.Message}";
            }
        }
    }
}