/***************************************************************************
 * fltk.cpp is part of Math Graphic Library
 * Copyright (C) 2007-2014 Alexey Balakin <mathgl.abalakin@gmail.ru>       *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU Library General Public License as       *
 *   published by the Free Software Foundation; either version 3 of the    *
 *   License, or (at your option) any later version.                       *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU Library General Public     *
 *   License along with this program; if not, write to the                 *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
#include <FL/Fl_Pixmap.H>
#include <FL/fl_ask.H>
#include <FL/Fl_Double_Window.H>
#include <FL/fl_draw.H>
#include <FL/Fl_File_Chooser.H>
//#include <unistd.h>
//-----------------------------------------------------------------------------
#ifdef USE_GETTEXT
#include <libintl.h>
#else
// Workaround for gcc 4.2
#ifndef _LIBINTL_H
#define gettext(x)	(x)
#endif
#endif
//-----------------------------------------------------------------------------
#include "mgl2/canvas_wnd.h"
#include "mgl2/fltk.h"
//-----------------------------------------------------------------------------
#include "xpm/alpha_on.xpm"
#include "xpm/light_on.xpm"
#include "xpm/zoom_on.xpm"
#include "xpm/show_on.xpm"
#include "xpm/rotate_on.xpm"
#include "xpm/show_sl.xpm"
#include "xpm/next_sl.xpm"
#include "xpm/prev_sl.xpm"
#include "xpm/left_1.xpm"
#include "xpm/right_1.xpm"
#include "xpm/down_1.xpm"
#include "xpm/norm_1.xpm"
#include "xpm/zoom_1.xpm"
#include "xpm/up_1.xpm"
#include "xpm/alpha.xpm"
#include "xpm/light.xpm"
#include "xpm/zoom_in.xpm"
#include "xpm/zoom_out.xpm"
#include "xpm/rotate.xpm"
#include "xpm/ok.xpm"
#include "xpm/wire.xpm"
//-----------------------------------------------------------------------------
Fl_Pixmap xpm_a1(alpha_xpm), xpm_a2(alpha_on_xpm);
Fl_Pixmap xpm_l1(light_on_xpm), xpm_l2(light_xpm);
Fl_Pixmap xpm_z1(zoom_in_xpm), xpm_z2(zoom_on_xpm);
Fl_Pixmap xpm_s1(show_sl_xpm), xpm_s2(show_on_xpm);
Fl_Pixmap xpm_r1(rotate_xpm), xpm_r2(rotate_on_xpm);
Fl_Pixmap xpm_wire(wire_xpm);
//-----------------------------------------------------------------------------
/// Class allows the window creation for displaying plot bitmap with the help of FLTK library
/** NOTE!!! All frames are saved in memory. So animation with many frames require a lot memory and CPU time (for example, for mouse rotation).*/
class mglCanvasFL : public mglCanvasWnd
{
public:
using mglCanvasWnd::Window;
	Fl_Window *Wnd;		///< Pointer to window
	Fl_MGLView *mgl;	///< Pointer to MGL widget with buttons

	mglCanvasFL();
	~mglCanvasFL();

