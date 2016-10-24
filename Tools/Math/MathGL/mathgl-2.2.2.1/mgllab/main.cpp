/* main.cpp is part of UDAV
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
#include <errno.h>
#include <locale.h>
#include "udav.h"
//-----------------------------------------------------------------------------
#ifndef MGL_DOC_DIR
#ifdef WIN32
#define MGL_DOC_DIR ""
#else
#define MGL_DOC_DIR "/usr/local/share/doc/mathgl/"
#endif
#endif
//-----------------------------------------------------------------------------
char	title[256];
int num_windows = 0, auto_exec=1, plastic_scheme=1, internal_font=0;
Fl_Preferences pref(Fl_Preferences::USER,"abalakin","mgllab");
char *docdir=0;
//-----------------------------------------------------------------------------
void set_title(Fl_Window* w)
{
	if (filename[0] == '\0') strcpy(title, "Untitled");
	else
	{
		char *slash;
		slash = strrchr(filename, '/');
#ifdef WIN32
		if (slash == NULL) slash = strrchr(filename, '\\');
#endif
		if (slash != NULL) strncpy(title, slash + 1,256);
		else strncpy(title, filename,256);
	}
	if (changed) strcat(title, gettext(" (modified)"));
	w->label(title);
}
//-----------------------------------------------------------------------------
void fname_cb(Fl_Widget*, void *v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	char *file = fl_file_chooser(gettext("Insert File Name?"), gettext("All Files (*)"), 0);
	if(file)
	{
		char *str = new char[strlen(file)+4];
		snprintf(str,strlen(file)+4," '%s'",file);
		e->editor->insert(str);
		delete []str;
	}
}
//-----------------------------------------------------------------------------
void new_cb(Fl_Widget*, void*)
{
	if (!check_save()) return;
	filename[0] = '\0';
	textbuf->select(0, textbuf->length());
	textbuf->remove_selection();
	changed = 0;
	textbuf->call_modify_callbacks();
}
//-----------------------------------------------------------------------------
void open_cb(Fl_Widget*, void *v)
{
	if (!check_save()) return;
	char *lastname=0;
	if(*filename==0)	{	pref.get("last_file",lastname,"");	strncpy(filename, lastname,256);	}
	char *newfile = fl_file_chooser(gettext("Open File?"),
		gettext("MGL Files (*.mgl)\tDAT Files (*.{dat,csv})\tAll Files (*)"), filename);
	if(lastname)	free(lastname);
	if(newfile != NULL)
	{
		load_file(newfile, -1);
		if(auto_exec)	((ScriptWindow*)v)->graph->update();
	}
}
//-----------------------------------------------------------------------------
void close_cb(Fl_Widget*, void* v)
{
	Fl_Window* w = (Fl_Window*)v;
	if (num_windows == 1 && !check_save())	return;

	w->hide();
	textbuf->remove_modify_callback(changed_cb, w);
	delete w;
	num_windows--;
	if (!num_windows) exit(0);
}
//-----------------------------------------------------------------------------
void quit_cb(Fl_Widget*, void*)
{
	if (changed && !check_save())	return;
	exit(0);
}
//-----------------------------------------------------------------------------
void save_cb(Fl_Widget*w, void*v)
{
	if (filename[0] == '\0')	{	saveas_cb(w,v);	return;	}	// No filename - get one!
	else save_file(filename);
}
//-----------------------------------------------------------------------------
void saveas_cb(Fl_Widget*, void*)
{
	char *newfile, *fname=0;
	FILE *fp=0;
	while(1)
	{
		newfile = fl_file_chooser(gettext("Save File As?"), "*.mgl", filename);
		if(!newfile || !newfile[0])	break;
		if(!strchr(newfile,'.'))
		{
			if(fname)	delete []fname;
			fname = new char[strlen(newfile)+5];
			strcpy(fname,newfile);	strcat(fname,".mgl");
			newfile = fname;
		}
		fp = fopen(newfile,"r");
		if(fp)
		{
			fclose(fp);
			if(fl_choice(gettext("File is exesist. Overwrite it?"),0,gettext("No"),gettext(" Yes "))==2)
				break;
		}
		else	break;
	}
	if (newfile != NULL)	save_file(newfile);
	if(fname)	delete []fname;
}
//-----------------------------------------------------------------------------
ScriptWindow *new_view();
void view_cb(Fl_Widget*, void*)
{	Fl_Window* w = new_view();	w->show();	}
//-----------------------------------------------------------------------------
void hint_cb(Fl_Widget*, void*)	{}
//-----------------------------------------------------------------------------
Fl_Menu_Item menuitems[] = {
//	{ gettext("File"), 0, 0, 0, FL_SUBMENU },
	{ gettext("File/New File"),			0, new_cb },
	{ gettext("File/Open File..."),		FL_CTRL + 'o', open_cb },
	{ gettext("File/Insert File..."),	FL_CTRL + 'i', insert_cb },
	{ gettext("File/Save File"),			FL_CTRL + 's', save_cb },
	{ gettext("File/Save File As..._"),	FL_CTRL + FL_SHIFT + 's', saveas_cb, 0, FL_MENU_DIVIDER },
/*TODO	{ gettext("Export"), 0, 0, 0, 	FL_SUBMENU },*/
	{ gettext("File/New View"),		FL_ALT + 'w', view_cb },
	{ gettext("File/Close View_"),	FL_CTRL + 'w', close_cb, 0, FL_MENU_DIVIDER },
	{ gettext("File/Exit"),			FL_ALT + 'x', quit_cb },
