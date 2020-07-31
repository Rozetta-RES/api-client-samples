using common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FileTranslationDemo
{
    public class FileTranslateFlow
    {        
        private FileTranslateClient client;
        private System.Timers.Timer aTimer = null;
        private ClassiiiUser classiiiUser;
        private string translateId;
        private bool done = false;
        private const int Interval = 10000;
        public FileTranslateFlow(string baseUrl)
        {            
            client = new FileTranslateClient(baseUrl);
        }
        public async Task<bool> T4ooFlowAsync(T4ooUser t4ooUser, string[] files, string[] langs, int fieldId)
        {

            string authCode = await client.GetAuthCodeAsync(t4ooUser);

            ClassiiiUser classiiiUser = await client.AuthenticateAsync(t4ooUser, authCode);

            return await ClassiiiFlowAsync(classiiiUser, files, langs, fieldId);
        }
        public async Task<bool> ClassiiiFlowAsync(ClassiiiUser classiiiUser, string[] files, string[] langs, int fieldId)
        {
            this.classiiiUser = classiiiUser;

            long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            FileTranslationOption option = new FileTranslationOption
            {
                Langs = langs,
                AccessKey = classiiiUser.AccessKey,
                SecretKey = classiiiUser.SecretKey,
                Nonce = timeStamp.ToString(),
                FieldId = fieldId
            };

            translateId = await client.TranslateAsync(files, classiiiUser.ContractId, option);

            if (aTimer != null)
                StopTimer(aTimer);

            aTimer = new System.Timers.Timer();

            aTimer.Interval = Interval;

            aTimer.Elapsed += OnTimedEvent;

            aTimer.AutoReset = true;

            aTimer.Enabled = true;

            while (!done)
            {
                Thread.Sleep(1000);
            }

            return true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // waiting for all translation done
            Task<Translate> task = client.GetOneHistoryAsync(classiiiUser, translateId);

            task.Wait(Interval);

            Translate translate = task.Result;

            if (translate==null)
                return;

            foreach (TranslateItem item in translate.items)
            {
                if (!item.done)
                {
                    return;
                }
            }
            List<string> ids = new List<string>();

            foreach (TranslateItem item in translate.items)
            {
                ids.Add(item.translateItemId);
            }

            string strIds = JsonConvert.SerializeObject(ids);

            Task<bool> downloadTask = client.DownloadZipAsync(classiiiUser, strIds, @"C:\mydocuments");

            downloadTask.Wait(Interval);

            done = downloadTask.Result;

            if(done)
                StopTimer(aTimer);
        }
        private void StopTimer(System.Timers.Timer timer)
        {
            aTimer.Enabled = false;
            aTimer.Stop();
            aTimer.Dispose();
        }
    }
}
