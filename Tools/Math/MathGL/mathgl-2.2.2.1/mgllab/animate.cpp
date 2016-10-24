/* animate.cpp is part of UDAV
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
#include <ctype.h>
#include <string.h>
#include <FL/Fl_Tabs.H>
#include <FL/Fl_Round_Button.H>
#include <FL/Fl_Multiline_Input.H>
#include <FL/Fl_Float_Input.H>
#include "udav.h"
//-----------------------------------------------------------------------------
struct ArgumentDlg
{
public:
	Fl_Window* wnd;
	int OK;
	Fl_Input *a[10];

	ArgumentDlg()	{	memset(this,0,sizeof(ArgumentDlg));	create_dlg();	}
	~ArgumentDlg()	{	delete wnd;	}
	void FillResult();
protected:
	void create_dlg();
} argument_dlg;
//-----------------------------------------------------------------------------
void argument_dlg_cb(Fl_Widget *, void *v)
{	argument_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void ArgumentDlg::create_dlg()
{
	wnd = new Fl_Window(325, 275, gettext("Script arguments"));
	a[1] = new Fl_Input(10, 25, 150, 25, gettext("Value for $1"));	a[1]->align(FL_ALIGN_TOP_LEFT);
	a[2] = new Fl_Input(165, 25, 150, 25, gettext("Value for $2"));	a[2]->align(FL_ALIGN_TOP_LEFT);
	a[3] = new Fl_Input(10, 70, 150, 25, gettext("Value for $3"));	a[3]->align(FL_ALIGN_TOP_LEFT);
	a[4] = new Fl_Input(165, 70, 150, 25, gettext("Value for $4"));	a[4]->align(FL_ALIGN_TOP_LEFT);
	a[5] = new Fl_Input(10, 115, 150, 25, gettext("Value for $5"));	a[5]->align(FL_ALIGN_TOP_LEFT);
	a[6] = new Fl_Input(165, 115, 150, 25, gettext("Value for $6"));a[6]->align(FL_ALIGN_TOP_LEFT);
	a[7] = new Fl_Input(10, 160, 150, 25, gettext("Value for $7"));	a[7]->align(FL_ALIGN_TOP_LEFT);
	a[8] = new Fl_Input(165, 160, 150, 25, gettext("Value for $8"));a[8]->align(FL_ALIGN_TOP_LEFT);
	a[9] = new Fl_Input(10, 205, 150, 25, gettext("Value for $9"));	a[9]->align(FL_ALIGN_TOP_LEFT);
	a[0] = new Fl_Input(165, 205, 150, 25, gettext("Value for $0"));a[0]->align(FL_ALIGN_TOP_LEFT);

	Fl_Button *o;
	o = new Fl_Button(75, 240, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(175, 240, 75, 25, gettext("OK"));	o->callback(argument_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
void ArgumentDlg::FillResult()
{
	if(a[0]->value()[0])	Parse->AddParam(0,a[0]->value());
	if(a[1]->value()[0])	Parse->AddParam(1,a[1]->value());
	if(a[2]->value()[0])	Parse->AddParam(2,a[2]->value());
	if(a[3]->value()[0])	Parse->AddParam(3,a[3]->value());
	if(a[4]->value()[0])	Parse->AddParam(4,a[4]->value());
	if(a[5]->value()[0])	Parse->AddParam(5,a[5]->value());
	if(a[6]->value()[0])	Parse->AddParam(6,a[6]->value());
	if(a[7]->value()[0])	Parse->AddParam(7,a[7]->value());
	if(a[8]->value()[0])	Parse->AddParam(8,a[8]->value());
	if(a[9]->value()[0])	Parse->AddParam(9,a[9]->value());
}
//-----------------------------------------------------------------------------
void argument_cb(Fl_Widget *, void *)
{
	ArgumentDlg *s = &argument_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	s->FillResult();
}
//-----------------------------------------------------------------------------
void argument_set(int n, const char *s)
{
	if(n<0 || n>9)	return;
	Parse->AddParam(n,s);
	argument_dlg.a[n]->value(s);
}
//-----------------------------------------------------------------------------
AnimateDlg animate_dlg;
//-----------------------------------------------------------------------------
void animate_dlg_cb(Fl_Widget *, void *v)
{
	animate_dlg.swap = false;
	if(!animate_dlg.rt->value() && !animate_dlg.rv->value())
		fl_message(gettext("You have to select textual string or numeric cycle"));
	else if(animate_dlg.rv->value() && animate_dlg.dx->value()==0)
		fl_message(gettext("You have to set nonzero step in cycle"));
	else
	{
		double t0=atof(animate_dlg.x0->value()), t1=atof(animate_dlg.x1->value()), dt=atof(animate_dlg.dx->value());
		if((t1-t0)*dt<0)
		{
			if(fl_ask(gettext("Order of first and last value is wrong. Swap it?")))
			{
				char s[32];	snprintf(s,32,"%g",t0);
				animate_dlg.x0->value(animate_dlg.x1->value());
				animate_dlg.x1->value(s);
			}
			else
			{	fl_message(gettext("Wrong boundaries"));	return;	}
		}
		animate_dlg.OK = true;	((Fl_Window *)v)->hide();
	}
}
//-----------------------------------------------------------------------------
void animate_rad_cb(Fl_Widget *, void *v)
{
	animate_dlg.rt->value(0);
	animate_dlg.rv->value(0);
	((Fl_Round_Button *)v)->value(1);
}
//-----------------------------------------------------------------------------
void animate_put_cb(Fl_Widget *, void *)
{
	if(animate_dlg.rt->value())
	{
		if(animate_dlg.txt->value()==0 || strlen(animate_dlg.txt->value())==0)	return;
		char *s = new char[1+strlen(animate_dlg.txt->value())], *a=s;
		strcpy(s, animate_dlg.txt->value());
		for(int i=0;s[i]!=0;i++)
		{
			if(s[i]=='\n')
			{
				s[i] = 0;
				textbuf->append("\n##a ");
				textbuf->append(a);
				a = s+i+1;
			}
		}
		if(*a)
		{	textbuf->append("\n##a ");	textbuf->append(a);	}
		delete []s;
	}
	else if(animate_dlg.rv->value())
	{
		char *s = new char[128];
		snprintf(s,128,"\n##c %s %s %s",animate_dlg.x0->value(),animate_dlg.x1->value(),animate_dlg.dx->value());
		textbuf->append(s);
		delete []s;
	}
}
//-----------------------------------------------------------------------------
void AnimateDlg::create_dlg()
{
	wnd = new Fl_Window(335, 350, gettext("Animation"));
	new Fl_Box(10, 5, 315, 25, gettext("Redraw picture for $0 equal to:"));
	rt = new Fl_Round_Button(10, 30, 200, 25, gettext("strings in lines below"));
	rt->callback(animate_rad_cb, rt);
	rv = new Fl_Round_Button(220, 30, 105, 25, gettext("values"));
	rv->callback(animate_rad_cb, rv);
	txt = new Fl_Multiline_Input(10, 60, 200, 250);
	x0 = new Fl_Float_Input(220, 80, 105, 25, gettext("from"));			x0->align(FL_ALIGN_TOP_LEFT);
	x1 = new Fl_Float_Input(220, 130, 105, 25, gettext("to"));			x1->align(FL_ALIGN_TOP_LEFT);
	dx = new Fl_Float_Input(220, 180, 105, 25, gettext("with step"));	dx->align(FL_ALIGN_TOP_LEFT);

	Fl_Button *o;
	o = new Fl_Button(230, 215, 80, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(230, 250, 80, 25, gettext("OK"));	o->callback(animate_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	save = new Fl_Check_Button(220, 285, 105, 25, gettext("save slides"));
	save->tooltip(gettext("Keep slides in memory (faster animation but require more memory)"));
	save->down_box(FL_DOWN_BOX);	save->hide();

	o = new Fl_Button(10, 315, 100, 25, gettext("Put to script"));	o->callback(animate_put_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	dt = new Fl_Float_Input(220, 315, 105, 25, gettext("Delay (in sec)"));//	dx->align(FL_ALIGN_TOP_LEFT);

	wnd->end();
}
//-----------------------------------------------------------------------------
void AnimateDlg::FillResult(Fl_MGL* e)
{
	e->NArgs = e->ArgCur = 0;
	if(e->ArgBuf)	delete [](e->ArgBuf);	e->ArgBuf = 0;
	e->AnimDelay = atof(dt->value());
	if(rt->value())
	{
		char *s;
		e->ArgBuf = new char[1+strlen(txt->value())];
		strncpy(e->ArgBuf, txt->value(),32);
		s = e->Args[0] = e->ArgBuf;	e->NArgs = 1;
		for(int i=0;s[i]!=0;i++)
			if(s[i]=='\n')
			{
				s[i] = 0;	e->Args[e->NArgs] = s+i+1;	e->NArgs += 1;
			}
		if(e->Args[e->NArgs-1][0]==0)	e->NArgs -= 1;
	}
	else if(rv->value() && atof(dx->value()))
	{
		double t0=atof(x0->value()), t1=atof(x1->value()), dt=atof(dx->value()), t;
		if((t1-t0)/dt<1)
		{
			e->ArgBuf = new char[32];	snprintf(e->ArgBuf,32,"%g",t0);
			e->NArgs = 1;	e->Args[0] = e->ArgBuf;	return;
		}
		if((t1-t0)/dt>999)
		{
			fl_message(gettext("Too many slides. Reduce to 1000 slides."));
			dt = (t1-t0)/998;
		}
		e->ArgBuf = new char[32*int(1+(t1-t0)/dt)];
		for(t=t0;(dt>0&&t<=t1)||(dt<0&&t>=t1);t+=dt)
		{
			snprintf(e->ArgBuf + 32*e->NArgs,32,"%g\0",t);
			e->Args[e->NArgs] = e->ArgBuf + 32*e->NArgs;
			e->NArgs += 1;
		}
	}
	else	fl_message(gettext("No selection. So nothing to do"));
}
//-----------------------------------------------------------------------------
void animate_cb(Fl_Widget *, void *v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	AnimateDlg *s = &animate_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	s->FillResult(e->graph);
}
//-----------------------------------------------------------------------------
void cpy_arg_buf(const char *str, long *size, char **buf)
{
	const char *end;
	for(end=str; *end>' '; end++);
	if(end>=str+*size)
	{	*size = end-str+1;	*buf = (char *)realloc(*buf,*size);	}
	memset(*buf,0,*size);
	strncpy(*buf,str,end-str);
}
//-----------------------------------------------------------------------------
void fill_animate(const char *text)
{
	long size=128,i;
	const char *str = text;
	char *buf = (char *)malloc(size), tmp[4]="#$0";
	for(i=0;i<10;i++)	// first read script arguments (if one)
	{
		tmp[2] = '0'+i;
		if((str=strstr(text,tmp)))
		{
			str+=3;
			while(*str>0 && *str<=' ' && *str!='\n')	str++;
			cpy_arg_buf(str,&size,&buf);
			argument_dlg.a[i]->value(buf);
			Parse->AddParam(i,buf);
		}
	}

	char *a = (char *)malloc(size);
	memset(a,0,size);	i = 0;
	str = text;
	while((str = strstr(str, "##")))	// now read animation parameters
	{
		if(str[2]=='a')
		{
			str += 3;
			while(*str>0 && *str<=' ' && *str!='\n')	str++;
			if(*str==0 || *str=='\n')	return;	// empty comment
			cpy_arg_buf(str,&size,&buf);
			if(i==0)	Parse->AddParam(0,buf);	// put first value as $0
			i += strlen(buf)+1;
			if(i>=size)
			{
				size = (1+ (i+2)/128)*128;
				a = (char *)realloc(a,size);
			}
			strcat(a,buf);	strcat(a,"\n");
		}
		if(str[2]=='c')
		{
			str += 3;
			register long j=0,l=strlen(str);
			char *s=new char[l+1],*s1=0,*s2=0,*s3=0;
			bool sp=true;	strcpy(s,str);
			for(j=0;j<l;j++)
			{
				if(isspace(s[j]))	{	s[j]=0;	sp=true;	}
				else if(sp)
				{
					sp=false;
					if(!s1)	s1=s+j;	else if(!s2) s2=s+j;	else s3=s+j;
				}
				animate_dlg.x0->value(s1);
				animate_dlg.x1->value(s2);
				animate_dlg.dx->value(s3);
				animate_dlg.rv->value(1);
			}
			delete []s;
		}
	}
	if(i)
	{	animate_dlg.txt->value(a);	animate_dlg.rt->value(1);	}
	free(buf);	free(a);
}
