﻿using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslationDemo
{    
    public class UserDictionaryClient
    {
        private string baseUrl;

        private string jwtTokenRequestUrl;

        private int defautDuration = 5 * 60;
        public UserDictionaryClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.jwtTokenRequestUrl = this.baseUrl + "/token";
        }
        public async Task<UserDictionaryItem[]> GetUserDictionaryAsync(
            RozettaApiUser rozettaApiUser)
        {
            string url = baseUrl + "/dictionary";

            string jwtToken = await HttpUtils.GenerateJwtDataAsync(
                rozettaApiUser.AccessKey,
                rozettaApiUser.SecretKey,
                this.defautDuration,
                this.jwtTokenRequestUrl);

            Dictionary<string, object> headers = HttpUtils.BuildJwtHeaders(jwtToken);

            var content = await HttpUtils.SendAsync(HttpMethod.Get, url, headers, null);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "", data = new { entries = new UserDictionaryItem[] { } } };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("get user dictionary failed");

            return serverResp.data.entries;
        }
        public async Task<bool> AddUserDictionaryItemAsync(
            RozettaApiUser rozettaApiUser, UserDictionaryItem item)
        {
            string url = baseUrl + "/dictionary";

            string jwtToken = await HttpUtils.GenerateJwtDataAsync(
                rozettaApiUser.AccessKey,
                rozettaApiUser.SecretKey,
                this.defautDuration,
                this.jwtTokenRequestUrl);

            Dictionary<string, object> headers = HttpUtils.BuildJwtHeaders(jwtToken);

            Dictionary<string, object> body = new Dictionary<string, object>() {
                { "fromLang", item.fromLang },
                { "fromText", item.fromText },
                { "toLang", item.toLang },
                { "toText", item.toText }
            };            

            var content = await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "" };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("add user dictionary item failed");

            return true;
        }

        public async Task<bool> DeleteUserDictionaryItemAsync(
            RozettaApiUser rozettaApiUser, int id)
        {
            string url = baseUrl + "/dictionary/"+ id;

            string jwtToken = await HttpUtils.GenerateJwtDataAsync(
                rozettaApiUser.AccessKey,
                rozettaApiUser.SecretKey,
                this.defautDuration,
                this.jwtTokenRequestUrl);

            Dictionary<string, object> headers = HttpUtils.BuildJwtHeaders(jwtToken);

            var content = await HttpUtils.SendAsync(HttpMethod.Delete, url, headers, null);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "" };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("delete user dictionary item failed");

            return true;
        }

        public async Task<bool> UpdateUserDictionaryItemAsync(
            RozettaApiUser rozettaApiUser, int id, UserDictionaryItem item)
        {
            string url = baseUrl + "/dictionary/" + id;

            string jwtToken = await HttpUtils.GenerateJwtDataAsync(
                rozettaApiUser.AccessKey,
                rozettaApiUser.SecretKey,
                this.defautDuration,
                this.jwtTokenRequestUrl);

            Dictionary<string, object> headers = HttpUtils.BuildJwtHeaders(jwtToken);

            Dictionary<string, object> body = new Dictionary<string, object>() {
                { "fromLang", item.fromLang },
                { "fromText", item.fromText },
                { "toLang", item.toLang },
                { "toText", item.toText }
            };

            var content = await HttpUtils.SendAsync(HttpMethod.Put, url, headers, body);

            var byteArray = await content.ReadAsByteArrayAsync();

            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var definition = new { status = "" };

            var serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition);

            if (serverResp.status != ResponseStatus.Success)
                throw new Exception("update user dictionary item failed");

            return true;
        }
    }
}
