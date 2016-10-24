/***************************************************************************
 *   Copyright (C) 2008 by Alexey Balakin                                  *
 *   mathgl.abalakin@gmail.com                                             *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
#include <QLayout>
#include <QLineEdit>
#include <QPushButton>
#include <QTextBrowser>
#include <QToolButton>
#include <stdio.h>
//-----------------------------------------------------------------------------
#include "help_pnl.h"
extern QString pathHelp;
void raisePanel(QWidget *w);
//-----------------------------------------------------------------------------
QWidget *createHlpPanel(QWidget *p)		{	return new HelpPanel(p);	}
void showHelpMGL(QWidget *p,QString s)
{
	HelpPanel *hlp = dynamic_cast<HelpPanel *>(p);
	if(hlp)	hlp->showHelp(s);
}
//void showExMGL(QWidget *hlp)			{	((HelpPanel *)hlp)->showExamples();	}
//-----------------------------------------------------------------------------
HelpPanel::HelpPanel(QWidget *parent) : QWidget(parent)
{
	QPushButton *b;
	QToolButton *t;
	QVBoxLayout *o = new QVBoxLayout(this);
	QHBoxLayout *a = new QHBoxLayout();	o->addLayout(a);
	help = new QTextBrowser(this);		o->addWidget(help);
	help->setOpenExternalLinks(false);

	b = new QPushButton(QPixmap(":/xpm/go-previous.png"), tr("Backward"));
	connect(b, SIGNAL(clicked()), help, SLOT(backward()));	a->addWidget(b);
	entry = new QLineEdit(this);	a->addWidget(entry);
	connect(entry, SIGNAL(textChanged(const QString &)), this, SLOT(showHelp(const QString &)));
	connect(entry, SIGNAL(returnPressed()), this, SLOT(showHelp()));
	b = new QPushButton(QPixmap(":/xpm/go-next.png"), tr("Forward"));
	connect(b, SIGNAL(clicked()), help, SLOT(forward()));	a->addWidget(b);
//	b = new QPushButton(QPixmap(":/xpm/help-faq.png"), tr("Examples"));
//	connect(b, SIGNAL(clicked()), this, SLOT(showExamples()));	a->addWidget(b);
	t = new QToolButton(this);	t->setIcon(QPixmap(":/xpm/zoom-in.png"));
	connect(t, SIGNAL(clicked()), this, SLOT(zoomIn()));	a->addWidget(t);
	t = new QToolButton(this);	t->setIcon(QPixmap(":/xpm/zoom-out.png"));
	connect(t, SIGNAL(clicked()), this, SLOT(zoomOut()));	a->addWidget(t);
	setWindowTitle(tr("Help"));
}
//-----------------------------------------------------------------------------
// void HelpPanel::showExamples()
// {
// 	QStringList s;	s<<(pathHelp);
// 	help->setSearchPaths(s);
// 	setWindowTitle("Examples");	raisePanel(this);
// 	help->setSource(tr("mgl_en")+"_2.html");
// }
//-----------------------------------------------------------------------------
void HelpPanel::showHelp(const QString &txt)
{
	QString cmd=txt;
	raisePanel(this);
	QStringList s;	s<<(pathHelp);
	help->setSearchPaths(s);
	if(cmd.isEmpty())	cmd = entry->text().trimmed();
	if(cmd.isEmpty())	help->setSource(tr("mgl_en")+".html");
	else	help->setSource(tr("mgl_en")+".html#"+cmd);
	setWindowTitle("Help");
}
//-----------------------------------------------------------------------------
void HelpPanel::zoomIn()
{	QFont f(help->font());	f.setPointSize(f.pointSize()+1);	help->setFont(f);	}
//-----------------------------------------------------------------------------
void HelpPanel::zoomOut()
{	QFont f(help->font());	f.setPointSize(f.pointSize()-1);	help->setFont(f);	}
//-----------------------------------------------------------------------------
