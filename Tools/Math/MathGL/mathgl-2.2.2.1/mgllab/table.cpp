/* table.cpp is part of UDAV
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
#include <FL/Fl_Spinner.H>
#include <FL/Fl_Output.H>
#include <FL/Fl_Float_Input.H>
#include <FL/Fl_Value_Input.H>
#include <FL/Fl_Round_Button.H>
#include "udav.h"
//-----------------------------------------------------------------------------
void addto_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *s = fl_input(gettext("Enter number for addition to data values"),0);
	if(s)	{	mgl_data_add_num(e->var, atof(s));	e->refresh();	}
}
//-----------------------------------------------------------------------------
void subto_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *s = fl_input(gettext("Enter number for subtraction from data values"),0);
	if(s)	{	mgl_data_sub_num(e->var, atof(s));	e->refresh();	}
}
//-----------------------------------------------------------------------------
void multo_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *s = fl_input(gettext("Enter number for multiplication of data values"),0);
	if(s)	{	mgl_data_mul_num(e->var, atof(s));	e->refresh();	}
}
//-----------------------------------------------------------------------------
void divto_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *s = fl_input(gettext("Enter number for division of data values"),0);
	if(s)	{	mgl_data_div_num(e->var, atof(s));	e->refresh();	}
}
//-----------------------------------------------------------------------------
struct XYZDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Box *box;
	Fl_Check_Button *ch;
	Fl_Spinner *mx, *my, *mz;
	XYZDlg()	{	memset(this,0,sizeof(XYZDlg));	create_dlg();	}
	~XYZDlg()	{	delete wnd;	}
	void create_dlg();
} xyz_dlg;
//-----------------------------------------------------------------------------
void xyz_dlg_cb(Fl_Widget *, void *v)
{	xyz_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void XYZDlg::create_dlg()
{
	wnd = new Fl_Double_Window(325, 125, gettext("Change data sizes"));
	box = new Fl_Box(10, 10, 305, 40);
	box->box(UDAV_THIN_UP_BOX);
	box->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);

	mx = new Fl_Spinner(30, 55, 75, 25, "mx");
	mx->tooltip(gettext("New size of data on 1st dimension (x-direction)"));
	my = new Fl_Spinner(135, 55, 75, 25, "my");
	my->tooltip(gettext("New size of data on 2nd dimension (y-direction)"));
	mz = new Fl_Spinner(240, 55, 75, 25, "mz");
	mz->tooltip(gettext("New size of data on 3d dimension (z-direction)"));
	ch = new Fl_Check_Button(15, 90, 95, 25);

	Fl_Button *o;
	o = new Fl_Button(125, 90, 85, 25, gettext("Cancel"));	o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Do nothing and close this window"));
	o = new Fl_Return_Button(230, 90, 85, 25, gettext("Change"));o->callback(xyz_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Change (resize) data"));
	wnd->end();
}
//-----------------------------------------------------------------------------
void new_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	xyz_dlg.box->label(gettext("Specify new data size\nData will be zero filled"));
	xyz_dlg.ch->label(gettext("not used"));	xyz_dlg.OK = false;
	xyz_dlg.wnd->set_modal();		xyz_dlg.wnd->show();
	while(xyz_dlg.wnd->shown())	Fl::wait();
	if(xyz_dlg.OK)
	{
		e->var->Create(int(xyz_dlg.mx->value()),
			int(xyz_dlg.my->value()), int(xyz_dlg.mz->value()));
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
void resize_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	xyz_dlg.box->label(gettext("Specify new data size\nData will be interpolated"));
	xyz_dlg.ch->label(gettext("not used"));	xyz_dlg.OK = false;
	xyz_dlg.wnd->set_modal();		xyz_dlg.wnd->show();
	while(xyz_dlg.wnd->shown())	Fl::wait();
	if(xyz_dlg.OK)
	{
		mglData d = e->var->Resize(int(xyz_dlg.mx->value()), int(xyz_dlg.my->value()), int(xyz_dlg.mz->value()));
		mgl_data_set(e->var, &d);
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
void squeeze_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	xyz_dlg.box->label(gettext("Specify the skiping step\nEach m-th point will be skiped"));
	xyz_dlg.ch->label(gettext("smoothed"));	xyz_dlg.OK = false;
	xyz_dlg.wnd->set_modal();		xyz_dlg.wnd->show();
	while(xyz_dlg.wnd->shown())	Fl::wait();
	if(xyz_dlg.OK)
	{
		e->var->Squeeze(int(xyz_dlg.mx->value()), int(xyz_dlg.my->value()),
					int(xyz_dlg.mz->value()), xyz_dlg.ch->value());
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
struct ChngDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Check_Button *dx, *dy, *dz;
	Fl_Choice *kind, *type;
	ChngDlg()	{	memset(this,0,sizeof(ChngDlg));	create_dlg();	}
	~ChngDlg()	{	delete wnd;	}
	void create_dlg();
	void execute(mglData *d);
} chng_dlg;
//-----------------------------------------------------------------------------
void chng_dlg_cb(Fl_Widget *, void *v)
{	chng_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void ChngDlg::execute(mglData *d)
{
	char r[8]="3";
	if(dx->value())	strcat(r,"x");
	if(dy->value())	strcat(r,"y");
	if(dz->value())	strcat(r,"z");
	if(!r[0])	return;
	if(type->value()==1)	r[0] = '5';
	if(type->value()==2)	r[0] = ' ';
	switch(kind->value())
	{
	case 0:	d->Smooth(r);		break;
	case 1:	d->CumSum(r);		break;
	case 2:	d->Integral(r);	break;
	case 3:	d->Diff(r);		break;
	case 4:	d->Diff2(r);		break;
	case 5:	d->Swap(r);		break;
	}
}
//-----------------------------------------------------------------------------
void ChngDlg::create_dlg()
{
	Fl_Menu_Item k[]={{gettext("Smooth")}, {gettext("CumSum")}, { gettext("Integrate")},
		{ gettext("Difference")}, { gettext("Double diff.")}, { gettext("Swap parts")}, {0}};
	Fl_Menu_Item t[]={{gettext("Linear *3")}, {gettext("Linear *5")}, {gettext("Parabolic *5")},{0}};
	wnd = new Fl_Double_Window(165, 215, gettext("Change data"));
	kind = new Fl_Choice(10, 25, 145, 25, gettext("Type of operation"));
	kind->align(FL_ALIGN_TOP_LEFT);	kind->copy(k);
	dx = new Fl_Check_Button(10, 55, 140, 25, gettext("apply in x direction"));
	dy = new Fl_Check_Button(10, 80, 140, 25, gettext("apply in y direction"));
	dz = new Fl_Check_Button(10, 105, 140, 25, gettext("apply in z direction"));
	type = new Fl_Choice(10, 145, 145, 25, gettext("Type of smoothing"));
	type->align(FL_ALIGN_TOP_LEFT);	type->copy(t);

	Fl_Button *o;
	o = new Fl_Button(10, 180, 65, 25, gettext("Cancel"));	o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(90, 180, 65, 25, gettext("Do"));o->callback(chng_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
void smooth_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(0);
	chng_dlg.type->activate();	chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void cumsum_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(1);
	chng_dlg.type->deactivate();chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void integr_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(2);
	chng_dlg.type->deactivate();chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void diff_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(3);
	chng_dlg.type->deactivate();chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void diff2_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(4);
	chng_dlg.type->deactivate();chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void swap_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	chng_dlg.kind->value(5);
	chng_dlg.type->deactivate();chng_dlg.OK = false;
	chng_dlg.wnd->set_modal();	chng_dlg.wnd->show();
	while(chng_dlg.wnd->shown())	Fl::wait();
	if(chng_dlg.OK)
	{	chng_dlg.execute(e->var);	e->refresh();	}
}
//-----------------------------------------------------------------------------
struct NwdtDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Check_Button *dx, *dy, *dz;
	Fl_Choice *kind;
	Fl_Input *name;
	NwdtDlg()	{	memset(this,0,sizeof(NwdtDlg));	create_dlg();	}
	~NwdtDlg()	{	delete wnd;	}
	void create_dlg();
} nwdt_dlg;
//-----------------------------------------------------------------------------
void nwdt_dlg_cb(Fl_Widget *, void *v)
{	nwdt_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void NwdtDlg::create_dlg()
{
	Fl_Menu_Item k[]={{gettext("Summation of")}, {gettext("Maximum of")}, { gettext("Minimum of")}, {0}};
	wnd = new Fl_Double_Window(165, 215, gettext("Extract data"));
	kind = new Fl_Choice(10, 25, 145, 25, gettext("Type of operation"));
	kind->align(FL_ALIGN_TOP_LEFT);	kind->copy(k);
	dx = new Fl_Check_Button(10, 55, 140, 25, gettext("apply in x direction"));
	dy = new Fl_Check_Button(10, 80, 140, 25, gettext("apply in y direction"));
	dz = new Fl_Check_Button(10, 105, 140, 25, gettext("apply in z direction"));
	name = new Fl_Input(10, 145, 145, 25, gettext("Name for output"));
	name->align(FL_ALIGN_TOP_LEFT);

	Fl_Button *o;
	o = new Fl_Button(10, 180, 65, 25, gettext("Cancel"));	o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o = new Fl_Return_Button(90, 180, 65, 25, gettext("Do"));o->callback(chng_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	wnd->end();
}
//-----------------------------------------------------------------------------
void asum_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	nwdt_dlg.kind->value(0);	nwdt_dlg.OK = false;
	nwdt_dlg.wnd->set_modal();	nwdt_dlg.wnd->show();
	while(nwdt_dlg.wnd->shown())	Fl::wait();
	if(nwdt_dlg.OK)
	{
		char r[8]="";
		if(nwdt_dlg.dx->value())	strcat(r,"x");
		if(nwdt_dlg.dy->value())	strcat(r,"y");
		if(nwdt_dlg.dz->value())	strcat(r,"z");
		if(!r[0])	return;
		if(!nwdt_dlg.name->value()[0] || !strcmp(nwdt_dlg.name->value(),e->label()))
			fl_alert(gettext("Name for output variable should be differ from this name"));
		else
		{
			mglData d = e->var->Sum(r);
			mgl_data_set(Parse->AddVar(nwdt_dlg.name->value()), &d);
		}
	}
}
//-----------------------------------------------------------------------------
void amax_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	nwdt_dlg.kind->value(0);	nwdt_dlg.OK = false;
	nwdt_dlg.wnd->set_modal();	nwdt_dlg.wnd->show();
	while(nwdt_dlg.wnd->shown())	Fl::wait();
	if(nwdt_dlg.OK)
	{
		char r[8]="";
		if(nwdt_dlg.dx->value())	strcat(r,"x");
		if(nwdt_dlg.dy->value())	strcat(r,"y");
		if(nwdt_dlg.dz->value())	strcat(r,"z");
		if(!r[0])	return;
		if(!nwdt_dlg.name->value()[0] || !strcmp(nwdt_dlg.name->value(),e->label()))
			fl_alert(gettext("Name for output variable should be differ from this name"));
		else
		{
			mglData d = e->var->Max(r);
			mgl_data_set(Parse->AddVar(nwdt_dlg.name->value()), &d);
		}
	}
}
//-----------------------------------------------------------------------------
void amin_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	nwdt_dlg.kind->value(0);	nwdt_dlg.OK = false;
	nwdt_dlg.wnd->set_modal();	nwdt_dlg.wnd->show();
	while(nwdt_dlg.wnd->shown())	Fl::wait();
	if(nwdt_dlg.OK)
	{
		char r[8]="";
		if(nwdt_dlg.dx->value())	strcat(r,"x");
		if(nwdt_dlg.dy->value())	strcat(r,"y");
		if(nwdt_dlg.dz->value())	strcat(r,"z");
		if(!r[0])	return;
		if(!nwdt_dlg.name->value()[0] || !strcmp(nwdt_dlg.name->value(), e->label()))
			fl_alert(gettext("Name for output variable should be differ from this name"));
		else
		{
			mglData d = e->var->Min(r);
			mgl_data_set(Parse->AddVar(nwdt_dlg.name->value()), &d);
		}
	}
}
//-----------------------------------------------------------------------------
void load_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	char *newfile = fl_file_chooser(gettext("Load Data?"),
		gettext("DAT Files (*.{dat,csv})\tAll Files (*)"), 0);
	if(newfile != NULL)
	{	e->var->Read(newfile);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void save_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	char *newfile = fl_file_chooser(gettext("Save Data?"),
		gettext("DAT Files (*.{dat,csv})\tAll Files (*)"), 0);
	if(newfile != NULL)	e->var->Save(newfile);
}
//-----------------------------------------------------------------------------
void exp_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *scheme, *newfile = fl_file_chooser(gettext("Export Data?"),
		gettext("PNG Files (*.png)\tAll Files (*)"), 0);
	if(newfile != NULL)
	{
		scheme = fl_input(gettext("Enter color scheme"),MGL_DEF_SCH);
		if(scheme)	e->var->Export(newfile,scheme);
	}
}
//-----------------------------------------------------------------------------
void imp_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *scheme, *newfile = fl_file_chooser(gettext("Import Data?"),
		gettext("PNG Files (*.png)\tAll Files (*)"), 0);
	if (newfile != NULL)
	{
		scheme = fl_input(gettext("Enter color scheme"),MGL_DEF_SCH);
		if(scheme)
		{	e->var->Import(newfile,scheme);	e->refresh();	}
	}
}
//-----------------------------------------------------------------------------
void list_dat_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	mglData *d = e->var;
	if(d->nx*d->ny+d->ny>1020)
	{	fl_message(gettext("Too many numbers (>1000) on slice"));	return;	}
	if(d->nz>1)	fl_message(gettext("Only current slice will be inserted"));
	char *list = new char[16384];
	strcpy(list,"list\t");
	register long i,j;
	char s[32];
	for(j=0;j<d->ny;j++)
	{
		for(i=0;i<d->nx;i++)
		{
			snprintf(s,32,"%g\t",d->a[i+d->nx*(j+e->get_slice()*d->ny)]);
			strcat(list,s);
		}
		if(j<d->ny-1)	strcat(list,"|\t");
	}
	textbuf->insert(0,list);
	delete []list;
}
//-----------------------------------------------------------------------------
void modify_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	const char *eq=fl_input(gettext("Enter formula for data modification\nHere x, y, z in range [0,1], u is data value"),0);
	if (eq != NULL)	{	e->var->Modify(eq);	e->refresh();	}
}
//-----------------------------------------------------------------------------
void plot_dat_cb(Fl_Widget *, void *v);
//-----------------------------------------------------------------------------
struct NrmDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Value_Input *min, *max;
	Fl_Choice *dir;
	Fl_Check_Button *sym;
	NrmDlg()	{	memset(this,0,sizeof(NrmDlg));	create_dlg();	}
	~NrmDlg()	{	delete wnd;	}
	void create_dlg();
} nrm_dlg;
//-----------------------------------------------------------------------------
void nrm_dlg_cb(Fl_Widget *, void *v)
{	nrm_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void NrmDlg::create_dlg()
{
	Fl_Menu_Item k[]={{"x"}, {"y"}, { "z"}, {0}};
	wnd = new Fl_Double_Window(135, 215);
	min = new Fl_Value_Input(10, 25, 115, 25, gettext("Minimal value (v1)"));
	min->align(FL_ALIGN_TOP_LEFT);
	min->tooltip(gettext("Minimal value for resulting data values"));
	max = new Fl_Value_Input(10, 70, 115, 25, gettext("Maximal value (v2)"));
	max->align(FL_ALIGN_TOP_LEFT);
	max->tooltip(gettext("Maximal value for resulting data values"));
	dir = new Fl_Choice(10, 115, 115, 25, gettext("Direction"));
	dir->align(FL_ALIGN_TOP_LEFT);	dir->copy(k);
	dir->tooltip(gettext("Direction along which data will be filled"));
	sym = new Fl_Check_Button(10, 115, 115, 25, gettext("Symetrical range"));
	sym->tooltip(gettext("Normalize in symmetrical range: -max(|v1|,|v2|) ... max(|v1|,|v2|)"));

	Fl_Button *o;
	o = new Fl_Button(25, 150, 85, 25, gettext("Cancel"));	o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Do nothing and close this window"));
	o = new Fl_Return_Button(25, 180, 85, 25, gettext("Change"));o->callback(nrm_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Change data values and close this window"));
	wnd->end();
}
//-----------------------------------------------------------------------------
void fill_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	nrm_dlg.OK = false;			nrm_dlg.wnd->label(gettext("Fill in range"));
	nrm_dlg.dir->show();		nrm_dlg.sym->hide();
	nrm_dlg.wnd->set_modal();	nrm_dlg.wnd->show();
	while(nrm_dlg.wnd->shown())	Fl::wait();
	if(nrm_dlg.OK)
	{
		char r='x';
		if(nrm_dlg.dir->value()==1)	r='y';
		if(nrm_dlg.dir->value()==2)	r='z';
		e->var->Fill(nrm_dlg.min->value(),nrm_dlg.max->value(),r);
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
void normal_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	nrm_dlg.OK = false;			nrm_dlg.wnd->label(gettext("Normalize data"));
	nrm_dlg.dir->hide();		nrm_dlg.sym->show();
	nrm_dlg.wnd->set_modal();	nrm_dlg.wnd->show();
	while(nrm_dlg.wnd->shown())	Fl::wait();
	if(nrm_dlg.OK)
	{
		e->var->Norm(nrm_dlg.min->value(), nrm_dlg.max->value(), nrm_dlg.sym->value());
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
struct CropDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Input *x1,*x2, *y1,*y2, *z1,*z2;
	CropDlg()	{	memset(this,0,sizeof(CropDlg));	create_dlg();	}
	~CropDlg()	{	delete wnd;	}
	void create_dlg();
} crop_dlg;
//-----------------------------------------------------------------------------
void crop_dlg_cb(Fl_Widget *, void *v)
{	crop_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void CropDlg::create_dlg()
{
	wnd = new Fl_Double_Window(230, 155, gettext("Crop data"));
	x1 = new Fl_Input(45, 25, 80, 25, gettext("Lower bound"));	x1->align(FL_ALIGN_TOP);
	x2 = new Fl_Input(140, 25, 80, 25, gettext("Upper bound"));	x2->align(FL_ALIGN_TOP);
	y1 = new Fl_Input(45, 55, 80, 25);
	y2 = new Fl_Input(140, 55, 80, 25);
	z1 = new Fl_Input(45, 85, 80, 25);
	z2 = new Fl_Input(140, 85, 80, 25);

	new Fl_Box(15, 25, 25, 25, "X");
	new Fl_Box(15, 55, 25, 25, "Y");
	new Fl_Box(15, 85, 25, 25, "Z");
	Fl_Button *o;
	o = new Fl_Button(45, 120, 75, 25, gettext("Cancel"));		o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Do nothing and close this window"));
	o = new Fl_Return_Button(145, 120, 75, 25, gettext("Crop"));	o->callback(crop_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Change data values and close this window"));
	wnd->end();
}
//-----------------------------------------------------------------------------
void crop_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	crop_dlg.OK = false;
	crop_dlg.wnd->set_modal();	crop_dlg.wnd->show();
	while(crop_dlg.wnd->shown())	Fl::wait();
	if(crop_dlg.OK)
	{
		int n1,n2;
		n1 = 0;	n2 = e->var->nx;
		if(crop_dlg.x1->value()[0])	n1 = atoi(crop_dlg.x1->value());
		if(crop_dlg.x2->value()[0])	n2 = atoi(crop_dlg.x2->value());
		e->var->Crop(n1, n2, 'x');
		n1 = 0;	n2 = e->var->ny;
		if(crop_dlg.y1->value()[0])	n1 = atoi(crop_dlg.y1->value());
		if(crop_dlg.y2->value()[0])	n2 = atoi(crop_dlg.y2->value());
		e->var->Crop(n1, n2, 'y');
		n1 = 0;	n2 = e->var->nz;
		if(crop_dlg.z1->value()[0])	n1 = atoi(crop_dlg.z1->value());
		if(crop_dlg.z2->value()[0])	n2 = atoi(crop_dlg.z2->value());
		e->var->Crop(n1, n2, 'z');
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
struct TrspDlg
{
	Fl_Window *wnd;
	bool OK;

	Fl_Round_Button *xyz, *xzy, *yxz, *yzx, *zxy, *zyx;
	TrspDlg()	{	memset(this,0,sizeof(TrspDlg));	create_dlg();	}
	~TrspDlg()	{	delete wnd;	}
	void create_dlg();
} trsp_dlg;
//-----------------------------------------------------------------------------
void trsp_dlg_cb(Fl_Widget *, void *v)
{	trsp_dlg.OK = true;	((Fl_Window *)v)->hide();	}
//-----------------------------------------------------------------------------
void trsp_rad_cb(Fl_Widget *w, void *v)
{
	TrspDlg* e = (TrspDlg*)v;
	e->xyz->value(0);	e->xzy->value(0);
	e->yxz->value(0);	e->yzx->value(0);
	e->zxy->value(0);	e->zyx->value(0);
	((Fl_Round_Button *)w)->setonly();
}
//-----------------------------------------------------------------------------
void TrspDlg::create_dlg()
{
	wnd = new Fl_Double_Window(220, 170, gettext("Transpose data"));
	Fl_Group *g = new Fl_Group(10, 30, 200, 90, gettext("Select new order of dimensions"));
	g->box(FL_DOWN_BOX);
	yxz = new Fl_Round_Button(20, 40, 75, 25, "y - x - z");	yxz->callback(trsp_rad_cb,this);
	zyx = new Fl_Round_Button(20, 65, 75, 25, "z - y - x");	zyx->callback(trsp_rad_cb,this);
	zxy = new Fl_Round_Button(20, 90, 75, 25, "z - x - y");	zxy->callback(trsp_rad_cb,this);
	yzx = new Fl_Round_Button(100, 40, 75, 25, "y - z - x");yzx->callback(trsp_rad_cb,this);
	xzy = new Fl_Round_Button(100, 65, 75, 25, "x - z - y");xzy->callback(trsp_rad_cb,this);
	xyz = new Fl_Round_Button(100, 90, 75, 25, "x - y - z");xyz->callback(trsp_rad_cb,this);
	g->end();

	Fl_Button *o;
	o = new Fl_Button(25, 130, 75, 25, gettext("Cancel"));	o->callback(close_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Do nothing and close this window"));
	o = new Fl_Return_Button(125, 130, 75, 25, gettext("Do"));	o->callback(trsp_dlg_cb,wnd);
	o->box(UDAV_UP_BOX);	o->down_box(UDAV_DOWN_BOX);
	o->tooltip(gettext("Change data values and close this window"));
	wnd->end();
}
//-----------------------------------------------------------------------------
void transp_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	trsp_dlg.OK = false;
	trsp_dlg.wnd->set_modal();	trsp_dlg.wnd->show();
	while(trsp_dlg.wnd->shown())	Fl::wait();
	if(trsp_dlg.OK)
	{
		if(trsp_dlg.xyz->value())	e->var->Transpose("xyz");
		if(trsp_dlg.xzy->value())	e->var->Transpose("xzy");
		if(trsp_dlg.yxz->value())	e->var->Transpose("yxz");
		if(trsp_dlg.yzx->value())	e->var->Transpose("yzx");
		if(trsp_dlg.zxy->value())	e->var->Transpose("zxy");
		if(trsp_dlg.zyx->value())	e->var->Transpose("zyx");
		e->refresh();
	}
}
//-----------------------------------------------------------------------------
void first_sl_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	e->slice->value(0);
	e->set_slice(0);
	e->go_home();
}
//-----------------------------------------------------------------------------
void last_sl_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	e->slice->value(e->num_slice()-1);
	e->set_slice(e->num_slice()-1);
	e->go_home();
}
//-----------------------------------------------------------------------------
void prev_sl_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	int p = int(e->slice->value())-1;
	if(p<0)	p = 0;
	e->slice->value(p);		e->set_slice(p);
	e->go_home();
}
//-----------------------------------------------------------------------------
void next_sl_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	int p = int(e->slice->value())+1;
	if(p>=e->num_slice())	p = e->num_slice()-1;
	e->slice->value(p);		e->set_slice(p);
	e->go_home();
}
//-----------------------------------------------------------------------------
void first_cl_cb(Fl_Widget*, void*v)
{
	TableWindow* e = (TableWindow*)v;
	e->go_home();
}
//-----------------------------------------------------------------------------
void change_sl_cb(Fl_Widget*w, void*v)
{
	TableWindow* e = (TableWindow*)v;
	e->set_slice(long(e->slice->value()));
	e->go_home();
}
//-----------------------------------------------------------------------------
Fl_Menu_Item tablemenu[60] = {
	{ gettext("General"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Load from file"),	0, load_dat_cb },
		{ gettext("Import from PNG"),0, imp_dat_cb },
		{ gettext("Save to file"),	0, save_dat_cb },
		{ gettext("Export to PNG"),	0, exp_dat_cb, 0, FL_MENU_DIVIDER },
		{ gettext("Insert as list"),	0, list_dat_cb },
		{ gettext("Plot data"),		0, plot_dat_cb },
//		{ gettext("Info for data"),	0, info_dat_cb },
		{ 0 },
	{ gettext("Sizes"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Create new"),	0, new_dat_cb },
		{ gettext("Resize"),		0, resize_cb },
		{ gettext("Squeeze"),	0, squeeze_cb },
		{ gettext("Crop"),		0, crop_cb },
		{ gettext("Transpose"),	0, transp_cb },
//		{ gettext("Extend"),		0, extend_cb },
		{ 0 },
	{ gettext("Fill"), 0, 0, 0, FL_SUBMENU },
		{ gettext("By formula"),	0, modify_cb },
		{ gettext("In range"),	0, fill_cb },
		{ gettext("Normalize"),	0, normal_cb },
		{ 0 },
	{ gettext("Change"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Smooth"),		0, smooth_cb },
		{ gettext("CumSum"),		0, cumsum_cb },
		{ gettext("Integrate"),	0, integr_cb },
		{ gettext("Difference"),	0, diff_cb },
		{ gettext("Double diff."),	0, diff2_cb },
		{ gettext("Swap parts"),	0, swap_cb },
		{ 0 },
	{ gettext("Another"), 0, 0, 0, FL_SUBMENU },
//		{ gettext("Histogram of"),	0, hist_cb },
		{ gettext("Summation of"),	0, asum_cb },
		{ gettext("Maximum of"),	0, amax_cb },
		{ gettext("Minimum of"),	0, amin_cb },
		{ 0 },
	{ gettext("Operations"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Add to"),		0, addto_cb },
		{ gettext("Subtract to"),0, subto_cb },
		{ gettext("Multiply by"),0, multo_cb },
		{ gettext("Divide by"),	0, divto_cb },
		{ 0 },
	{ gettext("Navigation"), 0, 0, 0, FL_SUBMENU },
		{ gettext("First slice"), FL_CTRL + FL_F + 1, first_sl_cb },
		{ gettext("Prev slice"), FL_CTRL + FL_F + 2, prev_sl_cb },
		{ gettext("Next slice"), FL_CTRL + FL_F + 3, next_sl_cb },
		{ gettext("Last slice"),	FL_CTRL + FL_F + 4, last_sl_cb, 0, FL_MENU_DIVIDER },
		{ gettext("First cell"), FL_ALT + FL_F + 1, first_cl_cb },
//		{ gettext("Last cell"), FL_ALT + FL_F + 2, last_cl_cb },
//		{ gettext("Center grid"), FL_ALT + FL_F + 3, center_cl_cb },
		{ 0 },
	{ 0 }
};
//-----------------------------------------------------------------------------
#include "xpm/document-new.xpm"
#include "xpm/plot.xpm"
#include "xpm/document-open.xpm"
#include "xpm/document-save.xpm"
#include "xpm/document-import.xpm"
#include "xpm/document-export.xpm"
#include "xpm/format-indent-more.xpm"
#include "xpm/diff.xpm"
#include "xpm/func.xpm"
#include "xpm/size.xpm"
#include "xpm/tran.xpm"
#include "xpm/crop.xpm"
#include "xpm/go-first.xpm"
#include "xpm/go-last.xpm"
TableWindow::TableWindow(int x, int y, int w, int h, const char* t) : Fl_Window(x, y, w, h, t)
{
	var = 0;
//	menu = new Fl_Menu_Bar(0, 0, w, 30);
//	menu->copy(tablemenu, this);
	Fl_Button *o;
	Fl_Group *g;


	g = new Fl_Group(0,0,30,350);
	o = new Fl_Button(0, 0, 25, 25);	o->image(new Fl_Pixmap(document_new_xpm));
	o->callback(new_dat_cb,this);		o->tooltip(gettext("Create new data with zero filling"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 25, 25, 25);	o->image(new Fl_Pixmap(document_open_xpm));
	o->callback(load_dat_cb,this);		o->tooltip(gettext("Load data from file"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 50, 25, 25);	o->image(new Fl_Pixmap(document_import_xpm));
	o->callback(imp_dat_cb,this);		o->tooltip(gettext("Import data from PNG file"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 75, 25, 25);	o->image(new Fl_Pixmap(document_save_xpm));
	o->callback(save_dat_cb,this);		o->tooltip(gettext("Save data to file"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 100, 25, 25);	o->image(new Fl_Pixmap(document_export_xpm));
	o->callback(exp_dat_cb,this);		o->tooltip(gettext("Export data to PNG file"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(0, 130, 25, 25);	o->image(new Fl_Pixmap(format_indent_more_xpm));
	o->callback(list_dat_cb,this);		o->tooltip(gettext("Insert to script as 'list' command"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 155, 25, 25);	o->image(new Fl_Pixmap(plot_xpm));
	o->callback(plot_dat_cb,this);		o->tooltip(gettext("Plot data"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(0, 185, 25, 25);	o->image(new Fl_Pixmap(diff_xpm));
	o->callback(smooth_cb,this);		o->tooltip(gettext("Apply operator (smoothing, integration, difference ...) to data"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 210, 25, 25);	o->image(new Fl_Pixmap(func_xpm));
	o->callback(modify_cb,this);		o->tooltip(gettext("Fill data by formula"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 235, 25, 25);	o->image(new Fl_Pixmap(size_xpm));
	o->callback(resize_cb,this);		o->tooltip(gettext("Resize data with smoothing"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 260, 25, 25);	o->image(new Fl_Pixmap(crop_xpm));
	o->callback(crop_cb,this);		o->tooltip(gettext("Crop (cut off edges) data"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(0, 285, 25, 25);	o->image(new Fl_Pixmap(tran_xpm));
	o->callback(transp_cb,this);		o->tooltip(gettext("Transpose data dimensions"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	g->end();	g->resizable(0);


	g = new Fl_Group(30,0,200,30);
	o = new Fl_Button(30, 0, 25, 25);	o->image(new Fl_Pixmap(go_first_xpm));
	o->callback(first_sl_cb,this);		o->tooltip(gettext("Go to first slice (Ctrl-F1)"));
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	slice = new Fl_Counter(55, 0, 90, 25, 0);	slice->callback(change_sl_cb,this);
	slice->lstep(10);	slice->step(1);	slice->tooltip(gettext("Id of slice on third (z-) dimension"));
//	slice->box(FL_PLASTIC_UP_BOX);//	slice->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(147, 0, 25, 25);	o->image(new Fl_Pixmap(go_last_xpm));
	o->callback(last_sl_cb,this);		o->tooltip(gettext("Go to last slice (Ctrl-F4)"));
	g->end();	g->resizable(0);

	data = new Fl_Data_Table(30,30,w-30,h-30);
	data->row_header(1);	data->row_header_width(70);
	data->row_resize(1);	data->rows(1);
	data->row_height_all(25);
	data->col_header(1);	data->col_header_height(25);
	data->col_resize(1);	data->cols(1);
	data->col_width_all(70);

	end();	resizable(data);
//	callback(close_table_cb, this);
}
//-----------------------------------------------------------------------------
TableWindow::~TableWindow()
{	parent()->deactivate();
	(parent()->parent())->remove(parent());
	parent()->remove(this);	delete parent();	}
//-----------------------------------------------------------------------------
void delete_cb(void *v)	{	if(v)	delete (TableWindow *)v;	}
//-----------------------------------------------------------------------------
void TableWindow::update(mglVar *v)
{
	if(v==0)	return;
	char ss[1024];
	wcstombs(ss,v->s.c_str(),1024);
	label(ss);	v->func = delete_cb;
//	if(var)	var->o = 0;
	var = v; 	v->o = this;
	refresh();
}
//-----------------------------------------------------------------------------
void TableWindow::refresh()
{
	if(var==0)	return;
	deactivate();	nz = var->nz;
	sl = 0;	slice->range(0,nz-1);

	data->rows(var->ny);	data->cols(var->nx);
	data->ny = var->ny;	data->nx = var->nx;
	data->data = var->a;
	activate();
//	show();
}
//-----------------------------------------------------------------------------
void TableWindow::set_slice(long s)
{
	if(s>=0 && s<nz)
	{
		sl = s;
		data->data = var->a + var->nx * var->ny * sl;
		refresh();
	}
}
//-----------------------------------------------------------------------------
void TableWindow::go_home()
{
}
//-----------------------------------------------------------------------------
