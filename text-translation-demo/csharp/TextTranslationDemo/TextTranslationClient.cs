using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslationDemo
{
    class TextTranslationClient
    {
        private string baseUrl;
        public TextTranslationClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        public async Task<TextTranslationResult[]> TranslateTextBySyncModeAsync(
            ClassiiiUser classiiiUser,
            TextTranslationOption option,
            string[] text)
        {
            string url=baseUrl +"/translate";

            Dictionary<string, object> headers= HttpUtils.BuildHeaders(classiiiUser, url);

            Dictionary<string, object> body = BuildBody(option, text);

            var content = await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { taskId = "", translationResult = new TextTranslationResult[] { } } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("text translation failed");

            return serverResp.data.translationResult;
        }

        private static Dictionary<string, object> BuildBody(TextTranslationOption option, string[] text)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("fieldId", option.FieldId.ToString());
            body.Add("targetLang", option.TargetLang);
            body.Add("sourceLang", option.SourceLang);
            body.Add("text", text);
            if (option.AutoSplit != null)
                body.Add("autoSplit", option.AutoSplit);
            if (option.Type != null)
                body.Add("type", option.Type);
            if (option.RemoveFakeLineBreak != null)
                body.Add("removeFakeLineBreak", option.RemoveFakeLineBreak);
            return body;
        }



        public async Task<string> TranslateTextByAsyncModeAsync(
            ClassiiiUser classiiiUser,
            TextTranslationOption option,
            string[] text)
        {
            string url = baseUrl + "/translate/async";

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(classiiiUser, url);

            Dictionary<string, object> body = BuildBody(option, text);

            var content = await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { queueId = "" } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("text translation failed");

            return serverResp.data.queueId;
        }

        public async Task<TextTranslationResult[]> GetTranslationResultAsync(
            ClassiiiUser classiiiUser,            
            string queueId)
        {
            string url = baseUrl + "/translate/async/"+ queueId;

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(classiiiUser, url);
            
            var content = await HttpUtils.SendAsync(HttpMethod.Get, url, headers, null);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { taskId = "", translationResult = new TextTranslationResult[] { } } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("text translation failed");

            if (serverResp.data.taskId == null)
            {
                return new TextTranslationResult[] { };
            }

            return serverResp.data.translationResult;
        }
    }
}
