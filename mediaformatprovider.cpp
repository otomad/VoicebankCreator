#include "mediaformatprovider.h"

static QStringList VoicebankCreator::getNameFilters() {
	QStringList result;
	const QList<FileFormat> formats = QMediaFormat().supportedFileFormats(QMediaFormat::Decode);
	for (qsizetype i = 0, size = formats.size(); i < size; i++) {
		const FileFormat format = formats.at(i);
		QMediaFormat mediaFormat(format);
		const QMimeType mimeType = mediaFormat.mimeType();
		if (mimeType.isValid()) {
			QString filter = QMediaFormat::fileFormatDescription(format) + " (";
			const QStringList suffixes = mimeType.suffixes();
			for (qsizetype i = 0, size = suffixes.size(); i < size; i++) {
				if (i) filter += u' ';
				filter += "*." + suffixes.at(i);
			}
			filter += u')';
			result.append(filter);
		}
	}
	return result;
}
