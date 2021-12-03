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
            FileTranslateFlow flow = new FileTranslateFlow("https://translate.classiii.info/api/v1");
            string[] files = { @"C:\mydocuments\morning.docx" };
            string[] langs = { "en" };
            int fieldId = 1;
            bool done = false;
            ClassiiiUser classiiiUser = new ClassiiiUser {
                AccessKey = UserInfo.CLASSIII_ACCESS_KEY,
                SecretKey = UserInfo.CLASSIII_SECRET_KEY,
                ContractId = UserInfo.FILE_CONTRACT_ID
            };
            done = await flow.ClassiiiFlowAsync(classiiiUser, files, langs, fieldId);
            Debug.Assert(done);
        }
    }
}
