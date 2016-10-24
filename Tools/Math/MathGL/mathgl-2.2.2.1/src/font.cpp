/***************************************************************************
 * font.cpp is part of Math Graphic Library
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
#include <locale.h>
#include <ctype.h>
#include <wctype.h>

// #ifdef WIN32
// #include <windows.h>
// #else
// #include <wchar.h>
// #endif

#include "mgl2/base.h"
#include "mgl2/font.h"
//-----------------------------------------------------------------------------
extern unsigned mgl_numg, mgl_cur;
extern float mgl_fact;
extern long mgl_gen_fnt[516][6];
extern short mgl_buf_fnt[246080];
extern mglTeXsymb mgl_tex_symb[];
//mglFont mglDefFont("nofont");
mglFont mglDefFont;
//-----------------------------------------------------------------------------
char mglGetStyle(const char *how, int *font, int *align)
{
	char col=0;
	if(align)	*align = 1;	// centering text by default
	if(!how || *how==0)	return col;
	// NOTE: no brightness for text color
	for(;*how && *how!=':';how++)
		if(strchr(MGL_COLORS,*how))	col=*how;
	if(align)
	{
		*align = 1;
		if(strchr(how,'R'))	*align = 2;
		if(strchr(how,'C'))	*align = 1;
		if(strchr(how,'L'))	*align = 0;
		if(strchr(how,'D'))	*align+= 4;
	}
	if(font)
	{
		*font = 0;
		if(strchr(how,'b'))	*font = *font|MGL_FONT_BOLD;
		if(strchr(how,'i'))	*font = *font|MGL_FONT_ITAL;
		if(strchr(how,'w'))	*font = *font|MGL_FONT_WIRE;
		if(strchr(how,'o'))	*font = *font|MGL_FONT_OLINE;
		if(strchr(how,'u'))	*font = *font|MGL_FONT_ULINE;
	}
	return col;
}
//-----------------------------------------------------------------------------
float mglFont::Puts(const char *str,const char *how,float col) const
{
	int font=0, align=1;	float w=0;
	char cc=mglGetStyle(how,&font,&align);
	MGL_TO_WCS(str,w = Puts(wcs,font,align,cc?-cc:col));
	return w;
}
//-----------------------------------------------------------------------------
float mglFont::Width(const char *str,const char *how) const
{
	int font=0;	float w=0;
	mglGetStyle(how,&font);
	MGL_TO_WCS(str,w = Width(wcs,font));
	return w;
}
//-----------------------------------------------------------------------------
float mglFont::Puts(const wchar_t *str,const char *how,float col) const
{
	int font=0, align=1;
	char cc=mglGetStyle(how,&font,&align);
	return Puts(str, font, align,cc?-cc:col);
}
//-----------------------------------------------------------------------------
float mglFont::Width(const wchar_t *str,const char *how) const
{
	int font=0;
	mglGetStyle(how,&font);
	return Width(str, font);
}
//-----------------------------------------------------------------------------
float mglFont::Puts(const wchar_t *str,int font,int align, float col) const
{
	if(numg==0 || !str || *str==0)	return 0;
	float ww=0,w=0,h = (align&4) ? 500./fact[0] : 0;
	size_t size = mgl_wcslen(str)+1,i,num=0;
	if(parse)
	{
		unsigned *wcs = new unsigned[size], *buf=wcs;
		memcpy(wcs,str,size*sizeof(wchar_t));
		Convert(str, wcs);
		for(i=0;wcs[i];i++)
		{
			if(wcs[i]=='\n')	// parse '\n' symbol
			{
				wcs[i]=0;	w = Puts(buf,0,0,1.f,0x10|font,col);	// find width
				Puts(buf,-w*(align&3)/2.f,-h - 500.*num/fact[0],1.f,font,col);	// draw it really
				buf=wcs+i+1;	num++;	if(w>ww)	ww=w;
			}
			if(wcs[i]=='\\' && wcs[i+1]=='n' && (wcs[i+2]>' ' || wcschr(L"{}[]()!@#$%^&*/-?.,_=+\\\"", wcs[i+2])))	// parse '\n' symbol
			{
				wcs[i]=0;	w = Puts(buf,0,0,1.f,0x10|font,col);	// find width
				Puts(buf,-w*(align&3)/2.f,-h - 500.*num/fact[0],1.f,font,col);	// draw it really
				buf=wcs+i+2;	num++;	if(w>ww)	ww=w;
			}
		}
		// draw string itself
		w = Puts(buf,0,0,1.f,0x10|font,col);	// find width
		Puts(buf,-w*(align&3)/2.f,-h - 500.*num/fact[0],1.f,font,col);	// draw it really
		if(w>ww)	ww=w;
		delete []wcs;
	}
	else
	{
		int s = (font/MGL_FONT_BOLD)&3;
		long j;
		h *= fact[0]/fact[s];
		for(i=0;i<size;i++)		// find width
		{
			j = str[i]!=' ' ? Internal(str[i]) : Internal('!');
			if(j==-1)	continue;
			w+= width[s][j]/fact[s];
		}
		ww = w;		w *= -(align&3)/2.f;
		if(gr)	for(i=0;i<size;i++)		// draw it
		{
			if(str[i]!=' ')
			{
				j = Internal(str[i]);
				if(j==-1)	continue;
				gr->Glyph(w, -h, 1, s+(font&MGL_FONT_WIRE)?4:0, j, col);
			}
			else	j = 0;//Internal('!');
			w+= width[s][j]/fact[s];
		}
	}
	return ww;
}
//-----------------------------------------------------------------------------
float mglFont::Width(const wchar_t *str,int font) const
{
	if(numg==0 || !str || *str==0)	return 0;
	float ww=0,w=0;
	size_t size = mgl_wcslen(str)+1,i;
	if(parse)
	{
		unsigned *wcs = new unsigned[size], *buf=wcs;
		memcpy(wcs,str,size*sizeof(wchar_t));
		Convert(str, wcs);
		for(i=0;wcs[i];i++)	if(wcs[i]=='\n')	// parse '\n' symbol
		{
			wcs[i]=0;	w = Puts(buf,0,0,1.,0x10|font,'k');	// find width
			buf=wcs+i+1;	if(w>ww)	ww=w;
		}
		w = Puts(buf,0,0,1.,0x10|font,'k');
		if(w<ww)	w=ww;
		delete []wcs;
	}
	else
	{
		long j;
		int s = (font/MGL_FONT_BOLD)&3;
		for(i=0;i<size;i++)
		{
			j = str[i]!=' ' ? Internal(str[i]) : Internal('!');
			if(j==-1)	continue;
			w+= width[s][j]/fact[s];
		}
	}
	return w;
}
//-----------------------------------------------------------------------------
float mglFont::Height(int font) const
{
	if(numg==0)	return 0;
	int s = (font/MGL_FONT_BOLD)&3;
	return (500.f)/fact[s];
}
//-----------------------------------------------------------------------------
float mglFont::Height(const char *how) const
{
	if(numg==0)	return 0;
	int s=0;
	if(how)
	{
		if(strchr(how,'b'))	s = s|1;
		if(strchr(how,'i'))	s = s|2;
	}
	return (500.f)/fact[s];
}
//-----------------------------------------------------------------------------
long mglFont::Internal(unsigned s) const
{
	register long i,i1=0,i2=numg-1;
	register wchar_t j = wchar_t(s & MGL_FONT_MASK);
	// let suppose that id[i]<id[i+1]
	while(i1<i2)
	{
		i = (i1+i2)/2;
		if(j<id[i])			i2 = i;
		else if(j>id[i])	i1=i+1;	// i is bad
		else return i;
	}
	return j==id[i2] ? i2 : -1;
}
//-----------------------------------------------------------------------------
/// Table of acents and its UTF8 codes
MGL_NO_EXPORT mglTeXsymb mgl_act_symb[] = {
	{0x02c6, L"hat"}, {0x02dc, L"tilde"}, {0x02d9, L"dot"}, {0x00a8, L"ddot"}, {0x20db, L"dddot"}, {0x20dc, L"ddddot"}, {0x02ca, L"acute"}, {0x02c7, L"check"}, {0x02cb, L"grave"}, {0x20d7, L"vec"}, {0x02c9, L"bar"}, {0x02d8, L"breve"},
	/*end*/{0, L"\0"}};
