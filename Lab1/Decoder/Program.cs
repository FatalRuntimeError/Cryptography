using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Decoder
{
    class Program
    {
        static int BestKeysAmount = 15;
        static int i = 0;
        static char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();
        static Dictionary<char, double> monogramFreq;
        static Dictionary<string, double> bigramFreq;

        static Dictionary<char, double> actualMonoFreq;
        static Dictionary<string, double> actualBiFreq;

        static string Shuffle()
        {
            Random rand = new Random(Environment.TickCount + i++);
            StringBuilder shuffled = new StringBuilder(new string(alphabet));

            for (int i = 0; i < 33; i++)
            {
                int j = rand.Next(0, i);
                char temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            return shuffled.ToString();
        }

        static void ReadMonogramFreq()
        {
            monogramFreq = new Dictionary<char, double>();
            using (StreamReader stream = new StreamReader("frequencies/MonogramFreq.txt", Encoding.UTF8))
                for (int i = 0; i < 33; i++)
                {
                    string buffer = stream.ReadLine();
                    string[] tokens = buffer.Split(' ');
                    monogramFreq.Add(char.Parse(tokens[0]), double.Parse(tokens[1]));
                }
        }

        static void ReadBigramFreq()
        {
            bigramFreq = new Dictionary<string, double>();
            using (StreamReader stream = new StreamReader("frequencies/BigramFreq.txt", Encoding.UTF8))
                for (int i = 0; i < 33 * 33; i++)
                {
                    string buffer = stream.ReadLine();
                    string[] tokens = buffer.Split(' ');
                    bigramFreq.Add(tokens[0], double.Parse(tokens[1]));
                }
        }

        static void TextAnalysis(string text, string key)
        {
            double monoFreqBit = 1.0 / text.Length;
            double biFreqBit = 1.0 / (text.Length - 1);

            Dictionary<char, char> accordance = new Dictionary<char, char>();
            for (int i = 0; i < 33; i++)
                accordance.Add(key[i], alphabet[i]);

            foreach (var letter1 in alphabet)
            {
                actualMonoFreq[letter1] = 0.0;
                foreach (var letter2 in alphabet)
                    actualBiFreq[letter1.ToString() + letter2.ToString()] = 0.0;
            }

            for (int i = 0; i < text.Length - 1; i++)
            {
                char monogram = accordance[text[i]];
                string bigram = monogram.ToString();
                bigram += accordance[text[i + 1]];
                actualMonoFreq[monogram] += monoFreqBit;
                actualBiFreq[bigram] += biFreqBit;
            }

            actualMonoFreq[accordance[text[text.Length - 1]]] += monoFreqBit;

            actualMonoFreq = actualMonoFreq.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
            actualBiFreq = actualBiFreq.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        static double MeaningfulnessMetric()
        {
            double monogramSum = 0.0;
            double bigramSum = 0.0;

            foreach (var letter1 in alphabet)
            {
                monogramSum += Math.Abs(actualMonoFreq[letter1] - monogramFreq[letter1]);

                foreach (var letter2 in alphabet)
                {
                    string bigram = letter1.ToString() + letter2.ToString();
                    bigramSum += Math.Abs(actualBiFreq[bigram] - bigramFreq[bigram]);
                }
            }

            bigramSum *= 3;

            return bigramSum + monogramSum;
        }

        static string RecodeText(string text, string key)
        {
            StringBuilder recodedTextBuilder = new StringBuilder();

            Dictionary<char, char> accordance = new Dictionary<char, char>();
            for (int i = 0; i < 33; i++)
                accordance.Add(key[i], alphabet[i]);

            for (int i = 0; i < text.Length; i++)
                recodedTextBuilder.Append(accordance[text[i]]);

            return recodedTextBuilder.ToString();
        }

        static string formatText(string text)
        {
            StringBuilder formattedText = new StringBuilder();

            text = text.ToLower();
            foreach (var letter in text)
                if (letter >= 'а' && letter <= 'я' || letter == 'ё')
                    formattedText.Append(letter);

            return formattedText.ToString();
        }

        static Dictionary<string, double> GeneticAlgorithm(string text, Dictionary<string, double> keys)
        {
            Dictionary<string, double> checkedKeys = new Dictionary<string, double>();

            foreach (var key in keys)
            {
                TextAnalysis(text, key.Key);
                checkedKeys.Add(key.Key, MeaningfulnessMetric());
            }

            var sortedKeys = checkedKeys.OrderBy(x => x.Value).ToList();

            Dictionary<string, double> bestKeys = new Dictionary<string, double>();
            for (int i = 0; i < BestKeysAmount; i++)
                bestKeys.Add(sortedKeys[i].Key, sortedKeys[i].Value);

            return bestKeys;
        }

        static Dictionary<string, double> GenerateKeys(Dictionary<string, double> keys)
        {
            Dictionary<string, double> newKeys = new Dictionary<string, double>(keys);

            var tmp = keys.ToList();
            List<string> randomKeys = new List<string>();

            for (int i = 0; i < 15; i++)
                randomKeys.Add(tmp[i].Key);

            for (int i = 0; i < 35; i++)
            {
                string newKey = Shuffle();
                randomKeys.Add(newKey);
            }

            string firstKey = keys.First().Key;

            foreach (var key in randomKeys)
            {
                Random rand = new Random(Environment.TickCount + i++);

                int a = rand.Next(32);
                int b = rand.Next(a, 34);
                string parent1 = firstKey;
                string parent2 = key;

                char[] child1 = parent1.ToCharArray();
                char[] child2 = parent2.ToCharArray();

                Dictionary<char, char> accordance1 = new Dictionary<char, char>();
                Dictionary<char, char> accordance2 = new Dictionary<char, char>();

                for (int i = a; i < b; i++)
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                    accordance1.Add(parent1[i], parent2[i]);
                    accordance2.Add(parent2[i], parent1[i]);
                }

                for (int i = 0; i < a; i++)
                {
                    if (accordance1.ContainsKey(child1[i]))
                        child1[i] = accordance1[child1[i]];
                    else if (accordance2.ContainsKey(child1[i]))
                        child1[i] = accordance2[child1[i]];

                    if (accordance1.ContainsKey(child2[i]))
                        child2[i] = accordance1[child2[i]];
                    else if (accordance2.ContainsKey(child2[i]))
                        child2[i] = accordance2[child2[i]];
                }

                for (int i = b; i < 33; i++)
                {
                    if (accordance1.ContainsKey(child1[i]))
                        child1[i] = accordance1[child1[i]];
                    else if (accordance2.ContainsKey(child1[i]))
                        child1[i] = accordance2[child1[i]];

                    if (accordance1.ContainsKey(child2[i]))
                        child2[i] = accordance1[child2[i]];
                    else if (accordance2.ContainsKey(child2[i]))
                        child2[i] = accordance2[child2[i]];
                }

                bool isFull = true;
                for (int i = 0; i < 33 && isFull; i++)
                    isFull = child1.Contains(alphabet[i]);

                string child1str = new string(child1);
                string child2str = new string(child2);

                if (isFull)
                {
                    if (!newKeys.ContainsKey(child1str))
                        newKeys.Add(child1str, 0.0);

                    if (!newKeys.ContainsKey(child2str))
                        newKeys.Add(child2str, 0.0);
                }

            }

            foreach (var key in keys)
            {
                Random rand = new Random(Environment.TickCount + i++);

                int a = rand.Next(33);
                int b = rand.Next(33);

                StringBuilder newKeyBuilder = new StringBuilder(key.Key);
                char temp = newKeyBuilder[a];
                newKeyBuilder[a] = newKeyBuilder[b];
                newKeyBuilder[b] = temp;
                string newKey = newKeyBuilder.ToString();

                if (!newKeys.ContainsKey(newKey))
                    newKeys.Add(newKey, 0.0);
            }

            return newKeys;
        }

        static void Main(string[] args)
        {
            // Чтение статистики
            ReadMonogramFreq();
            ReadBigramFreq();

            // Инициализация статистики зашифрованного текста
            actualMonoFreq = new Dictionary<char, double>();
            actualBiFreq = new Dictionary<string, double>();

            for (int i = 0; i < 33; i++)
            {
                actualMonoFreq.Add(alphabet[i], 0);
                for (int j = 0; j < 33; j++)
                    actualBiFreq.Add(alphabet[i].ToString() + alphabet[j].ToString(), 0);
            }

            // Чтение зашифрованного текста
            string text;
            using (StreamReader stream = new StreamReader("Text.txt", Encoding.UTF8))
                text = formatText(stream.ReadToEnd());

            // Чтение исходных ключей
            Dictionary<string, double> keys = new Dictionary<string, double>();
            while (keys.Count < BestKeysAmount)
            {
                string newKey = Shuffle();

                if (!keys.ContainsKey(newKey))
                    keys.Add(newKey, 0.0);
            }

            int iterNum = 500;
            for (int i = 0; i < iterNum; i++)
            {
                keys = GenerateKeys(keys);
                keys = GeneticAlgorithm(text, keys);
            }

            string recodedText = RecodeText(text, keys.First().Key);

            using (StreamWriter stream = new StreamWriter("result.txt", false, Encoding.UTF8))
                stream.Write(recodedText);
        }
    }
}
