using System;
using System.Collections.Generic;
using System.Text;

namespace TextTranslationDemo
{
    public class UserDictionaryItem
    {
        public int id { get; set; }
        public string fromLang { get; set; }
        public string fromText { get; set; }
        public string toLang { get; set; }
        public string toText { get; set; }
        public override bool Equals(Object obj)
        {
            return obj is UserDictionaryItem && this == (UserDictionaryItem)obj;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(fromLang, fromText, toLang, toText).GetHashCode();
        }

        public static bool operator ==(UserDictionaryItem x, UserDictionaryItem y)
        {
            return x.fromLang == y.fromLang 
                && x.fromText == y.fromText 
                && x.toLang == y.toLang 
                && x.toText == y.toText;
        }
        public static bool operator !=(UserDictionaryItem x, UserDictionaryItem y)
        {
            return !(x == y);
        }
    }
}
