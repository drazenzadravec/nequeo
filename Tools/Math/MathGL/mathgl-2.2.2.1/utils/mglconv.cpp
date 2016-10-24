/***************************************************************************
 * mglconv.cpp is part of Math Graphic Library
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
void mgl_error_print(const char *Message, void *par);
void mgl_ask_gets(const wchar_t *quest, wchar_t *res);
//-----------------------------------------------------------------------------
int main(int argc, char *argv[])
{
	mglGraph gr;
	mglParse p(true);
	char ch, buf[2048], iname[256]="", oname[256]="";
	std::vector<std::wstring> var;

	register size_t i, n;
	while(1)
	{
		ch = getopt(argc, argv, "1:2:3:4:5:6:7:8:9:ho:L:C:A:");
		if(ch>='1' && ch<='9')	p.AddParam(ch-'0', optarg);
		else if(ch=='L')	setlocale(LC_CTYPE, optarg);
		else if(ch=='A')
		{
			std::wstring str;
			for(i=0;optarg[i];i++)	str.push_back(optarg[i]);
			var.push_back(str);
		}
		else if(ch=='C')
		{
			double v1,v2,dv=1,v;
			int res=sscanf(optarg,"%lg:%lg:%lg",&v1,&v2,&dv);
			if(res<3)	dv=1;
			wchar_t num[64];
			for(v=v1;v<=v2;v+=dv)
			{
				mglprintf(num,64,L"%g",v);
				var.push_back(num);
			}
		}
		else if(ch=='h' || (ch==-1 && optind>=argc))
		{
			printf("mglconv convert mgl script to bitmap png file.\nCurrent version is 2.%g\n",MGL_VER2);
			printf("Usage:\tmglconv [parameter(s)] scriptfile\n");
			printf(
				"\t-1 str       set str as argument $1 for script\n"
				"\t...          ...\n"
				"\t-9 str       set str as argument $9 for script\n"
				"\t-L loc       set locale to loc\n"
				"\t-o name      set output file name\n"
				"\t-            get script from standard input\n"
				"\t-A val       add animation value val\n"
				"\t-C n1:n2:dn  add animation value in range [n1,n2] with step dn\n"
				"\t-C n1:n2     add animation value in range [n1,n2] with step 1\n"
				"\t-            get script from standard input\n"
				"\t-h           print this message\n" );
			ch = 'h';	break;
		}
		else if(ch=='o')	strncpy(oname, optarg,256);
		else if(ch==-1 && optind<argc)
		{	strncpy(iname, argv[optind][0]=='-'?"":argv[optind],256);	break;	}
	}
	if(ch=='h')	return 0;
	if(*oname==0)	{	strncpy(oname,*iname?iname:"out",250);	strcat(oname,".png");	}
	
	mgl_ask_func = mgl_ask_gets;
	// prepare for animation
	std::wstring str;
	setlocale(LC_CTYPE, "");
	FILE *fp = *iname?fopen(iname,"r"):stdin;
	wchar_t cw;
	while((cw=fgetwc(fp))!=WEOF)	str.push_back(cw);
//	while(!feof(fp))	str.push_back(fgetwc(fp));
	if(*iname)	fclose(fp);

	for(i=0;;)	// collect exact values
	{
		n = str.find(L"##a ",i);
		if(n==std::string::npos)	break;
		i = n+4;	var.push_back(str.substr(i,str.find('\n',i)));
	}
	n = str.find(L"##c ");
	if(n!=std::string::npos)
	{
		double v1,v2,dv,v;
		wscanf(str.c_str()+n+4,L"%lg%lg%lg",&v1,&v2,&dv);
		wchar_t ss[64];
		for(v=v1;v<=v2;v+=dv)
		{	mglprintf(ss,64,L"%g",v);	var.push_back(ss);	}
	}
	bool gif = !strcmp(oname+strlen(oname)-4,".gif");
	if(var.size()>1)	// there is animation
	{
		if(gif)	gr.StartGIF(oname);
		for(i=0;i<var.size();i++)
		{
			gr.NewFrame();
			printf("frame %ld for $0 = \"%ls\"\n",i,var[i].c_str());
			p.AddParam(0,var[i].c_str());
			p.Execute(&gr,str.c_str());
			if(gr.Message()[0])	printf("%s\n",gr.Message());
			gr.EndFrame();
			snprintf(buf,2048,"%s-%ld",oname,i);
			if(!gif)	gr.WriteFrame(buf);
		}
		if(gif)	gr.CloseGIF();
	}
	else
	{
		p.Execute(&gr,str.c_str());
		printf("%s\n",gr.Message());
		gr.WriteFrame(oname);
	}
	if(!mglGlobalMess.empty())	printf("%s",mglGlobalMess.c_str());
	printf("Write output to %s\n",oname);
	return 0;
}
//-----------------------------------------------------------------------------
