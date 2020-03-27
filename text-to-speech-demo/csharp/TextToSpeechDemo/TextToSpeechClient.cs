using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechDemo
{
    public class TextToSpeechClient
    {
        private string baseUrl;
        public TextToSpeechClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        public async Task<bool> TextToSpeechtAsync(
            ClassiiiUser classiiiUser,
            string targetLang,
            string text,
            string saveFilePath)
        {
            string url = baseUrl + "/translate/tts";

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(classiiiUser, url);

            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("targetLang", targetLang);
            body.Add("text", text);

            var content = await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body);

            var byteArray = await content.ReadAsByteArrayAsync();
            
            await File.WriteAllBytesAsync(saveFilePath, byteArray);            

            return true;
        }
    }
}
