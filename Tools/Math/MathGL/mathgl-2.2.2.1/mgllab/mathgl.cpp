/* mathgl.cpp is part of UDAV
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
#include "udav.h"
//-----------------------------------------------------------------------------
#include "xpm/alpha.xpm"
#include "xpm/light.xpm"
#include "xpm/alpha_on.xpm"
#include "xpm/light_on.xpm"
#include "xpm/zoom-fit-best.xpm"
#include "xpm/zoom-fit-best-r.xpm"
#include "xpm/film-r.xpm"
#include "xpm/film-b.xpm"
#include "xpm/media-seek-forward.xpm"
#include "xpm/media-seek-backward.xpm"
#include "xpm/go-previous.xpm"
#include "xpm/go-next.xpm"
#include "xpm/go-down.xpm"
#include "xpm/zoom-out.xpm"
#include "xpm/zoom-in.xpm"
#include "xpm/go-up.xpm"
#include "xpm/zoom-original.xpm"
#include "xpm/view-refresh.xpm"
#include "xpm/rotate.xpm"
#include "xpm/rotate_on.xpm"
#include "xpm/document-properties.xpm"
//#include "xpm/preferences-system.xpm"
#include "xpm/wire.xpm"
//-----------------------------------------------------------------------------
extern int internal_font;
mglParse *Parse=0;
//-----------------------------------------------------------------------------
void udav_error(const char *Message, void *v)
{	((Fl_MGL*)v)->status->label(Message);	}
mreal udav_delay(void *v)
{	return ((Fl_MGL*)v)->AnimDelay;	}
void udav_reload(void *v)
{	Parse->RestoreOnce();	((Fl_MGL*)v)->update();	}
//-----------------------------------------------------------------------------
void udav_next(void *v)	{	((Fl_MGL*)v)->next_frame();	}
void Fl_MGL::next_frame()
{
	if(NArgs==0)
	{
		animate_cb(this,this);
		if(NArgs==0)	return;
	}
	ArgCur = (ArgCur+1) % NArgs;
	Parse->AddParam(0,Args[ArgCur]);
	update();
}
//-----------------------------------------------------------------------------
void udav_prev(void *v)	{	((Fl_MGL*)v)->prev_frame();	}
void Fl_MGL::prev_frame()
{
	if(NArgs==0)
	{
		animate_cb(this,this);
		if(NArgs==0)	return;
	}
	ArgCur = ArgCur>0 ? ArgCur-1 : NArgs-1;
	Parse->AddParam(0,Args[ArgCur]);
	update();
}
//-----------------------------------------------------------------------------
Fl_MGL::Fl_MGL(int x, int y, int w, int h, const char *label) : Fl_MGLView(x,y,w,h,label)
{
	if(!Parse)	Parse = new mglParse;
	Parse->AllowSetSize(true);
	ArgBuf = 0;	NArgs = ArgCur = 0;
	script = script_pre = 0;	par = this;
	next = udav_next;	delay = udav_delay;
	prev = udav_prev;	reload = udav_reload;
/*#ifdef WIN32
//	setlocale(LC_TYPE,"russian_Russia.CP1251");
	char *path;
	get_doc_dir(path);
	if(!FMGL->GetFont()->Load("STIX",path && path[0] ? path : "."))	FMGL->GetFont()->Restore();
	free(path);
#endif*/
}
//-----------------------------------------------------------------------------
Fl_MGL::~Fl_MGL()	{	clear_scripts();	if(ArgBuf)	delete []ArgBuf;	}
//-----------------------------------------------------------------------------
void Fl_MGL::clear_scripts()
{
	if(script)		free(script);
	if(script_pre)	free(script_pre);
}
//-----------------------------------------------------------------------------
void Fl_MGL::scripts(char *scr, char *pre)
{	clear_scripts();	script=scr;	script_pre=pre;	}
//-----------------------------------------------------------------------------
int Fl_MGL::Draw(mglGraph *gr)
{
	Parse->Execute(gr,script_pre);
	Parse->Execute(gr,script);
	status->label(gr->Message());
	return 0;
}
//-----------------------------------------------------------------------------
void Fl_MGL::update()
{
	// NOTE: hint for old style View(). May be I should remove it!
	if(!script || !strstr(script,"rotate"))	mgl_rotate(FMGL->get_graph(),0,0,0);

	Fl_MGLView::update();

	mglVar *v = Parse->FindVar("");
	while(v)
	{
		if(v->o)	((TableWindow *)v->o)->update(v);
		v = v->next;
	}
}
//-----------------------------------------------------------------------------
void add_suffix(char *fname, const char *ext)
{
	long n=strlen(fname);
	if(n>4 && fname[n-4]=='.')
	{	fname[n-3]=ext[0];	fname[n-2]=ext[1];	fname[n-1]=ext[2];	}
	else	{	strcat(fname,".");	strcat(fname,ext);	}

}
//-----------------------------------------------------------------------------
