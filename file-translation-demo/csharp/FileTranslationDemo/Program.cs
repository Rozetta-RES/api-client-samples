using common;
using System;
using System.Diagnostics;

namespace FileTranslationDemo
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            UserInfo.init("config.json");
            FileTranslateFlow flow = new FileTranslateFlow("https://translate.rozetta-api.info/api/v1");
            string[] files = { @"C:\mydocuments\morning.docx" };
            string[] langs = { "en" };
            int fieldId = 1;
            bool done = false;
            RozettaApiUser rozettaApiUser = new RozettaApiUser {
                AccessKey = UserInfo.ROZETTA_API_ACCESS_KEY,
                SecretKey = UserInfo.ROZETTA_API_SECRET_KEY,
                ContractId = UserInfo.FILE_CONTRACT_ID
            };
            done = await flow.RozettaApiFlowAsync(rozettaApiUser, files, langs, fieldId);
            Debug.Assert(done);
        }
    }
}
