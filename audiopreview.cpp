#include "audiopreview.h"

AudioPreview::AudioPreview(QQuickItem *parent) : QQuickPaintedItem(parent) {
	// QAudioFormat format;
	// format.setSampleRate(44100);
	// format.setChannelCount(2);
	// format.setSampleFormat(QAudioFormat::Int16);

	decoder = new QAudioDecoder(this);

	connect(decoder, &QAudioDecoder::bufferReady, this, &AudioPreview::onBufferReady);
	connect(decoder, &QAudioDecoder::finished, this, &AudioPreview::onDecodingFinished);
	connect(decoder, qOverload<QAudioDecoder::Error>(&QAudioDecoder::error), this, &AudioPreview::onDecodingError);
}

AudioPreview::~AudioPreview() {
	delete decoder;
}

QString AudioPreview::getUrl() const { return url; }

void AudioPreview::setUrl(QString url) {
	decoder->setSource(QUrl(url));
	decoder->start();

	this->url = url;
}

void AudioPreview::onBufferReady() {
	QAudioBuffer buffer = decoder->read();
	if (!buffer.isValid()) {
		qWarning() << "Invalid audio buffer";
		return;
	}
	data = QByteArray::fromRawData(reinterpret_cast<const char*>(buffer.data<double>()), buffer.byteCount()); // TODO: supports multiple bit depths.
}

void AudioPreview::onDecodingFinished() {
	// audioSink->stop();
}

void AudioPreview::onDecodingError(QAudioDecoder::Error error) {
	qWarning() << "Decoding Error: " << decoder->errorString();
}

void AudioPreview::paint(QPainter *painter) {
	QBrush brush((QColor(Qt::white)));
	painter->setBrush(brush);
	painter->setPen(Qt::SolidLine);
	painter->setRenderHint(QPainter::Antialiasing);
	QSizeF itemSize = size();
}
