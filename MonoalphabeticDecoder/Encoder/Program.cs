using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Encoder
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

            // Чтение текста
            string text;
            using (StreamReader stream = new StreamReader(args[0], Encoding.UTF8))
                text = stream.ReadToEnd();

            // Форматирование текста
            StringBuilder formattedText = new StringBuilder();

            text = text.ToLower();
            foreach (var letter in text)
                if (letter >= 'а' && letter <= 'я' || letter == 'ё')
                    formattedText.Append(letter);

            // Чтение ключа
            char[] key = new char[33];
            using (StreamReader stream = new StreamReader(args[1], Encoding.UTF8))
                key = stream.ReadToEnd().ToCharArray();

            // Создание словаря для соответствий
            Dictionary<char, char> accordance = new Dictionary<char, char>();
            for (int i = 0; i < 33; i++)
                accordance.Add(alphabet[i], key[i]);

            // Кодирование текста
            StringBuilder encodedText = new StringBuilder();
             foreach (var letter in formattedText.ToString())
                encodedText.Append(accordance[letter]);

            // Запись закодированного текста
            using (StreamWriter stream = new StreamWriter(args[2], false, Encoding.UTF8))
                stream.WriteLine(encodedText.ToString());

        }
    }
}
