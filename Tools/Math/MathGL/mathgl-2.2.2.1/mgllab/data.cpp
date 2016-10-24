/* data.cpp is part of UDAV
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
#include <FL/Fl_Choice.H>
#include <FL/Fl_Spinner.H>
#include <FL/Fl_Output.H>
#include <FL/Fl_Check_Button.H>
#include "udav.h"
//-----------------------------------------------------------------------------
void option_in_cb(Fl_Widget *, void *v);
void style_in_cb(Fl_Widget *, void *v);
//-----------------------------------------------------------------------------
struct VarDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Choice *var;
	Fl_Spinner *dim1, *dim2, *dim3;
	VarDlg()	{	memset(this,0,sizeof(VarDlg));	create_dlg();	}
	~VarDlg()	{	delete wnd;	}
	void create_dlg();
	char *get_result();
	void init();
} var_dlg;
//-----------------------------------------------------------------------------
void VarDlg::init()
{
	char ss[1024];
	var->clear();
	mglVar *v=Parse->FindVar("");
	while(v)
	{
		wcstombs(ss,v->s.c_str(),1024);
		var->add(ss,0,0,v);
		v = v->next;
	}
}
//-----------------------------------------------------------------------------
char *VarDlg::get_result()
{
	static char res[64];
	char a1[16]=":",a2[16]=":",a3[16]=":";
	res[0]=0;
	if(var->value()<0)	return res;
	const Fl_Menu_Item m=var->menu()[var->value()];
	if(m.text[0])
	{
		if(dim3->value()>=0)
		{
			if(dim1->value()>=0)	snprintf(a1,16,"%g",dim1->value());
			if(dim2->value()>=0)	snprintf(a2,16,"%g",dim2->value());
			snprintf(a3,16,"%g",dim3->value());
			snprintf(res,64,"%s(%s,%s,%s)",m.text,a1,a2,a3);
		}
		else if(dim2->value()>=0)
		{
			if(dim1->value()>=0)	snprintf(a1,16,"%g",dim1->value());
			snprintf(a2,16,"%g",dim2->value());
			snprintf(res,64,"%s(%s,%s)",m.text,a1,a2);
		}
		else if(dim1->value()>=0)
		{
			snprintf(a1,16,"%g",dim1->value());
			snprintf(res,64,"%s(%s)",m.text,a1);
		}
		else	strncpy(res,m.text,64);
	}
	return res;
}
//-----------------------------------------------------------------------------
void var_chg_cb(Fl_Widget *, void *)
{
	const Fl_Menu_Item m=var_dlg.var->menu()[var_dlg.var->value()];
	if(m.text[0] && m.user_data())
	{
		mglVar *a = (mglVar *)m.user_data();
		var_dlg.dim1->range(-1,a->nx-1);
		var_dlg.dim2->range(-1,a->ny-1);
		var_dlg.dim3->range(-1,a->nz-1);
	}
}
//-----------------------------------------------------------------------------
void var_in_cb(Fl_Widget *, void *v)
{
	Fl_Input *e = (Fl_Input*)v;
	VarDlg *s = &var_dlg;
	s->OK = false;
	s->init();
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	e->value(s->get_result());
}
//-----------------------------------------------------------------------------
void var_dlg_cb(Fl_Widget *, void *v)
{	var_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void VarDlg::create_dlg()
{
	wnd = new Fl_Double_Window(190, 180, gettext("Variable"));
	var = new Fl_Choice(100, 10, 75, 25, gettext("Variable name"));	// !!!  add variables here !!!
	var->callback(var_chg_cb);
	dim1 = new Fl_Spinner(100, 40, 75, 25, gettext("First index"));
	dim1->range(-1,0);	dim1->value(-1);	dim1->step(1);
	dim1->tooltip(gettext("Value of first dimensions (-1 for all range)"));
	dim2 = new Fl_Spinner(100, 70, 75, 25, gettext("Second index"));
	dim2->range(-1,0);	dim2->value(-1);	dim2->step(1);
	dim2->tooltip(gettext("Value of second dimensions (-1 for all range)"));
	dim3 = new Fl_Spinner(100, 100, 75, 25, gettext("Third index"));
	dim3->range(-1,0);	dim3->value(-1);	dim3->step(1);
	dim3->tooltip(gettext("Value of third dimensions (-1 for all range)"));
	Fl_Button *o;
	o = new Fl_Button(15, 140, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o = new Fl_Return_Button(100, 140, 75, 25, gettext("OK"));	o->callback(var_dlg_cb,wnd);
	wnd->end();
}
//-----------------------------------------------------------------------------
const char *cmds[]={
"plot|area|bars|barh|boxplot|chart|error|mark|region|stem|step|tens|textmark|torus|tube",
"surf|axial|belt|boxs|cont|contd|contf|dens|fall|grid2|mesh|tile|grad",
"surf3|cloud|beam|cont3|conta|contf3|contfa|dens3|densa|grid3|grida",
"map|stfa|surfa|surfc|tile|surf3a|surf3c",
"flow|pipe|traj|vect|vectc|vectl|dew",
"contx|conty|contz|contfx|contfy|contfz|densx|densy|densz|triplot|tricont|quadplot|crust|dots",
"text|title|fgets|legend|addlegend|clearlegend|legendbox",
"new|var|copy|delete|insert|read|readmat|readall|readhdf|save|savehdf|export|import|info|idset",
"fill|fillsample|modify|put|crop|extend|rearrange|squeeze|transpose|cumsum|diff|diff2|sinfft|cosfft|hankel|envelop|integrate|mirror|norm|normsl|sew|smooth|swap|roll|addto|subto|divto|multo",
"combine|evaluate|max|min|hist|jacobian|momentum|resize|sum|trace|transform|transforma|stfad|pde|qo2d|ray",
"axis|box|colorbar|grid|xlabel|ylabel|zlabel|tlabel",
"alpha|alphadef|transparent|transptype|ambient|light|fog|arrowsize|barwidth|linewidth|marksize|plotfactor|zoom|cut|axialdir|mesgnum|font|palette|rotatetext",
"axis|ranges|caxis|crange|xrange|yrange|zrange|origin|ternary|adjust|ctick|xtick|ytick|ztick|ticklen|tickstl",
"subplot|inplot|rotate|aspect|columnplot|perspective",
"call|func|chdir|define|if|elseif|else|endif|for|next|once|stop|write|setsize",
"fit|fits|putsfit",
"fplot|fsurf|ball|cone|curve|drop|facex|facey|facez|line|rect|sphere"};
const char *first[]={"plot", "surf", "surf3", "map", "flow", "contx", "text", "new", "fill", "combine", "alpha", "axis", "subplot", "call", "fit", "fplot"};
const char *cmd_types="1D plots|2D plots|3D plots|Dual plots|Vector plots|Other plots|Text and legend|Create data and I-O|Data handling|Data extraction|Axis and colorbar|General setup|Axis setup|Scale and rotate|Program flow|Nonlinear fitting|Primitives";
//-----------------------------------------------------------------------------
void data_file(char *fn)
{
	static int num=0;
	static char name[32], res[256];
	snprintf(name,32,"mgl_%d",num);	num++;
	mglData *v = Parse->AddVar(name);
	v->Read(fn);
	if(v->nz>1)
		snprintf(res,256,"#read %s '%s'\nrotate 40 60\ncrange %s\nbox\nsurf3 %s\n", name, fn, name, name);
	else if(v->ny>1)
		snprintf(res,256,"#read %s '%s'\nrotate 40 60\ncrange %s\nzrange %s\nbox\nsurf %s\n", name, fn, name, name, name);
	else
		snprintf(res,256,"#read %s '%s'\nyrange %s\nbox\nplot %s\n", name, fn, name, name);
	textbuf->text(res);
}
//-----------------------------------------------------------------------------
//
//			New Command dialog
//
//-----------------------------------------------------------------------------
struct CmdDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Choice *type, *cmd;
	Fl_Input *var_x, *var_y, *var_z, *var_u, *var_v, *var_w;
	Fl_Box *fmt, *dsc;
	Fl_Input *stl, *zval, *par1, *par2, *opt;
	Fl_Help_View *help;

	CmdDlg()	{	memset(this,0,sizeof(CmdDlg));	create_dlg();	}
	~CmdDlg()	{	delete wnd;	}
	void create_dlg();
	char *get_result();
} cmd_dlg;
//-----------------------------------------------------------------------------
void cmd_dlg_cb(Fl_Widget *, void *v)	// add variables checking
{	cmd_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void type_cmd_cb(Fl_Widget *, void *)
{
	int val = cmd_dlg.type->value();
	if(val>=0 && val<16)
	{
		cmd_dlg.cmd->clear();	cmd_dlg.cmd->add(cmds[val]);
		cmd_dlg.dsc->copy_label(Parse->CmdDesc(first[val]));
		cmd_dlg.fmt->copy_label(Parse->CmdFormat(first[val]));

		static char str[300];	// load help for command
#ifdef WIN32
		snprintf(str,300,"%s\\mgl_en.html#%s",docdir,first[val]);
#else
		snprintf(str,300,"%s/mgl_en.html#%s",docdir,first[val]);
#endif
		cmd_dlg.help->load(str);
	}
	cmd_dlg.cmd->value(0);
}
//-----------------------------------------------------------------------------
void desc_cmd_cb(Fl_Widget *, void *)
{
	const char *name = cmd_dlg.cmd->mvalue()->text;
	cmd_dlg.dsc->copy_label(Parse->CmdDesc(name));
	cmd_dlg.fmt->copy_label(Parse->CmdFormat(name));

	static char str[300];	// load help for command
#ifdef WIN32
	snprintf(str,300,"%s\\mgl_en.html#%s",docdir,name);
#else
	snprintf(str,300,"%s/mgl_en.html#%s",docdir,name);
#endif
	cmd_dlg.help->load(str);
}
//-----------------------------------------------------------------------------
void CmdDlg::create_dlg()
{
	Fl_Button *o;
	wnd = new Fl_Double_Window(500, 450, gettext("Command properties"));
	type = new Fl_Choice(90, 10, 170, 25, gettext("Type of plot"));
	type->tooltip(gettext("Select one of general types of plot"));
	cmd = new Fl_Choice(350, 10, 100, 25, gettext("Command"));
	cmd->tooltip(gettext("Select kind of plot in this group"));

	fmt = new Fl_Box(0, 40, 500, 25);
	fmt->box(UDAV_DOWN_BOX);
	fmt->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);
	fmt->tooltip(gettext("Format of command and its arguments"));
	dsc = new Fl_Box(0, 70, 500, 25);
	dsc->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);
	dsc->tooltip(gettext("Short command description"));

	type->callback(type_cmd_cb,cmd);	cmd->callback(desc_cmd_cb,0);
	type->add(gettext(cmd_types));		type_cmd_cb(0,0);

	var_x = new Fl_Input(15, 115, 50, 25, "X");	var_x->align(FL_ALIGN_TOP);
	o = new Fl_Button(65, 115, 25, 25, "..");	o->callback(var_in_cb,var_x);
	var_y = new Fl_Input(95, 115, 50, 25, "Y");	var_y->align(FL_ALIGN_TOP);
	o = new Fl_Button(145, 115, 25, 25, "..");	o->callback(var_in_cb,var_y);
	var_z = new Fl_Input(175, 115, 50, 25, "Z");	var_z->align(FL_ALIGN_TOP);
	o = new Fl_Button(225, 115, 25, 25, "..");	o->callback(var_in_cb,var_z);
	var_u = new Fl_Input(255, 115, 50, 25, gettext("Vx or A"));	var_u->align(FL_ALIGN_TOP);
	o = new Fl_Button(305, 115, 25, 25, "..");	o->callback(var_in_cb,var_u);
	var_v = new Fl_Input(335, 115, 50, 25, gettext("Vy or C"));	var_v->align(FL_ALIGN_TOP);
	o = new Fl_Button(385, 115, 25, 25, "..");	o->callback(var_in_cb,var_v);
	var_w = new Fl_Input(415, 115, 50, 25, "Vz");var_w->align(FL_ALIGN_TOP);
	o = new Fl_Button(465, 115, 25, 25, "..");	o->callback(var_in_cb,var_w);

	stl = new Fl_Input(15, 165, 50, 25, gettext("Style"));
	stl->align(FL_ALIGN_TOP);	stl->tooltip(gettext("String argument with command style (or scheme or font)"));
	o = new Fl_Button(65, 165, 25, 25, "..");	o->callback(style_in_cb, stl);

	zval = new Fl_Input(95, 165, 75, 25, gettext("zVal or sVal"));
	zval->align(FL_ALIGN_TOP);
	zval->tooltip(gettext("Z-value or value of slice.\nKeep empty for default value"));
	par1 = new Fl_Input(175, 165, 75, 25, gettext("Text or dir"));
	par1->align(FL_ALIGN_TOP);
	par1->tooltip(gettext("Text (in text command) or direction (in cont3, contf3, dens3)"));
	par2 = new Fl_Input(255, 165, 75, 25, gettext("Number"));
	par2->align(FL_ALIGN_TOP);
	par2->tooltip(gettext("Number of contours in cont* commands"));

	opt = new Fl_Input(15, 215, 290, 25, gettext("Options"));
	opt->align(FL_ALIGN_TOP_LEFT);
	opt->tooltip(gettext("Command options"));
	o = new Fl_Button(305, 215, 25, 25, "..");			o->callback(option_in_cb, opt);

	o = new Fl_Button(405, 180, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb, wnd);
	o = new Fl_Return_Button(405, 215, 75, 25, gettext("OK"));	o->callback(cmd_dlg_cb, wnd);

	help = new Fl_Help_View(0, 250, 500, 200);
	wnd->end();
}
//-----------------------------------------------------------------------------
char *CmdDlg::get_result()
{
	static char res[1024],buf[128];
	res[0]=0;
	const char *cn = cmd->mvalue()->text;

	bool sl3 = !strcmp(cn,"cont3") || !strcmp(cn,"contf3") || !strcmp(cn,"dens3");
	strcpy(res,"\n");	strncat(res,cn,1022);
	snprintf(buf,128,"%s%s%s%s%s%s%s%s%s%s%s%s", var_x->value()[0]?" ":"", var_x->value(),
		var_y->value()[0]?" ":"", var_y->value(), var_z->value()[0]?" ":"", var_z->value(),
		var_u->value()[0]?" ":"", var_u->value(), var_v->value()[0]?" ":"", var_v->value(),
		var_w->value()[0]?" ":"", var_w->value());
	strcat(res,buf);

	if(!strcmp(cn,"text") && par1->value()[0])
	{	strcat(res," '");	strcat(res,par1->value());	strcat(res,"'");	}
	if(sl3 && !par1->value()[0])
	{
		strcat(res," 'x'");
		fl_message(gettext("You should specify direction.\nDirection 'x' is selected by default"));
		if(zval->value()[0])
		{	snprintf(buf,128," %d",atoi(zval->value()));	strcat(res,buf);	}
	}
	if(sl3 && par1->value()[0])
	{
		strcat(res," '");	strcat(res,par1->value());	strcat(res,"'");
		if(zval->value()[0])
		{	snprintf(buf,128," %d",atoi(zval->value()));	strcat(res,buf);	}
	}
	if(stl->value()[0])
	{	strcat(res," '");	strcat(res,stl->value());	strcat(res,"'");	}
	if(!sl3 && zval->value()[0])
	{	snprintf(buf,128," %d",atoi(zval->value()));	strcat(res,buf);	}
	if(!sl3 && par2->value()[0])
	{	snprintf(buf,128," %d",atoi(par2->value()));	strcat(res,buf);	}
	if(opt->value()[0])	strcat(res,opt->value());
//	strcat(res,"\n");
	return res;
}
//-----------------------------------------------------------------------------
void command_cb(Fl_Widget *, void *v)
{
	CmdDlg *s = &cmd_dlg;
	ScriptWindow* e = (ScriptWindow*)v;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
	{
		long i=e->editor->insert_position(), j=textbuf->line_end(i);
		e->editor->insert_position(j);
		e->editor->insert(s->get_result());
	}
}
//-----------------------------------------------------------------------------
void plot_dat_cb(Fl_Widget *, void *)
{
	CmdDlg *s = &cmd_dlg;
	s->OK = false;
	s->wnd->set_modal();
	s->wnd->show();
	while(s->wnd->shown())	Fl::wait();
	if(s->OK)	// replace current selection
		textbuf->insert(textbuf->length(), s->get_result());
}
//-----------------------------------------------------------------------------
