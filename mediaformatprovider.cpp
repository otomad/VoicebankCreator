#include "mediaformatprovider.h"

static inline bool isAudioFormat(FileFormat format) { return format < QMediaFormat::Mpeg4Audio; }

QStringList VoicebankCreator::getNameFilters() {
	QStringList result;
	const QList<FileFormat> formats = QMediaFormat().supportedFileFormats(QMediaFormat::Decode);
	QStringList audioSuffixes, videoSuffixes, allSuffixes;
	for (qsizetype i = 0, size = formats.size(); i < size; i++) {
		const FileFormat format = formats.at(i);
		QMediaFormat mediaFormat(format);
		QStringList* generalSuffixes = isAudioFormat(format) ? &audioSuffixes : &videoSuffixes;
		const QMimeType mimeType = mediaFormat.mimeType();
		if (mimeType.isValid()) {
			QStringList suffixes = mimeType.suffixes();
			for (qsizetype i = 0, size = suffixes.size(); i < size; i++)
				suffixes[i] = "*." + suffixes[i];
			allSuffixes += suffixes;
			*generalSuffixes += suffixes;
			const QString description = QMediaFormat::fileFormatDescription(format);
			result += description + " (" + suffixes.join(' ') + ")";
		}
	}
	result.prepend((QString)"Audio Files" + " (" + audioSuffixes.join(' ') + ")");
	result.prepend((QString)"Video Files" + " (" + videoSuffixes.join(' ') + ")");
	result.prepend((QString)"All Supported Files" + " (" + allSuffixes.join(' ') + ")");
	return result;
}
