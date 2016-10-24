/***************************************************************************
 * data_io.cpp is part of Math Graphic Library
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

#include "mgl2/data.h"
#include "mgl2/datac.h"
#include "mgl2/eval.h"
#include "mgl2/thread.h"

#if MGL_HAVE_HDF5
//#define H5_NO_DEPRECATED_SYMBOLS
#define H5_USE_16_API
#include <hdf5.h>
#endif
#if MGL_HAVE_HDF4
#define intf hdf4_intf
#include <hdf/mfhdf.h>
#undef intf
#endif

//#define isn(ch)		((ch)<' ' && (ch)!='\t')
#define isn(ch)		((ch)=='\n')
//-----------------------------------------------------------------------------
HMDT MGL_EXPORT mgl_create_data()	{	return new mglData;	}
HMDT MGL_EXPORT mgl_create_data_size(long nx, long ny, long nz){	return new mglData(nx,ny,nz);	}
HMDT MGL_EXPORT mgl_create_data_file(const char *fname)		{	return new mglData(fname);	}
void MGL_EXPORT mgl_delete_data(HMDT d)	{	if(d)	delete d;	}
//-----------------------------------------------------------------------------
uintptr_t MGL_EXPORT mgl_create_data_()
{	return uintptr_t(new mglData());	}
uintptr_t MGL_EXPORT mgl_create_data_size_(int *nx, int *ny, int *nz)
{	return uintptr_t(new mglData(*nx,*ny,*nz));	}
uintptr_t MGL_EXPORT mgl_create_data_file_(const char *fname,int l)
{	char *s=new char[l+1];	memcpy(s,fname,l);	s[l]=0;
	uintptr_t r = uintptr_t(new mglData(s));	delete []s;	return r;	}
void MGL_EXPORT mgl_delete_data_(uintptr_t *d)
{	if(_DT_)	delete _DT_;	}
//-----------------------------------------------------------------------------
void mglFromStr(HMDT d,char *buf,long NX,long NY,long NZ)	// TODO: add multithreading read
{
	if(NX<1 || NY <1 || NZ<1)	return;
	mgl_data_create(d, NX,NY,NZ);
	long nb = strlen(buf);
	register long i=0, j=0;
	setlocale(LC_NUMERIC, "C");
	while(j<nb)
	{
		while(buf[j]<=' ' && j<nb)	j++;
		while(buf[j]=='#')		// skip comment
		{
			if(i>0 || buf[j+1]!='#')	// this is columns id
				while(!isn(buf[j]) && j<nb)	j++;
			else
			{
				while(!isn(buf[j]) && j<nb)
				{
					if(buf[j]>='a' && buf[j]<='z')
						d->id.push_back(buf[j]);
					j++;
				}
			}
			while(buf[j]<=' ' && j<nb)	j++;
		}
		char *s=buf+j;
		while(buf[j]>' ' && buf[j]!=',' && buf[j]!=';' && j<nb)	j++;
		buf[j]=0;
		d->a[i] = strstr(s,"NAN")?NAN:atof(s);
		i++;	if(i>=NX*NY*NZ)	break;
	}
	setlocale(LC_NUMERIC, "");
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set(HMDT d, HCDT a)
{
	if(!a)	return;
	const mglData *dd = dynamic_cast<const mglData *>(a);	// faster for mglData
	mgl_data_create(d, a->GetNx(), a->GetNy(), a->GetNz());
	if(dd)	// this one should be much faster
		memcpy(d->a, dd->a, d->nx*d->ny*d->nz*sizeof(mreal));
	else	// very inefficient!!!
#pragma omp parallel for collapse(3)
		for(long k=0;k<d->nz;k++)	for(long j=0;j<d->ny;j++)	for(long i=0;i<d->nx;i++)
			d->a[i+d->nx*(j+d->ny*k)] = a->v(i,j,k);
}
void MGL_EXPORT mgl_data_set_(uintptr_t *d, uintptr_t *a)	{	mgl_data_set(_DT_,_DA_(a));	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_values(HMDT d, const char *v,long NX,long NY,long NZ)
{
	if(NX<1 || NY <1 || NZ<1)	return;
	register long n=strlen(v)+1;
	char *buf = new char[n];
	memcpy(buf,v,n);
	mglFromStr(d,buf,NX,NY,NZ);
	delete []buf;
}
void MGL_EXPORT mgl_data_set_values_(uintptr_t *d, const char *val, int *nx, int *ny, int *nz, int l)
{	char *s=new char[l+1];	memcpy(s,val,l);	s[l]=0;
	mgl_data_set_values(_DT_,s,*nx,*ny,*nz);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_vector(HMDT d, gsl_vector *v)
{
#if MGL_HAVE_GSL
	if(!v || v->size<1)	return;
	mgl_data_create(d, v->size,1,1);
#pragma omp parallel for
	for(long i=0;i<d->nx;i++)	d->a[i] = v->data[i*v->stride];
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_matrix(HMDT d, gsl_matrix *m)
{
#if MGL_HAVE_GSL
	if(!m || m->size1<1 || m->size2<1)	return;
	mgl_data_create(d, m->size1,m->size2,1);
#pragma omp parallel for collapse(2)
	for(long j=0;j<d->ny;j++)	for(long i=0;i<d->nx;i++)
		d->a[i+j*d->nx] = m->data[i * m->tda + j];
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_float(HMDT d, const float *A,long NX,long NY,long NZ)
{
	if(NX<=0 || NY<=0 || NZ<=0)	return;
	mgl_data_create(d, NX,NY,NZ);	if(!A)	return;
#if MGL_USE_DOUBLE
#pragma omp parallel for
	for(long i=0;i<NX*NY*NZ;i++)	d->a[i] = A[i];
#else
	memcpy(d->a,A,NX*NY*NZ*sizeof(float));
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_double(HMDT d, const double *A,long NX,long NY,long NZ)
{
	if(NX<=0 || NY<=0 || NZ<=0)	return;
	mgl_data_create(d, NX,NY,NZ);	if(!A)	return;
#if MGL_USE_DOUBLE
	memcpy(d->a,A,NX*NY*NZ*sizeof(double));
#else
#pragma omp parallel for
	for(long i=0;i<NX*NY*NZ;i++)	d->a[i] = A[i];
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_float2(HMDT d, float const * const *A,long N1,long N2)
{
	if(N1<=0 || N2<=0)	return;
	mgl_data_create(d, N2,N1,1);	if(!A)	return;
#if MGL_USE_DOUBLE
#pragma omp parallel for collapse(2)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)	d->a[j+i*N2] = A[i][j];
#else
#pragma omp parallel for
	for(long i=0;i<N1;i++)	memcpy(d->a+i*N2,A[i],N2*sizeof(float));
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_double2(HMDT d, double const *const *A,long N1,long N2)
{
	if(N1<=0 || N2<=0)	return;
	mgl_data_create(d, N2,N1,1);	if(!A)	return;
#if MGL_USE_DOUBLE
#pragma omp parallel for
	for(long i=0;i<N1;i++)	memcpy(d->a+i*N2,A[i],N2*sizeof(double));
#else
#pragma omp parallel for collapse(2)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)	d->a[j+i*N2] = A[i][j];
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_float3(HMDT d, float const * const * const *A,long N1,long N2,long N3)
{
	if(N1<=0 || N2<=0 || N3<=0)	return;
	mgl_data_create(d, N3,N2,N1);	if(!A)	return;
#if MGL_USE_DOUBLE
#pragma omp parallel for collapse(3)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)	for(long k=0;k<N3;k++)
		d->a[k+N3*(j+i*N2)] = A[i][j][k];
#else
#pragma omp parallel for collapse(2)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)
		memcpy(d->a+N3*(j+i*N2),A[i][j],N3*sizeof(float));
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_double3(HMDT d, double const * const * const *A,long N1,long N2,long N3)
{
	if(N1<=0 || N2<=0 || N3<=0)	return;
	mgl_data_create(d, N3,N2,N1);	if(!A)	return;
#if MGL_USE_DOUBLE
#pragma omp parallel for collapse(2)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)
		memcpy(d->a+N3*(j+i*N2),A[i][j],N3*sizeof(double));
#else
#pragma omp parallel for collapse(3)
	for(long i=0;i<N1;i++)	for(long j=0;j<N2;j++)	for(long k=0;k<N3;k++)
		d->a[k+N3*(j+i*N2)] = A[i][j][k];
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_float1_(uintptr_t *d, const float *A,int *NX)
{	mgl_data_set_float(_DT_,A,*NX,1,1);	}
void MGL_EXPORT mgl_data_set_double1_(uintptr_t *d, const double *A,int *NX)
{	mgl_data_set_double(_DT_,A,*NX,1,1);	}
void MGL_EXPORT mgl_data_set_float_(uintptr_t *d, const float *A,int *NX,int *NY,int *NZ)
{	mgl_data_set_float(_DT_,A,*NX,*NY,*NZ);	}
void MGL_EXPORT mgl_data_set_double_(uintptr_t *d, const double *A,int *NX,int *NY,int *NZ)
{	mgl_data_set_double(_DT_,A,*NX,*NY,*NZ);	}
void MGL_EXPORT mgl_data_set_float2_(uintptr_t *d, const float *A,int *N1,int *N2)
{	mgl_data_set_float(_DT_,A,*N1,*N2,1);	}
void MGL_EXPORT mgl_data_set_double2_(uintptr_t *d, const double *A,int *N1,int *N2)
{	mgl_data_set_double(_DT_,A,*N1,*N2,1);	}
void MGL_EXPORT mgl_data_set_float3_(uintptr_t *d, const float *A,int *N1,int *N2,int *N3)
{	mgl_data_set_float(_DT_,A,*N1,*N2,*N3);	}
void MGL_EXPORT mgl_data_set_double3_(uintptr_t *d, const double *A,int *N1,int *N2,int *N3)
{	mgl_data_set_double(_DT_,A,*N1,*N2,*N3);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_rearrange(HMDT d, long mx,long my,long mz)
{
	if(mx<1)	return;	// wrong mx
	if(my<1)	{	my = d->nx*d->ny*d->nz/mx;	mz = 1;	}
	else if(mz<1)	mz = (d->nx*d->ny*d->nz)/(mx*my);
	long m = mx*my*mz;
	if(m==0 || m>d->nx*d->ny*d->nz)	return;	// too high desired dimensions
	d->nx = mx;	d->ny = my;	d->nz = mz;	d->NewId();
}
void MGL_EXPORT mgl_data_rearrange_(uintptr_t *d, int *mx, int *my, int *mz)
{	mgl_data_rearrange(_DT_,*mx,*my,*mz);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_set_id(HMDT d, const char *ids)	{	d->id = ids;	}
void MGL_EXPORT mgl_data_set_id_(uintptr_t *d, const char *eq,int l)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	mgl_data_set_id(_DT_, s);	delete []s;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_save(HCDT d, const char *fname,long ns)
{
	FILE *fp = fopen(fname,"w");
	if(!fp)	return;
	register long i,j,k;
	long nx=d->GetNx(), ny=d->GetNy(), nz=d->GetNz();
	setlocale(LC_NUMERIC, "C");
	if(ns<0 || (ns>=nz && nz>1))	for(k=0;k<nz;k++)
	{	// save whole data
		const mglData *dr = dynamic_cast<const mglData *>(d);
		if(dr && !dr->id.empty())	fprintf(fp,"## %s\n",dr->id.c_str());
		const mglDataC *dc = dynamic_cast<const mglDataC *>(d);
		if(dc && !dc->id.empty())	fprintf(fp,"## %s\n",dc->id.c_str());
		for(i=0;i<ny;i++)
		{
			for(j=0;j<nx-1;j++)	fprintf(fp,"%g\t",d->v(j,i,k));
			fprintf(fp,"%g\n",d->v(nx-1,i,k));
		}
		fprintf(fp,"\n");
	}
	else
	{	// save selected slice
		if(nz>1)	for(i=0;i<ny;i++)
		{
			for(j=0;j<nx-1;j++)	fprintf(fp,"%g\t",d->v(j,i,ns));
			fprintf(fp,"%g\n",d->v(nx-1,i,ns));
		}
		else if(ns<ny)	for(j=0;j<nx;j++)
			fprintf(fp,"%g\t",d->v(j,ns));
	}
	setlocale(LC_NUMERIC, "");
	fclose(fp);
}
void MGL_EXPORT mgl_data_save_(uintptr_t *d, const char *fname,int *ns,int l)
{	char *s=new char[l+1];	memcpy(s,fname,l);	s[l]=0;
	mgl_data_save(_DT_,s,*ns);		delete []s;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT char *mgl_read_gz(gzFile fp)
{
	long size=1024,n=0,m;
	char *buf=(char*)malloc(size);
	while((m=gzread(fp,buf+size*n,size))>0)
	{
		if(m<size)	{	buf[size*n+m]=0;	break;	}
		n++;		buf=(char*)realloc(buf,size*(n+1));
	}
	return buf;
}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read(HMDT d, const char *fname)
{
	long l=1,m=1,k=1;
	long nb,i;
	gzFile fp = gzopen(fname,"r");
	if(!fp)
	{
		if(!d->a)	mgl_data_create(d, 1,1,1);
		return	false;
	}
	char *buf = mgl_read_gz(fp);
	nb = strlen(buf);	gzclose(fp);

	bool first=false;
	register char ch;
	for(i=nb-1;i>=0;i--)	if(buf[i]>' ')	break;
	buf[i+1]=0;	nb = i;		// remove tailing spaces
	for(i=0;i<nb-1 && !isn(buf[i]);i++)	// determine nx
	{
		while(buf[i]=='#')	{	while(!isn(buf[i]) && i<nb)	i++;	}
		ch = buf[i];
		if(ch>' ' && !first)	first=true;
		if(first && (ch==' ' || ch=='\t' || ch==',' || ch==';') && buf[i+1]>' ') k++;
	}
	first = false;
	for(i=0;i<nb-1;i++)					// determine ny
	{
		ch = buf[i];
		if(ch=='#')	while(!isn(buf[i]) && i<nb)	i++;
		if(isn(ch))
		{
			while(buf[i+1]==' ' || buf[i+1]=='\t') i++;
			if(isn(buf[i+1]))	{first=true;	break;	}
			m++;
		}
		if(ch=='\f')	break;
	}
	if(first)	for(i=0;i<nb-1;i++)		// determine nz
	{
		ch = buf[i];
		if(ch=='#')	while(!isn(buf[i]) && i<nb)	i++;
//		if(ch=='#')	com = true;	// comment
		if(isn(ch))
		{
//			if(com)	{	com=false;	continue;	}
			while(buf[i+1]==' ' || buf[i+1]=='\t') i++;
			if(isn(buf[i+1]))	l++;
		}
	}
	else	for(i=0;i<nb-1;i++)	if(buf[i]=='\f')	l++;
	mglFromStr(d,buf,k,m,l);
	free(buf);	return true;
}
int MGL_EXPORT mgl_data_read_(uintptr_t *d, const char *fname,int l)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	int r = mgl_data_read(_DT_, s);	delete []s;		return r;	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_create(HMDT d,long mx,long my,long mz)
{
	d->nx = mx>0 ? mx:1;	d->ny = my>0 ? my:1;	d->nz = mz>0 ? mz:1;
	if(d->a && !d->link)	delete [](d->a);
	d->a = new mreal[d->nx*d->ny*d->nz];
	d->id.clear();	d->link=false;
	memset(d->a,0,d->nx*d->ny*d->nz*sizeof(mreal));
}
void MGL_EXPORT mgl_data_create_(uintptr_t *d, int *nx,int *ny,int *nz)
{	mgl_data_create(_DT_,*nx,*ny,*nz);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_link(HMDT d, mreal *A, long mx,long my,long mz)
{
	if(!A)	return;
	if(!d->link && d->a)	delete [](d->a);
	d->nx = mx>0 ? mx:1;	d->ny = my>0 ? my:1;	d->nz = mz>0 ? mz:1;
	d->link=true;	d->a=A;	d->NewId();
}
void MGL_EXPORT mgl_data_link_(uintptr_t *d, mreal *A, int *nx,int *ny,int *nz)
{	mgl_data_link(_DT_,A,*nx,*ny,*nz);	}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_dim(HMDT d, const char *fname,long mx,long my,long mz)
{
	if(mx<=0 || my<=0 || mz<=0)	return false;
	gzFile fp = gzopen(fname,"r");
	if(!fp)	return false;
	char *buf = mgl_read_gz(fp);
	gzclose(fp);
	mglFromStr(d,buf,mx,my,mz);
	free(buf);	return true;
}
int MGL_EXPORT mgl_data_read_dim_(uintptr_t *d, const char *fname,int *mx,int *my,int *mz,int l)
{	char *s=new char[l+1];	memcpy(s,fname,l);	s[l]=0;
	int r = mgl_data_read_dim(_DT_,s,*mx,*my,*mz);	delete []s;	return r;	}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_mat(HMDT d, const char *fname, long dim)
{
	if(dim<=0 || dim>3)	return false;
	gzFile fp = gzopen(fname,"r");
	if(!fp)	return false;
	long nx=1, ny=1, nz=1;
	char *buf = mgl_read_gz(fp);
	long nb = strlen(buf);	gzclose(fp);

	register long j=0,i,l;
	while(j<nb)
	{
		if(buf[j]=='#')	while(!isn(buf[j]))	j++;	// skip comment
		while(buf[j]<=' ' && j<nb)	j++;
		break;
	}
	if(dim==1)
	{
		sscanf(buf+j,"%ld",&nx);
		while(buf[j]!='\n' && j<nb)	j++;	j++;
//		while(buf[j]>' ')	j++;
	}
	else if(dim==2)
	{
		sscanf(buf+j,"%ld%ld",&nx,&ny);
		while(buf[j]!='\n' && j<nb)	j++;	j++;
		char *b=buf+j, ch;
		for(i=l=0;b[i];i++)
		{
			while(b[i]=='#')	{	while(!isn(b[i]) && b[i])	i++;	}
			if(b[i]=='\n')	l++;
		}
		if(l==nx*ny || l==nx*ny+1)	// try to read 3d data (i.e. columns of matrix nx*ny)
		{
			nz=ny;	ny=nx;	nx=1;
			bool first = false;
			for(i=l=0;b[i] && !isn(b[i]);i++)	// determine nx
			{
				while(b[i]=='#')	{	while(!isn(b[i]) && b[i])	i++;	}
				ch = b[i];
				if(ch>' ' && !first)	first=true;
				if(first && (ch==' ' || ch=='\t' || ch==',' || ch==';') && b[i+1]>' ') nx++;
			}
		}
	}
	else if(dim==3)
	{
		sscanf(buf+j,"%ld%ld%ld",&nx,&ny,&nz);
		while(buf[j]!='\n' && j<nb)	j++;	j++;
/*		while(buf[j]>' ' && j<nb)	j++;
		while(buf[j]<=' ' && j<nb)	j++;
		while(buf[j]>' ' && j<nb)	j++;
		while(buf[j]<=' ' && j<nb)	j++;
		while(buf[j]>' ' && j<nb)	j++;*/
	}
	mglFromStr(d,buf+j,nx,ny,nz);
	free(buf);	return true;
}
int MGL_EXPORT mgl_data_read_mat_(uintptr_t *d, const char *fname,int *dim,int l)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	int r = mgl_data_read_mat(_DT_,s,*dim);	delete []s;	return r;	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_max(HCDT d)
{
	mreal m1=-INFINITY;
	long nn=d->GetNN();
	const mglData *b = dynamic_cast<const mglData *>(d);
#pragma omp parallel
	{
		register mreal m=-INFINITY, v;
		if(b)
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = b->a[i];	m = m<v ? v:m;	}
		else
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = d->vthr(i);	m = m<v ? v:m;	}
#pragma omp critical(max_dat)
		{	m1 = m1>m ? m1:m;	}
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_max_(uintptr_t *d)	{	return mgl_data_max(_DT_);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_min(HCDT d)
{
	mreal m1=INFINITY;
	long nn=d->GetNN();
	const mglData *b = dynamic_cast<const mglData *>(d);
#pragma omp parallel
	{
		register mreal m=INFINITY, v;
		if(b)
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = b->a[i];	m = m>v ? v:m;	}
		else
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = d->vthr(i);	m = m>v ? v:m;	}
#pragma omp critical(min_dat)
		{	m1 = m1<m ? m1:m;	}
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_min_(uintptr_t *d)	{	return mgl_data_min(_DT_);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_neg_max(HCDT d)
{
	mreal m1=0;
	long nn=d->GetNN();
	const mglData *b = dynamic_cast<const mglData *>(d);
#pragma omp parallel
	{
		register mreal m=0, v;
		if(b)
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = b->a[i];	m = m<v && v<0 ? v:m;	}
		else
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = d->vthr(i);	m = m<v && v<0 ? v:m;	}
#pragma omp critical(max_dat)
		{	m1 = m1>m ? m1:m;	}
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_neg_max_(uintptr_t *d)	{	return mgl_data_neg_max(_DT_);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_pos_min(HCDT d)
{
	mreal m1=INFINITY;
	long nn=d->GetNN();
	const mglData *b = dynamic_cast<const mglData *>(d);
#pragma omp parallel
	{
		register mreal m=INFINITY, v;
		if(b)
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = b->a[i];	m = m>v && v>0 ? v:m;	}
		else
#pragma omp for nowait
			for(long i=0;i<nn;i++)
			{	v = d->vthr(i);	m = m>v && v>0 ? v:m;	}
#pragma omp critical(min_dat)
		{	m1 = m1<m ? m1:m;	}
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_pos_min_(uintptr_t *d)	{	return mgl_data_pos_min(_DT_);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_max_int(HCDT d, long *i, long *j, long *k)
{
	mreal m1=-INFINITY;
	long nx=d->GetNx(), ny=d->GetNy(), nn=d->GetNN();
#pragma omp parallel
	{
		register mreal m=-INFINITY, v;
		long im=-1,jm=-1,km=-1;
#pragma omp for nowait
		for(long ii=0;ii<nn;ii++)
		{
			v = d->vthr(ii);
			if(m < v)
			{	m=v;	im=ii%nx;	jm=(ii/nx)%ny;	km=ii/(nx*ny);   }
		}
#pragma omp critical(max_int)
		if(m1 < m)	{	m1=m;	*i=im;	*j=jm;	*k=km;   }
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_max_int_(uintptr_t *d, int *i, int *j, int *k)
{	long ii,jj,kk;	mreal res=mgl_data_max_int(_DT_,&ii,&jj,&kk);
	*i=ii;	*j=jj;	*k=kk;	return res;	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_min_int(HCDT d, long *i, long *j, long *k)
{
	mreal m1=INFINITY;
	long nx=d->GetNx(), ny=d->GetNy(), nn=d->GetNN();
#pragma omp parallel
	{
		register mreal m=INFINITY, v;
		long im=-1,jm=-1,km=-1;
#pragma omp for nowait
		for(long ii=0;ii<nn;ii++)
		{
			v = d->vthr(ii);
			if(m > v)
			{	m=v;	im=ii%nx;	jm=(ii/nx)%ny;	km=ii/(nx*ny);   }
		}
#pragma omp critical(min_int)
		if(m1 > m)	{	m1=m;	*i=im;	*j=jm;	*k=km;   }
	}
	return m1;
}
mreal MGL_EXPORT mgl_data_min_int_(uintptr_t *d, int *i, int *j, int *k)
{	long ii,jj,kk;	mreal res=mgl_data_min_int(_DT_,&ii,&jj,&kk);
	*i=ii;	*j=jj;	*k=kk;	return res;	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_max_real(HCDT d, mreal *x, mreal *y, mreal *z)
{
	long im=-1,jm=-1,km=-1;
	long nx=d->GetNx(), ny=d->GetNy(), nz=d->GetNz();
	mreal m=mgl_data_max_int(d,&im,&jm,&km), v, v1, v2;
	*x=im;	*y=jm;	*z=km;

	v = d->v(im,jm,km);
	if(nx>2)
	{
		if(im==0)	im=1;
		if(im==nx-1)im=nx-2;
		v1 = d->v(im+1,jm,km);	v2 = d->v(im-1,jm,km);
		*x = (v1+v2-2*v)==0 ? im : im+(v1-v2)/(v1+v2-2*v)/2;
	}
	if(ny>2)
	{
		if(jm==0)	jm=1;
		if(jm==ny-1)jm=ny-2;
		v1 = d->v(im,jm+1,km);	v2 = d->v(im,jm-1,km);
		*y = (v1+v2-2*v)==0 ? jm : jm+(v1-v2)/(v1+v2-2*v)/2;
	}
	if(nz>2)
	{
		if(km==0)	km=1;
		if(km==nz-1)km=nz-2;
		v1 = d->v(im,jm,km+1);	v2 = d->v(im,jm,km-1);
		*z = (v1+v2-2*v)==0 ? km : km+(v1-v2)/(v1+v2-2*v)/2;
	}
	return m;
}
mreal MGL_EXPORT mgl_data_max_real_(uintptr_t *d, mreal *x, mreal *y, mreal *z)
{	return mgl_data_max_real(_DT_,x,y,z);	}
//-----------------------------------------------------------------------------
mreal MGL_EXPORT mgl_data_min_real(HCDT d, mreal *x, mreal *y, mreal *z)
{
	long im=-1,jm=-1,km=-1;
	long nx=d->GetNx(), ny=d->GetNy(), nz=d->GetNz();
	mreal m=mgl_data_min_int(d,&im,&jm,&km), v, v1, v2;
	*x=im;	*y=jm;	*z=km;

	v = d->v(im,jm,km);
	if(nx>2)
	{
		if(im==0)	im=1;
		if(im==nx-1)im=nx-2;
		v1 = d->v(im+1,jm,km);	v2 = d->v(im-1,jm,km);
		*x = (v1+v2-2*v)==0 ? im : im+(v1-v2)/(v1+v2-2*v)/2;
	}
	if(ny>2)
	{
		if(jm==0)	jm=1;
		if(jm==ny-1)jm=ny-2;
		v1 = d->v(im,jm+1,km);	v2 = d->v(im,jm-1,km);
		*y = (v1+v2-2*v)==0 ? jm : jm+(v1-v2)/(v1+v2-2*v)/2;
	}
	if(nz>2)
	{
		if(km==0)	km=1;
		if(km==nz-1)km=nz-2;
		v1 = d->v(im,jm,km+1);	v2 = d->v(im,jm,km-1);
		*z = (v1+v2-2*v)==0 ? km : km+(v1-v2)/(v1+v2-2*v)/2;
	}
	return m;
}
mreal MGL_EXPORT mgl_data_min_real_(uintptr_t *d, mreal *x, mreal *y, mreal *z)
{	return mgl_data_min_real(_DT_,x,y,z);	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_fill_x(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	long nx=t->p[0],ny=t->p[1];
	mreal *b=t->a, x1=t->b[0], dx=t->b[1];
	register char dir = t->s[0];
	if(dir=='x')
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
		for(long i0=t->id;i0<t->n;i0+=mglNumThr)	b[i0] = x1+dx*(i0%nx);
	else if(dir=='y')
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
		for(long i0=t->id;i0<t->n;i0+=mglNumThr)	b[i0] = x1+dx*((i0/nx)%ny);
	else if(dir=='z')
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
		for(long i0=t->id;i0<t->n;i0+=mglNumThr)	b[i0] = x1+dx*(i0/(nx*ny));
	return 0;
}
void MGL_EXPORT mgl_data_fill(HMDT d, mreal x1,mreal x2,char dir)
{
	if(mgl_isnan(x2))	x2=x1;
	if(dir<'x' || dir>'z')	dir='x';
	long par[2]={d->nx,d->ny};
	mreal b[2]={x1,x2-x1};
	if(dir=='x')	b[1] *= d->nx>1 ? 1./(d->nx-1):0;
	if(dir=='y')	b[1] *= d->ny>1 ? 1./(d->ny-1):0;
	if(dir=='z')	b[1] *= d->nz>1 ? 1./(d->nz-1):0;
	mglStartThread(mgl_fill_x,0,d->nx*d->ny*d->nz,d->a,b,0,par,0,0,0,&dir);
}
void MGL_EXPORT mgl_data_fill_(uintptr_t *d, mreal *x1,mreal *x2,const char *dir,int)
{	mgl_data_fill(_DT_,*x1,*x2,*dir);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_norm(HMDT d, mreal v1,mreal v2,long sym,long dim)
{
	long s,nn=d->nx*d->ny*d->nz;
	mreal a1=INFINITY,a2=-INFINITY,v,*a=d->a;
	s = dim*d->ny*(d->nz>1 ? d->nx : 1);
	for(long i=s;i<nn;i++)	// determines borders of existing data
	{	a1 = a1>a[i] ? a[i]:a1;	a2 = a2<a[i] ? a[i]:a2;	}
	if(a1==a2)  {  if(a1!=0)	a1=0.;  else a2=1;  }
	if(v1>v2)	{	v=v1;	v1=v2;	v2=v;	}	// swap if uncorrect
	if(sym)				// use symmetric
	{
		v2 = -v1>v2 ? -v1:v2;	v1 = -v2;
		a2 = -a1>a2 ? -a1:a2;	a1 = -a2;
	}
	v2 = (v2-v1)/(a2-a1);	v1 = v1-a1*v2;
#pragma omp parallel for
	for(long i=s;i<nn;i++)	a[i] = v1 + v2*a[i];
}
void MGL_EXPORT mgl_data_norm_(uintptr_t *d, mreal *v1,mreal *v2,int *sym,int *dim)
{	mgl_data_norm(_DT_,*v1,*v2,*sym,*dim);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_squeeze(HMDT d, long rx,long ry,long rz,long smooth)
{
	long kx,ky,kz, nx=d->nx, ny=d->ny, nz=d->nz;
	mreal *b;

	// simple checking
	if(rx>=nx)	rx=nx-1;	if(rx<1)	rx=1;
	if(ry>=ny)	ry=ny-1;	if(ry<1)	ry=1;
	if(rz>=nz)	rz=nz-1;	if(rz<1)	rz=1;
	// new sizes
	kx = 1+(nx-1)/rx;	ky = 1+(ny-1)/ry;	kz = 1+(nz-1)/rz;
	b = new mreal[kx*ky*kz];
	if(!smooth)
#pragma omp parallel for collapse(3)
		for(long k=0;k<kz;k++)	for(long j=0;j<ky;j++)	for(long i=0;i<kx;i++)
			b[i+kx*(j+ky*k)] = d->a[i*rx+nx*(j*ry+ny*rz*k)];
	else
#pragma omp parallel for collapse(3)
		for(long k=0;k<kz;k++)	for(long j=0;j<ky;j++)	for(long i=0;i<kx;i++)
		{
			long dx,dy,dz,i1,j1,k1;
			dx = (i+1)*rx<=nx ? rx : nx-i*rx;
			dy = (j+1)*ry<=ny ? ry : ny-j*ry;
			dz = (k+1)*rz<=nz ? rz : nz-k*rz;
			mreal s = 0;
			for(k1=k*rz;k1<k*rz+dz;k1++)	for(j1=j*ry;j1<j*ry+dz;j1++)	for(i1=i*rx;i1<i*rx+dx;i1++)
				s += d->a[i1+nx*(j1+ny*k1)];
			b[i+kx*(j+ky*k)] = s/(dx*dy*dz);
		}
	if(!d->link)	delete [](d->a);
	d->a=b;	d->nx = kx;  d->ny = ky;  d->nz = kz;	d->NewId();	d->link=false;
}
void MGL_EXPORT mgl_data_squeeze_(uintptr_t *d, int *rx,int *ry,int *rz,int *smooth)
{	mgl_data_squeeze(_DT_,*rx,*ry,*rz,*smooth);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_extend(HMDT d, long n1, long n2)
{
	long nx=d->nx, ny=d->ny, nz=d->nz;
	if(nz>2 || n1==0)	return;
	long mx, my, mz;
	mreal *b=0;
	if(n1>0) // extend to higher dimension(s)
	{
		n2 = n2>0 ? n2:1;
		mx = nx;	my = ny>1?ny:n1;	mz = ny>1 ? n1 : n2;
		b = new mreal[mx*my*mz];
		if(ny>1)
#pragma omp parallel for
			for(long i=0;i<n1;i++)	memcpy(b+i*nx*ny, d->a, nx*ny*sizeof(mreal));
		else
#pragma omp parallel for
			for(long i=0;i<n1*n2;i++)	memcpy(b+i*nx, d->a, nx*sizeof(mreal));
	}
	else
	{
		mx = -n1;	my = n2<0 ? -n2 : nx;	mz = n2<0 ? nx : ny;
		if(n2>0 && ny==1)	mz = n2;
		b = new mreal[mx*my*mz];
		if(n2<0)
#pragma omp parallel for collapse(2)
			for(long j=0;j<nx;j++)	for(long i=0;i<mx*my;i++)
				b[i+mx*my*j] = d->a[j];
		else
#pragma omp parallel for collapse(2)
			for(long j=0;j<nx*ny;j++)	for(long i=0;i<mx;i++)
				b[i+mx*j] = d->a[j];
		if(n2>0 && ny==1)
#pragma omp parallel for
			for(long i=0;i<n2;i++)
				memcpy(b+i*mx*my, d->a, mx*my*sizeof(mreal));
	}
	if(!d->link)	delete [](d->a);
	d->a=b;	d->nx=mx;	d->ny=my;	d->nz=mz;
	d->NewId();		d->link=false;
}
void MGL_EXPORT mgl_data_extend_(uintptr_t *d, int *n1, int *n2)
{	mgl_data_extend(_DT_,*n1,*n2);	}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_transpose(HMDT d, const char *dim)
{
	long nx=d->nx, ny=d->ny, nz=d->nz, n;
	mreal *b=new mreal[nx*ny*nz], *a=d->a;
	if(!strcmp(dim,"xyz"))	memcpy(b,a,nx*ny*nz*sizeof(mreal));
	else if(!strcmp(dim,"xzy") || !strcmp(dim,"zy"))
	{
#pragma omp parallel for collapse(3)
		for(long j=0;j<ny;j++)	for(long k=0;k<nz;k++)	for(long i=0;i<nx;i++)
			b[i+nx*(k+nz*j)] = a[i+nx*(j+ny*k)];
		n=nz;	nz=ny;	ny=n;
	}
	else if(!strcmp(dim,"yxz") || !strcmp(dim,"yx"))
	{
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long i=0;i<nx;i++)	for(long j=0;j<ny;j++)
			b[j+ny*(i+nx*k)] = a[i+nx*(j+ny*k)];
		n=nx;	nx=ny;	ny=n;
	}
	else if(!strcmp(dim,"yzx"))
	{
#pragma omp parallel for collapse(3)
		for(long k=0;k<nz;k++)	for(long i=0;i<nx;i++)	for(long j=0;j<ny;j++)
			b[j+ny*(k+nz*i)] = a[i+nx*(j+ny*k)];
		n=nx;	nx=ny;	ny=nz;	nz=n;
	}
	else if(!strcmp(dim,"zxy"))
	{
#pragma omp parallel for collapse(3)
		for(long i=0;i<nx;i++)	for(long j=0;j<ny;j++)	for(long k=0;k<nz;k++)
			b[k+nz*(i+nx*j)] = a[i+nx*(j+ny*k)];
		n=nx;	nx=nz;	nz=ny;	ny=n;
	}
	else if(!strcmp(dim,"zyx") || !strcmp(dim,"zx"))
	{
#pragma omp parallel for collapse(3)
		for(long i=0;i<nx;i++)	for(long j=0;j<ny;j++)	for(long k=0;k<nz;k++)
			b[k+nz*(j+ny*i)] = a[i+nx*(j+ny*k)];
		n=nz;	nz=nx;	nx=n;
	}
	memcpy(a,b,nx*ny*nz*sizeof(mreal));	delete []b;
	n=d->nx;	d->nx=nx;	d->ny=ny;	d->nz=nz;
	if(nx!=n)	d->NewId();
}
void MGL_EXPORT mgl_data_transpose_(uintptr_t *d, const char *dim,int l)
{	char *s=new char[l+1];	memcpy(s,dim,l);	s[l]=0;
	mgl_data_transpose(_DT_,s);	delete []s;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_modify(void *par)
{
	mglThreadD *t=(mglThreadD *)par;
	const mglFormula *f = (const mglFormula *)(t->v);
	long nx=t->p[0],ny=t->p[1],nz=t->p[2];
	mreal *b=t->a, dx,dy,dz;
	const mreal *v=t->b, *w=t->c;
	dx=nx>1?1/(nx-1.):0;	dy=ny>1?1/(ny-1.):0;	dz=nz>1?1/(nz-1.):0;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(i*dx, j*dy, k*dz, b[i0], v?v[i0]:0, w?w[i0]:0);
	}
	return 0;
}
void MGL_EXPORT mgl_data_modify(HMDT d, const char *eq,long dim)
{
	long nx=d->nx, ny=d->ny, nz=d->nz, par[3]={nx,ny,nz};
	mglFormula f(eq);
	if(dim<0)	dim=0;
	if(nz>1)	// 3D array
	{
		par[2] -= dim;	if(par[2]<0)	par[2]=0;
		mglStartThread(mgl_modify,0,nx*ny*par[2],d->a+nx*ny*dim,0,0,par,&f);
	}
	else		// 2D or 1D array
	{
		par[1] -= dim;	if(par[1]<0)	par[1]=0;
		mglStartThread(mgl_modify,0,nx*par[1],d->a+nx*dim,0,0,par,&f);
	}
}
void MGL_EXPORT mgl_data_modify_(uintptr_t *d, const char *eq,int *dim,int l)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	mgl_data_modify(_DT_,s,*dim);	delete []s;	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_modify_gen(void *par)
{
	mglThreadV *t=(mglThreadV *)par;
	const mglFormula *f = (const mglFormula *)(t->v);
	register long nx=t->p[0],ny=t->p[1],nz=t->p[2];
	mreal *b=t->a, dx,dy,dz;
	HCDT v=(HCDT)t->b, w=(HCDT)t->c;
	dx=nx>1?1/(nx-1.):0;	dy=ny>1?1/(ny-1.):0;	dz=nz>1?1/(nz-1.):0;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i0=t->id;i0<t->n;i0+=mglNumThr)
	{
		register long i=i0%nx, j=((i0/nx)%ny), k=i0/(nx*ny);
		b[i0] = f->Calc(i*dx, j*dy, k*dz, b[i0], v?v->vthr(i0):0, w?w->vthr(i0):0);
	}
	return 0;
}
void MGL_EXPORT mgl_data_modify_vw(HMDT d, const char *eq,HCDT vdat,HCDT wdat)
{
	const mglData *v = dynamic_cast<const mglData *>(vdat);
	const mglData *w = dynamic_cast<const mglData *>(wdat);
	long nn = d->nx*d->ny*d->nz, par[3]={d->nx,d->ny,d->nz};
	if(vdat && vdat->GetNN()!=nn)	return;
	if(wdat && wdat->GetNN()!=nn)	return;
	mglFormula f(eq);
	if(v && w)	mglStartThread(mgl_modify,0,nn,d->a,v->a,w->a,par,&f);
	else if(vdat && wdat)	mglStartThreadV(mgl_modify_gen,nn,d->a,vdat,wdat,par,&f);
	else if(v)	mglStartThread(mgl_modify,0,nn,d->a,v->a,0,par,&f);
	else if(vdat)	mglStartThreadV(mgl_modify_gen,nn,d->a,vdat,0,par,&f);
	else	mglStartThread(mgl_modify,0,nn,d->a,0,0,par,&f);
}
void MGL_EXPORT mgl_data_modify_vw_(uintptr_t *d, const char *eq, uintptr_t *v, uintptr_t *w,int l)
{	char *s=new char[l+1];	memcpy(s,eq,l);	s[l]=0;
	mgl_data_modify_vw(_DT_,s,_DA_(v),_DA_(w));	delete []s;	}
//-----------------------------------------------------------------------------
#if MGL_HAVE_HDF4
int MGL_EXPORT mgl_data_read_hdf4(HMDT d,const char *fname,const char *data)
{
	int32 sd = SDstart(fname,DFACC_READ), nn, i;
	if(sd==-1)	return false;	// is not a HDF4 file
	char name[64];
	SDfileinfo(sd,&nn,&i);
	for(i=0;i<nn;i++)
	{
		int32 sds, rank, dims[32], type, attr, in[2]={0,0};
		sds = SDselect(sd,i);
		SDgetinfo(sds,name,&rank,dims,&type,&attr);
		if(!strcmp(name,data))	// as I understand there are possible many datas with the same name
		{
			if(rank==1)			{	dims[2]=dims[0];	dims[0]=dims[1]=1;	}
			else if(rank==2)	{	dims[2]=dims[1];	dims[1]=dims[0];	dims[0]=1;	}
//			else if(rank>3)		continue;
			long mm=dims[0]*dims[1]*dims[2];
			if(type==DFNT_FLOAT32)
			{
				float *b = new float[mm];
				SDreaddata(sds,in,0,dims,b);
				mgl_data_set_float(d,b,dims[2],dims[1],dims[0]);
				delete []b;
			}
			if(type==DFNT_FLOAT64)
			{
				double *b = new double[mm];
				SDreaddata(sds,in,0,dims,b);
				mgl_data_set_double(d,b,dims[2],dims[1],dims[0]);
				delete []b;
			}
		}
		SDendaccess(sds);
	}
	SDend(sd);
	return true;
}
#else
int MGL_EXPORT mgl_data_read_hdf4(HMDT ,const char *,const char *)
{	mglGlobalMess += "HDF4 support was disabled. Please, enable it and rebuild MathGL.\n";	return false;	}
#endif
//-----------------------------------------------------------------------------
#if MGL_HAVE_HDF5
void MGL_EXPORT mgl_data_save_hdf(HCDT dat,const char *fname,const char *data,int rewrite)
{
	const mglData *d = dynamic_cast<const mglData *>(dat);	// NOTE: only for mglData
	if(!d)	{	mglData d(dat);	mgl_data_save_hdf(&d,fname,data,rewrite);	return;	}
	hid_t hf,hd,hs;
	hsize_t dims[3];
	long rank = 3, res;
	H5Eset_auto(0,0);
	res=H5Fis_hdf5(fname);
	if(res>0 && !rewrite)	hf = H5Fopen(fname, H5F_ACC_RDWR, H5P_DEFAULT);
	else	hf = H5Fcreate(fname, H5F_ACC_TRUNC, H5P_DEFAULT, H5P_DEFAULT);
	if(hf<0)	return;
	if(d->nz==1 && d->ny == 1)	{	rank=1;	dims[0]=d->nx;	}
	else if(d->nz==1)	{	rank=2;	dims[0]=d->ny;	dims[1]=d->nx;	}
	else	{	rank=3;	dims[0]=d->nz;	dims[1]=d->ny;	dims[2]=d->nx;	}
	hs = H5Screate_simple(rank, dims, 0);
#if MGL_USE_DOUBLE
	hid_t mem_type_id = H5T_NATIVE_DOUBLE;
#else
	hid_t mem_type_id = H5T_NATIVE_FLOAT;
#endif
	hd = H5Dcreate(hf, data, mem_type_id, hs, H5P_DEFAULT);
	H5Dwrite(hd, mem_type_id, hs, hs, H5P_DEFAULT, d->a);
	H5Dclose(hd);	H5Sclose(hs);	H5Fclose(hf);
}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_hdf(HMDT d,const char *fname,const char *data)
{
	hid_t hf,hd,hs;
	hsize_t dims[3];
	long rank, res = H5Fis_hdf5(fname);
	if(res<=0)	return mgl_data_read_hdf4(d,fname,data);
	hf = H5Fopen(fname, H5F_ACC_RDONLY, H5P_DEFAULT);
	if(hf<0)	return false;
	hd = H5Dopen(hf,data);
	if(hd<0)	return false;
	hs = H5Dget_space(hd);
	rank = H5Sget_simple_extent_ndims(hs);
	if(rank>0 && rank<=3)
	{
		H5Sget_simple_extent_dims(hs,dims,0);
		if(rank==1)			{	dims[2]=dims[0];	dims[0]=dims[1]=1;	}
		else if(rank==2)	{	dims[2]=dims[1];	dims[1]=dims[0];	dims[0]=1;	}
//		else if(rank>3)		continue;
		mgl_data_create(d,dims[2],dims[1],dims[0]);
#if MGL_USE_DOUBLE
		H5Dread(hd, H5T_NATIVE_DOUBLE, H5S_ALL, H5S_ALL, H5P_DEFAULT, d->a);
#else
		H5Dread(hd, H5T_NATIVE_FLOAT, H5S_ALL, H5S_ALL, H5P_DEFAULT, d->a);
#endif
	}
	H5Sclose(hs);	H5Dclose(hd);	H5Fclose(hf);	return true;
}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_datas_hdf(const char *fname, char *buf, long size)
{
	hid_t hf,hg,hd,ht;
	if(!buf || size<1)	return 0;
	buf[0]=0;
	hf = H5Fopen(fname, H5F_ACC_RDONLY, H5P_DEFAULT);
	if(!hf)	return 0;
	hg = H5Gopen(hf,"/");
	hsize_t num, i;
	char name[256];
	long pos=0,len;
	H5Gget_num_objs(hg, &num);	// replace by H5G_info_t t; H5Gget_info(hg,&t); num=t.nlinks;
	for(i=0;i<num;i++)
	{
		if(H5Gget_objtype_by_idx(hg, i)!=H5G_DATASET)	continue;
		H5Gget_objname_by_idx(hg, i, name, 256);	// replace by H5Lget_name_by_idx(hg,".",i,0,0,name,256,0) ?!
		hd = H5Dopen(hf,name);
		ht = H5Dget_type(hd);
		len = strlen(name);		if(pos+len+2>size)	break;
		if(H5Tget_class(ht)==H5T_FLOAT || H5Tget_class(ht)==H5T_INTEGER)
		{	strcat(buf,name);	strcat(buf,"\t");	pos += len+1;	}
		H5Dclose(hd);	H5Tclose(ht);
	}
	H5Gclose(hg);	H5Fclose(hf);
	return i;
}
#else
void MGL_EXPORT mgl_data_save_hdf(HCDT ,const char *,const char *,int )
{	mglGlobalMess += "HDF5 support was disabled. Please, enable it and rebuild MathGL.\n";	}
int MGL_EXPORT mgl_datas_hdf(const char *, char *, long )
{	mglGlobalMess += "HDF5 support was disabled. Please, enable it and rebuild MathGL.\n";	return 0;}
int MGL_EXPORT mgl_data_read_hdf(HMDT ,const char *,const char *)
{	mglGlobalMess += "HDF5 support was disabled. Please, enable it and rebuild MathGL.\n";	return false;}
#endif
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_hdf_(uintptr_t *d, const char *fname, const char *data,int l,int n)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	char *t=new char[n+1];		memcpy(t,data,n);	t[n]=0;
	int r = mgl_data_read_hdf(_DT_,s,t);	delete []s;	delete []t;	return r;	}
void MGL_EXPORT mgl_data_save_hdf_(uintptr_t *d, const char *fname, const char *data, int *rewrite,int l,int n)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	char *t=new char[n+1];		memcpy(t,data,n);	t[n]=0;
	mgl_data_save_hdf(_DT_,s,t,*rewrite);	delete []s;	delete []t;	}
//-----------------------------------------------------------------------------
bool MGL_EXPORT mgl_add_file(long &kx,long &ky, long &kz, mreal *&b, mglData *d,bool as_slice)
{
	if(as_slice && d->nz==1)
	{
		if(kx==d->nx && d->ny==1)
		{
			b = (mreal *)realloc(b,kx*(ky+1)*sizeof(mreal));
			memcpy(b+kx*ky,d->a,kx*sizeof(mreal));		ky++;
		}
		else if(kx==d->nx && ky==d->ny)
		{
			b = (mreal *)realloc(b,kx*ky*(kz+1)*sizeof(mreal));
			memcpy(b+kx*ky*kz,d->a,kx*ky*sizeof(mreal));	kz++;
		}
		else	return false;
	}
	else
	{
		if(d->ny*d->nz==1 && ky*kz==1)
		{
			b = (mreal *)realloc(b,(kx+d->nx)*sizeof(mreal));
			memcpy(b+kx,d->a,d->nx*sizeof(mreal));	kx+=d->nx;
		}
		else if(kx==d->nx && kz==1 && d->nz==1)
		{
			b = (mreal *)realloc(b,kx*(ky+d->ny)*sizeof(mreal));
			memcpy(b+kx*ky,d->a,kx*d->ny*sizeof(mreal));	ky+=d->ny;
		}
		else if(kx==d->nx && ky==d->ny)
		{
			b = (mreal *)realloc(b,kx*kx*(kz+d->nz)*sizeof(mreal));
			memcpy(b+kx*ky*kz,d->a,kx*ky*d->nz*sizeof(mreal));	kz+=d->nz;
		}
		else	return false;
	}
	return true;
}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_range(HMDT dat, const char *templ, double from, double to, double step, int as_slice)
{
	mglData d;
	mreal t = from, *b;
	long kx,ky,kz,n=strlen(templ)+20;
	char *fname = new char[n];

	//read first file
	do{	snprintf(fname,n,templ,t);	t+= step;	} while(!mgl_data_read(&d,fname) && t<=to);

	if(t>to)	{	delete []fname;	return false;	}
	kx = d.nx;	ky = d.ny;	kz = d.nz;
	b = (mreal *)malloc(kx*ky*kz*sizeof(mreal));
	memcpy(b,d.a,kx*ky*kz*sizeof(mreal));

	// read other files
	for(;t<=to;t+=step)
	{
		snprintf(fname,n,templ,t);
		if(mgl_data_read(&d,fname))
			if(!mgl_add_file(kx,ky,kz,b,&d,as_slice))
			{	delete []fname;	free(b);	return false;	}
	}
	dat->Set(b,kx,ky,kz);
	delete []fname;	free(b);
	return true;
}
int MGL_EXPORT mgl_data_read_range_(uintptr_t *d, const char *fname, mreal *from, mreal *to, mreal *step, int *as_slice,int l)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	int r = mgl_data_read_range(_DT_,s,*from,*to,*step,*as_slice);	delete []s;	return r;	}
//-----------------------------------------------------------------------------
int MGL_EXPORT mgl_data_read_all(HMDT dat, const char *templ, int as_slice)
{
#ifndef WIN32
	mglData d;
	glob_t res;
	size_t i;
	mreal *b;
	long kx,ky,kz;
	glob (templ, GLOB_TILDE, NULL, &res);

	//read first file
	for(i=0;i<res.gl_pathc;i++)
		if(mgl_data_read(&d,res.gl_pathv[i]))	break;

	if(i>=res.gl_pathc)
	{	globfree (&res);	return false;	}
	kx = d.nx;	ky = d.ny;	kz = d.nz;
	b = (mreal *)malloc(kx*ky*kz*sizeof(mreal));
	memcpy(b,d.a,kx*ky*kz*sizeof(mreal));

	for(;i<res.gl_pathc;i++)
	{
		if(mgl_data_read(&d,res.gl_pathv[i]))
			if(!mgl_add_file(kx,ky,kz,b,&d,as_slice))
			{	globfree (&res);	free(b);	return false;	}
	}
	dat->Set(b,kx,ky,kz);

	globfree (&res);	free(b);
	return true;
#else
	return false;
#endif
}
int MGL_EXPORT mgl_data_read_all_(uintptr_t *d, const char *fname, int *as_slice,int l)
{	char *s=new char[l+1];		memcpy(s,fname,l);	s[l]=0;
	int r = mgl_data_read_all(_DT_,s,*as_slice);	delete []s;	return r;	}
//-----------------------------------------------------------------------------
