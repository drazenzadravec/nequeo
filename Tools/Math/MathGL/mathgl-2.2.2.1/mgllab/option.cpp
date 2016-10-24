/* option.cpp is part of UDAV
 * Copyright (C) 2007-2014 Alexey Balakin <mathgl.abalakin@gmail.ru>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library General Public License
 * as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
#include <string.h>
#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Box.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Check_Button.H>
#include <FL/Fl_Round_Button.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Return_Button.H>
#include <FL/Fl_Tabs.H>
#include <FL/Fl_Group.H>
#include <FL/Fl_Choice.H>
#include <FL/Fl_Spinner.H>
#include <FL/Fl_Output.H>
#include <FL/Fl_Text_Buffer.H>
#include "udav.h"
//-----------------------------------------------------------------------------
extern Fl_Menu_Item colors[];
extern Fl_Text_Buffer	*textbuf;
//-----------------------------------------------------------------------------
struct OptionDlg
{
public:
	Fl_Window* wnd;
	int OK;
	char result[256];
	OptionDlg()	{	memset(this,0,sizeof(OptionDlg));	create_dlg();	}
	~OptionDlg()	{	delete wnd;	}
	void FillResult();
protected:
	Fl_Input *xmin, *xmax, *ymin, *ymax, *zmin, *zmax, *cmin, *cmax;
	Fl_Input *alpha, *amb, *mesh, *font;
	Fl_Choice *cut;

	void create_dlg();
} option_dlg;
//-----------------------------------------------------------------------------
struct StyleDlg
{
public:
friend void style_set_cb(Fl_Widget *, void *v);
friend void style_rdo_cb(Fl_Widget *, void *v);
friend void font_cb(Fl_Widget *, void *v);
friend void line_cb(Fl_Widget *, void *v);
friend void face_cb(Fl_Widget *, void *v);
	Fl_Window* wnd;
	int OK;
	char result[16];
	StyleDlg()	{	memset(this,0,sizeof(StyleDlg));	create_dlg();	}
	~StyleDlg()	{	delete wnd;	}
protected:
	Fl_Tabs *tab;
	Fl_Group *ltab, *stab, *ftab;
	Fl_Choice *cl, *cf, *c[7], *ae, *as;
	Fl_Choice *dash, *mark, *dir, *text;
	Fl_Spinner *lw;
	Fl_Output *res;
	Fl_Check_Button *d, *w, *sc, *rm, *it, *bf, *gt;
	Fl_Round_Button *rl, *rc, *rr;

	void create_dlg();
} style_dlg;
//-----------------------------------------------------------------------------
void option_dlg_cb(Fl_Widget *, void *v)
{	option_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void style_dlg_cb(Fl_Widget *, void *v)
{	style_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void OptionDlg::create_dlg()
{
	Fl_Button *o;
	wnd = new Fl_Window(490, 180, gettext("Command options"));
	new Fl_Box(10, 15, 75, 25, gettext("X-Range"));
	xmin = new Fl_Input(85, 15, 75, 25);
	xmin->tooltip(gettext("Minimal value of X for cutting or for coordinate filling"));
	xmax = new Fl_Input(165, 15, 75, 25);
	xmax->tooltip(gettext("Maximal value of X for cutting or for coordinate filling"));
	new Fl_Box(245, 15, 75, 25, gettext("Y-Range"));
	ymin = new Fl_Input(320, 15, 75, 25);
	ymin->tooltip(gettext("Minimal value of Y for cutting or for coordinate filling"));
	ymax = new Fl_Input(400, 15, 75, 25);
	ymax->tooltip(gettext("Maximal value of Y for cutting or for coordinate filling"));
	new Fl_Box(10, 45, 75, 25, gettext("Z-Range"));
	zmin = new Fl_Input(85, 45, 75, 25);
	zmin->tooltip(gettext("Minimal value of Z for cutting or for coordinate filling"));
	zmax = new Fl_Input(165, 45, 75, 25);
	zmax->tooltip(gettext("Maximal value of Z for cutting or for coordinate filling"));
	new Fl_Box(245, 45, 75, 25, gettext("C-Range"));
	cmin = new Fl_Input(320, 45, 75, 25);
	cmin->tooltip(gettext("Low border for determining color or alpha"));
	cmax = new Fl_Input(400, 45, 75, 25);
	cmax->tooltip(gettext("Upper border for determining color or alpha"));
	{	Fl_Box *o = new Fl_Box(15, 75, 460, 5);			o->box(FL_UP_BOX);	}
	alpha = new Fl_Input(25, 105, 75, 25, "Alpha");		alpha->align(FL_ALIGN_TOP);
	alpha->tooltip(gettext("Alpha value (transparency) of surface or cloud"));
	amb = new Fl_Input(110, 105, 75, 25, gettext("Ambient"));	amb->align(FL_ALIGN_TOP);
	amb->tooltip(gettext("Own brightness of the surface"));
	mesh = new Fl_Input(195, 105, 75, 25, gettext("Mesh Num"));	mesh->align(FL_ALIGN_TOP);
	mesh->tooltip(gettext("Approximate number of mesh lines in plot"));
	font = new Fl_Input(280, 105, 75, 25, gettext("Font Size"));	font->align(FL_ALIGN_TOP);
	font->tooltip(gettext("Act as default value for font size"));
	cut = new Fl_Choice(365, 105, 75, 25, gettext("Cutting"));	cut->align(FL_ALIGN_TOP);
	cut->add(gettext("on"));	cut->add(gettext("off"));
	cut->tooltip(gettext("Set cutting off/on for particular plot"));

	o = new Fl_Button(320, 145, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(405, 145, 75, 25, gettext("OK"));	o->callback(option_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
void OptionDlg::FillResult()
{
	double x1=0,y1=0,z1=0,x2=0,y2=0,z2=0;
	bool u1,v1,w1,u2,v2,w2;
	char str[64];
	result[0]=0;

	u1 = xmin->value()[0];	if(u1)	x1 = atof(xmin->value());
	u2 = xmax->value()[0];	if(u2)	x2 = atof(xmax->value());
	v1 = ymin->value()[0];	if(v1)	y1 = atof(ymin->value());
	v2 = ymin->value()[0];	if(v2)	y2 = atof(ymax->value());
	w1 = zmin->value()[0];	if(w1)	z1 = atof(zmin->value());
	w2 = zmin->value()[0];	if(w2)	z2 = atof(zmax->value());
	if(u1 && u2)	{snprintf(str,64,"; xrange %g %g",x1,x2);	strcat(result,str);}
	if(v1 && v2)	{snprintf(str,64,"; yrange %g %g",y1,y2);	strcat(result,str);}
	if(w1 && w2)	{snprintf(str,64,"; zrange %g %g",z1,z2);	strcat(result,str);}

	u1 = cmin->value()[0];	if(u1)	x1 = atof(cmin->value());
	u2 = cmax->value()[0];	if(u2)	x2 = atof(cmax->value());
	if(u1&&u2)	{snprintf(str,64,"; crange %g %g",x1,x2);	strcat(result,str);}

	if(alpha->value()[0])
	{	snprintf(str,64,"; alpha %g",atof(alpha->value()));	strcat(result,str);}
	if(amb->value()[0])
	{	snprintf(str,64,"; ambient %g",atof(amb->value()));	strcat(result,str);}
	if(mesh->value()[0])
	{	snprintf(str,64,"; meshnum %g",atof(mesh->value()));strcat(result,str);}
	if(font->value()[0])
	{	snprintf(str,64,"; fontsize '%g'",atof(font->value()));	strcat(result,str);}
	if(cut->value()>=0)
	{snprintf(str,64,"; cut %s",cut->value()==0?"on":"off");	strcat(result,str);}
}
//-----------------------------------------------------------------------------
void option_cb(Fl_Widget *, void *v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	OptionDlg *s = &option_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// insert at the end of string
	{
		long i=e->editor->insert_position(), j=textbuf->line_end(i);
		s->FillResult();
		e->editor->insert_position(j);
		e->editor->insert(s->result);
	}
}
//-----------------------------------------------------------------------------
void option_in_cb(Fl_Widget *, void *v)
{
	Fl_Input* e = (Fl_Input*)v;
	OptionDlg *s = &option_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)
	{
		s->FillResult();
		e->value(s->result);
	}
}
//-----------------------------------------------------------------------------
Fl_Menu_Item arrows[] = {
	{("none")},	//_
	{("arrow")},	//A
	{("back arrow")},	//V
	{("stop")},	//I
	{("size")},	//K
	{("triangle")},	//T
	{("square")},	//S
	{("rhomb")},	//D
	{("circle")},	//O
	{0}};
//-----------------------------------------------------------------------------
Fl_Menu_Item dashing[] = {
	{("solid")},	//-
	{("dash")},	//|
	{("dash dot")},	//j
	{("small dash")},	//;
	{("small dash dot")},	//i
	{("dots")},	//:
	{("none")},	//
	{0}};
//-----------------------------------------------------------------------------
Fl_Menu_Item markers[] = {
	{("none")},	//
	{("circle")},	//o
	{("cross")},	//+
	{("skew cross")},	//x
	{("square")},	//s
	{("rhomb")},	//d
	{("point")},	//.
	{("triangle up")},	//^
	{("triangle down")},	//v
	{0}};
//-----------------------------------------------------------------------------
void style_set_cb(Fl_Widget *,void *)
{
	StyleDlg  *s = &style_dlg;
	Fl_Widget *t = s->tab->value();
	const char *cols = " wbgrcmylenuqphkWBGRCMYLENUQPH";
	char *r = s->result;
	long i=0,j;
	if(t==s->ltab)	// line style
	{
		const char *aa = "_AVIKTSDO", *dd="-|j;i: ", *mm="#o+xsd.^v";
		if(s->cl->value()>0)	r[i++]=cols[s->cl->value()];
		if(s->dash->value()>0)	r[i++]=dd[s->dash->value()];
		if(s->mark->value()>0)	r[i++]=mm[s->mark->value()];
		if(s->lw->value()>1 || s->lw->value()==0)
			r[i++] = '0'+int(0.1+s->lw->value());
		if(s->as->value()>0)
		{	r[i++]=aa[s->ae->value()];	r[i++]=aa[s->as->value()];	}
		else if(s->ae->value()>0)	r[i++]=aa[s->ae->value()];
	}
	else if(t==s->stab)	// surf style
	{
		for(j=0;j<7;j++)
		{
			if(s->c[j]->value()>0)	r[i++]=cols[s->c[j]->value()];
			else break;
		}
		if(s->d->value())	r[i++] = 'd';
		if(s->w->value())	r[i++] = '#';
		if(s->dir->value()>=0)	r[i++] = 'x'+s->dir->value();
		if(s->text->value()>0)	r[i++] = s->text->value()==1 ? 't':'T';
	}
	else if(t==s->ftab)	// text style
	{
		if(s->rm->value())	r[i++] = 'r';
		if(s->sc->value())	r[i++] = 's';
		if(s->it->value())	r[i++] = 'i';
		if(s->bf->value())	r[i++] = 'b';
		if(s->gt->value() && !s->rm->value())	r[i++] = 'g';
		if(s->rl->value())	r[i++] = 'L';
		else if(s->rc->value())	r[i++] = 'C';
		else if(s->rr->value())	r[i++] = 'R';
		if(s->cf->value()>0)
		{	r[i++]=':';	r[i++]=cols[s->cf->value()];	}
	}
	r[i]=0;
	s->res->value(r);
}
//-----------------------------------------------------------------------------
void style_rdo_cb(Fl_Widget *,void *v)
{
	StyleDlg  *s = &style_dlg;
	s->rl->value(0);	s->rc->value(0);	s->rr->value(0);
	((Fl_Round_Button *)v)->value(1);
	style_set_cb(0,0);
}
//-----------------------------------------------------------------------------
void StyleDlg::create_dlg()
{
	wnd = new Fl_Window(295, 337, gettext("String with line/surf/text style"));
	tab = new Fl_Tabs(0, 0, 295, 255);	tab->callback(style_set_cb);
	tab->box(UDAV_UP_BOX);

	ltab = new Fl_Group(0, 25, 295, 230, gettext("Line style"));
	as = new Fl_Choice(10, 50, 80, 25, gettext("Arrow at start"));
	as->align(FL_ALIGN_TOP);	as->copy(arrows);	as->callback(style_set_cb);
//	as->tooltip(gettext("Type of arrow at first point of line or curve"));
	dash = new Fl_Choice(110, 50, 80, 25, gettext("Dashing"));
	dash->align(FL_ALIGN_TOP);	dash->copy(dashing);dash->callback(style_set_cb);
//	dash->tooltip(gettext("Type dashing for line or curve"));
	ae = new Fl_Choice(210, 50, 80, 25, gettext("Arrow at end"));
	ae->align(FL_ALIGN_TOP);	ae->copy(arrows);	ae->callback(style_set_cb);
//	ae->tooltip(gettext("Type of arrow at last point of line or curve"));
	cl = new Fl_Choice(110, 85, 80, 25, gettext("Color"));	cl->copy(colors);
	cl->callback(style_set_cb);
	mark = new Fl_Choice(110, 120, 80, 25, gettext("Marks"));
	mark->copy(markers);	mark->callback(style_set_cb);
//	mark->tooltip(gettext("Type of marks at positions of data points"));
	lw = new Fl_Spinner(110, 155, 80, 25, gettext("Line width"));
	lw->range(0,9);	lw->step(1);	lw->callback(style_set_cb);
//	lw->tooltip(gettext("Relative width of line or curve"));
	ltab->end();

	stab = new Fl_Group(0, 25, 295, 230, gettext("Color scheme"));	stab->hide();
	c[0] = new Fl_Choice(15, 45, 75, 25, gettext("Color order"));
	c[0]->align(FL_ALIGN_TOP);	c[0]->copy(colors);	c[0]->callback(style_set_cb);
	c[1] = new Fl_Choice(15, 75, 75, 25);	c[1]->copy(colors);	c[1]->callback(style_set_cb);
	c[2] = new Fl_Choice(15, 105, 75, 25);	c[2]->copy(colors);	c[2]->callback(style_set_cb);
	c[3] = new Fl_Choice(15, 135, 75, 25);	c[3]->copy(colors);	c[3]->callback(style_set_cb);
	c[4] = new Fl_Choice(15, 165, 75, 25);	c[4]->copy(colors);	c[4]->callback(style_set_cb);
	c[5] = new Fl_Choice(15, 195, 75, 25);	c[5]->copy(colors);	c[5]->callback(style_set_cb);
	c[6] = new Fl_Choice(15, 225, 75, 25);	c[6]->copy(colors);	c[6]->callback(style_set_cb);
	d = new Fl_Check_Button(100, 45, 180, 25, gettext("Colors along coordinates"));
//	w->tooltip(gettext("Set face color proportional to its position"));
	d->callback(style_set_cb);
	w = new Fl_Check_Button(100, 75, 180, 25, gettext("Wire or mesh plot"));
	w->callback(style_set_cb);
//	w->tooltip(gettext("Switch to draw wire isosurface or set to draw mesh on surface"));
	dir = new Fl_Choice(210, 105, 75, 25, gettext("Axial direction"));
	dir->add("x");	dir->add("y");	dir->add("z");	dir->callback(style_set_cb);
	text = new Fl_Choice(210, 135, 75, 25, gettext("Text on contours"));
//	text->tooltip("Draw contour values near contour lines"));
	text->add(gettext("none"));	text->add(gettext("under"));
	text->add(gettext("above"));	text->callback(style_set_cb);
	stab->end();

	ftab = new Fl_Group(0, 25, 295, 230, gettext("Text style"));		ftab->hide();
	sc = new Fl_Check_Button(15, 40, 120, 25, gettext("Script font/style"));	sc->callback(style_set_cb);
	rm = new Fl_Check_Button(15, 70, 120, 25, gettext("Roman font"));	rm->callback(style_set_cb);
	gt = new Fl_Check_Button(15, 100, 120, 25, gettext("Gothic font"));	gt->callback(style_set_cb);
	it = new Fl_Check_Button(15, 130, 120, 25, gettext("Italic style"));	it->callback(style_set_cb);
	bf = new Fl_Check_Button(15, 160, 120, 25, gettext("Bold style"));	bf->callback(style_set_cb);
	cf = new Fl_Choice(200, 40, 80, 25, gettext("Text color"));	cf->copy(colors);cf->callback(style_set_cb);
	{ Fl_Box* o = new Fl_Box(160, 90, 120, 90, gettext("Alignment"));
		o->box(FL_DOWN_BOX);	o->align(FL_ALIGN_TOP);}
	rl = new Fl_Round_Button(170, 100, 100, 25, gettext("left"));		rl->callback(style_rdo_cb,rl);
	rc = new Fl_Round_Button(170, 125, 100, 25, gettext("at center"));	rc->callback(style_rdo_cb,rc);
	rr = new Fl_Round_Button(170, 150, 100, 25, gettext("right"));		rr->callback(style_rdo_cb,rr);
	ftab->end();

	tab->end();
	res = new Fl_Output(50, 265, 235, 25, gettext("Result"));
//	res->tooltip(gettext("Resulting string which will be used as argument of a command"));
	Fl_Button *o;
	o = new Fl_Button(125, 300, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(210, 300, 75, 25, gettext("OK"));	o->callback(style_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
void style_cb(Fl_Widget *, void *v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	StyleDlg *s = &style_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
	{
		int p,q;
		char str[20];
		snprintf(str,20,"'%s'",s->result);
		textbuf->selection_position(&p, &q);
		if(p==q)	e->editor->insert(str);
		else		textbuf->replace_selection(str);
	}
}
//-----------------------------------------------------------------------------
void style_in_cb(Fl_Widget *, void *v)
{
	Fl_Input* e = (Fl_Input*)v;
	StyleDlg *s = &style_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	e->value(s->result);
}
//-----------------------------------------------------------------------------
void font_cb(Fl_Widget *, void *v)
{
	Fl_Input* e = (Fl_Input *)v;
	StyleDlg *s = &style_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->tab->value(s->ftab);
	s->ltab->deactivate();
	s->stab->deactivate();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
		e->value(s->result);
	s->ltab->activate();
	s->stab->activate();
}
//-----------------------------------------------------------------------------
void line_cb(Fl_Widget *, void *v)
{
	Fl_Input* e = (Fl_Input *)v;
	StyleDlg *s = &style_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->tab->value(s->ltab);
	s->ftab->deactivate();
	s->stab->deactivate();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
		e->value(s->result);
	s->ftab->activate();
	s->stab->activate();
}
//-----------------------------------------------------------------------------
void face_cb(Fl_Widget *, void *v)
{
	Fl_Input* e = (Fl_Input *)v;
	StyleDlg *s = &style_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->tab->value(s->stab);
	s->ltab->deactivate();
	s->ftab->deactivate();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
		e->value(s->result);
	s->ltab->activate();
	s->ftab->activate();
}
//-----------------------------------------------------------------------------
