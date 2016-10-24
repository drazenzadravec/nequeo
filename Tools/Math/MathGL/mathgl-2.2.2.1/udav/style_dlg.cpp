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
#include <QPushButton>
#include <QMessageBox>
#include <QComboBox>
#include <QCheckBox>
#include <QRadioButton>
#include <QGroupBox>
#include <QTabWidget>
#include <QSlider>
#include <QSpinBox>
#include <QLayout>
#include <QLabel>
#include <QLineEdit>
#include <mgl2/mgl.h>
//-----------------------------------------------------------------------------
#include "xpm/none.xpm"
#include "xpm/mark_.xpm"
#include "xpm/mark_cf.xpm"
#include "xpm/mark_x.xpm"
#include "xpm/mark_p.xpm"
#include "xpm/mark_pf.xpm"
#include "xpm/mark_o.xpm"
#include "xpm/mark_of.xpm"
#include "xpm/mark_s.xpm"
#include "xpm/mark_sf.xpm"
#include "xpm/mark_d.xpm"
#include "xpm/mark_df.xpm"
#include "xpm/mark_v.xpm"
#include "xpm/mark_vf.xpm"
#include "xpm/mark_t.xpm"
#include "xpm/mark_tf.xpm"
#include "xpm/mark_l.xpm"
#include "xpm/mark_lf.xpm"
#include "xpm/mark_r.xpm"
#include "xpm/mark_rf.xpm"
#include "xpm/mark_y.xpm"
#include "xpm/mark_a.xpm"
#include "xpm/mark_n.xpm"
#include "xpm/dash_e.xpm"
#include "xpm/dash_s.xpm"
#include "xpm/dash_l.xpm"
#include "xpm/dash_m.xpm"
#include "xpm/dash_d.xpm"
#include "xpm/dash_i.xpm"
#include "xpm/dash_j.xpm"
#include "xpm/arrow_n.xpm"
#include "xpm/arrow_a.xpm"
#include "xpm/arrow_v.xpm"
#include "xpm/arrow_i.xpm"
#include "xpm/arrow_k.xpm"
#include "xpm/arrow_t.xpm"
#include "xpm/arrow_s.xpm"
#include "xpm/arrow_d.xpm"
#include "xpm/arrow_o.xpm"
//-----------------------------------------------------------------------------
#include "style_dlg.h"
void fillColors(QComboBox *cb);
void fillArrows(QComboBox *cb);
void fillDashes(QComboBox *cb);
void fillMarkers(QComboBox *cb);
void convertFromGraph(QPixmap &pic, mglGraph *gr, uchar **buf);
//-----------------------------------------------------------------------------
StyleDialog::StyleDialog(QWidget *parent) : QDialog(parent)
{
	setWindowTitle(tr("UDAV - Insert style/scheme"));
	QWidget *p;
	QHBoxLayout *h;
	QVBoxLayout *v, *u, *vv;
	QGridLayout *g;
	QLabel *l;
	QPushButton *b;
	grBuf = 0;
	tab = new QTabWidget(this);
	// line style
	p = new QWidget(this);
	g = new QGridLayout(p);	g->setAlignment(Qt::AlignTop);
//	g->setColStretch(0, 1);	g->setColStretch(1, 1);	g->setColStretch(2, 1);
	l = new QLabel(tr("Arrow at start"), p);	g->addWidget(l, 0, 0);
	l = new QLabel(tr("Dashing"), p);		g->addWidget(l, 0, 1);
	l = new QLabel(tr("Arrow at end"), p);	g->addWidget(l, 0, 2);
	a1 = new QComboBox(p);	g->addWidget(a1, 1, 0);	fillArrows(a1);
	dash = new QComboBox(p);	g->addWidget(dash, 1, 1);	fillDashes(dash);
	a2 = new QComboBox(p);	g->addWidget(a2, 1, 2);	fillArrows(a2);
	l = new QLabel(tr("Color"), p);	g->addWidget(l, 2, 0, Qt::AlignRight);
	cline=new QComboBox(p);	g->addWidget(cline, 2, 1);	fillColors(cline);

	nline = new QSlider(p);		g->addWidget(nline, 2, 2);
	nline->setRange(1, 9);		nline->setValue(5);
	nline->setTickPosition(QSlider::TicksBothSides);
	nline->setTickInterval(1);	nline->setPageStep(2);
	nline->setOrientation(Qt::Horizontal);
	
	l = new QLabel(tr("Marks"), p);	g->addWidget(l, 3, 0, Qt::AlignRight);
	mark = new QComboBox(p);	g->addWidget(mark, 3, 1);	fillMarkers(mark);
	l = new QLabel(tr("Line width"), p);	g->addWidget(l, 4, 0, Qt::AlignRight);
	width = new QSpinBox(p);	g->addWidget(width, 4, 1);
	width->setRange(1,9);	width->setValue(1);
	connect(a1,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(a2,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(dash,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(mark,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(cline,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(nline,SIGNAL(valueChanged(int)), this, SLOT(updatePic()));
	connect(width,SIGNAL(valueChanged(int)), this, SLOT(updatePic()));
	tab->addTab(p, tr("Line style"));
	// color scheme
	p = new QWidget(this);
	v = new QVBoxLayout(p);	v->setAlignment(Qt::AlignTop);
	g = new QGridLayout();	v->addLayout(g);
//	g->setColStretch(0, 1);			g->setColStretch(1, 1);
	l = new QLabel(tr("Color order"), p);	g->addWidget(l, 0, 0);
	l = new QLabel(tr("Saturation"),p);		g->addWidget(l, 0, 1);
	for(int i=0;i<7;i++)
	{
		cc[i] = new QComboBox(p);	g->addWidget(cc[i], i+1, 0);
		fillColors(cc[i]);
		nn[i] = new QSlider(p);		g->addWidget(nn[i], i+1, 1);
		nn[i]->setRange(1, 9);		nn[i]->setValue(5);
		nn[i]->setTickPosition(QSlider::TicksBothSides);
		nn[i]->setTickInterval(1);	nn[i]->setPageStep(2);
		nn[i]->setOrientation(Qt::Horizontal);
		connect(cc[i],SIGNAL(activated(int)), this, SLOT(updatePic()));
		connect(nn[i],SIGNAL(valueChanged(int)), this, SLOT(updatePic()));
	}
	swire = new QCheckBox(tr("Wire or mesh plot"),p);	v->addWidget(swire);
	g = new QGridLayout();	v->addLayout(g);
	l = new QLabel(tr("Axial direction"), p);	g->addWidget(l, 0, 0, Qt::AlignRight);
	l = new QLabel(tr("Text on contours"), p);	g->addWidget(l, 1, 0, Qt::AlignRight);
	axial = new QComboBox(p);	g->addWidget(axial, 0, 1);
	axial->addItem(tr("none"));	axial->addItem("x");
	axial->addItem("y");	axial->addItem("z");
	ctext = new QComboBox(p);	g->addWidget(ctext, 1, 1);
	ctext->addItem(tr("none"));	ctext->addItem(tr("under"));	ctext->addItem(tr("above"));
	connect(axial,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(ctext,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(swire,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	tab->addTab(p, tr("Color scheme"));
	// font style
	p = new QWidget(this);
	v = new QVBoxLayout(p);	v->setAlignment(Qt::AlignTop);
	h = new QHBoxLayout();	v->addLayout(h);
	u = new QVBoxLayout();	h->addLayout(u);
	bold = new QCheckBox(tr("Bold style"), p);	u->addWidget(bold);
	ital = new QCheckBox(tr("Italic style"), p);u->addWidget(ital);
	wire = new QCheckBox(tr("Wire style"), p);	u->addWidget(wire);
	uline = new QCheckBox(tr("Underline"), p);	u->addWidget(uline);
	oline = new QCheckBox(tr("Overline"), p);	u->addWidget(oline);
	u = new QVBoxLayout();	h->addLayout(u);
	l = new QLabel(tr("Text color"), p);		u->addWidget(l);
	cfont = new QComboBox(p);	fillColors(cfont);	u->addWidget(cfont);
	u->addSpacing(6);
	align = new QGroupBox(tr("Text align"), p);	u->addWidget(align);
	vv = new QVBoxLayout(align);		//vv->addSpacing(11);
	rbL = new QRadioButton(tr("left"), align);	vv->addWidget(rbL);
	rbC = new QRadioButton(tr("at center"), align);
	vv->addWidget(rbC);	rbC->setChecked(true);
	rbR = new QRadioButton(tr("right"), align);	vv->addWidget(rbR);
	connect(bold,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(ital,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(wire,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(uline,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(oline,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(cfont,SIGNAL(activated(int)), this, SLOT(updatePic()));
	connect(rbL,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(rbC,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	connect(rbR,SIGNAL(toggled(bool)), this, SLOT(updatePic()));
	tab->addTab(p, tr("Font style"));
	connect(tab,SIGNAL(currentChanged(int)), this, SLOT(updatePic()));

	// dialog itself
	v = new QVBoxLayout(this);	v->addWidget(tab);
	h = new QHBoxLayout();		v->addLayout(h);
	l = new QLabel(tr("Resulting string"), this);	h->addWidget(l);	h->addStretch(1);
	pic = new QLabel(this);	pic->setMinimumSize(QSize(128,30));	h->addWidget(pic);
	res = new QLineEdit(this);	res->setReadOnly(true);	v->addWidget(res);

	h = new QHBoxLayout();	v->addLayout(h);	h->addStretch(1);
	b = new QPushButton(tr("Cancel"), this);	h->addWidget(b);
	connect(b, SIGNAL(clicked()),this, SLOT(reject()));
	b = new QPushButton(tr("OK"), this);		h->addWidget(b);
	connect(b, SIGNAL(clicked()),this, SLOT(accept()));
	b->setDefault(true);
}
//-----------------------------------------------------------------------------
StyleDialog::~StyleDialog()	{	if(grBuf)	delete []grBuf;	}
//-----------------------------------------------------------------------------
void fillColors(QComboBox *cb)
{
//	string id : 	"wbgrcmylenuqphkWBGRCMYLENUQPH"
	QPixmap pic(16,16);
	cb->addItem(QPixmap(none_xpm), QObject::tr("none or default"));
	pic.fill(QColor(255,255,255));	cb->addItem(pic, QObject::tr("w - white"));
	pic.fill(QColor(0,0,255));		cb->addItem(pic, QObject::tr("b - blue"));
	pic.fill(QColor(0,255,0));		cb->addItem(pic, QObject::tr("g - lime"));
	pic.fill(QColor(255,0,0));		cb->addItem(pic, QObject::tr("r - red"));
	pic.fill(QColor(0,255,255));	cb->addItem(pic, QObject::tr("c - cyan"));
	pic.fill(QColor(255,0,255));	cb->addItem(pic, QObject::tr("m - magenta"));
	pic.fill(QColor(255,255,0));	cb->addItem(pic, QObject::tr("y - yellow"));
	pic.fill(QColor(0,255,127));	cb->addItem(pic, QObject::tr("l - springgreen"));
	pic.fill(QColor(127,255,0));	cb->addItem(pic, QObject::tr("e - lawngreen"));
	pic.fill(QColor(0,127,255));	cb->addItem(pic, QObject::tr("n - skyblue"));
	pic.fill(QColor(127,0,255));	cb->addItem(pic, QObject::tr("u - blueviolet"));
	pic.fill(QColor(255,127,0));	cb->addItem(pic, QObject::tr("q - orange"));
	pic.fill(QColor(255,0,127));	cb->addItem(pic, QObject::tr("p - deeppink"));
	pic.fill(QColor(127,127,127));	cb->addItem(pic, QObject::tr("h - gray"));
	pic.fill(QColor(0,0,0));		cb->addItem(pic, QObject::tr("k - black"));
	pic.fill(QColor(179,179,179));	cb->addItem(pic, QObject::tr("W - lightgray"));
	pic.fill(QColor(0,0,127));		cb->addItem(pic, QObject::tr("B - navy"));
	pic.fill(QColor(0,127,0));		cb->addItem(pic, QObject::tr("G - green"));
	pic.fill(QColor(127,0,0));		cb->addItem(pic, QObject::tr("R - maroon"));
	pic.fill(QColor(0,127,127));	cb->addItem(pic, QObject::tr("C - teal"));
	pic.fill(QColor(127,0,127));	cb->addItem(pic, QObject::tr("M - purple"));
	pic.fill(QColor(127,127,0));	cb->addItem(pic, QObject::tr("Y - olive"));
	pic.fill(QColor(0,127,77));		cb->addItem(pic, QObject::tr("L - seagreen"));
	pic.fill(QColor(77,127,0));		cb->addItem(pic, QObject::tr("E - darklawn"));
	pic.fill(QColor(0,77,127));		cb->addItem(pic, QObject::tr("N - darkskyblue"));
	pic.fill(QColor(77,0,127));		cb->addItem(pic, QObject::tr("U - indigo"));
	pic.fill(QColor(127,77,0));		cb->addItem(pic, QObject::tr("Q - brown"));
	pic.fill(QColor(127,0,77));		cb->addItem(pic, QObject::tr("P - darkpink"));
	pic.fill(QColor(77,77,77));		cb->addItem(pic, QObject::tr("H - darkgray"));
}
//-----------------------------------------------------------------------------
void fillArrows(QComboBox *cb)
{
	// "AVIKTSDO"
	cb->addItem(QPixmap(arrow_n_xpm), QObject::tr("none"));
	cb->addItem(QPixmap(arrow_a_xpm), QObject::tr("arrow"));
	cb->addItem(QPixmap(arrow_v_xpm), QObject::tr("back arrow"));
	cb->addItem(QPixmap(arrow_i_xpm), QObject::tr("stop"));
	cb->addItem(QPixmap(arrow_k_xpm), QObject::tr("size"));
	cb->addItem(QPixmap(arrow_t_xpm), QObject::tr("triangle"));
	cb->addItem(QPixmap(arrow_s_xpm), QObject::tr("square"));
	cb->addItem(QPixmap(arrow_d_xpm), QObject::tr("rhomb"));
	cb->addItem(QPixmap(arrow_o_xpm), QObject::tr("circle"));
}
//-----------------------------------------------------------------------------
void fillDashes(QComboBox *cb)
{
	// "-|;=ji: "
	cb->addItem(QPixmap(dash_s_xpm), QObject::tr("solid"));
	cb->addItem(QPixmap(dash_l_xpm), QObject::tr("long dash"));
	cb->addItem(QPixmap(dash_m_xpm), QObject::tr("dash"));
	cb->addItem(QPixmap(dash_e_xpm), QObject::tr("small dash"));
	cb->addItem(QPixmap(dash_j_xpm), QObject::tr("dash dot"));
	cb->addItem(QPixmap(dash_i_xpm), QObject::tr("small dash dot"));
	cb->addItem(QPixmap(dash_d_xpm), QObject::tr("dots"));
	cb->addItem(QPixmap(mark_n_xpm), QObject::tr("none"));
}
//-----------------------------------------------------------------------------
void fillMarkers(QComboBox *cb)
{
	// ".+x*sdv^<>o.*+xsdv^<>o" : nf = 10
	cb->addItem(QPixmap(mark_n_xpm), QObject::tr("none"));
	cb->addItem(QPixmap(mark__xpm), QObject::tr("dot"));
	cb->addItem(QPixmap(mark_p_xpm), QObject::tr("plus"));
	cb->addItem(QPixmap(mark_x_xpm), QObject::tr("skew cross"));
	cb->addItem(QPixmap(mark_a_xpm), QObject::tr("asterix"));
	cb->addItem(QPixmap(mark_s_xpm), QObject::tr("square"));
	cb->addItem(QPixmap(mark_d_xpm), QObject::tr("rhomb"));
	cb->addItem(QPixmap(mark_v_xpm), QObject::tr("triangle down"));
	cb->addItem(QPixmap(mark_t_xpm), QObject::tr("triangle up"));
	cb->addItem(QPixmap(mark_l_xpm), QObject::tr("triangle left"));
	cb->addItem(QPixmap(mark_r_xpm), QObject::tr("triangle right"));
	cb->addItem(QPixmap(mark_o_xpm), QObject::tr("circle"));

	cb->addItem(QPixmap(mark_cf_xpm), QObject::tr("circled dot"));
	cb->addItem(QPixmap(mark_y_xpm),  QObject::tr("Y-sign"));
	cb->addItem(QPixmap(mark_pf_xpm), QObject::tr("squared plus"));
	cb->addItem(QPixmap(none_xpm),	  QObject::tr("squared cross"));

	cb->addItem(QPixmap(mark_sf_xpm), QObject::tr("solid square"));
	cb->addItem(QPixmap(mark_df_xpm), QObject::tr("solid rhomb"));
	cb->addItem(QPixmap(mark_vf_xpm), QObject::tr("solid triangle down"));
	cb->addItem(QPixmap(mark_tf_xpm), QObject::tr("solid triangle up"));
	cb->addItem(QPixmap(mark_lf_xpm), QObject::tr("solid triangle left"));
	cb->addItem(QPixmap(mark_rf_xpm), QObject::tr("solid triangle right"));
	cb->addItem(QPixmap(mark_of_xpm), QObject::tr("solid circle"));
}
//-----------------------------------------------------------------------------
void StyleDialog::updatePic()
{
	static mglGraph gr(0,128,30);
	static bool f = true;
	mglData x(3), y(3), a(32,2);
	x.Fill(-1,1);	a.Fill(-1,1);
	if(!f)	gr.Clf();
	if(f)
	{
		gr.SubPlot(1,1,0,"");
		gr.SetMarkSize(20);
		gr.SetArrowSize(20);
		f = false;
	}
	result = "";
	int i,j;
	QString col="wbgrcmylenuqphkWBGRCMYLENUQPH", mrk=".+x*sdv^<>o.*+xsdv^<>o", dsh="|;=ji: ", arw="AVIKTSDO", s;
	switch(tab->currentIndex())
	{
	case 0:	// line style
		i = a2->currentIndex();		if(i>0)	result += arw[i-1];
		j = a1->currentIndex();		if(j>0)
		{
			if(i==0)	result += '_';
			result += arw[j-1];
		}
		i = dash->currentIndex();	if(i>0)	result += dsh[i-1];
		i = mark->currentIndex();	if(i>0)	result += mrk[i-1];
		if(i>11)	result += '#';
		i = cline->currentIndex();
		if(i>0)
		{
			j = nline->value();
			if(j!=5)	result += "{"+col[i-1]+char('0'+j)+"}";
			else		result += col[i-1];
		}
		i = width->value();		if(i>1)	result += char('0'+i);
		gr.Plot(x,y,result.toStdString().c_str());
		break;
	case 1: // color sceheme
		for(j=0;j<7;j++)
		{
			i = cc[j]->currentIndex();
			if(i<1)	break;
			QCharRef c = col[i-1];
			i = nn[j]->value();
			if(i!=5)	result += "{"+c+char('0'+i)+"}";
			else		result += c;
		}
		if(swire->isChecked())	result += '#';
		i = ctext->currentIndex();
		if(i==1)	result += 't';
		if(i==2)	result += 'T';
		i = axial->currentIndex();
		if(i>0)	result = result+':'+char('x'+i-1);
		gr.Surf(a,result.toStdString().c_str());
		break;
	case 2: // text style
		i = cfont->currentIndex();
		if(i>1)	result += col[i-1];
		result += ':';
		if(bold->isChecked())	result += 'b';
		if(ital->isChecked())	result += 'i';
		if(wire->isChecked())	result += 'w';
		if(uline->isChecked())	result += 'u';
		if(oline->isChecked())	result += 'o';
		if(rbL->isChecked())	result += 'L';
		if(rbC->isChecked())	result += 'C';
		if(rbR->isChecked())	result += 'R';
		gr.Puts(mglPoint(0,-0.5),"Font test",result.toStdString().c_str(),-10);
		break;
	}
	result = "'" + result + "'";
	res->setText(result);
	QPixmap p;
	convertFromGraph(p, &gr, &grBuf);
	pic->setPixmap(p);
}
//-----------------------------------------------------------------------------
void convertFromGraph(QPixmap &pic, mglGraph *gr, uchar **buf)
{
	register long w=gr->GetWidth(), h=gr->GetHeight();
	if(*buf)	delete [](*buf);
	*buf = new uchar[4*w*h];
	gr->GetBGRN(*buf,4*w*h);
	QImage img(*buf, w, h, QImage::Format_RGB32);
	pic = QPixmap::fromImage(img);
}
//-----------------------------------------------------------------------------
