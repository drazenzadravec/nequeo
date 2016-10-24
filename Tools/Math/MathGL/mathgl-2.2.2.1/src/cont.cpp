/***************************************************************************
 * cont.cpp is part of Math Graphic Library
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
// NOTE: Borland before 2007 (i.e. < 0x0600) use <algorithm.h>, after 0x0630 it use <algorithm>.
// I don't find information about 2009, 2010 versions (i.e. 0x0610 and 0x0620).
// May be condition below can be rewritten as (__CODEGEARC__ >=  0x0600)
#if !defined(__BORLANDC__) || (__CODEGEARC__ >=  0x0630)
#include <algorithm>
#else
#include <algorithm.h>
#endif

#include "mgl2/surf.h"
#include "mgl2/cont.h"
#include "mgl2/data.h"
#include "mgl2/eval.h"
#include "mgl2/font.h"
#include "mgl2/base.h"
//-----------------------------------------------------------------------------
//
//	Text printing along a curve
//
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_string_curve(mglBase *gr,long f,long ,const long *ff,const long *nn,const wchar_t *text, const char *font, mreal size)
{
	if(f<0 || nn[f]==-1)	return;	// do nothing since there is no curve
	if(!font)	font="";
	int pos = strchr(font,'T') ? 1:-1, align;
	char cc=mglGetStyle(font,0,&align);		align = align&3;
	mreal c=cc ? -cc : gr->GetClrC(ff[f]);
	mreal h=gr->TextHeight(font,size)/2, tet, tt;
	wchar_t L[2]=L"a";
	register long i,j,k,m;

	std::vector<mglPoint> qa, qb;	// curves above and below original
	mglPoint p=gr->GetPntP(ff[f]), q=p, s=gr->GetPntP(ff[nn[f]]), l=!(s-q), t=l;
	qa.push_back(q+l*h);	qb.push_back(q-l*h);
	for(i=nn[f];i>=0 && i!=f;i=nn[i])	// construct curves
	{
		if(gr->Stop)	return;
		p=q;	q=s;	l=t;
		if(nn[i]>=0 && ff[nn[i]]>=0)	{	s=gr->GetPntP(ff[nn[i]]);	t=!(s-q);	}
		tet = t.x*l.y-t.y*l.x;
		tt = 1+fabs(t.x*l.x+t.y*l.y);
		if(tet>0)
		{	qa.push_back(q+l*h);	qa.push_back(q+t*h);	qb.push_back(q-(l+t)*(h/tt));	}
		else if(tet<0)
		{	qb.push_back(q-l*h);	qb.push_back(q-t*h);	qa.push_back(q+(l+t)*(h/tt));	}
		else
		{	qa.push_back(q+l*h);	qb.push_back(q-l*h);	}
	}
	if(pos>0)	qa=qb;
	// adjust text direction
	bool rev = align==2;
	char *fnt = new char[strlen(font)+3];	strcpy(fnt,font);
	if(qa[0].x>qa[1].x)
	{
		if(align==0){	strcat(fnt,":R");	align=2;	}
		else if(align==1)	rev = true;
		else		{	strcat(fnt,":L");	align=0;	}
	}
	if(rev)	reverse(qa.begin(),qa.end());
	long len = mgl_wcslen(text);
	mreal *wdt=new mreal[len+1];
	for(j=0;j<len;j++)	{	L[0]=text[j];	wdt[j]=1.2*gr->TextWidth(L,font,size);	}
	wdt[len]=0;

	// place glyphs points
	mglPoint *pt=new mglPoint[len+1];
	pt[0] = qa[0];	m = qa.size();

	mreal a,b,d,w,t1,t2;
	for(i=j=0,tt=0;j<len;j++)
	{
		if(gr->Stop)	{	delete []wdt;	delete []pt;	delete []fnt;	return;	}
		w = align==1 ? wdt[j] : (wdt[j]+wdt[j+1])/2;	p = pt[j];
		for(k=i+1;k<m;k++)	if((p-qa[k]).norm()>w)	break;
		if(k>i+1 && k<m)	tt=-1;
		i = k<m ? k-1 : m-2;		// check if end of curve
		q = qa[i];	s = qa[i+1];	// points of line segment
		a = (q-s)*(q-s);	b = (q-p)*(q-s);	d = (q-p)*(q-p)-w*w;
		w = sqrt(b*b-a*d);		// NOTE: b*b>a*d should be here!
		if(b*b>1e3*a*d)	{	t1 = d/(b+w);	t2 = d/(b-w);	}	// keep precision
		else			{	t1 = (b-w)/a;	t2 = (b+w)/a;	}
		if(t1<0 || t1<tt)	t1=t2;	// t1<t2 should be here!
		tt=t1;	pt[j+1] = q+(s-q)*tt;
	}
	if(rev)	pos=-pos;
	for(j=0;j<len;j++)	// draw text
	{
		L[0] = text[align!=2?j:len-1-j];	s = pt[j+1]-pt[j];	l = !s;
		gr->text_plot(gr->AddPnt(pt[j]+(pos*h)*l,c,s,-1,-1),L,font,size,0.05,c);
	}
	delete []wdt;	delete []pt;	delete []fnt;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textw_xyz(HMGL gr, HCDT x, HCDT y, HCDT z,const wchar_t *text, const char *font, const char *opt)
{
	long n=y->GetNx();
	if(mgl_check_dim1(gr,x,y,z,0,"Text"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("TextC",cgid++);

	long *nn = new long[n], *ff = new long[n];
	const mglData *mdx = dynamic_cast<const mglData *>(x);
	const mglData *mdy = dynamic_cast<const mglData *>(y);
	const mglData *mdz = dynamic_cast<const mglData *>(z);
	if(mdx && mdy && mdz)
#pragma omp parallel for
		for(long i=0;i<n;i++)
			ff[i] = gr->AddPnt(mglPoint(mdx->a[i],mdy->a[i],mdz->a[i]),-1);
	else
#pragma omp parallel for
		for(long i=0;i<n;i++)
			ff[i] = gr->AddPnt(mglPoint(x->v(i),y->v(i),z->v(i)),-1);
#pragma omp parallel for
	for(long i=1;i<n;i++)	nn[i-1] = i;
	nn[n-1]=-1;
	mgl_string_curve(gr,0,n,ff,nn,text,font,-1);
	gr->EndGroup();
	delete []ff;	delete []nn;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textw_xy(HMGL gr, HCDT x, HCDT y, const wchar_t *text, const char *font, const char *opt)
{
	gr->SaveState(opt);
	mglData z(y->GetNx());
	mreal zm = gr->AdjustZMin();	z.Fill(zm,zm);
	mgl_textw_xyz(gr,x,y,&z,text,font,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textw_y(HMGL gr, HCDT y, const wchar_t *text, const char *font, const char *opt)
{
	gr->SaveState(opt);
	mglData x(y->GetNx()), z(y->GetNx());
	x.Fill(gr->Min.x,gr->Max.x);
	mreal zm = gr->AdjustZMin();	z.Fill(zm,zm);
	mgl_textw_xyz(gr,&x,y,&z,text,font,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_xyz(HMGL gr, HCDT x, HCDT y, HCDT z,const char *text, const char *font, const char *opt)
{
	MGL_TO_WCS(text,mgl_textw_xyz(gr,x,y,z, wcs, font, opt));
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_xy(HMGL gr, HCDT x, HCDT y, const char *text, const char *font, const char *opt)
{
	mglData z(y->GetNx());
	mreal zm = gr->AdjustZMin();	z.Fill(zm,zm);
	mgl_text_xyz(gr,x,y,&z,text,font,opt);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_y(HMGL gr, HCDT y, const char *text, const char *font, const char *opt)
{
	mglData x(y->GetNx()), z(y->GetNx());
	x.Fill(gr->Min.x,gr->Max.x);
	mreal zm = gr->AdjustZMin();	z.Fill(zm,zm);
	mgl_text_xyz(gr,&x,y,&z,text,font,opt);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z,const char *text,const char *font, const char *opt,int l,int n,int lo)
{	char *s=new char[l+1];	memcpy(s,text,l);	s[l]=0;
char *f=new char[n+1];	memcpy(f,font,n);	f[n]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_text_xyz(_GR_, _DA_(x),_DA_(y), _DA_(z), s, f, o);
delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, const char *text, const char *font, const char *opt, int l,int n,int lo)
{	char *s=new char[l+1];	memcpy(s,text,l);	s[l]=0;
char *f=new char[n+1];	memcpy(f,font,n);	f[n]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_text_xy(_GR_, _DA_(x),_DA_(y),s,f,o);
delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_text_y_(uintptr_t *gr, uintptr_t *y, const char *text, const char *font, const char *opt, int l,int n,int lo)
{	char *s=new char[l+1];	memcpy(s,text,l);	s[l]=0;
char *f=new char[n+1];	memcpy(f,font,n);	f[n]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_text_y(_GR_, _DA_(y),s,f,o);	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
//
//	Cont series
//
//-----------------------------------------------------------------------------
struct mglSegment
{
	long next,prev;
	mglPoint p1,p2;
	mglSegment(mglPoint q1,mglPoint q2)	{p1=q1;p2=q2;next=prev=-1;}
};
// function for connecting arbitrary line segments
/*void MGL_NO_EXPORT mgl_connect(HMGL gr, mreal val, HCDT a, HCDT x, HCDT y, HCDT z, mreal c, int text,long ak)
{
	long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2 || x->GetNx()*x->GetNy()!=n*m || y->GetNx()*y->GetNy()!=n*m || z->GetNx()*z->GetNy()!=n*m)
	{	gr->SetWarn(mglWarnDim,"ContGen");	return;	}
	std::vector<mglSegment> ss,cc;

	register long i,j;
	mreal d1,d2,d3,d4;
	bool o1,o2,o3,o4;
	mglPoint p1,p2,p3,p4,q1,q2,q3,q4;
	for(i=0;i<n;i++)	for(j=0;j<m;j++)	// prepare segments
	{
		if(gr->Stop)	return;
		d1 = mgl_d(val,a->v(i,j,ak),a->v(i+1,j,ak));		o1 = d1>=0 && d1<1;
		d2 = mgl_d(val,a->v(i,j,ak),a->v(i,j+1,ak));		o2 = d2>=0 && d2<1;
		d3 = mgl_d(val,a->v(i+1,j+1,ak),a->v(i+1,j,ak));	o3 = d3>=0 && d3<1;
		d4 = mgl_d(val,a->v(i+1,j+1,ak),a->v(i,j+1,ak));	o4 = d4>=0 && d4<1;
		p1 = mglPoint(x->v(i,j), y->v(i,j),z->v(i,j));
		p2 = mglPoint(x->v(i+1,j), y->v(i+1,j),z->v(i+1,j));
		p3 = mglPoint(x->v(i,j+1), y->v(i,j+1),z->v(i,j+1));
		p4 = mglPoint(x->v(i+1,j+1), y->v(i+1,j+1),z->v(i+1,j+1));
		q1 = p1*(1-d1)+p2*d1;	q2 = p1*(1-d2)+p3*d1;
		q3 = p4*(1-d3)+p2*d3;	q4 = p4*(1-d4)+p3*d4;
		if(o1 && o2)	{	o1 = o2 = false;	ss.push_back(mglSegment(q1,q2));	}
		if(o1 && o3)	{	o1 = o3 = false;	ss.push_back(mglSegment(q1,q3));	}
		if(o1 && o4)	{	o1 = o4 = false;	ss.push_back(mglSegment(q1,q4));	}
		if(o2 && o3)	{	o2 = o3 = false;	ss.push_back(mglSegment(q2,q3));	}
		if(o2 && o4)	{	o2 = o4 = false;	ss.push_back(mglSegment(q2,q4));	}
		if(o3 && o4)	{	o3 = o4 = false;	ss.push_back(mglSegment(q3,q4));	}
	}
	// connect it
	if(ss.size()==0)	return;
	for(i=0;i<ss.size();i++)	// lets try most stupid algorithm (can be VERY slow)
	{
		mglSegment &s1=ss[i];
		for(j=0;j<ss.size();j++)
		{
			mglSegment &s2=ss[j];
			if(s2.prev<0 && s1.p2==s2.p1)	{	s1.next = j;	s2.prev=i;	continue;	}
			if(s2.next<0 && s1.p1==s2.p2)	{	s1.prev = j;	s2.next=i;	continue;	}
			//			if(s2.prev<0 && s1.p2==s2.p1)
			//			{	s1.next = j;	s2.prev=i;	continue;	}
		}
	}
}*/
//-----------------------------------------------------------------------------
// NOTE! returned must be deleted!!!
struct mglPnt2	{	mreal x,y;	mglPnt2(mreal xx=0,mreal yy=0)	{x=xx;y=yy;}	};
long *mgl_cont_prep(mreal val, HCDT a,long ak, std::vector<mglPnt2> &kk)
{
	long n=a->GetNx(), m=a->GetNy();
	mreal d, r, kx, ky;
	register long i,j,k, pc=0;
	kk.clear();
	// add intersection point of isoline and Y axis
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(ma)
	{
		for(j=0;j<m;j++)	for(i=0;i<n-1;i++)
		{
			d = mgl_d(val,ma->a[i+n*(j+m*ak)],ma->a[i+1+n*(j+m*ak)]);
			if(d>=0 && d<1)	kk.push_back(mglPnt2(i+d,j));
		}
		// add intersection point of isoline and X axis
		for(j=0;j<m-1;j++)	for(i=0;i<n;i++)
		{
			d = mgl_d(val,ma->a[i+n*(j+m*ak)],ma->a[i+n*(j+1+m*ak)]);
			if(d>=0 && d<1)	kk.push_back(mglPnt2(i,j+d));
		}
	}
	else	for(j=0;j<m-1;j++)	for(i=0;i<n-1;i++)
	{
		register mreal vv = a->v(i,j,ak);
		d = (i<n-1)?mgl_d(val,vv,a->v(i+1,j,ak)):-1;
		if(d>=0 && d<1)	kk.push_back(mglPnt2(i+d,j));
		d = (j<m-1)?mgl_d(val,vv,a->v(i,j+1,ak)):-1;
		if(d>=0 && d<1)	kk.push_back(mglPnt2(i,j+d));
	}

	pc = kk.size();
	if(pc==0)	return	NULL;	// deallocate arrays and finish if no point
	// allocate arrays for curve (nn - next, ff - prev)
	long *nn = new long[pc], *ff = new long[pc];
	// -1 is not parsed, -2 starting
	for(i=0;i<pc;i++)	nn[i] = ff[i] = -1;
	// connect points to line
	long i11,i12,i21,i22,j11,j12,j21,j22;
	j=-1;	// current point
	do{
		if(j>=0)
		{
			kx = kk[j].x;	ky = kk[j].y;	i = -1;
			i11 = long(kx+1e-5);	i12 = long(kx-1e-5);
			j11 = long(ky+1e-5);	j12 = long(ky-1e-5);
			r=10;
			for(k=0;k<pc;k++)	// find closest point in grid
			{
				if(k==j || k==ff[j] || ff[k]!=-1)	continue;	// point is marked
				i21 = long(kk[k].x+1e-5);	i22 = long(kk[k].x-1e-5);
				j21 = long(kk[k].y+1e-5);	j22 = long(kk[k].y-1e-5);
				// check if in the same cell
				register bool cond = (i11==i21 || i11==i22 || i12==i21 || i12==i22) &&
				(j11==j21 || j11==j22 || j12==j21 || j12==j22);
				d = hypot(kk[k].x-kx,kk[k].y-ky);	// if several then select closest
				if(cond && d<r)	{	r=d;	i=k;	}
			}
			if(i<0)	j = -1;	// no free close points
			else			// mark the point
			{	nn[j] = i;	ff[i] = j;	j = nn[i]<0 ? i : -1;	}
		}
		if(j<0)
		{
			for(k=0;k<pc;k++)	if(nn[k]==-1)	// first check edges
			{
				if(kk[k].x==0 || fabs(kk[k].x-n+1)<1e-5 || kk[k].y==0 || fabs(kk[k].y-m+1)<1e-5)
				{	nn[k]=-2;	j = k;	break;	}
			}
			if(j<0)	for(k=0;k<pc;k++)	if(nn[k]==-1)	// or any points inside
			{	j = k;	nn[k]=-2;	break;	}
		}
	}while(j>=0);
	delete []ff;	return nn;
}
//-----------------------------------------------------------------------------
// NOTE! All data MUST have the same size! Only first slice is used!
void MGL_EXPORT mgl_cont_gen(HMGL gr, mreal val, HCDT a, HCDT x, HCDT y, HCDT z, mreal c, int text,long ak)
{
	long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2 || x->GetNx()*x->GetNy()!=n*m || y->GetNx()*y->GetNy()!=n*m || z->GetNx()*z->GetNy()!=n*m)
	{	gr->SetWarn(mglWarnDim,"ContGen");	return;	}

	std::vector<mglPnt2> kk;
	long *nn = mgl_cont_prep(val, a, ak, kk), *ff;
	if(!nn)	return;	// nothing to do
	register long i, pc=kk.size();
	register mreal xx, yy;
	ff = new long[pc];	gr->Reserve(pc);
	for(i=0;i<pc;i++)
	{
		xx = kk[i].x;	yy = kk[i].y;
		ff[i] = gr->AddPnt(mglPoint(mgl_data_linear(x,xx,yy,ak), mgl_data_linear(y,xx,yy,ak), mgl_data_linear(z,xx,yy,ak)), c);
	}

	if(text && pc>1)
	{
		wchar_t wcs[64];
		mglprintf(wcs,64,L"%4.3g",val);
		mglPoint t;
		mreal del = 2*gr->TextWidth(wcs,"",-0.5);
		// find width and height of drawing area
		mreal ar=gr->GetRatio(), w=gr->FontFactor(), h;
		if(del<w/5)	del = w/5;
		if(ar<1) h=w/ar;	else {	h=w;	w*=ar;	}
		m=long(2*w/del)+1;	n=long(2*h/del)+1;	// don't need data size anymore
		long *oo=new long[n*m];
		mreal *rr=new mreal[n*m];
		for(i=0;i<n*m;i++)	{	oo[i]=-1;	rr[i]=del*del/4;	}

		register long j,k;
		for(k=0;k<pc;k++)	// print label several times if possible
		{
			if(nn[k]<0)	continue;
			t = gr->GetPntP(ff[k]);
			i = long(t.x/del);	t.x -= i*del;
			j = long(t.y/del);	t.y -= j*del;
			if(i<0 || i>=m || j<0 || j>=n)	continue;	// never should be here!
			xx = t.x*t.x+t.y*t.y;	i += m*j;
			if(rr[i]>xx)	{	rr[i]=xx;	oo[i]=k;	}
		}
		for(i=0;i<n*m;i++)	if(oo[i]>=0)
			mgl_string_curve(gr,oo[i],pc,ff,nn,wcs,"t:C",-0.5);
		delete []oo;	delete []rr;
	}
	for(i=0;i<pc;i++)	if(nn[i]>=0)	gr->line_plot(ff[i], ff[nn[i]]);
	delete []nn;	delete []ff;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_gen(HMGL gr, double val, HCDT a, HCDT x, HCDT y, HCDT z, const char *sch)
{
	bool text=(mglchr(sch,'t'));
	gr->SetPenPal(sch);
	mgl_cont_gen(gr,val,a,x,y,z,gr->CDef,text,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_xy_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	long n=z->GetNx(),m=z->GetNy();
	if(mgl_check_dim2(gr,x,y,z,0,"Cont"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Cont",cgid++);

	bool text=(mglchr(sch,'t'));
	bool fixed=(mglchr(sch,'_')) || (gr->Min.z==gr->Max.z);
	long s=gr->AddTexture(sch);
	gr->SetPenPal(sch);

	mglData xx, yy;
	if(x->GetNx()*x->GetNy()!=m*n || y->GetNx()*y->GetNy()!=m*n)	// make
	{
		xx.Create(n, m);		yy.Create(n, m);
		const mglData *mx = dynamic_cast<const mglData *>(x);
		const mglData *my = dynamic_cast<const mglData *>(y);
		if(mx && my)
#pragma omp parallel for collapse(2)
			for(long i=0;i<n;i++)	for(long j=0;j<m;j++)
			{	xx.a[i+n*j] = mx->a[i];	yy.a[i+n*j] = my->a[j];	}
		else
#pragma omp parallel for collapse(2)
			for(long i=0;i<n;i++)	for(long j=0;j<m;j++)
			{	xx.a[i+n*j] = x->v(i);	yy.a[i+n*j] = y->v(j);	}
		x = &xx;	y = &yy;
	}
	// x, y -- have the same size z
#pragma omp parallel
	{
		mglData zz(n, m);
#pragma omp for collapse(2)
		for(long j=0;j<z->GetNz();j++)	for(long i=0;i<v->GetNx();i++)
		{
			if(gr->Stop)	continue;
			mreal v0 = v->v(i), z0 = fixed ? gr->Min.z : v0;
			if(z->GetNz()>1)
				z0 = gr->Min.z+(gr->Max.z-gr->Min.z)*mreal(j)/(z->GetNz()-1);
			zz.Fill(z0,z0);
			mgl_cont_gen(gr,v0,z,x,y,&zz,gr->GetC(s,v0),text,j);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_val(HMGL gr, HCDT v, HCDT z, const char *sch, const char *opt)
{
	register long n = z->GetNx(), m = z->GetNy();
	if(m<2 || n<2)	{	gr->SetWarn(mglWarnLow,"Cont");	return;	}
	gr->SaveState(opt);
	mglData x(n, m), y(n, m);
	x.Fill(gr->Min.x,gr->Max.x,'x');
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_cont_xy_val(gr,v,&x,&y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_xy(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_cont_xy_val(gr,&v,x,y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont(HMGL gr, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_cont_val(gr,&v,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_xy_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_cont_xy_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(a), s, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_cont_val(_GR_, _DA_(v), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_cont_xy(_GR_, _DA_(x), _DA_(y), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont_(uintptr_t *gr, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_cont(_GR_, _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	ContF series
//
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_add_pnt(HMGL gr, mreal d, HCDT x, HCDT y, HCDT z, long i1, long j1, long i2, long j2, mreal c, bool edge)
{
	long res=-1;
	if(edge || (d>0 && d<1))
	{
		mglPoint p,u,v;
		p = mglPoint(x->v(i1,j1)*(1-d)+x->v(i2,j2)*d,
					 y->v(i1,j1)*(1-d)+y->v(i2,j2)*d,
					 z->v(i1,j1)*(1-d)+z->v(i2,j2)*d);
		u = mglPoint(x->dvx(i1,j1)*(1-d)+x->dvx(i2,j2)*d,
					 y->dvx(i1,j1)*(1-d)+y->dvx(i2,j2)*d,
					 z->dvx(i1,j1)*(1-d)+z->dvx(i2,j2)*d);
		v = mglPoint(x->dvy(i1,j1)*(1-d)+x->dvy(i2,j2)*d,
					 y->dvy(i1,j1)*(1-d)+y->dvy(i2,j2)*d,
					 z->dvy(i1,j1)*(1-d)+z->dvy(i2,j2)*d);
		res = gr->AddPnt(p,c,u^v);
	}
	return res;
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_add_range(HMGL gr, HCDT a, HCDT x, HCDT y, HCDT z, long i1, long j1, long di, long dj, mreal c, long &u1, long &u2, long ak, mreal v1, mreal v2)
{
	long i2=i1+di, j2=j1+dj;

	mreal f1 = a->v(i1,j1,ak),	f2 = a->v(i2,j2,ak), d1, d2;
	d1 = mgl_d(v1,f1,f2);
	u1 = mgl_add_pnt(gr,d1,x,y,z,i1,j1,i2,j2,c,false);
	d2 = mgl_d(v2,f1,f2);
	u2 = mgl_add_pnt(gr,d2,x,y,z,i1,j1,i2,j2,c,false);
	if(d1>d2)	{	j2=u1;	u1=u2;	u2=j2;	}
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_add_edges(HMGL gr, HCDT a, HCDT x, HCDT y, HCDT z, long i1, long j1, long di, long dj, mreal c, long &u1, long &u2, long ak, mreal v1, mreal v2)
{
	long i2=i1+di, j2=j1+dj;
	u1 = u2 = -1;

	mreal f1 = a->v(i1,j1,ak),	f2 = a->v(i2,j2,ak);
	if(f1<=v2 && f1>=v1)
		u1 = mgl_add_pnt(gr,0,x,y,z,i1,j1,i2,j2,c,true);
	if(f2<=v2 && f2>=v1)
		u2 = mgl_add_pnt(gr,1,x,y,z,i1,j1,i2,j2,c,true);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_gen(HMGL gr, mreal v1, mreal v2, HCDT a, HCDT x, HCDT y, HCDT z, mreal c, long ak)
{
	long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2 || x->GetNx()*x->GetNy()!=n*m || y->GetNx()*y->GetNy()!=n*m || z->GetNx()*z->GetNy()!=n*m)
	{	gr->SetWarn(mglWarnDim,"ContFGen");	return;	}

	register long i,j;
	gr->Reserve(n*m);
	long *kk = new long[4*n], l1,l2, r1,r2, t1,t2, u1,u2, b1,b2, d1,d2, p[8],num;
	memset(kk,-1,2*n*sizeof(long));
	for(i=0;i<n-1;i++)	// add intersection points for first line
	{
		if(gr->Stop)	{	delete []kk;	return;	}
		mgl_add_range(gr,a,x,y,z, i,0,1,0, c,u1,u2, ak,v1,v2);
		kk[4*i]=u1;		kk[4*i+1]=u2;
		mgl_add_edges(gr,a,x,y,z, i,0,1,0, c,d1,d2, ak,v1,v2);
		kk[4*i+2]=d1;		kk[4*i+3]=d2;
	}
	for(j=1;j<m;j++)	// add intersection points
	{
		mgl_add_range(gr,a,x,y,z, 0,j-1,0,1, c,r1,r2, ak,v1,v2);
		for(i=0;i<n-1;i++)
		{
			if(gr->Stop)	{	delete []kk;	return;	}
			l1 = r1;		l2 = r2;	num=0;
			t1 = kk[4*i];	t2 = kk[4*i+1];
			b1 = kk[4*i+2];	b2 = kk[4*i+3];
			// right edge
			mgl_add_range(gr,a,x,y,z, i+1,j-1,0,1, c,r1,r2, ak,v1,v2);
			// top edge
			mgl_add_range(gr,a,x,y,z, i,j,1,0, c,u1,u2, ak,v1,v2);
			kk[4*i]=u1;		kk[4*i+1]=u2;
			mgl_add_edges(gr,a,x,y,z, i,j,1,0, c,d1,d2, ak,v1,v2);
			kk[4*i+2]=d1;	kk[4*i+3]=d2;
			// collect points
			if(b1>=0)	p[num++] = b1;	if(t1>=0)	p[num++] = t1;
			if(t2>=0)	p[num++] = t2;	if(b2>=0)	p[num++] = b2;
			if(r1>=0)	p[num++] = r1;	if(r2>=0)	p[num++] = r2;
			if(d2>=0)	p[num++] = d2;	if(u2>=0)	p[num++] = u2;
			if(u1>=0)	p[num++] = u1;	if(d1>=0)	p[num++] = d1;
			if(l2>=0)	p[num++] = l2;	if(l1>=0)	p[num++] = l1;
			// draw it
			if(num<3)	continue;
			if(num==4)	gr->quad_plot(p[0],p[1],p[3],p[2]);
			else if(num==3)	gr->trig_plot(p[0],p[1],p[2]);
			else if(num==5)
			{
				gr->quad_plot(p[0],p[1],p[3],p[2]);
				gr->trig_plot(p[0],p[3],p[4]);
			}
			else if(num==6)
			{
				gr->quad_plot(p[0],p[1],p[3],p[2]);
				gr->quad_plot(p[0],p[3],p[5],p[4]);
			}
		}
	}
	delete []kk;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_gen(HMGL gr, double v1, double v2, HCDT a, HCDT x, HCDT y, HCDT z, const char *c)
{
	gr->SetPenPal(c);
	mgl_contf_gen(gr,v1,v2,a,x,y,z,gr->CDef,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_xy_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	long n=z->GetNx(),m=z->GetNy();
	if(mgl_check_dim2(gr,x,y,z,0,"ContF"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("ContF",cgid++);
	long s=gr->AddTexture(sch);

	bool fixed=(mglchr(sch,'_')) || (gr->Min.z==gr->Max.z);
	mglData xx, yy;
	if(x->GetNx()*x->GetNy()!=m*n || y->GetNx()*y->GetNy()!=m*n)	// make
	{
		xx.Create(n, m);		yy.Create(n, m);
		const mglData *mx = dynamic_cast<const mglData *>(x);
		const mglData *my = dynamic_cast<const mglData *>(y);
		if(mx && my)
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = mx->a[i];	yy.a[i+n*j] = my->a[j];	}
		else
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = x->v(i);	yy.a[i+n*j] = y->v(j);	}
		x = &xx;	y = &yy;
	}
	// x, y -- have the same size z
#pragma omp parallel
	{
		mglData zz(n, m);
#pragma omp for collapse(2)
		for(long j=0;j<z->GetNz();j++)	for(long i=0;i<v->GetNx()-1;i++)
		{
			if(gr->Stop)	continue;
			mreal v0 = v->v(i), z0 = fixed ? gr->Min.z : v0;
			if(z->GetNz()>1)
				z0 = gr->Min.z+(gr->Max.z-gr->Min.z)*mreal(j)/(z->GetNz()-1);
			zz.Fill(z0,z0);
			mgl_contf_gen(gr,v0,v->v(i+1),z,x,y,&zz,gr->GetC(s,v0),j);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_val(HMGL gr, HCDT v, HCDT z, const char *sch, const char *opt)
{
	register long n = z->GetNx(), m = z->GetNy();
	if(n<2 || m<2)	{	gr->SetWarn(mglWarnLow,"Cont");	return;	}
	gr->SaveState(opt);
	mglData x(n, m), y(n, m);
	x.Fill(gr->Min.x,gr->Max.x,'x');
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_contf_xy_val(gr,v,&x,&y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_xy(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num+2);	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contf_xy_val(gr,&v,x,y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf(HMGL gr, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num+2);	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contf_val(gr,&v,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_xy_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contf_xy_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(a), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contf_val(_GR_, _DA_(v), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contf_xy(_GR_, _DA_(x), _DA_(y), _DA_(a), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf_(uintptr_t *gr, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contf(_GR_, _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	ContD series
//
//-----------------------------------------------------------------------------
int MGL_NO_EXPORT mgl_get_ncol(const char *sch, char *res)
{
	register long i,j=0;
	if(sch)	for(i=0;sch[i]&&sch[i]!=':';i++)	if(strchr(MGL_COLORS,sch[i]))
	{	if(res)	res[j]=sch[i];	j++;	}
	return j?j:strlen(MGL_DEF_PAL);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_xy_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	long i,j=0,n=z->GetNx(),m=z->GetNy();
	if(mgl_check_dim2(gr,x,y,z,0,"ContD"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("ContD",cgid++);

	bool fixed=(mglchr(sch,'_')) || (gr->Min.z==gr->Max.z);
	if(sch)	for(i=0;sch[i];i++)	if(strchr(MGL_COLORS,sch[i]))	j++;
	if(j==0)	sch = MGL_DEF_PAL;
	long s = gr->AddTexture(sch,1);
	int nc = gr->GetNumPal(s*256);
	mglData xx, yy;
	if(x->GetNx()*x->GetNy()!=m*n || y->GetNx()*y->GetNy()!=m*n)	// make
	{
		xx.Create(n, m);		yy.Create(n, m);
		const mglData *mx = dynamic_cast<const mglData *>(x);
		const mglData *my = dynamic_cast<const mglData *>(y);
		if(mx && my)
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = mx->a[i];	yy.a[i+n*j] = my->a[j];	}
		else
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = x->v(i);	yy.a[i+n*j] = y->v(j);	}
		x = &xx;	y = &yy;
	}
	// x, y -- have the same size z
	mreal dc = nc>1 ? 1/(MGL_FEPSILON*(nc-1)) : 0;
#pragma omp parallel
	{
		mglData zz(n, m);
#pragma omp for collapse(2)
		for(long j=0;j<z->GetNz();j++)	for(long i=0;i<v->GetNx()-1;i++)
		{
			if(gr->Stop)	continue;
			mreal v0 = v->v(i), z0 = fixed ? gr->Min.z : v0;
			if(z->GetNz()>1)
				z0 = gr->Min.z+(gr->Max.z-gr->Min.z)*mreal(j)/(z->GetNz()-1);
			zz.Fill(z0,z0);
			mgl_contf_gen(gr,v0,v->v(i+1),z,x,y,&zz,s+i*dc,j);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_val(HMGL gr, HCDT v, HCDT z, const char *sch, const char *opt)
{
	register long n = z->GetNx(), m = z->GetNy();
	if(n<2 || m<2)	{	gr->SetWarn(mglWarnLow,"ContD");	return;	}
	gr->SaveState(opt);
	mglData x(n, m), y(n, m);
	x.Fill(gr->Min.x,gr->Max.x,'x');
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_contd_xy_val(gr,v,&x,&y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_xy(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	gr->SaveState(opt);
	mglData v(mgl_get_ncol(sch,0)+1);
	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contd_xy_val(gr,&v,x,y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd(HMGL gr, HCDT z, const char *sch, const char *opt)
{
	gr->SaveState(opt);
	mglData v(mgl_get_ncol(sch,0)+1);
	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contd_val(gr,&v,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_xy_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contd_xy_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(a), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contd_val(_GR_, _DA_(v), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contd_xy(_GR_, _DA_(x), _DA_(y), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contd_(uintptr_t *gr, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contd(_GR_, _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	ContV series
//
//-----------------------------------------------------------------------------
// NOTE! All data MUST have the same size! Only first slice is used!
void MGL_EXPORT mgl_contv_gen(HMGL gr, mreal val, mreal dval, HCDT a, HCDT x, HCDT y, HCDT z, mreal c, long ak)
{
	long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2 || x->GetNx()*x->GetNy()!=n*m || y->GetNx()*y->GetNy()!=n*m || z->GetNx()*z->GetNy()!=n*m)
	{	gr->SetWarn(mglWarnDim,"ContGen");	return;	}

	std::vector<mglPnt2> kk;
	long *nn = mgl_cont_prep(val, a, ak, kk), *ff;
	if(!nn)	return;	// nothing to do
	register long i, pc=kk.size();
	register mreal xx, yy;
	ff = new long[2*pc];	gr->Reserve(2*pc);
	mglPoint p,q;
	for(i=0;i<pc;i++)
	{
		xx = kk[i].x;	yy = kk[i].y;
		p = mglPoint(mgl_data_linear(x,xx,yy,ak), mgl_data_linear(y,xx,yy,ak), mgl_data_linear(z,xx,yy,ak));
		q = mglPoint(p.y,-p.x);		ff[i] = gr->AddPnt(p, c, q);
		ff[i+pc] = gr->AddPnt(mglPoint(p.x, p.y, p.z+dval), c, q);
	}

	for(i=0;i<pc;i++)	if(nn[i]>=0)	gr->quad_plot(ff[i], ff[nn[i]], ff[i+pc], ff[nn[i]+pc]);
	delete []nn;	delete []ff;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_xy_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	long n=z->GetNx(),m=z->GetNy();
	if(mgl_check_dim2(gr,x,y,z,0,"ContV"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("ContV",cgid++);
	bool fixed=(mglchr(sch,'_')) || (gr->Min.z==gr->Max.z);
	long s=gr->AddTexture(sch);
	gr->SetPenPal(sch);

	mglData xx, yy;
	if(x->GetNx()*x->GetNy()!=m*n || y->GetNx()*y->GetNy()!=m*n)	// make
	{
		xx.Create(n, m);		yy.Create(n, m);
		const mglData *mx = dynamic_cast<const mglData *>(x);
		const mglData *my = dynamic_cast<const mglData *>(y);
		if(mx && my)
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = mx->a[i];	yy.a[i+n*j] = my->a[j];	}
		else
#pragma omp parallel for collapse(2)
			for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			{	xx.a[i+n*j] = x->v(i);	yy.a[i+n*j] = y->v(j);	}
		x = &xx;	y = &yy;
	}
	// x, y -- have the same size z
#pragma omp parallel
	{
		mglData zz(n, m);
#pragma omp for collapse(2)
		for(long j=0;j<z->GetNz();j++)	for(long i=0;i<v->GetNx();i++)
		{
			if(gr->Stop)	continue;
			mreal v0 = v->v(i), z0 = fixed ? gr->Min.z : v0;
			if(z->GetNz()>1)	z0 = gr->Min.z+(gr->Max.z-gr->Min.z)*mreal(j)/(z->GetNz()-1);
			zz.Fill(z0,z0);
			mreal dv = (gr->Max.c-gr->Min.c)/8;
			if(i>0)	dv = v->v(i-1)-v->v(i);
			else if(i<v->GetNx()-1)	dv = v->v(i)-v->v(i+1);
			if(fixed)	dv=-dv;
			mgl_contv_gen(gr,v0,dv,z,x,y,&zz,gr->GetC(s,v0),j);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_val(HMGL gr, HCDT v, HCDT z, const char *sch, const char *opt)
{
	register long n = z->GetNx(), m = z->GetNy();
	if(n<2 || m<2)	{	gr->SetWarn(mglWarnLow,"Cont");	return;	}
	gr->SaveState(opt);
	mglData x(n, m), y(n, m);
	x.Fill(gr->Min.x,gr->Max.x,'x');
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_contv_xy_val(gr,v,&x,&y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_xy(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_contv_xy_val(gr,&v,x,y,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv(HMGL gr, HCDT z, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_contv_val(gr,&v,z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_xy_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contv_xy_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(a), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contv_val(_GR_, _DA_(v), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contv_xy(_GR_, _DA_(x), _DA_(y), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contv_(uintptr_t *gr, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_contv(_GR_, _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Cont3 series
//
//-----------------------------------------------------------------------------
struct _mgl_slice	{	mglData x,y,z,a;	};
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_get_slice(_mgl_slice &s, HCDT x, HCDT y, HCDT z, HCDT a, char dir, mreal d, bool both)
{
	register long i,j,i0,n=a->GetNx(),m=a->GetNy(),l=a->GetNz(), nx=1,ny=1,p;

	if(dir=='x')	{	nx = m;	ny = l;	if(d<0)	d = n/2.;	}
	if(dir=='y')	{	nx = n;	ny = l;	if(d<0)	d = m/2.;	}
	if(dir=='z')	{	nx = n;	ny = m;	if(d<0)	d = l/2.;	}
	s.x.Create(nx,ny);	s.y.Create(nx,ny);
	s.z.Create(nx,ny);	s.a.Create(nx,ny);
	p = long(d);	d -= p;
	if(dir=='x' && p>=n-1)	{	d+=p-n+2;	p=n-2;	}
	if(dir=='y' && p>=m-1)	{	d+=p-m+2.;	p=m-2;	}
	if(dir=='z' && p>=l-1)	{	d+=p-l+2;	p=l-2;	}
	mreal v;

	if(both)
	{
		if(dir=='x')	for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
		{
			i0 = i+nx*j;
			s.x.a[i0] = x->v(p,i,j)*(1-d) + x->v(p+1,i,j)*d;
			s.y.a[i0] = y->v(p,i,j)*(1-d) + y->v(p+1,i,j)*d;
			s.z.a[i0] = z->v(p,i,j)*(1-d) + z->v(p+1,i,j)*d;
			s.a.a[i0] = a->v(p,i,j)*(1-d) + a->v(p+1,i,j)*d;
		}
		if(dir=='y')	for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
		{
			i0 = i+nx*j;
			s.x.a[i0] = x->v(i,p,j)*(1-d) + x->v(i,p+1,j)*d;
			s.y.a[i0] = y->v(i,p,j)*(1-d) + y->v(i,p+1,j)*d;
			s.z.a[i0] = z->v(i,p,j)*(1-d) + z->v(i,p+1,j)*d;
			s.a.a[i0] = a->v(i,p,j)*(1-d) + a->v(i,p+1,j)*d;
		}
		if(dir=='z')	for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
		{
			i0 = i+nx*j;
			s.x.a[i0] = x->v(i,j,p)*(1-d) + x->v(i,j,p+1)*d;
			s.y.a[i0] = y->v(i,j,p)*(1-d) + y->v(i,j,p+1)*d;
			s.z.a[i0] = z->v(i,j,p)*(1-d) + z->v(i,j,p+1)*d;
			s.a.a[i0] = a->v(i,j,p)*(1-d) + a->v(i,j,p+1)*d;
		}
	}
	else	// x, y, z -- vectors
	{
		if(dir=='x')
		{
			v = x->v(p)*(1-d)+x->v(p+1)*d;
			for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
			{
				i0 = i+nx*j;	s.x.a[i0] = v;
				s.y.a[i0] = y->v(i);	s.z.a[i0] = z->v(j);
				s.a.a[i0] = a->v(p,i,j)*(1-d) + a->v(p+1,i,j)*d;
			}
		}
		if(dir=='y')
		{
			v = y->v(p)*(1-d)+y->v(p+1)*d;
			for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
			{
				i0 = i+nx*j;	s.y.a[i0] = v;
				s.x.a[i0] = x->v(i);	s.z.a[i0] = z->v(j);
				s.a.a[i0] = a->v(i,p,j)*(1-d) + a->v(i,p+1,j)*d;
			}
		}
		if(dir=='z')
		{
			v = z->v(p)*(1-d)+z->v(p+1)*d;
			for(j=0;j<ny;j++)	for(i=0;i<nx;i++)
			{
				i0 = i+nx*j;	s.z.a[i0] = v;
				s.x.a[i0] = x->v(i);	s.y.a[i0] = y->v(j);
				s.a.a[i0] = a->v(i,j,p)*(1-d) + a->v(i,j,p+1)*d;
			}
		}
	}
}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_get_slice_md(_mgl_slice &s, const mglData *x, const mglData *y, const mglData *z, const mglData *a, char dir, mreal d, bool both)
{
	long n=a->nx,m=a->ny,l=a->nz, nx=1,ny=1,p;

	if(dir=='x')	{	nx = m;	ny = l;	if(d<0)	d = n/2.;	}
	if(dir=='y')	{	nx = n;	ny = l;	if(d<0)	d = m/2.;	}
	if(dir=='z')	{	nx = n;	ny = m;	if(d<0)	d = l/2.;	}
	s.x.Create(nx,ny);	s.y.Create(nx,ny);
	s.z.Create(nx,ny);	s.a.Create(nx,ny);
	p = long(d);	d -= p;
	if(dir=='x' && p>=n-1)	{	d+=p-n+2;	p=n-2;	}
	if(dir=='y' && p>=m-1)	{	d+=p-m+2.;	p=m-2;	}
	if(dir=='z' && p>=l-1)	{	d+=p-l+2;	p=l-2;	}
	mreal v;

	if(both)
	{
		if(dir=='x')
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = p+n*(i+m*j);
				s.x.a[i0] = x->a[i1]*(1-d) + x->a[i1+1]*d;
				s.y.a[i0] = y->a[i1]*(1-d) + y->a[i1+1]*d;
				s.z.a[i0] = z->a[i1]*(1-d) + z->a[i1+1]*d;
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+1]*d;
			}
		if(dir=='y')
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = i+n*(p+m*j);
				s.x.a[i0] = x->a[i1]*(1-d) + x->a[i1+n]*d;
				s.y.a[i0] = y->a[i1]*(1-d) + y->a[i1+n]*d;
				s.z.a[i0] = z->a[i1]*(1-d) + z->a[i1+n]*d;
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+n]*d;
			}
		if(dir=='z')
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = i+n*(j+m*p);
				s.x.a[i0] = x->a[i1]*(1-d) + x->a[i1+n*m]*d;
				s.y.a[i0] = y->a[i1]*(1-d) + y->a[i1+n*m]*d;
				s.z.a[i0] = z->a[i1]*(1-d) + z->a[i1+n*m]*d;
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+n*m]*d;
			}
	}
	else	// x, y, z -- vectors
	{
		if(dir=='x')
		{
			v = x->a[p]*(1-d)+x->a[p+1]*d;
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = p+n*(i+m*j);
				s.x.a[i0] = v;	s.y.a[i0] = y->a[i];	s.z.a[i0] = z->a[j];
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+1]*d;
			}
		}
		if(dir=='y')
		{
			v = y->a[p]*(1-d)+y->a[p+1]*d;
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = i+n*(p+m*j);
				s.x.a[i0] = x->a[i];	s.y.a[i0] = v;	s.z.a[i0] = z->a[j];
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+n]*d;
			}
		}
		if(dir=='z')
		{
			v = z->a[p]*(1-d)+z->a[p+1]*d;
#pragma omp parallel for collapse(2)
			for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			{
				register long i0 = i+nx*j, i1 = i+n*(j+m*p);
				s.x.a[i0] = x->a[i];	s.y.a[i0] = y->a[j];	s.z.a[i0] = v;
				s.a.a[i0] = a->a[i1]*(1-d) + a->a[i1+n*m]*d;
			}
		}
	}
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_xyz_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	bool both = mgl_isboth(x,y,z,a);
	if(mgl_check_dim3(gr,both,x,y,z,a,0,"Cont3"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Cont3",cgid++);
	char dir='y';
	if(mglchr(sch,'x'))	dir='x';
	if(mglchr(sch,'z'))	dir='z';

	bool text=(mglchr(sch,'t'));
	long ss=gr->AddTexture(sch);
	gr->SetPenPal(sch);

	_mgl_slice s;
	const mglData *mx = dynamic_cast<const mglData *>(x);
	const mglData *my = dynamic_cast<const mglData *>(y);
	const mglData *mz = dynamic_cast<const mglData *>(z);
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(mx&&my&&mz&&ma)	mgl_get_slice_md(s,mx,my,mz,ma,dir,sVal,both);
	else mgl_get_slice(s,x,y,z,a,dir,sVal,both);
#pragma omp parallel for
	for(long i=0;i<v->GetNx();i++)
	{
		register mreal v0 = v->v(i);
		mgl_cont_gen(gr,v0,&s.a,&s.x,&s.y,&s.z,gr->GetC(ss,v0),text,0);
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_val(HMGL gr, HCDT v, HCDT a, const char *sch, double sVal, const char *opt)
{
	gr->SaveState(opt);
	mglData x(a->GetNx()), y(a->GetNy()),z(a->GetNz());
	x.Fill(gr->Min.x,gr->Max.x);
	y.Fill(gr->Min.y,gr->Max.y);
	z.Fill(gr->Min.z,gr->Max.z);
	mgl_cont3_xyz_val(gr,v,&x,&y,&z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont3");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_cont3_xyz_val(gr,&v,x,y,z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3(HMGL gr, HCDT a, const char *sch, double sVal, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Cont3");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_cont3_val(gr,&v,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_xyz_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cont3_xyz_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cont3_val(_GR_, _DA_(v), _DA_(a), s, *sVal, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cont3_xyz(_GR_, _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cont3_(uintptr_t *gr, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cont3(_GR_, _DA_(a), s, *sVal, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Dens3 series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dens3_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	bool both = mgl_isboth(x,y,z,a);
	if(mgl_check_dim3(gr,both,x,y,z,a,0,"Dens3"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Dens3",cgid++);
	char dir='y';
	if(mglchr(sch,'x'))	dir='x';
	if(mglchr(sch,'z'))	dir='z';

	_mgl_slice s;
	const mglData *mx = dynamic_cast<const mglData *>(x);
	const mglData *my = dynamic_cast<const mglData *>(y);
	const mglData *mz = dynamic_cast<const mglData *>(z);
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(mx&&my&&mz&&ma)	mgl_get_slice_md(s,mx,my,mz,ma,dir,sVal,both);
	else mgl_get_slice(s,x,y,z,a,dir,sVal,both);
	mgl_surfc_xy(gr,&s.x,&s.y,&s.z,&s.a,sch,0);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dens3(HMGL gr, HCDT a, const char *sch, double sVal, const char *opt)
{
	gr->SaveState(opt);
	mglData x(a->GetNx()), y(a->GetNy()),z(a->GetNz());
	x.Fill(gr->Min.x,gr->Max.x);
	y.Fill(gr->Min.y,gr->Max.y);
	z.Fill(gr->Min.z,gr->Max.z);
	mgl_dens3_xyz(gr,&x,&y,&z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dens3_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_dens3_xyz(_GR_, _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dens3_(uintptr_t *gr, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_dens3(_GR_, _DA_(a), s, *sVal, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Grid3 series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_grid3_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	bool both = mgl_isboth(x,y,z,a);
	if(mgl_check_dim3(gr,both,x,y,z,a,0,"Grid3"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Grid3",cgid++);
	char dir='y';
	if(mglchr(sch,'x'))	dir='x';
	if(mglchr(sch,'z'))	dir='z';

	_mgl_slice s;
	const mglData *mx = dynamic_cast<const mglData *>(x);
	const mglData *my = dynamic_cast<const mglData *>(y);
	const mglData *mz = dynamic_cast<const mglData *>(z);
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(mx&&my&&mz&&ma)	mgl_get_slice_md(s,mx,my,mz,ma,dir,sVal,both);
	else mgl_get_slice(s,x,y,z,a,dir,sVal,both);
	mgl_mesh_xy(gr,&s.x,&s.y,&s.z,sch,0);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_grid3(HMGL gr, HCDT a, const char *sch, double sVal, const char *opt)
{
	gr->SaveState(opt);
	mglData x(a->GetNx()), y(a->GetNy()), z(a->GetNz());
	x.Fill(gr->Min.x,gr->Max.x);
	y.Fill(gr->Min.y,gr->Max.y);
	z.Fill(gr->Min.z,gr->Max.z);
	mgl_grid3_xyz(gr,&x,&y,&z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_grid3_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_grid3_xyz(_GR_, _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_grid3_(uintptr_t *gr, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_grid3(_GR_, _DA_(a), s, *sVal, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	ContF3 series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_xyz_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	bool both = mgl_isboth(x,y,z,a);
	if(mgl_check_dim3(gr,both,x,y,z,a,0,"ContF3"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("ContF3",cgid++);
	char dir='y';
	if(mglchr(sch,'x'))	dir='x';
	if(mglchr(sch,'z'))	dir='z';

	long ss=gr->AddTexture(sch);
	_mgl_slice s;
	const mglData *mx = dynamic_cast<const mglData *>(x);
	const mglData *my = dynamic_cast<const mglData *>(y);
	const mglData *mz = dynamic_cast<const mglData *>(z);
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(mx&&my&&mz&&ma)	mgl_get_slice_md(s,mx,my,mz,ma,dir,sVal,both);
	else mgl_get_slice(s,x,y,z,a,dir,sVal,both);
#pragma omp parallel for
	for(long i=0;i<v->GetNx()-1;i++)
	{
		register mreal v0 = v->v(i);
		mgl_contf_gen(gr,v0,v->v(i+1),&s.a,&s.x,&s.y,&s.z,gr->GetC(ss,v0),0);
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_val(HMGL gr, HCDT v, HCDT a, const char *sch, double sVal, const char *opt)
{
	gr->SaveState(opt);
	mglData x(a->GetNx()), y(a->GetNy()),z(a->GetNz());
	x.Fill(gr->Min.x,gr->Max.x);
	y.Fill(gr->Min.y,gr->Max.y);
	z.Fill(gr->Min.z,gr->Max.z);
	mgl_contf3_xyz_val(gr,v,&x,&y,&z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, double sVal, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"ContF3");	return;	}
	mglData v(Num+2);	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contf3_xyz_val(gr,&v,x,y,z,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3(HMGL gr, HCDT a, const char *sch, double sVal, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?7:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"ContF3");	return;	}
	mglData v(Num+2);	v.Fill(gr->Min.c, gr->Max.c);
	mgl_contf3_val(gr,&v,a,sch,sVal,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_xyz_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_contf3_xyz_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_contf3_val(_GR_, _DA_(v), _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_contf3_xyz(_GR_, _DA_(x), _DA_(y), _DA_(z), _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_contf3_(uintptr_t *gr, uintptr_t *a, const char *sch, mreal *sVal, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_contf3(_GR_, _DA_(a), s, *sVal, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Axial series
//
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_find_prev(long i, long pc, long *nn)
{
	for(long k=0;k<pc;k++)	if(nn[k]==i)	return k;
	return -1;
}
void MGL_NO_EXPORT mgl_axial_plot(mglBase *gr,long pc, mglPoint *ff, long *nn,char dir,mreal cc,int wire)
{
	mglPoint a(0,0,1),b,c,p,q1,q2;
	if(dir=='x')	a = mglPoint(1,0,0);
	if(dir=='y')	a = mglPoint(0,1,0);
	b = !a;	c = a^b;

	register long i,j,k;
	long p1,p2,p3,p4;
	gr->Reserve(pc*82);
	for(i=0;i<pc;i++)
	{
		if(gr->Stop)	return;
		k = mgl_find_prev(i,pc,nn);
		if(nn[i]<0)	continue;
		q1 = k<0 ? ff[nn[i]]-ff[i]  : (ff[nn[i]]-ff[k])*0.5;
		q2 = nn[nn[i]]<0 ? ff[nn[i]]-ff[i]  : (ff[nn[nn[i]]]-ff[i])*0.5;

		p = a*ff[i].y + c*ff[i].x;
		p1 = wire ? gr->AddPnt(p,cc) : gr->AddPnt(p,cc,(a*q1.y + c*q1.x)^b);
		p = a*ff[nn[i]].y + c*ff[nn[i]].x;
		p2 = wire ? gr->AddPnt(p,cc) : gr->AddPnt(p,cc,(a*q2.y + c*q2.x)^b);
		if(wire==1)	gr->line_plot(p1,p2);
		else if(wire)	{	gr->mark_plot(p1,'.');	gr->mark_plot(p2,'.');	}

		for(j=1;j<41;j++)
		{
			p3 = p1;	p4 = p2;
			register float co = mgl_cos[(j*18)%360], si = mgl_cos[(270+j*18)%360];
//			fi = j*M_PI/20;		si = sin(fi);	co = cos(fi);
			p = a*ff[i].y + b*(si*ff[i].x) +  c*(co*ff[i].x);
			p1 = wire ?	gr->AddPnt(p,cc) : gr->AddPnt(p,cc,(a*q1.y + b*(si*q1.x) +  c*(co*q1.x))^(b*co-c*si));
			p = a*ff[nn[i]].y + b*(si*ff[nn[i]].x) +  c*(co*ff[nn[i]].x);
			p2 = wire ?	gr->AddPnt(p,cc) : gr->AddPnt(p,cc,(a*q2.y + b*(si*q2.x) +  c*(co*q2.x))^(b*co-c*si));
			if(wire==1)
			{	gr->line_plot(p1,p2);	gr->line_plot(p1,p3);
			gr->line_plot(p4,p2);	gr->line_plot(p4,p3);	}
			else if(wire)	{	gr->mark_plot(p1,'.');	gr->mark_plot(p2,'.');	}
			else	gr->quad_plot(p3,p4,p1,p2);
		}
	}
}
//-----------------------------------------------------------------------------
// NOTE! All data MUST have the same size! Only first slice is used!
void MGL_EXPORT mgl_axial_gen(HMGL gr, mreal val, HCDT a, HCDT x, HCDT y, mreal c, char dir,long ak,int wire)
{
	long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2 || x->GetNx()*x->GetNy()!=n*m || y->GetNx()*y->GetNy()!=n*m)
	{	gr->SetWarn(mglWarnDim,"ContGen");	return;	}

	mglPoint *kk = new mglPoint[2*n*m],*pp = new mglPoint[2*n*m],p;
	mreal d, kx, ky;
	register long i,j,k, pc=0,i0;
	// Usually number of points is much smaller. So, there is no reservation.
	//	gr->Reserve(2*n*m);

	// add intersection point of isoline and X or Y axis
	const mglData *mx = dynamic_cast<const mglData *>(x);
	const mglData *my = dynamic_cast<const mglData *>(y);
	const mglData *ma = dynamic_cast<const mglData *>(a);
	if(mx&&my&&ma)	for(j=0;j<m;j++)	for(i=0;i<n;i++)
	{
		if(gr->Stop)	{	delete []kk;	delete []pp;	return;	}
		i0 = i+n*j;
		d = (i<n-1)?mgl_d(val,ma->a[i0+n*m*ak],ma->a[i0+1+n*m*ak]):-1;
		if(d>=0 && d<1)
		{
			pp[pc] = mglPoint(mx->a[i0]*(1-d)+mx->a[i0+1]*d, my->a[i0]*(1-d)+my->a[i0+1]*d);
			kk[pc] = mglPoint(i+d,j);	pc++;
		}
		d = (j<m-1)?mgl_d(val,ma->a[i0+n*m*ak],ma->a[i0+n*m*ak+n]):-1;
		if(d>=0 && d<1)
		{
			pp[pc] = mglPoint(mx->a[i0]*(1-d)+mx->a[i0+n]*d, my->a[i0]*(1-d)+my->a[i0+n]*d);
			kk[pc] = mglPoint(i,j+d);	pc++;
		}
	}
	else	for(j=0;j<m;j++)	for(i=0;i<n;i++)
	{
		if(gr->Stop)	{	delete []kk;	delete []pp;	return;	}
		register mreal va=a->v(i,j,ak),vx=x->v(i,j),vy=y->v(i,j);
		d = (i<n-1)?mgl_d(val,va,a->v(i+1,j,ak)):-1;
		if(d>=0 && d<1)
		{
			pp[pc] = mglPoint(vx*(1-d)+x->v(i+1,j)*d, vy*(1-d)+y->v(i+1,j)*d);
			kk[pc] = mglPoint(i+d,j);	pc++;
		}
		d = (j<m-1)?mgl_d(val,va,a->v(i,j+1,ak)):-1;
		if(d>=0 && d<1)
		{
			pp[pc] = mglPoint(vx*(1-d)+x->v(i,j+1)*d, vy*(1-d)+y->v(i,j+1)*d);
			kk[pc] = mglPoint(i,j+d);	pc++;
		}
	}
	// deallocate arrays and finish if no point
	if(pc==0)	{	delete []kk;	delete []pp;	return;	}
	// allocate arrays for curve
	long *nn = new long[pc], *ff = new long[pc];
	for(i=0;i<pc;i++)	nn[i] = ff[i] = -1;
	// connect points to line
	long i11,i12,i21,i22,j11,j12,j21,j22;
	j=-1;	// current point
	do{
		if(gr->Stop)	{	delete []kk;	delete []pp;	delete []nn;	delete []ff;	return;	}
		if(j>=0)
		{
			kx = kk[j].x;	ky = kk[j].y;	i = -1;
			i11 = long(kx+1e-5);	i12 = long(kx-1e-5);
			j11 = long(ky+1e-5);	j12 = long(ky-1e-5);
			for(k=0;k<pc;k++)	// find closest point in grid
			{
				if(k==j || k==ff[j] || ff[k]!=-1)	continue;	// point is marked
				i21 = long(kk[k].x+1e-5);	i22 = long(kk[k].x-1e-5);
				j21 = long(kk[k].y+1e-5);	j22 = long(kk[k].y-1e-5);
				// check if in the same cell
				register bool cond = (i11==i21 || i11==i22 || i12==i21 || i12==i22) &&
				(j11==j21 || j11==j22 || j12==j21 || j12==j22);
				if(cond){	i=k;	break;	}
			}
			if(i<0)	j = -1;	// no free close points
			else			// mark the point
			{	nn[j] = i;	ff[i] = j;	j = nn[i]<0 ? i : -1;	}
		}
		if(j<0)
		{
			for(k=0;k<pc;k++)	if(nn[k]==-1)	// first check edges
			{
				if(kk[k].x==0 || fabs(kk[k].x-n+1)<1e-5 || kk[k].y==0 || fabs(kk[k].y-m+1)<1e-5)
				{	nn[k]=-2;	j = k;	break;	}
			}
			if(j<0)	for(k=0;k<pc;k++)	if(nn[k]==-1)	// or any points inside
			{	j = k;	nn[k]=-2;	break;	}
		}
	}while(j>=0);
	mgl_axial_plot(gr,pc,pp,nn,dir,c,wire);
	delete []kk;	delete []nn;	delete []ff;	delete []pp;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_xy_val(HMGL gr, HCDT v, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	long n=z->GetNx(),m=z->GetNy();
	if(mgl_check_dim2(gr,x,y,z,0,"Axial"))	return;
	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Axial",cgid++);
	long s=gr->AddTexture(sch);
	char dir='y';
	if(mglchr(sch,'x'))	dir = 'x';
	if(mglchr(sch,'z'))	dir = 'z';

	mglData xx, yy;
	if(x->GetNx()*x->GetNy()!=m*n || y->GetNx()*y->GetNy()!=m*n)	// make
	{
		xx.Create(n, m);		yy.Create(n, m);
		const mglData *mx = dynamic_cast<const mglData *>(x);
		const mglData *my = dynamic_cast<const mglData *>(y);
		if(mx && my)
#pragma omp parallel for collapse(2)
			for(long i=0;i<n;i++)	for(long j=0;j<m;j++)
			{	xx.a[i+n*j] = mx->a[i];	yy.a[i+n*j] = my->a[j];	}
		else
#pragma omp parallel for collapse(2)
			for(long i=0;i<n;i++)	for(long j=0;j<m;j++)
			{	xx.a[i+n*j] = x->v(i);	yy.a[i+n*j] = y->v(j);	}
		x = &xx;	y = &yy;
	}
	// x, y -- have the same size z
	int wire = mglchr(sch,'#')?1:0;
	if(mglchr(sch,'.'))	wire = 2;
#pragma omp parallel for collapse(2)
	for(long j=0;j<z->GetNz();j++)	for(long i=0;i<v->GetNx();i++)
	{
		if(gr->Stop)	continue;
		register mreal v0 = v->v(i);
		mgl_axial_gen(gr,v0,z,x,y,gr->GetC(s,v0),dir,j,wire);
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_val(HMGL gr, HCDT v, HCDT a, const char *sch, const char *opt)
{
	register long n=a->GetNx(), m=a->GetNy();
	if(n<2 || m<2)	{	gr->SetWarn(mglWarnLow,"Axial");	return;	}
	gr->SaveState(opt);
	mglData x(n, m), y(n, m);
	if(gr->Max.x*gr->Min.x>=0)	x.Fill(gr->Min.x,gr->Max.x,'x');
	else	x.Fill(0,gr->Max.x,'x');
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_axial_xy_val(gr,v,&x,&y,a,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_xy(HMGL gr, HCDT x, HCDT y, HCDT a, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?3:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Axial");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_axial_xy_val(gr,&v,x,y,a,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial(HMGL gr, HCDT a, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long Num = mgl_isnan(r)?3:long(r+0.5);
	if(Num<1)	{	gr->SetWarn(mglWarnCnt,"Axial");	return;	}
	mglData v(Num);
	for(long i=0;i<Num;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(Num+1);
	mgl_axial_val(gr,&v,a,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_xy_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_axial_xy_val(_GR_, _DA_(v), _DA_(x), _DA_(y), _DA_(a), s, o);
delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_val_(uintptr_t *gr, uintptr_t *v, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_axial_val(_GR_, _DA_(v), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_axial_xy(_GR_, _DA_(x), _DA_(y), _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_axial_(uintptr_t *gr, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_axial(_GR_, _DA_(a), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//		Torus series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_torus(HMGL gr, HCDT r, HCDT z, const char *sch, const char *opt)
{
	long i,j,n=r->GetNx();
	if(n*r->GetNy()!=z->GetNx()*z->GetNy())	{	gr->SetWarn(mglWarnDim,"Torus");	return;	}
	if(n<2)		{	gr->SetWarn(mglWarnLow,"Torus");	return;	}
	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Torus",cgid++);

	mglPoint *pp = new mglPoint[n];
	long *nn = new long[n];
	long ss=gr->AddTexture(sch);
	char dir='y';
	if(mglchr(sch,'x'))	dir = 'x';
	if(mglchr(sch,'z'))	dir = 'z';

	mreal c = gr->GetC(ss,gr->Min.c);
	const mglData *mr = dynamic_cast<const mglData *>(r);
	const mglData *mz = dynamic_cast<const mglData *>(z);
	int wire = mglchr(sch,'#')?1:0;
	if(mglchr(sch,'.'))	wire = 2;
	for(j=0;j<r->GetNy();j++)
	{
		if(mr&&mz)	for(i=0;i<n;i++)
		{
			nn[i] = i<n-1 ? i+1 : -1;
			pp[i] = mglPoint(mr->a[i+n*j], mz->a[i+n*j]);
		}
		else	for(i=0;i<n;i++)
		{
			nn[i] = i<n-1 ? i+1 : -1;
			pp[i] = mglPoint(r->v(i,j), z->v(i,j));
		}
		mgl_axial_plot(gr,n,pp,nn,dir,c,wire);
	}
	gr->EndGroup();
	delete []nn;	delete []pp;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_torus_(uintptr_t *gr, uintptr_t *r, uintptr_t *z, const char *pen, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
mgl_torus(_GR_, _DA_(r), _DA_(z), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
