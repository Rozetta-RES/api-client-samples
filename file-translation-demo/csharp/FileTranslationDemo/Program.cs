using common;
using System;
using System.Diagnostics;

namespace FileTranslationDemo
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            UserInfo.init("config.json");
            FileTranslateFlow flow = new FileTranslateFlow("https://translate.classiii.info/prealpha/api/v1");
            //FileTranslateFlow flow = new FileTranslateFlow("http://localhost:5555/v1");
            T4ooUser t4ooUser = new T4ooUser { orgId = UserInfo.T4OO_ORG_ID, userId = UserInfo.T4OO_USER_ID, password = UserInfo.T4OO_PASSWORD };
            string[] files = { @"C:\mydocuments\morning.docx" };
            string[] langs = { "en" };
            int fieldId = 1;
            bool done = false;
            //done = await flow.T4ooFlowAsync(t4ooUser, files, langs, fieldId);
            //Debug.Assert(done);
            ClassiiiUser classiiiUser = new ClassiiiUser { AccessKey = UserInfo.CLASSIII_ACCESS_KEY, SecretKey = UserInfo.CLASSIII_SECRET_KEY };
            done = await flow.ClassiiiFlowAsync(classiiiUser, files, langs, fieldId);
            Debug.Assert(done);
        }
    }
}
