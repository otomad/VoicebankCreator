using System.Drawing.Imaging;
using System.IO;

using NAudio.Wave;

namespace VoicebankCreator.Helpers;

internal static class Extensions {
	public static ImageSource ToImageSource(this System.Drawing.Image image) {
		using MemoryStream? memoryStream = new();
		image.Save(memoryStream, ImageFormat.Png);
		memoryStream.Seek(0, SeekOrigin.Begin);

		BitmapImage? bitmapImage = new();
		bitmapImage.BeginInit();
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		bitmapImage.StreamSource = memoryStream;
		bitmapImage.EndInit();
		bitmapImage.Freeze();

		return bitmapImage;
	}

	public static void PrepareToReplay(this AudioFileReader waveStream) {
		waveStream.Seek(0, SeekOrigin.Begin);
	}
}
