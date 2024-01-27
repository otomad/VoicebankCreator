using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.Drawing;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using System.Windows.Media.Imaging;

namespace VoicebankCreator.Media;

public class AudioPlayer {
	private WaveOutEvent? outputDevice;
	public AudioFileReader? audio;
	public AudioFileReader? audioForWaveform;
	private string? filePath;
	public string? FilePath {
		get => filePath;
		set {
			filePath = value;
			outputDevice?.Stop();
			audio?.Dispose();
			audioForWaveform?.Dispose();
			if (filePath != null) {
				audio = new(filePath);
				audioForWaveform = new(filePath);
			}
		}
	}

	public AudioPlayer(string? filePath = null) {
		FilePath = filePath;
	}

	public void Play(double playbackRate = 1) {
		if (audio == null) return;
		new Thread(() => {
			outputDevice?.Stop();
			outputDevice = new();
			IWaveProvider resultAudio;
			audio.PrepareToReplay();
			resultAudio = audio;
			if (playbackRate != 1) {
				audio.ToWaveProvider16();
				SampleToWaveProvider16 wave16 = new(audio);
				WaveFormat format = new((int)(wave16.WaveFormat.SampleRate * playbackRate), wave16.WaveFormat.BitsPerSample, wave16.WaveFormat.Channels);

				byte[] buffer = new byte[audio.Length];
				wave16.Read(buffer, 0, buffer.Length);
				using RawSourceWaveStream newAudio = new(new MemoryStream(buffer), format);
				resultAudio = newAudio;
			}

			outputDevice.Init(resultAudio);
			outputDevice.Play();
			outputDevice.PlaybackStopped += (sender, e) => {
				if (sender is not WaveOutEvent outputDevice) return;
				outputDevice.Dispose();
			};
			while (outputDevice?.PlaybackState == PlaybackState.Playing) {
				Thread.Sleep(500);
			}
		}).Start();
	}

	public void Stop() {
		outputDevice?.Stop();
	}

	public void DrawWaveform(WriteableBitmap wBitmap, System.Windows.Media.Color? color = null) {
		DrawWaveform(wBitmap, color.ToDrawingColor());
	}

	public void DrawWaveform(WriteableBitmap wBitmap, Color? color = null) {
		color ??= Color.Green;
		if (audioForWaveform == null) return;
		audioForWaveform.PrepareToReplay();
		float[] data = new float[audioForWaveform.Length];
		audioForWaveform.Read(data, 0, data.Length);

		const int PADDING_WIDTH = 0;
		int width = wBitmap.PixelWidth - 2 * PADDING_WIDTH;
		int height = wBitmap.PixelHeight - 2 * PADDING_WIDTH;
		wBitmap.Lock();

		using Bitmap backBitmap = new(width, height, wBitmap.BackBufferStride, PixelFormat.Format32bppArgb, wBitmap.BackBuffer);

		using Graphics graphics = Graphics.FromImage(backBitmap);
		graphics.Clear(Color.Transparent);
		Pen pen = new(color.Value);

		int size = data.Length / 4;
		for (int iPixel = 0; iPixel < width; iPixel++) {
			// determine start and end points within WAV
			int start = (int)(iPixel * ((float)size / width));
			int end = (int)((iPixel + 1) * ((float)size / width));
			float min = float.MaxValue;
			float max = float.MinValue;
			for (int i = start; i < end; i++) {
				float val = data[i];
				min = val < min ? val : min;
				max = val > max ? val : max;
			}
			int yMax = PADDING_WIDTH + height - (int)((max + 1) * 0.5 * height);
			int yMin = PADDING_WIDTH + height - (int)((min + 1) * 0.5 * height);
			graphics.DrawLine(pen, iPixel + PADDING_WIDTH, yMax, iPixel + PADDING_WIDTH, yMin);
		}

		graphics.Flush();

		wBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
		wBitmap.Unlock();
	}
}
