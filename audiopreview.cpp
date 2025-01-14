#include "audiopreview.h"

AudioPreview::AudioPreview(QQuickItem *parent) : QQuickPaintedItem(parent) {
	// QAudioFormat format;
	// format.setSampleRate(44100);
	// format.setChannelCount(2);
	// format.setSampleFormat(QAudioFormat::Int16);

	QAudioFormat format;
	format.setSampleRate(44100);
	format.setChannelCount(1);
	format.setSampleFormat(QAudioFormat::Int16);

	decoder = new QAudioDecoder(this);
	decoder->setAudioFormat(format);

	connect(decoder, &QAudioDecoder::bufferReady, this, &AudioPreview::onBufferReady);
	connect(decoder, &QAudioDecoder::finished, this, &AudioPreview::onDecodingFinished);
	connect(decoder, qOverload<QAudioDecoder::Error>(&QAudioDecoder::error), this, &AudioPreview::onDecodingError);
}

AudioPreview::~AudioPreview() { }

void AudioPreview::setUrl(QString url) {
	QUrl source(url);
	samples.clear();
	// samples->squeeze();
	decoder->setSource(source);
	decoder->start();
	peak = 0;

	this->url = url;
}

void AudioPreview::onBufferReady() {
	QAudioBuffer buffer = decoder->read();
	if (!buffer.isValid()) {
		qWarning() << "Invalid audio buffer!";
		return;
	}
	if (!peak) peak = getPeakValue(buffer.format());
	const qint16 *data = buffer.constData<qint16>();
	qsizetype start = samples.count(), length = buffer.sampleCount();
	samples.resize(start + length);
	for (qsizetype i = 0, j = start; i < length; i++, j++)
		samples[j] = data[i] / peak;
}

/**
 * @see https://stackoverflow.com/questions/46947668/draw-waveform-from-raw-data-using-qaudioprobe
 * @return The peak value.
 */
qreal AudioPreview::getPeakValue(const QAudioFormat &format) {
	qreal ret(0);
	if (format.isValid()){
		switch (format.sampleFormat()) {
			case QAudioFormat::Unknown:
			case QAudioFormat::NSampleFormats:
			default:
				qWarning() << "Unknown sample format!";
				ret = 1;
				break;
			case QAudioFormat::Float:
				ret = 1.00003;
				break;
			case QAudioFormat::Int32:
#ifdef Q_OS_WIN
				ret = INT_MAX;
#elifdef Q_OS_UNIX
				ret = SHRT_MAX;
#endif
				break;
			case QAudioFormat::Int16:
				ret = SHRT_MAX;
				break;
			case QAudioFormat::UInt8:
				ret = UCHAR_MAX;
				break;
		}
	}
	return ret;
}

void AudioPreview::onDecodingFinished() {
	// audioSink->stop();
	// qDebug() << "Finish!!!";
	if (samples.empty()) return;
	// qDebug() << samples.count() / 44100;
	// for (qsizetype i = 0; i < samples.count(); i++)
	// 	if (samples[i] != 0) {
	// 		qDebug() << i;
	// 		break;
	// 	}
	update();
	// qDebug() << contentsBoundingRect();
	// qDebug() << contentsSize();
}

void AudioPreview::onDecodingError(QAudioDecoder::Error error) {
	qWarning() << "Decoding Error: " << decoder->errorString();
}

void AudioPreview::paint(QPainter *painter) {
	if (samples.empty()) return;
	QBrush brush(color);
	painter->setBrush(brush);
	painter->setPen(Qt::NoPen);
	painter->setRenderHint(QPainter::Antialiasing);
	QSizeF itemSize = size();
	qreal halfHeight = itemSize.height() / 2;
	for (qreal i = 0; i < itemSize.width(); i++) {
		qsizetype x = i / itemSize.width() * samples.count();
		qreal y = samples[x];
		painter->drawRect(i, halfHeight, 1, halfHeight * y);
	}
}
