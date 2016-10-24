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
#include <QTableWidget>
#include <QToolButton>
#include <QInputDialog>
#include <QMessageBox>
#include <mgl2/mgl.h>
//-----------------------------------------------------------------------------
#include "mem_pnl.h"
#include "info_dlg.h"
//-----------------------------------------------------------------------------
#include "xpm/table.xpm"
#include "xpm/preview.xpm"
//-----------------------------------------------------------------------------
extern bool mglAutoSave;
extern mglParse parser;
QWidget *newDataWnd(InfoDialog *inf, QWidget *wnd, mglVar *v);
void refreshData(QWidget *w);
//-----------------------------------------------------------------------------
QWidget *createMemPanel(QWidget *p)	// NOTE: parent should be MainWindow
{
	MemPanel *m = new MemPanel(p);
	m->wnd = p;	return m;
}
//-----------------------------------------------------------------------------
void refreshMemPanel(QWidget *p)
{
	MemPanel *m = dynamic_cast<MemPanel *>(p);
	if(m)	m->refresh();
}
//-----------------------------------------------------------------------------
MemPanel::MemPanel(QWidget *parent) : QWidget(parent)
{
	QHBoxLayout *h;
	QVBoxLayout *v;
	QToolButton *b;

	infoDlg = new InfoDialog(this);
	infoDlg->setModal(true);	infoDlg->allowRefresh=false;

	v = new QVBoxLayout(this);	h = new QHBoxLayout();	v->addLayout(h);
	b = new QToolButton(this);	b->setIcon(QPixmap(":/xpm/document-new.png"));
	b->setToolTip(tr("Create new data array"));		h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(newTable()));
	b = new QToolButton(this);	b->setIcon(QPixmap(table_xpm));
	b->setToolTip(tr("Edit selected data array"));	h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(editData()));
	b = new QToolButton(this);	b->setIcon(QPixmap(":/xpm/edit-delete.png"));
	b->setToolTip(tr("Delete selected data array"));		h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(delData()));
	b = new QToolButton(this);	b->setIcon(QPixmap(preview_xpm));
	b->setToolTip(tr("Properties of selected data array"));	h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(infoData()));
	b = new QToolButton(this);	b->setIcon(QPixmap(":/xpm/view-refresh.png"));
	b->setToolTip(tr("Update list of data arrays"));		h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(refresh()));
	h->addStretch(1);
	b = new QToolButton(this);	b->setIcon(QPixmap(":/xpm/edit-delete.png"));
	b->setToolTip(tr("Delete ALL data arrays"));	h->addWidget(b);
	connect(b, SIGNAL(clicked()), this, SLOT(delAllData()));

	colSort = 0;
	tab = new QTableWidget(this);	tab->setColumnCount(3);	v->addWidget(tab);
	QStringList sl;	sl<<tr("Name")<<tr("Sizes")<<tr("Memory");
	tab->setHorizontalHeaderLabels(sl);
	connect(tab, SIGNAL(cellClicked(int,int)), this, SLOT(tableClicked(int,int)));
	connect(tab, SIGNAL(cellDoubleClicked(int,int)), this, SLOT(tableDClicked(int,int)));

	setWindowTitle(tr("Memory"));
}
//-----------------------------------------------------------------------------
void MemPanel::tableClicked(int, int col)
{	colSort = col;	tab->sortItems(col);	}
//-----------------------------------------------------------------------------
void MemPanel::tableDClicked(int row, int)	{	editData(row);	}
//-----------------------------------------------------------------------------
void MemPanel::newTable()
{
	bool ok;
	QString name = QInputDialog::getText(this, tr("UDAV - New variable"),
				tr("Enter name for new variable"), QLineEdit::Normal, "", &ok);
	if(!ok || name.isEmpty())	return;
	mglVar *v = parser.AddVar(name.toStdString().c_str());
	QWidget *t;
	if(v->o)	t = (QWidget *)v->o;
	else		t = newDataWnd(infoDlg,wnd,v);
	t->showMaximized();	t->activateWindow();
	refresh();
}
//-----------------------------------------------------------------------------
void MemPanel::editData(int n)
{
	if(tab->rowCount()<1)	return;
	if(n<0)	n = tab->currentRow();
	if(n<0)	n = 0;
	mglVar *v = parser.FindVar(tab->item(n,0)->text().toStdString().c_str());
	if(!v)	return;
	QWidget *t;
	if(v->o)	t = (QWidget *)v->o;
	else		t = newDataWnd(infoDlg,wnd,v);
	t->showMaximized();	t->activateWindow();
}
//-----------------------------------------------------------------------------
void MemPanel::delData()
{
	if(tab->rowCount()<1)	return;
	int	n = tab->currentRow();
	if(n<0)	n = 0;
	mglVar *v = parser.FindVar(tab->item(n,0)->text().toStdString().c_str());
	if(!v && v->o)	((QWidget *)v->o)->close();
	parser.DeleteVar(tab->item(n,0)->text().toStdString().c_str());
	refresh();
}
//-----------------------------------------------------------------------------
void MemPanel::delAllData()
{
	if(QMessageBox::information(this, tr("UDAV - delete all data"),
			tr("Do you want to delete all data?"), QMessageBox::No,
			QMessageBox::Yes)!=QMessageBox::Yes)	return;
	parser.DeleteAll();	refresh();
}
//-----------------------------------------------------------------------------
void MemPanel::infoData()
{
	if(tab->rowCount()<1)	return;
	int	n = tab->currentRow();
	if(n<0)	n = 0;
	mglVar *v = parser.FindVar(tab->item(n,0)->text().toStdString().c_str());
	if(!v)	return;
	infoDlg->setVar(v);
	QString s = QString::fromStdWString(v->s);
	infoDlg->setWindowTitle(s + tr(" - UDAV preview"));
	infoDlg->refresh();
	infoDlg->show();
}
//-----------------------------------------------------------------------------
void MemPanel::refresh()
{
	mglVar *v = parser.FindVar("");
	int n = 0;
	while(v)	{	v = v->next;	n++;	}
	tab->setRowCount(n);
	v = parser.FindVar("");	n = 0;
	QString s;
	QTableWidgetItem *it;
	Qt::ItemFlags flags=Qt::ItemIsSelectable|Qt::ItemIsEnabled;
	while(v)
	{
		s = QString::fromStdWString(v->s);
		it = new QTableWidgetItem(s);
		tab->setItem(n,0,it);	it->setFlags(flags);
		s.sprintf("%ld * %ld * %ld", v->nx, v->ny, v->nz);
		it = new QTableWidgetItem(s);
		tab->setItem(n,1,it);	it->setFlags(flags);
		it->setTextAlignment(Qt::AlignHCenter|Qt::AlignVCenter);
		s.sprintf("%12ld", v->nx*v->ny*v->nz*sizeof(mreal));
		it = new QTableWidgetItem(s);
		tab->setItem(n,2,it);	it->setFlags(flags);
		it->setTextAlignment(Qt::AlignRight|Qt::AlignVCenter);
		if(v->o)	refreshData((QWidget *)v->o);
		v = v->next;	n++;
	}
	tab->sortItems(colSort);
}
//-----------------------------------------------------------------------------
