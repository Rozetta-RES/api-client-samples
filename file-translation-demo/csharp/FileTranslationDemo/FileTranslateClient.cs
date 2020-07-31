﻿using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileTranslationDemo
{
    public class FileTranslateClient
    {        
        private string baseUrl;
        public FileTranslateClient(string baseUrl) 
        {
            this.baseUrl = baseUrl;
        }
        public async Task<string> GetAuthCodeAsync(T4ooUser t4ooUser)
        {
            Dictionary<string, object> t4ooUserDict = new Dictionary<string, object>{
                    { "orgId", t4ooUser.orgId },
                    { "userId", t4ooUser.userId }
            };
            var content = await HttpUtils.SendAsync(HttpMethod.Post, baseUrl + "/auth-code", null, t4ooUserDict);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { id = "" } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                return null;

            return serverResp.data.id;
        }
        public async Task<ClassiiiUser> AuthenticateAsync(T4ooUser t4ooUser, string authCode)
        {
            string encryptedPassword = new RC4(authCode).Enc(t4ooUser.password);

            Dictionary<string, object> t4ooUserDict = new Dictionary<string, object>{
                    { "orgId", t4ooUser.orgId },
                    { "userId", t4ooUser.userId },
                    { "password", encryptedPassword }
            };
            var content = await HttpUtils.SendAsync(HttpMethod.Post, baseUrl + "/authenticate", null, t4ooUserDict);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { accessKey = "", secretKey = "" } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                return null;

            return new ClassiiiUser { AccessKey = serverResp.data.accessKey, SecretKey = serverResp.data.secretKey };
        }

        public async Task<string> TranslateAsync(string[] files, string contractId, FileTranslationOption option)
        {
            string url = baseUrl + "/file-translate";

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(
                new ClassiiiUser { AccessKey= option.AccessKey, SecretKey=option.SecretKey}, url);

            Dictionary<string, string> body = new Dictionary<string, string>{
                    { "fieldId", option.FieldId.ToString() },
                    { "targetLangs", JsonConvert.SerializeObject(option.Langs) }
            };
            if (!string.IsNullOrEmpty(contractId)){
                body.Add("contractId", contractId);
            }

            Dictionary<string, string> fileDict = new Dictionary<string, string>();
            foreach(string file in files)
            {
                fileDict.Add("files", file);
            }   

            var content = await HttpUtils.PostFileAsync(url, headers, body, fileDict);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { translateId = ""} };            

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status!=ResponseStatus.Success)
                throw new Exception("upload file failed");

            return serverResp.data.translateId;

        }

        public Translate[] GetAllHistories(ClassiiiUser classiiiUser)
        {
            Translate[] ret = { };
            return ret;
        }
        public async Task<Translate> GetOneHistoryAsync(ClassiiiUser classiiiUser, string translateId)
        {
            string url = baseUrl + "/translate-result/" + translateId;

            Dictionary<string, object> headers = HttpUtils.BuildHeaders(classiiiUser, url);

            var content = await HttpUtils.SendAsync(HttpMethod.Get, url, headers, null);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new Translate { } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                return null;

            return serverResp.data;
        }

        public async Task<bool> DownloadZipAsync(ClassiiiUser classiiiUser,string ids, string savePath)
        {            
            string url = baseUrl + "/downloads?ids=" + HttpUtility.UrlEncode(ids);

            string apiPath = HttpUtility.UrlDecode(new Uri(url).PathAndQuery);
            string nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string signature = Utils.GetSignature(nonce, apiPath, classiiiUser.SecretKey);

            Dictionary<string, object> headers=new Dictionary<string, object>{
                    { "accessKey", classiiiUser.AccessKey },
                    { "nonce", nonce },
                    { "signature", signature }
            };            

            var content = await HttpUtils.SendAsync(HttpMethod.Get, url, headers, null);

            MemoryStream stream = new MemoryStream();

            await content.CopyToAsync(stream);

            Utils.UnzipStream(stream, savePath);

            return true;
        }        
    }
}
