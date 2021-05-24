using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    public class HttpUtils
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<HttpContent> SendAsync(HttpMethod method, string url, Dictionary<string, object> headers, Dictionary<string, object> bodyDict)
        {
            var request = new HttpRequestMessage(method, url);
            if (headers!=null && headers.Count>0) { 
                foreach (KeyValuePair<string, object> header in headers)
                {                
                    request.Headers.Add(header.Key, header.Value.ToString());
                }
            }
            if (bodyDict != null && bodyDict.Count > 0)
            {
                var jsonStr = JsonConvert.SerializeObject(bodyDict, Formatting.Indented);
                var stringContent = new StringContent(jsonStr,Encoding.UTF8, "application/json");
                request.Content = stringContent;
            }


            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception(string.Format("Http failed {0}", response.StatusCode));

            return response.Content;            
        }

        public static async Task<HttpContent> PostFileAsync(
            string url, Dictionary<string, object> headers, Dictionary<string, string> bodyDict, Dictionary<string, string> files)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (headers != null && headers.Count > 0)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value.ToString());
                }
            }
            var content = new MultipartFormDataContent();

            if (files != null && files.Count > 0)
            {
                foreach (KeyValuePair<string, string> filePair in files)
                {
                    var fileNameOnly = Path.GetFileName(filePair.Value);
                    var fileContent = new StreamContent(File.OpenRead(filePair.Value));
                    string fileNameHeader = string.Format("form-data; name=\"{0}\" ;filename=\"{1}\"", filePair.Key, fileNameOnly);

                    fileContent.Headers.Add("Content-Disposition",
                        new string(Encoding.UTF8.GetBytes(fileNameHeader).Select(b => (char)b).ToArray()));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Add(fileContent);
                }
            }

            if (bodyDict != null && bodyDict.Count > 0)
            {

                
                foreach(KeyValuePair<string, string> pair in bodyDict)
                {
                    var stringContent = new StringContent(String.Format("{0}", pair.Value));
                    stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = pair.Key,
                    };
                    content.Add(stringContent, pair.Key);
                }
            }
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception(string.Format("post file failed {0}", response.StatusCode));

            return response.Content;
        }
        public static Dictionary<string, object> BuildHeaders(ClassiiiUser classiiiUser, string url)
        {
            string apiPath = new Uri(url).PathAndQuery;
            string nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string signature = Utils.GetSignature(nonce, apiPath, classiiiUser.SecretKey);

            return new Dictionary<string, object>{
                    { "accessKey", classiiiUser.AccessKey },
                    { "nonce", nonce },
                    { "signature", signature },
            };
        }

        public static Dictionary<string, object> BuildJwtHeaders(string jwtToken)
        {
            return new Dictionary<string, object>{
                    { "Authorization", $"Bearer {jwtToken}" },
            };
        }

        public static async Task<string> GenerateJwtDataAsync(string accessKey, string secretKey, int secondsOfDuration, string url)
        {
            var body = new Dictionary<string, object>
            {
                { "accessKey", accessKey },
                { "secretKey", secretKey },
                { "duration", secondsOfDuration }
            };
            var content = await SendAsync(HttpMethod.Post, url, null, body);
            var byteArray = await content.ReadAsByteArrayAsync();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { encodedJWT = "" } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != "success")
                throw new Exception("get user dictionary failed");

            return serverResp.data.encodedJWT;
        }
    }
}
