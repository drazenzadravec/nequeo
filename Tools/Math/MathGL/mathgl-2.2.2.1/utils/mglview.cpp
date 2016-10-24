/***************************************************************************
 * mglview.cpp is part of Math Graphic Library
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
#include <locale.h>
#include <unistd.h>

#include "mgl2/mgl.h"
#include "mgl2/qt.h"
//-----------------------------------------------------------------------------
std::wstring str;
mglParse p(true);
void mgl_error_print(const char *Message, void *par);
void mgl_ask_fltk(const wchar_t *quest, wchar_t *res);
void mgl_ask_qt(const wchar_t *quest, wchar_t *res);
void mgl_ask_gets(const wchar_t *quest, wchar_t *res);
//-----------------------------------------------------------------------------
int show(mglGraph *gr)
{
	p.Execute(gr,str.c_str());
	printf("%s\n",gr->Message());
	return 0;
}
//-----------------------------------------------------------------------------
int main(int argc, char **argv)
{
	char ch, iname[256]="";
	
	while(1)
	{
		ch = getopt(argc, argv, "1:2:3:4:5:6:7:8:9:ho:L:");
		if(ch>='1' && ch<='9')	p.AddParam(ch-'0', optarg);
		else if(ch=='L')	setlocale(LC_CTYPE, optarg);
		else if(ch=='h' || (ch==-1 && optind>=argc))
		{
			printf("mglconv convert mgl script to bitmap png file.\nCurrent version is 2.%g\n",MGL_VER2);
			printf("Usage:\tmglview [parameter(s)] scriptfile\n");
			printf(
				"\t-1 str       set str as argument $1 for script\n"
				"\t...          ...\n"
				"\t-9 str       set str as argument $9 for script\n"
				"\t-L loc       set locale to loc\n"
				"\t-            get script from standard input\n"
				"\t-h           print this message\n" );
			ch = 'h';	break;
		}
		else if(ch==-1 && optind<argc)
		{	strncpy(iname, argv[optind][0]=='-'?"":argv[optind],256);	break;	}
	}
	if(ch=='h')	return 0;

	bool mgld=(*iname && iname[strlen(iname)-1]=='d');
	if(!mgld)
	{
		setlocale(LC_CTYPE, "");
		FILE *fp = *iname?fopen(iname,"r"):stdin;
		if(fp)
		{
			wchar_t ch;
			while((ch=fgetwc(fp))!=WEOF)	str.push_back(ch);
			fclose(fp);
		}
	}

	mgl_ask_func = mgl_ask_gets;
	mgl_ask_func = mgl_ask_qt;
	mglQT gr(mgld?NULL:show, *iname?iname:"mglview");
	if(mgld)
	{
		gr.Setup(false);
		setlocale(LC_NUMERIC, "C");
		gr.ImportMGLD(iname);
		setlocale(LC_NUMERIC, "");
		gr.Update();
	}
	if(!mglGlobalMess.empty())	printf("%s",mglGlobalMess.c_str());
	gr.Run();
	return 0;
}
//-----------------------------------------------------------------------------
