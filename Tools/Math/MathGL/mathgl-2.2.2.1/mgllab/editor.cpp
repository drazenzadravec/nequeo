/* editor.cpp is part of UDAV
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
#ifdef __MWERKS__
# define FL_DLL
#endif
#include "udav.h"
//-----------------------------------------------------------------------------
int		changed = 0;
char	filename[256] = "";
Fl_Text_Buffer	*textbuf = 0;
void data_file(char *v);
//-----------------------------------------------------------------------------
// Syntax highlighting stuff...
Fl_Text_Buffer	 *stylebuf = 0;
Fl_Text_Display::Style_Table_Entry styletable[] = {	// Style table
		{ FL_BLACK,		FL_COURIER,			14, 0 },	// A - Plain
		{ FL_DARK_GREEN,FL_COURIER_ITALIC,	14, 0 },	// B - Line comments
		{ FL_BLUE,		FL_COURIER,			14, 0 },	// C - Number
		{ FL_RED,		FL_COURIER,			14, 0 },	// D - Strings
		{ FL_DARK_BLUE,	FL_COURIER_BOLD,	14, 0 },	// E - Usual ommand
		{ FL_DARK_CYAN,	FL_COURIER_BOLD,	14, 0 },	// F - Flow command
		{ FL_DARK_MAGENTA,	FL_COURIER_BOLD,14, 0 },	// G - New-data command
		{ FL_DARK_RED,	FL_COURIER,	14, 0 },	// H - Option
		{ FL_DARK_GREEN,FL_COURIER_BOLD,	14, 0 }};  // I - Inactive command
//-----------------------------------------------------------------------------
bool is_sfx(const char *s)	// suffix
{
	register long i,n=strlen(s);
	for(i=0;i<n && s[i]>='a';i++);
	if(i==1 && s[0]=='a')	return true;
	if(i==2 && strchr("axyz",s[1]) && strchr("nmawsk",s[0]))	return true;
	if(i==3 && (!strncmp("fst",s,3) || !strncmp("lst",s,3) || !strncmp("max",s,3) ||
				!strncmp("min",s,3) || !strncmp("sum",s,3)))
		return true;
	return false;
//	char *t = new char[i+1];	memcpy(t,s,i*sizeof(char));	t[i]=0;
}
//-----------------------------------------------------------------------------
bool is_opt(const char *s)	// option
{
	const char *o[]={"xrange","yrange","zrange","crange","alpha",
					"cut","value","meshnum","size","legend"};
	int l[10] = {6, 6, 6, 6, 5, 3, 5, 7, 4, 6};
	register long i;
	for(i=0;i<10;i++)	if(!strncmp(o[i],s,l[i]) && s[l[i]]<=' ')	return true;
	return false;
}
//-----------------------------------------------------------------------------
bool is_num(const char *s)	// number
{
	register long i,n=strlen(s);
	if(s[0]==':' && (s[1]<=' ' || s[1]==';'))	return true;
	if(n>=2 && !strncmp("pi",s,2) && (s[2]<=' ' || s[2]==';' || s[2]==':'))	return true;
	if(n>=2 && !strncmp("on",s,2) && (s[2]<=' ' || s[2]==';' || s[2]==':'))	return true;
	if(n>=3 && !strncmp("off",s,3) && (s[3]<=' ' || s[3]==';' || s[2]==':'))	return true;
	if(n>=3 && !strncmp("nan",s,3) && (s[3]<=' ' || s[3]==';' || s[2]==':'))	return true;
	for(i=0;i<n;i++)
	{
		if(s[i]<=' ' || s[i]==';')	break;
		if(!strchr("+-.eE0123456789",s[i]))	return false;
	}
	return true;
//	char *t = new char[i+1];	memcpy(t,s,i*sizeof(char));	t[i]=0;
}
//-----------------------------------------------------------------------------
char is_cmd(const char *s)	// command
{
	register long i,n=strlen(s)+1;
	char res=0, *w=new char[n];	strcpy(w,s);
	for(i=0;i<n;i++)	if(!isalnum(s[i]))	w[i]=0;
	int rts = Parse->CmdType(w);
	if(rts==5)		res = 'G';
	else if(rts==7)	res = 'F';
	else if(rts)	res = 'E';
	delete []w;		return res;
}
//-----------------------------------------------------------------------------
// Parse text and produce style data.
void style_parse(const char *text, char *style, int /*length*/)
{
	register long i;
	long n=strlen(text);
	bool nl=true, arg=true;
	// Style letters:
	// A - Plain
	// B - Line comments
	// C - Number
	// D - Strings
	// E - Usual command
	// F - Flow command
	// G - New data command
	// H - Option

	for(i=0;i<n;i++)
	{
		style[i] = 'A';
		if(text[i]=='#')	// comment
			for(;i<n && text[i]!='\n';i++)	style[i]='B';
		if(text[i]=='\'')	// string
		{
			arg = false;	style[i]='D';	i++;
			for(;i<n && text[i]!='\n' && text[i]!='\'';i++)	style[i]='D';
			style[i]='D';
		}
		if(text[i]=='\n' || text[i]==':')	{	nl=true;	style[i]='A';	continue;	}
		char r = is_cmd(text+i);
		if(nl && r)	// command name
		{	for(;i<n && isalnum(text[i]);i++)	style[i]=r;		i--;	}
		if(text[i]<=' ' || text[i]==';')	{	arg = true;	continue;	}
		if(arg && is_opt(text+i))	// option
		{	for(;i<n && isalpha(text[i]);i++)	style[i]='H';	i--;	}
		if(arg && is_num(text+i))	// number
		{
			if(text[i]==':' && (isspace(text[i+1]) || text[i+1]==':'))
				style[i]='C';
			else for(;i<n && strchr("+-.eE0123456789pionaf",text[i]) ;i++)
				style[i]='C';
			i--;
		}
		if(text[i]=='.' && is_sfx(text+i+1))	// option (suffix)
		{
			style[i]='H';	i++;
			for(;i<n && isalpha(text[i]);i++)	style[i]='H';
		}
		nl = arg = false;
	}
}
//-----------------------------------------------------------------------------
// Initialize the style buffer...
void style_init(void)
{
	char *style = new char[textbuf->length() + 1];
	char *text = textbuf->text();
	memset(style, 'A', textbuf->length());
	style[textbuf->length()] = '\0';
	if (!stylebuf) stylebuf = new Fl_Text_Buffer(textbuf->length());
	style_parse(text, style, textbuf->length());
	stylebuf->text(style);
	delete[] style;
	free(text);
}
//-----------------------------------------------------------------------------
// Update unfinished styles.
void style_unfinished_cb(int, void*) {}
//-----------------------------------------------------------------------------
// Update the style buffer...
void style_update(int pos,		// Position of update
				int nInserted,	// Number of inserted chars
				int nDeleted,	// Number of deleted chars
				int	/*nRestyled*/,			// Number of restyled chars
				const char */*deletedText*/,// Text that was deleted
				void *cbArg)	// Callback data
{
	long	start, end;	// Start and end of text
	char last,		// Last style on line
		*style,		// Style data
		*text;		// Text data

	// If this is just a selection change, just unselect the style buffer...
	if (nInserted == 0 && nDeleted == 0) {	stylebuf->unselect();	return;  }
	// Track changes in the text buffer...
	if (nInserted > 0)
	{
		// Insert characters into the style buffer...
		style = new char[nInserted + 1];
		memset(style, 'A', nInserted);
		style[nInserted] = '\0';

		stylebuf->replace(pos, pos + nDeleted, style);
		delete[] style;
	}
	else	// Just delete characters in the style buffer...
		stylebuf->remove(pos, pos + nDeleted);

	// Select the area that was just updated to avoid unnecessary callbacks...
	stylebuf->select(pos, pos + nInserted - nDeleted);

	// Re-parse the changed region; we do this by parsing from the
	// beginning of the previous line of the changed region to the end of
	// the line of the changed region...  Then we check the last
	// style character and keep updating if we have a multi-line
	// comment character...
	start = textbuf->line_start(pos);
	end   = textbuf->line_end(pos + nInserted);
	text  = textbuf->text_range(start, end);
	style = stylebuf->text_range(start, end);
	if (start==end)	last = 0;
	else	last = style[end-start-1];
	style_parse(text, style, end - start);
	stylebuf->replace(start, end, style);
	((Fl_Text_Editor *)cbArg)->redisplay_range(start, end);

	if (start==end || last != style[end-start-1])
	{
		// Either the user deleted some text, or the last character on
		// the line changed styles, so reparse the remainder of the buffer...
		free(text);	free(style);

		end   = textbuf->length();
		text  = textbuf->text_range(start, end);
		style = stylebuf->text_range(start, end);
		style_parse(text, style, end - start);
		stylebuf->replace(start, end, style);
		((Fl_Text_Editor *)cbArg)->redisplay_range(start, end);
	}
	free(text);	free(style);
}
//-----------------------------------------------------------------------------
ScriptWindow::ScriptWindow(int w, int h, const char* t) : Fl_Double_Window(w, h, t)
{
	replace_dlg = new Fl_Window(300, 105, gettext("Replace"));
	replace_find = new Fl_Input(80, 10, 210, 25, gettext("Find:"));
	replace_find->align(FL_ALIGN_LEFT);

	replace_with = new Fl_Input(80, 40, 210, 25, gettext("Replace:"));
	replace_with->align(FL_ALIGN_LEFT);

	replace_all = new Fl_Button(10, 70, 90, 25, gettext("Replace All"));
	replace_all->callback((Fl_Callback *)replall_cb, this);
	replace_all->box(UDAV_UP_BOX);	replace_all->down_box(UDAV_DOWN_BOX);

	replace_next = new Fl_Return_Button(105, 70, 120, 25, "Replace Next");
	replace_next->callback((Fl_Callback *)replace2_cb, this);
	replace_next->box(UDAV_UP_BOX);	replace_next->down_box(UDAV_DOWN_BOX);

	replace_cancel = new Fl_Button(230, 70, 60, 25, gettext("Cancel"));
	replace_cancel->callback((Fl_Callback *)replcan_cb, this);
	replace_cancel->box(UDAV_UP_BOX);	replace_cancel->down_box(UDAV_DOWN_BOX);

	replace_dlg->end();
	replace_dlg->set_non_modal();
	editor = 0;		*search = 0;

	setup_dlg = new SetupDlg;
	setup_dlg->CreateDlg();
}
//-----------------------------------------------------------------------------
ScriptWindow::~ScriptWindow()
{
	delete replace_dlg;
	delete setup_dlg->wnd;
}
//-----------------------------------------------------------------------------
int check_save(void)
{
  if (!changed) return 1;
  int r = fl_choice(gettext("The current file has not been saved.\n"
					"Would you like to save it now?"),
					gettext("Cancel"), gettext("Save"), gettext("Don't Save"));
  if(r==1)	{	save_cb(0,0);	return !changed;	} // Save the file...
  return (r==2) ? 1 : 0;
}
//-----------------------------------------------------------------------------
int loading = 0;
void load_file(char *newfile, int ipos)
{
	long len = strlen(newfile);
	pref.set("last_file",newfile);
	if(ipos==-1 && (!strcmp(newfile+len-4,".dat") || !strcmp(newfile+len-4,".csv")))
	{
		data_file(newfile);
		strncpy(newfile+len-4,".mgl",4);
		strncpy(filename, newfile,256);
	}
	else
	{
		loading = 1;
		int insert = (ipos != -1);
		changed = insert;
		if(!insert) *filename=0;
		long r;
		if(!insert)	r = textbuf->loadfile(newfile);
		else r = textbuf->insertfile(newfile, ipos);

		char *t = textbuf->text();
#ifndef WIN32
		register size_t i,l=strlen(t);
		for(i=0;i<l;i++)	if(t[i]=='\r')	t[i]=' ';
		textbuf->text(t);
#endif
		fill_animate(t);	free(t);

		if (r)
			fl_alert(gettext("Error reading from file \'%s\':\n%s."), newfile, strerror(errno));
		else	if(!insert)	strncpy(filename, newfile,256);
		loading = 0;
		textbuf->call_modify_callbacks();
	}
}
//-----------------------------------------------------------------------------
void save_file(char *newfile)
{
	pref.set("last_file",newfile);
	if (textbuf->savefile(newfile))
		fl_alert(gettext("Error writing to file \'%s\':\n%s."), newfile, strerror(errno));
	else
		strncpy(filename, newfile,256);
	changed = 0;
	textbuf->call_modify_callbacks();
}
//-----------------------------------------------------------------------------
void copy_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	Fl_Text_Editor::kf_copy(0, e->editor);
}
//-----------------------------------------------------------------------------
void cut_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	Fl_Text_Editor::kf_cut(0, e->editor);
}
//-----------------------------------------------------------------------------
void delete_cb(Fl_Widget*, void*) {	textbuf->remove_selection();	}
//-----------------------------------------------------------------------------
void find_cb(Fl_Widget* w, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	const char *val;
	val = fl_input(gettext("Search String:"), e->search);
	if (val != NULL) {	strncpy(e->search, val,256);	find2_cb(w, v);	}
}
//-----------------------------------------------------------------------------
void find2_cb(Fl_Widget* w, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	if (e->search[0] == '\0')	{	find_cb(w, v);	return;	}

	int pos = e->editor->insert_position();
	long found = textbuf->search_forward(pos, e->search, &pos);
	if (found) {
		// Found a match; select and update the position...
		textbuf->select(pos, pos+strlen(e->search));
		e->editor->insert_position(pos+strlen(e->search));
		e->editor->show_insert_position();
	}
	else fl_alert(gettext("No occurrences of \'%s\' found!"), e->search);
}
//-----------------------------------------------------------------------------
void changed_cb(int, int nInserted, int nDeleted,int, const char*, void* v)
{
	if ((nInserted || nDeleted) && !loading) changed = 1;
	ScriptWindow *w = (ScriptWindow *)v;
	set_title(w);
	if (loading) w->editor->show_insert_position();
}
//-----------------------------------------------------------------------------
void insert_cb(Fl_Widget*, void *v)
{
	char *newfile = fl_file_chooser(gettext("Insert File?"), "*", filename);
	ScriptWindow *w = (ScriptWindow *)v;
	if (newfile != NULL) load_file(newfile, w->editor->insert_position());
}
//-----------------------------------------------------------------------------
void paste_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	Fl_Text_Editor::kf_paste(0, e->editor);
}
//-----------------------------------------------------------------------------
void replace_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	e->replace_dlg->show();
}
//-----------------------------------------------------------------------------
void replace2_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	const char *find = e->replace_find->value();
	const char *replace = e->replace_with->value();
	if (find[0] == '\0')	{	e->replace_dlg->show();	return;	}
	e->replace_dlg->hide();

	int pos = e->editor->insert_position();
	long found = textbuf->search_forward(pos, find, &pos);
	if (found)
	{
		// Found a match; update the position and replace text...
		textbuf->select(pos, pos+strlen(find));
		textbuf->remove_selection();
		textbuf->insert(pos, replace);
		textbuf->select(pos, pos+strlen(replace));
		e->editor->insert_position(pos+strlen(replace));
		e->editor->show_insert_position();
	}
	else fl_alert(gettext("No occurrences of \'%s\' found!"), find);
}
//-----------------------------------------------------------------------------
void replall_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	const char *find = e->replace_find->value();
	const char *replace = e->replace_with->value();

	find = e->replace_find->value();
	if (find[0] == '\0')	{	e->replace_dlg->show();	return;	}
	e->replace_dlg->hide();
	e->editor->insert_position(0);
	long times = 0;

	// Loop through the whole string
	for (long found = 1; found;)
	{
		int pos = e->editor->insert_position();
		found = textbuf->search_forward(pos, find, &pos);
		if (found)
		{
			// Found a match; update the position and replace text...
			textbuf->select(pos, pos+strlen(find));
			textbuf->remove_selection();
			textbuf->insert(pos, replace);
			e->editor->insert_position(pos+strlen(replace));
			e->editor->show_insert_position();
			times++;
		}
	}
	if (times) fl_message(gettext("Replaced %ld occurrences."), times);
	else fl_alert(gettext("No occurrences of \'%s\' found!"), find);
}
//-----------------------------------------------------------------------------
void replcan_cb(Fl_Widget*, void* v)
{
	ScriptWindow* e = (ScriptWindow*)v;
	e->replace_dlg->hide();
}
//-----------------------------------------------------------------------------
void view_cb(Fl_Widget*, void*);
//#include "xpm/window.xpm"
//#include "xpm/option.xpm"
//#include "xpm/table.xpm"
#include "xpm/plot.xpm"
#include "xpm/help-contents.xpm"
#include "xpm/edit-cut.xpm"
#include "xpm/edit-copy.xpm"
#include "xpm/edit-paste.xpm"
#include "xpm/edit-find.xpm"
#include "xpm/document-open.xpm"
#include "xpm/document-new.xpm"
#include "xpm/document-save.xpm"
Fl_Widget *add_editor(ScriptWindow *w)
{
	Fl_Window *w1=new Fl_Window(0,30,300,430,0);
	Fl_Group *g = new Fl_Group(0,0,290,30);
	Fl_Button *o;

	o = new Fl_Button(0, 1, 25, 25);	o->image(new Fl_Pixmap(document_new_xpm));
	o->tooltip(gettext("New script"));	o->callback(new_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(25, 1, 25, 25);	o->tooltip(gettext("Open script or data file"));
	o->image(new Fl_Pixmap(document_open_xpm));	o->callback(open_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(50, 1, 25, 25);	o->tooltip(gettext("Save script to file"));
	o->image(new Fl_Pixmap(document_save_xpm));	o->callback(save_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(80, 1, 25, 25);	o->tooltip(gettext("Cut selection to clipboard"));
	o->image(new Fl_Pixmap(edit_cut_xpm));	o->callback(cut_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(105, 1, 25, 25);	o->tooltip(gettext("Copy selection to clipboard"));
	o->image(new Fl_Pixmap(edit_copy_xpm));	o->callback(copy_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(130, 1, 25, 25);	o->tooltip(gettext("Paste text from clipboard"));
	o->image(new Fl_Pixmap(edit_paste_xpm));	o->callback(paste_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(155, 1, 25, 25);	o->tooltip(gettext("Find text"));
	o->image(new Fl_Pixmap(edit_find_xpm));	o->callback(find_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);

	o = new Fl_Button(185, 1, 25, 25);	o->tooltip(gettext("Insert MGL command"));
	o->image(new Fl_Pixmap(plot_xpm));	o->callback(command_cb,w);
	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
//	o = new Fl_Button(185, 1, 25, 25);	o->tooltip(gettext("Insert command options"));
//	o->image(new Fl_Pixmap(option_xpm));	o->callback(option_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
//	o = new Fl_Button(210, 1, 25, 25);	o->tooltip(gettext("Edit data array"));
//	o->image(new Fl_Pixmap(table_xpm));	o->callback(table_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
//	o = new Fl_Button(235, 1, 25, 25);	o->tooltip(gettext("New view window"));
//	o->image(new Fl_Pixmap(window_xpm));o->callback(view_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	o = new Fl_Button(210, 1, 25, 25);	o->tooltip(gettext("Show help window"));
	o->image(new Fl_Pixmap(help_contents_xpm));	o->callback(help_cb,w);
//	o->box(FL_PLASTIC_UP_BOX);	o->down_box(FL_PLASTIC_DOWN_BOX);
	g->end();	g->resizable(0);

	w->editor = new Fl_Text_Editor(0, 28, 300, 400);
	w->editor->buffer(textbuf);
	w->editor->highlight_data(stylebuf, styletable, sizeof(styletable) / sizeof(styletable[0]), 'A', style_unfinished_cb, 0);
	w->editor->textfont(FL_COURIER);

	textbuf->add_modify_callback(style_update, w->editor);
	textbuf->add_modify_callback(changed_cb, w);
	textbuf->call_modify_callbacks();

	w1->end();
	w1->resizable(w->editor);	//w->graph);
	return w1;
}
//-----------------------------------------------------------------------------
