using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Decoder
{
    class Program
    {
        static char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

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

        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0], Encoding.UTF8);
            text = formatText(text);

            GeneticAlgorithm algorithm = new GeneticAlgorithm(text, 15, int.Parse(args[1]));

            string recodedText = RecodeText(text, algorithm.BestKey);
            File.WriteAllText(args[2], recodedText, Encoding.UTF8);
        }
    }

    public class GeneticAlgorithm
    {
        static int i = 0;
        static char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();
        public int BestKeysAmount { get; }
        public string Text { get; }
        public Statistics Stat { get; }
        public Dictionary<string, double> AllKeys { get; }
        public Dictionary<string, double> BestKeys { get; }
        public string BestKey { get; }

        public GeneticAlgorithm(string text, int bestKeysAmount, int iterNum)
        {
            BestKeysAmount = bestKeysAmount;
            Text = text;

            AllKeys = new Dictionary<string, double>();
            BestKeys = new Dictionary<string, double>();

            while (AllKeys.Count < bestKeysAmount)
            {
                string key = Shuffle();
                if (!AllKeys.ContainsKey(key))
                    AllKeys.Add(key, 0.0);
            }

            Stat = new Statistics("frequencies/MonogramFreq.txt", "frequencies/BigramFreq.txt");

            for (int i = 0; i < iterNum; i++)
            {
                GenerateNewKeys();
                Selection();
                Console.WriteLine(BestKeys.First().Value);
            }

            BestKey = BestKeys.First().Key;
        }

        private string Shuffle()
        {
            Random rand = new Random();
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

        private void Selection()
        {
            BestKeys.Clear();

            foreach (var key in AllKeys.Keys.ToArray())
            {
                Stat.TextAnalysis(Text, key);
                AllKeys[key] = Stat.MeaningfullnessMetric();
            }

            var sortedKeys = (from key in AllKeys
                              orderby key.Value
                              select key).ToList();

            for (int i = 0; i < BestKeysAmount; i++)
                BestKeys.Add(sortedKeys[i].Key, sortedKeys[i].Value);

            AllKeys.Clear();
        }

        private void Mutation(string key)
        {
            Random rand = new Random(i++);
            int a = rand.Next(32);
            int b = rand.Next(33);

            StringBuilder newKeyBuilder = new StringBuilder(key);
            char temp = newKeyBuilder[a];
            newKeyBuilder[a] = newKeyBuilder[b];
            newKeyBuilder[b] = temp;
            string newKey = newKeyBuilder.ToString();

            if (!AllKeys.ContainsKey(newKey))
                AllKeys.Add(newKey, 0.0);
        }

        private void Crossover(string parent1, string parent2)
        {
            Random rand = new Random(i++);

            int a = rand.Next(32);
            int b = rand.Next(a, 34);

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

            string child1str = new string(child1);
            string child2str = new string(child2);

            if (CheckKey(child1str))
            {
                if (!AllKeys.ContainsKey(child1str))
                    AllKeys.Add(child1str, 0.0);

                if (!AllKeys.ContainsKey(child2str))
                    AllKeys.Add(child2str, 0.0);
            }
        }

        private bool CheckKey(string key)
        {
            bool isFull = true;
            for (int i = 0; i < 33 && isFull; i++)
                isFull = key.Contains(alphabet[i]);

            return isFull;
        }

        private void GenerateNewKeys()
        {
            foreach (var key in BestKeys.Keys)
                Crossover(key, BestKeys.First().Key);

            foreach (var key in BestKeys.Keys)
                for (int i = 0; i < 5; i++)
                    Mutation(key);
        }
    }

    public class Statistics
    {
        static char[] alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

        Dictionary<char, double> MonogramFreq { get; }
        Dictionary<string, double> BigramFreq { get; }

        Dictionary<char, double> ActualMonoFreq { get; set; }
        Dictionary<string, double> ActualBiFreq { get; set; }

        public Statistics(string monoFilename, string biFilename)
        {
            // Инициализация эталонной частотности
            MonogramFreq = new Dictionary<char, double>();
            BigramFreq = new Dictionary<string, double>();
            ReadFromFile(monoFilename, biFilename);

            // Инициализация реальной статистики зашифрованного текста
            ActualMonoFreq = new Dictionary<char, double>();
            ActualBiFreq = new Dictionary<string, double>();

            foreach (var pair in MonogramFreq)
                ActualMonoFreq.Add(pair.Key, pair.Value);

            foreach (var pair in BigramFreq)
                ActualBiFreq.Add(pair.Key, pair.Value);
        }

        private void ReadFromFile(string monoFilename, string biFilename)
        {
            string buffer = File.ReadAllText(monoFilename, Encoding.UTF8);
            string[] lines = buffer.Split('\n');
            for (int i = 0; i < 33; i++)
            {
                string[] tokens = lines[i].Split(' ');
                MonogramFreq.Add(char.Parse(tokens[0]), double.Parse(tokens[1]));
            }

            buffer = File.ReadAllText(biFilename, Encoding.UTF8);
            lines = buffer.Split('\n');
            for (int i = 0; i < 33 * 33; i++)
            {
                string[] tokens = lines[i].Split(' ');
                BigramFreq.Add(tokens[0], double.Parse(tokens[1]));
            }
        }

        public void TextAnalysis(string text, string key)
        {
            double monoFreqBit = 1.0 / text.Length;
            double biFreqBit = 1.0 / (text.Length - 1);

            Dictionary<char, char> accordance = new Dictionary<char, char>();
            for (int i = 0; i < 33; i++)
                accordance.Add(key[i], alphabet[i]);

            ClearActualFreq();

            for (int i = 0; i < text.Length - 1; i++)
            {
                char monogram = accordance[text[i]];
                string bigram = monogram.ToString();
                bigram += accordance[text[i + 1]];
                ActualMonoFreq[monogram] += monoFreqBit;
                ActualBiFreq[bigram] += biFreqBit;
            }

            ActualMonoFreq[accordance[text[text.Length - 1]]] += monoFreqBit;
        }

        public double MeaningfullnessMetric()
        {
            double monogramSum = 0.0;
            double bigramSum = 0.0;

            foreach (var pair in MonogramFreq)
                monogramSum += Math.Abs(ActualMonoFreq[pair.Key] - pair.Value);

            foreach (var pair in BigramFreq)
                bigramSum += Math.Abs(ActualBiFreq[pair.Key] - pair.Value);

            bigramSum *= 3;

            return bigramSum + monogramSum;
        }

        private void ClearActualFreq()
        {
            foreach (var key in ActualMonoFreq.Keys.ToArray())
                ActualMonoFreq[key] = 0.0;

            foreach (var key in ActualBiFreq.Keys.ToArray()) 
                ActualBiFreq[key] = 0.0;
        }
    }
}
