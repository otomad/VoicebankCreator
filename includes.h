#ifndef INCLUDES_H
#define INCLUDES_H

#include <QObject>
#include <QFileInfo>
#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <QCommandLineParser>
#include <QDir>
#include <QMediaFormat>
#include <QMimeType>
#include <QStringBuilder>
#include <QTranslator>
#include <QFont>
#include <QIcon>
// #include "boolinq.h"

// /**
//  * @brief Returns a translated version of sourceText.
//  * @details
//  * Optionally based on a disambiguation string and value of n for strings containing plurals;
//  * otherwise returns QString::fromUtf8(sourceText) if no appropriate translated string is available.
//  *
//  * @param sourceText - Source text to be translated.
//  * @param disambiguation - An additional identifying string may be passed in disambiguation (nullptr by default).
//  * @param n - A number indicates the plural form of the text.
//  * @return A translated version of sourceText.
//  */
// static inline QString tr(const char *sourceText, const char *disambiguation = nullptr, int n = -1) {
// 	return QObject::tr(sourceText, disambiguation, n);
// }

#endif // INCLUDES_H
