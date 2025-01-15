#ifndef AUDIOPREVIEW_H
#define AUDIOPREVIEW_H

#include "includes.h"
#include <QQuickPaintedItem>
#include <QtMultimedia>

class AudioPreview : public QQuickPaintedItem {
		Q_OBJECT
		QML_ELEMENT
		Q_PROPERTY(QString url READ getUrl WRITE setUrl /*NOTIFY urlChanged*/)
		Q_PROPERTY(QColor color READ getColor WRITE setColor)
		Q_PROPERTY(float loadingProgress READ getLoadingProgress NOTIFY loadingProgressChanged)

	public:
		AudioPreview(QQuickItem *parent = NULL);
		virtual ~AudioPreview();
		virtual void paint(QPainter *painter);

		inline QString getUrl() const { return url; }
		void setUrl(QString url);

		inline QColor getColor() const { return color; }
		inline void setColor(QColor color) { this->color = color; update(); }

		inline float getLoadingProgress() const { return loadingProgress; }

	private:
		QString url;
		QAudioDecoder *decoder;
		QList<qreal> samples;
		qreal getPeakValue(const QAudioFormat &format);
		qreal peak;
		QColor color;
		float loadingProgress = -1;
		void setLoadingProgress(float value);
		void clear();

	protected slots:
		void onBufferReady();
		void onDecodingFinished();
		void onDecodingError(QAudioDecoder::Error error);

	signals:
		// void urlChanged(const QString &newUrl);
		void loadingProgressChanged(const float loadingProgress);
};

#endif // AUDIOPREVIEW_H
