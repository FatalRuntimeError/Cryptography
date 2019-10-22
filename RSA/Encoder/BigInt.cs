using System;
using System.Collections.Generic;

namespace Encoder
{
   public class BigInt
	{
		private List<int> numbers = new List<int>();

		public BigInt()
		{
			numbers.Add(0);
		}

		public BigInt(BigInt num)
		{
			for (int i = 0; i < num.Size; i++)
				numbers.Add(num[i]);
		}

		public BigInt(int num)
		{
			while (num > 0)
			{
				int digit = num % 10;
				numbers.Add(digit);
				num /= 10;
			}
		}

		public BigInt(long num)
		{
			while (num > 0)
			{
				long digit = num % 10;
				numbers.Add((int)digit);
				num /= 10;
			}
		}

		public BigInt(string s)
		{
			char[] arr = s.ToCharArray();
			Array.Reverse(arr);
			s = new string(arr);

			foreach (char letter in s)
				numbers.Add(int.Parse(letter.ToString()));
		}

		public int Size => numbers.Count;

		private int this[int i]
		{
			get => numbers[i];
			set => numbers[i] = value;
		}
		
		public void Add(BigInt a)
		{
			int length = Math.Max(a.Size, Size) + 1;
			for (int i = 0; i < length - Size; i++)
				numbers.Add(0);

			for (int i = 0; i < a.Size; i++)
			{
				numbers[i] += a[i];
				int carry = numbers[i] / 10;

				for (int j = i; carry == 1; j++)
				{
					numbers[j] %= 10;
					numbers[j + 1] += 1;
					carry = numbers[j + 1] / 10;
				}
			}

			for (int i = numbers.Count - 1; numbers[i] == 0; i--)
				numbers.RemoveAt(i);
		}

		public void Mult(BigInt a)
		{
			int length = a.Size + Size + 1;
			List<int> temp = new List<int>();
			for (int i = 0; i < length; i++)
				temp.Add(0);

			for (int ai = 0; ai < a.Size; ai++)
				for (int i = 0; i < Size; i++)
					temp[ai + i] += numbers[i] * a[ai];

			for (int i = 0; i < length - 1; i++)
			{
				temp[i + 1] += temp[i] / 10;
				temp[i] %= 10;
			}

			for (int i = temp.Count - 1; temp[i] == 0; i--)
				temp.RemoveAt(i);

			numbers = temp;
		}
	}
}
