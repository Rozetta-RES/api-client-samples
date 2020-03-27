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

            string baseUrl = "https://translate.classiii.info/api/v1";            

            ClassiiiUser classiiiUser = new ClassiiiUser { AccessKey = UserInfo.CLASSIII_ACCESS_KEY, SecretKey = UserInfo.CLASSIII_SECRET_KEY };
            
            // 音声認識を試す
            await TestSpeechToText(baseUrl, classiiiUser);
        }

        private static async System.Threading.Tasks.Task TestSpeechToText(string baseUrl, ClassiiiUser classiiiUser)
        {
            SpeechToTextClient textTranslationClient = new SpeechToTextClient(baseUrl);
            //  音声認識
            string result = await textTranslationClient.SpeechToTextAsync(classiiiUser, "ja", "../../../0001_near.wav");
            
            Debug.Assert(result== "そこに着いたらもう一度誰かに尋ねてください");
        }
    }
}
