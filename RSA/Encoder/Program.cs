using System;
using System.IO;
using System.Text;

namespace Encoder
{
	class Program
	{
		static int ModularPow(int b, int exp, int mod)
		{
			int result = 1;

			while (exp > 0)
			{
				if (exp % 2 == 1)
					result = (result * b) % mod;
				exp >>= 1;
				b = (b * b) % mod;
			}

			return result;
		}

		static void ParseKey(string filename, ref int mod, ref int exp)
		{
			string data = File.ReadAllText(filename);
			string[] tokens = data.Split(' ');
			mod = int.Parse(tokens[0]);
			exp = int.Parse(tokens[1]);
		}

		static void EncodeMSG()
		{
			int mod = 0, exp = 0;
			Console.Write("Enter file with public key: ");
			string filename = Console.ReadLine();
			ParseKey(filename, ref mod, ref exp);

			Console.Write("Enter file with message: ");
			filename = Console.ReadLine();
			int m = int.Parse(File.ReadAllText(filename));

			int c = ModularPow(m, exp, mod);
			File.WriteAllText("EncodedMessage.txt", c.ToString());
		}

		static void DecodeMSG()
		{

			int mod = 0, exp = 0;
			Console.Write("Enter file with private key: ");
			string filename = Console.ReadLine();
			ParseKey(filename, ref mod, ref exp);

			Console.Write("Enter file with message: ");
			filename = Console.ReadLine();
			int m = int.Parse(File.ReadAllText(filename));

			int c = ModularPow(m, exp, mod);
			File.WriteAllText("DecodedMessage.txt", c.ToString());
		}

		static void Main(string[] args)
		{

			Console.WriteLine("1 - Encode message.");
			Console.WriteLine("2 - Decode message.");
			int choice = 0;
			while (choice != 1 && choice != 2)
			{
				Console.Write("Your choice: ");
				choice = int.Parse(Console.ReadLine());
			}

			if (choice == 1)
				EncodeMSG();

			if (choice == 2)
				DecodeMSG();
		}
	}
}
