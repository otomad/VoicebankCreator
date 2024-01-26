using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace VoicebankCreator.Media;

public class AudioPlayer {
	private WaveOutEvent? outputDevice;
	private string? filePath;
	public string? FilePath {
		get => filePath;
		set {
			filePath = value;
			outputDevice?.Stop();
			outputDevice?.Dispose();
		}
	}

	public AudioPlayer(string? filePath = null) {
		FilePath = filePath;
	}

	public void Play(double playbackRate = 1) {
		new Thread(() => {
			outputDevice?.Stop();
			outputDevice = new();
			IWaveProvider resultAudio;
			using AudioFileReader audio = new(FilePath);
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
}
