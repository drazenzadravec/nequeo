/* udav.h is part of UDAV
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
//-----------------------------------------------------------------------------
#ifndef _UDAV_H_
#define _UDAV_H_
//-----------------------------------------------------------------------------
#ifdef __MWERKS__
# define FL_DLL
#endif
#ifdef USE_GETTEXT
	#include <libintl.h>
#else
	#define gettext(x)	(x)
#endif
#include <FL/Fl.H>
#include <FL/Fl_Group.H>
#include <FL/Fl_Double_Window.H>
#include <FL/fl_ask.H>
#include <FL/Fl_File_Chooser.H>
#include <FL/Fl_Menu_Bar.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Return_Button.H>
#include <FL/Fl_Text_Buffer.H>
#include <FL/Fl_Text_Editor.H>
#include <FL/Fl_Pixmap.H>
#include <FL/Fl_Counter.H>
#include <Fl/Fl_Scroll.H>
#include <FL/Fl_Tabs.H>
#include <FL/Fl_Help_View.H>
#include <Fl/Fl_Table.H>
#include <Fl/Fl_Round_Button.H>
#include <Fl/Fl_Float_Input.H>
#include <Fl/Fl_Multiline_Input.H>
//-----------------------------------------------------------------------------
#ifdef USE_PLASTIC
	#define UDAV_UP_BOX			FL_PLASTIC_UP_BOX
	#define UDAV_DOWN_BOX		FL_PLASTIC_DOWN_BOX
	#define UDAV_EDIT_BOX		FL_PLASTIC_THIN_DOWN_BOX
	#define UDAV_THIN_UP_BOX	FL_PLASTIC_THIN_UP_BOX
#else
	#define UDAV_UP_BOX			FL_GTK_UP_BOX
	#define UDAV_DOWN_BOX		FL_GTK_DOWN_BOX
	#define UDAV_EDIT_BOX		FL_GTK_DOWN_BOX
	#define UDAV_THIN_UP_BOX	FL_GTK_THIN_UP_BOX
#endif
//-----------------------------------------------------------------------------
#include "mgl2/fltk.h"
//-----------------------------------------------------------------------------
extern mglParse *Parse;
extern Fl_Menu_Item colors[];
extern Fl_Preferences pref;
extern char *docdir;
class Fl_MGL;
//-----------------------------------------------------------------------------
class Fl_Data_Table : public Fl_Table
{
private:
	int row, col;
	Fl_Input* input;
protected:
	void draw_cell(TableContext context, int R, int C, int X, int Y, int W, int H);
	static void event_callback(Fl_Widget*, void*v)
	{	((Fl_Data_Table*)v)->cell_click();	}
	void cell_click();

public:
	mreal *data;
	int nx, ny;

	Fl_Data_Table(int x, int y, int w, int h, const char *l=0);
    ~Fl_Data_Table() { }

	void set_value();
    void rows(int val) { if (input->visible()) input->do_callback(); Fl_Table::rows(val); }
    void cols(int val) { if (input->visible()) input->do_callback(); Fl_Table::cols(val); }
    inline int rows() { return Fl_Table::rows(); }
    inline int cols() { return Fl_Table::cols(); }
};
//-----------------------------------------------------------------------------
struct AnimateDlg
{
	friend void animate_dlg_cb(Fl_Widget *, void *v);
	friend void animate_rad_cb(Fl_Widget *, void *v);
	friend void fill_animate(const char *text);
	friend void animate_put_cb(Fl_Widget *, void *);
public:
	Fl_Window* wnd;
	int OK;
	AnimateDlg()	{	memset(this,0,sizeof(AnimateDlg));	create_dlg();	}
	~AnimateDlg()	{	delete wnd;	}
	void FillResult(Fl_MGL* e);
protected:
	bool swap;
	Fl_Round_Button *rt, *rv;
	Fl_Multiline_Input *txt;
	Fl_Float_Input *x0, *x1, *dx, *dt;
	Fl_Check_Button *save;
	void create_dlg();
};
//-----------------------------------------------------------------------------
class Fl_MGL : public Fl_MGLView, public mglDraw
{
friend class AnimateDlg;
public:
	Fl_Widget *status;		///< StatusBar for mouse coordinates
	const char *AnimBuf;		///< buffer for animation
	const char **AnimS0;
	int AnimNum;
	mreal AnimDelay;

	Fl_MGL(int x, int y, int w, int h, const char *label=0);
	~Fl_MGL();

	/// Drawing itself
	int Draw(mglGraph *);
	/// Update (redraw) plot
	void update();
	/// Set main scr and optional pre scripts for execution
	void scripts(char *scr, char *pre);
	/// Clear scripts internally saved
	void clear_scripts();
	/// Show next frame
	void next_frame();
	/// Show prev frame
	void prev_frame();

protected:
	char *Args[1000], *ArgBuf;
	int NArgs, ArgCur;

	char *script;		///< main script
	char *script_pre;	///< script with settings
};
//-----------------------------------------------------------------------------
struct TableWindow : public Fl_Window
{
public:
	TableWindow(int x, int y, int w, int h, const char* t=0);
	~TableWindow();
	void update(mglVar *v);
	void refresh();
	void set_slice(long s);
	inline long get_slice() { return sl; }
	inline long num_slice()	{	return nz;	}
	void go_home();

	Fl_Data_Table *data;
	Fl_Menu_Bar	*menu;
//	Fl_Output *main;
	Fl_Counter *slice;
	mglData *var;
protected:
//	long nx,ny,nz;
	long nz;
	long sl;		// current slice
	char sl_id[64];	// slice id
};
//-----------------------------------------------------------------------------
class SetupDlg
{
public:
	Fl_Window *wnd;
	bool OK;
	Fl_Input *templ;

	SetupDlg()	{	memset(this,0,sizeof(SetupDlg));	}
	~SetupDlg()	{	delete wnd;	}
	void CreateDlg();
	char *ToScript();
private:
	Fl_Input *xmin, *ymin, *zmin, *cmin;
	Fl_Input *xmax, *ymax, *zmax, *cmax;
	Fl_Input *xorg, *yorg, *zorg;
	Fl_Input *xlab, *ylab, *zlab, *font;
	Fl_Choice *xpos, *ypos, *zpos, *axial, *cid[10];
	Fl_Input *xtik, *ytik, *ztik;
	Fl_Input *xsub, *ysub, *zsub;
	Fl_Input *alphad, *ambient, *basew, *mesh, *size;
	Fl_Check_Button *alpha, *light, *rotate, *lid[10];
	Fl_Input *xid[10], *yid[10], *zid[10], *bid[10];
	Fl_Help_View *code;

	void CreateGen();
	void CreateLid();
};
//-----------------------------------------------------------------------------
class ScriptWindow : public Fl_Double_Window
{
public:
	ScriptWindow(int w, int h, const char* t);
	~ScriptWindow();

	Fl_Window	*replace_dlg;
	Fl_Input	*replace_find;
	Fl_Input	*replace_with;
	Fl_Button	*replace_all;
	Fl_Return_Button	*replace_next;
	Fl_Button	*replace_cancel;
	Fl_Text_Editor		*editor;
	Fl_Menu_Bar	*menu;
	Fl_Tabs *ltab, *rtab;
	Fl_Help_View *hd;
	Fl_Input *link_cmd;
	Fl_Group *ghelp;
	Fl_Browser *var;
	Fl_Box *status;

	void mem_init();
	void mem_pressed(int n);
	SetupDlg 	*setup_dlg;
	char		search[256];
	Fl_MGL		*graph;
};
//-----------------------------------------------------------------------------
// Editor window functions
void find2_cb(Fl_Widget *, void *);
void replall_cb(Fl_Widget *, void *);
void replace2_cb(Fl_Widget *, void *);
void replcan_cb(Fl_Widget *, void *);
void insert_cb(Fl_Widget *, void *);
void paste_cb(Fl_Widget *, void *);
void replace_cb(Fl_Widget *, void *);
void copy_cb(Fl_Widget *, void *);
void cut_cb(Fl_Widget *, void *);
void find_cb(Fl_Widget *, void *);
void delete_cb(Fl_Widget *, void *);
void changed_cb(int, int nInserted, int nDeleted,int, const char*, void* v);
//-----------------------------------------------------------------------------
// General callback functions
void new_cb(Fl_Widget *, void *);
void open_cb(Fl_Widget *, void *);
void save_cb(Fl_Widget*, void*);
void saveas_cb(Fl_Widget*, void*);
void help_cb(Fl_Widget*, void*);
//-----------------------------------------------------------------------------
// Graphical callback functions
void setup_cb(Fl_Widget *, void *);
void style_cb(Fl_Widget *, void *);
void option_cb(Fl_Widget *, void *);
void argument_cb(Fl_Widget *, void *);
void variables_cb(Fl_Widget *, void *);
void settings_cb(Fl_Widget *, void *);
void command_cb(Fl_Widget *, void *);
//-----------------------------------------------------------------------------
// Dialogs callback functions
void close_dlg_cb(Fl_Widget *w, void *);
void font_cb(Fl_Widget *, void *v);
void line_cb(Fl_Widget *, void *v);
void face_cb(Fl_Widget *, void *v);
void data_cb(Fl_Widget *, void *v);
//-----------------------------------------------------------------------------
void style_init(void);
int check_save(void);
void load_file(char *newfile, int ipos);
void save_file(char *newfile);
Fl_Widget *add_editor(ScriptWindow *w);
Fl_Widget *add_mem(ScriptWindow *w);
void set_title(Fl_Window* w);
//-----------------------------------------------------------------------------
// Animation
void animate_cb(Fl_Widget *, void *v);
void fill_animate(const char *text);
//-----------------------------------------------------------------------------
Fl_Widget *add_help(ScriptWindow *w);
void help_cb(Fl_Widget*, void*v);
void example_cb(Fl_Widget*, void*v);
void about_cb(Fl_Widget*, void*);
//-----------------------------------------------------------------------------
void newcmd_cb(Fl_Widget*,void*);
//-----------------------------------------------------------------------------
extern Fl_Text_Buffer	*textbuf;
extern char	filename[256];
extern int	changed;
//-----------------------------------------------------------------------------
#endif
//-----------------------------------------------------------------------------
