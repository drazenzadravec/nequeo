/***************************************************************************
 * data_gr.cpp is part of Math Graphic Library
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

#ifndef WIN32
#include <glob.h>
#endif

#include "mgl2/datac.h"
#include "mgl2/evalc.h"
#include "mgl2/thread.h"
#include "mgl2/base.h"
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_refill_gr(HMGL gr, HMDT dat, HCDT xdat, HCDT ydat, HCDT zdat, HCDT vdat, long sl, const char *opt)
{
	if(!vdat)	return;
	gr->SaveState(opt);
	if(!ydat && !zdat)	mgl_data_refill_x(dat,xdat,vdat,gr->Min.x,gr->Max.x,sl);
//	else if(!xdat && !zdat)	mgl_data_refill_x(dat,ydat,vdat,gr->Min.y,gr->Max.y,sl);
//	else if(!xdat && !ydat)	mgl_data_refill_x(dat,zdat,vdat,gr->Min.z,gr->Max.z,sl);
	else if(!zdat)	mgl_data_refill_xy(dat,xdat,ydat,vdat,gr->Min.x,gr->Max.x,gr->Min.y,gr->Max.y,sl);
//	else if(!ydat)	mgl_data_refill_xy(dat,xdat,zdat,vdat,gr->Min.x,gr->Max.x,gr->Min.z,gr->Max.z,sl);
//	else if(!xdat)	mgl_data_refill_xy(dat,ydat,zdat,vdat,gr->Min.y,gr->Max.y,gr->Min.z,gr->Max.z,sl);
	else	mgl_data_refill_xyz(dat,xdat,ydat,zdat,vdat,gr->Min.x,gr->Max.x,gr->Min.y,gr->Max.y,gr->Min.z,gr->Max.z);
	gr->LoadState();
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_refill_x_(uintptr_t *d, uintptr_t *xdat, uintptr_t *vdat, mreal *x1, mreal *x2, long *sl)
{	mgl_data_refill_x(_DT_,_DA_(xdat),_DA_(vdat),*x1,*x2,*sl);	}
void MGL_EXPORT mgl_data_refill_xy_(uintptr_t *d, uintptr_t *xdat, uintptr_t *ydat, uintptr_t *vdat, mreal *x1, mreal *x2, mreal *y1, mreal *y2, long *sl)
{	mgl_data_refill_xy(_DT_,_DA_(xdat),_DA_(ydat),_DA_(vdat),*x1,*x2,*y1,*y2,*sl);	}
void MGL_EXPORT mgl_data_refill_xyz_(uintptr_t *d, uintptr_t *xdat, uintptr_t *ydat, uintptr_t *zdat, uintptr_t *vdat, mreal *x1, mreal *x2, mreal *y1, mreal *y2, mreal *z1, mreal *z2)
{	mgl_data_refill_xyz(_DT_,_DA_(xdat),_DA_(ydat),_DA_(zdat),_DA_(vdat),*x1,*x2,*y1,*y2,*z1,*z2);	}
void MGL_EXPORT mgl_data_refill_gr_(uintptr_t *gr, uintptr_t *d, uintptr_t *xdat, uintptr_t *ydat, uintptr_t *zdat, uintptr_t *vdat, long *sl, const char *opt,int l)
{	char *s=new char[l+1];	memcpy(s,opt,l);	s[l]=0;
	mgl_data_refill_gr(_GR_,_DT_,_DA_(xdat),_DA_(ydat),_DA_(zdat),_DA_(vdat),*sl,s);	delete []s;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_fill_f(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	const mglFormula *f = (const mglFormula *)(t->v);
	long nx=t->p[0],ny=t->p[1];
	mreal *b=t->a;
	const mreal *v=t->b, *w=t->c, *x=t->d;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(x[0]+i*x[1], x[2]+j*x[3], x[4]+k*x[5], b[i0], v?v[i0]:0, w?w[i0]:0);
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_fill_fgen(void *par)
{
	mglThreadV *t=(mglThreadV *)par;
	const mglFormula *f = (const mglFormula *)(t->v);
	long nx=t->p[0],ny=t->p[1];
	mreal *b=t->a;
	HCDT v=(HCDT)t->b, w=(HCDT)t->c;
	const mreal *x=t->d;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(x[0]+i*x[1], x[2]+j*x[3], x[4]+k*x[5], b[i0], v?v->vthr(i0):0, w?w->vthr(i0):0);
	}
	return 0;
}
void MGL_EXPORT mgl_data_fill_eq(HMGL gr, HMDT d, const char *eq, HCDT vdat, HCDT wdat, const char *opt)
{
	const mglData *v = dynamic_cast<const mglData *>(vdat);
	const mglData *w = dynamic_cast<const mglData *>(wdat);
	long nn = d->nx*d->ny*d->nz, par[3]={d->nx,d->ny,d->nz};
	if(vdat && vdat->GetNN()!=nn)	return;
	if(wdat && wdat->GetNN()!=nn)	return;
	gr->SaveState(opt);
	mreal xx[6]={gr->Min.x,0, gr->Min.y,0, gr->Min.z,0};
	if(d->nx>1)	xx[1] = (gr->Max.x-gr->Min.x)/(d->nx-1.);
	if(d->ny>1)	xx[3] = (gr->Max.y-gr->Min.y)/(d->ny-1.);
	if(d->nz>1)	xx[5] = (gr->Max.z-gr->Min.z)/(d->nz-1.);
	mglFormula f(eq);
	if(v && w)	mglStartThread(mgl_fill_f,0,nn,d->a,v->a,w->a,par,&f,xx);
	else if(vdat && wdat)	mglStartThreadV(mgl_fill_fgen,nn,d->a,vdat,wdat,par,&f,xx);
	else if(v)	mglStartThread(mgl_fill_f,0,nn,d->a,v->a,0,par,&f,xx);
	else if(vdat)	mglStartThreadV(mgl_fill_fgen,nn,d->a,vdat,0,par,&f,xx);
	else	mglStartThread(mgl_fill_f,0,nn,d->a,0,0,par,&f,xx);
	gr->LoadState();
}
void MGL_EXPORT mgl_data_fill_eq_(uintptr_t *gr, uintptr_t *d, const char *eq, uintptr_t *v, uintptr_t *w, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_data_fill_eq(_GR_,_DT_,s,_DA_(v),_DA_(w),o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_cfill_f(void *par)
{
	mglThreadC *t=(mglThreadC *)par;
	const mglFormulaC *f = (const mglFormulaC *)(t->v);
	long nx=t->p[0],ny=t->p[1];
	dual *b=t->a;
	const dual *v=t->b, *w=t->c, *x=t->d;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(x[0]+mreal(i)*x[1], x[2]+mreal(j)*x[3], x[4]+mreal(k)*x[5],
						b[i0], v?v[i0]:dual(0,0), w?w[i0]:dual(0,0));
	}
	return 0;
}
MGL_NO_EXPORT void *mgl_cfill_fgen(void *par)
{
	mglThreadV *t=(mglThreadV *)par;
	const mglFormulaC *f = (const mglFormulaC *)(t->v);
	long nx=t->p[0],ny=t->p[1];
	dual *b=t->aa;
	HCDT v=(HCDT)t->b, w=(HCDT)t->c;
	const mreal *x=t->d;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(x[0]+i*x[1], x[2]+j*x[3], x[4]+k*x[5],
						b[i0], v?v->vthr(i0):0, w?w->vthr(i0):0);
	}
	return 0;
}
void MGL_EXPORT mgl_datac_fill_eq(HMGL gr, HADT d, const char *eq, HCDT vdat, HCDT wdat, const char *opt)
{
	const mglDataC *v = dynamic_cast<const mglDataC *>(vdat);
	const mglDataC *w = dynamic_cast<const mglDataC *>(wdat);
	long nn = d->nx*d->ny*d->nz, par[3]={d->nx,d->ny,d->nz};
	if(v && v->nx*v->ny*v->nz!=nn)	return;
	if(w && w->nx*w->ny*w->nz!=nn)	return;
	gr->SaveState(opt);
	mreal xx[6]={gr->Min.x,0, gr->Min.y,0, gr->Min.z,0};
	if(d->nx>1)	xx[1] = (gr->Max.x-gr->Min.x)/(d->nx-1.);
	if(d->ny>1)	xx[3] = (gr->Max.y-gr->Min.y)/(d->ny-1.);
	if(d->nz>1)	xx[5] = (gr->Max.z-gr->Min.z)/(d->nz-1.);
	dual cc[6]={xx[0],xx[1],xx[2],xx[3],xx[4],xx[5]};
	mglFormulaC f(eq);
	if(v && w)	mglStartThreadC(mgl_cfill_f,0,nn,d->a,v->a,w->a,par,&f,cc);
	else if(vdat && wdat)	mglStartThreadV(mgl_cfill_fgen,nn,d->a,vdat,wdat,par,&f,xx);
	else if(v)	mglStartThreadC(mgl_cfill_f,0,nn,d->a,v->a,0,par,&f,cc);
	else if(vdat)	mglStartThreadV(mgl_cfill_fgen,nn,d->a,vdat,0,par,&f,xx);
	else	mglStartThreadC(mgl_cfill_f,0,nn,d->a,0,0,par,&f,cc);
	gr->LoadState();
}
void MGL_EXPORT mgl_datac_fill_eq_(uintptr_t *gr, uintptr_t *d, const char *eq, uintptr_t *v, uintptr_t *w, const char *opt,int l,int lo)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	char *o=new char[lo+1];	memcpy(o,opt,lo);	o[lo]=0;
	mgl_datac_fill_eq(_GR_,_DC_,s,_DA_(v),_DA_(w),o);	delete []o;	delete []s;	}
//-----------------------------------------------------------------------------
