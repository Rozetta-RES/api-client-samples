namespace FileTranslationDemo
{
    internal class TranslationOption
    {
        public string[] Langs { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Nonce { get; set; }
        public int FieldId { get; set; }
    }
}