using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Overmind.Services
{
    public class AzureOCRService
    { 
        private readonly DocumentAnalysisClient _client;

        public AzureOCRService(IConfiguration configuration)
        {
            string endpoint = configuration["AZURE_OCR_ENDPOINT"];
            string key = configuration["AZURE_OCR_KEY"];
            _client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        public async Task<string> ExtractTextAsync(string imagePath)
        {
            using FileStream stream = new(imagePath, FileMode.Open);
            var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);
            return string.Join("\n", operation.Value.Content);
        }
    }
}
