#include "mediaformatprovider.h"

static inline bool isAudioFormat(FileFormat format) { return format >= QMediaFormat::Mpeg4Audio; }

static QString concatNameFilter(QString description, QStringList suffixes) {
	return QString("%1 (%2)").arg(description, suffixes.join(' '));
}

QStringList VoicebankCreator::getNameFilters() {
	QStringList result;
	QList<FileFormat> formats = QMediaFormat().supportedFileFormats(QMediaFormat::Decode);
	std::sort(formats.begin(), formats.end());
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
			QString description = QMediaFormat::fileFormatDescription(format);
			// Rename "Wave File" to "Wave Files" to match the name of other filter items.
			if (description.endsWith(" File", Qt::CaseInsensitive)) description += 's';
			result += concatNameFilter(description, suffixes);
		}
	}
	result.prepend(concatNameFilter("Audio Files", audioSuffixes));
	result.prepend(concatNameFilter("Video Files", videoSuffixes));
	result.prepend(concatNameFilter("All Supported Files", allSuffixes));
	return result;
}
