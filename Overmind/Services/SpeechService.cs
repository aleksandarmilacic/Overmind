using System.Speech.Synthesis;

namespace Overmind.Services
{
    public class SpeechService
    {
        public static void Speak(string message)
        {
            using SpeechSynthesizer synth = new();
            synth.SelectVoice("Microsoft Zira Desktop");
            synth.Speak(message);
        }
    }
}
