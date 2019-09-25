using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Program
    {
        static string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        static string Shuffle()
        {
            Random rand = new Random(Environment.TickCount);
            StringBuilder shuffled = new StringBuilder(alphabet);

            for (int i = 0; i < 33; i++)
            {
                int j = rand.Next(0, i);
                char temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            return shuffled.ToString();
        }

        static void Main(string[] args)
        {
            using (StreamWriter fstream = new StreamWriter("keys.txt", true, Encoding.UTF8))
                fstream.WriteLine(Shuffle());
        }
    }
}
