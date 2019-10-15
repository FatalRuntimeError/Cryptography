using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BigramAnalizer
{
    class Program
    {
        static char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

        static string FormatText(string text)
        {
            StringBuilder formattedTextBuilder = new StringBuilder();
            text = text.ToLower();
            foreach (var letter in text)
                if (letter >= 'а' && letter <= 'я' || letter == 'ё')
                    formattedTextBuilder.Append(letter);

            return formattedTextBuilder.ToString();
        }

        static Dictionary<char, double> CreateMonogramFreq(string formattedText)
        {
            Dictionary<char, double> monogramFreq = new Dictionary<char, double>();

            double freqBit = 1.0 / formattedText.Length;

            foreach (var letter in alphabet)
                monogramFreq.Add(letter, 0);

            foreach (var letter in formattedText)
                monogramFreq[letter] += freqBit;

            return monogramFreq;
        }

        static Dictionary<string, double> CreateBigramFreq(string formattedText)
        {
            Dictionary<string, double> bigramFreq = new Dictionary<string, double>();

            double freqBit = 1.0 / (formattedText.Length - 1);

            foreach (var letter1 in alphabet)
                foreach (var letter2 in alphabet)
                    bigramFreq.Add(letter1.ToString() + letter2.ToString(), 0);

            for (int i = 0; i < formattedText.Length - 1; i++)
            {
                string bigram = formattedText[i].ToString() + formattedText[i + 1].ToString();
                bigramFreq[bigram] += freqBit;
            }

            return bigramFreq;
        }

        static void Main(string[] args)
        {
            string text;
            using (StreamReader stream = new StreamReader(args[0], Encoding.UTF8))
                text = stream.ReadToEnd();

            string formattedText = FormatText(text);

            // Получение частоты биграмм и монограмм
            Dictionary<char, double> monogramFreq = CreateMonogramFreq(formattedText);
            Dictionary<string, double> bigramFreq = CreateBigramFreq(formattedText);

            // Вывод частотности монограмм
            using (StreamWriter stream = new StreamWriter(args[1], false, Encoding.UTF8))
                foreach (var pair in monogramFreq.OrderBy(pair => -pair.Value))
                    stream.WriteLine("{0} {1}", pair.Key, pair.Value);

            // Вывод частотности биграмм
            using (StreamWriter stream = new StreamWriter(args[2], false, Encoding.UTF8))
                foreach (var pair in bigramFreq.OrderBy(pair => -pair.Value))
                    stream.WriteLine("{0} {1}", pair.Key, pair.Value);
        }
    }
}