//-----------------------------------------------------------------------------
int MGL_NO_EXPORT mgl_tex_symb_cmp(const void *a, const void *b)
{
	const mglTeXsymb *aa = (const mglTeXsymb *)a;
	const mglTeXsymb *bb = (const mglTeXsymb *)b;
	return wcscmp(aa->tex, bb->tex);
}
//-----------------------------------------------------------------------------
// parse LaTeX commands (mostly symbols and acents, and some font-style commands)
unsigned mglFont::Parse(const wchar_t *s) const
{
	register long k;
	unsigned res = unsigned(-2);		// Default is no symbol
	if(!s || !s[0])	return res;
	for(k=0;mgl_tex_symb[k].kod;k++);	// determine the number of symbols
	mglTeXsymb tst, *rts;
	tst.tex = s;
	rts = (mglTeXsymb *) bsearch(&tst, mgl_tex_symb, k, sizeof(mglTeXsymb), mgl_tex_symb_cmp);
	if(rts)	return rts->kod;

//	for(k=0;mgl_tex_symb[k].kod;k++)	// special symbols
//		if(!wcscmp(s,mgl_tex_symb[k].tex))
//			return mgl_tex_symb[k].kod;
	for(k=0;mgl_act_symb[k].kod;k++)	// acents
		if(!wcscmp(s,mgl_act_symb[k].tex))
			return mgl_act_symb[k].kod | MGL_FONT_ZEROW;
	// arbitrary UTF symbol
	if(s[0]=='u' && s[1]=='t' && s[2]=='f')
//	{	wscanf(s+3,"%lx",&k);	return wchar_t(k);	}
	{	k = wcstoul(s+3,NULL,16);	return wchar_t(k);	}
	// font/style changes for next symbol
	if(!wcscmp(s,L"big"))			res = unsigned(-5);
	else if(!wcscmp(s,L"frac"))		res = unsigned(-6);
	else if(!wcscmp(s,L"stack"))	res = unsigned(-7);
	else if(!wcscmp(s,L"overset"))	res = unsigned(-8);
	else if(!wcscmp(s,L"underset"))	res = unsigned(-9);
	else if(!wcscmp(s,L"stackr"))	res = unsigned(-10);
	else if(!wcscmp(s,L"stackl"))	res = unsigned(-11);
	else if(!wcscmp(s,L"sub"))		res = unsigned(-12);
	else if(!wcscmp(s,L"sup"))		res = unsigned(-13);
	else if(!wcscmp(s,L"textsc"))	res = unsigned(-14);	// new
	else if(!wcscmp(s,L"b"))		res = MGL_FONT_BOLD;
	else if(!wcscmp(s,L"i"))		res = MGL_FONT_ITAL;
	else if(!wcscmp(s,L"bi"))		res = MGL_FONT_BOLD|MGL_FONT_ITAL;
	else if(!wcscmp(s,L"r"))		res = unsigned(-1);
	else if(!wcscmp(s,L"a"))		res = MGL_FONT_OLINE;
	else if(!wcscmp(s,L"u"))		res = MGL_FONT_ULINE;
	else if(!wcscmp(s,L"n"))		res = '\n';
	else if(!wcscmp(s,L"overline"))	res = MGL_FONT_OLINE;
	else if(!wcscmp(s,L"underline"))res = MGL_FONT_ULINE;
	else if(!wcscmp(s,L"textbf"))	res = MGL_FONT_BOLD;
	else if(!wcscmp(s,L"textit"))	res = MGL_FONT_ITAL;
	else if(!wcscmp(s,L"textrm"))	res = unsigned(-1);
	else if(!wcscmp(s,L"w"))		res = MGL_FONT_WIRE;
	else if(!wcscmp(s,L"wire"))		res = MGL_FONT_WIRE;
	else if(!wcsncmp(s,L"color",5))	res = MGL_COLOR_MASK + (0xff & s[5]);
	return res;
}
//-----------------------------------------------------------------------------
void mglFont::Convert(const wchar_t *str, unsigned *res) const
{
	register size_t r,i,j,k,i0;
	wchar_t s[128]=L"", ch;		// TeX command and current char
	for(i=j=0;str[i];i++)
	{
		ch = str[i];
		if(ch=='\\')	// It can be TeX command
		{
			if(wcschr(L"{}_^\\@# ",str[i+1]))	// No, it is usual symbol
				res[j++] = str[++i];
			else		// Yes, it is TeX command
			{
				i0=i+1;
				for(k=0;isalnum(str[++i]) && k<127;k++)	s[k] = str[i];
				s[k] = 0;
				r = Parse(s);
				if(r==unsigned(-2))			// command not found, so use next symbol itself
				{	res[j++] = str[i0];	i = i0;	}
				else if(r)
				{
					res[j++] = r;
					if(str[i]>' ')	i--;
					if(str[i]==0)	break;
				}
			}
		}
		else if(ch<=' ' && ch!='\n')	res[j++] = ' ';	// no \t at this moment :(
		else if(ch=='_')	res[j++] = MGL_FONT_LOWER;
		else if(ch=='^')	res[j++] = MGL_FONT_UPPER;
		else if(ch=='@')	res[j++] = MGL_FONT_UPPER|MGL_FONT_LOWER;
		else if(ch=='{')	res[j++] = unsigned(-3);
		else if(ch=='}')	res[j++] = unsigned(-4);
		else if(ch=='#' && str[i+1]>' ')
			res[j++] = MGL_COLOR_MASK + (0xff & str[++i]);
		else	res[j++] = ch;				// It is just symbol
	}
	res[j] = 0;
}
//-----------------------------------------------------------------------------
float mglFont::get_ptr(long &i,unsigned *str, unsigned **b1, unsigned **b2,float &w1,float &w2, float f1, float f2, int st) const
{
	static unsigned s1[2]={0,0}, s2[2]={0,0};
	register long k;
	i++;
	if(str[i]==unsigned(-3))
	{
		i++;	*b1 = str+i;
		for(k=1;k>0 && str[i];i++)
		{
			if(str[i]==unsigned(-4))	k--;
			if(str[i]==unsigned(-3))	k++;
		}
		str[i-1]=0;
	}
	else	{	s1[0] = str[i];	*b1 = s1;	i++;	}
	if(str[i]==unsigned(-3))
	{
		i++;	*b2 = str+i;
		for(k=1;k>0 && str[i];i++)
		{
			if(str[i]==unsigned(-4))	k--;
			if(str[i]==unsigned(-3))	k++;
		}
		str[i-1]=0;
	}
	else	{	s2[0] = str[i];	*b2 = s2;	i++;	}
	i--;
	w1 = Puts(*b1, 0, 0, f1, 0x10|st,'k');
	w2 = Puts(*b2, 0, 0, f2, 0x10|st,'k');
	return w1>w2 ? w1 : w2;
}
//-----------------------------------------------------------------------------
void mglFont::draw_ouline(int st, float x, float y, float f, float g, float ww, float ccol) const
{
	if(st&MGL_FONT_OLINE)
		gr->Glyph(x,y+499*f/g, ww*g, (st&MGL_FONT_WIRE)?12:8, 0, ccol);
	if(st&MGL_FONT_ULINE)
		gr->Glyph(x,y-200*f/g, ww*g, (st&MGL_FONT_WIRE)?12:8, 0, ccol);
}
//-----------------------------------------------------------------------------
#define MGL_CLEAR_STYLE {st = style;	yy = y;	ff = f;	ccol=col;	a = (st/MGL_FONT_BOLD)&3;}
float mglFont::Puts(const unsigned *text, float x,float y,float f,int style,float col) const
{
	if(numg==0)	return 0;
	register long j,k;
	long i;
	register unsigned s,ss;
	float w=0;				// string width
	int st = style;			// current style
	unsigned *b1, *b2;		// pointer to substring
	unsigned *str;			// string itself
	float yy=y, ff=f, ww, w1, w2;
	float ccol=col;
	int a = (st/MGL_FONT_BOLD)&3;
	for(i=0;text[i];i++);
	str = new unsigned[i+1];
	memcpy(str,text,(i+1)*sizeof(unsigned));

	for(i=0;str[i];i++)
	{
		s = str[i];		ww = 0;
		if(s==unsigned(-3))	// recursion call here
		{
			i++;	b1 = str+i;
			for(k=1;k>0 && str[i];i++)
			{
				if(str[i]==unsigned(-4))	k--;
				if(str[i]==unsigned(-3))	k++;
			}
			str[i-1]=0;	i--;
			ww = Puts(b1, x, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s=='\n')	// newline
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff, ff, st);
			Puts(b1, x+(ww-w1)/2, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy-600*ff/fact[a], ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-9))	// underset
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff, ff/4, st);
			Puts(b1, x+(ww-w1)/2, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy-150*ff/fact[a], ff/4, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-8))	// overset
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff, ff/4, st);
			Puts(b1, x+(ww-w1)/2, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy+375*ff/fact[a], ff/4, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-12))	// sub
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff, ff/4, st);
			Puts(b1, x+(ww-w1)/2, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy-250*ff/fact[a], ff/4, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-13))	// sup
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff, ff/4, st);
			Puts(b1, x+(ww-w1)/2, yy, ff, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy+450*ff/fact[a], ff/4, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-11))	// stackl
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff*0.45, ff*0.45, st);
			Puts(b1, x, yy+250*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x, yy-110*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-10))	// stacr
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff*0.45, ff*0.45, st);
			Puts(b1, x+(ww-w1), yy+250*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2), yy-110*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-7))	// stack
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff*0.45, ff*0.45, st);
			Puts(b1, x+(ww-w1)/2, yy+250*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy-110*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-6))	// frac
		{
			ww = get_ptr(i, str, &b1, &b2, w1, w2, ff*0.45, ff*0.45, st);
			Puts(b1, x+(ww-w1)/2, yy+250*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			Puts(b2, x+(ww-w2)/2, yy-110*ff/fact[a], ff*0.45, (st&(~MGL_FONT_OLINE)&(~MGL_FONT_ULINE)), ccol);
			if(gr && !(style&0x10))	// add under-/over- line now
			{
				draw_ouline(st,x,y,f,fact[a],ww,ccol);
				gr->Glyph(x,y+150*f/fact[a], ww*fact[a], (st&MGL_FONT_WIRE)?12:8, 0, ccol);
			}
			MGL_CLEAR_STYLE
		}
		else if(s==unsigned(-4))	MGL_CLEAR_STYLE	// should be never here but if I miss sth ...
		else if(s==unsigned(-14))	// script symbols
		{
			k=1;
			if(str[i+1]==unsigned(-3))	for(j=i+2;k>0 && str[j];j++)
			{
				if(str[j]==unsigned(-3))	k++;
				if(str[j]==unsigned(-4))	k--;
				if(iswlower(str[j]))
					str[j] = MGL_FONT_UPPER|MGL_FONT_LOWER|towupper(str[j]);
			}
		}
		else if(s==unsigned(-5))	// large symbol
			ff *= 1.5;
		else if(s==unsigned(-1))	// set normal font
			st = style & MGL_FONT_ROMAN;
		else if((s&MGL_COLOR_MASK)==MGL_COLOR_MASK)	// color specification
			ccol = -float(s & 0xff);
		else
		{
			ss = s&MGL_FONT_MASK;
			if(ss)	// draw symbol (glyph)
			{
				if(ss>' ')
				{
					j = Internal(ss);
					if(j==-1)	continue;
					if(s & MGL_FONT_ZEROW)	yy += 100*ff/fact[a];
					if(gr && !(style&0x10))
					{
						if(st & MGL_FONT_WIRE)	gr->Glyph(x,yy,ff,a+4,j,ccol);
						else					gr->Glyph(x,yy,ff,a,j,ccol);
					}
				}
				else	j = Internal('!');
				ww = ff*width[a][j]/fact[a];
				if(gr && !(style&0x10))	// add under-/over- line now
					draw_ouline(st,x,y,f,fact[a],ww,ccol);
				if(s & MGL_FONT_ZEROW)	ww = 0;
				MGL_CLEAR_STYLE
			}
			// apply new styles
			if(s/MGL_FONT_BOLD)	st = st | (s & MGL_FONT_STYLE);
			a = (st/MGL_FONT_BOLD)&3;
			ss = (s/MGL_FONT_UPPER)%4;
			if(ss)
			{
				if(ss==1)		{	ff *=0.6;	yy += 200*ff/fact[a];	}	// =  500*0.4
				else if(ss==2)	{	ff *=0.6;	yy -=  80*ff/fact[a];	}	// = -500*0.16
				else if(ss==3)	{	ff *=0.8;	yy +=  0*60*ff/fact[a];	}	// =  500*0.12
			}
		}
		x += ww;	w += ww;
	}
	delete []str;
	return w;
}
//-----------------------------------------------------------------------------
void mglFont::mem_alloc()
{
	id = new wchar_t[numg];
	width[0] = new short[numg];	width[1] = new short[numg];
	width[2] = new short[numg];	width[3] = new short[numg];
	tr[0] = new int[numg];	numt[0] = new short[numg];
	tr[1] = new int[numg];	numt[1] = new short[numg];
	tr[2] = new int[numg];	numt[2] = new short[numg];
	tr[3] = new int[numg];	numt[3] = new short[numg];
	ln[0] = new int[numg];	numl[0] = new short[numg];
	ln[1] = new int[numg];	numl[1] = new short[numg];
	ln[2] = new int[numg];	numl[2] = new short[numg];
	ln[3] = new int[numg];	numl[3] = new short[numg];
}
//-----------------------------------------------------------------------------
// copy normal style as default for other styles
void mglFont::main_copy()
{
#pragma omp parallel sections
	{
#pragma omp section
		memcpy(numl[1],numl[0],numg*sizeof(short));
#pragma omp section
		memcpy(numl[2],numl[0],numg*sizeof(short));
#pragma omp section
		memcpy(numl[3],numl[0],numg*sizeof(short));
#pragma omp section
		memcpy(ln[1],ln[0],numg*sizeof(int));
#pragma omp section
		memcpy(ln[2],ln[0],numg*sizeof(int));
#pragma omp section
		memcpy(ln[3],ln[0],numg*sizeof(int));
#pragma omp section
		memcpy(numt[1],numt[0],numg*sizeof(short));
#pragma omp section
		memcpy(numt[2],numt[0],numg*sizeof(short));
#pragma omp section
		memcpy(numt[3],numt[0],numg*sizeof(short));
#pragma omp section
		memcpy(tr[1],tr[0],numg*sizeof(int));
#pragma omp section
		memcpy(tr[2],tr[0],numg*sizeof(int));
#pragma omp section
		memcpy(tr[3],tr[0],numg*sizeof(int));
#pragma omp section
		memcpy(width[1],width[0],numg*sizeof(short));
#pragma omp section
		memcpy(width[2],width[0],numg*sizeof(short));
#pragma omp section
		memcpy(width[3],width[0],numg*sizeof(short));
	}
}
//-----------------------------------------------------------------------------
bool mglFont::read_def()
{
	numg = mgl_numg;
	// copy default factor for other font styles;
	fact[1] = fact[2] = fact[3] = fact[0] = mgl_fact*mgl_fgen;
	Buf = new short[mgl_cur];	// prealocate buffer
	memset(Buf,0,mgl_cur*sizeof(short));
	// now allocate memory for all fonts
	mem_alloc();
	// and load symbols itself
#ifndef WIN32	// win32 don't initialized threads before main()
#pragma omp parallel for
#endif
	for(long i=0;i<int(numg);i++)
	{
		id[i] = mgl_gen_fnt[i][0];
		width[0][i] = mgl_gen_fnt[i][1];
		numl[0][i] = mgl_gen_fnt[i][2];
		ln[0][i] = mgl_gen_fnt[i][3];
		numt[0][i] = mgl_gen_fnt[i][4];
		tr[0][i] = mgl_gen_fnt[i][5];
	}
	memcpy(Buf, mgl_buf_fnt, mgl_cur*sizeof(short));
	numb = mgl_cur;
#ifndef WIN32	// win32 don't initialized threads before main()
	main_copy();	// copy normal style as default for other styles
#else
	memcpy(numl[1],numl[0],numg*sizeof(short));
	memcpy(numl[2],numl[0],numg*sizeof(short));
	memcpy(numl[3],numl[0],numg*sizeof(short));
	memcpy(ln[1],ln[0],numg*sizeof(int));
	memcpy(ln[2],ln[0],numg*sizeof(int));
	memcpy(ln[3],ln[0],numg*sizeof(int));
	memcpy(numt[1],numt[0],numg*sizeof(short));
	memcpy(numt[2],numt[0],numg*sizeof(short));
	memcpy(numt[3],numt[0],numg*sizeof(short));
	memcpy(tr[1],tr[0],numg*sizeof(int));
	memcpy(tr[2],tr[0],numg*sizeof(int));
	memcpy(tr[3],tr[0],numg*sizeof(int));
	memcpy(width[1],width[0],numg*sizeof(short));
	memcpy(width[2],width[0],numg*sizeof(short));
	memcpy(width[3],width[0],numg*sizeof(short));
#endif
	return true;
}
//-----------------------------------------------------------------------------
bool mglFont::read_data(const char *fname, float *ff, short *wdt, short *lnum, int *posl, short *tnum, int *post, std::vector<short> &buf)	// TODO add buffer for input file?!
{
	gzFile fp;
	char str[256];
	int n, tmpw, tmpnl, tmpnt, retVal;
	unsigned s, tmpi, tmppl, tmppt;
	fp = gzopen(fname,"r");	if(!fp)	return false;	// false if no file
	// first string is comment (not used), second string have information
	if(!gzgets(fp,str,256) || strncmp(str,"# font",6) || !gzgets(fp,str,256))
	{	gzclose(fp);	return false;	}
	retVal = sscanf(str, "%d%f%u", &n, ff, &s);
	//Check sscanf read all data  (3 items)
	if(retVal != 3)	{	gzclose(fp);	return false;	}

	for(int i=0;i<n;i++)
	{
		gzgets(fp,str,256);
		retVal = sscanf(str,"%u%d%d%u%d%u", &tmpi, &tmpw, &tmpnl, &tmppl, &tmpnt, &tmppt);
		if(retVal != 6)	{	gzclose(fp);	buf.clear();	return false;	}
		long j=Internal(unsigned(tmpi));	if(j<0)	continue;
		if(wdt)	wdt[j] = tmpw;
		lnum[j] = tmpnl;	posl[j] = -1-tmppl;
		tnum[j] = tmpnt;	post[j] = -1-tmppt;
	}
	for(unsigned i=0;i<s;i++)
	{
		for(int j=0;j<256;j++)	if((str[j] = gzgetc(fp))<=' ')	break;
		buf.push_back(atoi(str));
	}
	gzclose(fp);		// finish wire normal font
	return true;
}
//-----------------------------------------------------------------------------
bool mglFont::read_main(const char *fname, std::vector<short> &buf)	// TODO add buffer for input file?!
{
	gzFile fp;
	int tmpi, tmpw, tmpnl, tmpnt;
	unsigned s, tmppl, tmppt;
	char str[256];

	fp = gzopen(fname,"r");	if(!fp)	return false;	// this font must be in any case
	// first string is comment (not used), second string have information
	if(!gzgets(fp,str,256) || strncmp(str,"# font",6) || !gzgets(fp,str,256))
	{	gzclose(fp);	return false;	}
	sscanf(str, "%u%f%u", &numg, fact, &s);
	fact[1] = fact[2] = fact[3] = fact[0];	// copy default factor for other font styles;
	// now allocate memory for all fonts
	mem_alloc();
	// and load symbols itself
	for(size_t i=0;i<numg;i++)
	{
		gzgets(fp,str,256);
		sscanf(str,"%d%d%d%u%d%u", &tmpi, &tmpw, &tmpnl, &tmppl, &tmpnt, &tmppt);
		id[i] = tmpi;		width[0][i] = tmpw;
		numl[0][i] = tmpnl; ln[0][i] = tmppl;
		numt[0][i] = tmpnt;	tr[0][i] = tmppt;
	}
	for(unsigned i=0;i<s;i++)
	{
		for(int j=0;j<256;j++)	if((str[j] = gzgetc(fp))<=' ')	break;
		buf.push_back(atoi(str));
	}
	gzclose(fp);	// finish wire normal font
	main_copy();	// copy normal style as default for other styles
	return true;
}
//-----------------------------------------------------------------------------
//extern std::string mglGlobalMess;	///< Buffer for receiving global messages
bool mglFont::Load(const char *base, const char *path)
{
//	base = 0;
	char *buf=0,sep='/';
#ifdef WIN32
	sep='\\';
#endif
	char str[256];
	const char *oldLocale = setlocale(LC_NUMERIC,"C");
	if(!path)	path = MGL_FONT_PATH;
	if(base && *base)
	{
		buf = new char[strlen(base)+1];
		strcpy(buf,base);
		if(strchr(buf,sep))
		{
			int i;
			for(i=strlen(buf);i>=0 && buf[i]!=sep;i--);
			path = buf;		buf[i]=0;	base = buf+i+1;
		}
	}
	Clear();							// first clear old

	snprintf(str,256,"%s%c%s.vfm",path,sep,base?base:"");
	std::vector<short> norm, bold, ital, both;
	if(!(base && *base) || !read_main(str,norm))
	{
//		mglGlobalMess += "Load built-in font.\n";
		read_def();	setlocale(LC_NUMERIC,oldLocale);
		if(buf)	delete []buf;	return true;
	}
	fact[1] = fact[2] = fact[3] = fact[0];

#pragma omp parallel sections
	{
		//================== bold ===========================================
#pragma omp section
		{	char str[256];	snprintf(str,256,"%s%c%s_b.vfm",path,sep,base);	// this file may absent
		read_data(str, fact+1, width[1], numl[1], ln[1], numt[1], tr[1], bold);	}

		//================== italic =========================================
#pragma omp section
		{	char str[256];	snprintf(str,256,"%s%c%s_i.vfm",path,sep,base);
		read_data(str, fact+2, width[2], numl[2], ln[2], numt[2], tr[2], ital);	}

		//================== bold-italic ====================================
#pragma omp section
		{	char str[256];	snprintf(str,256,"%s%c%s_bi.vfm",path,sep,base);
		read_data(str, fact+3, width[3], numl[3], ln[3], numt[3], tr[3], both);	}
	}

	// now collect data
	numb = norm.size()+bold.size()+ital.size()+both.size();
	Buf = new short[numb];
#pragma omp parallel for
	for(long i=0;i<long(norm.size());i++)	Buf[i]=norm[i];
	long cur = norm.size(), len = long(bold.size());
	if(bold.size()>0)
#pragma omp parallel for
		for(long i=0;i<len;i++)	Buf[i+cur]=bold[i];
#pragma omp parallel for
	for(long i=0;i<numg;i++)	if(ln[1][i]<0)
	{	ln[1][i] = cur-1-ln[1][i];	tr[1][i] = cur-1-tr[1][i];	}
	cur += len;		len = long(ital.size());
	if(ital.size()>0)
#pragma omp parallel for
		for(long i=0;i<len;i++)	Buf[i+cur]=ital[i];
#pragma omp parallel for
	for(long i=0;i<numg;i++)	if(ln[2][i]<0)
	{	ln[2][i] = cur-1-ln[2][i];	tr[2][i] = cur-1-tr[2][i];	}
	cur += len;		len = long(both.size());
	if(both.size()>0)
#pragma omp parallel for
		for(long i=0;i<len;i++)	Buf[i+cur]=both[i];
#pragma omp parallel for
	for(long i=0;i<numg;i++)	if(ln[3][i]<0)
	{	ln[3][i] = cur-1-ln[3][i];	tr[3][i] = cur-1-tr[3][i];	}

	// Finally normalize all factors
	fact[0] *= mgl_fgen;	fact[1] *= mgl_fgen;
	fact[2] *= mgl_fgen;	fact[3] *= mgl_fgen;
	setlocale(LC_NUMERIC,oldLocale);
	if(buf)	delete []buf;
	return true;
}
//-----------------------------------------------------------------------------
#if MGL_HAVE_PTHREAD
extern pthread_mutex_t mutexRnd;
#endif
//-----------------------------------------------------------------------------
float mgl_cos[360];
void MGL_NO_EXPORT mgl_init()
{
#if MGL_HAVE_PTHREAD
	pthread_mutex_init(&mutexRnd,0);
#endif
#ifndef WIN32	// win32 don't initialized threads before main()
#pragma omp parallel for
#endif
	for(long i=0;i<360;i++)	mgl_cos[i] = cos(i*M_PI/180.);
}
//-----------------------------------------------------------------------------
mglFont::mglFont(const char *name, const char *path)
{
	parse = true;	numg=0;	gr=0;
//	if(this==&mglDefFont)	Load(name, path);	else	Copy(&mglDefFont);
	if(name && *name)	Load(name, path);
	else if(this!=&mglDefFont)	Copy(&mglDefFont);
	else
	{
		mgl_init();		// NOTE: this call init function for the library.
		Load(MGL_DEF_FONT_NAME,0);
	}
}
mglFont::~mglFont()	{	Clear();	}
void mglFont::Restore()	{	Copy(&mglDefFont);	}
//-----------------------------------------------------------------------------
void mglFont::Clear()
{
//	if(gr)	gr->Clf();
	if(numg)
	{
		delete []id;	delete []Buf;	numg = 0;
		delete [](width[0]);	delete [](width[1]);	delete [](width[2]);	delete [](width[3]);
		delete [](tr[0]);		delete [](tr[1]);		delete [](tr[2]);		delete [](tr[3]);
		delete [](ln[0]);		delete [](ln[1]);		delete [](ln[2]);		delete [](ln[3]);
		delete [](numt[0]);		delete [](numt[1]);		delete [](numt[2]);		delete [](numt[3]);
		delete [](numl[0]);		delete [](numl[1]);		delete [](numl[2]);		delete [](numl[3]);
	}
}
//-----------------------------------------------------------------------------
void mglFont::Copy(mglFont *f)
{
	if(!f || f==this)	return;
	Clear();
	numg = f->numg;		numb = f->numb;
	mem_alloc();
	// copy general data
#pragma omp parallel sections
	{
#pragma omp section
		memcpy(id,f->id,numg*sizeof(wchar_t));
#pragma omp section
		memcpy(width[0],f->width[0],numg*sizeof(short));
#pragma omp section
		memcpy(width[1],f->width[1],numg*sizeof(short));
#pragma omp section
		memcpy(width[2],f->width[2],numg*sizeof(short));
#pragma omp section
		memcpy(width[3],f->width[3],numg*sizeof(short));
#pragma omp section
		memcpy(tr[0],f->tr[0],numg*sizeof(unsigned));
#pragma omp section
		memcpy(tr[1],f->tr[1],numg*sizeof(unsigned));
#pragma omp section
		memcpy(tr[2],f->tr[2],numg*sizeof(unsigned));
#pragma omp section
		memcpy(tr[3],f->tr[3],numg*sizeof(unsigned));
#pragma omp section
		memcpy(numt[0],f->numt[0],numg*sizeof(short));
#pragma omp section
		memcpy(numt[1],f->numt[1],numg*sizeof(short));
#pragma omp section
		memcpy(numt[2],f->numt[2],numg*sizeof(short));
#pragma omp section
		memcpy(numt[3],f->numt[3],numg*sizeof(short));
#pragma omp section
		memcpy(ln[0],f->ln[0],numg*sizeof(unsigned));
#pragma omp section
		memcpy(ln[1],f->ln[1],numg*sizeof(unsigned));
#pragma omp section
		memcpy(ln[2],f->ln[2],numg*sizeof(unsigned));
#pragma omp section
		memcpy(ln[3],f->ln[3],numg*sizeof(unsigned));
#pragma omp section
		memcpy(numl[0],f->numl[0],numg*sizeof(short));
#pragma omp section
		memcpy(numl[1],f->numl[1],numg*sizeof(short));
#pragma omp section
		memcpy(numl[2],f->numl[2],numg*sizeof(short));
#pragma omp section
		memcpy(numl[3],f->numl[3],numg*sizeof(short));
#pragma omp section
		memcpy(fact,f->fact,4*sizeof(float));
	}
	// now copy symbols descriptions
	Buf = new short[numb];	memcpy(Buf, f->Buf, numb*sizeof(short));
}
//-----------------------------------------------------------------------------
