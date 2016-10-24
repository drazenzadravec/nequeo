/* setup.cpp is part of UDAV
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
#include "mgl2/mgl.h"
#include <string.h>
#include <FL/Fl_Tabs.H>
#include <FL/Fl_Round_Button.H>
#include <FL/Fl_Multiline_Input.H>
#include <FL/Fl_Spinner.H>
#include <locale.h>
#include "udav.h"
//-----------------------------------------------------------------------------
extern int auto_exec, plastic_scheme, internal_font;
//-----------------------------------------------------------------------------
void setup_dlg_cb(Fl_Widget *, void *v)
{	SetupDlg *s = (SetupDlg *)v;	s->OK = true;	s->wnd->hide();	}
//-----------------------------------------------------------------------------
void setup_sav_cb(Fl_Widget *, void *v)
{
	SetupDlg *e = (SetupDlg *)v;
	char *buf = e->ToScript();
	const char *fname;
	if(buf[0])
	{
		fname = e->templ->value();
		if(fname[0]==0)	fname = "template.mgl";
		FILE *fp = fopen(fname,"w");
		fputs(buf,fp);
		fclose(fp);
	}
	free(buf);
}
//-----------------------------------------------------------------------------
void close_dlg_cb(Fl_Widget *, void *v)
{	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void SetupDlg::CreateGen()
{
	xmin = new Fl_Input(105, 50, 75, 25);	xmax = new Fl_Input(105, 80, 75, 25);
	ymin = new Fl_Input(190, 50, 75, 25);	ymax = new Fl_Input(190, 80, 75, 25);
	zmin = new Fl_Input(275, 50, 75, 25);	zmax = new Fl_Input(275, 80, 75, 25);
	cmin = new Fl_Input(360, 50, 75, 25);	cmax = new Fl_Input(360, 80, 75, 25);

	xorg = new Fl_Input(105, 110, 75, 25);
	yorg = new Fl_Input(190, 110, 75, 25);
	zorg = new Fl_Input(275, 110, 75, 25);
	xlab = new Fl_Input(105, 140, 75, 25);
	ylab = new Fl_Input(190, 140, 75, 25);
	zlab = new Fl_Input(275, 140, 75, 25);

	xpos = new Fl_Choice(105, 170, 75, 25);	xpos->add(gettext("at minumum"));
		xpos->add("at center");	xpos->add(gettext("at maxumum"));	xpos->value(1);
	ypos = new Fl_Choice(190, 170, 75, 25);	ypos->add(gettext("at minumum"));
		ypos->add("at center");	ypos->add(gettext("at maxumum"));	ypos->value(1);
	zpos = new Fl_Choice(275, 170, 75, 25);	zpos->add(gettext("at minumum"));
		zpos->add("at center");	zpos->add(gettext("at maxumum"));	zpos->value(1);
	xtik = new Fl_Input(105, 200, 75, 25);
	ytik = new Fl_Input(190, 200, 75, 25);
	ztik = new Fl_Input(275, 200, 75, 25);
	xsub = new Fl_Input(105, 230, 75, 25);
	ysub = new Fl_Input(190, 230, 75, 25);
	zsub = new Fl_Input(275, 230, 75, 25);

	{ Fl_Box* o = new Fl_Box(10, 260, 470, 5);	o->box(FL_DOWN_BOX);	}
	alphad = new Fl_Input(20, 285, 75, 25, gettext("AlphaDef"));	alphad->align(FL_ALIGN_TOP);
	ambient = new Fl_Input(105, 285, 75, 25, gettext("Ambient"));	ambient->align(FL_ALIGN_TOP);
	basew = new Fl_Input(190, 285, 75, 25, gettext("Base Width"));	basew->align(FL_ALIGN_TOP);
	mesh = new Fl_Input(275, 285, 75, 25, gettext("Mesh Num"));		mesh->align(FL_ALIGN_TOP);
	axial = new Fl_Choice(360, 285, 75, 25, gettext("Axial Dir"));	axial->align(FL_ALIGN_TOP);
	axial->add("x");	axial->add("y");	axial->add("z");
	font = new Fl_Input(20, 330, 50, 25, gettext("Font"));			font->align(FL_ALIGN_TOP);
	{ Fl_Button* o = new Fl_Button(70, 330, 25, 25, "..");	o->callback(font_cb, font);
		o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);	}
	size = new Fl_Input(105, 330, 75, 25, gettext("Font Size"));	size->align(FL_ALIGN_TOP);
	alpha = new Fl_Check_Button(190, 330, 75, 25, gettext("Alpha on"));
	light = new Fl_Check_Button(275, 330, 75, 25, gettext("Light on"));
	rotate = new Fl_Check_Button(360, 330, 90, 25, gettext("Rotate text"));
}
//-----------------------------------------------------------------------------
void SetupDlg::CreateLid()
{
	const char *str[10]={"0:", "1:", "2:", "3:", "4:", "5:", "6:", "7:", "8:", "9:"};
	int h;
	for(long i=0;i<10;i++)
	{
		h = 55 + 30*i;
		new Fl_Box(10, h, 25, 25, str[i]);
		lid[i] = new Fl_Check_Button(35, h, 40, 25, gettext("on"));
		xid[i] = new Fl_Input(85, h, 75, 25);
		yid[i] = new Fl_Input(165, h, 75, 25);
		zid[i] = new Fl_Input(245, h, 75, 25);
		cid[i] = new Fl_Choice(325, h, 75, 25);
		cid[i]->copy(colors);
		bid[i] = new Fl_Input(405, h, 75, 25);
	}
}
//-----------------------------------------------------------------------------
void SetupDlg::CreateDlg()
{
	OK = false;
	wnd = new Fl_Window(490, 406, gettext("Setup graphics"));
	Fl_Tabs* t = new Fl_Tabs(0, 0, 490, 360);	t->box(UDAV_UP_BOX);

	Fl_Group *g = new Fl_Group(0, 25, 485, 330, gettext("General"));
	new Fl_Box(105, 30, 75, 20, gettext("X axis"));
	new Fl_Box(190, 30, 75, 20, gettext("Y axis"));
	new Fl_Box(275, 30, 75, 20, gettext("Z axis"));
	new Fl_Box(360, 30, 75, 20, gettext("Color"));

	new Fl_Box(25, 50, 75, 25, gettext("Minimal"));
	new Fl_Box(25, 80, 75, 25, gettext("Maximal"));
	new Fl_Box(25, 110, 75, 25, gettext("Origin"));
	new Fl_Box(25, 140, 75, 25, gettext("Label"));
	new Fl_Box(25, 170, 75, 25, gettext("Position"));
	new Fl_Box(25, 200, 75, 25, gettext("Ticks"));
	new Fl_Box(25, 230, 75, 25, gettext("SubTicks"));
	CreateGen();
	g->end();

	g = new Fl_Group(0, 25, 485, 330, gettext("Light"));	g->hide();
	new Fl_Box(10, 30, 25, 25, gettext("ID"));
	new Fl_Box(40, 30, 40, 25, gettext("State"));
	new Fl_Box(85, 30, 75, 25, gettext("X position"));
	new Fl_Box(165, 30, 75, 25, gettext("Y position"));
	new Fl_Box(245, 30, 75, 25, gettext("Z position"));
	new Fl_Box(325, 30, 75, 25, gettext("Color"));
	new Fl_Box(405, 30, 75, 25, gettext("Brightness"));
	CreateLid();
	g->end();

	g = new Fl_Group(0, 25, 485, 330, gettext("Setup code"));	g->hide();
	code = new Fl_Help_View(0, 25, 485, 330);
	g->end();

	t->end();	//Fl_Group::current()->resizable(t);
	Fl_Button *o;
	templ = new Fl_Input(120, 370, 110, 25, gettext("Template name"));
	templ->value("template.mgl");
	o = new Fl_Button(230, 370, 80, 25, gettext("Save"));	o->callback(setup_sav_cb, wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Save settings to file template.mgl.\nYou may use it later by 'call template.mgl'"));

	o = new Fl_Button(315, 370, 80, 25, gettext("Cancel"));	o->callback(close_dlg_cb, wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(400, 370, 80, 25, gettext("OK"));	o->callback(setup_dlg_cb, this);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
char *SetupDlg::ToScript()
{
	long i;
	double x1=0,y1=0,z1=0,x2=0,y2=0,z2=0;
	bool u1,v1,w1,u2,v2,w2;
	const char *cols = " wbgrcmylenuqphkWBGRCMYLENUQPH";
	char *buf = (char *)malloc(1024*sizeof(char)), str[128];
	long num = 1024, cur = 0;
	buf[0]=0;
	if(!OK)	return buf;

	for(i=0;i<10;i++)	// set light sources
	{
//fl_message("before %d lid:%d xid:%s yid:%s zid:%s",i,lid[i]->value(), xid[i]->value(),yid[i]->value(),zid[i]->value());
		if(!lid[i]->value())	continue;
		if(!xid[i]->value()[0] || !yid[i]->value()[0] || !zid[i]->value()[0])	continue;
		x1=atof(xid[i]->value());	y1=atof(yid[i]->value());	z1=atof(zid[i]->value());
		if(!bid[i]->value()[0])
			cur += snprintf(str,128,"light %ld %g %g %g '%c'\n",i,x1,y1,z1,
						cols[cid[i]->value()]);
		else
			cur += snprintf(str,128,"light %ld %g %g %g '%c' %g\n",i,x1,y1,z1,
						cols[cid[i]->value()],atof(bid[i]->value()));
		strcat(buf,str);
	}
	u1 = xmin->value()[0];	if(u1)	x1 = atof(xmin->value());
	u2 = xmax->value()[0];	if(u2)	x2 = atof(xmax->value());
	v1 = ymin->value()[0];	if(v1)	y1 = atof(ymin->value());
	v2 = ymin->value()[0];	if(v2)	y2 = atof(ymax->value());
	w1 = zmin->value()[0];	if(w1)	z1 = atof(zmin->value());
	w2 = zmin->value()[0];	if(w2)	z2 = atof(zmax->value());
	if(u1&&v1&&w1&&u2&&v2&&w2)
	{
		cur+=snprintf(str,128,"axis %g %g %g %g %g %g\n",x1,y1,z1,x2,y2,z2);
		strcat(buf,str);
	}
	else
	{
		if(u1 && u2)	{cur+=snprintf(str,128,"xrange %g %g\n",x1,x2);	strcat(buf,str);}
		if(v1 && v2)	{cur+=snprintf(str,128,"yrange %g %g\n",y1,y2);	strcat(buf,str);}
		if(w1 && w2)	{cur+=snprintf(str,128,"zrange %g %g\n",z1,z2);	strcat(buf,str);}
	}
	u1 = cmin->value()[0];	if(u1)	x1 = atof(cmin->value());
	u2 = cmax->value()[0];	if(u2)	x2 = atof(cmax->value());
	if(u1&&u2)	{cur+=snprintf(str,128,"crange %g %g\n",x1,x2);	strcat(buf,str);}
	if(cur>num-256)	{	num+=512;	buf = (char *)realloc(buf,num*sizeof(char));	}

	u1 = xorg->value()[0];	if(u1)	x1 = atof(xorg->value());
	v1 = yorg->value()[0];	if(v1)	y1 = atof(yorg->value());
	w1 = zorg->value()[0];	if(w1)	z1 = atof(zorg->value());
	if(u1&&v1&&w1)	{snprintf(str,128,"origin %g %g %g\n",x1,y1,z1);	strcat(buf,str);}

	u1 = xtik->value()[0];	if(u1)	x1 = atof(xtik->value());
	u2 = xsub->value()[0];	if(u2)	x2 = atoi(xsub->value());
	v1 = ytik->value()[0];	if(v1)	y1 = atof(ytik->value());
	v2 = ysub->value()[0];	if(v2)	y2 = atoi(ysub->value());
	w1 = ztik->value()[0];	if(w1)	z1 = atof(ztik->value());
	w2 = zsub->value()[0];	if(w2)	z2 = atoi(zsub->value());
	if(u1 && u2)	{cur+=snprintf(str,128,"xtick %g %g\n",x1,x2);	strcat(buf,str);}
	if(v1 && v2)	{cur+=snprintf(str,128,"ytick %g %g\n",y1,y2);	strcat(buf,str);}
	if(w1 && w2)	{cur+=snprintf(str,128,"ztick %g %g\n",z1,z2);	strcat(buf,str);}
	if(u1 && !u2)	{cur+=snprintf(str,128,"xtick %g\n",x1);	strcat(buf,str);}
	if(v1 && !v2)	{cur+=snprintf(str,128,"ytick %g\n",y1);	strcat(buf,str);}
	if(w1 && !w2)	{cur+=snprintf(str,128,"ztick %g\n",z1);	strcat(buf,str);}

	if(xlab->value()[0])
	{
		cur+=snprintf(str,128,"xlabel '%s' %d\n",xlab->value(), xpos->value()-1);
		strcat(buf,str);
	}
	if(ylab->value()[0])
	{
		cur+=snprintf(str,128,"ylabel '%s' %d\n",ylab->value(), ypos->value()-1);
		strcat(buf,str);
	}
	if(zlab->value()[0])
	{
		cur+=snprintf(str,128,"zlabel '%s' %d\n",zlab->value(), zpos->value()-1);
		strcat(buf,str);
	}
	if(alphad->value()[0])
	{
		cur+=snprintf(str,128,"alphadef %g\n",atof(alphad->value()));
		strcat(buf,str);
	}
	if(ambient->value()[0])
	{
		cur+=snprintf(str,128,"ambient %g\n",atof(ambient->value()));
		strcat(buf,str);
	}

	if(basew->value()[0])
	{
		cur+=snprintf(str,128,"baselinewidth %g\n",atof(basew->value()));
		strcat(buf,str);
	}
	if(mesh->value()[0])
	{
		cur+=snprintf(str,128,"meshnum %g\n",atof(mesh->value()));
		strcat(buf,str);
	}
	if(axial->value()>=0)
	{
		cur+=snprintf(str,128,"axialdir '%c'\n",'x'+axial->value());
		strcat(buf,str);
	}

	if(font->value()[0])
	{
		cur+=snprintf(str,128,"font '%s'",font->value());
		strcat(buf,str);
		if(size->value())	cur+=snprintf(str,128," %g\n",atof(size->value()));
		else	cur+=snprintf(str,128,"\n");
		strcat(buf,str);
	}
	if(rotate->value())	{cur+=snprintf(str,128,"rotatetext on\n");	strcat(buf,str);}

	if(alpha->value())	{cur+=snprintf(str,128,"alpha on\n");	strcat(buf,str);}
	if(light->value())	{cur+=snprintf(str,128,"light on\n");	strcat(buf,str);}

	code->value(buf);
	return buf;
}
//-----------------------------------------------------------------------------
void setup_cb(Fl_Widget *, void *d)
{
	if(d==0)	return;
	SetupDlg *s = ((ScriptWindow *)d)->setup_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	((ScriptWindow *)d)->graph->update();
}
//-----------------------------------------------------------------------------
Fl_Menu_Item colors[] = {
	{"-----", 0,0,0,0,0,0,0, 0},	//
	{("white"), 0,0,0,0,0,0,0, fl_rgb_color(0,0,0)},	//w
	{("blue"), 0,0,0,0,0,0,0, fl_rgb_color(0,0,255)},			//b
	{("lime"), 0,0,0,0,0,0,0, fl_rgb_color(0,255,0)},			//g
	{("red"), 0,0,0,0,0,0,0, fl_rgb_color(255,0,0)},			//r
	{("cyan"), 0,0,0,0,0,0,0, fl_rgb_color(0,255,255)},		//c
	{("magenta"), 0,0,0,0,0,0,0, fl_rgb_color(255,0,255)},	//m
	{("yellow"), 0,0,0,0,0,0,0, fl_rgb_color(255,255,0)},		//y
	{("springgreen"), 0,0,0,0,0,0,0, fl_rgb_color(0,255,127)},//l
	{("lawngreen"), 0,0,0,0,0,0,0, fl_rgb_color(127,255,0)},	//e
	{("skyblue"), 0,0,0,0,0,0,0, fl_rgb_color(0,127,255)},	//n
	{("blueviolet"), 0,0,0,0,0,0,0, fl_rgb_color(127,0,255)},	//u
	{("orange"), 0,0,0,0,0,0,0, fl_rgb_color(255,127,0)},		//q
	{("deeppink"), 0,0,0,0,0,0,0, fl_rgb_color(255,0,127)},	//p
	{("gray"), 0,0,0,0,0,0,0, fl_rgb_color(127,127,127)},		//h

	{("black"), 0,0,0,0,0,0,0, fl_rgb_color(0,0,0)},	//k
	{("lightgray"), 0,0,0,0,0,0,0, fl_rgb_color(179,179,179)},	//W
	{("navy"), 0,0,0,0,0,0,0, fl_rgb_color(0,0,127)},	//B
	{("green"), 0,0,0,0,0,0,0, fl_rgb_color(0,127,0)},	//G
	{("maroon"), 0,0,0,0,0,0,0, fl_rgb_color(127,0,0)},	//R
	{("teal"), 0,0,0,0,0,0,0, fl_rgb_color(0,127,127)},	//C
	{("purple"), 0,0,0,0,0,0,0, fl_rgb_color(127,0,127)},	//M
	{("olive"), 0,0,0,0,0,0,0, fl_rgb_color(127,127,0)},	//Y
	{("seagreen"), 0,0,0,0,0,0,0, fl_rgb_color(0,127,77)},	//L
	{("darklawn"), 0,0,0,0,0,0,0, fl_rgb_color(77,127,0)},	//E
	{("darkskyblue"), 0,0,0,0,0,0,0, fl_rgb_color(0,77,127)},	//N
	{("indigo"), 0,0,0,0,0,0,0, fl_rgb_color(77,0,127)},	//U
	{("brown"), 0,0,0,0,0,0,0, fl_rgb_color(127,77,0)},	//Q
	{("darkpink"), 0,0,0,0,0,0,0, fl_rgb_color(127,0,77)},	//P
	{("darkgray"), 0,0,0,0,0,0,0, fl_rgb_color(77,77,77)},	//H
{0, 0,0,0,0,0,0,0, 0}};
//-----------------------------------------------------------------------------
struct PropDlg
{
	Fl_Window *wnd;
	Fl_MGL *graph;

	Fl_Input *path, *locale, *font, *fpath;
	Fl_Check_Button *plast, *aexec, *ifont;
	PropDlg()	{	memset(this,0,sizeof(PropDlg));	create_dlg();	}
	~PropDlg()	{	delete wnd;	}
	void create_dlg();
	void finish();
	void init();
} prop_dlg;
//-----------------------------------------------------------------------------
void PropDlg::init()
{
	int a, p;
	char *buf;
	pref.get("plastic_scheme",p,1);	plast->value(p);
	pref.get("auto_exec",a,1);		aexec->value(a);
	pref.get("internal_font",a,0);	ifont->value(a);
	path->value(docdir);
	pref.get("font_dir",buf,"");	fpath->value(buf);	free(buf);
	pref.get("font_name",buf,"");	font->value(buf);	free(buf);
	pref.get("locale",buf,"ru_RU.cp1251");	locale->value(buf);	free(buf);
}
//-----------------------------------------------------------------------------
void PropDlg::finish()
{
	int a, p;
	p = plast->value();
	if(p!=plastic_scheme)
	{
		plastic_scheme = p;
		pref.set("plastic_scheme",p);
		Fl::scheme(p?"plastic":"none");
	}
	a = aexec->value();
	if(a!=auto_exec)
	{
		auto_exec = a;
		pref.set("auto_exec",a);
	}
	a = ifont->value();
	if(a!=internal_font)
	{
		internal_font = a;
		pref.set("internal_font",a);
	}
	if(path->value()[0])	pref.set("help_dir",path->value());
	if(locale->value()[0])
	{
		pref.set("locale", locale->value());
		setlocale(LC_CTYPE, locale->value());
	}
	pref.set("font_dir",fpath->value());
	pref.set("font_name",font->value());
	if(graph)	mgl_load_font(graph->FMGL->get_graph(), font->value(), fpath->value());
}
//-----------------------------------------------------------------------------
void prop_dlg_cb(Fl_Widget *, void *v)
{	prop_dlg.finish();	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void PropDlg::create_dlg()
{
	wnd = new Fl_Double_Window(320, 300, gettext("UDAV settings"));
	path = new Fl_Input(10, 25, 305, 25, gettext("Path for help files"));	path->align(FL_ALIGN_TOP_LEFT);

	font = new Fl_Input(10, 75, 305, 25, gettext("Font typeface"));	font->align(FL_ALIGN_TOP_LEFT);
	fpath = new Fl_Input(10, 125, 305, 25, gettext("Path for font files"));	fpath->align(FL_ALIGN_TOP_LEFT);
	locale = new Fl_Input(10, 175, 305, 25, gettext("Select locale"));	locale->align(FL_ALIGN_TOP_LEFT);

	plast = new Fl_Check_Button(10, 210, 210, 25, gettext("Use plastic scheme"));
	aexec = new Fl_Check_Button(10, 240, 210, 25, gettext("Execute after script loading"));
	ifont = new Fl_Check_Button(10, 270, 210, 25, gettext("Use only internal font"));

	Fl_Button *o;
	o = new Fl_Button(240, 210, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o = new Fl_Return_Button(240, 240, 75, 25, gettext("OK"));	o->callback(prop_dlg_cb,wnd);
	wnd->end();
}
//-----------------------------------------------------------------------------
void settings_cb(Fl_Widget *, void *v)
{
	PropDlg *s = &prop_dlg;
	s->graph = ((ScriptWindow *)v)->graph;
	s->init();
	s->wnd->set_modal();
	s->wnd->show();
}
//-----------------------------------------------------------------------------
