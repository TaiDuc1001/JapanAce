using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
// using NMeCab;

namespace Helper
{
    public static class GeneralHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        // public static ushort GetTotalWords(string input)
        // {
        //     if (string.IsNullOrWhiteSpace(input))
        //     {
        //         return 0;
        //     }

        //     // Initialize MeCab
        //     var parameter = new MeCabParam();
        //     var tagger = MeCabTagger.Create(parameter);

        //     // Tokenize the input
        //     var nodes = tagger.ParseToNodes(input);

        //     // Count words (excluding punctuation and whitespace)
        //     int wordCount = nodes.Count(node =>
        //         !string.IsNullOrEmpty(node.Surface) &&
        //         !char.IsPunctuation(node.Surface[0]) &&
        //         !char.IsWhiteSpace(node.Surface[0]));

        //     return (ushort)wordCount;
        // }

        public static bool IsJapanese(string input)
        {
            // Hiragana range: 0x3040 to 0x309F
            char[] hiragana = Enumerable.Range(0x3040, 0x309F - 0x3040 + 1).Select(c => (char)c).ToArray();

            // Katakana range: 0x30A0 to 0x30FF
            char[] katakana = Enumerable.Range(0x30A0, 0x30FF - 0x30A0 + 1).Select(c => (char)c).ToArray();

            // Kanji range: 0x4E00 to 0x9FFF (common Kanji) and 0x3400 to 0x4DBF (rare Kanji)
            char[] kanji = Enumerable.Range(0x4E00, 0x9FFF - 0x4E00 + 1)
                                .Concat(Enumerable.Range(0x3400, 0x4DBF - 0x3400 + 1))
                                .Select(c => (char)c).ToArray();

            // Japanese punctuation marks
            string japanesePunctuation = "。、！？；：・「」『』（）【】｛｝〈〉《》〔〕";

            // Combine all allowed characters
            var japaneseChars = hiragana.Concat(katakana).Concat(kanji).Concat(japanesePunctuation).ToArray();

            // Check if all characters in the input are allowed
            return input.All(c => japaneseChars.Contains(c) || char.IsWhiteSpace(c) || char.IsDigit(c));
        }

        public static List<int> GenerateRandomNumbers(int x, int y)
        {
            var rand = new Random();
            var result = new List<int>();

            for (int i = 0; i < x; i++)
            {
                result.Add(1);
            }

            int remaining = y - x;

            while (remaining > 0)
            {
                int index = rand.Next(0, x);
                result[index]++;
                remaining--;
            }

            return result;
        }
    }
}