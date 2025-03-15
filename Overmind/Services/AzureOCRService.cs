using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Overmind.Services
{
    public class AzureOCRService
    {
        private static readonly string Endpoint = Environment.GetEnvironmentVariable("AZURE_ENDPOINT");
        private static readonly string Key = Environment.GetEnvironmentVariable("AZURE_KEY");
        private static readonly DocumentAnalysisClient Client = new(new Uri(Endpoint), new AzureKeyCredential(Key));

        public static async Task<string> ExtractTextAsync(string imagePath)
        {
            using FileStream stream = new(imagePath, FileMode.Open);
            var operation = await Client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);
            return string.Join("\n", operation.Value.Content);
        }
    }
}
