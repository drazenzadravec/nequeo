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
#ifdef WIN32
#include <io.h>
#include <direct.h>
#else
#include <unistd.h>
#endif
#include <QUrl>
#include <QFile>
#include <QMenuBar>
#include <QMessageBox>
#include <QApplication>
#include <QSettings>
#include <QSplitter>
#include <QFileDialog>
#include <QStatusBar>
#include <QTextStream>
#include <QDockWidget>
#include <QCloseEvent>
#include <QTextCodec>
#include <QTranslator>
#include <QVariant>
#include <QMimeData>
//-----------------------------------------------------------------------------
#if !defined(WIN32) && !defined(__APPLE__)
#include <X11/Xlib.h>
#endif
//-----------------------------------------------------------------------------
#include <mgl2/qmathgl.h>
#include "udav_wnd.h"
#include "text_pnl.h"
#include "plot_pnl.h"
#include "prop_dlg.h"
#include "qmglsyntax.h"
//-----------------------------------------------------------------------------
extern bool mglAutoExecute;
PropDialog *propDlg=0;
int MainWindow::num_wnd=0;
QStringList recentFiles;
int MaxRecentFiles=5;
bool editPosBottom = false;
bool mglAutoSave = false;
bool mglHighlight = true;
bool mglAutoPure = true;
bool mglCompleter = true;
bool loadInNewWnd = false;
QString pathHelp;
extern mglParse parser;
extern QColor mglColorScheme[10];
extern QString defFontFamily;
extern int defFontSize;
extern QString pathFont;
extern int defWidth, defHeight;
//-----------------------------------------------------------------------------
QWidget *createCalcDlg(QWidget *p, QTextEdit *e);
QDialog *createArgsDlg(QWidget *p);
QWidget *createMemPanel(QWidget *p);
QWidget *createHlpPanel(QWidget *p);
void showHelpMGL(QWidget *hlp, QString s);
void addDataPanel(QWidget *p, QWidget *w, QString name)
{
	MainWindow *wnd = dynamic_cast<MainWindow *>(p);
	if(wnd)	wnd->addPanel(w, name);
}
//-----------------------------------------------------------------------------
#ifndef UDAV_DIR
#ifdef WIN32
#define UDAV_DIR ""
#else
#define UDAV_DIR "/usr/local/share/udav/"
#endif
#endif
//-----------------------------------------------------------------------------
int mgl_cmd_cmp(const void *a, const void *b);
void udavLoadDefCommands();
void udavShowHint(QWidget *);
void mgl_ask_qt(const wchar_t *quest, wchar_t *res);
//-----------------------------------------------------------------------------
int main(int argc, char **argv)
{
//	QCoreApplication::setAttribute(Qt::AA_X11InitThreads);
#if !defined(WIN32) && !defined(__APPLE__)
	// try to fix possible multi-threading errors
	// must be placed before ANY window creation
	XInitThreads();
#endif
	mgl_ask_func = mgl_ask_qt;
	QApplication a(argc, argv);
	QTranslator translator;
//QTextCodec::setCodecForCStrings(QTextCodec::codecForName("UTF-8"));
	QString lang="";
	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	pathHelp = settings.value("/helpPath", MGL_DOC_DIR).toString();
#if defined(WIN32)
	if(pathHelp.isEmpty())	pathHelp = a.applicationDirPath()+"\\";
#endif
	pathFont = settings.value("/userFont", "").toString();
	lang = settings.value("/udavLang", "").toString();
	bool showHint = settings.value("/showHint", true).toBool();
	mglCompleter = settings.value("/completer",  true).toBool();
	settings.endGroup();
	if(pathHelp.isEmpty())	pathHelp=MGL_DOC_DIR;

	if(!lang.isEmpty())
	{
		if(!translator.load("udav_"+lang, UDAV_DIR))
			translator.load("udav_"+lang, pathHelp);
		a.installTranslator(&translator);
	}

	udavLoadDefCommands();
	parser.AllowSetSize(true);
	MainWindow *mw = new MainWindow();
	if(argc>1)
	{
		QTextCodec *codec = QTextCodec::codecForLocale();
		mw->load(codec->toUnicode(argv[1]), true);
	}
	mw->show();
	a.connect(&a, SIGNAL(lastWindowClosed()), &a, SLOT(quit()));
	if(showHint)	udavShowHint(mw);
	return a.exec();
}
//-----------------------------------------------------------------------------
//
//		mgl addon
//
//-----------------------------------------------------------------------------
/*mglCommand udav_base_cmd[] = {
	{L"fplot",L"Plot curve by formula",L"fplot 'func' ['stl'='' num=100]", mgls_fplot, mglc_fplot},
	{L"fsurf",L"Plot surface by formula",L"fsurf 'func' ['stl'='' numx=100 numy=100]", mgls_fsurf, mglc_fsurf},
	{L"fgets",L"Print string from file",L"fgets x y {z} 'fname' [pos=0 'stl'='' size=-1.4]", mgls_fgets, mglc_fgets},
{L"",0,0,0,0}};
//-----------------------------------------------------------------------------
void udavAddCommands(const mglCommand *cmd)	// NOTE it work but I don't how I can use it
{
	int i, mp, mc;
	// determine the number of symbols
	for(i=0;parser.Cmd[i].name[0];i++){};	mp = i;
	for(i=0;cmd[i].name[0];i++){};			mc = i;
	mglCommand *buf = new mglCommand[mp+mc+1];
	memcpy(buf, parser.Cmd, mp*sizeof(mglCommand));
	memcpy(buf+mp, cmd, (mc+1)*sizeof(mglCommand));
	qsort(buf, mp+mc, sizeof(mglCommand), mgl_cmd_cmp);
	if(parser.Cmd!=mgls_base_cmd)	delete []parser.Cmd;
	parser.Cmd = buf;
}*/
//-----------------------------------------------------------------------------
void udavLoadDefCommands()	{}	//{	udavAddCommands(udav_base_cmd);	}
//-----------------------------------------------------------------------------
//
//	Class MainWindow
//
//-----------------------------------------------------------------------------
MainWindow::MainWindow(QWidget *wp) : QMainWindow(wp)
{
	QAction *a;
	setWindowTitle(tr("untitled - UDAV"));
	setAttribute(Qt::WA_DeleteOnClose);

	split = new QSplitter(this);
	ltab = new QTabWidget(split);
	ltab->setMovable(true);	ltab->setTabPosition(QTabWidget::South);
//	ltab->setTabsClosable(true);
	rtab = new QTabWidget(split);
	rtab->setMovable(true);	rtab->setTabPosition(QTabWidget::South);

	messWnd = new QDockWidget(tr("Messages and warnings"),this);
	mess = new TextEdit(this);	messWnd->setWidget(mess);
	messWnd->setAllowedAreas(Qt::TopDockWidgetArea | Qt::BottomDockWidgetArea);
	addDockWidget(Qt::BottomDockWidgetArea, messWnd);
	messWnd->resize(size().width(), 0);	new MessSyntax(mess);
	connect(mess,SIGNAL(cursorPositionChanged()),this,SLOT(messClicked()));
	connect(mess,SIGNAL(selectionChanged()),this,SLOT(messClicked()));

	calcWnd = new QDockWidget(tr("Calculator"),this);

	aload = a = new QAction(QPixmap(":/xpm/document-open.png"), tr("Open file"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(choose()));
	a->setToolTip(tr("Open and execute/show script or data from file (Ctrl+O).\nYou may switch off automatic exection in UDAV properties."));
	a->setShortcut(Qt::CTRL+Qt::Key_O);

	asave = a = new QAction(QPixmap(":/xpm/document-save.png"), tr("Save script"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(save()));
	a->setToolTip(tr("Save script to a file (Ctrl+S)"));
	a->setShortcut(Qt::CTRL+Qt::Key_S);

	acalc = a = new QAction(QPixmap(":/xpm/accessories-calculator.png"), tr("Calculator"), this);
	a->setShortcut(Qt::Key_F4);	a->setCheckable(true);
	connect(a, SIGNAL(toggled(bool)), calcWnd, SLOT(setVisible(bool)));
	connect(calcWnd, SIGNAL(visibilityChanged(bool)), a, SLOT(setChecked(bool)));
	a->setToolTip(tr("Show calculator which evaluate and help to type textual formulas.\nTextual formulas may contain data variables too."));
	a->setChecked(false);	calcWnd->setVisible(false);

	ainfo = a = new QAction(tr("Show info"), this);
	a->setShortcut(Qt::Key_F2);	a->setCheckable(true);
	connect(a, SIGNAL(toggled(bool)), messWnd, SLOT(setVisible(bool)));
	connect(messWnd, SIGNAL(visibilityChanged(bool)), a, SLOT(setChecked(bool)));
	a->setChecked(false);	messWnd->setVisible(false);

	graph = new PlotPanel(this);
	rtab->addTab(graph,QPixmap(":/xpm/x-office-presentation.png"),tr("Canvas"));
	//	connect(info,SIGNAL(addPanel(QWidget*)),this,SLOT(addPanel(QWidget*)));
	info = createMemPanel(this);
	rtab->addTab(info,QPixmap(":/xpm/system-file-manager.png"),tr("Info"));
	hlp = createHlpPanel(this);
	rtab->addTab(hlp,QPixmap(":/xpm/help-contents.png"),tr("Help"));
	edit = new TextPanel(this);	edit->graph = graph;
	graph->textMGL = edit->edit;
	connect(graph->mgl,SIGNAL(showWarn(QString)),mess,SLOT(setText(QString)));
	connect(graph,SIGNAL(clearWarn()),mess,SLOT(clear()));
	ltab->addTab(edit,QPixmap(":/xpm/text-x-generic.png"),tr("Script"));

	calcWnd->setWidget(createCalcDlg(this, edit->edit));
	calcWnd->setAllowedAreas(Qt::TopDockWidgetArea | Qt::BottomDockWidgetArea);
	addDockWidget(Qt::BottomDockWidgetArea, calcWnd);
	calcWnd->resize(size().width(), 200);

	makeMenu();
	setCentralWidget(split);
	setWindowIcon(QIcon(":/udav.png"));
	readSettings();
	if(!propDlg)	propDlg = new PropDialog;

	connect(graph, SIGNAL(save()), this, SLOT(save()));
	connect(graph, SIGNAL(setStatus(const QString &)), this, SLOT(setStatus(const QString &)));
	connect(graph, SIGNAL(animPutText(const QString &)), edit, SLOT(animPutText(const QString &)));
	connect(graph,SIGNAL(giveFocus()),edit->edit,SLOT(setFocus()));
	connect(graph->mgl, SIGNAL(objChanged(int)), edit, SLOT(setCursorPosition(int)));
//	connect(graph->mgl, SIGNAL(posChanged(QString)), statusBar(), SLOT(showMessage(QString)));
	connect(graph->mgl, SIGNAL(refreshData()), this, SLOT(refresh()));
	connect(graph->mgl, SIGNAL(refreshData()), edit, SLOT(refreshData()));
	connect(graph->mgl,SIGNAL(doubleClick(int)),edit,SLOT(newCmd(int)));

	connect(mess, SIGNAL(textChanged()), this, SLOT(warnChanged()));
//	connect(mdi, SIGNAL(subWindowActivated(QMdiSubWindow *)), this, SLOT(subActivated(QMdiSubWindow *)));
	connect(propDlg, SIGNAL(sizeChanged(int,int)), graph->mgl, SLOT(imgSize(int,int)));
//	connect(propDlg, SIGNAL(propUpdated()), edit->edit, SLOT(update()));	// TODO: update qmglsyntax
	connect(edit->edit,SIGNAL(textChanged()), this, SLOT(setAsterix()));
	connect(edit->edit, SIGNAL(cursorPositionChanged()), this, SLOT(editPosChanged()));
	connect(edit,SIGNAL(setCurrentFile(QString)),this,SLOT(setCurrentFile(QString)));
	connect(edit,SIGNAL(setStatus(QString)),this,SLOT(setStatus(QString)));

	setStatus(tr("Ready"));
	num_wnd++;
	edit->setAcceptDrops(false);	// for disabling default action by 'edit'
	setAcceptDrops(true);
}
//-----------------------------------------------------------------------------
void MainWindow::makeMenu()
{
	QAction *a;
	QMenu *o;

	// file menu
	{
	o = menuBar()->addMenu(tr("File"));
	a = new QAction(QPixmap(":/xpm/document-new.png"), tr("New script"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(newDoc()));
	a->setToolTip(tr("Create new empty script window (Ctrl+N)."));
	a->setShortcut(Qt::CTRL+Qt::Key_N);	o->addAction(a);

	o->addAction(aload);
	o->addAction(asave);

	a = new QAction(tr("Save as ..."), this);
	connect(a, SIGNAL(triggered()), this, SLOT(saveAs()));
	o->addAction(a);

	o->addSeparator();
	o->addAction(tr("Print script"), edit, SLOT(printText()));
	a = new QAction(QPixmap(":/xpm/document-print.png"), tr("Print graphics"), this);
	connect(a, SIGNAL(triggered()), graph->mgl, SLOT(print()));
	a->setToolTip(tr("Open printer dialog and print graphics (Ctrl+P)"));
	a->setShortcut(Qt::CTRL+Qt::Key_P);	o->addAction(a);
	o->addSeparator();
	fileMenu = o->addMenu(tr("Recent files"));
	o->addSeparator();
	o->addAction(tr("Quit"), qApp, SLOT(closeAllWindows()));
	}

	menuBar()->addMenu(edit->menu);
	menuBar()->addMenu(graph->menu);

	// settings menu
	{
	o = menuBar()->addMenu(tr("Settings"));
	a = new QAction(QPixmap(":/xpm/preferences-system.png"), tr("Properties"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(properties()));
	a->setToolTip(tr("Show dialog for UDAV properties."));	o->addAction(a);
	o->addAction(tr("Set arguments"), createArgsDlg(this), SLOT(exec()));

	o->addAction(acalc);
	o->addAction(ainfo);
	}

	menuBar()->addSeparator();
	o = menuBar()->addMenu(tr("Help"));
	a = new QAction(QPixmap(":/xpm/help-contents.png"), tr("MGL help"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(showHelp()));
	a->setToolTip(tr("Show help on MGL commands (F1)."));
	a->setShortcut(Qt::Key_F1);	o->addAction(a);
	a = new QAction(QPixmap(":/xpm/help-faq.png"), tr("Hints"), this);
	connect(a, SIGNAL(triggered()), this, SLOT(showHint()));
	a->setToolTip(tr("Show hints of MGL usage."));	o->addAction(a);
	o->addAction(tr("About"), this, SLOT(about()));
	o->addAction(tr("About Qt"), this, SLOT(aboutQt()));
}
//-----------------------------------------------------------------------------
void MainWindow::closeEvent(QCloseEvent* ce)
{
	bool ok=true;
	writeSettings();
	if(edit->isModified())
		switch(QMessageBox::information(this, tr("UDAV"),
				tr("Do you want to save the changes to the document?"),
				QMessageBox::Yes, QMessageBox::No, QMessageBox::Cancel))
		{
			case QMessageBox::Yes:	save();	break;
			case QMessageBox::No:	break;
			default:	ok=false;	break;
		}
	if(ok)
	{
		num_wnd--;
		ce->accept();
		if(num_wnd==0)	QApplication::quit();
	}
	else	ce->ignore();
}
//-----------------------------------------------------------------------------
void MainWindow::dropEvent(QDropEvent * de)
{
	// Linux Qt send "text/plain" mime data in drop event
	// Windows version send "text/uri-list"
	QTextCodec *codec = QTextCodec::codecForLocale();
	QString filename;
	if ( de->mimeData()->hasFormat("text/plain") )
	{
		// Under linux just convert the text from the local encodig to Qt's unicode
		filename = codec->toUnicode(de->mimeData()->data("text/plain"));
		if (filename.indexOf("file:") == 0)
			load(filename.remove("file://").trimmed(), false);
	}else
	if ( de->mimeData()->hasUrls() )
	{
		// Under win - parse the dropped data and find the path to local file
		QList<QUrl> UrlList;
		QFileInfo finfo;
		UrlList = de->mimeData()->urls();
		if ( UrlList.size() > 0) // if at least one QUrl is present in list
		{
			filename = UrlList[0].toLocalFile(); // convert first QUrl to local path
			finfo.setFile( filename );
			if ( finfo.isFile() )
			{
				load(filename, false);
			}
		}
	}
}
//-----------------------------------------------------------------------------
void MainWindow::dragEnterEvent(QDragEnterEvent *event)
{
	QTextCodec *codec = QTextCodec::codecForLocale();
	QString filename = codec->toUnicode(event->mimeData()->data("text/plain"));
	/*if ( event->provides("text/plain") )
	{
		QTextCodec *codec = QTextCodec::codecForLocale();
		QString instring = codec->toUnicode(event->mimeData()->data("text/plain"));
		if ( instring.indexOf("file://") >= 0)
		{
			event->acceptProposedAction();
			setStatus(instring);
		}
	}
	else */
	if(event->mimeData()->hasUrls())
	{
		QList<QUrl> UrlList;
		QFileInfo finfo;
		UrlList = event->mimeData()->urls();
		if ( UrlList.size() > 0) // if at least one QUrl is present in list
		{
			filename = UrlList[0].toLocalFile(); // convert first QUrl to local path
			finfo.setFile(filename);
			if ( finfo.isFile() )
			{
				event->acceptProposedAction();
				setStatus(filename);
			}
		}
	}
}
//-----------------------------------------------------------------------------
void MainWindow::showHelp()
{
	QString s = edit->selection(), dlm(" #;:\t");
	int i, n = s.length();
	for(i=0;i<n;i++)	if(dlm.contains(s[i]))	break;
	s.truncate(i);
//	s = s.section(' ',0);
	showHelpMGL(hlp,s);
}
//-----------------------------------------------------------------------------
int mgl_cmd_cmp(const void *a, const void *b);
void MainWindow::editPosChanged()
{
	register int i, n;
	QString text = edit->selection(), dlm(" #;:\t");
	n = text.length();
	for(i=0;i<n;i++)	if(dlm.contains(text[i]))	break;
	text.truncate(i);

	const char *desc = parser.CmdDesc(text.toStdString().c_str());
	const char *form = parser.CmdFormat(text.toStdString().c_str());
	if(form)	setStatus(QString::fromLocal8Bit(desc)+": "+QString::fromLocal8Bit(form));
	else	setStatus(tr("Not recognized"));
}
//-----------------------------------------------------------------------------
void MainWindow::setEditPos(bool bottom)
{	split->setOrientation(bottom ? Qt::Vertical : Qt::Horizontal);	}
//-----------------------------------------------------------------------------
void MainWindow::properties()	{	propDlg->exec();	}
//-----------------------------------------------------------------------------
void MainWindow::messClicked()
{
	QString m = mess->toPlainText(), q;
	int p = mess->textCursor().blockNumber();
	for(;p>=0;p--)
	{
		q = m.section('\n',p,p);
		if(q.contains("in line "))
		{
			QString s = q.section(' ',-1);
			int n = s.toInt()-1;	if(n<0)	return;
			edit->moveCursor(QTextCursor::Start);
			for(int i=0;i<n;i++)	edit->moveCursor(QTextCursor::NextBlock);
			break;
		}
	}
	edit->setFocus();
}
//-----------------------------------------------------------------------------
void MainWindow::warnChanged()
{
	if(mess->toPlainText().isEmpty())
	{	messWnd->hide();	ainfo->setChecked(false);	}
	else
	{	messWnd->show();	ainfo->setChecked(true);	}
}
//-----------------------------------------------------------------------------
void MainWindow::about()
{
	QString s = tr("UDAV v. 2.")+QString::number(MGL_VER2)+
				tr("\n(c) Alexey Balakin, 2007-2014\nhttp://mathgl.sf.net/");
	QMessageBox::about(this, tr("UDAV - about"), s);
}
//-----------------------------------------------------------------------------
void MainWindow::aboutQt()
{	QMessageBox::aboutQt(this, tr("About Qt"));	}
//-----------------------------------------------------------------------------
void MainWindow::writeSettings()
{
	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	settings.setValue("/animDelay", animDelay);
	settings.setValue("/geometry/size", size());
//	settings.setValue("/geometry/dock", messWnd->size());
	settings.setValue("/geometry/split_e/w1", split->sizes().at(0));
	settings.setValue("/geometry/split_e/w2", split->sizes().at(1));

	settings.setValue("/recentFiles", recentFiles);
	settings.setValue("/recentFilesMax", MaxRecentFiles);
	settings.setValue("/helpPath", pathHelp);
	settings.setValue("/userFont", pathFont);
	settings.setValue("/colComment",mglColorScheme[0].name());
	settings.setValue("/colString", mglColorScheme[1].name());
	settings.setValue("/colKeyword",mglColorScheme[2].name());
	settings.setValue("/colOption", mglColorScheme[3].name());
	settings.setValue("/colSuffix", mglColorScheme[4].name());
	settings.setValue("/colNumber", mglColorScheme[5].name());
	settings.setValue("/colACKeyword", mglColorScheme[6].name());
	settings.setValue("/colFCKeyword", mglColorScheme[7].name());
	settings.setValue("/colReserved", mglColorScheme[8].name());
	settings.setValue("/colCurrLine", mglColorScheme[9].name());
	settings.setValue("/autoExec",  mglAutoExecute);
	settings.setValue("/autoSave",  mglAutoSave);
	settings.setValue("/highlight",  mglHighlight);
	settings.setValue("/autoPure",  mglAutoPure);
	settings.setValue("/editAtTop", editPosBottom);
	settings.setValue("/fontFamily", defFontFamily);
	settings.setValue("/fontSize", defFontSize);
	settings.setValue("/loadInNewWnd", loadInNewWnd);
	settings.setValue("/completer",  mglCompleter);
	settings.endGroup();
}
//-----------------------------------------------------------------------------
void MainWindow::readSettings()
{
	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	pathHelp = settings.value("/helpPath", MGL_DOC_DIR).toString();
	if(pathHelp.isEmpty())	pathHelp=MGL_DOC_DIR;
	MaxRecentFiles = settings.value("/recentFilesMax", 5).toInt();
	animDelay = settings.value("/animDelay", 500).toInt();
	resize(settings.value("/geometry/size", QSize(880,720)).toSize());
	QList<int> le;
	le.append(settings.value("/geometry/split_e/w1", 230).toInt());
	le.append(settings.value("/geometry/split_e/w2", 604).toInt());
	split->setSizes(le);

	pathFont = settings.value("/userFont", "").toString();
	mglColorScheme[0] = QColor(settings.value("/colComment","#007F00").toString());
	mglColorScheme[1] = QColor(settings.value("/colString", "#FF0000").toString());
	mglColorScheme[2] = QColor(settings.value("/colKeyword","#00007F").toString());
	mglColorScheme[3] = QColor(settings.value("/colOption", "#7F0000").toString());
	mglColorScheme[4] = QColor(settings.value("/colSuffix", "#7F0000").toString());
	mglColorScheme[5] = QColor(settings.value("/colNumber", "#0000FF").toString());
	mglColorScheme[6] = QColor(settings.value("/colACKeyword","#7F007F").toString());
	mglColorScheme[7] = QColor(settings.value("/colFCKeyword","#007F7F").toString());
	mglColorScheme[8] = QColor(settings.value("/colReserved", "#0000FF").toString());
	mglColorScheme[9] = QColor(settings.value("/colCurrLine", "#FFFFCC").toString());
	mglAutoSave = settings.value("/autoSave",  false).toBool();
	mglHighlight = settings.value("/highlight",  true).toBool();
	mglAutoPure = settings.value("/autoPure",  true).toBool();
	mglAutoExecute = settings.value("/autoExec",  true).toBool();
	editPosBottom = settings.value("/editAtTop", false).toBool();
	mglCompleter = settings.value("/completer",  true).toBool();
	loadInNewWnd = settings.value("/loadInNewWnd", false).toBool();
	defFontFamily = settings.value("/fontFamily", "Georgia").toString();
	defFontSize = settings.value("/fontSize", 10).toInt();
	edit->setEditorFont();	setEditPos(editPosBottom);
	graph->setMGLFont(pathFont);

	defWidth = settings.value("/defWidth", 640).toInt();
	defHeight = settings.value("/defHeight", 480).toInt();
	graph->mgl->setSize(defWidth, defHeight);

	recentFiles = settings.value("/recentFiles").toStringList();
	settings.endGroup();
	updateRecentFileItems();
}
//-----------------------------------------------------------------------------
void MainWindow::setStatus(const QString &txt)
{	statusBar()->showMessage(txt, 5000);	}
//-----------------------------------------------------------------------------
void MainWindow::setCurrentFile(const QString &fileName)
{
	filename = fileName;
	mgl_set_plotid(graph->mgl->getGraph(), fileName.toStdString().c_str());
	edit->setModified(false);
	if(filename.isEmpty())
		setWindowTitle(tr("untitled - UDAV"));
	else
	{
		setWindowTitle(QFileInfo(filename).fileName()+tr(" - UDAV"));
		int i = recentFiles.indexOf(filename);
		if(i>=0)	recentFiles.removeAt(i);
		recentFiles.push_front(filename);
		updateRecentFileItems();
		if(chdir(qPrintable(QFileInfo(filename).path())))
			QMessageBox::warning(this, tr("UDAV - save current"),
				tr("Couldn't change to folder ")+QFileInfo(filename).path());
	}
}
//-----------------------------------------------------------------------------
void MainWindow::openRecentFile()
{
	QAction *a = qobject_cast<QAction *>(sender());
	if(!a)	return;
	if(edit->isModified())
		switch(QMessageBox::information(this, tr("UDAV - save current"),
				tr("Do you want to save the changes to the document?"),
				QMessageBox::Yes, QMessageBox::No, QMessageBox::Cancel))
		{
			case QMessageBox::Yes:	save();	break;
			case QMessageBox::No:	break;
			default:	return;
		}
	QString fn = recentFiles[a->data().toInt()];
	if(!fn.isEmpty())	load(fn);
}
//-----------------------------------------------------------------------------
void MainWindow::updateRecentFileItems()
{
	foreach(QWidget *w, QApplication::topLevelWidgets())
	{
		MainWindow *wnd = qobject_cast<MainWindow *>(w);
		if(wnd)	wnd->updateRecent();
	}
}
//-----------------------------------------------------------------------------
void MainWindow::updateRecent()
{
	QAction *a;
	fileMenu->clear();	qApp->processEvents();
	for(int i=0; i<recentFiles.size() && i<MaxRecentFiles; i++)
	{
		QString text="&"+QString::number(i+1)+" "+QFileInfo(recentFiles[i]).fileName();
		a = fileMenu->addAction(text, this, SLOT(openRecentFile()));
		a->setData(i);
	}
}
//-----------------------------------------------------------------------------
void MainWindow::newDoc()
{
	MainWindow *ed = new MainWindow;
	ed->show();	ed->activateWindow();
}
//-----------------------------------------------------------------------------
void MainWindow::choose()
{
	if(edit->isModified())
		switch(QMessageBox::information(this, tr("UDAV - save current"),
				tr("Do you want to save the changes to the document?"),
				QMessageBox::Yes, QMessageBox::No, QMessageBox::Cancel))
		{
			case QMessageBox::Yes:	save();	break;
			case QMessageBox::No:	break;
			default:	return;
		}
	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	QString fn = QFileDialog::getOpenFileName(this,
			tr("UDAV - Open file"),
			settings.value("/filePath", MGL_DOC_DIR).toString(),
			tr("MGL scripts (*.mgl)\nHDF5 files (*.hdf *.h5)\nText files (*.txt)\nData files (*.dat)\nAll files (*.*)"));
	settings.endGroup();
	if(!fn.isEmpty())	load(fn);
	else	setStatus(tr("Loading aborted"));
}
//-----------------------------------------------------------------------------
void MainWindow::load(const QString &fileName, bool noNewWnd)
{
	// save current path
	QFileInfo fi(fileName);		filename = fileName;
	QSettings settings("udav","UDAV");
	settings.setPath(QSettings::IniFormat, QSettings::UserScope, "UDAV");
	settings.beginGroup("/UDAV");
	settings.setValue("/filePath", fi.absolutePath());
	settings.endGroup();
	// open new window if it is required
	if(loadInNewWnd && !noNewWnd)
	{
		MainWindow *mw = new MainWindow;
		mw->edit->load(fileName);
		mw->show();	//ed->activateWindow();
	}
	else	edit->load(fileName);
}
//-----------------------------------------------------------------------------
void MainWindow::save()
{
	if(filename.isEmpty())	saveAs();
	else	edit->save(filename);
}
//-----------------------------------------------------------------------------
void MainWindow::saveAs()
{
	QString fn;
	fn = QFileDialog::getSaveFileName(this, tr("UDAV - save file"), "",
			tr("MGL scripts (*.mgl)\nHDF5 files (*.hdf *.h5)\nAll files (*.*)"));
	if(fn.isEmpty())
	{	setStatus(tr("Saving aborted"));	return;	}
	else
	{
		int nn=fn.length();
		if(fn[nn-4]!='.' && fn[nn-3]!='.')	fn = fn + ".mgl";
		filename = fn;		save();
	}
}
//-----------------------------------------------------------------------------
void MainWindow::setAsterix()
{
	if(edit->isModified())
	{
		if(filename.isEmpty())
			setWindowTitle(tr("untitled* - UDAV"));
		else
			setWindowTitle(QFileInfo(filename).fileName()+tr("* - UDAV"));
	}
	else
	{
		if(filename.isEmpty())
			setWindowTitle(tr("untitled - UDAV"));
		else
			setWindowTitle(QFileInfo(filename).fileName()+tr(" - UDAV"));
	}
}
//-----------------------------------------------------------------------------
void updateDataItems()
{
	foreach (QWidget *w, QApplication::topLevelWidgets())
		if(w->inherits("MainWindow"))	((MainWindow *)w)->refresh();
}
//-----------------------------------------------------------------------------
void MainWindow::addPanel(QWidget *w, QString name)
{
	ltab->addTab(w,QPixmap(":/xpm/x-office-spreadsheet.png"),name);
	ltab->setCurrentWidget(w);
}
//-----------------------------------------------------------------------------
MainWindow *findMain(QWidget *wnd)
{
	MainWindow *mw=0;
	QObject *w=wnd;

	while(w)
	{
		mw = dynamic_cast<MainWindow *>(w);
		if(mw)	break;	else w = w->parent();
	}
	return mw;
}
//-----------------------------------------------------------------------------
void raisePanel(QWidget *w)
{
	MainWindow *mw=findMain(w);
	if(mw)	mw->rtab->setCurrentWidget(w);
}
//-----------------------------------------------------------------------------
