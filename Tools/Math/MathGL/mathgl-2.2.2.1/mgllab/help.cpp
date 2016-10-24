/* help.cpp is part of UDAV
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
#include "udav.h"
#include <ctype.h>
#include <FL/Fl_Select_Browser.H>
//-----------------------------------------------------------------------------
void help_cb(Fl_Widget*, void*v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	long i=e->editor->insert_position(), j0=textbuf->line_start(i),j;

	static char str[300];
	char s[32]="", *buf = textbuf->text();
	memset(s,0,32*sizeof(char));
	for(j=j0;!isspace(buf[j]) && buf[j]!='#' && buf[j]!=';' && j<31+j0;j++)
		s[j-j0] = buf[j];
	free(buf);
#ifdef WIN32
	snprintf(str,300,"%s\\mgl_en.html#%s",docdir,s);
#else
	snprintf(str,300,"%s/mgl_en.html#%s",docdir,s);
#endif
	e->hd->load(str);
	if(e->rtab)	e->rtab->value(e->ghelp);
}
//-----------------------------------------------------------------------------
void link_cb(Fl_Widget*, void*v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	static char str[300];
#ifdef WIN32
	snprintf(str,300,"%s\\mgl_en.html#%s",docdir,e->link_cmd->value());
#else
	snprintf(str,300,"%s/mgl_en.html#%s",docdir,e->link_cmd->value());
#endif
	e->hd->load(str);
	if(e->rtab)	e->rtab->value(e->ghelp);
}
//-----------------------------------------------------------------------------
void example_cb(Fl_Widget*, void*v)
{
	static char str[300];
	ScriptWindow* e = (ScriptWindow*)v;
#ifdef WIN32
	snprintf(str,300,"%s\\mgl_en.html\\mgl_en_2.html",docdir);
#else
	snprintf(str,300,"%s/mgl_en.html/mgl_en_2.html",docdir);
#endif
	e->hd->load(str);	e->rtab->value(e->ghelp);
	if(e->rtab)	e->rtab->value(e->ghelp);
}
//-----------------------------------------------------------------------------
void help_in_cb(Fl_Widget*, void*v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	e->hd->textsize(e->hd->textsize()+1);
}
//-----------------------------------------------------------------------------
void help_out_cb(Fl_Widget*, void*v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	e->hd->textsize(e->hd->textsize()-1);
}
//-----------------------------------------------------------------------------
#include "xpm/udav.xpm"
void about_cb(Fl_Widget*, void*)
{
	static char s[128];
	snprintf(s,128,gettext("UDAV v. 2.%g\n(c) Alexey Balakin, 2007\nhttp://udav.sf.net/"), MGL_VER2);
	Fl_Double_Window* w = new Fl_Double_Window(355, 130, "About UDAV");
	Fl_Box* o = new Fl_Box(10, 15, 65, 65);
	o->box(FL_UP_BOX);	o->color(55);	o->image(new Fl_Pixmap(udav_xpm));
	o = new Fl_Box(85, 15, 260, 65);	o->box(UDAV_DOWN_BOX);
	o->label(s);
	Fl_Button *b = new Fl_Return_Button(255, 90, 90, 30, "Close");
	b->callback(close_dlg_cb,w);
	b->box(UDAV_UP_BOX);	b->down_box(UDAV_DOWN_BOX);
	w->end();	w->set_modal();	w->show();
}
//-----------------------------------------------------------------------------
#include "xpm/zoom-out.xpm"
#include "xpm/zoom-in.xpm"
#include "xpm/help-faq.xpm"
Fl_Widget *add_help(ScriptWindow *w)
{
	Fl_Window *w1=new Fl_Window(300,30,630,430,0);
	Fl_Group *g = new Fl_Group(0,0,290,30);
	Fl_Button *o;

	w->link_cmd = new Fl_Input(0,1,150,25);
	w->link_cmd->when(FL_WHEN_CHANGED);
	w->link_cmd->callback(link_cb,w);

	o = new Fl_Button(155, 1, 25, 25);	o->tooltip(gettext("MGL samples and hints"));
	o->image(new Fl_Pixmap(help_faq_xpm));	o->callback(example_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(180, 1, 25, 25);	o->tooltip(gettext("Increase font size"));
	o->image(new Fl_Pixmap(zoom_in_xpm));	o->callback(help_in_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(205, 1, 25, 25);	o->tooltip(gettext("Decrease font size"));
	o->image(new Fl_Pixmap(zoom_out_xpm));	o->callback(help_out_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	g->end();	g->resizable(0);

	w->hd = new Fl_Help_View(0,28,630,400);
	w1->end();	link_cb(w,w);
	w1->resizable(w->hd);	return w1;
}
//-----------------------------------------------------------------------------
void mem_dlg_cb0(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_pressed(0);	}
//-----------------------------------------------------------------------------
void mem_dlg_cb1(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_pressed(1);	}
//-----------------------------------------------------------------------------
void mem_dlg_cb2(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_pressed(2);	}
//-----------------------------------------------------------------------------
void mem_dlg_cb3(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_pressed(3);	}
//-----------------------------------------------------------------------------
void mem_update_cb(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_init();	}
//-----------------------------------------------------------------------------
Fl_Widget *add_mem(ScriptWindow *w)
{
	static int widths[] = {220,205,0};
	Fl_Button *o;
	Fl_Box *b;
//	wnd = new Fl_Double_Window(335, 405, gettext("Data browser"));
	Fl_Window *wnd = new Fl_Window(300,30,630,430,0);

//	Fl_Group *g = new Fl_Group(10,10,610,395);
	b = new Fl_Box(0, 10, 630, 25, gettext("Existed data arrays"));	b->labeltype(FL_ENGRAVED_LABEL);
	b = new Fl_Box(0, 35, 220, 25, gettext("name"));
	b->box(FL_THIN_UP_BOX);	b->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);
	b = new Fl_Box(220, 35, 205, 25, gettext("dimensions"));
	b->box(FL_THIN_UP_BOX);	b->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);
	b = new Fl_Box(425, 35, 205, 25, gettext("mem. usage"));
	b->box(FL_THIN_UP_BOX);	b->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);

	w->var = new Fl_Select_Browser(0, 60, 630, 335);	w->var->column_char('\t');
	w->var->align(FL_ALIGN_TOP);	w->var->column_widths(widths);
	w->var->tooltip(gettext("List of available data."));
//	g->end();

	o = new Fl_Button(10, 400, 95, 25, gettext("Edit"));	o->callback(mem_dlg_cb0,w);
	o->tooltip(gettext("Open table with selected data for editing."));
	o = new Fl_Button(120, 400, 95, 25, gettext("Plot"));	o->callback(mem_dlg_cb1,w);
	o->tooltip(gettext("Plot selected data."));
	o = new Fl_Button(230, 400, 95, 25, gettext("Delete"));	o->callback(mem_dlg_cb2,w);
	o->tooltip(gettext("Delete selected data."));
	o = new Fl_Button(340, 400, 95, 25, gettext("New"));	o->callback(mem_dlg_cb3,w);
	o->tooltip(gettext("Open dialog for new data creation."));
	o = new Fl_Button(450, 400, 95, 25, gettext("Refresh"));	o->callback(mem_update_cb,w);
	o->tooltip(gettext("Refresh list of variables."));
//	o = new Fl_Button(120, 335, 95, 25, gettext("Load"));	o->callback(mem_dlg_cb,(void *)4);
//	o = new Fl_Button(230, 335, 95, 25, gettext("Save"));	o->callback(mem_dlg_cb,(void *)5);
//	o = new Fl_Button(10, 370, 95, 25, gettext("Update"));	o->callback(mem_upd_cb,0);
	wnd->end();	wnd->resizable(w->var);	return wnd;
}
//-----------------------------------------------------------------------------
void ScriptWindow::mem_init()
{
	char str[128];
	var->clear();
	mglVar *v=Parse->FindVar("");
	while(v)
	{
		snprintf(str,128,"%ls\t%ld*%ld*%ld\t%ld\t", v->s.c_str(), v->nx, v->ny, v->nz, sizeof(mreal)*v->nx*v->ny*v->nz);
		var->add(str,v);
		v = v->next;
	}
}
//-----------------------------------------------------------------------------
void ScriptWindow::mem_pressed(int kind)
{
	TableWindow *w;
	int ind = var->value();
	mglVar *v = (mglVar *)var->data(ind);
	static char res[128];
	if(!v && kind!=3)	return;
	if(kind==0)
	{
		w = (TableWindow *)v->o;
		if(!w)
		{
			char ss[1024];
			wcstombs(ss,v->s.c_str(),1024);	ss[v->s.length()]=0;
			ltab->begin();
			Fl_Group *gg = new Fl_Group(0,30,300,430);
			w = new TableWindow(0,30,300,430);
			gg->label(ss);	gg->end();	ltab->end();
		}
		w->update(v);	ltab->value(w->parent());	w->show();
	}
	else if(kind==1)
	{
		if(v->nz>1)		snprintf(res,128,"box\nsurf3 %ls\n",v->s.c_str());
		else if(v->ny>1)	snprintf(res,128,"box\nsurf %ls\n",v->s.c_str());
		else				snprintf(res,128,"box\nplot %ls\n",v->s.c_str());
		textbuf->text(res);
	}
	else if(kind==2)
		Parse->DeleteVar(v->s.c_str());
	else if(kind==3)
	{
		const char *name = fl_input(gettext("Enter name for new variable"),"dat");
		if(!name)	return;
		v = Parse->AddVar(name);

		ltab->begin();
		Fl_Group *gg = new Fl_Group(0,30,300,430);
		w = new TableWindow(0,30,300,430);
		gg->label(name);	gg->end();	ltab->end();
		w->update(v);	ltab->value(w->parent());	w->show();
	}
	mem_init();
}
//-----------------------------------------------------------------------------
void variables_cb(Fl_Widget *, void *v)
{
/*	MemDlg *s = &mem_dlg;
	s->wnd->set_modal();
	s->init();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();*/
}
//-----------------------------------------------------------------------------
