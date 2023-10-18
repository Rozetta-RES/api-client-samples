using common;
using System;
using System.Diagnostics;

namespace SpeechToTextDemo
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            UserInfo.init("config.json");

            string baseUrl = "https://translate.rozetta-api.io/api/v1";

            RozettaApiUser user = new RozettaApiUser { AccessKey = UserInfo.ROZETTA_API_ACCESS_KEY, SecretKey = UserInfo.ROZETTA_API_SECRET_KEY };
            
            // 音声認識を試す
            await TestSpeechToText(baseUrl, user);
        }

        private static async System.Threading.Tasks.Task TestSpeechToText(string baseUrl, RozettaApiUser user)
        {
            SpeechToTextClient textTranslationClient = new SpeechToTextClient(baseUrl);
            //  音声認識
            string result = await textTranslationClient.SpeechToTextAsync(user, "ja", "../../../0001_near.wav");
            
            Debug.Assert(result== "そこに着いたらもう一度誰かに尋ねてください");
        }
    }
}
