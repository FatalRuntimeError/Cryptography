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
            string keyFile, txtFile, resultFile;
            char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

            // Ввод имен файлов
            Console.WriteLine("Введите название файла с ключом: ");
            keyFile = Console.ReadLine();

            Console.WriteLine("Введите название файла с текстом: ");
            txtFile = Console.ReadLine();

            Console.WriteLine("Введите название файла для записи результата: ");
            resultFile = Console.ReadLine();

            // Чтение ключа
            char[] key = new char[33];
            using (StreamReader stream = new StreamReader(keyFile, Encoding.UTF8))
                key = stream.ReadToEnd().ToCharArray();

            // Создание словаря для соответствий
            Dictionary<char, char> accordance = new Dictionary<char, char>();
            for (int i = 0; i < 33; i++)
                accordance.Add(alphabet[i], key[i]);

            // Чтение текста
            string text;
            using (StreamReader stream = new StreamReader(txtFile, Encoding.UTF8))
                text = stream.ReadToEnd();

            // Форматирование текста
            StringBuilder formattedText = new StringBuilder();

            text = text.ToLower();
            foreach (var letter in text)
                if (char.IsLetter(letter))
                    formattedText.Append(letter);

            // Кодирование текста
            StringBuilder encodedText = new StringBuilder();
             foreach (var letter in formattedText.ToString())
                encodedText.Append(accordance[letter]);

            // Запись закодированного текста
            using (StreamWriter stream = new StreamWriter(resultFile, false, Encoding.UTF8))
                stream.WriteLine(encodedText.ToString());

        }
    }
}
