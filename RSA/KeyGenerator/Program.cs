using System;
using System.IO;

namespace KeyGenerator
{
	class Program
	{
		static ulong GCD(ulong a, ulong b)
		{
			while (b != 0)
			{
				ulong t = b;
				b = a % b;
				a = t;
			}

			return a;
		}

		static ulong LCM(ulong a, ulong b) => a * b / GCD(a, b);

		static bool IsPrime(ulong num)
		{
			bool isPrime = true;
			ulong top = (ulong)Math.Sqrt(num) + 1;
			for (ulong i = 2; i < top && isPrime; i++)
				if (num % i == 0)
					isPrime = false;

			return isPrime;
		}

		static bool IsCoprime(ulong a, ulong b) => GCD(a, b) == 1;

		static void ReadPQ(ref ulong p, ref ulong q)
		{
			do
			{
				do
				{
					Console.Write("Enter p: ");
					p = ulong.Parse(Console.ReadLine());

					if (!IsPrime(p))
						Console.WriteLine("ERROR!. p is not prime number.\n");
				}
				while (!IsPrime(p));

				do
				{
					Console.Write("Enter q: ");
					q = ulong.Parse(Console.ReadLine());

					if (!IsPrime(q))
						Console.WriteLine("ERROR!. q is not prime number.\n");
				}
				while (!IsPrime(q));

				if (p == q)
					Console.WriteLine("ERROR! q equals p.\n");
			}
			while (p == q);
		}

		static ulong ModInv(ulong e, ulong ln)
		{
			ulong t = 0, newt = 1;
			ulong r = ln, newr = e;

			while (newr != 0)
			{
				ulong q = r / newr;
				(t, newt) = (newt, t - q * newt);
				(r, newr) = (newr, r - q * newr);
			}

			return t < 0 ? t + ln : t;
		}

		static void Main(string[] args)
		{
			// Read p and q and check if p and q are prime
			ulong p = 0, q = 0;
			ReadPQ(ref p, ref q);

			// Compute n and lambda(n)
			ulong n = p * q;
			ulong ln = LCM(p - 1, q - 1);

			// Generate e and check if e and lambda(n) are coprime
			Random rand = new Random();
			ulong e = (1 << 16) + 1;

			if (e >= ln)
			{
				e = (ulong)rand.Next(2, (int)ln);
				while (!IsCoprime(e, ln))
				{
					e++;
					if (e == ln)
						e = (ulong)rand.Next(2, (int)ln);
				}
			}


			while (!IsCoprime(e, ln))
				e--;

			// Compute d
			ulong d = ModInv(e, ln);

			// Writing keys to files
			File.WriteAllText("PublicKey.txt", string.Format("{0} {1}", n, e));
			File.WriteAllText("PrivateKey.txt", string.Format("{0} {1}", n, d));
		}
	}
}
