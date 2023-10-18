using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToTextDemo
{
    public class SpeechToTextClient
    {
        private string baseUrl;
        public SpeechToTextClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        public async Task<string> SpeechToTextAsync(
            RozettaApiUser user,
            string sourceLang,
            string filePath)
        {
            string url = baseUrl + "/translate/stt";

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(user, url);

            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("sourceLang", sourceLang);
            body.Add("audioFile", Path.GetFileName(filePath));

            Dictionary<string, string> fileDict = new Dictionary<string, string>();
            fileDict.Add("audioFile", filePath);

            var content = await HttpUtils.PostFileAsync(url, headers, body, fileDict);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data =new { result = "" } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("speech to text failed");

            return serverResp.data.result;
        }        
    }
}
