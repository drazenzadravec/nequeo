/***************************************************************************
 * window.h is part of Math Graphic Library
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
#ifndef _MGL_QMATHGL_H_
#define _MGL_QMATHGL_H_
//-----------------------------------------------------------------------------
#include <string>
#include <QWidget>
#include <QPixmap>
#include <mgl2/wnd.h>
//-----------------------------------------------------------------------------
class QTextEdit;
class QMenu;
class QMainWindow;
class QScrollArea;
class QSpinBox;
class QTimer;
class mglCanvas;
//-----------------------------------------------------------------------------
/// Class is Qt widget which display MathGL graphics
class MGL_EXPORT QMathGL : public QWidget
{
	Q_OBJECT
public:
	friend void *mgl_qt_thr(void *);
	QString appName; 	///< Application name for message boxes
	bool autoResize; 	///< Allow auto resizing (default is false)
	bool enableMouse;	///< Enable mouse handlers
	bool enableWheel;	///< Enable mouse wheel handlers
	QString primitives;	///< Manual primitives, defined by user

	QMathGL(QWidget *parent = 0, Qt::WindowFlags f = 0);
	virtual ~QMathGL();
	double getRatio();
	void setPopup(QMenu *p)	{	popup = p;	}	///< Set popup menu pointer
	void setSize(int w, int h);		///< Set window/picture sizes
	void setGraph(HMGL GR);	///< Set grapher object
	inline void setGraph(mglGraph *GR)
	{	setGraph(GR->Self());	}
	inline HMGL getGraph()	{	return (HMGL)gr;	}
	/// Set drawing functions and its parameter
	void setDraw(int (*func)(mglBase *gr, void *par), void *par);
	void setDraw(mglDraw *dr);
	inline void setDraw(int (*func)(mglGraph *gr))
	{	setDraw(func?mgl_draw_graph:0,(void*)func);	}
	inline void zoomRegion(mreal xx1,mreal xx2,mreal yy1, mreal yy2)
	{	x1=xx1;	y1=yy1;	x2=xx2;	y2=yy2;	}

	int getPer()	{return int(per);}	///< Get perspective value
	int getPhi()	{return int(phi);}	///< Get Phi-angle value
	int getTet()	{return int(tet);}	///< Get Theta-angle value
	bool getAlpha()	{return alpha;}	///< Get transparency state
	bool getLight()	{return light;}	///< Get lightning state
	bool getZoom()	{return zoom;}	///< Get mouse zooming state
	bool getRotate(){return rotate;}	///< Get mouse rotation state
	bool getViewYZ(){return viewYZ;}	///< Get mouse rotation axis
	bool isActive(int xs,int ys);	///< Check if active point is pressed

public slots:
	void refresh();
	void update();			///< Update picture
	void copy();			///< copy graphics to clipboard
	void copyClickCoor();	///< copy click coordinates to clipboard
	void print();			///< Print plot
	void stop();			///< Stop execution
	void setPer(int p);		///< Set perspective value
	void setPhi(int p);		///< Set Phi-angle value
	void setTet(int t);		///< Set Theta-angle value
	void setAlpha(bool a);	///< Switch on/off transparency
	void setLight(bool l);	///< Switch on/off lightning
	void setGrid(bool r);	///< Switch on/off grid drawing
	void imgSize(int w, int h);	///< Set image size
	void setViewYZ(bool v);	///< Switch on/off rotation around Y and Z axis

	void setCustZoom(bool a);	///< Switch on/off using custom zoom
	void setCustDraw(bool a);	///< Switch on/off using custom draw
	void setZoom(bool z);	///< Switch on/off mouse zooming
	void setRotate(bool r);	///< Switch on/off mouse rotation
	void zoomIn();			///< Zoom in graphics
	void zoomOut();			///< Zoom out graphics
	void restore();			///< Restore zoom and rotation to default values
	//	void reload();			///< Reload data and execute script

	void shiftLeft();		///< Shift graphics to left direction
	void shiftRight();		///< Shift graphics to right direction
	void shiftUp();			///< Shift graphics to up direction
	void shiftDown();		///< Shift graphics to down direction

	void exportPNG(QString fname="");	///< export to PNG file
	void exportPNGs(QString fname="");	///< export to PNG file (no transparency)
	void exportGIF(QString fname="");	///< export to GIF file
	void exportJPG(QString fname="");	///< export to JPEG file
	void exportBPS(QString fname="");	///< export to bitmap EPS file
	void exportEPS(QString fname="");	///< export to vector EPS file
	void exportSVG(QString fname="");	///< export to SVG file
	void exportTEX(QString fname="");	///< export to SVG file
	void exportTGA(QString fname="");	///< export to TGA file

	void exportXYZ(QString fname="");	///< export to XYZ file
	void exportOBJ(QString fname="");	///< export to OBJ file
	void exportSTL(QString fname="");	///< export to STL file
	void exportOFF(QString fname="");	///< export to OFF file
//	void exportX3D(QString fname="");	///< export to XYZ file
	void exportPRC(QString fname="");	///< export to PRC file
	void exportMGLD(QString fname="");	///< export to MGLD file
	void setMGLFont(QString path);		///< restore/load font for graphics

	void addMark();						///< add marker into primitives
	void addLine();						///< add line into primitives
	void addRect();						///< add rectangle into primitives
	void addCurve();					///< add curve into primitives
	void addRhomb();					///< add rhombus into primitives
	void addEllipse();					///< add ellipse into primitives
	void addText(QString txt="");		///< add text into primitives
	void setStyle(int id, QString stl);///< set style for primitive with id

	void adjust();		///< Adjust plot size to fill entire window
	void nextSlide();	///< Show next slide
	void prevSlide();	///< Show previous slide
	void animation(bool st=true);	///< Start animation
	void about();		///< Show about information
	void aboutQt();		///< Show information about Qt version

signals:
	void gridChanged(int);		///< Grid drawing changed (by mouse or by toolbar)
	void phiChanged(int);		///< Phi angle changed (by mouse or by toolbar)
	void tetChanged(int);		///< Tet angle changed (by mouse or by toolbar)
	void perChanged(int);		///< Perspective changed (by mouse or by toolbar)
	void alphaChanged(bool);	///< Transparency changed (by toolbar)
	void lightChanged(bool);	///< Lighting changed (by toolbar)
	void zoomChanged(bool);		///< Zooming changed (by toolbar)
	void rotateChanged(bool);	///< Rotation changed (by toolbar)
	void usePrimChanged(bool);	///< Use primitive changed (i.e. have or not drawing function)
	void viewYZChanged(bool);	///< Rotation axis changed (by toolbar)
	void mouseClick(mreal,mreal,mreal);	///< Position of mouse click
	void frameChanged(int);		///< Need another frame to show
	void showWarn(QString);		///< Show warnings
	void posChanged(QString message);	///< user click to show mouse position
	void objChanged(int objId);	///< User double-click to select object/line
	void refreshData();
	void doubleClick(int id);	///< Double mouse click by object with id
	void askStyle(int id);		///< Update style
	/// user can define its own zooming function
	void customZoom(double x1, double y1, double x2, double y2, double tet, double phi, double per);
	/// user can define its own drawing/setting function which will be called before main drawing
	void customDraw(double x1, double y1, double x2, double y2, bool draw);

protected:
	void paintEvent(QPaintEvent *);
	void resizeEvent(QResizeEvent *);
	void mousePressEvent(QMouseEvent *);
	void mouseReleaseEvent(QMouseEvent *);
	void mouseMoveEvent(QMouseEvent *);
	void wheelEvent(QWheelEvent *);
	void mouseDoubleClickEvent(QMouseEvent *);

	mglCanvas *gr;		///< Built-in mglCanvasQT-er instance (used by default)
	void *draw_par;		///< Parameters for drawing function mglCanvasWnd::DrawFunc.
	/// Drawing function for window procedure. It should return the number of frames.
	int (*draw_func)(mglBase *gr, void *par);
	mglDraw *draw;		///< Class for drawing -- need to call directly due to inheritance mechanism
	QString mousePos;	///< Last mouse position
	QPixmap pic;			///< Pixmap for drawing (changed by update)
	double tet, phi;		///< Rotation angles
	double per;			///< Value of perspective ( must be in [0,1) )
	bool alpha;			///< Transparency state
	bool light;			///< Lightning state
	bool custZoom;		///< Use custom zoom instead of built in
	bool custDraw;		///< Use custom draw before main drawing
	bool zoom;			///< Mouse zoom state
	bool grid;			///< Grid drawing state
	bool rotate;			///< Mouse rotation state
	bool viewYZ;			///< Set mouse rotation around Y and Z axis (instead of X and Z)
	mreal x1,x2,y1,y2;	///< Zoom in region
	mreal ax1,ax2,ay1,ay2;	///< Axis range zoom
	bool showMessage;	///< Flag for showing messages (enabled by each execute())
	QMenu *popup;		///< Pointer to pop-up menu
	QTimer *timer;		///< Timer for animation
private:
	int x0, y0, xe, ye;		///< Temporary variables for mouse
	uchar *grBuf;
	void draw_thr();
};
//-----------------------------------------------------------------------------
/// Class for drawing the MGL script
class MGL_EXPORT mglDrawScript : public mglDraw
{
public:
	HMPR par;		///< Parser to be used
	QString text;	///< Script to be drawn
	long line;		///< Line which will be highlited
	mglDrawScript(HMPR p):mglDraw()	{	par=p;	line=-1;	}
	virtual ~mglDrawScript() {}
	int Draw(mglGraph *gr)
	{	gr->Highlight(line+1);	mgl_parse_textw(gr->Self(),par,text.toStdWString().c_str());	return 0;	}
};
//-----------------------------------------------------------------------------
/// Convert bitmap from mglCanvasWnd to QPixmap
void mglConvertFromGraph(QPixmap &pic, mglCanvas *gr, uchar **buf);
/// Make menu, toolbars and return popup menu for MainWindow
QMenu *mglMakeMenu(QMainWindow* Wnd, QMathGL* QMGL, QSpinBox*& tet, QSpinBox*& phi);
//-----------------------------------------------------------------------------
#endif
