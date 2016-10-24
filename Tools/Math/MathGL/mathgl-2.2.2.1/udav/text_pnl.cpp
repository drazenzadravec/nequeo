/***************************************************************************
 *   Copyright (C) 2008 by Alexey Balakin                                  *
 *   mathgl.abalakin@gmail.com                                             *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
#include <QPrintDialog>
#include <QMessageBox>
#include <QTextStream>
#include <QFileDialog>
#include <QToolButton>
#include <QCompleter>
#include <QBoxLayout>
#include <QPrinter>
#include <QMenu>
#include "mgl2/qmathgl.h"
//-----------------------------------------------------------------------------
#include "udav_wnd.h"
#include "qmglsyntax.h"
#include "find_dlg.h"
#include "opt_dlg.h"
#include "style_dlg.h"
#include "files_dlg.h"
#include "newcmd_dlg.h"
#include "setup_dlg.h"
#include "text_pnl.h"
#include "plot_pnl.h"
//-----------------------------------------------------------------------------
FilesDialog *files_dlg=0;
QString defFontFamily;
int defFontSize;
bool mglAutoExecute = true;
extern mglParse parser;
extern bool mglCompleter;
QWidget *createDataOpenDlg(QWidget *p);
QString getOpenDataFile(QWidget *w, QString filename);
//-----------------------------------------------------------------------------
TextPanel::TextPanel(QWidget *parent) : QWidget(parent)
{
	printer = new QPrinter;
	findDialog = new FindDialog(this);
	optDialog = new OptionDialog(this);
	stlDialog = new StyleDialog(this);
	newCmdDlg = new NewCmdDialog(this);
	setupDlg = new SetupDialog(this);
	dataOpenDlg = createDataOpenDlg(this);
	if(!files_dlg)	files_dlg= new FilesDialog;

	register int i,n=parser.GetCmdNum();
	for(i=0;i<n;i++) 	words<<QString::fromLocal8Bit(parser.GetCmdName(i));
	vars = words;

	connect(setupDlg, SIGNAL(putText(const QString &)), this, SLOT(animPutText(const QString &)));
	connect(newCmdDlg, SIGNAL(result(const QString&)), this, SLOT(putLine(const QString&)));
	connect(findDialog, SIGNAL(findText(const QString &, bool, bool)), this, SLOT(findText(const QString &, bool, bool)));
	connect(findDialog, SIGNAL(replText(const QString &, const QString &, bool, bool)), this, SLOT(replText(const QString &, const QString &, bool, bool)));

	edit = new TextEdit(this);	edit->setAcceptRichText(false);
	new QMGLSyntax(edit);
	defFontFamily = edit->fontFamily();
	defFontSize = int(edit->fontPointSize());
	edit->setLineWrapMode(QTextEdit::NoWrap);
	setCompleter(mglCompleter);

	QBoxLayout *v,*h;
	menu = new QMenu(tr("Edit"),this);
	v = new QVBoxLayout(this);
	h = new QHBoxLayout();	v->addLayout(h);
	toolTop(h);				v->addWidget(edit);
}
//-----------------------------------------------------------------------------
TextPanel::~TextPanel()	{	delete printer;	}
//-----------------------------------------------------------------------------
void TextPanel::setCompleter(bool en)
{
	if(en)
	{
		QCompleter *completer = new QCompleter(vars, this);
		completer->setCaseSensitivity(Qt::CaseInsensitive);
		completer->setCompletionMode(QCompleter::PopupCompletion);
		edit->setCompleter(completer);
	}
	else	edit->setCompleter(0);
//	completer->setCompletionMode(en ? QCompleter::PopupCompletion : QCompleter::InlineCompletion);
}
//-----------------------------------------------------------------------------
void TextPanel::insNVal()
{
	QString sel=edit->textCursor().selectedText();
	if(sel.isEmpty())
	{
		QMessageBox::warning(this,tr("UDAV"),tr("There is no selection to evaluate."));
		return;
	}
	wchar_t *txt=new wchar_t[sel.length()+1];
	sel.toWCharArray(txt);	txt[sel.length()]=0;
	mglData res=parser.Calc(txt);
	delete []txt;
	edit->textCursor().insertText(QString::number(res.GetVal(0)));
}
//-----------------------------------------------------------------------------
void TextPanel::insFitF()
{
	QString str(graph->getFit());
	if(str.isEmpty())
	{
		QMessageBox::warning(this,tr("UDAV"),tr("There is no fitted formula."));
		return;
	}
	edit->textCursor().insertText("'"+str+"'");
}
//-----------------------------------------------------------------------------
void TextPanel::insFile()
{
	QString str = QFileDialog::getOpenFileName(this, tr("UDAV - Insert filename"));
	if(str.isEmpty())	return;
	edit->textCursor().insertText("'"+str+"'");
}
//-----------------------------------------------------------------------------
void TextPanel::insPath()
{
	QString str = QFileDialog::getExistingDirectory(this, tr("UDAV - Insert filename"));
	if(str.isEmpty())	return;
	edit->textCursor().insertText("'"+str+"'");
}
//-----------------------------------------------------------------------------
void TextPanel::refreshData()
{
	vars=words;
	mglVar *v = parser.FindVar("");
	while(v)
	{
		if(v->s.length()>2)	vars<<QString::fromStdWString(v->s);
		v = v->next;
	}
	setCompleter(mglCompleter);
}
//-----------------------------------------------------------------------------
void TextPanel::printText()
{
	QPrintDialog printDlg(printer, this);
	if (printDlg.exec() == QDialog::Accepted)
	{
		setStatus(tr("Printing..."));
		edit->print(printer);
		setStatus(tr("Printing completed"));
	}
	else	setStatus(tr("Printing aborted"));
}
//-----------------------------------------------------------------------------
void TextPanel::find()
{
	findDialog->show();
	findDialog->raise();
	findDialog->activateWindow();
}
//-----------------------------------------------------------------------------
bool TextPanel::findText(const QString &str, bool cs, bool fw)
{
//	static int para=0, index=0;
	static QTextDocument::FindFlags f;
	static QString stri="";
	if(!str.isEmpty())
	{
		stri = str;
		f = QTextDocument::FindFlags();
		if(fw)	f = f|QTextDocument::FindBackward;
		if(cs)	f = f|QTextDocument::FindCaseSensitively;
	}
	bool res = edit->find(stri, f);
	if(!res)
		QMessageBox::information(this, tr("UDAV - find text"), tr("No string occurrence is found"));
	return res;
}
//-----------------------------------------------------------------------------
void TextPanel::replText(const QString &str, const QString &txt, bool cs, bool fw)
{
	static bool res=false;
	if(str.isEmpty())	{	res = false;	return;	}
	if(res)	edit->textCursor().insertText(txt);
	res = findText(str, cs, fw);
}
//-----------------------------------------------------------------------------
void TextPanel::addOptions()
{
	if(optDialog->exec()==QDialog::Accepted)
	{
		edit->moveCursor(QTextCursor::EndOfLine);
		edit->insertPlainText(optDialog->getOption());
	}
}
//-----------------------------------------------------------------------------
void TextPanel::animPutText(const QString &s)
{	edit->moveCursor(QTextCursor::Start);	edit->insertPlainText(s);	}
//-----------------------------------------------------------------------------
//void TextPanel::putText(const QString &txt)	{	edit->insertPlainText(txt);	}
//-----------------------------------------------------------------------------
void TextPanel::putLine(const QString &txt)
{	edit->moveCursor(QTextCursor::StartOfLine);
	edit->insertPlainText(txt+"\n");	}
//-----------------------------------------------------------------------------
void TextPanel::addStyle()
{
	if(stlDialog->exec()==QDialog::Accepted)
	{
		QString s = edit->textCursor().block().text();
		int i = s.indexOf(';');
		if(i<0)	edit->moveCursor(QTextCursor::EndOfLine);
		else
		{
			edit->moveCursor(QTextCursor::StartOfBlock);
			// foolish way :(
			for(;i>0;i--)	edit->moveCursor(QTextCursor::Left);
		}
		edit->insertPlainText(stlDialog->getStyle());
	}
}
//-----------------------------------------------------------------------------
void TextPanel::setEditorFont(QFont *f)
{	edit->setFont(f ? *f : QFont(defFontFamily, defFontSize));	}
//-----------------------------------------------------------------------------
QString TextPanel::selection()
{	return edit->textCursor().block().text();	}
//-----------------------------------------------------------------------------
void TextPanel::setCursorPosition(int n)
{
	graph->setCurPos(n);
	if(n<0)	return;
	edit->moveCursor(QTextCursor::Start);
	for(int i=0;i<n;i++)	edit->moveCursor(QTextCursor::NextBlock);
	edit->setFocus();
}
//-----------------------------------------------------------------------------
void TextPanel::newCmd(int n)
{
	if(n>0)	setCursorPosition(n-1);
	else if(n==0)	return;
	newCmdDlg->parseCmd(edit->textCursor().block().text());
	newCmdDlg->show();
}
//-----------------------------------------------------------------------------
#if MGL_HAVE_HDF5
#define H5_USE_16_API
#include <hdf5.h>
void TextPanel::loadHDF5(const QString &fileName)
{
	// H5T_C_S1 - C string
	hid_t hf,hg,hd,hs,ht;
	hsize_t dims[3];
	long rank;
	hf = H5Fopen(fileName.toStdString().c_str(), H5F_ACC_RDONLY, H5P_DEFAULT);
	if(!hf)	return;
	hg = H5Gopen(hf, "/");
	hsize_t num, nx, ny, nz, i;
	char name[256];
	H5Gget_num_objs(hg, &num);
	for(i=0;i<num;i++)
	{
		if(H5Gget_objtype_by_idx(hg, i)!=H5G_DATASET)	continue;
		H5Gget_objname_by_idx(hg, i, name, 256);
		hd = H5Dopen(hg,name);	hs = H5Dget_space(hd);
		ht = H5Dget_type(hd);
		rank = H5Sget_simple_extent_ndims(hs);
		if(H5Tget_class(ht)==H5T_STRING)	// load script
		{
			H5Sget_simple_extent_dims(hs,dims,0);
			char *buf = new char[dims[0]+1];
			H5Dread(hd, H5T_C_S1, H5S_ALL, H5S_ALL, H5P_DEFAULT, buf);
			buf[dims[0]]=0;		// to be sure :)
			QString str = buf;
			if(str.contains("#----- End of QMathGL block -----\n"))
			{
				graph->mgl->primitives = str.section("#----- End of QMathGL block -----\n",0,0);
				str = str.section("#----- End of QMathGL block -----\n",1);
			}
			edit->setText(str);
			graph->animParseText(edit->toPlainText());
			setCurrentFile(fileName);
			delete []buf;
			setStatus(tr("Loaded document %1").arg(fileName));
			if(mglAutoExecute)	graph->execute();
		}
		else if(H5Tget_class(ht)==H5T_FLOAT || H5Tget_class(ht)==H5T_INTEGER)
		{
			for(int j=0;name[j];j++)	if(!isalnum(name[j]))	name[j]='_';
			mglVar *v = parser.AddVar(name);
			nx = ny = nz = 1;
			if(rank>0 && rank<=3)
			{
				H5Sget_simple_extent_dims(hs,dims,0);
				switch(rank)
				{
					case 1:	nx=dims[0];	break;
					case 2:	nx=dims[1];	ny=dims[0];	break;
					case 3:	nx=dims[2];	ny=dims[1];	nz=dims[0];	break;
				}
				v->Create(nx, ny, nz);
#if MGL_USE_DOUBLE
				H5Dread(hd, H5T_NATIVE_DOUBLE, H5S_ALL, H5S_ALL, H5P_DEFAULT, v->a);
#else
				H5Dread(hd, H5T_NATIVE_FLOAT, H5S_ALL, H5S_ALL, H5P_DEFAULT, v->a);
#endif
			}
		}
		H5Dclose(hd);	H5Sclose(hs);	H5Tclose(ht);
	}
	H5Gclose(hg);	H5Fclose(hf);
}
//-----------------------------------------------------------------------------
void TextPanel::saveHDF5(const QString &fileName)
{
	hid_t hf,hd,hs;
	hsize_t dims[3];
	long rank = 3;

	H5Eset_auto(0,0);
	hf = H5Fcreate(fileName.toStdString().c_str(), H5F_ACC_TRUNC, H5P_DEFAULT, H5P_DEFAULT);
	if(hf<0)
	{
		setStatus(tr("Could not write to %1").arg(fileName));
		return;
	}
	{	// save script
		QString txt;
		if(!graph->mgl->primitives.isEmpty())
			txt += graph->mgl->primitives + "#----- End of QMathGL block -----";
		txt += edit->toPlainText();
		dims[0] = txt.length()+1;
		char *buf = new char[dims[0]+1];
		memcpy(buf, txt.toStdString().c_str(), dims[0]);
		buf[dims[0]]=0;
		hs = H5Screate_simple(1, dims, 0);
		hd = H5Dcreate(hf, "mgl_script", H5T_C_S1, hs, H5P_DEFAULT);
		H5Dwrite(hd, H5T_C_S1, hs, hs, H5P_DEFAULT, buf);
		H5Dclose(hd);	H5Sclose(hs);
		delete []buf;
	}
	mglVar *v = parser.FindVar("");
	char name[256];
	while(v)
	{
		wcstombs(name,v->s.c_str(),v->s.length()+1);
		if(v->nz==1 && v->ny == 1)
		{	rank = 1;	dims[0] = v->nx;	}
		else if(v->nz==1)
		{	rank = 2;	dims[0] = v->ny;	dims[1] = v->nx;	}
		else
		{	rank = 3;	dims[0] = v->nz;	dims[1] = v->ny;	dims[2] = v->nx;	}
		hs = H5Screate_simple(rank, dims, 0);
		hd = H5Dcreate(hf, name, H5T_IEEE_F32LE, hs, H5P_DEFAULT);

		H5Dwrite(hd, H5T_NATIVE_FLOAT, hs, hs, H5P_DEFAULT, v->a);
		H5Dclose(hd);	H5Sclose(hs);
		v = v->next;
	}
	H5Fclose(hf);
	setCurrentFile(fileName);
	setStatus(tr("File %1 saved").arg(fileName));
	return;
}
#else
void TextPanel::saveHDF5(const QString &fileName){}
void TextPanel::loadHDF5(const QString &fileName){}
#endif
//-----------------------------------------------------------------------------
void TextPanel::load(const QString &fileName)
{
	if(fileName.right(4).toLower()==".dat")
	{
		QString code = getOpenDataFile(dataOpenDlg, fileName);
		if(!code.isEmpty())
		{
			setCurrentFile(fileName.left(fileName.length()-3)+"mgl");
			edit->setText(code);
		}
	}
	else if(fileName.right(4).toLower()==".hdf" || fileName.right(3).toLower()==".h5")
		loadHDF5(fileName);
	else
	{
		QFile f(fileName);
		if(!f.open(QIODevice::ReadOnly))
		{
			QMessageBox::warning(this,tr("UDAV - open file"),
								tr("Couldn't open file ")+"'"+fileName+"'",
								QMessageBox::Ok,0,0);
			return;
		}

		QTextStream ts(&f);
		ts.setAutoDetectUnicode(true);
//		ts.setCodec(QTextCodec::codecForLocale());

		QString str=ts.readAll();
		int narg=0,i=-1;
		if(str.contains('%'))
		{
			while((i = str.indexOf('%',i+1))>0)
			{
				char ch = str.at(i+1).toLatin1();
				if(ch>='1' && ch<='9' && ch-'0'>narg)
					narg = ch-'0';
			}
			if(narg>0)
			{
				files_dlg->setNumFiles(narg);
				if(!files_dlg->exec())	return;	// nothing to do
				str = files_dlg->putFiles(str);
			}
		}
		if(str.contains("#----- End of QMathGL block -----\n"))
		{
			graph->mgl->primitives = str.section("#----- End of QMathGL block -----\n",0,0);
			str = str.section("#----- End of QMathGL block -----\n",1);
		}
		
		if(narg>0)	setCurrentFile(fileName.left(fileName.length()-3)+"mgl");
		edit->setText(str);
		graph->animParseText(edit->toPlainText());
		if(narg==0)	setCurrentFile(fileName);
	}
	setStatus(tr("Loaded document ")+fileName);
	if(mglAutoExecute)	graph->execute();
}
//-----------------------------------------------------------------------------
void TextPanel::save(const QString &fileName)
{
	if(fileName.right(4)==".hdf" || fileName.right(3)==".h5")
	{	saveHDF5(fileName);	return;	}
	QString text;
	if(!graph->mgl->primitives.isEmpty())
		text += graph->mgl->primitives + "#----- End of QMathGL block -----\n";
	text += edit->toPlainText();
	QFile f(fileName);
	if(!f.open(QIODevice::WriteOnly))
	{
		setStatus(tr("Could not write to %1").arg(fileName));
		return;
	}
	QTextStream t(&f);
	t.setAutoDetectUnicode(true);
	t << text;	f.close();
	setCurrentFile(fileName);
	setStatus(tr("File %1 saved").arg(fileName));
}
//-----------------------------------------------------------------------------
void TextPanel::addSetup()	{	setupDlg->exec();	}
//-----------------------------------------------------------------------------
#include "xpm/option.xpm"
#include "xpm/style.xpm"
//-----------------------------------------------------------------------------
void TextPanel::toolTop(QBoxLayout *l)
{
	QAction *a;
	QMenu *o=menu, *oo;
	QToolButton *bb;
	const MainWindow *mw=findMain(this);

	// general buttons
	if(mw)
	{
		bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(mw->aload);
		bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(mw->asave);
	}
	// edit menu
	a = new QAction(QPixmap(":/xpm/edit-undo.png"), tr("Undo"), this);
	connect(a, SIGNAL(triggered()), edit, SLOT(undo()));
	a->setToolTip(tr("Undo editor change (Ctrl+Z)."));
	a->setShortcut(Qt::CTRL+Qt::Key_Z);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	a = new QAction(QPixmap(":/xpm/edit-redo.png"), tr("Redo"), this);
	connect(a, SIGNAL(triggered()), edit, SLOT(redo()));
	a->setToolTip(tr("Redo editor change (Ctrl+Shift+Z)."));
	a->setShortcut(Qt::CTRL+Qt::SHIFT+Qt::Key_Z);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	o->addSeparator();
	o->addAction(tr("Clear all"), edit, SLOT(clear()));
	a = new QAction(QPixmap(":/xpm/edit-cut.png"), tr("Cut text"), this);
	connect(a, SIGNAL(triggered()), edit, SLOT(cut()));
	a->setToolTip(tr("Cut selected text to clipboard (Ctrl+X)."));
	a->setShortcut(Qt::CTRL+Qt::Key_X);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	a = new QAction(QPixmap(":/xpm/edit-copy.png"), tr("Copy text"), this);
	connect(a, SIGNAL(triggered()), edit, SLOT(copy()));
	a->setToolTip(tr("Copy selected text or data to clipboard (Ctrl+C)."));
	a->setShortcut(Qt::CTRL+Qt::Key_C);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	a = new QAction(QPixmap(":/xpm/edit-paste.png"), tr("Paste text"), this);
	connect(a, SIGNAL(triggered()), edit, SLOT(paste()));
	a->setToolTip(tr("Paste text or data from clipboard (Ctrl+V)."));
	a->setShortcut(Qt::CTRL+Qt::Key_V);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	o->addAction(QPixmap(":/xpm/edit-select-all.png"), tr("Select all"), edit, SLOT(selectAll()), Qt::CTRL+Qt::Key_A);
	o->addSeparator();

	a = new QAction(QPixmap(":/xpm/edit-find.png"), tr("Find/Replace"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(find()));
	a->setToolTip(tr("Show dialog for text finding (Ctrl+F)."));
	a->setShortcut(Qt::CTRL+Qt::Key_F);	o->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	a = new QAction(tr("Find next"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(findText()));
	a->setShortcut(Qt::Key_F3);	o->addAction(a);
	o->addSeparator();

	// insert menu
	oo = o->addMenu(tr("Insert"));
	a = new QAction(QPixmap(":/xpm/format-indent-more.png"), tr("New command"), this);
	a->setShortcut(Qt::META+Qt::Key_C);	connect(a, SIGNAL(triggered()), this, SLOT(newCmd()));
	a->setToolTip(tr("Show dialog for new command and put it into the script."));
	oo->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	a = new QAction(tr("Fitted formula"), this);
	a->setShortcut(Qt::META+Qt::Key_F);	connect(a, SIGNAL(triggered()), this, SLOT(insFitF()));
	a->setToolTip(tr("Insert last fitted formula with found coefficients."));
	oo->addAction(a);
	a = new QAction(QPixmap(style_xpm), tr("Plot style"), this);
	a->setShortcut(Qt::META+Qt::Key_S);	connect(a, SIGNAL(triggered()), this, SLOT(addStyle()));
	a->setToolTip(tr("Show dialog for styles and put it into the script.\nStyles define the plot view (color scheme, marks, dashing and so on)."));
	oo->addAction(a);
	a = new QAction(QPixmap(option_xpm), tr("Command options"), this);
	a->setShortcut(Qt::META+Qt::Key_O);	connect(a, SIGNAL(triggered()), this, SLOT(addOptions()));
	a->setToolTip(tr("Show dialog for options and put it into the script.\nOptions are used for additional setup the plot."));
	oo->addAction(a);
	a = new QAction(tr("Numeric value"), this);
	a->setShortcut(Qt::META+Qt::Key_N);	connect(a, SIGNAL(triggered()), this, SLOT(insNVal()));
	a->setToolTip(tr("Replace expression by its numerical value."));
	oo->addAction(a);
	a = new QAction(QPixmap(":/xpm/x-office-spreadsheet.png"), tr("File name"), this);
	a->setShortcut(Qt::META+Qt::Key_P);	connect(a, SIGNAL(triggered()), this, SLOT(insFile()));
	a->setToolTip(tr("Select and insert file name."));
	oo->addAction(a);
	a = new QAction(QPixmap(":/xpm/folder.png"), tr("Folder path"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(insPath()));
	a->setToolTip(tr("Select and insert folder name."));
	oo->addAction(a);

	a = new QAction(QPixmap(":/xpm/document-properties.png"), tr("Graphics setup"), this);
	a->setShortcut(Qt::META+Qt::Key_G);	connect(a, SIGNAL(triggered()), this, SLOT(addSetup()));
	a->setToolTip(tr("Show dialog for plot setup and put code into the script.\nThis dialog setup axis, labels, lighting and other general things."));
	oo->addAction(a);
	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(a);

	l->addStretch(1);
	if(mw)	bb = new QToolButton(this);	l->addWidget(bb);	bb->setDefaultAction(mw->acalc);
}
//-----------------------------------------------------------------------------
