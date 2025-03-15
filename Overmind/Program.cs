using Overmind.Services;
using System;
using System.Drawing;
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
            Console.Title = $"Overmind - {GameName} AI Assistant";
            Console.WriteLine("🌌 Welcome to Overmind! 🌌");
            Console.WriteLine("Initializing AI systems...");
            Console.WriteLine($"🔍 Monitoring '{GameName}' window every {CaptureIntervalSeconds} seconds.");
            Console.WriteLine("Press [ESC] to stop.");

            var captureService = new GameWindowCaptureService(GameName);

            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Console.WriteLine($"⏳ Capturing '{GameName}' window...");

                // Capture screenshot
                Bitmap screenshot = captureService.CaptureGameWindow();
                if (screenshot == null)
                {
                    Console.WriteLine("❌ Game window not found. Retrying in 10 seconds...");
                    await Task.Delay(CaptureIntervalSeconds * 1000);
                    continue;
                }

                // Save screenshot
                string filePath = $"game_{DateTime.Now:HHmmss}.png";
                screenshot.Save(filePath);
                Console.WriteLine($"📸 Screenshot saved: {filePath}");

                // Extract text via OCR
                string extractedText = await AzureOCRService.ExtractTextAsync(filePath);
                Console.WriteLine($"🧠 Extracted Game Data:\n{extractedText}");

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    Console.WriteLine("⚠️ No text detected. Skipping AI analysis.");
                    await Task.Delay(CaptureIntervalSeconds * 1000);
                    continue;
                }

                // Get AI strategy advice
                Console.WriteLine("🤖 Analyzing game state...");
                string strategyAdvice = await OpenAIAssistantService.GetGameStrategyAdviceAsync(extractedText, GameName);
                Console.WriteLine($"💡 AI Advice:\n{strategyAdvice}");

                // Speak AI response
                SpeechService.Speak(strategyAdvice);

                Console.WriteLine($"🔄 Next capture in {CaptureIntervalSeconds} seconds...");
                await Task.Delay(CaptureIntervalSeconds * 1000);
            }

            Console.WriteLine("🛑 Overmind stopped.");
        }
    }
}
