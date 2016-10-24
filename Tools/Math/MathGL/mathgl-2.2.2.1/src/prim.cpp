/***************************************************************************
 * prim.cpp is part of Math Graphic Library
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
#include "mgl2/canvas.h"
#include "mgl2/prim.h"
#include "mgl2/data.h"
//-----------------------------------------------------------------------------
//
//	Mark & Curve series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_mark(HMGL gr, double x, double y, double z,const char *mark)
{
	char mk = gr->SetPenPal(mark);
	if(!mk)	mk = '.';
	if(mgl_isnan(z))	z=2*gr->Max.z-gr->Min.z;
	static int cgid=1;	gr->StartGroup("MarkS",cgid++);
	long k = gr->AddPnt(mglPoint(x,y,z),gr->CDef,mglPoint(NAN),-1,3);
	gr->mark_plot(k,mk,gr->GetPenWidth()); 	gr->AddActive(k);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_mark_(uintptr_t *gr, mreal *x, mreal *y, mreal *z, const char *pen,int l)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	mgl_mark(_GR_, *x,*y,*z,s);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_ball(HMGL gr, double x, double y, double z)
{
	static int cgid=1;	gr->StartGroup("Ball",cgid++);
	if(mgl_isnan(z))	z=2*gr->Max.z-gr->Min.z;
	long k = gr->AddPnt(mglPoint(x,y,z),gr->AddTexture('r'),mglPoint(NAN),-1,3);
	gr->mark_plot(k,'.');	gr->AddActive(k);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_ball_(uintptr_t *gr, mreal *x, mreal *y, mreal *z)
{	mgl_ball(_GR_, *x,*y,*z);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_line(HMGL gr, double x1, double y1, double z1, double x2, double y2, double z2, const char *pen,int n)
{
	static int cgid=1;	gr->StartGroup("Line",cgid++);
	if(mgl_isnan(z1) || mgl_isnan(z2))	z1=z2=2*gr->Max.z-gr->Min.z;
	mglPoint p1(x1,y1,z1), p2(x2,y2,z2), p=p1,nn=mglPoint(NAN);
	gr->SetPenPal(pen);
	n = (n<2) ? 2 : n;

	register long i,k1,k2;
	register mreal s;
	gr->Reserve(n);
	k1 = gr->AddPnt(p,gr->CDef,nn,-1,3);	gr->AddActive(k1);
	for(i=1;i<n;i++)
	{
		if(gr->Stop)	return;
		s = i/mreal(n-1);	p = p1*(1-s)+p2*s;	k2 = k1;
		k1 = gr->AddPnt(p,gr->CDef,nn,-1,3);
		gr->line_plot(k2,k1);
		if(i==1)	gr->arrow_plot(k2,k1,gr->Arrow1);
		if(i==n-1)	gr->arrow_plot(k1,k2,gr->Arrow2);
	}
	gr->AddActive(k1,1);	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_line_(uintptr_t *gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, const char *pen,int *n,int l)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	mgl_line(_GR_, *x1,*y1,*z1, *x2,*y2,*z2,s,*n);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_curve(HMGL gr, double x1, double y1, double z1, double dx1, double dy1, double dz1, double x2, double y2, double z2, double dx2, double dy2, double dz2, const char *pen,int n)
{
	static int cgid=1;	gr->StartGroup("Curve",cgid++);
	if(mgl_isnan(z1) || mgl_isnan(z2))	z1=z2=2*gr->Max.z-gr->Min.z;
	mglPoint p1(x1,y1,z1), p2(x2,y2,z2), d1(dx1,dy1,dz1), d2(dx2,dy2,dz2), a,b,p=p1,nn=mglPoint(NAN);
	a = 3*(p2-p1)-d2-2*d1;	b = d1+d2-2*(p2-p1);
	n = (n<2) ? 2 : n;
	gr->SetPenPal(pen);

	register long i,k1,k2;
	register mreal s;
	gr->Reserve(n);
	k1=gr->AddPnt(p,gr->CDef,nn,-1,3);	gr->AddActive(k1);
	for(i=1;i<n;i++)
	{
		if(gr->Stop)	return;
		s = i/(n-1.);	p = p1+s*(d1+s*(a+s*b));	k2 = k1;
		k1 = gr->AddPnt(p,gr->CDef,nn,-1,3);
		gr->line_plot(k2,k1);
		if(i==1)	gr->arrow_plot(k2,k1,gr->Arrow1);
		if(i==n-1)	gr->arrow_plot(k1,k2,gr->Arrow2);
	}
	gr->AddActive(gr->AddPnt(p1+d1,gr->CDef,nn,-1,3),1);
	gr->AddActive(gr->AddPnt(p2-d2,gr->CDef,nn,-1,3),3);
	gr->AddActive(k1,2);	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_curve_(uintptr_t* gr, mreal *x1, mreal *y1, mreal *z1, mreal *dx1, mreal *dy1, mreal *dz1, mreal *x2, mreal *y2, mreal *z2, mreal *dx2, mreal *dy2, mreal *dz2, const char *pen,int *n, int l)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	mgl_curve(_GR_, *x1,*y1,*z1, *dx1,*dy1,*dz1, *x2,*y2,*z2, *dx2,*dy2,*dz2, s, *n);	delete []s;}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_error_box(HMGL gr, double x, double y, double z, double ex, double ey, double ez, const char *pen)
{
	static int cgid=1;	gr->StartGroup("ErBox",cgid++);
	char mk=gr->SetPenPal(pen);
	mglPoint p(x,y,z), q,nn=mglPoint(NAN);
	gr->Reserve(7);
	long k1,k2;
	q = p;	q.x += ex;	k1 = gr->AddPnt(q,gr->CDef,nn,0,3);
	q = p;	q.x -= ex;	k2 = gr->AddPnt(q,gr->CDef,nn,0,3);
	gr->line_plot(k1,k2);	gr->arrow_plot(k1,k2,'I');	gr->arrow_plot(k2,k1,'I');
	q = p;	q.y += ey;	k1 = gr->AddPnt(q,gr->CDef,nn,0,3);
	q = p;	q.y -= ey;	k2 = gr->AddPnt(q,gr->CDef,nn,0,3);
	gr->line_plot(k1,k2);	gr->arrow_plot(k1,k2,'I');	gr->arrow_plot(k2,k1,'I');
	q = p;	q.z += ez;	k1 = gr->AddPnt(q,gr->CDef,nn,0,3);
	q = p;	q.z -= ez;	k2 = gr->AddPnt(q,gr->CDef,nn,0,3);
	gr->line_plot(k1,k2);	gr->arrow_plot(k1,k2,'I');	gr->arrow_plot(k2,k1,'I');
	if(mk)	gr->mark_plot(gr->AddPnt(p,gr->CDef,nn,0,3),mk);
	gr->AddActive(gr->AddPnt(p,gr->CDef,nn,-1,3),0);
	gr->AddActive(gr->AddPnt(p+mglPoint(ex,ey),gr->CDef,nn,-1,3),1);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_error_box_(uintptr_t *gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, const char *pen,int l)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	mgl_error_box(_GR_, *x1,*y1,*z1, *x2,*y2,*z2,s);	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Face series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_face(HMGL gr, double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, const char *stl)
{
	static int cgid=1;	gr->StartGroup("Face",cgid++);
	long pal;
	gr->SetPenPal(stl,&pal);
//	mreal c1,c2,c3,c4,zz=(gr->Min.z+gr->Max.z)/2;
	mreal c1,c2,c3,c4,zz=2*gr->Max.z-gr->Min.z;
	c1=c2=c3=c4=gr->CDef;
	if(mgl_isnan(z0))	z0 = zz;	if(mgl_isnan(z1))	z1 = zz;
	if(mgl_isnan(z2))	z2 = zz;	if(mgl_isnan(z3))	z3 = zz;
	mglPoint p1(x0,y0,z0), p2(x1,y1,z1), p3(x2,y2,z2), p4(x3,y3,z3);
	if(gr->GetNumPal(pal)>=4)
	{	c2=gr->NextColor(pal,1);	c3=gr->NextColor(pal,2);	c4=gr->NextColor(pal,3);	}
	mglPoint q1,q2,q3,q4;
	q1 = (p2-p1)^(p3-p1);	q4 = (p2-p4)^(p3-p4);
	q2 = (p1-p2)^(p4-p2);	q3 = (p1-p3)^(p4-p3);
	gr->Reserve(4);
	long k1,k2,k3,k4;
	double a = gr->get(MGL_ENABLE_ALPHA)?-1:1;
	k1 = gr->AddPnt(p1,c1,q1,a,11);	gr->AddActive(k1,0);
	k2 = gr->AddPnt(p2,c2,q2,a,11);	gr->AddActive(k2,1);
	k3 = gr->AddPnt(p3,c3,q3,a,11);	gr->AddActive(k3,2);
	k4 = gr->AddPnt(p4,c4,q4,a,11);	gr->AddActive(k4,3);
	gr->quad_plot(k1,k2,k3,k4);
	if(mglchr(stl,'#'))
	{
		gr->Reserve(4);
		pal = gr->AddTexture('k');
		k1=gr->CopyNtoC(k1,pal);	k2=gr->CopyNtoC(k2,pal);
		k3=gr->CopyNtoC(k3,pal);	k4=gr->CopyNtoC(k4,pal);
		gr->line_plot(k1,k2);		gr->line_plot(k1,k3);
		gr->line_plot(k3,k4);		gr->line_plot(k2,k4);
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facex(HMGL gr, double x0, double y0, double z0, double wy, double wz, const char *stl, double d1, double d2)
{	mgl_face(gr, x0,y0,z0, x0,y0+wy,z0, x0,y0,z0+wz, x0,y0+wy+d1,z0+wz+d2, stl);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facey(HMGL gr, double x0, double y0, double z0, double wx, double wz, const char *stl, double d1, double d2)
{	mgl_face(gr, x0,y0,z0, x0+wx,y0,z0, x0,y0,z0+wz, x0+wx+d1,y0,z0+wz+d2, stl);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facez(HMGL gr, double x0, double y0, double z0, double wx, double wy, const char *stl, double d1, double d2)
{	mgl_face(gr, x0,y0,z0, x0,y0+wy,z0, x0+wx,y0,z0, x0+wx+d1,y0+wy+d2,z0, stl);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facex_(uintptr_t* gr, mreal *x0, mreal *y0, mreal *z0, mreal *wy, mreal *wz, const char *stl, mreal *dx, mreal *dy, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_facex(_GR_, *x0,*y0,*z0,*wy,*wz,s,*dx,*dy);	delete []s;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facey_(uintptr_t* gr, mreal *x0, mreal *y0, mreal *z0, mreal *wx, mreal *wz, const char *stl, mreal *dx, mreal *dy, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_facey(_GR_, *x0,*y0,*z0,*wx,*wz,s,*dx,*dy);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_facez_(uintptr_t* gr, mreal *x0, mreal *y0, mreal *z0, mreal *wx, mreal *wy, const char *stl, mreal *dx, mreal *dy, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_facez(_GR_, *x0,*y0,*z0,*wx,*wy,s,*dx,*dy);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_face_(uintptr_t* gr, mreal *x0, mreal *y0, mreal *z0, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, mreal *x3, mreal *y3, mreal *z3, const char *stl, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_face(_GR_, *x0,*y0,*z0, *x1,*y1,*z1, *x2,*y2,*z2, *x3,*y3,*z3, stl);	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Cone
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cone(HMGL gr, double x1, double y1, double z1, double x2, double y2, double z2, double r1, double r2, const char *stl)
{
	if(r2<0)	r2=r1;
	if(r1==0 && r2==0)	return;

	static int cgid=1;	gr->StartGroup("Cone",cgid++);
	mglPoint p1(x1,y1,z1), p2(x2,y2,z2), p,q(NAN,NAN),t(NAN,NAN), d=p2-p1,a,b;
	a=!d;	a.Normalize();		b=d^a;	b.Normalize();
	long ss=gr->AddTexture(stl);
	mreal c1=gr->GetC(ss,p1.z), c2=gr->GetC(ss,p2.z), dr=r2-r1;
	long *kk=new long[164],k1=-1,k2=-1;
	bool edge = mglchr(stl,'@');
	bool wire = mglchr(stl,'#');
	gr->Reserve(edge?166:82);
	if(edge && !wire)
	{
		k1=gr->AddPnt(p1,c1,d,-1,3);
		k2=gr->AddPnt(p2,c2,d,-1,3);
	}
	long n=wire?6:18;	//wire?6:18;
	if(mglchr(stl,'4'))	n=2;
	else if(mglchr(stl,'6'))	n=3;
	else if(mglchr(stl,'8'))	n=4;
	bool refr = n>6;
	if(refr)	t=d;

#pragma omp parallel for firstprivate(p,q)
	for(long i=0;i<2*n+1;i++)
	{
		if(gr->Stop)	continue;
		register int f = n!=4?(2*i+1)*90/n:45*i;
		register mreal co = mgl_cos[f%360], si = mgl_cos[(f+270)%360];
		p = p1+(r1*co)*a+(r1*si)*b;
		if(refr)	q = (si*a-co*b)^(d + (dr*co)*a + (dr*si)*b);
		kk[i] = gr->AddPnt(p,c1,q,-1,3);
		if(edge && !wire)	kk[i+82] = gr->AddPnt(p,c1,t,-1,3);
		p = p2+(r2*co)*a+(r2*si)*b;
		kk[i+2*n+1] = gr->AddPnt(p,c2,q,-1,3);
		if(edge && !wire)	kk[i+123] = gr->AddPnt(p,c2,t,-1,3);
	}
	if(wire)
//#pragma omp parallel for		// useless
		for(long i=0;i<2*n;i++)
		{
			gr->line_plot(kk[i],kk[i+1]);
			gr->line_plot(kk[i],kk[i+2*n+1]);
			gr->line_plot(kk[i+2*n+2],kk[i+1]);
			gr->line_plot(kk[i+2*n+2],kk[i+2*n+1]);
		}
	else
#pragma omp parallel for
		for(long i=0;i<2*n;i++)
		{
			gr->quad_plot(kk[i],kk[i+1],kk[i+2*n+1],kk[i+2*n+2]);
			if(edge)
			{
				gr->trig_plot(k1,kk[i+82],kk[i+83]);
				gr->trig_plot(k2,kk[i+123],kk[i+124]);
			}
		}
	gr->EndGroup();	delete []kk;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cone_(uintptr_t* gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, mreal *r1, mreal *r2, const char *stl, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_cone(_GR_, *x1,*y1,*z1, *x2,*y2,*z2,*r1,*r2,s);	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Bars series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, const char *pen, const char *opt)
{
	long i=5,j,m,mx,my,mz,n=z->GetNx(),nx=x->GetNx(), nz=z->GetNy(), pal;
	if(mgl_check_dim1(gr,x,z,y,0,"Cones",true))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Cones",cgid++);
	m = x->GetNy() > y->GetNy() ? x->GetNy() : y->GetNy();	m = nz > m ? nz : m;

	bool above= mglchr(pen,'a');
	bool wire = mglchr(pen,'#');
	bool tube = mglchr(pen,'t');
	mreal *dd=new mreal[2*n], x1,z0,zz,d, vx,vy,vz,v0,v1,dv=nx>n?1:0;
	if(mglchr(pen,'<'))	dv = 1;
	if(mglchr(pen,'^'))	dv = 0;
	if(mglchr(pen,'>'))	dv = -1;

	gr->SetPenPal(pen,&pal);
	char c1[8];	memset(c1,0,8);	c1[0] ='@';
	char c2[8];	memset(c2,0,8);	c2[0] ='@';
	if(wire)	{	c1[5]=c2[5]='#';	i++;	}
	if(mglchr(pen,'&'))	c1[i]=c2[i]='4';
	else if(mglchr(pen,'4'))	c1[i]=c2[i]='4';
	else if(mglchr(pen,'6'))	c1[i]=c2[i]='6';
	else if(mglchr(pen,'8'))	c1[i]=c2[i]='8';
	memset(dd,0,2*n*sizeof(mreal));
	z0 = gr->GetOrgZ('x');
	for(i=0;i<n;i++)	for(j=0;j<m;j++)	dd[i] += z->v(i, j<nz ? j:0);
	for(j=0;j<m;j++)
	{
		gr->NextColor(pal);		memcpy(c1+1,gr->last_line(),4);
		if(gr->GetNumPal(pal)==2*m)
		{	gr->NextColor(pal);	memcpy(c2+1,gr->last_line(),4);	}
		else	memcpy(c2,c1,8);
		mx = j<x->GetNy() ? j:0;	my = j<y->GetNy() ? j:0;	mz = j<nz ? j:0;
		for(i=0;i<n;i++)
		{
			if(gr->Stop)	{	delete []dd;	return;	}
			vx=x->v(i,mx);	vy=y->v(i,my);	vz=z->v(i,mz);
			v0=y->v(i,0);	v1=i<nx-1 ? x->v(i+1,mx):x->v(i-1,mx);
			d = i<nx-1 ? v1-vx : vx-v1;
			x1 = vx + d/2*(dv-0.*gr->BarWidth);	// TODO
			d *= 0.7*gr->BarWidth;
			if(above)
			{
				zz = j>0?dd[i+n]:z0;	dd[i+n] += vz;
				mgl_cone(gr, x1,v0,zz, x1,v0,dd[i+n],
						 tube?d:d*(dd[i]-zz)/(dd[i]-z0),
						 tube?d:d*(dd[i]-dd[i+n])/(dd[i]-z0), c1);
			}
			else	mgl_cone(gr, x1,vy,z0, x1,vy,vz, d,tube?d:0, vz<0?c1:c2);
		}
	}
	gr->EndGroup();	delete []dd;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones_xz(HMGL gr, HCDT x, HCDT z, const char *pen, const char *opt)
{
	gr->SaveState(opt);
	mglData y(z);
	y.Fill(gr->Min.y,gr->Max.y,'y');
	mgl_cones_xyz(gr,x,&y,z,pen,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones(HMGL gr, HCDT z, const char *pen, const char *opt)
{
	gr->SaveState(opt);
	mglData x(z->GetNx()+1);
	x.Fill(gr->Min.x,gr->Max.x);
	mgl_cones_xz(gr,&x,z,pen,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *pen, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cones_xyz(_GR_,_DA_(x),_DA_(y),_DA_(z),s,o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones_xz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, const char *pen, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cones_xz(_GR_,_DA_(x),_DA_(y),s,o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_cones_(uintptr_t *gr, uintptr_t *y,	const char *pen, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,pen,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_cones(_GR_,_DA_(y),s,o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Ellipse & Rhomb
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_ellipse(HMGL gr, double x1, double y1, double z1, double x2, double y2, double z2, double r, const char *stl)
{
	const int n = 41;
	long pal=0,n0,n1=-1,n2,m1=-1,m2;
	static int cgid=1;	gr->StartGroup("Ellipse",cgid++);
	gr->SetPenPal(stl,&pal);
	mreal c=gr->NextColor(pal), d;
	mreal k=(gr->GetNumPal(pal)>1)?gr->NextColor(pal):gr->AddTexture('k');
	bool fill = !mglchr(stl,'#'), box = mglchr(stl,'@') || !fill;
	if(!fill)	k=c;

	gr->Reserve(2*n+1);
	if(mgl_isnan(z1) || mgl_isnan(z2))	z1=z2=2*gr->Max.z-gr->Min.z;
	mglPoint p1(x1,y1,z1), p2(x2,y2,z2), v=p2-p1;
	d = v.norm();
	if(d==0)	v = mglPoint(1);	else	v /= d;
	mglPoint u=mglPoint(0,0,1)^v, q=u^v, p, s=(p1+p2)/2.;
	u *= r;		v *= sqrt(d*d/4+r*r);
	// central point first
	n0 = gr->AddPnt(p1,c,q,-1,11);	gr->AddActive(n0);
	gr->AddActive(gr->AddPnt(p2,c,q,-1,11),1);
	for(long i=0;i<n;i++)
	{
		if(gr->Stop)	return;
		register int t=i*360/(n-1);
		p = s+v*mgl_cos[t%360]+u*mgl_cos[(270+t)%360];
		n2 = n1;	n1 = gr->AddPnt(p,c,q,-1,11);
		if(i==n/4)	gr->AddActive(n1,2);
		m2 = m1;	m1 = gr->CopyNtoC(n1,k);
		if(i>0)
		{
			if(fill)	gr->trig_plot(n0,n1,n2);
			if(box)	gr->line_plot(m1,m2);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_rhomb(HMGL gr, double x1, double y1, double z1, double x2, double y2, double z2, double r, const char *stl)
{
	long pal=0, n1,n2,n3,n4;
	static int cgid=1;	gr->StartGroup("Rhomb",cgid++);
	gr->SetPenPal(stl,&pal);
	mreal c=gr->NextColor(pal);
	mreal k=(gr->GetNumPal(pal)>1)?gr->NextColor(pal):gr->AddTexture('k');
	mreal b=(gr->GetNumPal(pal)>2)?gr->NextColor(pal):c;
	bool fill = !mglchr(stl,'#'), box = mglchr(stl,'@') || !fill;
	if(!fill)	k=c;
	gr->Reserve(8);
	if(mgl_isnan(z1) || mgl_isnan(z2))	z1=z2=2*gr->Max.z-gr->Min.z;
	mglPoint p1(x1,y1,z1), p2(x2,y2,z2), u=mglPoint(0,0,1)^(p1-p2), q=u^(p1-p2), p, s,qq;
	u = (r/u.norm())*u;	s = (p1+p2)/2.;
	p = p1;	q = qq;	n1 = gr->AddPnt(p,c,qq,-1,11);
	p = s+u;q = qq;	n2 = gr->AddPnt(p,b==c?c:k,qq,-1,11);
	p = p2;	q = qq;	n3 = gr->AddPnt(p,b,qq,-1,11);
	p = s-u;q = qq;	n4 = gr->AddPnt(p,b==c?c:k,qq,-1,11);
	gr->AddActive(n1,0);	gr->AddActive(n2,2);	gr->AddActive(n3,1);
	if(fill)	gr->quad_plot(n1,n2,n4,n3);
	n1 = gr->CopyNtoC(n1,k);	n2 = gr->CopyNtoC(n2,k);
	n3 = gr->CopyNtoC(n3,k);	n4 = gr->CopyNtoC(n4,k);
	if(box)
	{	gr->line_plot(n1,n2);	gr->line_plot(n2,n3);
		gr->line_plot(n3,n4);	gr->line_plot(n4,n1);	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_ellipse_(uintptr_t* gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, mreal *r, const char *stl,int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_ellipse(_GR_,*x1,*y1,*z1,*x2,*y2,*z2,*r,s);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_rhomb_(uintptr_t* gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, mreal *r, const char *stl,int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_rhomb(_GR_,*x1,*y1,*z1,*x2,*y2,*z2,*r,s);	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Sphere & Drop
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_sphere(HMGL gr, double x, double y, double z, double r, const char *stl)
{	mgl_drop(gr,x,y,z,1,0,0,2*r,stl,0,1);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_sphere_(uintptr_t* gr, mreal *x, mreal *y, mreal *z, mreal *r, const char *stl,int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_sphere(_GR_, *x,*y,*z,*r,s);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_drop(HMGL gr, mglPoint p, mglPoint q, double r, double c, double sh, double a)
{
	mglPoint p1,p2,pp,qq;
	if(q.norm()==0)	{	q = mglPoint(1,0,0);	sh=0;	}
	q.Normalize();	p1 = !q;	p2 = q^p1;	r /= 2;

	static int cgid=1;	gr->StartGroup("Drop",cgid++);
	const int m=12, n=2*m+1;
	gr->Reserve(n*m);
	long *nn=new long[2*n],n1,n2;
	mreal x,y,z,rr,dr;

	z = r*(1+sh)*(1+sh);	n1 = gr->AddPnt(p + q*z,c,q,-1,3);
	z = r*(1+sh)*(sh-1);	n2 = gr->AddPnt(p + q*z,c,q,-1,3);

	for(long i=0;i<=m;i++)	for(long j=0;j<n;j++)	// NOTE use prev.points => not for omp
	{
		if(gr->Stop)	continue;
		if(i>0 && i<m)
		{
			register int u=i*180/m, v=180*j/m+202;
			register float co=mgl_cos[u%360], si=mgl_cos[(u+270)%360];
			register float cv=mgl_cos[v%360], sv=mgl_cos[(v+270)%360];
			rr = r*a*si*(1.+sh*co)/(1+sh);
			dr = r*a/(1+sh)*(co*(1.+sh*co) - sh*si*si);
			x = rr*cv;	y = rr*sv;
			z = r*(1+sh)*(co+sh);
			pp = p + p1*x + p2*y + q*z;
			qq = (p1*sv-p2*cv)^(p1*(dr*cv) + p2*(dr*sv) - q*(r*(1+sh)*si));
			nn[j+n]=nn[j];	nn[j]=gr->AddPnt(pp,c,qq,-1,3);
		}
		else if(i==0)	nn[j] = n1;
		else if(i==m)	{	nn[j+n]=nn[j];	nn[j]=n2;	}
		if(i*j>0)	gr->quad_plot(nn[j-1], nn[j], nn[j+n-1], nn[j+n]);
	}
	delete []nn;	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_drop(HMGL gr, double x1, double y1, double z1, double x2, double y2, double z2, double r, const char *stl, double sh, double a)
{
	mreal c=gr->AddTexture((stl && stl[0]) ? stl[0]:'r');
	mgl_drop(gr,mglPoint(x1,y1,z1), mglPoint(x2,y2,z2), r, c, sh, a);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_drop_(uintptr_t* gr, mreal *x1, mreal *y1, mreal *z1, mreal *x2, mreal *y2, mreal *z2, mreal *r, const char *stl, mreal *shift, mreal *ap, int l)
{	char *s=new char[l+1];	memcpy(s,stl,l);	s[l]=0;
	mgl_drop(_GR_, *x1,*y1,*z1, *x2,*y2,*z2, *r,s,*shift,*ap);	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Dew series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dew_xy(HMGL gr, HCDT x, HCDT y, HCDT ax, HCDT ay, const char *sch, const char *opt)
{
	long n=ax->GetNx(),m=ax->GetNy();
	if(mgl_check_dim2(gr,x,y,ax,ay,"Dew"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("DewXY",cgid++);

	long ss = gr->AddTexture(sch);
	bool inv = mglchr(sch,'i');
	mreal zVal = gr->Min.z, xm=0;
	long tx=1,ty=1;
	if(gr->MeshNum>1)	{	tx=(n-1)/(gr->MeshNum-1);	ty=(m-1)/(gr->MeshNum-1);	}
	if(tx<1)	tx=1;	if(ty<1)	ty=1;

#pragma omp parallel
	{
		register mreal xm1=0,ym;
#pragma omp for nowait collapse(3)
		for(long k=0;k<ax->GetNz();k++)	for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
		{
			ym = sqrt(ax->v(i,j,k)*ax->v(i,j,k)+ay->v(i,j,k)*ay->v(i,j,k));
			xm1 = xm1>ym ? xm1 : ym;
		}
#pragma omp critical(max_vec)
		{xm = xm>xm1 ? xm:xm1;}
	}
	xm = 1./MGL_FEPSILON/(xm==0 ? 1:xm);

	for(long k=0;k<ax->GetNz();k++)
	{
		if(ax->GetNz()>1)	zVal = gr->Min.z+(gr->Max.z-gr->Min.z)*mreal(k)/(ax->GetNz()-1);
//#pragma omp parallel for collapse(2)	// gain looks negligible?!?
		for(long i=0;i<n;i+=tx)	for(long j=0;j<m;j+=ty)
		{
			if(gr->Stop)	continue;
			register mreal xx=GetX(x,i,j,k).x, yy=GetY(y,i,j,k).x, dd;
			register mreal dx = i<n-1 ? (GetX(x,i+1,j,k).x-xx) : (xx-GetX(x,i-1,j,k).x);
			register mreal dy = j<m-1 ? (GetY(y,i,j+1,k).x-yy) : (yy-GetY(y,i,j-1,k).x);
			dx *= tx;	dy *= ty;

			mglPoint q = mglPoint(ax->v(i,j,k),ay->v(i,j,k));	dd = q.norm();
			if(inv)	q = -q;
			mgl_drop(gr,mglPoint(xx, yy, zVal),q,(dx<dy?dx:dy)/2,gr->GetC(ss,dd*xm,false),dd*xm,1);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dew_2d(HMGL gr, HCDT ax, HCDT ay, const char *sch, const char *opt)
{
	gr->SaveState(opt);
	mglData x(ax->GetNx()), y(ax->GetNy());
	x.Fill(gr->Min.x,gr->Max.x);
	y.Fill(gr->Min.y,gr->Max.y);
	mgl_dew_xy(gr,&x,&y,ax,ay,sch,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dew_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *ax, uintptr_t *ay, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_dew_xy(_GR_, _DA_(x), _DA_(y), _DA_(ax), _DA_(ay), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_dew_2d_(uintptr_t *gr, uintptr_t *ax, uintptr_t *ay, const char *sch, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,sch,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_dew_2d(_GR_, _DA_(ax), _DA_(ay), s, o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
//
//	Puts series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_puts(HMGL gr, double x, double y, double z,const char *text, const char *font, double size)
{	mgl_puts_dir(gr, x, y, z, NAN, NAN, 0, text, font, size);	}
void MGL_EXPORT mgl_putsw(HMGL gr, double x, double y, double z,const wchar_t *text, const char *font, double size)
{	mgl_putsw_dir(gr, x, y, z, NAN, NAN, 0, text, font, size);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_puts_dir(HMGL gr, double x, double y, double z, double dx, double dy, double dz, const char *text, const char *font, double size)
{
	MGL_TO_WCS(text,mgl_putsw_dir(gr, x, y, z, dx, dy, dz, wcs, font, size));
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_putsw_dir(HMGL gr, double x, double y, double z, double dx, double dy, double dz, const wchar_t *text, const char *font, double size)
{
	bool a=mglchr(font,'a'), A=mglchr(font,'A');
	static int cgid=1;	gr->StartGroup("Puts",cgid++);
	mglCanvas *g = dynamic_cast<mglCanvas *>(gr);
	if(g && (a||A))
	{
		g->Push();	g->Identity(a);
		gr->set(MGL_DISABLE_SCALE);
		register mreal s=a?1:g->GetPlotFactor();
		x = (2*x-1)*s;	y = (2*y-1)*s;
		dx= (2*dx-1)*s;	dy= (2*dy-1)*s;
	}
	if(mgl_isnan(z))	z=2*gr->Max.z-gr->Min.z;
	mglPoint p(x,y,z), d(dx-x,dy-y,dz-z);
	long k = gr->AddPnt(p,-1,d,-1,7);
	gr->AddActive(k,0);
	gr->AddActive(gr->AddPnt(mglPoint(dx,dy,dz),-1,d,-1,7),1);
	if(g && (a||A))	{	g->Pop();	gr->clr(MGL_DISABLE_SCALE);	}
	gr->text_plot(k,text,font,size);
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_puts_(uintptr_t *gr, mreal *x, mreal *y, mreal *z,const char *text, const char *font, mreal *size, int l, int n)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,font,n);	f[n]=0;
	mgl_putsw_dir(_GR_, *x, *y, *z, NAN, NAN, 0, s, f, *size);
	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_puts_dir_(uintptr_t *gr, mreal *x, mreal *y, mreal *z, mreal *dx, mreal *dy, mreal *dz, const char *text, const char *font, mreal *size, int l, int n)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,font,n);	f[n]=0;
	mgl_putsw_dir(_GR_, *x, *y, *z, *dx, *dy, *dz, s, f, *size);
	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
//
//	TextMark series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmarkw_xyzr(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT r, const wchar_t *text, const char *fnt, const char *opt)
{
	long m,mx,my,mz,mr,n=y->GetNx();
	if(mgl_check_dim0(gr,x,y,z,r,"TextMark"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("TextMark",cgid++);
	m = x->GetNy() > y->GetNy() ? x->GetNy() : y->GetNy();
	m = z->GetNy() > m ? z->GetNy() : m;
	m = r->GetNy() > m ? r->GetNy() : m;
	gr->Reserve(n*m);

	mglPoint p,q(NAN);
	for(long j=0;j<m;j++)
	{
		mx = j<x->GetNy() ? j:0;	my = j<y->GetNy() ? j:0;
		mz = j<z->GetNy() ? j:0;	mr = j<r->GetNy() ? j:0;
#pragma omp parallel for private(p)	// NOTE this should be useless ?!?
		for(long i=0;i<n;i++)
		{
			if(gr->Stop)	continue;
			p = mglPoint(x->v(i,mx), y->v(i,my), z->v(i,mz));
			register long k = gr->AddPnt(p,-1,q);
			gr->text_plot(k, text, fnt, -0.5*fabs(r->v(i,mr)));
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmarkw_xyr(HMGL gr, HCDT x, HCDT y, HCDT r, const wchar_t *text, const char *fnt, const char *opt)
{
	gr->SaveState(opt);
	mglData z(y->GetNx());	z.Fill(gr->Min.z,gr->Min.z);
	mgl_textmarkw_xyzr(gr,x,y,&z,r,text,fnt,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmarkw_yr(HMGL gr, HCDT y, HCDT r, const wchar_t *text, const char *fnt, const char *opt)
{
	register long n=y->GetNx();
	gr->SaveState(opt);
	mglData x(n);	x.Fill(gr->Min.x,gr->Max.x);
	mglData z(n);	z.Fill(gr->Min.z,gr->Min.z);
	mgl_textmarkw_xyzr(gr,&x,y,&z,r,text,fnt,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmarkw(HMGL gr, HCDT y, const wchar_t *text, const char *fnt, const char *opt)
{
	register long n=y->GetNx();
	gr->SaveState(opt);
	mglData r(n);	r.Fill(1,1);
	mglData x(n);	x.Fill(gr->Min.x,gr->Max.x);
	mglData z(n);	z.Fill(gr->Min.z,gr->Min.z);
	mgl_textmarkw_xyzr(gr,&x,y,&z,&r,text,fnt,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_xyzr(HMGL gr, HCDT x, HCDT y, HCDT z, HCDT r, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_textmarkw_xyzr(gr, x, y, z, r, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_xyr(HMGL gr, HCDT x, HCDT y, HCDT r, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_textmarkw_xyr(gr, x, y, r, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_yr(HMGL gr, HCDT y, HCDT r, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_textmarkw_yr(gr, y, r, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark(HMGL gr, HCDT y, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_textmarkw(gr, y, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_xyzr_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, uintptr_t *r, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	memcpy(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_textmarkw_xyzr(_GR_, _DA_(x),_DA_(y),_DA_(z),_DA_(r),s,f, o);
	delete []o;	delete []s;		delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_xyr_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *r, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_textmarkw_xyr(_GR_, _DA_(x),_DA_(y),_DA_(r),s,f, o);
	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_yr_(uintptr_t *gr, uintptr_t *y, uintptr_t *r, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_textmarkw_yr(_GR_, _DA_(y),_DA_(r),s,f, o);	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_textmark_(uintptr_t *gr, uintptr_t *y, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_textmarkw(_GR_, _DA_(y),s,f, o);	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
//
//	Label series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_labelw_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, const wchar_t *text, const char *fnt, const char *opt)
{
	long m,mx,my,mz,n=y->GetNx();
	if(mgl_check_dim1(gr,x,y,z,0,"Label"))	return;

	gr->SaveState(opt);
	static int cgid=1;	gr->StartGroup("Label",cgid++);
	m = x->GetNy() > y->GetNy() ? x->GetNy() : y->GetNy();	m = z->GetNy() > m ? z->GetNy() : m;

	mglPoint q(NAN);
	wchar_t tmp[32];
	for(long j=0;j<m;j++)
	{
		mx = j<x->GetNy() ? j:0;	my = j<y->GetNy() ? j:0;	mz = j<z->GetNy() ? j:0;
#pragma omp parallel for private(tmp)
		for(long i=0;i<n;i++)
		{
			if(gr->Stop)	continue;
			mreal xx=x->v(i,mx), yy=y->v(i,my), zz=z->v(i,mz);
			register long kk = gr->AddPnt(mglPoint(xx,yy,zz),-1,q),k,l;
			std::wstring buf;
			for(k=l=0;text[k];k++)
			{
				if(text[k]!='%' || (k>0 && text[k-1]=='\\'))
				{	buf += text[k];	continue;	}
				else if(text[k+1]=='%')	{	buf+='%';	k++;	continue;	}
				else if(text[k+1]=='n')	mglprintf(tmp,32,L"%ld",i);
				else if(text[k+1]=='x')	mglprintf(tmp,32,L"%.2g",xx);
				else if(text[k+1]=='y')	mglprintf(tmp,32,L"%.2g",yy);
				else if(text[k+1]=='z')	mglprintf(tmp,32,L"%.2g",zz);
				else {	buf+='%';	continue;	}
				buf += tmp;	k++;
			}
			gr->text_plot(kk, buf.c_str(), fnt, -0.7, 0.05);
		}
	}
	gr->EndGroup();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_labelw_xy(HMGL gr, HCDT x, HCDT y, const wchar_t *text, const char *fnt, const char *opt)
{
	gr->SaveState(opt);
	mglData z(y->GetNx());	z.Fill(gr->Min.z,gr->Min.z);
	mgl_labelw_xyz(gr,x,y,&z,text,fnt,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_labelw_y(HMGL gr, HCDT y, const wchar_t *text, const char *fnt, const char *opt)
{
	register long n=y->GetNx();
	if(n<2)	{	gr->SetWarn(mglWarnLow,"TextMark");	return;	}
	gr->SaveState(opt);
	mglData x(n);	x.Fill(gr->Min.x,gr->Max.x);
	mglData z(n);	z.Fill(gr->Min.z,gr->Min.z);
	mgl_labelw_xyz(gr,&x,y,&z,text,fnt,0);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_xyz(HMGL gr, HCDT x, HCDT y, HCDT z, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_labelw_xyz(gr, x, y, z, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_xy(HMGL gr, HCDT x, HCDT y, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_labelw_xy(gr, x, y, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_y(HMGL gr, HCDT y, const char *str, const char *fnt, const char *opt)
{	MGL_TO_WCS(str,mgl_labelw_y(gr, y, wcs, fnt, opt));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_xyz_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, uintptr_t *z, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_labelw_xyz(_GR_, _DA_(x),_DA_(y),_DA_(z),s,f, o);
	delete []o;	delete []s;		delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_xy_(uintptr_t *gr, uintptr_t *x, uintptr_t *y, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_labelw_xy(_GR_, _DA_(x),_DA_(y),s,f, o);
	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_label_y_(uintptr_t *gr, uintptr_t *y, const char *text, const char *fnt, const char *opt, int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_labelw_y(_GR_, _DA_(y),s,f, o);	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
//
//	Table series
//
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_tablew(HMGL gr, double x, double y, HCDT val, const wchar_t *text, const char *fnt, const char *opt)
{
	mglCanvas *g = dynamic_cast<mglCanvas *>(gr);
	if(g)	g->Table(x,y,val,text,fnt,opt);
}
void MGL_EXPORT mgl_table(HMGL gr, double x, double y, HCDT val, const char *text, const char *fnt, const char *opt)
{
	if(!text)	mgl_tablew(gr,x,y,val,L"",fnt,opt);
	else	MGL_TO_WCS(text,mgl_tablew(gr, x, y, val, wcs, fnt, opt));
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_table_(uintptr_t *gr, mreal *x, mreal *y, uintptr_t *val, const char *text, const char *fnt, const char *opt,int l,int n,int lo)
{	wchar_t *s=new wchar_t[l+1];	mbstowcs(s,text,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,fnt,n);	f[n]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_tablew(_GR_, *x, *y, _DA_(val),s,f, o);
	delete []o;	delete []s;	delete []f;	}
//-----------------------------------------------------------------------------
