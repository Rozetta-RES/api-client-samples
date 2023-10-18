using common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TextToSpeechDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            UserInfo.init("config.json");

            string baseUrl = "https://translate.rozetta-api.io/api/v1";

            RozettaApiUser user = new RozettaApiUser { AccessKey = UserInfo.ROZETTA_API_ACCESS_KEY, SecretKey = UserInfo.ROZETTA_API_SECRET_KEY };

            // 音声合成を試す
            await TestTextToSpeech(baseUrl, user, "ja", "こんにちは", "hello.wav");
        }
        private static async Task TestTextToSpeech(string baseUrl,
            RozettaApiUser user, 
            string targetLang, 
            string text, 
            string saveFilePath)
        {
            TextToSpeechClient textTranslationClient = new TextToSpeechClient(baseUrl);
            //  音声合成
            bool result = await textTranslationClient.TextToSpeechtAsync(user, targetLang, text, saveFilePath);

            Debug.Assert(result);
        }
    }
}
