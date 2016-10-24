/***************************************************************************
 * full_test.cpp is part of Math Graphic Library
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
#include <time.h>
#include <locale.h>
#include <time.h>
#if !defined(_MSC_VER) && !defined(__BORLANDC__)
#include <getopt.h>
#endif
#include <vector>
#include "mgl2/mgl.h"
#include "mgl2/eval.h"
//-----------------------------------------------------------------------------
void mgl_create_cpp_font(HMGL gr, const wchar_t *how);
//-----------------------------------------------------------------------------
struct mglSample	/// Structure for list of samples
{
	const char *name;
	void (*func)(mglGraph*);
	const char *mgl;
};
extern mglSample samp[];
extern const char *mmgl_dat_prepare;
//-----------------------------------------------------------------------------
int mgl_cmd_smp(const void *a, const void *b)
{
	const mglSample *aa = (const mglSample *)a;
	const mglSample *bb = (const mglSample *)b;
	return strcmp(aa->name, bb->name);
}
//-----------------------------------------------------------------------------
int type = 0;
int dotest  = 0;
int width  = 800;
int height = 600;
int big  = 0;
int srnd = 0;
int use_mgl = 0;
int verbose = 0;
int quality  = MGL_DRAW_NORM;
//-----------------------------------------------------------------------------
void mgls_prepare1d(mglData *y, mglData *y1=0, mglData *y2=0, mglData *x1=0, mglData *x2=0);
void mgls_prepare2d(mglData *a, mglData *b=0, mglData *v=0);
void mgls_prepare3d(mglData *a, mglData *b=0);
void mgls_prepare2v(mglData *a, mglData *b);
void mgls_prepare3v(mglData *ex, mglData *ey, mglData *ez);
//-----------------------------------------------------------------------------
void save(mglGraph *gr,const char *name,const char *suf);
void smgl_stfa(mglGraph *gr);	// STFA sample
void smgl_text(mglGraph *gr);	// text drawing
void smgl_surf(mglGraph *gr);
#include <mgl2/base.h>
#include <mgl2/font.h>
void test(mglGraph *gr)
{
	union {unsigned long b;double d;float f;} t;	t.b=0;
	t.d = NAN;	printf("NANd: %g --> %lx\t",t.d,t.b);	t.b=0;
	t.f = NAN;	printf("NANf: %g --> %lx\n",t.f,t.b);	t.b=0;
	
	t.d = INFINITY;	printf("INFd: %g --> %lx\t",t.d,t.b);	t.b=0;
	t.f = INFINITY;	printf("INFf: %g --> %lx\n",t.f,t.b);	t.b=0;

const unsigned long mgl_nan[2] = {0x7fffffffffffffff, 0x7fffffff};
#define NANd    (*(double*)mgl_nan)
#define NANf    (*(float*)(mgl_nan+1))
	t.d = NANd;	printf("NANd: %g --> %lx\t",t.d,t.b);	t.b=0;
	t.f = NANf;	printf("NANf: %g --> %lx\n",t.f,t.b);	t.b=0;

	
const unsigned long mgl_inf[2] = {0x7ff0000000000000, 0x7f800000};
#define INFd    (*(double*)mgl_inf)
#define INFf    (*(float*)(mgl_inf+1))
	t.d = INFd;	printf("INFd: %g --> %lx\t",t.d,t.b);	t.b=0;
	t.f = INFf;	printf("INFf: %g --> %lx\n",t.f,t.b);	t.b=0;

	return;

	mglData y(50);
	y.Modify("sin(10*x) + 10");
	gr->SetRange('y', y);
	gr->Box();
	gr->Axis();
	gr->Plot(y);
	y.Save("test.dat");
	return;


	mglParse par;
	setlocale(LC_CTYPE, "");
	par.Execute(gr,"new x 50 40 '0.8*sin(pi*x)*sin(pi*(y+1)/2)'\n\
					new y 50 40 '0.8*cos(pi*x)*sin(pi*(y+1)/2)'\n\
					new z 50 40 '0.8*cos(pi*(y+1)/2)'\nlight on\n\
					title 'parametric form':rotate 50 60:box\n\
					surf x y z ''\nwrite '1.tex'");
//	par.Execute(gr,"light on:addlegend 'r' 'r':legend");
}
//-----------------------------------------------------------------------------
#if !defined(_MSC_VER) && !defined(__BORLANDC__)
static struct option longopts[] =
{
	{ "mini",	no_argument,	&big,	3 },
	{ "big",	no_argument,	&big,		1 },
	{ "web",	no_argument,	&big,		2 },
	{ "bps",	no_argument,	&type,		8 },
	{ "help",	no_argument,	NULL,		'?' },
	{ "height",	required_argument,	NULL,	'h' },
	{ "png",	no_argument,	&type,		0 },
	{ "eps",	no_argument,	&type,		1 },
	{ "gif",	no_argument,	&type,		6 },
	{ "jpeg",	no_argument,	&type,		4 },
	{ "kind",	required_argument,	NULL,	'k' },
	{ "list",	no_argument,	NULL,		'l' },
	{ "mgl",	no_argument,	&use_mgl,	1 },
	{ "none",	no_argument,	&type,		7 },
	{ "obj",	no_argument,	&type,		11 },
	{ "obj_old",no_argument,	&type,		10 },
	{ "off",	no_argument,	&type,		12 },
	{ "prc",	no_argument,	&type,		5 },
	{ "pdf",	no_argument,	&type,		9 },
	{ "solid",	no_argument,	&type,		3 },
	{ "srnd",	no_argument,	&srnd,		1 },
	{ "svg",	no_argument,	&type,		2 },
	{ "stl",	no_argument,	&type,		13 },
	{ "tex",	no_argument,	&type,		14 },
	{ "json",	no_argument,	&type,		15 },
	{ "jsonz",	no_argument,	&type,		16 },
	{ "test",	no_argument,	&dotest,	1 },
	{ "font",	no_argument,	&dotest,	2 },
	{ "time",	no_argument,	&dotest,	3 },
	{ "thread",	required_argument,	NULL,	't' },
	{ "verbose",no_argument,	&verbose,	1 },
	{ "width",	required_argument,	NULL,	'w' },
	{ "quality",required_argument,	NULL,	'q' },
	{ NULL,		0,				NULL,		0 }
};
//-----------------------------------------------------------------------------
void usage()
{
	puts (
//		"--png		- output png\n"
		"--width=num	- png picture width\n"
		"--height=num	- png picture height\n"
		"--mini		- png picture is 200x150\n"
		"--big		- png picture is 1920x1440\n"
		"--web		- png picture is 640x480\n"
		"--prc		- output prc\n"
		"--pdf		- output pdf\n"
		"--eps		- output EPS\n"
		"--tex		- output LaTeX\n"
		"--jpeg		- output JPEG\n"
		"--json		- output JSON\n"
		"--jsonz		- output JSONz\n"
		"--solid	- output solid PNG\n"
		"--svg		- output SVG\n"
		"--obj		- output obj/mtl\n"
		"--obj_old	- output obj/mtl in old way\n"
		"--off		- output off\n"
		"--stl		- output stl\n"
		"--none		- none output\n"
		"--srnd		- use the same random numbers in any run\n"
		"--list		- print list of sample names\n"
		"--kind=name	- produce only this sample\n"
		"--thread=num	- number of threads used\n"
		"--mgl		- use MGL scripts for samples\n"
		"--test		- run in test mode\n"
		"--time		- measure execution time for all samples\n"
		"--font		- write current font as C++ file\n"
		"--quality=val	- use specified quality for plot(s)\n"
	);
}
#endif
//-----------------------------------------------------------------------------
void save(mglGraph *gr,const char *name,const char *suf="")
{
	//	return;
	char buf[128];
	switch(type)
	{
		case 1:	// EPS
			snprintf(buf,128,"%s%s.eps",name,suf);
			gr->WriteEPS(buf);
			break;
		case 2:	// SVG
			snprintf(buf,128,"%s%s.svg",name,suf);
			gr->WriteSVG(buf);	break;
		case 3:	// PNG
			snprintf(buf,128,"%s%s.png",name,suf);
			gr->WritePNG(buf,0,true);	break;
		case 4:	// JPEG
			snprintf(buf,128,"%s%s.jpg",name,suf);
			gr->WriteJPEG(buf);	break;
		case 5:	// PRC
			snprintf(buf,128,"%s%s.prc",name,suf);
			gr->WritePRC(buf,"",false);	break;
		case 6:	// GIF
			snprintf(buf,128,"%s%s.gif",name,suf);
			gr->WriteGIF(buf);	break;
		case 7:	gr->Finish();	// none
			break;
		case 8:	// EPS to PNG
			snprintf(buf,128,"%s%s.png",name,suf);
			gr->WritePNG(buf,0,false);
			break;
 		case 9:	// PDF
			snprintf(buf,128,"%s%s.prc",name,suf);
			gr->WritePRC(buf);	remove(buf);	break;
		case 10:	// old OBJ
			snprintf(buf,128,"%s%s.obj",name,suf);
			gr->WriteOBJold(buf);	break;
		case 11:	// OBJ
			snprintf(buf,128,"%s%s.obj",name,suf);
			gr->WriteOBJ(buf);	break;
		case 12:	// OFF
			snprintf(buf,128,"%s%s.off",name,suf);
			gr->WriteOFF(buf);	break;
		case 13:	// STL
			snprintf(buf,128,"%s%s.stl",name,suf);
			gr->WriteSTL(buf);	break;
		case 14:	// TeX
			snprintf(buf,128,"%s%s.tex",name,suf);
			gr->WriteTEX(buf);	break;
		case 15:	// JSON
			snprintf(buf,128,"%s%s.json",name,suf);
			gr->WriteJSON(buf);	break;
		case 16:	// JSON
			snprintf(buf,128,"%s%s.jsonz",name,suf);
			gr->WriteJSON(buf,"",true);	break;
		default:// PNG (no alpha)
#if MGL_HAVE_PNG
			snprintf(buf,128,"%s%s.png",name,suf);
			gr->WritePNG(buf,0,false);	break;
#else
			snprintf(buf,128,"%s%s.bmp",name,suf);
			gr->WriteBMP(buf);	break;
#endif
	}
}
//-----------------------------------------------------------------------------
int main(int argc,char **argv)
{
	const char *suf = "";
	char name[256]="", *tmp;
	int ch;
	time_t st,en;	time(&st);
	mglGraph *gr = NULL;
	mglSample *s=samp;
#if !defined(_MSC_VER) && !defined(__BORLANDC__)
	while(( ch = getopt_long_only(argc, argv, "", longopts, NULL)) != -1)
		switch(ch)
		{
			case 0:		break;
			case 'w':	width =atoi(optarg);	break;
			case 'h':	height=atoi(optarg);	break;
			case 'q':	quality =atoi(optarg);	break;
			case 'k':	strncpy(name, optarg,256);
						tmp=strchr(name,'.');	if(tmp)	*tmp=0;
						tmp=strchr(name,'-');	if(tmp)	*tmp=0;
						break;
			case 't':	mgl_set_num_thr(atoi(optarg));	break;
			case 'l':
				while(s->name[0])	{	printf("%s ",s->name);	s++;	}
				printf("\n");	return 0;
			case '?':
			default:	usage();	return 0;
		}
#endif

	if(dotest==1)	printf("Global (before):%s\n",mglGlobalMess.c_str());
	gr = new mglGraph;
	if(	type==11|| type==12|| type==5 || type==9)	width=height;
	switch(big)
	{
	case 1:	gr->SetSize(1920,1440);	suf = "-lg";	break;
	case 2:	gr->SetSize(640,480);	break;
	case 3:	gr->SetSize(192,144);	suf = "-sm";	break;
	default:	gr->SetSize(width,height);
	}
	gr->SetQuality(quality);

	if(dotest==1)
	{
		mgl_set_test_mode(true);	test(gr);
		time(&en);	printf("time is %g sec\n",difftime(en,st));
		gr->WritePNG("test.png","",false);
		gr->WriteEPS("test.eps");
		printf("Messages:%s\n",gr->Message());
		printf("Global:%s\n",mglGlobalMess.c_str());
		delete gr;	return 0;
	}
	else if(dotest==2)
	{	mgl_create_cpp_font(gr->Self(), L"!-~,¡-ÿ,̀-̏,Α-ω,ϑ,ϕ,ϖ,ϰ,ϱ,ϵ,А-я,ℏ,ℑ,ℓ,ℜ,←-↙,∀-∯,≠-≯,⟂");
		delete gr;	return 0;	}
	else if(dotest==3)
	{
		int qual[7]={0,1,2,4,5,6,8};
		size_t ll=strlen(mmgl_dat_prepare)+1;
		mglParse par;
		par.AllowSetSize(true);	setlocale(LC_CTYPE, "");
		FILE *fp = fopen(big?"time_big.texi":"time.texi","w");
		fprintf(fp,"@multitable @columnfractions .16 .12 .12 .12 .12 .12 .12 .12\n");
		fprintf(fp,"@headitem Name");
		for(int i=0;i<7;i++)	fprintf(fp," @tab q=%d",qual[i]);
		clock_t beg,end;
		while(s->name[0])	// all samples
		{
			char *buf = new char[strlen(s->mgl)+ll];
			strcpy(buf,s->mgl);	strcat(buf,mmgl_dat_prepare);
			fprintf(fp,"\n@item %s",s->name);

			printf("%s",s->name);
			for(int i=0;i<7;i++)
			{
				gr->DefaultPlotParam();
				gr->SetQuality(qual[i]);	gr->Clf();
				beg = clock();
				if(!use_mgl)	s->func(gr);
				else 	par.Execute(gr,buf);
				gr->Finish();
				end = clock();
				fprintf(fp," @tab %.3g",double(end-beg)/CLOCKS_PER_SEC);
				printf("\t%d->%g",qual[i],double(end-beg)/CLOCKS_PER_SEC);
				fflush(fp);	fflush(stdout);
			}
			printf("\n");	delete []buf;	s++;
		}
		fprintf(fp,"\n@end multitable\n");	fclose(fp);
	}

	if(type==15 || type==16)	big=3;	// save mini version for json

	if(srnd)	mgl_srnd(1);
	gr->VertexColor(false);	gr->Compression(false);
	if(name[0]==0)
	{
		while(s->name[0])	// all samples
		{
			gr->DefaultPlotParam();	gr->Clf();
			if(use_mgl)
			{
				mglParse par;
				par.AllowSetSize(true);
				setlocale(LC_CTYPE, "");
				char *buf = new char[strlen(s->mgl)+strlen(mmgl_dat_prepare)+1];
				strcpy(buf,s->mgl);		strcat(buf,mmgl_dat_prepare);
				printf("\n-------\n%s\n-------\n",verbose?buf:s->mgl);
				par.Execute(gr,buf);	delete []buf;
				const char *mess = gr->Message();
				if(*mess)	printf("Warnings: %s\n-------\n",mess);
			}
			else	s->func(gr);
			save(gr, s->name, suf);
			printf("%s ",s->name);	fflush(stdout);	s++;
		}
		printf("\n");
	}
	else	// manual sample
	{
		mglSample tst;	tst.name=name;
		int i=0;
		for(i=0;samp[i].name[0];i++);	// determine the number of samples
		s = (mglSample *) bsearch(&tst, samp, i, sizeof(mglSample), mgl_cmd_smp);
		if(s)
		{
			gr->DefaultPlotParam();	gr->Clf();
			if(use_mgl)
			{
				mglParse par;
				par.AllowSetSize(true);
				setlocale(LC_CTYPE, "");
				char *buf = new char[strlen(s->mgl)+strlen(mmgl_dat_prepare)+1];
				strcpy(buf,s->mgl);		strcat(buf,mmgl_dat_prepare);
				printf("\n-------\n%s\n-------\n",verbose?buf:s->mgl);
				par.Execute(gr,buf);	delete []buf;
				const char *mess = gr->Message();
				if(*mess)	printf("Warnings: %s\n-------\n",mess);
			}
			else	s->func(gr);
			save(gr, s->name, suf);
		}
		else	printf("no sample %s\n",name);
	}
	delete gr;	return 0;
}
//-----------------------------------------------------------------------------
