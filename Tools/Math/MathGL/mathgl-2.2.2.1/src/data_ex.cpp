/***************************************************************************
 * data_new.cpp is part of Math Graphic Library
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
#include <ctype.h>
#include "mgl2/data.h"
#include "mgl2/eval.h"
#include "mgl2/thread.h"
//-----------------------------------------------------------------------------
HMDT MGL_EXPORT mgl_data_trace(HCDT d)
{
	long nx=d->GetNx(),ny=d->GetNy(),nz=d->GetNz();
	mglData *r=new mglData(nx);
	if(ny>=nx && nz>=nx)
#pragma omp parallel for
		for(long i=0;i<nx;i++)	r->a[i] = d->v(i,i,i);
	else if(ny>=nx)
#pragma omp parallel for
		for(long i=0;i<nx;i++)	r->a[i] = d->v(i,i);
	else
#pragma omp parallel for
		for(long i=0;i<nx;i++)	r->a[i] = d->v(i);
	return r;
}
uintptr_t MGL_EXPORT mgl_data_trace_(uintptr_t *d)
{	return uintptr_t(mgl_data_trace(_DT_));	}
//-----------------------------------------------------------------------------
HMDT MGL_EXPORT mgl_data_subdata_ext(HCDT d, HCDT xx, HCDT yy, HCDT zz)
{
	long n=0,m=0,l=0,j,k;
	bool ix=false, iy=false, iz=false;
	if(xx->GetNz()>1)	// 3d data
	{
		n = xx->GetNx();	m = xx->GetNy();	l = xx->GetNz();
		j = yy->GetNx()*yy->GetNy()*yy->GetNz();	if(j>1 && j!=n*m*l)	return 0;	// wrong sizes
		k = zz->GetNx()*zz->GetNy()*zz->GetNz();	if(k>1 && k!=n*m*l)	return 0;	// wrong sizes
		ix = true;	iy = j>1;	iz = k>1;
	}
	else if(yy->GetNz()>1)
	{
		n = yy->GetNx();	m = yy->GetNy();	l = yy->GetNz();
		j = xx->GetNx()*xx->GetNy()*xx->GetNz();	if(j>1 && j!=n*m*l)	return 0;	// wrong sizes
		k = zz->GetNx()*zz->GetNy()*zz->GetNz();	if(k>1 && k!=n*m*l)	return 0;	// wrong sizes
		iy = true;	ix = j>1;	iz = k>1;
	}
	else if(zz->GetNz()>1)
	{
		n = zz->GetNx();	m = zz->GetNy();	l = zz->GetNz();
		j = yy->GetNx()*yy->GetNy()*yy->GetNz();	if(j>1 && j!=n*m*l)	return 0;	// wrong sizes
		k = xx->GetNx()*xx->GetNy()*xx->GetNz();	if(k>1 && k!=n*m*l)	return 0;	// wrong sizes
		iz = true;	iy = j>1;	ix = k>1;
	}
	else if(xx->GetNy()>1)	// 2d data
	{
		n = xx->GetNx();	m = xx->GetNy();	l = 1;
		j = yy->GetNx()*yy->GetNy();	if(j>1 && j!=n*m)	return 0;	// wrong sizes
		k = zz->GetNx()*zz->GetNy();	if(k>1 && k!=n*m)	return 0;	// wrong sizes
		ix = true;	iy = j>1;	iz = k>1;
	}
	else if(yy->GetNy()>1)
	{
		n = yy->GetNx();	m = yy->GetNy();	l = 1;
		j = xx->GetNx()*xx->GetNy();	if(j>1 && j!=n*m)	return 0;	// wrong sizes
		k = zz->GetNx()*zz->GetNy();	if(k>1 && k!=n*m)	return 0;	// wrong sizes
		iy = true;	ix = j>1;	iz = k>1;
	}
	else if(zz->GetNy()>1)
	{
		n = zz->GetNx();	m = zz->GetNy();	l = 1;
		j = yy->GetNx()*yy->GetNy();	if(j>1 && j!=n*m)	return 0;	// wrong sizes
		k = xx->GetNx()*xx->GetNy();	if(k>1 && k!=n*m)	return 0;	// wrong sizes
		iz = true;	iy = j>1;	ix = k>1;
	}
	long nx=d->GetNx(),ny=d->GetNy(),nz=d->GetNz();
	mreal vx=xx->v(0), vy=yy->v(0), vz=zz->v(0);
	mglData *r=new mglData;
	if(n*m*l>1)	// this is 2d or 3d data
	{
		r->Create(n,m,l);
#pragma omp parallel for
		for(long i0=0;i0<n*m*l;i0++)
		{
			register mreal x = ix?xx->vthr(i0):vx;
			register mreal y = iy?yy->vthr(i0):vy;
			register mreal z = iz?zz->vthr(i0):vz;
			r->a[i0] = mgl_data_linear(d,x,y,z);
		}
		return r;
	}
	// this is 1d data -> try as normal SubData()
	if(xx->GetNx()>1 || vx>=0)	{	n=xx->GetNx();	ix=true;	}
	else	{	n=nx;	ix=false;	}
	if(yy->GetNx()>1 || vy>=0)	{	m=yy->GetNx();	iy=true;	}
	else	{	m=ny;	iy=false;	}
	if(zz->GetNx()>1 || vz>=0)	{	l=zz->GetNx();	iz=true;	}
	else	{	l=nz;	iz=false;	}
	r->Create(n,m,l);
#pragma omp parallel for collapse(3)
	for(long k=0;k<l;k++)	for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
	{
		register mreal x = ix?xx->v(i):i, y = iy?yy->v(j):j, z = iz?zz->v(k):k;
		r->a[i+n*(j+m*k)] = mgl_data_linear(d,x,y,z);
	}
	if(m==1)	{	r->ny=r->nz;	r->nz=1;	}// "squeeze" dimensions
	if(n==1)	{	r->nx=r->ny;	r->ny=r->nz;	r->nz=1;	r->NewId();}
	return r;
}
//-----------------------------------------------------------------------------
HMDT MGL_EXPORT mgl_data_subdata(HCDT d, long xx,long yy,long zz)
{
	mglData x,y,z;
	x.a[0]=xx;	y.a[0]=yy;	z.a[0]=zz;
	return mgl_data_subdata_ext(d,&x,&y,&z);
}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_data_subdata_(uintptr_t *d, int *xx,int *yy,int *zz)
{	return uintptr_t(mgl_data_subdata(_DT_,*xx,*yy,*zz));	}
uintptr_t MGL_EXPORT mgl_data_subdata_ext_(uintptr_t *d, uintptr_t *xx, uintptr_t *yy, uintptr_t *zz)
{	return uintptr_t(mgl_data_subdata_ext(_DT_,_DA_(xx),_DA_(yy),_DA_(zz)));	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_column(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	const mglFormula *f = (const mglFormula *)t->v;
	long nx=t->p[0];
	mreal *b=t->a, var[MGL_VS];
	const mreal *a=t->b;
	memset(var,0,('z'-'a')*sizeof(mreal));
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<t->n;i+=mglNumThr)
	{
		for(long j=0;j<nx;j++)
			if(t->s[j]>='a' && t->s[j]<='z')
				var[t->s[j]-'a'] = a[j+nx*i];
		b[i] = f->Calc(var);
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_column(HCDT dat, const char *eq)
{	// NOTE: only for mglData (for speeding up)
	long nx=dat->GetNx(),ny=dat->GetNy(),nz=dat->GetNz();
	mglFormula f(eq);
	mglData *r=new mglData(ny,nz);
	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(d)	mglStartThread(mgl_column,0,ny*nz,r->a,d->a,0,&nx,&f,0,0,d->id.c_str());
	return r;
}
uintptr_t MGL_EXPORT mgl_data_column_(uintptr_t *d, const char *eq,int l)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	uintptr_t r = uintptr_t(mgl_data_column(_DT_,s));
	delete []s;	return r;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_resize(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0]+0.1, ny=t->p[1]+0.1;
	long n1=t->p[3]+0.1,n2=t->p[4]+0.1,n3=t->p[5]+0.1;
	mreal *b=t->a;
	const mreal *a=t->b, *c=t->c;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = mglSpline3(a,n1,n2,n3, c[0]+i*c[1], c[2]+j*c[3], c[4]+k*c[5]);
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_resize_box(HCDT dat, long mx,long my,long mz, mreal x1,mreal x2, mreal y1,mreal y2, mreal z1,mreal z2)
{	// NOTE: only for mglData (for speeding up)
	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(!d)	return 0;
	register long nx = d->nx-1, ny = d->ny-1, nz = d->nz-1;
	mx = mx<1 ? nx+1:mx;	my = my<1 ? ny+1:my;	mz = mz<1 ? nz+1:mz;
	mglData *r=new mglData(mx,my,mz);

	mreal par[6]={nx*x1,0,ny*y1,0,nz*z1,0};
	long nn[6]={mx,my,mz,nx+1,ny+1,nz+1};
	if(mx>1)	par[1] = nx*(x2-x1)/(mx-1);
	if(my>1)	par[3] = ny*(y2-y1)/(my-1);
	if(mz>1)	par[5] = nz*(z2-z1)/(mz-1);
	mglStartThread(mgl_resize,0,mx*my*mz,r->a,d->a,par,nn);
	return r;
}
HMDT MGL_EXPORT mgl_data_resize(HCDT d, long mx,long my,long mz)
{	return mgl_data_resize_box(d, mx,my,mz,0,1,0,1,0,1);	}
uintptr_t MGL_EXPORT mgl_data_resize_(uintptr_t *d, int *mx,int *my,int *mz)
{	return uintptr_t(mgl_data_resize(_DT_,*mx,*my,*mz));	}
uintptr_t MGL_EXPORT mgl_data_resize_box_(uintptr_t *d, int *mx,int *my,int *mz, mreal *x1,mreal *x2, mreal *y1,mreal *y2, mreal *z1,mreal *z2)
{	return uintptr_t(mgl_data_resize_box(_DT_,*mx,*my,*mz,*x1,*x2,*y1,*y2,*z1,*z2));	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_combine(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0];
	mreal *b=t->a;
	const mreal *c=t->b, *d=t->c;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)	b[i0] = c[i0%nx]*d[i0/nx];
	return 0;
}
HMDT MGL_EXPORT mgl_data_combine(HCDT d1, HCDT d2)
{	// NOTE: only for mglData (for speeding up)
	const mglData *a=dynamic_cast<const mglData *>(d1);
	const mglData *b=dynamic_cast<const mglData *>(d2);

	if(!a || !b || a->nz>1 || (a->ny>1 && b->ny>1) || b->nz>1)	return 0;
	mglData *r=new mglData;
	long n1=a->ny,n2=b->nx;
	bool dim2=true;
	if(a->ny==1)	{	n1=b->nx;	n2=b->ny;	dim2 = false;	}
	r->Create(a->nx,n1,n2);
	if(dim2)	n1=a->nx*a->ny;	else	{	n1=a->nx;	n2=b->nx*b->ny;	}
	mglStartThread(mgl_combine,0,n1*n2,r->a,a->a,b->a,&n1);
	return r;
}
uintptr_t MGL_EXPORT mgl_data_combine_(uintptr_t *a, uintptr_t *b)
{	return uintptr_t(mgl_data_combine(_DA_(a),_DA_(b)));	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_sum_z(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		b[i]=0;
		for(long j=0;j<nz;j++)	b[i] += a[i+nn*j];
		b[i] /= nz;
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_sum_y(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = (i%nx)+nx*ny*(i/nx);	b[i]=0;
		for(long j=0;j<ny;j++)	b[i] += a[k+nx*j];
		b[i] /= ny;
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_sum_x(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = i*nx;	b[i]=0;
		for(long j=0;j<nx;j++)	b[i] += a[j+k];
		b[i] /= nx;
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_sum(HCDT dat, const char *dir)
{
	if(!dir || *dir==0)	return 0;
	long nx=dat->GetNx(),ny=dat->GetNy(),nz=dat->GetNz();
	long p[3]={nx,ny,nz};
	mreal *b = new mreal[nx*ny*nz];
	mreal *c = new mreal[nx*ny*nz];

	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(d)	memcpy(c,d->a,nx*ny*nz*sizeof(mreal));
	else
#pragma omp parallel for
		for(long i=0;i<nx*ny*nz;i++)	c[i]=dat->vthr(i);

	if(strchr(dir,'z') && nz>1)
	{
		mglStartThread(mgl_sum_z,0,nx*ny,b,c,0,p);
		memcpy(c,b,nx*ny*sizeof(mreal));	p[2] = 1;
	}
	if(strchr(dir,'y') && ny>1)
	{
		mglStartThread(mgl_sum_y,0,nx*p[2],b,c,0,p);
		memcpy(c,b,nx*p[2]*sizeof(mreal));	p[1] = p[2];	p[2] = 1;
	}
	if(strchr(dir,'x') && nx>1)
	{
		mglStartThread(mgl_sum_x,0,p[1]*p[2],b,c,0,p);
		p[0] = p[1];	p[1] = p[2];	p[2] = 1;
	}
	mglData *r=new mglData(p[0],p[1],p[2]);
	memcpy(r->a,b,p[0]*p[1]*p[2]*sizeof(mreal));
	delete []b;	delete []c;	return r;
}
uintptr_t MGL_EXPORT mgl_data_sum_(uintptr_t *d, const char *dir,int l)
{	char *s=new char[l+1];	memcpy(s,dir,l);	s[l]=0;
	uintptr_t r=uintptr_t(mgl_data_sum(_DT_,s));	delete []s;	return r;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_max_z(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		b[i]=a[i];
		for(long j=1;j<nz;j++)	if(b[i]<a[i+nn*j]) b[i] = a[i+nn*j];
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_max_y(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = (i%nx)+nx*ny*(i/nx);	b[i]=a[k];
		for(long j=1;j<ny;j++)	if(b[i]<a[k+nx*j])	b[i]=a[k+nx*j];
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_max_x(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = i*nx;	b[i]=a[k];
		for(long j=1;j<nx;j++)	if(b[i]<a[j+k])	b[i]=a[j+k];
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_max_dir(HCDT dat, const char *dir)
{
	if(!dir || *dir==0)	return 0;
	long nx=dat->GetNx(),ny=dat->GetNy(),nz=dat->GetNz();
	long p[3]={nx,ny,nz};
	mreal *b = new mreal[nx*ny*nz];
	mreal *c = new mreal[nx*ny*nz];

	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(d)	memcpy(c,d->a,nx*ny*nz*sizeof(mreal));
	else
#pragma omp parallel for
		for(long i=0;i<nx*ny*nz;i++)	c[i]=dat->vthr(i);

	if(strchr(dir,'z') && nz>1)
	{
		mglStartThread(mgl_max_z,0,nx*ny,b,c,0,p);
		memcpy(c,b,nx*ny*sizeof(mreal));	p[2] = 1;
	}
	if(strchr(dir,'y') && ny>1)
	{
		mglStartThread(mgl_max_y,0,nx*p[2],b,c,0,p);
		memcpy(c,b,nx*p[2]*sizeof(mreal));	p[1] = p[2];	p[2] = 1;
	}
	if(strchr(dir,'x') && nx>1)
	{
		mglStartThread(mgl_max_x,0,p[1]*p[2],b,c,0,p);
		p[0] = p[1];	p[1] = p[2];	p[2] = 1;
	}
	mglData *r=new mglData(p[0],p[1],p[2]);
	memcpy(r->a,b,p[0]*p[1]*p[2]*sizeof(mreal));
	delete []b;	delete []c;	return r;
}
uintptr_t MGL_EXPORT mgl_data_max_dir_(uintptr_t *d, const char *dir,int l)
{	char *s=new char[l+1];	memcpy(s,dir,l);	s[l]=0;
	uintptr_t r=uintptr_t(mgl_data_max_dir(_DT_,s));	delete []s;	return r;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_min_z(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		b[i]=a[i];
		for(long j=1;j<nz;j++)	if(b[i]>a[i+nn*j]) b[i] = a[i+nn*j];
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_min_y(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = (i%nx)+nx*ny*(i/nx);	b[i]=a[k];
		for(long j=1;j<ny;j++)	if(b[i]>a[k+nx*j])	b[i]=a[k+nx*j];
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_min_x(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = i*nx;	b[i]=a[k];
		for(long j=1;j<nx;j++)	if(b[i]>a[j+k])	b[i]=a[j+k];
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_min_dir(HCDT dat, const char *dir)
{
	if(!dir || *dir==0)	return 0;
	long nx=dat->GetNx(),ny=dat->GetNy(),nz=dat->GetNz();
	long p[3]={nx,ny,nz};
	mreal *b = new mreal[nx*ny*nz];
	mreal *c = new mreal[nx*ny*nz];

	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(d)	memcpy(c,d->a,nx*ny*nz*sizeof(mreal));
	else
#pragma omp parallel for
		for(long i=0;i<nx*ny*nz;i++)	c[i]=dat->vthr(i);

	if(strchr(dir,'z') && nz>1)
	{
		mglStartThread(mgl_min_z,0,nx*ny,b,c,0,p);
		memcpy(c,b,nx*ny*sizeof(mreal));	p[2] = 1;
	}
	if(strchr(dir,'y') && ny>1)
	{
		mglStartThread(mgl_min_y,0,nx*p[2],b,c,0,p);
		memcpy(c,b,nx*p[2]*sizeof(mreal));	p[1] = p[2];	p[2] = 1;
	}
	if(strchr(dir,'x') && nx>1)
	{
		mglStartThread(mgl_min_x,0,p[1]*p[2],b,c,0,p);
		p[0] = p[1];	p[1] = p[2];	p[2] = 1;
	}
	mglData *r=new mglData(p[0],p[1],p[2]);
	memcpy(r->a,b,p[0]*p[1]*p[2]*sizeof(mreal));
	delete []b;	delete []c;	return r;
}
uintptr_t MGL_EXPORT mgl_data_min_dir_(uintptr_t *d, const char *dir,int l)
{	char *s=new char[l+1];	memcpy(s,dir,l);	s[l]=0;
	uintptr_t r=uintptr_t(mgl_data_min_dir(_DT_,s));	delete []s;	return r;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_mom_z(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
	const mglFormula *eq = (const mglFormula *)t->v;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register mreal i0 = 0, i1 = 0;
		for(long k=0;k<nx;k++)	for(long j=0;j<ny;j++)
		{
			register long ii = k+nx*(j+ny*i);
			register mreal x = k/(nx-1.), y = j/(ny-1.), z = i/(nz-1.);
			i0+= a[ii];
			i1+= a[ii]*eq->Calc(x,y,z,a[ii]);
		}
		b[i] = i0>0 ? i1/i0 : 0;
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_mom_y(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
	const mglFormula *eq = (const mglFormula *)t->v;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register mreal i0 = 0, i1 = 0;
		for(long j=0;j<nx;j++)	for(long k=0;k<nz;k++)
		{
			register long ii = j+nx*(i+ny*k);
			register mreal x = k/(nx-1.), y = j/(ny-1.), z = i/(nz-1.);
			i0+= a[ii];
			i1+= a[ii]*eq->Calc(x,y,z,a[ii]);
		}
		b[i] = i0>0 ? i1/i0 : 0;
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_mom_x(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0], ny=t->p[1], nz=t->p[2], nn=t->n;
	mreal *b=t->a;
	const mreal *a=t->b;
	const mglFormula *eq = (const mglFormula *)t->v;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register mreal i0 = 0, i1 = 0;
		for(long j=0;j<ny;j++)	for(long k=0;k<nz;k++)
		{
			register long ii = i+nx*(j+ny*k);
			register mreal x = k/(nx-1.), y = j/(ny-1.), z = i/(nz-1.);
			i0+= a[ii];
			i1+= a[ii]*eq->Calc(x,y,z,a[ii]);
		}
		b[i] = i0>0 ? i1/i0 : 0;
	}
	return 0;
}
HMDT MGL_EXPORT mgl_data_momentum(HCDT dat, char dir, const char *how)
{	// NOTE: only for mglData (for speeding up)
	long nx=dat->GetNx(),ny=dat->GetNy(),nz=dat->GetNz();
	const mglData *d=dynamic_cast<const mglData *>(dat);
	if(!d)	return 0;
	mglFormula eq(how);
	long p[3]={nx,ny,nz};
	mglData *b=0;
	if(dir=='x')
	{	b=new mglData(nx);	mglStartThread(mgl_mom_x,0,nx,b->a,d->a,0,p,&eq);	}
	if(dir=='y')
	{	b=new mglData(ny);	mglStartThread(mgl_mom_y,0,ny,b->a,d->a,0,p,&eq);	}
	if(dir=='z')
	{	b=new mglData(nz);	mglStartThread(mgl_mom_z,0,nz,b->a,d->a,0,p,&eq);	}
	return b;
}
uintptr_t MGL_EXPORT mgl_data_momentum_(uintptr_t *d, char *dir, const char *how, int,int l)
{	char *s=new char[l+1];	memcpy(s,how,l);	s[l]=0;
	uintptr_t r=uintptr_t(mgl_data_momentum(_DT_,*dir, s));
	delete []s;	return r;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_eqmul(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0];
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<t->n;i+=mglNumThr)	b[i] *= a[i%nx];
	return 0;
}
void MGL_EXPORT mgl_data_mul_dat(HMDT d, HCDT a)
{
	long n, nx=d->nx, ny=d->ny, nz=d->nz;
	const mglData *b = dynamic_cast<const mglData *>(a);
	if(b)
	{
		if(b->nz==1 && b->ny==1 && nx==b->nx)
		{	n=nx;		mglStartThread(mgl_eqmul,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(b->nz==1 && ny==b->ny && nx==b->nx)
		{	n=nx*ny;	mglStartThread(mgl_eqmul,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(nz==b->nz && ny==b->ny && nx==b->nx)
		{	n=nx*ny*nz;	mglStartThread(mgl_eqmul,0,nx*ny*nz,d->a,b->a,0,&n);	}
	}
	else
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			d->a[i+nx*(j+ny*k)] *= a->v(i,j,k);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_mul_num(HMDT d, mreal a)
{
	long n=1, nx=d->nx, ny=d->ny, nz=d->nz;	mreal aa=a;
	mglStartThread(mgl_eqmul,0,nx*ny*nz,d->a,&aa,0,&n);
}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_eqdiv(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0];
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<t->n;i+=mglNumThr)	b[i] /= a[i%nx];
	return 0;
}
void MGL_EXPORT mgl_data_div_dat(HMDT d, HCDT a)
{
	long n, nx=d->nx, ny=d->ny, nz=d->nz;
	const mglData *b = dynamic_cast<const mglData *>(a);
	if(b)
	{
		if(b->nz==1 && b->ny==1 && nx==b->nx)
		{	n=nx;		mglStartThread(mgl_eqdiv,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(b->nz==1 && ny==b->ny && nx==b->nx)
		{	n=nx*ny;	mglStartThread(mgl_eqdiv,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(nz==b->nz && ny==b->ny && nx==b->nx)
		{	n=nx*ny*nz;	mglStartThread(mgl_eqdiv,0,nx*ny*nz,d->a,b->a,0,&n);	}
	}
	else
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			d->a[i+nx*(j+ny*k)] /= a->v(i,j,k);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_div_num(HMDT d, mreal a)
{
	long n=1, nx=d->nx, ny=d->ny, nz=d->nz;	mreal aa=a;
	mglStartThread(mgl_eqdiv,0,nx*ny*nz,d->a,&aa,0,&n);
}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_eqadd(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0];
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<t->n;i+=mglNumThr)	b[i] += a[i%nx];
	return 0;
}
void MGL_EXPORT mgl_data_add_dat(HMDT d, HCDT a)
{
	long n, nx=d->nx, ny=d->ny, nz=d->nz;
	const mglData *b = dynamic_cast<const mglData *>(a);
	if(b)
	{
		if(b->nz==1 && b->ny==1 && nx==b->nx)
		{	n=nx;		mglStartThread(mgl_eqadd,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(b->nz==1 && ny==b->ny && nx==b->nx)
		{	n=nx*ny;	mglStartThread(mgl_eqadd,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(nz==b->nz && ny==b->ny && nx==b->nx)
		{	n=nx*ny*nz;	mglStartThread(mgl_eqadd,0,nx*ny*nz,d->a,b->a,0,&n);	}
	}
	else
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			d->a[i+nx*(j+ny*k)] += a->v(i,j,k);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_add_num(HMDT d, mreal a)
{
	long n=1, nx=d->nx, ny=d->ny, nz=d->nz;	mreal aa=a;
	mglStartThread(mgl_eqadd,0,nx*ny*nz,d->a,&aa,0,&n);
}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_eqsub(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0];
	mreal *b=t->a;
	const mreal *a=t->b;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<t->n;i+=mglNumThr)	b[i] -= a[i%nx];
	return 0;
}
void MGL_EXPORT mgl_data_sub_dat(HMDT d, HCDT a)
{
	long n, nx=d->nx, ny=d->ny, nz=d->nz;
	const mglData *b = dynamic_cast<const mglData *>(a);
	if(b)
	{
		if(b->nz==1 && b->ny==1 && nx==b->nx)
		{	n=nx;		mglStartThread(mgl_eqsub,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(b->nz==1 && ny==b->ny && nx==b->nx)
		{	n=nx*ny;	mglStartThread(mgl_eqsub,0,nx*ny*nz,d->a,b->a,0,&n);	}
		else if(nz==b->nz && ny==b->ny && nx==b->nx)
		{	n=nx*ny*nz;	mglStartThread(mgl_eqsub,0,nx*ny*nz,d->a,b->a,0,&n);	}
	}
	else
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long j=0;j<ny;j++)	for(long i=0;i<nx;i++)
			d->a[i+nx*(j+ny*k)] -= a->v(i,j,k);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_sub_num(HMDT d, mreal a)
{
	long n=1, nx=d->nx, ny=d->ny, nz=d->nz;	mreal aa=a;
	mglStartThread(mgl_eqsub,0,nx*ny*nz,d->a,&aa,0,&n);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_mul_dat_(uintptr_t *d, uintptr_t *b)	{	mgl_data_mul_dat(_DT_, _DA_(b));	}
void MGL_EXPORT mgl_data_div_dat_(uintptr_t *d, uintptr_t *b)	{	mgl_data_div_dat(_DT_, _DA_(b));	}
void MGL_EXPORT mgl_data_add_dat_(uintptr_t *d, uintptr_t *b)	{	mgl_data_add_dat(_DT_, _DA_(b));	}
void MGL_EXPORT mgl_data_sub_dat_(uintptr_t *d, uintptr_t *b)	{	mgl_data_sub_dat(_DT_, _DA_(b));	}
void MGL_EXPORT mgl_data_mul_num_(uintptr_t *d, mreal *b)		{	mgl_data_mul_num(_DT_, *b);	}
void MGL_EXPORT mgl_data_div_num_(uintptr_t *d, mreal *b)		{	mgl_data_div_num(_DT_, *b);	}
void MGL_EXPORT mgl_data_add_num_(uintptr_t *d, mreal *b)		{	mgl_data_add_num(_DT_, *b);	}
void MGL_EXPORT mgl_data_sub_num_(uintptr_t *d, mreal *b)		{	mgl_data_sub_num(_DT_, *b);	}
//-----------------------------------------------------------------------------
void MGL_NO_EXPORT mgl_hist_p(mglThreadD *t,mreal *a)
{
	long n=t[0].p[0];
	memset(a,0,n*sizeof(mreal));
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=0;i<mglNumThr;i++)
	{
		mreal *b = t[i].a;
		for(long j=0;j<n;j++)	a[j] += b[j];
		delete []b;
	}
}
MGL_NO_EXPORT void *mgl_hist_1(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nn=t->n, n = t->p[0];
	mreal *b=new mreal[n];
	memset(b,0,n*sizeof(mreal));
	const mreal *a=t->b, *c=t->c, *v=(const mreal *)t->v;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register long k = long(n*(a[i]-v[0])/(v[1]-v[0]));
		if(k>=0 && k<n)
#if !MGL_HAVE_PTHREAD
#pragma omp critical(hist)
#endif
			b[k] += c ? c[i]:1.;
	}
	t->a = b;	return 0;
}
MGL_NO_EXPORT void *mgl_hist_2(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nn=t->n, n = t->p[0];
	long ns=t->p[1], nx=t->p[2], ny=t->p[3], nz=t->p[4];
	mreal *b=new mreal[n], d=1./ns;
	memset(b,0,n*sizeof(mreal));
	const mreal *a=t->b, *c=t->c, *v=(const mreal *)t->v;
	bool sp = n>0;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=t->id;i<nn;i+=mglNumThr)
	{
		register mreal x = d*(i%(nx*ns)), y = d*((i/(nx*ns))%(ny*ns)), z = d*(i/(nx*ny*ns*ns));
		register mreal f = sp ? mglSpline3(a,nx,ny,nz,x,y,z) : mglLinear(a,nx,ny,nz,x,y,z), w=1;
		if(c)	w = sp ? mglSpline3(c,nx,ny,nz,x,y,z) : mglLinear(c,nx,ny,nz,x,y,z);
		if(mgl_isnan(f) || mgl_isnan(w))	continue;
		register long k = long(n*(f-v[0])/(v[1]-v[0]));
		if(k>=0 && k<n)
#if !MGL_HAVE_PTHREAD
#pragma omp critical(hist)
#endif
			b[k] += w * d*d*d;
	}
	t->a = b;	return 0;
}
HMDT MGL_EXPORT mgl_data_hist(HCDT dat, long n, mreal v1, mreal v2, long nsub)
{
	const mglData *d = dynamic_cast<const mglData *>(dat);
	if(n<2 || v1==v2 || !d)	return 0;	// NOTE: For mglData only!
	mglData *b=new mglData(n);
	mreal v[2]={v1,v2};
	long nx=d->nx, ny=d->ny, nz=d->nz;
	long ns=abs(nsub)+1, p[5]={n,ns,nx,ny,nz};
	if(nsub==0)	mglStartThread(mgl_hist_1,mgl_hist_p, nx*ny*nz, b->a,d->a,0,p,v);
	else	mglStartThread(mgl_hist_2,mgl_hist_p, nx*ny*nz*ns*ns*ns, b->a,d->a,0,p,v);
	return b;
}
//-----------------------------------------------------------------------------
HMDT MGL_EXPORT mgl_data_hist_w(HCDT dat, HCDT weight, long n, mreal v1, mreal v2, long nsub)
{
	const mglData *d = dynamic_cast<const mglData *>(dat);
	const mglData *w = dynamic_cast<const mglData *>(weight);
	if(n<2 || v1==v2 || !d || !w)	return 0;	// NOTE: For mglData only!
	mglData *b=new mglData(n);
	mreal v[2]={v1,v2};

	long nx=d->nx, ny=d->ny, nz=d->nz;
	long ns=abs(nsub)+1, p[5]={n,ns,nx,ny,nz};
	if(nsub==0)	mglStartThread(mgl_hist_1,mgl_hist_p, nx*ny*nz, b->a,d->a,w->a,p,v);
	else	mglStartThread(mgl_hist_2,mgl_hist_p, nx*ny*nz*ns*ns*ns, b->a,d->a,w->a,p,v);
	return b;
}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_data_hist_(uintptr_t *d, int *n, mreal *v1, mreal *v2, int *nsub)
{	return uintptr_t(mgl_data_hist(_DT_,*n,*v1,*v2,*nsub));	}
uintptr_t MGL_EXPORT mgl_data_hist_w_(uintptr_t *d, uintptr_t *w, int *n, mreal *v1, mreal *v2, int *nsub)
{	return uintptr_t(mgl_data_hist_w(_DT_,_DA_(w),*n,*v1,*v2,*nsub));	}
//-----------------------------------------------------------------------------
long MGL_NO_EXPORT mgl_idx_var;
int MGL_NO_EXPORT mgl_cmd_idx(const void *a, const void *b)
{
	mreal *aa = (mreal *)a, *bb = (mreal *)b;
	return (aa[mgl_idx_var]>bb[mgl_idx_var] ? 1:(aa[mgl_idx_var]<bb[mgl_idx_var]?-1:0) );
}
void MGL_EXPORT mgl_data_sort(HMDT dat, long idx, long idy)
{
	mglData *d = dynamic_cast<mglData *>(dat);
	if(!d || idx>=d->nx || idx<0)	return;
	bool single = (d->nz==1 || idy<0);
	if(idy<0 || idy>d->ny)	idy=0;
	mgl_idx_var = idx+d->nx*idy;	// NOTE: not thread safe!!!
	if(single)	qsort(d->a, d->ny*d->nz, d->nx*sizeof(mreal), mgl_cmd_idx);
	else		qsort(d->a, d->nz, d->ny*d->nx*sizeof(mreal), mgl_cmd_idx);
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_sort_(uintptr_t *d, int *idx, int *idy)
{	mgl_data_sort(_DT_,*idx,*idy);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_find_root(mreal (*func)(mreal x, void *par), mreal x0, void *par)
{
	mreal x1=x0+1e-2*(x0?x0:1), f0=func(x0,par), f1=func(x1,par), x, f;
	if(fabs(f0)<1e-7)	return x0;
	if(fabs(f1)<1e-7)	return x1;
	if(f0==f1)	return NAN;
	for(long i=0;i<20;i++)
	{
		x = x1-f1*(x1-x0)/(f1-f0);	f = func(x,par);
		if(fabs(f)<1e-7)	return x;
/*		if(fabs(f-f1)>0.5*fmin(fabs(f),fabs(f1)))	// TODO switch to bisection if slow
		{
			x = (x1+x0)/2;	f = func(x,par);
			if(fabs(f)<1e-7)	return x;
		}*/
		x0=x1;	f0=f1;	x1=x;	f1=f;	// new points
	}
	return NAN;	// no roots found
}
//-----------------------------------------------------------------------------
struct MGL_NO_EXPORT mglFuncV	{	mglFormula *eq;	char var;	};
mreal MGL_NO_EXPORT mgl_funcv(mreal v, void *par)
{
	mglFuncV *f = (mglFuncV *)par;
	mreal var[MGL_VS];	memset(var,0,('z'-'a')*sizeof(mreal));
	var[f->var-'a'] = v;
	return f->eq->Calc(var);
}
HMDT MGL_EXPORT mgl_data_roots(const char *func, HCDT ini, char var)
{
	if(!ini)	return 0;
	mglData *res = new mglData(ini);

	mglFormula eq(func);
	mglFuncV f;	f.eq = &eq;	f.var = var;
	long n = res->nx*res->ny*res->nz;
#pragma omp parallel for
	for(long i=0;i<n;i++)
		res->a[i] = mgl_find_root(mgl_funcv,res->a[i],&f);
	return res;
}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_find_root_txt(const char *func, mreal ini, char var)
{
	mglFormula eq(func);
	mglFuncV f;	f.eq = &eq;	f.var = var;
	return mgl_find_root(mgl_funcv,ini,&f);
}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_data_roots_(const char *func, uintptr_t *ini, const char *var,int l,int)
{	char *s=new char[l+1];	memcpy(s,func,l);	s[l]=0;
	uintptr_t r = uintptr_t(mgl_data_roots(s,_DA_(ini),*var));
	delete []s;	return r;	}
mreal MGL_EXPORT mgl_find_root_txt_(const char *func, mreal *ini, const char *var,int l,int)
{	char *s=new char[l+1];	memcpy(s,func,l);	s[l]=0;
	mreal r = mgl_find_root_txt(s,*ini,*var);
	delete []s;	return r;	}
//-----------------------------------------------------------------------------
