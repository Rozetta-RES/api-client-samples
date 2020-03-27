using common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTranslationDemo
{
    public class FileTranslationOption
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Nonce { get; set; }
        public string Signature { get; set; }
        public int FieldId { get; set; }
        public string[] Langs { get; set; }
    }
}
