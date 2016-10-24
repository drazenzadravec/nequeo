/***************************************************************************
 * crust.cpp is part of Math Graphic Library
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
#include <float.h>
#include "mgl2/other.h"
#include "mgl2/data.h"
#include "mgl2/thread.h"
#include "mgl2/base.h"
//-----------------------------------------------------------------------------
//
//	TriPlot series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xyzc(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, const char *opt)
{
	long n = x->GetNx(), m = nums->GetNy();
	if(mgl_check_trig(gr,nums,x,y,z,a,"TriPlot"))	return;

	long ss=gr->AddTexture(sch);
	gr->SaveState(opt);	gr->SetPenPal("-");
	static int cgid=1;	gr->StartGroup("TriPlot",cgid++);
	mglPoint p1,p2,p3,q;

	bool wire = mglchr(sch,'#');
	long nc = a->GetNx();
	if(nc!=n && nc>=m)	// colors per triangle
	{
		gr->Reserve(m*3);
#pragma omp parallel for private(p1,p2,p3,q)
		for(long i=0;i<m;i++)
		{
			if(gr->Stop)	continue;
			register long k1 = long(nums->v(0,i)+0.5);
			p1 = mglPoint(x->v(k1), y->v(k1), z->v(k1));
			register long k2 = long(nums->v(1,i)+0.5);
			p2 = mglPoint(x->v(k2), y->v(k2), z->v(k2));
			register long k3 = long(nums->v(2,i)+0.5);
			p3 = mglPoint(x->v(k3), y->v(k3), z->v(k3));
			q = wire ? mglPoint(NAN,NAN) : (p2-p1) ^ (p3-p1);
			k1 = gr->AddPnt(p1,gr->GetC(ss,a->v(k1)),q);
			k2 = gr->AddPnt(p2,gr->GetC(ss,a->v(k2)),q);
			k3 = gr->AddPnt(p3,gr->GetC(ss,a->v(k3)),q);
			gr->trig_plot(k1,k2,k3);
		}
	}
	else if(nc>=n)		// colors per point
	{
		gr->Reserve(n);
		long *kk = new long[n];
		mglPoint *pp = new mglPoint[n];
#pragma omp parallel for
		for(long i=0;i<m;i++)	// add averaged normales
		{
			if(gr->Stop)	continue;
			register long k1 = long(nums->v(0,i)+0.5);
			register long k2 = long(nums->v(1,i)+0.5);
			register long k3 = long(nums->v(2,i)+0.5);
			if(!wire)
			{
				mglPoint q = mglPoint(x->v(k2)-x->v(k1), y->v(k2)-y->v(k1), z->v(k2)-z->v(k1)) ^
					mglPoint(x->v(k3)-x->v(k1), y->v(k3)-y->v(k1), z->v(k3)-z->v(k1));
				q.Normalize();
				// try be sure that in the same direction ...
				if(q.z<0)	q *= -1;
#pragma omp critical(quadplot)
				{pp[k1] += q;	pp[k2] += q;	pp[k3] += q;}
			}
			else	pp[k1]=pp[k2]=pp[k3]=mglPoint(NAN,NAN);
		}
#pragma omp parallel for
		for(long i=0;i<n;i++)	// add points
		{
			if(gr->Stop)	continue;
			kk[i] = gr->AddPnt(mglPoint(x->v(i), y->v(i), z->v(i)), gr->GetC(ss,a->v(i)), pp[i]);
		}
#pragma omp parallel for
		for(long i=0;i<m;i++)	// draw triangles
		{
			if(gr->Stop)	continue;
			register long k1 = long(nums->v(0,i)+0.5);
			register long k2 = long(nums->v(1,i)+0.5);
			register long k3 = long(nums->v(2,i)+0.5);
			if(wire)
			{
				gr->line_plot(kk[k1],kk[k2]);	gr->line_plot(kk[k1],kk[k3]);
				gr->line_plot(kk[k3],kk[k2]);
			}
			else	gr->trig_plot(kk[k1],kk[k2],kk[k3]);
		}
		delete []kk;	delete []pp;
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xyz(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{	mgl_triplot_xyzc(gr,nums,x,y,z,z,sch,opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xy(HMGL gr, HCDT nums, HCDT x, HCDT y, const char *sch, const char *opt)
{
	gr->SaveState(opt);
	mglData z(x->GetNx());
	mreal zm = gr->AdjustZMin();	z.Fill(zm,zm);
	mgl_triplot_xyzc(gr,nums,x,y,&z,&z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xyzc_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *c, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_triplot_xyzc(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), _DA_(c), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xyz_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_triplot_xyz(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_triplot_xy_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_triplot_xy(_GR_, _DA_(nums), _DA_(x), _DA_(y), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	QuadPlot series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xyzc(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, const char *opt)
{
	long n = x->GetNx(), m = nums->GetNy();
	if(mgl_check_trig(gr,nums,x,y,z,a,"QuadPlot",4))	return;

	long ss=gr->AddTexture(sch);
	gr->SaveState(opt);	gr->SetPenPal("-");
	static int cgid=1;	gr->StartGroup("QuadPlot",cgid++);
	mglPoint p1,p2,p3,p4;

	long nc = a->GetNx();
	bool wire = mglchr(sch,'#');
	if(nc!=n && nc>=m)	// colors per triangle
	{
		gr->Reserve(m*4);
#pragma omp parallel for private(p1,p2,p3,p4)
		for(long i=0;i<m;i++)
		{
			if(gr->Stop)	continue;
			register long k1 = long(nums->v(0,i)+0.5);
			p1 = mglPoint(x->v(k1), y->v(k1), z->v(k1));
			register long k2 = long(nums->v(1,i)+0.5);
			p2 = mglPoint(x->v(k2), y->v(k2), z->v(k2));
			register long k3 = long(nums->v(2,i)+0.5);
			p3 = mglPoint(x->v(k3), y->v(k3), z->v(k3));
			register long k4 = floor(nums->v(3,i)+0.5);
			p4 = mglPoint(x->v(k4), y->v(k4), z->v(k4));
			mglPoint q = wire ? mglPoint(NAN,NAN):(p2-p1) ^ (p3-p1);
			k1 = gr->AddPnt(p1,gr->GetC(ss,a->v(k1)),q);
			k2 = gr->AddPnt(p2,gr->GetC(ss,a->v(k2)),q);
			k3 = gr->AddPnt(p3,gr->GetC(ss,a->v(k3)),q);
			k4 = gr->AddPnt(p4,gr->GetC(ss,a->v(k4)),q);
			gr->quad_plot(k1,k2,k3,k4);
		}
	}
	else if(nc>=n)		// colors per point
	{
		gr->Reserve(n);
		long *kk = new long[n];
		mglPoint *pp = new mglPoint[n];
#pragma omp parallel for private(p1,p2,p3,p4)
		for(long i=0;i<m;i++)	// add averaged normales
		{
			if(gr->Stop)	continue;
			register long k1 = long(nums->v(0,i)+0.5);
			p1 = mglPoint(x->v(k1), y->v(k1), z->v(k1));
			register long k2 = long(nums->v(1,i)+0.5);
			p2 = mglPoint(x->v(k2), y->v(k2), z->v(k2));
			register long k3 = long(nums->v(2,i)+0.5);
			p3 = mglPoint(x->v(k3), y->v(k3), z->v(k3));
			register long k4 = floor(nums->v(3,i)+0.5);
			p4 = mglPoint(x->v(k4), y->v(k4), z->v(k4));

			if(wire)	pp[k1]=pp[k2]=pp[k3]=pp[k4]=mglPoint(NAN,NAN);
			else
			{
				mglPoint q1 = (p2-p1) ^ (p3-p1);	if(q1.z<0) q1*=-1;
				mglPoint q2 = (p2-p4) ^ (p3-p4);	if(q2.z<0) q2*=-1;
				mglPoint q3 = (p1-p2) ^ (p4-p2);	if(q3.z<0) q3*=-1;
				mglPoint q4 = (p1-p4) ^ (p4-p3);	if(q4.z<0) q4*=-1;
#pragma omp critical(quadplot)
				{pp[k1] += q1;	pp[k2] += q2;	pp[k3] += q3;	pp[k4] += q4;}
			}
		}
#pragma omp parallel for
		for(long i=0;i<n;i++)	// add points
		{
			if(gr->Stop)	continue;
			kk[i] = gr->AddPnt(mglPoint(x->v(i), y->v(i), z->v(i)),gr->GetC(ss,a->v(i)), pp[i]);
		}
#pragma omp parallel for
		for(long i=0;i<m;i++)	// draw quads
		{
			if(gr->Stop)	continue;
			register long k1 = floor(nums->v(0,i)+0.5);
			register long k2 = floor(nums->v(1,i)+0.5);
			register long k3 = floor(nums->v(2,i)+0.5);
			register long k4 = floor(nums->v(3,i)+0.5);
			if(wire)
			{
				gr->line_plot(kk[k1],kk[k2]);	gr->line_plot(kk[k1],kk[k3]);
				gr->line_plot(kk[k4],kk[k2]);	gr->line_plot(kk[k4],kk[k3]);
			}
			else	gr->quad_plot(kk[k1],kk[k2],kk[k3],kk[k4]);
		}
		delete []kk;	delete []pp;
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xyz(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{	mgl_quadplot_xyzc(gr,nums,x,y,z,z,sch,opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xy(HMGL gr, HCDT nums, HCDT x, HCDT y, const char *sch, const char *opt)
{
	gr->SaveState(opt);
	mglData z(x->GetNx());	z.Fill(gr->Min.z,gr->Min.z);
	mgl_quadplot_xyzc(gr,nums,x,y,&z,&z,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xyzc_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *c, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_quadplot_xyzc(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), _DA_(c), s, o);
	delete []o;	delete []s;}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xyz_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_quadplot_xyzc(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), _DA_(z), s, o);
	delete []o;	delete []s;}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_quadplot_xy_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_quadplot_xy(_GR_, _DA_(nums), _DA_(x), _DA_(y), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	TriCont series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyzcv(HMGL gr, HCDT v, HCDT nums, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, const char *opt)
{
	long n = x->GetNx(), m = nums->GetNy();
	if(mgl_check_trig(gr,nums,x,y,z,a,"TriCont"))	return;

	long ss=gr->AddTexture(sch);
	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("TriCont",cgid++);
	bool zVal = !(mglchr(sch,'_'));
	mglPoint p1,p2,p3;
#pragma omp parallel for private(p1,p2,p3) collapse(2)
	for(long k=0;k<v->GetNx();k++)	for(long i=0;i<m;i++)
	{
		if(gr->Stop)	continue;
		register long k1 = long(nums->v(0,i)+0.1);	if(k1<0 || k1>=n)	continue;
		register long k2 = long(nums->v(1,i)+0.1);	if(k2<0 || k2>=n)	continue;
		register long k3 = long(nums->v(2,i)+0.1);	if(k3<0 || k3>=n)	continue;
		register mreal val = v->v(k), c = gr->GetC(ss,val), d1,d2,d3;

		d1 = mgl_d(val,a->v(k1),a->v(k2));
		p1 = mglPoint(x->v(k1)*(1-d1)+x->v(k2)*d1, y->v(k1)*(1-d1)+y->v(k2)*d1,
					  zVal?z->v(k1)*(1-d1)+z->v(k2)*d1:gr->Min.z);
		d2 = mgl_d(val,a->v(k1),a->v(k3));
		p2 = mglPoint(x->v(k1)*(1-d2)+x->v(k3)*d2, y->v(k1)*(1-d2)+y->v(k3)*d2,
					  zVal?z->v(k1)*(1-d2)+z->v(k3)*d2:gr->Min.z);
		d3 = mgl_d(val,a->v(k2),a->v(k3));
		p3 = mglPoint(x->v(k2)*(1-d3)+x->v(k3)*d3, y->v(k2)*(1-d3)+y->v(k3)*d3,
					  zVal?z->v(k2)*(1-d3)+z->v(k3)*d3:gr->Min.z);
		if(d1>=0 && d1<=1 && d2>=0 && d2<=1)
		{
			k1 = gr->AddPnt(p1,c);	k2 = gr->AddPnt(p2,c);
			gr->line_plot(k1,k2);
		}
		else if(d1>=0 && d1<=1 && d3>=0 && d3<=1)
		{
			k1 = gr->AddPnt(p1,c);	k2 = gr->AddPnt(p3,c);
			gr->line_plot(k1,k2);
		}
		else if(d3>=0 && d3<=1 && d2>=0 && d2<=1)
		{
			k1 = gr->AddPnt(p3,c);	k2 = gr->AddPnt(p2,c);
			gr->line_plot(k1,k2);
		}
	}
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyzc(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, const char *opt)
{
	mreal r = gr->SaveState(opt);
	long n = (mgl_isnan(r) || r<=0) ? 7:long(r+0.5);
	mglData v(n);
	for(long i=0;i<n;i++)	v.a[i] = gr->Min.c + (gr->Max.c-gr->Min.c)*mreal(i+1)/(n+1);
	mgl_tricont_xyzcv(gr,&v,nums,x,y,z,a,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyc(HMGL gr, HCDT nums, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{	mgl_tricont_xyzc(gr,nums,x,y,z,z,sch,opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xycv(HMGL gr, HCDT v, HCDT nums, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{	mgl_tricont_xyzcv(gr,v,nums,x,y,z,z,sch,opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyzcv_(uintptr_t *gr, uintptr_t *v, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *c, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_tricont_xyzcv(_GR_, _DA_(v), _DA_(nums), _DA_(x), _DA_(y), _DA_(z), _DA_(c), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xycv_(uintptr_t *gr, uintptr_t *v, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_tricont_xycv(_GR_, _DA_(v), _DA_(nums), _DA_(x), _DA_(y), _DA_(z), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyzc_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *c, const char *sch, const char *opt, int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_tricont_xyzc(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), _DA_(c), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tricont_xyc_(uintptr_t *gr, uintptr_t *nums, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt, int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_tricont_xyc(_GR_, _DA_(nums), _DA_(x), _DA_(y), _DA_(z), s, o);
	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Dots series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots_ca(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT c, HCDT a, const char *sch, const char *opt)
{
	long n = x->GetNN(), d, k=1;
	if(x->GetNz()>1) 	k=3;		else if(x->GetNy()>1)	k=2;

	if(y->GetNN()!=n || z->GetNN()!=n || c->GetNN()!=n || (a && a->GetNN()!=n))
	{	gr->SetWarn(mglWarnDim,"Dots");	return;	}
	gr->SaveState(opt);

	d = gr->MeshNum>0 ? mgl_ipow(gr->MeshNum+1,k) : n;
	d = n>d ? n/d:1;

	static int cgid=1;	gr->StartGroup("Dots",cgid++);
	char mk=gr->SetPenPal(sch);
	long ss=gr->AddTexture(sch);
	if(mk==0)	mk='.';
	gr->Reserve(n);

#pragma omp parallel for
	for(long i=0;i<n;i+=d)
	{
		if(gr->Stop)	continue;
		mglPoint p = mglPoint(x->vthr(i),y->vthr(i),z->vthr(i));
		long pp = gr->AddPnt(p,gr->GetC(ss,c->vthr(i)),mglPoint(NAN),a?gr->GetA(a->vthr(i)):-1);
		gr->mark_plot(pp, mk);
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots_a(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT a, const char *sch, const char *opt)
{	mgl_dots_ca(gr, x, y, z, z, a, sch, opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{	mgl_dots_ca(gr, x, y, z, z, NULL, sch, opt);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_dots(_GR_, _DA_(x),_DA_(y),_DA_(z),s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots_a_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_dots_a(_GR_, _DA_(x),_DA_(y),_DA_(z),_DA_(a),s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dots_ca_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *c, uintptr_t *a, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_dots_ca(_GR_, _DA_(x),_DA_(y),_DA_(z),_DA_(c),_DA_(a),s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	mglTriangulation
//
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_crust(long n,mglPoint *pp,long **nn,mreal ff);
HMDT MGL_EXPORT mgl_triangulation_3d(HCDT x, HCDT y, HCDT z)
{	// TODO: should be used s-hull or q-hull
	mglData *nums=0;
	long n = x->GetNx(), m;
	if(y->GetNx()!=n || z->GetNx()!=n)	return nums;
	mglPoint *pp = new mglPoint[n];
	long *nn=0;
#pragma omp parallel for
	for(long i=0;i<n;i++)	pp[i] = mglPoint(x->v(i), y->v(i), z->v(i));
	m = mgl_crust(n,pp,&nn,0);

	if(m>0)
	{
		nums=new mglData(3,m);
#pragma omp parallel for
		for(long i=0;i<3*m;i++)	nums->a[i]=nn[i];
	}
	delete []pp;	free(nn);	return nums;
}
//-----------------------------------------------------------------------------
#include "s_hull/s_hull_pro.h"
HMDT MGL_EXPORT mgl_triangulation_2d(HCDT x, HCDT y)
{
	mglData *nums=0;
	long n = x->GetNN();
	if(y->GetNN()!=n)	return nums;
	// use s-hull here
	std::vector<Shx> pts;
	std::vector<size_t> out;
	Shx pt;

	for(long i=0;i<n;i++)
	{	pt.r = x->vthr(i);	pt.c = y->vthr(i);	pt.id = i;	pts.push_back(pt);	}
	std::vector<Triad> triads;
	if(de_duplicate(pts, out))
		mglGlobalMess += "There are duplicated points for triangulation.\n";
	s_hull_pro(pts, triads);
	long m = triads.size();
	nums=new mglData(3,m);
#pragma omp parallel for
	for(long i=0;i<m;i++)
	{
		nums->a[3*i]   = triads[i].a;
		nums->a[3*i+1] = triads[i].b;
		nums->a[3*i+2] = triads[i].c;
	}
	return nums;
}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_triangulation_3d_(uintptr_t *x, uintptr_t *y, uintptr_t *z)
{	return uintptr_t(mgl_triangulation_3d(_DA_(x),_DA_(y),_DA_(z)));	}
uintptr_t MGL_EXPORT mgl_triangulation_2d_(uintptr_t *x, uintptr_t *y)
{	return uintptr_t(mgl_triangulation_2d(_DA_(x),_DA_(y)));	}
//-----------------------------------------------------------------------------
//
//	DataGrid
//
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_grid_t(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0],ny=t->p[1];
	mreal *b=t->a;
	const mreal *x=t->b, *y=t->c, *d=t->d, *z=t->e;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{	// TODO check if rounding needed
		register long k1 = long(d[3*i0]), k2 = long(d[3*i0+1]), k3 = long(d[3*i0+2]);
		mreal dxu,dxv,dyu,dyv;
		mglPoint d1=mglPoint(x[k2]-x[k1],y[k2]-y[k1],z[k2]-z[k1]), d2=mglPoint(x[k3]-x[k1],y[k3]-y[k1],z[k3]-z[k1]), p;

		dxu = d2.x*d1.y - d1.x*d2.y;
		if(fabs(dxu)<1e-5) continue; // points lies on the same line
		dyv =-d1.x/dxu; dxv = d1.y/dxu;
		dyu = d2.x/dxu; dxu =-d2.y/dxu;

		long x1,y1,x2,y2;
		x1 = long(fmin(fmin(x[k1],x[k2]),x[k3])); // bounding box
		y1 = long(fmin(fmin(y[k1],y[k2]),y[k3]));
		x2 = long(fmax(fmax(x[k1],x[k2]),x[k3]));
		y2 = long(fmax(fmax(y[k1],y[k2]),y[k3]));
		x1 = x1>0 ? x1:0; x2 = x2<nx ? x2:nx-1;
		y1 = y1>0 ? y1:0; y2 = y2<ny ? y2:ny-1;
		if((x1>x2) | (y1>y2)) continue;

		register mreal u,v,xx,yy, x0 = x[k1], y0 = y[k1];
		register long i,j;
		for(i=x1;i<=x2;i++) for(j=y1;j<=y2;j++)
		{
			xx = (i-x0); yy = (j-y0);
			u = dxu*xx+dyu*yy; v = dxv*xx+dyv*yy;
			if((u<0) | (v<0) | (u+v>1)) continue;
			b[i+nx*j] = z[k1] + d1.z*u + d2.z*v;
		}
	}
	return 0;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_grid_xy(HMDT d, HCDT xdat, HCDT ydat, HCDT zdat, mreal x1, mreal x2, mreal y1, mreal y2)
{ // NOTE: only for mglData
	const mglData *x = dynamic_cast<const mglData *>(xdat);
	const mglData *y = dynamic_cast<const mglData *>(ydat);
	const mglData *z = dynamic_cast<const mglData *>(zdat);
	if(!x || !y || !z) return;
	long n=x->GetNN();
	if((n<3) || (y->GetNN()!=n) || (z->GetNN()!=n))	return;

	mglData *nums = mgl_triangulation_2d(x,y);
	if(nums->nx<3)	{	delete nums;	return;	}
	long nn = nums->ny, par[3]={d->nx,d->ny,d->nz};
	mreal xx[4]={x1,0, y1,0};
	if(d->nx>1) xx[1] = (d->nx-1.)/(x2-x1);
	if(d->ny>1) xx[3] = (d->ny-1.)/(y2-y1);

	mreal *xc=new mreal[n], *yc=new mreal[n];
#pragma omp parallel for
	for(long i=0;i<n;i++)	{	xc[i]=xx[1]*(x->a[i]-xx[0]);	yc[i]=xx[3]*(y->a[i]-xx[2]);	}
#pragma omp parallel for
	for(long i=0;i<d->nx*d->ny*d->nz;i++) d->a[i] = NAN;

	mglStartThread(mgl_grid_t,0,nn,d->a,xc,yc,par,0,nums->a,z->a);
	delete nums;	delete []xc;	delete []yc;
}
void MGL_EXPORT mgl_data_grid_xy_(uintptr_t *d, uintptr_t *x, uintptr_t *y, uintptr_t *z, mreal *x1, mreal *x2, mreal *y1, mreal *y2)
{	mgl_data_grid_xy(_DT_,_DA_(x),_DA_(y),_DA_(z),*x1,*x2,*y1,*y2);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_grid(HMGL gr, HMDT d, HCDT xdat, HCDT ydat, HCDT zdat, const char *opt)
{
	gr->SaveState(opt);
	mgl_data_grid_xy(d,xdat,ydat,zdat,gr->Min.x,gr->Max.x,gr->Min.y,gr->Max.y);
	gr->LoadState();
}
void MGL_EXPORT mgl_data_grid_(uintptr_t *gr, uintptr_t *d, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *opt,int lo)
{	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_data_grid(_GR_,_DT_,_DA_(x),_DA_(y),_DA_(z),o);	delete []o;	}
//-----------------------------------------------------------------------------
//
//	Crust series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_crust(HMGL gr, HCDT x, HCDT y, HCDT z, const char *sch, const char *opt)
{
	if(y->GetNx()!=x->GetNx() || z->GetNx()!=x->GetNx())
	{	gr->SetWarn(mglWarnDim,"Crust");	return;	}
	HMDT nums = mgl_triangulation_3d(x, y, z);
	mgl_triplot_xyzc(gr,nums,x,y,z,z,sch,opt);
	mgl_delete_data(nums);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_crust_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_crust(_GR_, _DA_(x),_DA_(y),_DA_(z),s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_insert_trig(long i1,long i2,long i3,long **n)
{
	static long Cur=0,Max=0;
	if(i1<0 || i2<0 || i3<0)	return Cur;
	if(*n==0)
	{
		Max = 1024;		Cur = 0;
		*n = (long *)malloc(Max*3*sizeof(long));
		memset(*n,0,Max*3*sizeof(long));
	}
	if(Cur>=Max)
	{
		Max += 1024;
		*n = (long *)realloc(*n,Max*3*sizeof(long));
		memset(*n+3*(Max-1024),0,3*1024*sizeof(long));
	}
	long *nn;
	register long i,k1;
	if(i1>i3)	{	k1=i1;	i1=i3;	i3=k1;	}	// simple sorting
	if(i1>i2)	{	k1=i1;	i1=i2;	i2=k1;	}
	if(i2>i3)	{	k1=i2;	i2=i3;	i3=k1;	}
	for(i=0;i<Cur;i++)	// check if it is unique
	{
		nn = *n + 3*i;
		if(nn[0]==i1 && nn[1]==i2 && nn[2]==i3)	return Cur;
	}
	nn = *n + 3*Cur;
	nn[0]=i1;	nn[1]=i2;	nn[2]=i3;
	Cur++;	return Cur;
}
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_get_next(long k1,long n,long *,long *set,mglPoint *qq)
{
	long i,j=-1;
	mreal r,rm=FLT_MAX;
	for(i=0;i<n;i++)
	{
		if(i==k1 || set[i]>0)	continue;
		r = mgl_norm(qq[i]-qq[k1]);
		if(r<rm)	{	rm=r;	j=i;	}
	}
	return j;
}
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_crust(long n,mglPoint *pp,long **nn,mreal ff)
{	// TODO: update to normal algorithm
	register long i,j;
	register mreal r,rm,rs;
	if(ff<=0)	ff=2;
	for(rs=0,i=0;i<n;i++)
	{
		for(rm = FLT_MAX,j=0;j<n;j++)
		{
			if(i==j)	continue;
			r = mgl_norm(pp[i]-pp[j]);
			if(rm>r)	rm = r;
		}
		rs += sqrt(rm);
	}
	rs *= ff/n;	rs = rs*rs;		// "average" distance
	long ind[100], set[100], ii;	// indexes of "close" points, flag that it was added and its number
	mglPoint qq[100];	// normalized point coordinates
	long k1,k2,k3,m=0;
	for(i=0;i<n;i++)	// now the triangles will be found
	{
		memset(set,0,100*sizeof(long));
		for(ii=0,j=0;j<n;j++)	// find close vertexes
		{
			r = mgl_norm(pp[i]-pp[j]);
			if(r<=rs && j!=i)	{	ind[ii] = j;	ii++;	if(ii==99)	break;}
		}
		if(ii<3)	continue;	// nothing to do
		for(j=0;j<ii;j++)
		{
			k1 = j;	k2 = ind[j];	k3 = i;
			qq[k1] = pp[k2] - pp[k3];	r = qq[k1].norm();
			qq[k1] /= r;
		}
		k1 = 0;
		while((k2=mgl_get_next(k1,ii,ind,set,qq))>0)
		{
			set[k1]=1;
			mgl_insert_trig(i,ind[k1],ind[k2],nn);
			k1 = k2;
		}
		m = mgl_insert_trig(i,ind[k1],ind[0],nn);
	}
	return m;
}
//-----------------------------------------------------------------------------
