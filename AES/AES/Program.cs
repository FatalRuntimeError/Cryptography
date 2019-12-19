using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace kript4lr333

{
	class Program
	{
		static string ChooseHashAlgorithm()
		{
			Console.WriteLine("Choose hash algorithm:");
			Console.WriteLine("1 - MD5");
			Console.WriteLine("2 - SHA1");
			Console.WriteLine("3 - SHA256");
			int choice = int.Parse(Console.ReadLine());

			string hashAlgorithm;
			switch (choice)
			{
				case 1:
					hashAlgorithm = "MD5";
					break;
				case 2:
					hashAlgorithm = "SHA1";
					break;
				case 3:
					hashAlgorithm = "SHA256";
					break;
				default:
					hashAlgorithm = "MD5";
					break;
			}

			return hashAlgorithm;
		}

		static void Main(string[] args)
		{
			Console.Write("Enter filename: ");
			string filename = Console.ReadLine();

			Console.Write("Enter filename to save result: ");
			string resultFilename = Console.ReadLine();

			Console.Write("Enter password: ");
			byte[] password = Encoding.Default.GetBytes(Console.ReadLine());

			string hashAlgorithm = ChooseHashAlgorithm();
			Encryptor encryptor = new Encryptor(hashAlgorithm, password);

			Console.WriteLine("Choose:");
			Console.WriteLine("1 - Encryption");
			Console.WriteLine("2 - Decryption");
			int choice = int.Parse(Console.ReadLine());

			switch (choice)
			{
				case 1:
					//Bitmap bmp = new Bitmap(Image.FromFile(filename));
					//bmp = encryptor.Encrypt(bmp);
					//bmp.Save(resultFilename, ImageFormat.Bmp);
					byte[] data = File.ReadAllBytes(filename);
					File.WriteAllBytes(resultFilename, encryptor.Encrypt(data));
					break;
				case 2:
					byte[] encryptedData = File.ReadAllBytes(filename);
					File.WriteAllBytes(resultFilename, encryptor.Decrypt(encryptedData));
					//Bitmap encryptedBmp = new Bitmap(Image.FromFile(filename));
					//encryptedBmp = encryptor.Decrypt(encryptedBmp);
					//encryptedBmp.Save(resultFilename, ImageFormat.Bmp);
					break;
			}
		}
	}

	class Encryptor
	{
		public HashAlgorithm hashAlgorithm { get; }
		public Aes aes { get; }

		public Encryptor(string hashAlgorithm, byte[] password)
		{
			this.hashAlgorithm = HashAlgorithm.Create(hashAlgorithm);
			this.password = password;

			key = this.hashAlgorithm.ComputeHash(password);
			aes = Aes.Create();
			aes.Padding = PaddingMode.Zeros;
			aes.Key = key;
			aes.IV = key;
			aes.Mode = CipherMode.CBC;
		}

		public byte[] Encrypt(byte[] data)
		{
			byte[] encryptedData;
			var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
			string strData = Encoding.ASCII.GetString(data);

			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter sw = new StreamWriter(cs))
						sw.Write(strData);

					encryptedData = ms.ToArray();
				}
			}

			return encryptedData;
		}

		public byte[] Decrypt(byte[] encryptedData)
		{
			byte[] data;
			var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

			using (MemoryStream ms = new MemoryStream(encryptedData))
			{
				using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader sr = new StreamReader(cs))
						data = Encoding.ASCII.GetBytes(sr.ReadToEnd());
				}
			}

			return data;
		}
	}

	class BMPEncryptor
	{
		public Encryptor encryptor { get; }

		public BMPEncryptor(string hashAlgorithm, byte[] password)
		{
			encryptor = new Encryptor(hashAlgorithm, password);
		}

		public Bitmap Encrypt(Bitmap bmp)
		{
			byte[] data = GetBMPData(bmp);
			byte[] encryptedData = encryptor.Encrypt(data);
			bmp = SetBMPData(bmp, encryptedData);

			return bmp;
		}

		public Bitmap Decrypt(Bitmap bmp)
		{
			throw new NotImplementedException();
		}

		private byte[] GetBMPData(Bitmap bmp)
		{
			int w = bmp.Width,
			h = bmp.Height;
			byte[] res = new byte[3 * h * w];

			for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					Color color = bmp.GetPixel(i, j);
					res[i * h * 3 + j * 3] = color.R;
					res[i * h * 3 + j * 3 + 1] = color.G;
					res[i * h * 3 + j * 3 + 2] = color.B;
				}

			return res;
		}

		private Bitmap SetBMPData(Bitmap bmp, byte[] data)
		{
			int w = bmp.Width,
			h = bmp.Height;

			for (int i = 0; i < w; i++)
				for (int j = 0; j < h; j++)
				{
					byte R = data[i * h * 3 + j * 3];
					byte G = data[i * h * 3 + j * 3 + 1];
					byte B = data[i * h * 3 + j * 3 + 2];
					Color color = Color.FromArgb(R, G, B);
					bmp.SetPixel(i, j, color);
				}

			return bmp;
		}
	}
}

