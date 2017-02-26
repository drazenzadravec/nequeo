// NequeoHttpUIClient.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <QtWidgets\QApplication>
#include <QtWidgets\QDesktopWidget>
#include <QtCore\QDir>

#include "httpwindow.h"

int main(int argc, char *argv[])
{
	FreeConsole();

	QApplication app(argc, argv);

	HttpWindow httpWin;
	const QRect availableSize = QApplication::desktop()->availableGeometry(&httpWin);
	httpWin.resize(availableSize.width() / 5, availableSize.height() / 5);
	httpWin.move((availableSize.width() - httpWin.width()) / 2, (availableSize.height() - httpWin.height()) / 2);

	httpWin.show();
	return app.exec();

}

