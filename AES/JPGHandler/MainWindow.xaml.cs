using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JPGHandler
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			AddData("selfie.jpg", "data.txt", "result.jpg");
		}

		public void AddData(string imageFile, string dataFile, string newImageFile)
		{
			JpegBitmapDecoder decoder = null;
			BitmapFrame bitmapFrame = null;

			using (Stream inStream = File.Open(imageFile, FileMode.Open))
			{
				decoder = new JpegBitmapDecoder(inStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
			}

			bitmapFrame = decoder.Frames[0];
			BitmapMetadata metaData = (BitmapMetadata)bitmapFrame.Metadata.Clone();

			byte[] data = File.ReadAllBytes(dataFile);
			metaData.SetQuery("/app1/ifd/exif:{uint=40092}", data);
			JpegBitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, bitmapFrame.Thumbnail, metaData, bitmapFrame.ColorContexts));

			using (Stream jpegStreamOut = File.Open(newImageFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				encoder.Save(jpegStreamOut);
			}
		}
	}
}