	/// Create a window for plotting. Now implemeted only for GLUT.
	void Window(int argc, char **argv, int (*draw)(mglBase *gr, void *p), const char *title,
						void *par=NULL, void (*reload)(void *p)=NULL, bool maximize=false);
	/// Switch on/off transparency (do not overwrite switches in user drawing function)
	void ToggleAlpha();
	/// Switch on/off lighting (do not overwrite switches in user drawing function)
	void ToggleLight();
	void ToggleRotate();	///< Switch on/off rotation by mouse
	void ToggleZoom();		///< Switch on/off zooming by mouse
	void ToggleNo();		///< Switch off all zooming and rotation
	void Update();			///< Update picture by calling user drawing function
	void Adjust();			///< Adjust size of bitmap to window size
	void GotoFrame(int d);	///< Show arbitrary frame (use relative step)
	void Animation();	///< Run animation (I'm too lasy to change it)
};
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_ask_fltk(const wchar_t *quest, wchar_t *res)
{
	static char buf[1024];	*res=0;
#if FL_MINOR_VERSION>=3
	fl_utf8fromwc(buf, 1024, quest, mgl_wcslen(quest)+1);
	const char *str = fl_input("%s",buf,"");
	if(str)	fl_utf8towc(str, strlen(str)+1, res, 1024);
#else
	wcstombs(buf,quest,mgl_wcslen(quest)+1);
	const char *str = fl_input("%s",buf,"");
	MGL_TO_WCS(str,wcscpy(res,str));
#endif
}
//-----------------------------------------------------------------------------
//
//		class Fl_MathGL
//
//-----------------------------------------------------------------------------
Fl_MathGL::Fl_MathGL(int xx, int yy, int ww, int hh, const char *lbl) : Fl_Widget(xx,yy,ww,hh,lbl)
{
	gr = new mglCanvas;
	tet=phi=x1=y1=0;	x2=y2=1;
	zoom = rotate = false;
	flag=x0=y0=xe=ye=0;
	tet_val = phi_val = 0;
	draw_par = 0;	draw_func = 0;	draw_cl = 0;
}
//-----------------------------------------------------------------------------
Fl_MathGL::~Fl_MathGL()	{	if(mgl_use_graph(gr,-1)<1)	mgl_delete_graph(gr);	}
//-----------------------------------------------------------------------------
void Fl_MathGL::set_graph(HMGL GR)
{
	mglCanvas *gg = dynamic_cast<mglCanvas *>(GR);
	if(!gg)	return;
	if(mgl_use_graph(gr,-1)<1)	mgl_delete_graph(gr);
	gr=gg;	mgl_use_graph(gg,1);
}
//-----------------------------------------------------------------------------
void Fl_MathGL::draw()
{
	// TODO: add active points drawing here (from Qt)
	const unsigned char *g = mgl_get_rgb(gr);
	int i, ww=mgl_get_width(gr), hh=mgl_get_height(gr);
	if(g)	fl_draw_image(g, x(), y(), ww, hh, 3);
	if(flag&4)
	{
		char str[5]="0.0";
		fl_color(192,192,192);
		for(i=1;i<10;i++)
		{
			str[2] = '0'+10-i;	fl_draw(str,30,30+i*hh/10);
			fl_line(30,30+i*hh/10,30+ww,30+i*hh/10);
			str[2] = '0'+i;	fl_draw(str,30+i*ww/10,30+hh);
			fl_line(30+i*ww/10,30,30+i*ww/10,30+hh);
		}
//		if(*MouseBuf)	fl_draw(MouseBuf,30,50);
	}
}
//-----------------------------------------------------------------------------
void Fl_MathGL::update()
{
	if(draw_func || draw_cl)
	{
		mgl_reset_frames(gr);
		if(mgl_get_flag(gr,MGL_CLF_ON_UPD))	mgl_set_def_param(gr);
		mgl_set_alpha(gr,flag&1);	mgl_set_light(gr,flag&2);
		if(tet_val)	tet = tet_val->value();
		if(phi_val)	phi = phi_val->value();
		mgl_zoom(gr,x1,y1,x2,y2);	mgl_view(gr,phi,0,tet);
		setlocale(LC_NUMERIC, "C");
		// use frames for quickly redrawing while adding/changing primitives
		if(mgl_is_frames(gr))	mgl_new_frame(gr);
		if(draw_func)	draw_func(gr, draw_par);	// drawing itself
		else	if(draw_cl)	{	mglGraph g(gr);	draw_cl->Draw(&g);	}
		if(mgl_is_frames(gr))	mgl_end_frame(gr);
		setlocale(LC_NUMERIC, "");
		const char *buf = mgl_get_mess(gr);
		if(*buf)	fl_message("%s",buf);
	}
	if(mgl_get_width(gr)!=w() || mgl_get_height(gr)!=h())
		size(mgl_get_width(gr), mgl_get_height(gr));
	redraw();	Fl::flush();
}
//-----------------------------------------------------------------------------
void Fl_MathGL::resize(int xx, int yy, int ww, int hh)
{	Fl_Widget::resize(xx,yy,ww,hh);	}
//-----------------------------------------------------------------------------
int Fl_MathGL::handle(int code)
{
	if(popup && code==FL_PUSH && Fl::event_button()==FL_RIGHT_MOUSE)
	{
		const Fl_Menu_Item *m = popup->popup(Fl::event_x(), Fl::event_y(), 0, 0, 0);
		if(m)	m->do_callback(wpar, vpar);
	}
	else if(!zoom && !rotate && code==FL_PUSH && Fl::event_button()==FL_LEFT_MOUSE)
	{
		mglCanvasWnd *g=dynamic_cast<mglCanvasWnd *>(gr);
		if(g && g->ClickFunc)	g->ClickFunc(draw_par);
		if(mgl_get_flag(gr,MGL_SHOW_POS))
		{
			mglPoint p = gr->CalcXYZ(Fl::event_x()-x(), Fl::event_y()-y());
			if(g)	g->LastMousePos = p;
			char s[128];
			snprintf(s,128,"x=%g, y=%g, z=%g",p.x,p.y,p.z);
			draw();	fl_color(FL_BLACK);		fl_draw(s,40,70);
		}
	}
	else if((!rotate && !zoom) || Fl::event_button()!=FL_LEFT_MOUSE)
	{
		if(code==FL_FOCUS || code==FL_UNFOCUS)	return 1;
		if(code==FL_KEYUP)
		{
			int key=Fl::event_key();
			if(!strchr(" .,wasdrfx",key))	return 0;
			if(key==' ')	{	update();	return 1;	}
			if(key=='w')
			{
				tet += 10;
				if(tet_val)	tet_val->value(tet);
				update();	return 1;
			}
			if(key=='s')
			{
				tet -= 10;
				if(tet_val)	tet_val->value(tet);
				update();	return 1;
			}
			if(key=='a')
			{
				phi += 10;
				if(phi_val)	phi_val->value(phi);
				update();	return 1;
			}
			if(key=='d')
			{
				phi -= 10;
				if(phi_val)	phi_val->value(phi);
				update();	return 1;
			}
			if(key=='x')
			{
				mglCanvasFL *g=dynamic_cast<mglCanvasFL *>(gr);
				if(g && g->mgl->FMGL==this)
				{	g->Wnd->hide();	return 1;	}
				else	return 0;
//				exit(0);
			}
			if(key==',')
			{
				mglCanvasFL *g=dynamic_cast<mglCanvasFL *>(gr);
				if(g && g->mgl->FMGL==this)
				{	g->PrevFrame();	return 1;	}
				else	return 0;
			}
			if(key=='.')
			{
				mglCanvasFL *g=dynamic_cast<mglCanvasFL *>(gr);
				if(g && g->mgl->FMGL==this)
				{	g->NextFrame();	return 1;	}
				else	return 0;
			}
			if(key=='r')
			{	flag = (flag&2) + ((~(flag&1))&1);	update();	return 1;	}
			if(key=='f')
			{	flag = (flag&1) + ((~(flag&2))&2);	update();	return 1;	}
		}
		return 0;
	}
	else if(code==FL_PUSH)	{	xe=x0=Fl::event_x();	ye=y0=Fl::event_y();	}
	else if(code==FL_DRAG)
	{
		xe=Fl::event_x();	ye=Fl::event_y();
		mreal ff = 240./sqrt(mreal(w()*h()));
		if(rotate)
		{
			phi += (x0-xe)*ff;
			tet += (y0-ye)*ff;
			if(phi>180)	phi-=360;		if(phi<-180)	phi+=360;
			if(tet>180)	tet-=360;		if(tet<-180)	tet+=360;
			if(tet_val)	tet_val->value(tet);
			if(phi_val)	phi_val->value(phi);
			x0 = xe;	y0 = ye;
			update();
		}
		redraw();
	}
	else if(code==FL_RELEASE)
	{
		if(zoom)
		{
			int w1=w(),h1=h();
			mreal _x1,_x2,_y1,_y2;
			_x1 = x1+(x2-x1)*(x0-x())/mreal(w1);
			_y1 = y2-(y2-y1)*(ye-y())/mreal(h1);
			_x2 = x1+(x2-x1)*(xe-x())/mreal(w1);
			_y2 = y2-(y2-y1)*(y0-y())/mreal(h1);
			x1=_x1;		x2=_x2;		y1=_y1;		y2=_y2;
			if(x1>x2)	{	_x1=x1;	x1=x2;	x2=_x1;	}
			if(y1>y2)	{	_x1=y1;	y1=y2;	y2=_x1;	}
			update();
		}
		else
		{
			if(tet_val)	tet_val->value(tet);
			if(phi_val)	phi_val->value(phi);
		}
		redraw();
	}
	return 1;
}
//-----------------------------------------------------------------------------
//
//		class Fl_MGLView
//
//-----------------------------------------------------------------------------
void Fl_MGLView::toggle(int &val, Fl_Button *b, const char *txt)
{
	val = 1-val;	b->value(val);
	if(menu && txt && *txt)
	{
		Fl_Menu_Item *m = (Fl_Menu_Item *)menu->find_item(gettext(txt));
		if(m && val)	m->set();
		if(m && !val)	m->clear();
	}
	update();
}
//-----------------------------------------------------------------------------
void Fl_MGLView::setoff(int &val, Fl_Button *b, const char *txt)
{
	val = 0;	b->value(val);
	if(menu && txt && *txt)
	{
		Fl_Menu_Item *m = (Fl_Menu_Item *)menu->find_item(gettext(txt));
		if(m && val)	m->set();
		if(m && !val)	m->clear();
	}
	//update();
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_grid_cb(Fl_Widget*, void* v)
{	if(v)	((Fl_MGLView*)v)->toggle_grid();	}
//-------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_alpha_cb(Fl_Widget*, void* v)	// alpha?xpm_a2:xpm_a1
{	if(v)	((Fl_MGLView*)v)->toggle_alpha();	}
void mglCanvasFL::ToggleAlpha()	{	Fl::lock();	mgl->toggle_alpha();	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_light_cb(Fl_Widget*, void* v)	// light?xpm_l2:xpm_l1
{	if(v)	((Fl_MGLView*)v)->toggle_light();	}
void mglCanvasFL::ToggleLight()	{	Fl::lock();	mgl->toggle_light();	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_norm_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	e->setoff_rotate();			e->setoff_zoom();
	e->FMGL->tet_val->value(0);	e->FMGL->phi_val->value(0);
	e->FMGL->set_zoom(0,0,1,1);
	e->update();
}
void mglCanvasFL::ToggleNo()	{	Fl::lock();	mgl_norm_cb(0,mgl);	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_zoom_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	e->setoff_rotate();	e->toggle_zoom();
}
void mglCanvasFL::ToggleZoom()	{	Fl::lock();	mgl_zoom_cb(0,mgl);	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_rotate_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	e->setoff_zoom();	e->toggle_rotate();
}
void mglCanvasFL::ToggleRotate()	{	Fl::lock();	mgl_rotate_cb(0,mgl);	Fl::unlock();	}
//-----------------------------------------------------------------------------
void Fl_MGLView::update()
{
	FMGL->set_state(zoom_bt->value(), rotate_bt->value());
	FMGL->set_flag(alpha + 2*light + 4*grid);
	FMGL->update();
}
void MGL_NO_EXPORT mgl_draw_cb(Fl_Widget*, void* v)
{	if(v)	((Fl_MGLView*)v)->update();	}
void mglCanvasFL::Update()		{	Fl::lock();	mgl->update();	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_png_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.png", 0);
	if(!fname || !fname[0])	return;
	mgl_write_png(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_bps_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.eps", 0);
	if(!fname || !fname[0])	return;
	mgl_write_bps(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_pngn_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.png", 0);
	if(!fname || !fname[0])	return;
	mgl_write_png_solid(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_jpeg_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.jpg", 0);
	if(!fname || !fname[0])	return;
	mgl_write_jpg(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_svg_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.svg", 0);
	if(!fname || !fname[0])	return;
	mgl_write_svg(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_eps_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.eps", 0);
	if(!fname || !fname[0])	return;
	mgl_write_eps(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_prc_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.prc", 0);
	if(!fname || !fname[0])	return;
	mgl_write_prc(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0,1);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_tex_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.tex", 0);
	if(!fname || !fname[0])	return;
	mgl_write_tex(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_obj_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.obj", 0);
	if(!fname || !fname[0])	return;
	mgl_write_obj(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0,1);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_off_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.off", 0);
	if(!fname || !fname[0])	return;
	mgl_write_off(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_export_stl_cb(Fl_Widget*, void* v)
{
	char *fname = fl_file_chooser(gettext("Save File As?"), "*.stl", 0);
	if(!fname || !fname[0])	return;
	mgl_write_stl(((Fl_MGLView*)v)->FMGL->get_graph(),fname,0);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_su_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (y2-y1)/3;	y1 -= d;	y2 -= d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sd_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (y2-y1)/3;	y1 += d;	y2 += d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sr_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (x2-x1)/3;	x1 -= d;	x2 -= d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sl_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (x2-x1)/3;	x1 += d;	x2 += d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sz_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (y2-y1)/4;	y1 += d;	y2 -= d;
	d = (x2-x1)/4;	x1 += d;	x2 -= d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_so_cb(Fl_Widget*, void* v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;	if(!e)	return;
	mreal x1,x2,y1,y2,d;
	e->FMGL->get_zoom(&x1,&y1,&x2,&y2);
	d = (y2-y1)/2;	y1 -= d;	y2 += d;
	d = (x2-x1)/2;	x1 -= d;	x2 += d;
	e->FMGL->set_zoom(x1,y1,x2,y2);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_adjust_cb(Fl_Widget*, void*v)
{	Fl_MGLView *e = (Fl_MGLView*)v;	if(e)	e->adjust();	}
void mglCanvasFL::Adjust()	{	Fl::lock();	mgl_adjust_cb(0,mgl);	Fl::unlock();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_oncemore_cb(Fl_Widget*, void*v)
{	Fl_MGLView *e = (Fl_MGLView*)v;	if(e && e->reload)	e->reload(e->par);	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_quit_cb(Fl_Widget*, void*)	{	Fl::first_window()->hide();	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_snext_cb(Fl_Widget*, void* v)
{	Fl_MGLView *e = (Fl_MGLView*)v;	if(e && e->next)	e->next(e->par);	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sprev_cb(Fl_Widget*, void* v)
{	Fl_MGLView *e = (Fl_MGLView*)v;	if(e && e->prev)	e->prev(e->par);	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_time_cb(void *v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;
	if(!e || !e->is_sshow() || !e->next || !e->delay)	return;
	e->next(e->par);
	Fl::repeat_timeout(e->delay(e->par), mgl_time_cb, v);
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_sshow_cb(Fl_Widget *, void *v)
{
	Fl_MGLView *e = (Fl_MGLView*)v;
	if(!e || !e->delay || !e->next)	return;
	e->toggle_sshow();
	if(e->is_sshow())	Fl::add_timeout(e->delay(e->par), mgl_time_cb, v);
}
void mglCanvasFL::Animation()	{	Fl::lock();	mgl_sshow_cb(0,mgl);	Fl::unlock();	}
void MGL_NO_EXPORT mgl_no_cb(Fl_Widget *, void *)	{}
//-----------------------------------------------------------------------------
Fl_Menu_Item pop_graph[20] = {
	{ gettext("Export"), 0, mgl_no_cb, 0, FL_SUBMENU,0,0,0,0},
		{ gettext("... as PNG"),	0, mgl_export_png_cb,0,0,0,0,0,0 },
		{ gettext("... as PNG (solid)"),	0, mgl_export_pngn_cb,0,0,0,0,0,0 },
		{ gettext("... as JPEG"),	0, mgl_export_jpeg_cb,0,0,0,0,0,0 },
		{ gettext("... as SVG"),	0, mgl_export_svg_cb,0,0,0,0,0,0 },
		{ gettext("... as vector EPS"),	0, mgl_export_eps_cb,0,0,0,0,0,0 },
		{ gettext("... as bitmap EPS"),	0, mgl_export_bps_cb, 0, FL_MENU_DIVIDER,0,0,0,0 },
		{ gettext("... as TeX"),	0, mgl_export_tex_cb,0,0,0,0,0,0 },
		{ gettext("... as OBJ"),	0, mgl_export_obj_cb,0,0,0,0,0,0 },
		{ gettext("... as PRC"),	0, mgl_export_prc_cb,0,0,0,0,0,0 },
		{ gettext("... as OFF"),	0, mgl_export_off_cb,0,0,0,0,0,0 },
		{ gettext("... as STL"),	0, mgl_export_stl_cb,0,0,0,0,0,0 },
		{ 0,0,0,0,0,0,0,0,0 },
	{ gettext("Copy graphics"),	0, 0, 0, FL_MENU_INACTIVE|FL_MENU_DIVIDER,0,0,0,0},
	{ gettext("Normal view"),	0, mgl_norm_cb,0,0,0,0,0,0 },
	{ gettext("Redraw plot"),	0, mgl_draw_cb,0,0,0,0,0,0 },
	{ gettext("Adjust size"),	0, mgl_adjust_cb,0,0,0,0,0,0 },
	{ gettext("Reload data"),	0, mgl_oncemore_cb,0,0,0,0,0,0 },
	{ 0,0,0,0,0,0,0,0,0 }
};
//-----------------------------------------------------------------------------
Fl_MGLView::Fl_MGLView(int xx, int yy, int ww, int hh, const char *lbl) : Fl_Window(xx,yy,ww,hh,lbl)
{
	grid = alpha = light = sshow = 0;	menu = 0;
	next = prev = reload = NULL;	delay = NULL;

	Fl_Button *o;
	Fl_Group *g = new Fl_Group(0,30,435,30);

	alpha_bt = new Fl_Button(0, 1, 25, 25);	alpha_bt->type(FL_TOGGLE_BUTTON);
	alpha_bt->image(xpm_a1);	alpha_bt->callback(mgl_alpha_cb,this);
	alpha_bt->tooltip(gettext("Switch on/off transparency in the picture"));
//	alpha_bt->box(FL_PLASTIC_UP_BOX);		alpha_bt->down_box(FL_PLASTIC_DOWN_BOX);
	light_bt = new Fl_Button(25, 1, 25, 25);	light_bt->type(FL_TOGGLE_BUTTON);
	light_bt->image(xpm_l1);	light_bt->callback(mgl_light_cb,this);
	light_bt->tooltip(gettext("Switch on/off lightning in the picture"));
//	light_bt->box(FL_PLASTIC_UP_BOX);		light_bt->down_box(FL_PLASTIC_DOWN_BOX);
	grid_bt = new Fl_Button(50, 1, 25, 25);	grid_bt->type(FL_TOGGLE_BUTTON);
	grid_bt->image(xpm_wire);	grid_bt->callback(mgl_grid_cb,this);
	grid_bt->tooltip(gettext("Switch on/off grid drawing"));
	//	grid_bt->box(FL_PLASTIC_UP_BOX);		grid_bt->down_box(FL_PLASTIC_DOWN_BOX);

	rotate_bt = new Fl_Button(80, 1, 25, 25);rotate_bt->type(FL_TOGGLE_BUTTON);
	rotate_bt->image(xpm_r1);	rotate_bt->callback(mgl_rotate_cb,this);
	rotate_bt->tooltip(gettext("Rotate picture by holding left mouse button"));
//	rotate_bt->box(FL_PLASTIC_UP_BOX);		rotate_bt->down_box(FL_PLASTIC_DOWN_BOX);
	zoom_bt = new Fl_Button(105, 1, 25, 25);	zoom_bt->type(FL_TOGGLE_BUTTON);
	zoom_bt->image(xpm_z1);	zoom_bt->callback(mgl_zoom_cb,this);
	zoom_bt->tooltip(gettext("Zoom in selected region of the picture"));
//	zoom_bt->box(FL_PLASTIC_UP_BOX);			zoom_bt->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(130, 1, 25, 25);		o->tooltip(gettext("Return picture to normal zoom"));
	o->image(new Fl_Pixmap(zoom_out_xpm));	o->callback(mgl_norm_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(160, 1, 25, 25);	o->tooltip(gettext("Refresh the picture"));
	o->image(new Fl_Pixmap(ok_xpm));	o->callback(mgl_draw_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	Fl_Counter *tet, *phi;
	tet = new Fl_Counter(195, 1, 90, 25, 0);	tet->callback(mgl_draw_cb,this);
	phi = new Fl_Counter(290, 1, 90, 25, 0);	phi->callback(mgl_draw_cb,this);
	tet->lstep(10);	tet->step(1);	tet->range(-180,180);
	tet->tooltip(gettext("Theta angle (tilt z-axis)"));
	phi->lstep(10);	phi->step(1);	phi->range(-180,180);
	phi->tooltip(gettext("Phi angle (rotate in x*y plane)"));
//	tet->box(FL_PLASTIC_UP_BOX);	phi->box(FL_PLASTIC_UP_BOX);
	g->end();	g->resizable(0);

	g = new Fl_Group(0,30,30,260);
	o = new Fl_Button(1, 30, 25, 25);		o->tooltip(gettext("Shift the picture up"));
	o->image(new Fl_Pixmap(up_1_xpm));		o->callback(mgl_su_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 55, 25, 25);		o->tooltip(gettext("Shift the picture left"));
	o->image(new Fl_Pixmap(left_1_xpm));	o->callback(mgl_sl_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 80, 25, 25);		o->tooltip(gettext("Zoom in the picture"));
	o->image(new Fl_Pixmap(zoom_1_xpm));	o->callback(mgl_sz_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 105, 25, 25);		o->tooltip(gettext("Zoom out the picture"));
	o->image(new Fl_Pixmap(norm_1_xpm));	o->callback(mgl_so_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 130, 25, 25);		o->tooltip(gettext("Shift the picture right"));
	o->image(new Fl_Pixmap(right_1_xpm));	o->callback(mgl_sr_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 155, 25, 25);		o->tooltip(gettext("Shift the picture down"));
	o->image(new Fl_Pixmap(down_1_xpm));	o->callback(mgl_sd_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(1, 185, 25, 25);		o->tooltip(gettext("Show previous frame in slideshow"));
	o->image(new Fl_Pixmap(prev_sl_xpm));	o->callback(mgl_sprev_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	anim_bt = new Fl_Button(1, 210, 25, 25);	anim_bt->type(FL_TOGGLE_BUTTON);
	anim_bt->image(xpm_s1);	anim_bt->callback(mgl_sshow_cb,this);
	anim_bt->tooltip(gettext("Run/Stop slideshow (graphics animation)"));
//	anim_bt->box(FL_PLASTIC_UP_BOX);		anim_bt->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(1, 235, 25, 25);		o->tooltip(gettext("Show next frame in slideshow"));
	o->image(new Fl_Pixmap(next_sl_xpm));	o->callback(mgl_snext_cb,this);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	g->end();	g->resizable(0);

	scroll = new Fl_Scroll(30, 30, 800, 600);
	//scroll->begin();
	FMGL = new Fl_MathGL(30, 30, 800, 600);
	FMGL->tet_val = tet;
	FMGL->phi_val = phi;
	FMGL->set_popup(pop_graph,FMGL,this);
	scroll->end();
	resizable(scroll);	end();
}
Fl_MGLView::~Fl_MGLView()	{}
//-----------------------------------------------------------------------------
//
//		class mglCanvasFL
//
//-----------------------------------------------------------------------------
mglCanvasFL::mglCanvasFL() : mglCanvasWnd()	{	Wnd=0;	}
mglCanvasFL::~mglCanvasFL()		{	if(Wnd)	delete Wnd;	}
//-----------------------------------------------------------------------------
void mglCanvasFL::GotoFrame(int d)
{
	int f = GetCurFig()+d;
	if(f>=GetNumFig())	f = 0;
	if(f<0)	f = GetNumFig()-1;
	if(GetNumFig()>0 && d)	{	SetCurFig(f);	mgl->FMGL->redraw();	}
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_fl_next(void *v)	{	((mglCanvasWnd*)v)->NextFrame();	}	///< Callback function for next frame
void MGL_NO_EXPORT mgl_fl_prev(void *v)	{	((mglCanvasWnd*)v)->PrevFrame();	}	///< Callback function for prev frame
void MGL_NO_EXPORT mgl_fl_reload(void *v)	{	((mglCanvasWnd*)v)->ReLoad();	}		///< Callback function for reloading
mreal MGL_NO_EXPORT mgl_fl_delay(void *v)	{	return ((mglCanvasWnd*)v)->GetDelay();	}	///< Callback function for delay
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_makemenu_fltk(Fl_Menu_ *m, Fl_MGLView *w)
{
	m->add("Graphics/Alpha", "^t", mgl_alpha_cb, w, FL_MENU_TOGGLE);
	m->add("Graphics/Light", "^l", mgl_light_cb, w, FL_MENU_TOGGLE);
	m->add("Graphics/Grid", "^g", mgl_grid_cb, w, FL_MENU_TOGGLE|FL_MENU_DIVIDER);

	m->add("Graphics/Restore", "^ ", mgl_norm_cb, w);
	m->add("Graphics/Redraw", "f5", mgl_draw_cb, w);
	m->add("Graphics/Adjust size", "f6", mgl_adjust_cb, w);
	m->add("Graphics/Reload data", "f9", mgl_oncemore_cb, w);
	//TODO	m->add("Graphics/Stop", "f7", mgl_stop_cb, w);
	//TODO	m->add("Graphics/Copy graphics","+^c", mgl_copyimg_cb, w);

	m->add("Graphics/Export/as PNG", "#p", mgl_export_png_cb, w);
	m->add("Graphics/Export/as solid PNG", "#f", mgl_export_pngn_cb, w);
	m->add("Graphics/Export/as JPEG", "#j", mgl_export_jpeg_cb, w);
	m->add("Graphics/Export/as SVG", "#s", mgl_export_svg_cb, w);
	m->add("Graphics/Export/as vector EPS", "#e", mgl_export_eps_cb, w);
	m->add("Graphics/Export/as bitmap EPS", "", mgl_export_bps_cb, w);

	m->add("Graphics/Animation/Slideshow", "^f5", mgl_sshow_cb, w, FL_MENU_TOGGLE);
	m->add("Graphics/Animation/Next frame", "#<", mgl_snext_cb, w);
	m->add("Graphics/Animation/Prev frame", "#>", mgl_sprev_cb, w);
	//TODO	m->add("Graphics/Animation/Setup", "", mgl_ssetup_cb, w);
}
//-----------------------------------------------------------------------------
void mglCanvasFL::Window(int argc, char **argv, int (*draw)(mglBase *gr, void *p), const char *title, void *par, void (*reload)(void *p), bool maximize)
{
	Fl::lock();
	SetDrawFunc(draw, par, reload);
	if(Wnd)	{	Wnd->label(title);	Wnd->show();	return;	}

	Wnd = new Fl_Double_Window(830,660,title);

	mgl = new Fl_MGLView(0,30,830,630);		mgl->par = this;

	mgl->menu = new Fl_Menu_Bar(0, 0, 830, 30);
	mgl_makemenu_fltk(mgl->menu, mgl);

	mgl->next = mgl_fl_next;	mgl->reload = mgl_fl_reload;
	mgl->prev = mgl_fl_prev;	mgl->delay= mgl_fl_delay;
	mgl->FMGL->set_graph(this);
	mgl->FMGL->set_draw(draw, par);

	Wnd->end();
	Wnd->resizable(mgl);	//w->graph);

	if(maximize)
	{
		int x,y,w,h;
		Fl::screen_xywh(x,y,w,h);
		Wnd->resize(x,y,w,h);
		Adjust();
	}

	char *tmp[1];	tmp[0]=new char[1];	tmp[0][0]=0;
	Wnd->show(argv ? argc:0, argv ? argv:tmp);
	delete []tmp[0];
}
//-----------------------------------------------------------------------------
HMGL MGL_EXPORT mgl_create_graph_fltk(int (*draw)(HMGL gr, void *p), const char *title, void *par, void (*load)(void *p))
{
	mglCanvasFL *g = new mglCanvasFL;
	g->Window(0,0,draw,title,par,load);
	return g;
}
int MGL_EXPORT mgl_fltk_run()		{	return Fl::run();	}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_create_graph_fltk_(const char *title, int l)
{
	char *s = new char[l+1];	memcpy(s,title,l);	s[l]=0;
	uintptr_t t = uintptr_t(mgl_create_graph_fltk(0,s,0,0));
	delete []s;	return t;
}
int MGL_EXPORT mgl_fltk_run_()	{	return mgl_fltk_run();	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_fltk_tmp(void *)
{	mgl_fltk_run();	return 0;	}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_fltk_thr()		// NOTE: Qt couldn't be running in non-primary thread
{
#if MGL_HAVE_PTHREAD
	static pthread_t thr;
	pthread_create(&thr,0,mgl_fltk_tmp,0);
	pthread_detach(thr);
#endif
	return 0;	// stupid, but I don't want keep result returned by Fl::Run()
}
//-----------------------------------------------------------------------------
