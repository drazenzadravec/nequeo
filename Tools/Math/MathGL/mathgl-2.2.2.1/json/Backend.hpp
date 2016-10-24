#pragma once

#include <QStringList>

class Backend : public QObject
{
	Q_OBJECT

public:
	explicit Backend(QObject *parent = 0);

public:
//	Q_INVOKABLE QString join(const QStringList list) const;
	Q_INVOKABLE QString show(const QString& text) const;
	Q_INVOKABLE QString coor(const QString& xy, const QString& text) const;
	Q_INVOKABLE QString geometry(const QString& mgl) const;
};

