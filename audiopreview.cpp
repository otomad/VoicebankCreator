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
	if (this->url == url) return;

	QUrl source(url);
	samples.clear();
	// samples->squeeze();
	clear();
	decoder->setSource(source);
	decoder->start();
	peak = 0;

	this->url = url;
}

static const float MAX_PROGRESS = 1;

void AudioPreview::onBufferReady() {
	setLoadingProgress(decoder->position() / (double)decoder->duration() * MAX_PROGRESS);
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
	setLoadingProgress(MAX_PROGRESS);
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
	painter->setCompositionMode(QPainter::CompositionMode_Source);
	if (loadingProgress <= 0) {
		painter->fillRect(0, 0, width(), height(), QBrush(QColor(Qt::transparent)));
		return;
	}
	if (samples.empty()) return;
	painter->setBrush(QBrush(color));
	painter->setPen(color);
	painter->setRenderHint(QPainter::SmoothPixmapTransform);
	QSizeF itemSize = size();
	qreal halfHeight = itemSize.height() / 2;
	const qreal dpi = 1; // window()->screen()->devicePixelRatio();
	qreal width_double = itemSize.width() * dpi;
	// NOTE: halfHeight represents half of the height, while width_double represents the width in the double type.
	qsizetype width = width_double;
	// QPointF maxPoints[width + 2], minPoints[width + 2];
	for (qsizetype i = 0; i < width; i++) {
		qsizetype x = i / width_double * samples.count();
		// qreal y = samples[x];
		// painter->drawRect(i, halfHeight, 1, halfHeight * y);
		// painter->drawLine(i, halfHeight, i, halfHeight + halfHeight * y);

		qsizetype xNext = qMin((qsizetype)((i + 1) / width_double * samples.count()), samples.count() - 1);
		// qreal yMax = samples[x], yMin = samples[x];
		// for (int xi = x + 1; xi < xNext; xi++) {
		// 	qreal yi = samples[xi];
		// 	if (yi > yMax) yMax = yi;
		// 	if (yi < yMin) yMin = yi;
		// }
		qreal yMax = *std::max_element(samples.begin() + x, samples.begin() + xNext),
			yMin = *std::min_element(samples.begin() + x, samples.begin() + xNext);

		// maxPoints[i] = QPointF(i / dpi, halfHeight + halfHeight * yMax);
		// minPoints[i] = QPointF(i / dpi, halfHeight + halfHeight * yMin);
		// painter->drawLine(i / dpi, halfHeight, i / dpi, halfHeight + halfHeight * yMax);
		// painter->drawLine(i / dpi, halfHeight, i / dpi, halfHeight + halfHeight * yMin);
		painter->drawLine(i / dpi, halfHeight * (1 + yMin), i / dpi, halfHeight * (1 + yMax));
	}
	// maxPoints[width] = minPoints[width] = QPointF(width / dpi, halfHeight);
	// maxPoints[width + 1] = minPoints[width + 1] = QPointF(0, halfHeight);
	// painter->drawPolygon(maxPoints, width + 2);
	// painter->drawPolygon(minPoints, width + 2);
}

/**
 * @brief Clear paint elements.
 */
void AudioPreview::clear() {
	setLoadingProgress(-1);
	update();
}

void AudioPreview::setLoadingProgress(float value) {
	if (value < 0) value = -1;
	else if (value > 1) value = 1;
	const int ACCURATE_TO = 100; // Update the loadingProgressChanged slot every 0.01 (100^-1).
	if ((int)(loadingProgress * ACCURATE_TO) == (int)(value * ACCURATE_TO)) return;
	loadingProgress = value;
	loadingProgressChanged(value);
}
