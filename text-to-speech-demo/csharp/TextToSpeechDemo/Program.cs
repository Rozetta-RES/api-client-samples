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

            string baseUrl = "https://translate.classiii.info/api/v1";

            ClassiiiUser classiiiUser = new ClassiiiUser { AccessKey = UserInfo.CLASSIII_ACCESS_KEY, SecretKey = UserInfo.CLASSIII_SECRET_KEY };

            // 音声合成を試す
            await TestTextToSpeech(baseUrl, classiiiUser, "ja", "こんにちは", "hello.wav");
        }
        private static async Task TestTextToSpeech(string baseUrl, 
            ClassiiiUser classiiiUser, 
            string targetLang, 
            string text, 
            string saveFilePath)
        {
            TextToSpeechClient textTranslationClient = new TextToSpeechClient(baseUrl);
            //  音声合成
            bool result = await textTranslationClient.TextToSpeechtAsync(classiiiUser, targetLang, text, saveFilePath);

            Debug.Assert(result);
        }
    }
}
