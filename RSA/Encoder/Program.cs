using System;
using System.IO;

namespace Encoder
{
	class Program
	{
		static ulong ModularAdd(ulong a, ulong b, ulong mod)
		{
			ulong result = 0;

			a = a % mod;
			b = b % mod;

			ulong diff = mod > a ? mod - a : a - mod;

			if (b < diff)
				result = a + b;
			else
				result = b - diff;

			return result % mod;
		}

		static ulong ModularMult(ulong a, ulong b, ulong mod)
		{
			ulong result = 0;
			a = a % mod;
			b = b % mod;

			while (b != 0)
			{
				if (b % 2 == 1)
					result = ModularAdd(result, a, mod);
				a = ModularAdd(a, a, mod);
				b >>= 1;
			}

			return result;
		}

		static ulong ModularPow(ulong b, ulong exp, ulong mod)
		{
			ulong result = 1;

			while (exp > 0)
			{
				if (exp % 2 == 1)
					result = ModularMult(result, b, mod);
				exp >>= 1;
				b = ModularMult(b, b, mod);
			}

			return result;
		}

		static void ParseKey(string filename, ref ulong mod, ref ulong exp)
		{
			string data = File.ReadAllText(filename);
			string[] tokens = data.Split(' ');
			mod = ulong.Parse(tokens[0]);
			exp = ulong.Parse(tokens[1]);
		}

		static void EncodeMSG()
		{
			ulong mod = 0, exp = 0;
			Console.Write("Enter file with public key: ");
			string filename = Console.ReadLine();
			ParseKey(filename, ref mod, ref exp);

			Console.Write("Enter file with message: ");
			filename = Console.ReadLine();
			ulong m = ulong.Parse(File.ReadAllText(filename));

			ulong c = ModularPow(m, exp, mod);
			File.WriteAllText("EncodedMessage.txt", c.ToString());
		}

		static void DecodeMSG()
		{

			ulong mod = 0, exp = 0;
			Console.Write("Enter file with private key: ");
			string filename = Console.ReadLine();
			ParseKey(filename, ref mod, ref exp);

			Console.Write("Enter file with message: ");
			filename = Console.ReadLine();
			ulong m = ulong.Parse(File.ReadAllText(filename));

			ulong c = ModularPow(m, exp, mod);
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
