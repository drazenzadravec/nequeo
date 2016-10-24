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
#include <QLabel>
#include <QLayout>
#include <QSettings>
#include <QLineEdit>
#include <QComboBox>
#include <QPushButton>
#include <QFileDialog>
#include <QTextStream>
#include <QRadioButton>
#include <mgl2/mgl.h>
#include "open_dlg.h"
int numDataOpened=0;
extern mglParse parser;
QStringList dataScr;
//-----------------------------------------------------------------------------
QWidget *createDataOpenDlg(QWidget *p)	{	return new DataOpenDialog(p);	}
QString getOpenDataFile(QWidget *w, QString filename)
{
	DataOpenDialog *d = dynamic_cast<DataOpenDialog *>(w);
	if(d)
	{
		d->setFile(filename);
		if(d->exec())	return d->getCode();
	}
	return QString();
}
//-----------------------------------------------------------------------------
DataOpenDialog::DataOpenDialog(QWidget *parent) : QDialog(parent)
{
	setWindowTitle(tr("UDAV - Open data file"));
	QHBoxLayout *a;
	QLabel *l;
	QPushButton *b;
	QVBoxLayout *o=new QVBoxLayout(this);

	a = new QHBoxLayout;	o->addLayout(a);
	l = new QLabel(tr("Data name"));	a->addWidget(l);
	char buf[32];	snprintf(buf,32,"mgl_%d",numDataOpened);
	name = new QLineEdit(buf,this);		a->addWidget(name);

	rA = new QRadioButton(tr("Auto detect data sizes"), this);
	rA->setChecked(true);	o->addWidget(rA);
	rM = new QRadioButton(tr("Set data sizes manually"), this);
	o->addWidget(rM);	a = new QHBoxLayout;	o->addLayout(a);
	l = new QLabel(tr("Nx"));	a->addWidget(l);
	nx = new QLineEdit("1",this);	a->addWidget(nx);
	l = new QLabel(tr("Ny"));	a->addWidget(l);
	ny = new QLineEdit("1",this);	a->addWidget(ny);
	l = new QLabel(tr("Nz"));	a->addWidget(l);
	nz = new QLineEdit("1",this);	a->addWidget(nz);
	r2 = new QRadioButton(tr("Matrix with sizes from file"), this);	o->addWidget(r2);
	r3 = new QRadioButton(tr("3D data with sizes from file"), this);o->addWidget(r3);


	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	dataScr = settings.value("/dataScr").toStringList().mid(0,10);
	dataScr.removeDuplicates();
	settings.endGroup();

	a = new QHBoxLayout;		o->addLayout(a);
	l = new QLabel(tr("Template"));	a->addWidget(l,0);
	scr = new QComboBox(this);		a->addWidget(scr,1);
	scr->setEditable(true);			scr->lineEdit()->setText("");
	scr->addItem(tr("default"));	scr->addItems(dataScr);
	b = new QPushButton("...", this);	a->addWidget(b,0);
	connect(b, SIGNAL(clicked()),this, SLOT(selectScr()));

	a = new QHBoxLayout;	o->addLayout(a);	a->addStretch(1);
	b = new QPushButton(tr("Cancel"),this);	a->addWidget(b);
	connect(b,SIGNAL(clicked()),this,SLOT(reject()));
	b = new QPushButton(tr("OK"), this);	a->addWidget(b);
	connect(b, SIGNAL(clicked()),this, SLOT(prepareResult()));
	b->setDefault(true);
}
//-----------------------------------------------------------------------------
DataOpenDialog::~DataOpenDialog(){}
//-----------------------------------------------------------------------------
void DataOpenDialog::selectScr()
{
	QString str = QFileDialog::getOpenFileName(this, tr("UDAV - Insert filename"),
					scr->lineEdit()->text(), tr("MGL files (*.mgl)"));
	if(!str.isEmpty())
	{
		scr->lineEdit()->setText(str);
		scr->insertItem(1,str);
		dataScr.insert(0,str);
		dataScr.removeDuplicates();
	}
}

//-----------------------------------------------------------------------------
void DataOpenDialog::prepareResult()
{
	code = "";	numDataOpened++;	data = name->text();
	// prepare unique value of name for next time
	char buf[32];	snprintf(buf,32,"mgl_%d",numDataOpened);	name->setText(buf);
	mglVar *v = parser.AddVar(data.toStdString().c_str());
	bool dd=0;
	if(rA->isChecked())	//	auto sizes
	{
		setlocale(LC_NUMERIC, "C");	v->Read(file.toStdString().c_str());	setlocale(LC_NUMERIC, "");
		if(v->nx==1)	{	v->nx = v->ny;	v->ny = v->nz;	}
		code=QString("#read %1 '%2'\n").arg(data).arg(file);
	}
	else if(rM->isChecked())	//	manual sizes
	{
		int x=nx->text().toInt(), y=ny->text().toInt(), z=nz->text().toInt();
		setlocale(LC_NUMERIC, "C");	v->Read(file.toStdString().c_str(),x,y,z);	setlocale(LC_NUMERIC, "");
		code=QString("#read %1 '%2' %3 %4 %5\n").arg(data).arg(file).arg(x).arg(y).arg(z);
	}
	else if(r2->isChecked())	//	matrix
	{
		setlocale(LC_NUMERIC, "C");	v->ReadMat(file.toStdString().c_str());	setlocale(LC_NUMERIC, "");
		code=QString("#readmat %1 '%2'\n").arg(data).arg(file);		dd=1;
	}
	else if(r3->isChecked())	//	3d-data
	{
		setlocale(LC_NUMERIC, "C");	v->ReadMat(file.toStdString().c_str(),3);	setlocale(LC_NUMERIC, "");
		code=QString("#readmat %1 '%2' 3\n").arg(data).arg(file);	dd=2;
	}
	if(scr->lineEdit()->text().isEmpty() || scr->lineEdit()->text()==tr("default"))
	{
		if(v->nz>1 || dd==2)
			code+=QString("rotate 40 60\ncrange %1:box\nsurf3 %1\n").arg(data);
		else if(v->ny>1 || dd==1)
			code+=QString("rotate 40 60\ncrange %1:zrange %1:box\nsurf %1\n").arg(data);
		else	code+=QString("yrange %1:box\nplot %1\n").arg(data);
	}
	else
	{
		QString str;
		QFile fp(scr->lineEdit()->text());
		if(fp.open(QFile::ReadOnly | QIODevice::Text))
		{
			QTextStream in(&fp);
			str = in.readAll();
			code += str.arg(data);
		}
	}

	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	settings.setValue("/dataScr", dataScr);
	settings.endGroup();

	accept();
}
//-----------------------------------------------------------------------------
void DataOpenDialog::setFile(const QString &fname)
{
	file=fname;
	mglData d(file.toStdString().c_str());
	rA->setText(tr("Auto detect data sizes (%1 x %2 x %3)").arg(d.nx).arg(d.ny).arg(d.nz));
}
//-----------------------------------------------------------------------------
