using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

            for (int i = 0; i < 33; i++)
            {
                int j = rand.Next(0, i);
                char temp = alphabet[i];
                alphabet[i] = alphabet[j];
                alphabet[j] = temp;
            }

            Console.WriteLine("Введите имя файла для записи ключа:");
            string filename = Console.ReadLine();

            using (StreamWriter fstream = new StreamWriter(filename, false, Encoding.UTF8))
                fstream.WriteLine(alphabet);
        }
    }
}
