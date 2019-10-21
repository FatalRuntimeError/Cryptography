using System;

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

		static void Main(string[] args)
		{ 
			int p = 0, q = 0;
			ReadPQ(ref p, ref q);

			int n = p * q;
			int lN = LCM(p - 1, q - 1);
		}
	}
}
