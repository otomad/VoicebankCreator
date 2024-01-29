using VoicebankCreator.Controls;

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace VoicebankCreator.Serializables;

public class Voicebank {
	public Dictionary<int, string> Sources = new();
	public TimeSpan Offset = TimeSpan.FromSeconds(0);
	public SortedDictionary<string, List<Syllable>> Syllables = new();

	public class Syllable {
		public TimeSpan Start;
		public TimeSpan Length;
		public string? Pitch;
		public int Source;
		[YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public TimeOffset? VideoOffset;
	}

	public class TimeOffset {
		public TimeSpan Start;
		public TimeSpan Length;
	}

	public Voicebank(string source, IEnumerable<RangeZone> rangeZones) {
		const int SOURCE_INDEX = 1;
		Sources.Add(SOURCE_INDEX, source);
		foreach (RangeZone rangeZone in rangeZones) {
			Syllable syllable = new() {
				Start = TimeSpan.FromSeconds(rangeZone.StartSeconds),
				Length = TimeSpan.FromSeconds(rangeZone.LengthSeconds),
				Pitch = "C#5",
				Source = SOURCE_INDEX,
			};
			if (Syllables.ContainsKey(rangeZone.Text))
				Syllables[rangeZone.Text].Add(syllable);
			else
				Syllables.Add(rangeZone.Text, new() { syllable });
		}
	}

	private static readonly Serializer serializer = new();

	public static string ToYaml(Voicebank voicebank) {
		string yaml = serializer.Serialize(voicebank);
		return yaml;
	}

	public static void ToYaml(Voicebank voicebank, string filePath) {
		string yaml = ToYaml(voicebank);
		using TextWriter writer = File.CreateText(filePath);
		writer.Write(yaml);
	}
}
