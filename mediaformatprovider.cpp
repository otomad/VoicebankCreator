#include "mediaformatprovider.h"

static inline bool isAudioFormat(FileFormat format) { return format >= QMediaFormat::Mpeg4Audio; }

static QString concatNameFilter(QString description, QStringList suffixes) {
	return QString("%1 (%2)").arg(description, suffixes.join(' '));
}

// Cannot use Qt build-in tr function in global function directly.
// see: https://stackoverflow.com/questions/64774242/qt5-tr-macro-does-not-translate-text
static inline auto tr(const char *sourceText) { return QCoreApplication::translate("VoicebankCreator", sourceText); }

QStringList VoicebankCreator::getNameFilters() {
	QStringList result;
	QList<FileFormat> formats = QMediaFormat().supportedFileFormats(QMediaFormat::Decode);
	std::sort(formats.begin(), formats.end());
	QStringList audioSuffixes, videoSuffixes, allSuffixes;
	for (const FileFormat format : formats) {
		QMediaFormat mediaFormat(format);
		QStringList &generalSuffixes = isAudioFormat(format) ? audioSuffixes : videoSuffixes;
		const QMimeType mimeType = mediaFormat.mimeType();
		if (mimeType.isValid()) {
			QStringList suffixes = mimeType.suffixes();
			for (QString &suffix : suffixes)
				suffix = "*." + suffix;
			allSuffixes += suffixes;
			generalSuffixes += suffixes;
			QString description = QMediaFormat::fileFormatDescription(format);
			// Rename "Wave File" to "Wave Files" to match the name of other filter items.
			if (description.endsWith(" File", Qt::CaseInsensitive)) description += 's';
			result += concatNameFilter(description, suffixes);
		}
	}
	result.prepend(concatNameFilter(tr("Audio Files"), audioSuffixes));
	result.prepend(concatNameFilter(tr("Video Files"), videoSuffixes));
	result.prepend(concatNameFilter(tr("All Supported Files"), allSuffixes));
	return result;
}
