using System;
using System.IO;

namespace KeyGenerator
{
	class Program
	{
		static int GCD(int a, int b)
		{
			while (b != 0)
			{
				int t = b;
				b = a % b;
				a = t;
			}

			return a;
		}

		static int LCM(int a, int b) => a * b / GCD(a, b);

		static bool IsPrime(int num)
		{
			bool isPrime = true;
			int top = (int)Math.Sqrt(num) + 1;
			for (int i = 2; i < top && isPrime; i++)
				if (num % i == 0)
					isPrime = false;

			return isPrime;
		}

		static bool IsCoprime(int a, int b) => GCD(a, b) == 1;

		static void ReadPQ(ref int p, ref int q)
		{
			do
			{
				do
				{
					Console.Write("Enter p: ");
					p = int.Parse(Console.ReadLine());

					if (!IsPrime(p))
						Console.WriteLine("ERROR!. p is not prime number.\n");
				}
				while (!IsPrime(p));

				do
				{
					Console.Write("Enter q: ");
					q = int.Parse(Console.ReadLine());

					if (!IsPrime(q))
						Console.WriteLine("ERROR!. q is not prime number.\n");
				}
				while (!IsPrime(q));

				if (p == q)
					Console.WriteLine("ERROR! q equals p.\n");
			}
			while (p == q);
		}

		static int ModInv(int e, int ln)
		{
			int t = 0, newt = 1;
			int r = ln, newr = e;

			while (newr != 0)
			{
				int q = r / newr;
				(t, newt) = (newt, t - q * newt);
				(r, newr) = (newr, r - q * newr);
			}

			return t < 0 ? t + ln : t;
		}

		static void Main(string[] args)
		{
			Random rand = new Random();
			int p = 0, q = 0;
			ReadPQ(ref p, ref q);

			int n = p * q;
			int ln = LCM(p - 1, q - 1);

			int e = rand.Next(2, ln);

			while (!IsCoprime(e, ln))
			{
				e++;
				if (e == ln)
					e = rand.Next(2, ln);
			}

			int d = ModInv(e, ln);

			File.WriteAllText("PublicKey.txt", string.Format("{0} {1}", n, e));
			File.WriteAllText("PrivateKey.txt", string.Format("{0} {1}", n, d));
		}
	}
}
