/***************************************************************************
 * data_png.cpp is part of Math Graphic Library
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
#include "mgl2/data.h"
#if MGL_HAVE_PNG
#include <png.h>
#endif
//-----------------------------------------------------------------------------
size_t MGL_NO_EXPORT mgl_col_dif(unsigned char *c1,unsigned char *c2,bool sum)
{
	size_t res,d1=abs(long(c1[0])-long(c2[0])),
		d2=abs(long(c1[1])-long(c2[1])),d3=abs(long(c1[2])-long(c2[2]));
	if(sum)	res = d1+d2+d3;
	else	res = mgl_max(d1,mgl_max(d2,d3));
	return res;
}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT unsigned char *mgl_create_scheme(const char *scheme,long &num)
{
	unsigned char *c=0,*cc=new unsigned char[3*strlen(scheme)+3],*c1,*c2;
	size_t nc=1,np=0,l=strlen(scheme);
	mglColor col;
	for(size_t i=0;i<l;i++)
	{
		col = mglColor(scheme[i]);
		if(col.Valid())
		{	cc[3*np]=255*col.r;	cc[3*np+1]=255*col.g;	cc[3*np+2]=255*col.b;	np++;	}
	}
	if(np<2)	{	delete []cc;	return 0;	}
	for(size_t i=0;i<np-1;i++)	nc+=mgl_col_dif(cc+3*i,cc+3*i+3,false);
	c = new unsigned char[3*nc+3];
	size_t dd,pos=0;
	for(size_t i=0;i<np-1;i++)
	{
		dd=mgl_col_dif(cc+3*i,cc+3*i+3,false);
		for(size_t j=0;j<dd;j++)
		{
			c1 = c+3*(pos+j);	c2 = cc+3*i;
			c1[0] = c2[0]+(c2[3]-c2[0])*j/dd;
			c1[1] = c2[1]+(c2[4]-c2[1])*j/dd;
			c1[2] = c2[2]+(c2[5]-c2[2])*j/dd;
		}
		pos += dd;
	}
	memcpy(c+3*nc-3,cc+3*np-3,3);
	delete []cc;
	num=nc;
	return c;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_import(HMDT d, const char *fname, const char *scheme,mreal v1,mreal v2)
{
#if MGL_HAVE_PNG
	if(v1>=v2)	return;
	long num=0;
	FILE *fp = fopen(fname, "rb");
	if (!fp)	return;
	png_structp png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, 0, 0, 0);
	if (!png_ptr)	{	fclose(fp);	return;	}
	png_infop info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr)
	{	png_destroy_read_struct(&png_ptr,0,0);	fclose(fp);	return;	}
	png_infop end_info = png_create_info_struct(png_ptr);
	if (!end_info)
	{	png_destroy_read_struct(&png_ptr,&info_ptr,0);	fclose(fp);	return;	}

	png_init_io(png_ptr, fp);
	png_read_png(png_ptr, info_ptr,
		PNG_TRANSFORM_STRIP_ALPHA|PNG_TRANSFORM_PACKING|
		PNG_TRANSFORM_STRIP_16|PNG_TRANSFORM_EXPAND,0);
	unsigned char **rows = png_get_rows(png_ptr, info_ptr);
	unsigned char *c = mgl_create_scheme(scheme,num);
	if(num>1)
	{
		long w=png_get_image_width(png_ptr, info_ptr);
		long h=png_get_image_height(png_ptr, info_ptr);
		d->Create(w,h,1);
#pragma omp parallel for collapse(2)
		for(long i=0;i<d->ny;i++)	for(long j=0;j<d->nx;j++)
		{
			size_t pos=0,val,mval=256;
			for(long k=0;k<num;k++)
			{
				val = mgl_col_dif(c+3*k,rows[d->ny-i-1]+3*j,true);
				if(val==0)	{	pos=k;	break;	}
				if(val<mval)	{	pos=k;	mval=val;	}
			}
			d->a[j+d->nx*i] = v1 + pos*(v2-v1)/num;
		}
	}
	png_destroy_read_struct(&png_ptr, &info_ptr,&end_info);
	fclose(fp);	delete []c;
#else
	mglGlobalMess += "PNG support was disabled. Please, enable it and rebuild MathGL.\n";
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_export(HCDT dd, const char *fname, const char *scheme,mreal v1,mreal v2,long ns)
{
#if MGL_HAVE_PNG
	long nx=dd->GetNx(), ny=dd->GetNy(), nz=dd->GetNz();
	const mglData *md = dynamic_cast<const mglData *>(dd);
	if(v1>v2)	return;
	if(ns<0 || ns>=nz)	ns=0;
	if(v1==v2)
	{
		v1 = INFINITY;	v2=-INFINITY;
		if(md)
//#pragma omp parallel for	// NOTE comparison here
			for(long i=0;i<nx*ny*nz;i++)
			{	register mreal vv = md->a[i];	if(vv<v1)	v1=vv;	if(vv>v2)	v2=vv;	}
		else
//#pragma omp parallel for	// NOTE comparison here
			for(long i=0;i<nx*ny*nz;i++)
			{	register mreal vv = dd->vthr(i);	if(vv<v1)	v1=vv;	if(vv>v2)	v2=vv;	}
	}
	if(v1==v2)	return;
	long num=0;
	unsigned char *c = mgl_create_scheme(scheme,num);
	if(num<2)	{	delete []c;		return;		}

	unsigned char **p = new unsigned char*[ny];
	unsigned char *d = new unsigned char[3*nx*ny];
#pragma omp parallel for
	for(long i=0;i<ny;i++)	p[i] = d+3*nx*(ny-1-i);
	if(md)
#pragma omp parallel for collapse(2)
		for(long i=0;i<ny;i++)	for(long j=0;j<nx;j++)
		{
			register mreal vv = md->a[j+nx*(i+ny*ns)];
			register long k = long(num*(vv-v1)/(v2-v1));
			if(k<0)	k=0;	if(k>=num) k=num-1;
			memcpy(d+3*(j+i*nx),c+3*k,3);
		}
	else
#pragma omp parallel for collapse(2)
		for(long i=0;i<ny;i++)	for(long j=0;j<nx;j++)
		{
			register long k = long(num*(dd->v(j,i,ns)-v1)/(v2-v1));
			if(k<0)	k=0;	if(k>=num) k=num-1;
			memcpy(d+3*(j+i*nx),c+3*k,3);
		}
	delete []c;

	FILE *fp = fopen(fname, "wb");
	if (!fp)	{	delete []p;	delete []d;	return;	}
	png_structp png_ptr = png_create_write_struct(PNG_LIBPNG_VER_STRING, 0, 0, 0);
	if (!png_ptr)	{	delete []p;	delete []d;	fclose(fp);	return;	}
	png_infop info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr)
	{	png_destroy_write_struct(&png_ptr,0);	delete []p;	delete []d;	fclose(fp);	return;	}
	png_init_io(png_ptr, fp);
	png_set_filter(png_ptr, 0, PNG_ALL_FILTERS);
	png_set_compression_level(png_ptr, Z_BEST_COMPRESSION);
	png_set_IHDR(png_ptr, info_ptr, nx, ny, 8, PNG_COLOR_TYPE_RGB,
			PNG_INTERLACE_NONE, PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT);
	png_set_rows(png_ptr, info_ptr, p);
	png_write_png(png_ptr, info_ptr,  PNG_TRANSFORM_IDENTITY, 0);
	png_write_end(png_ptr, info_ptr);
	png_destroy_write_struct(&png_ptr, &info_ptr);
	fclose(fp);	delete []p;	delete []d;
#else
	mglGlobalMess += "PNG support was disabled. Please, enable it and rebuild MathGL.\n";
#endif
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_data_export_(uintptr_t *d, const char *fname, const char *scheme,mreal *v1,mreal *v2,int *ns,int l,int n)
{
	char *s=new char[l+1];	memcpy(s,fname,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,scheme,n);	f[n]=0;
	mgl_data_export(_DT_,s,f,*v1,*v2,*ns);
	delete []s;		delete []f;
}
void MGL_EXPORT mgl_data_import_(uintptr_t *d, const char *fname, const char *scheme,mreal *v1,mreal *v2,int l,int n)
{
	char *s=new char[l+1];	memcpy(s,fname,l);	s[l]=0;
	char *f=new char[n+1];	memcpy(f,scheme,n);	f[n]=0;
	mgl_data_import(_DT_,s,f,*v1,*v2);
	delete []s;		delete []f;
}
//-----------------------------------------------------------------------------
