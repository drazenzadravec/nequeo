/***************************************************************************
 * fltk_example.cpp is part of Math Graphic Library
 * Copyright (C) 2007-2014 Alexey Balakin <mathgl.abalakin@gmail.ru>       *
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
#include "mgl2/fltk.h"
//-----------------------------------------------------------------------------
#if defined(WIN32) || defined(_MSC_VER) || defined(__BORLANDC__)
#include <windows.h>
#else
#include <unistd.h>
#endif
void long_calculations()	// just delay which correspond to simulate calculations
{
#if defined(WIN32) || defined(_MSC_VER) || defined(__BORLANDC__)
	Sleep(1000);
#else
	sleep(1);	// which can be very long
#endif
}
//-----------------------------------------------------------------------------
#if defined(PTHREAD_SAMPLE1)	// first variant of multi-threading usage of mglFLTK window
mglFLTK *gr=NULL;
void *calc(void *)
{
	mglPoint pnt;
	for(int i=0;i<10;i++)		// do calculation
	{
		long_calculations();	// which can be very long
		pnt = mglPoint(2*mgl_rnd()-1,2*mgl_rnd()-1);
		if(gr)
		{
			gr->Clf();			// make new drawing
			gr->Line(mglPoint(),pnt,"Ar2");
			char str[10] = "i=0";	str[2] = '0'+i;
			gr->Puts(mglPoint(),str);
			gr->Update();		// update window
		}
	}
	exit(0);
}
int main(int argc,char **argv)
{
	static pthread_t thr;
	pthread_create(&thr,0,calc,0);
	pthread_detach(thr);
	gr = new mglFLTK;
	gr->Run();	return 0;
}
#elif defined(PTHREAD_SAMPLE2)	// another variant of multi-threading usage of mglFLTK window. Work only if pthread was enabled for MathGL
mglPoint pnt;	// some global variable for changeable data
int main(int argc,char **argv)
{
	mglFLTK gr("test");
	gr.RunThr();	// <-- need MathGL version which use pthread
	for(int i=0;i<10;i++)	// do calculation
	{
		long_calculations();// which can be very long
		pnt = mglPoint(2*mgl_rnd()-1,2*mgl_rnd()-1);
		gr.Clf();			// make new drawing
		gr.Line(mglPoint(),pnt,"Ar2");
		char str[10] = "i=0";	str[3] = '0'+i;
		gr.Update();		// update window
	}
	return 0;	// finish calculations and close the window
}
#else		// just default samples
int test_wnd(mglGraph *gr);
int sample(mglGraph *gr);
int sample_1(mglGraph *gr);
int sample_2(mglGraph *gr);
int sample_3(mglGraph *gr);
int sample_d(mglGraph *gr);
//-----------------------------------------------------------------------------
int main(int argc,char **argv)
{
	mglFLTK *gr;
	char key = 0;
	if(argc>1)	key = argv[1][0]!='-' ? argv[1][0]:argv[1][1];
	else	printf("You may specify argument '1', '2', '3' or 'd' for viewing examples of 1d, 2d, 3d or dual plotting\n");
	switch(key)
	{
		case '1':	gr = new mglFLTK(sample_1,"1D plots");	break;
		case '2':	gr = new mglFLTK(sample_2,"2D plots");	break;
		case '3':	gr = new mglFLTK(sample_3,"3D plots");	break;
		case 'd':	gr = new mglFLTK(sample_d,"Dual plots");	break;
		case 't':	gr = new mglFLTK(test_wnd,"Testing");	break;
		default:	gr = new mglFLTK(sample,"Drop and waves");	break;
	}
	gr->Run();	return 0;
}
#endif
