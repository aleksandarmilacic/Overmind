using Microsoft.Extensions.Configuration;
using Overmind.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Overmind
{
    class Program
    {
        static readonly string GameName = "Stellaris";
        static readonly int CaptureIntervalSeconds = 10; // Change this to adjust loop timing

        static async Task Main(string[] args)
        {
            // Load Configuration (User Secrets)
            IConfiguration config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            Console.Title = $"Overmind - {GameName} AI Assistant";
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Welcome to Overmind!");
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Monitoring '{GameName}' every {CaptureIntervalSeconds} seconds.");
            Console.WriteLine("Press [ESC] to stop.");

            var captureService = new GameWindowCaptureService(GameName);
            var aiService = new OpenAIAssistantService(config);

            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Capturing '{GameName}' window...");

                // Capture screenshot as base64
                string base64Screenshot = captureService.CaptureGameWindowAsBase64();
                if (base64Screenshot != null)
                {
                    File.WriteAllBytes("test_capture.png", Convert.FromBase64String(base64Screenshot));
                    Console.WriteLine("Saved image as test_capture.png. Check visually.");
                }
                if (base64Screenshot == null)
                {
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Game window not found. Retrying...");
                    await Task.Delay(CaptureIntervalSeconds * 1000);
                    continue;
                }

                // Send screenshot to OpenAI for analysis
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Analyzing game state...");
                string strategyAdvice = await aiService.AnalyzeGameScreenshotAsync(base64Screenshot, GameName);
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} AI Advice:\n{strategyAdvice}");

                // Speak AI response
                SpeechService.Speak(strategyAdvice);

                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Next capture in {CaptureIntervalSeconds} seconds...");
                await Task.Delay(CaptureIntervalSeconds * 1000);
            }

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Overmind stopped.");
        }
    }
}
