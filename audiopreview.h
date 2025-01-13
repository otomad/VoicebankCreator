#ifndef AUDIOPREVIEW_H
#define AUDIOPREVIEW_H

#include "includes.h"
#include <QQuickPaintedItem>
#include <QtMultimedia>

class AudioPreview : public QQuickPaintedItem {
		Q_OBJECT
		QML_ELEMENT
		Q_PROPERTY(QString url READ getUrl WRITE setUrl /*NOTIFY urlChanged*/)

	public:
		AudioPreview(QQuickItem *parent = NULL);
		virtual ~AudioPreview();
		virtual void paint(QPainter *painter);

		QString getUrl() const;
		void setUrl(QString url);

	private:
		QString url;
		QAudioDecoder *decoder;
		QByteArray data;

	protected slots:
		void onBufferReady();
		void onDecodingFinished();
		void onDecodingError(QAudioDecoder::Error error);

	signals:
		// void urlChanged(const QString &newUrl);
};

#endif // AUDIOPREVIEW_H