//		{ 0 },
	{ gettext("Edit"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Cut"),			FL_CTRL + 'x', cut_cb },
		{ gettext("Copy"),			FL_CTRL + 'c', copy_cb },
		{ gettext("Paste"),			FL_CTRL + 'v', paste_cb },
		{ gettext("Delete"),		0, delete_cb, 0, FL_MENU_DIVIDER },
		{ gettext("Insert"), 0, 0, 0, 	FL_SUBMENU },
			{ gettext("options"),	FL_ALT + 'o', option_cb },
			{ gettext("style"),		FL_ALT + 'i', style_cb },
			{ gettext("filename"),	0, fname_cb },
			{ gettext("command"),	FL_ALT + 'c', command_cb },
			{ 0 },
		{ gettext("Properties"),	0, settings_cb },
		{ 0 },
	{ gettext("Search"), 0, 0, 0, FL_SUBMENU },
		{ gettext("Find..."),		FL_CTRL + 'f', find_cb },
		{ gettext("Find Again"),	FL_F + 3, find2_cb },
		{ gettext("Replace..."),	FL_CTRL + 'r', replace_cb },
		{ gettext("Replace Again"), FL_F + 4, replace2_cb },
		{ 0 },
/*TODO{ gettext("Graphics"), 0, 0, 0, FL_SUBMENU },*/
/*TODO{ gettext("Data"), 0, 0, 0, FL_SUBMENU },*/
	{ gettext("Help"), 0, 0, 0, FL_SUBMENU },
		{ gettext("MGL Help"),		FL_F + 1, help_cb },
		{ gettext("MGL Examples"),	0, example_cb },
		{ gettext("Hints and FAQ"),	0, hint_cb , 0, FL_MENU_INACTIVE},
		{ gettext("About UDAV"),	0, about_cb },
		{ 0 },
	{ 0 }
};
//-----------------------------------------------------------------------------
void mem_upd_cb(Fl_Widget *, void *v)
{	((ScriptWindow*)v)->mem_init();	}
//-----------------------------------------------------------------------------
ScriptWindow *new_view()
{
	Fl_Tabs* tt;
	Fl_Group *gg;
	ScriptWindow *w = new ScriptWindow(930, 510, title);
	w->begin();
	w->menu = new Fl_Menu_Bar(0, 0, 930, 30);

//	w->menu->add(gettext("File"), 0, 0, 0, FL_SUBMENU);	
	w->menu->add(gettext("File/New File"), "", new_cb);
	w->menu->add(gettext("File/Open File..."), "^o", open_cb, w);
	w->menu->add(gettext("File/Insert File..."),	"^i", insert_cb, w);
	w->menu->add(gettext("File/Save File"), "^s", save_cb, w);
	w->menu->add(gettext("File/Save File As..."), 0, saveas_cb, w, FL_MENU_DIVIDER);
	/*TODO	{ gettext("Export"), 0, 0, 0, 	FL_SUBMENU },*/
	w->menu->add(gettext("File/New View"), "#w", view_cb, w);
	w->menu->add(gettext("File/Close View"), "^w", close_cb, w, FL_MENU_DIVIDER);
	w->menu->add(gettext("File/Exit"), "#x", quit_cb);
//	w->menu->copy(menuitems, w);

	Fl_Tile *t = new Fl_Tile(0,30,930,455);
	tt = new Fl_Tabs(0,30,300,455,0);	tt->box(UDAV_UP_BOX);	w->ltab = tt;
	gg = new Fl_Group(0,30,300,430);	gg->label(gettext("Script"));
	add_editor(w);	gg->end();
	tt->end();

	tt = new Fl_Tabs(300,30,630,455,0);	tt->box(UDAV_UP_BOX);	w->rtab = tt;
	gg = new Fl_Group(300,30,630,430,gettext("Canvas"));
	w->graph = new Fl_MGL(300,30,630,430);	gg->end();
	gg = new Fl_Group(300,30,630,430,gettext("Help"));
	w->ghelp = gg;	add_help(w);	gg->end();
	gg = new Fl_Group(300,30,630,430,gettext("Memory"));
	add_mem(w);		gg->end();
	tt->end();

	w->status = new Fl_Box(0,485,930,25,"Ready");
	w->status->align(FL_ALIGN_LEFT|FL_ALIGN_INSIDE);
	w->status->color(FL_BACKGROUND_COLOR);
	w->status->box(FL_DOWN_BOX);
	w->graph->status = w->status;

	t->end();	w->end();	w->resizable(t);
	tt->callback(mem_upd_cb, w);
	w->callback((Fl_Callback *)close_cb, w);

	num_windows++;
	return w;
}
//-----------------------------------------------------------------------------
void argument_set(int n, const char *s);
int main(int argc, char **argv)
{
//	Fl::lock();
	mgl_ask_func = mgl_ask_fltk;
	char *buf, *buf2, ch;
	pref.get("locale",buf,"ru_RU.cp1251");	setlocale(LC_CTYPE, buf);	free(buf);
	pref.get("plastic_scheme",plastic_scheme,1);
	pref.get("internal_font",internal_font,0);
	pref.get("auto_exec",auto_exec,1);
	pref.get("help_dir",docdir,MGL_DOC_DIR);	// docdir should be freed at exit

	Fl::visual(FL_DOUBLE|FL_RGB);
	if(plastic_scheme) Fl::scheme("gtk+");

#ifdef USE_GETTEXT
//	setlocale (LC_NUMERIC, "");
//	bindtextdomain (PACKAGE, LOCALEDIR);
//	textdomain (PACKAGE);
#endif

	textbuf = new Fl_Text_Buffer;
	style_init();
	ScriptWindow *w = new_view();

	pref.get("font_dir",buf2,"");
	pref.get("font_name",buf,"");
	mgl_load_font(w->graph->FMGL->get_graph(),buf,buf2);
	if(buf)	free(buf);
	if(buf2)	free(buf2);

	buf = 0;
	while(1)
	{
		ch = getopt(argc, argv, "1:2:3:4:5:6:7:8:9:ho:L:");
		if(ch>='1' && ch<='9')	argument_set(ch-'0', optarg);
		else if(ch=='L')	setlocale(LC_CTYPE, optarg);
		else if(ch=='h')
		{
			printf("mglconv convert mgl script to bitmap png file.\nCurrent version is 2.%g\n",MGL_VER2);
			printf("Usage:\tmgllab [parameter(s)] scriptfile\n");
			printf(	"\t-1 str       set str as argument $1 for script\n"
					"\t...          ...\n"
					"\t-9 str       set str as argument $9 for script\n"
					"\t-L loc       set locale to loc\n"
//					"\t-            get script from standard input\n"
					"\t-h           print this message\n" );
			free(docdir);	return 0;
		}
		// NOTE: I will not parse stdin here
		else if(ch==-1 && optind<argc)	buf = argv[optind];
		else if(ch==-1 && optind>=argc)	break;
	}

	w->show(1, argv);
	if(buf && *buf && *buf!='-')
	{
		load_file(buf, -1);
		if(auto_exec)	w->graph->update();
	}
	return Fl::run();
}
//-----------------------------------------------------------------------------
