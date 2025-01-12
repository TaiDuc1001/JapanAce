using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

        public static ushort GetTotalWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            char[] delimiters = [' ', '\r', '\n', '\t', '.', ',', ';', ':', '!', '?'];

            string[] words = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return (ushort)words.Length;
        }


        public static bool IsJapanese(string input)
        {
            // Define Japanese character ranges
            char[] hiragana = Enumerable.Range(0x3041, 0x3096 - 0x3041 + 1).Select(c => (char)c).ToArray();
            char[] katakana = Enumerable.Range(0x30A1, 0x30FA - 0x30A1 + 1).Select(c => (char)c).ToArray();
            char[] kanji = Enumerable.Range(0x4E00, 0x9FFF - 0x4E00 + 1).Select(c => (char)c).ToArray();

            // Combine all Japanese characters
            var japaneseChars = hiragana.Concat(katakana).Concat(kanji).ToArray();

            // Check if the input contains only Japanese characters, digits, or common punctuation
            return input.All(c => japaneseChars.Contains(c) || char.IsWhiteSpace(c) || char.IsDigit(c) || ".,!?;:'\"()[]{}$%&*+-/".Contains(c));
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