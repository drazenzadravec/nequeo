/***************************************************************************
 * pixel.cpp is part of Math Graphic Library
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
#include <algorithm>
#include "mgl2/canvas.h"
#include "mgl2/thread.h"
//-----------------------------------------------------------------------------
void mglCanvas::SetSize(int w,int h)
{
	if(w<=0 || h<=0)	{	SetWarn(mglWarnSize,"SetSize");	return;	}
	Width = w;	Height = h;	Depth = long(sqrt(double(w*h)));
	if(G)	{	delete []G;	delete []C;	delete []Z;	delete []G4;delete []OI;	}
	G = new unsigned char[w*h*3];
	G4= new unsigned char[w*h*4];
	C = new unsigned char[w*h*12];
	Z = new float[w*h*3];	// only 3 planes
	OI= new int[w*h];
	InPlot(0,1,0,1,false);	Clf();
}
//-----------------------------------------------------------------------------
void mglDrawReg::set(mglCanvas *gr, int nx, int ny, int m)
{
	int mx = m%nx, my = m/nx;	ObjId = gr->ObjId;
	PDef = gr->mask;	angle = gr->MaskAn;
	x1 = gr->GetWidth()*mx/nx;		y1 = gr->GetHeight()-gr->GetHeight()*(my+1)/ny;
	x2 = gr->GetWidth()*(mx+1)/nx-1;	y2 = gr->GetHeight()-gr->GetHeight()*my/ny-1;
}
//-----------------------------------------------------------------------------
void mglCanvas::PutDrawReg(mglDrawReg *d, const mglCanvas *gr)
{
	if(!gr)	return;
	int dd = d->x2 - d->x1;
	register long i,j;
	for(j=d->y1;j<d->y2;j++)
	{
		i = d->x1+Width*(Height-1-j);
		memcpy(OI+i,gr->OI+i,dd*sizeof(int));
		memcpy(Z+3*i,gr->Z+3*i,3*dd*sizeof(float));
		memcpy(C+12*i,gr->C+12*i,12*dd);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::PostScale(const mglMatrix *M, mglPoint &p) const
{
	mglPoint q=p/(2*M->pf);
	p.x = M->x + q.x*M->b[0] + q.y*M->b[1] + q.z*M->b[2];
	p.y = M->y + q.x*M->b[3] + q.y*M->b[4] + q.z*M->b[5];
	p.z = M->z + q.x*M->b[6] + q.y*M->b[7] + q.z*M->b[8];
}
//-----------------------------------------------------------------------------
bool mglCanvas::ScalePoint(const mglMatrix *M, mglPoint &p, mglPoint &n, bool use_nan) const
{
	bool res = get(MGL_DISABLE_SCALE) || mglBase::ScalePoint(M,p,n,use_nan);
//	if(TernAxis&4)	return res;
	PostScale(M,p);

	mglPoint y=n;
	n.x = y.x*M->b[0] + y.y*M->b[1] + y.z*M->b[2];	// simpler for rotation only
	n.y = y.x*M->b[3] + y.y*M->b[4] + y.z*M->b[5];
	n.z = y.x*M->b[6] + y.y*M->b[7] + y.z*M->b[8];
	n.Normalize();
	return res;
}
//-----------------------------------------------------------------------------
long mglCanvas::ProjScale(int nf, long id, bool text)
{
	mglPoint pp = GetPntP(id), nn = GetPntN(id), q, p, n, u;
	if(mgl_isnan(pp.x))	return -1;
	q = RestorePnt(pp)/(2*B.pf);
	u = RestorePnt(nn,true);	u.Normalize();
	mreal w=B1.b[0]/2, h=B1.b[4]/2, d=B1.b[8]/2, xx=B1.x-w/2, yy=B1.y-h/2;
	if(nf==0)
	{	p.x = xx + q.x*w;	p.y = yy + q.y*h;	p.z = B1.z + q.z*d;	n = u;	}
	else if(nf==1)
	{	p.x = xx + q.x*w;	p.y = yy+h + q.z*h;	p.z = B1.z + q.y*d;	n = mglPoint(u.x,u.z,u.y);	}
	else if(nf==2)
	{	p.x = xx+w + q.z*w;	p.y = yy + q.y*h;	p.z = B1.z+ q.x*d;	n = mglPoint(u.z,u.y,u.x);	}
	else
	{	p.x = xx+w + q.x*B.b[0]/2 + q.y*B.b[1]/2 + q.z*B.b[2]/2;	n = nn;
		p.y = yy+h + q.x*B.b[3]/2 + q.y*B.b[4]/2 + q.z*B.b[5]/2;
		p.z = B.z + q.x*B.b[6]/2 + q.y*B.b[7]/2 + q.z*B.b[8]/2;	}
	return CopyProj(id,p,text?n:nn);
}
//-----------------------------------------------------------------------------
void mglCanvas::LightScale(const mglMatrix *M)
{
//#pragma omp parallel for
	for(long i=0;i<10;i++)
	{
		if(!light[i].n)	continue;
		light[i].p=light[i].d;	light[i].q=light[i].r;
		ScalePoint(M,light[i].q,light[i].p,false);
		light[i].p /= light[i].p.norm();
	}
}
//-----------------------------------------------------------------------------
// NOTE: Perspective is not fully supported now !!! Also it use LAST InPlot parameters!!!
mglPoint mglCanvas::RestorePnt(mglPoint ps, bool norm) const
{
	mreal s3 = 2*B.pf;
	mglPoint p;

	mreal W=Width/2, H=Height/2, D=Depth/2;
	mreal cx = B.z*Bp.b[2]+B.y*Bp.b[1]+B.x*Bp.b[0]-Bp.x*W-Bp.b[0]*W+W-Bp.b[1]*H-Bp.b[2]*D;
	mreal c0 = B.b[6]*Bp.b[2]+B.b[3]*Bp.b[1]+B.b[0]*Bp.b[0];
	mreal c1 = B.b[7]*Bp.b[2]+B.b[4]*Bp.b[1]+B.b[1]*Bp.b[0];
	mreal c2 = B.b[8]*Bp.b[2]+B.b[5]*Bp.b[1]+B.b[2]*Bp.b[0];
	mreal cy = B.z*Bp.b[5]+B.y*Bp.b[4]+B.x*Bp.b[3]-Bp.b[3]*W-Bp.y*H-Bp.b[4]*H+H-Bp.b[5]*D;
	mreal c3 = B.b[6]*Bp.b[5]+B.b[3]*Bp.b[4]+B.b[0]*Bp.b[3];
	mreal c4 = B.b[7]*Bp.b[5]+B.b[4]*Bp.b[4]+B.b[1]*Bp.b[3];
	mreal c5 = B.b[8]*Bp.b[5]+B.b[5]*Bp.b[4]+B.b[2]*Bp.b[3];
	mreal cz = B.z*Bp.b[8]+B.y*Bp.b[7]+B.x*Bp.b[6]-Bp.b[6]*W-Bp.b[7]*H-Bp.z*D-Bp.b[8]*D+D;
	mreal c6 = B.b[6]*Bp.b[8]+B.b[3]*Bp.b[7]+B.b[0]*Bp.b[6];
	mreal c7 = B.b[7]*Bp.b[8]+B.b[4]*Bp.b[7]+B.b[1]*Bp.b[6];
	mreal c8 = B.b[8]*Bp.b[8]+B.b[5]*Bp.b[7]+B.b[2]*Bp.b[6];
	if(norm)	cx=cy=cz=0;

	if(ps.z==ps.z)	// try to take into account perspective if z-value is provided
	{
		register float d = (1-Bp.pf)/(1-Bp.pf*ps.z/Depth);
		ps.x = Width/2 + (ps.x-Width/2)/d;
		ps.y = Height/2+ (ps.y-Height/2)/d;
	}
	mreal xx = ps.x-cx, yy = ps.y-cy, zz = ps.z-cz;
	mreal d1=c0*c4-c1*c3, d2=c1*c5-c2*c4, d3=c0*c5-c2*c3;

	if(zz==zz)	// try to use z-values
	{
		// put inverse matrix here: [x,y,z]=B^(-1)[xx,yy,zz]
		mreal det = (-c0*c4*c8+c1*c3*c8+c0*c5*c7-c2*c3*c7-c1*c5*c6+c2*c4*c6)/s3;
		p.x = (c2*c4-c1*c5)*zz+(c1*c8-c2*c7)*yy+(c5*c7-c4*c8)*xx;	p.x /= det;
		p.y = (c0*c5-c2*c3)*zz+(c2*c6-c0*c8)*yy+(c3*c8-c5*c6)*xx;	p.y /= det;
		p.z = (c1*c3-c0*c4)*zz+(c0*c7-c1*c6)*yy+(c4*c6-c3*c7)*xx;	p.z /= det;
	}
	else if(fabs(d1) > fabs(d2) && fabs(d1) > fabs(d3))	// x-y plane
	{
		p.z = 0;
		p.x = s3*(c4*xx-c1*yy)/d1;
		p.y = s3*(c0*yy-c3*xx)/d1;
	}
	else if(fabs(d2) > fabs(d3))	// y-z
	{
		p.x = 0;
		p.y = s3*(c5*xx-c2*yy)/d2;
		p.z = s3*(c1*yy-c4*xx)/d2;
	}
	else	// x-z
	{
		p.y = 0;
		p.x = s3*(c5*xx-c2*yy)/d3;
		p.z = s3*(c0*yy-c3*xx)/d3;
	}
	return p;
}
//-----------------------------------------------------------------------------
mglPoint mglCanvas::CalcXYZ(int xs, int ys, bool real) const
{
	if(xs<0 || ys<0 || xs>=Width || ys>=Height)	return mglPoint(NAN,NAN,NAN);
	mglPoint p, ps(xs,Height-ys,NAN);
	float zz = Z[3*(xs+Width*(Height-1-ys))];
	if(zz>-1e20f)	{	ps.z = zz;	real=false;	}
	p = RestorePnt(ps);
	return real ? mglPoint(NAN,NAN,NAN) : mglPoint(Min.x + (Max.x-Min.x)*(p.x+1)/2,
				Min.y + (Max.y-Min.y)*(p.y+1)/2, Min.z + (Max.z-Min.z)*(p.z+1)/2);
}
//-----------------------------------------------------------------------------
void mglCanvas::CalcScr(mglPoint p, int *xs, int *ys) const
{
	mglPoint n;
	ScalePoint(GetB(),p,n);
	if(xs)	*xs=int(p.x);
	if(ys)	*ys=int(p.y);
}
//-----------------------------------------------------------------------------
mglPoint mglCanvas::CalcScr(mglPoint p) const
{	int x,y;	CalcScr(p,&x,&y);	return mglPoint(x,y);	}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT int mgl_type_prior[8]={1,2,4,5, 0,3,0, 7};
bool mglCreationOrder=false;
bool operator<(const mglPrim &a, const mglPrim &b)
{
	if(mglCreationOrder)	return a.n1<b.n1;
	register int t1 = mgl_type_prior[a.type], t2 = mgl_type_prior[b.type];
	if(a.z!=b.z) 	return a.z < b.z;
	if(t1!=t2)		return t1 > t2;
	if(a.w!=b.w) 	return a.w > b.w;
	return a.n3 > b.n3;
}
//-----------------------------------------------------------------------------
bool operator>(const mglPrim &a, const mglPrim &b)
{
	if(mglCreationOrder)	return a.n1>b.n1;
	register int t1 = mgl_type_prior[a.type], t2 = mgl_type_prior[b.type];
	if(a.z!=b.z) 	return a.z > b.z;
	if(t1!=t2)		return t1 < t2;
	if(a.w!=b.w) 	return a.w < b.w;
	return a.n3 < b.n3;
}
//-----------------------------------------------------------------------------
MGL_NO_EXPORT void *mgl_canvas_thr(void *par)
{	mglThreadG *t=(mglThreadG *)par;	(t->gr->*(t->f))(t->id, t->n, t->p);	return NULL;	}
void mglStartThread(void (mglCanvas::*func)(long i, long n, const void *p), mglCanvas *gr, long n, const void *p=NULL)
{
	if(!func || !gr)	return;
#if MGL_HAVE_PTHREAD
	if(mglNumThr<1)	mgl_set_num_thr(0);
	if(mglNumThr>1)
	{
		pthread_t *tmp=new pthread_t[mglNumThr];
		mglThreadG *par=new mglThreadG[mglNumThr];
		register long i;
		for(i=0;i<mglNumThr;i++)	// put parameters into the structure
		{	par[i].gr=gr;	par[i].f=func;	par[i].n=n;	par[i].p=p;	par[i].id=i;	}
		for(i=0;i<mglNumThr;i++)	pthread_create(tmp+i, 0, mgl_canvas_thr, par+i);
		for(i=0;i<mglNumThr;i++)	pthread_join(tmp[i], 0);
		delete []tmp;	delete []par;
	}
	else
#endif
	{	mglNumThr = 1;	(gr->*func)(0,n,p);	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_combine(long id, long n, const void *)
{
	unsigned char c[4],*cc;
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for private(c,cc)
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{	cc = C+12*i;		memcpy(c,BDef,4);
		combine(c,cc+8);	combine(c,cc+4);
		combine(c,cc);		memcpy(G4+4*i,c,4);	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_memcpy(long id, long n, const void *)
{
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<n;i+=mglNumThr)	memcpy(G4+4*i,C+12*i,4);
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_backgr(long id, long n, const void *)
{
	unsigned char c[4];
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for private(c)
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{	memcpy(c,BDef,4);	combine(c,G4+4*i);	memcpy(G+3*i,c,3);	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_transform(long id, long n, const void *)
{
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{
		mglPnt &p=Pnt[i];
		if(p.sub)	continue;
		register float x = p.xx-Width/2., y = p.yy-Height/2., z = p.zz-Depth/2.;
		p.x = Bp.b[0]*x + Bp.b[1]*y + Bp.b[2]*z - Bp.x*Width/2;
		p.y = Bp.b[3]*x + Bp.b[4]*y + Bp.b[5]*z - Bp.y*Height/2;
		p.z = Bp.b[6]*x + Bp.b[7]*y + Bp.b[8]*z + Depth/2.;
		register float d = (1-Bp.pf)/(1-Bp.pf*p.z/Depth);
		p.x = Width/2 + d*p.x;	p.y = Height/2 + d*p.y;
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_setz_adv(long id, long n, const void *)
{
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{
		mglPrim &q=Prm[i];	q.z = Pnt[q.n1].z;
		if(q.type==1)	q.z = (q.z + Pnt[q.n2].z)/2;
		if(q.type==2)	q.z = (q.z + Pnt[q.n2].z + Pnt[q.n3].z)/3;
		if(q.type==3)	q.z = (q.z + Pnt[q.n2].z + Pnt[q.n3].z + Pnt[q.n4].z)/4;
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_prmcol(long id, long n, const void *)
{
	prm_col.resize(n);
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<n;i+=mglNumThr)
		prm_col[i] = GetColor(Prm[i]);
}
//-----------------------------------------------------------------------------
uint32_t mglCanvas::GetColor(const mglPrim &p)
{
	mglRGBA res, c1,c2,c3,c4;
	c1.c=pnt_col[p.type==1?p.n2:p.n1];
	unsigned r=c1.r[0], g=c1.r[1], b=c1.r[2], a=c1.r[3];
	switch(p.type)
	{
	case 2:
		c2.c=pnt_col[p.n2];	c3.c=pnt_col[p.n3];
		res.r[0]=(r+c2.r[0]+c3.r[0])/3;
		res.r[1]=(g+c2.r[1]+c3.r[1])/3;
		res.r[2]=(b+c2.r[2]+c3.r[2])/3;
		res.r[3]=(a+c2.r[3]+c3.r[3])/3;	break;
	case 3:
		c2.c=pnt_col[p.n2];	c3.c=pnt_col[p.n3];	c4.c=pnt_col[p.n4];
		res.r[0]=(r+c2.r[0]+c3.r[0]+c4.r[0])/4;
		res.r[1]=(g+c2.r[1]+c3.r[1]+c4.r[1])/4;
		res.r[2]=(b+c2.r[2]+c3.r[2]+c4.r[2])/4;
		res.r[3]=(a+c2.r[3]+c3.r[3]+c4.r[3])/4;	break;
	case 6:
		res.r[0]=p.n2&0xff;	res.r[1]=(p.n2/256)&0xff;	res.r[2]=(p.n2/65536)&0xff;	res.r[3]=255;	break;
//		res.c=p.n2;	break;
	default:
		res.c = c1.c;	break;
	}
	// add fog into resulting color
	float zf = FogDist*(p.z/Depth-0.5-FogDz);
	if(zf<0)	// add fog
	{
		unsigned char d = (unsigned char)(255*(1.-exp(5*zf)));
		unsigned char cb[4] = {BDef[0], BDef[1], BDef[2], d};
		if(d<255)	combine(res.r,cb);
	}
	return res.c;
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_pntcol(long id, long n, const void *)
{
	mglRGBA c;
	pnt_col.resize(n);
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for private(c)
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{	col2int(Pnt[i],c.r,-1);	pnt_col[i]=c.c;	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_setz(long id, long n, const void *)
{
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{	mglPrim &q=Prm[i];	q.z = Pnt[q.n1].z;	}
}
//-----------------------------------------------------------------------------
void mglCanvas::PreparePrim(int fast)
{
	if(fast!=2)
	{
		mglStartThread(&mglCanvas::pxl_transform,this,Pnt.size());
		if(fast==0)	mglStartThread(&mglCanvas::pxl_setz,this,Prm.size());
		else	mglStartThread(&mglCanvas::pxl_setz_adv,this,Prm.size());
		mglCreationOrder = false;
		std::sort(Prm.begin(), Prm.end());
	}
	if(fast>0)
	{
		mglStartThread(&mglCanvas::pxl_pntcol,this,Pnt.size());
		mglStartThread(&mglCanvas::pxl_prmcol,this,Prm.size());
	}
}
//-----------------------------------------------------------------------------
void mglBase::resort()
{
#pragma omp critical
	{
		mglCreationOrder = true;
		std::sort(Prm.begin(), Prm.end());
		mglCreationOrder = false;
		clr(MGL_FINISHED);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_primdr(long id, long , const void *)
{
#define Q	4	// should be >= sqrt(2*num_thr) ???
	int nx=Q,ny=Q;		// TODO find dependence on Q for 1, 2, 4, 8 threads. Try to select optimal
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
	for(long i=id;i<nx*ny;i+=mglNumThr)
	{
		mglDrawReg d;	d.set(this,nx,ny,i);
		for(size_t k=0;k<Prm.size();k++)
		{
			if(Stop)	continue;
			const mglPrim &p=Prm[k];
			d.PDef = p.n3;	d.pPos = p.s;
			d.ObjId = p.id;	d.PenWidth=p.w;
			d.angle = p.angl;
			if(p.type==2 || p.type==3) d.PDef = p.m;
			switch(p.type)
			{
			case 0:	mark_draw(Pnt[p.n1],p.n4,p.s,&d);	break;
			case 1:	line_draw(Pnt[p.n1],Pnt[p.n2],&d);	break;
			case 2:	trig_draw(Pnt[p.n1],Pnt[p.n2],Pnt[p.n3],true,&d);	break;
			case 3:	quad_draw(Pnt[p.n1],Pnt[p.n2],Pnt[p.n3],Pnt[p.n4],&d);	break;
			case 4:	glyph_draw(p,&d);	break;
			}
		}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_primpx(long id, long n, const void *)	// NOTE this variant is too slow ... may be later in CUDA???
{
	mglDrawReg d;	d.set(this,1,1,id);
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for firstprivate(d)
#endif
	for(long ii=0;ii<n;ii+=mglNumThr)
	{
		register long i=ii%Width, j=ii/Width;
		for(size_t k=0;k<Prm.size();k++)
		{
			if(Stop)	continue;
			const mglPrim &p=Prm[k];
			d.PDef = p.n3;	d.pPos = p.s;
			d.ObjId = p.id;	d.PenWidth=p.w;
			d.angle = p.angl;
			if(p.type==2 || p.type==3) d.PDef = p.m;
			switch(p.type)
			{
			case 0:	mark_pix(i,j,Pnt[p.n1],p.n4,p.s,&d);	break;
			case 1:	line_pix(i,j,Pnt[p.n1],Pnt[p.n2],&d);	break;
			case 2:	trig_pix(i,j,Pnt[p.n1],Pnt[p.n2],Pnt[p.n3],true,&d);	break;
			case 3:	quad_pix(i,j,Pnt[p.n1],Pnt[p.n2],Pnt[p.n3],Pnt[p.n4],&d);	break;
			case 4:	glyph_pix(i,j,p,&d);	break;
			}
		}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_dotsdr(long id, long n, const void *)
{
	unsigned char r[4]={0,0,0,255};
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for firstprivate(r)
#endif
	for(long i=id;i<n;i+=mglNumThr)
	{
		const mglPnt &p=Pnt[i];
		if(p.sub)	continue;
		register float x = p.xx-Width/2., y = p.yy-Height/2., z = p.zz-Depth/2.,xx,yy,zz;
		xx = Bp.b[0]*x + Bp.b[1]*y + Bp.b[2]*z - Bp.x*Width/2;
		yy = Bp.b[3]*x + Bp.b[4]*y + Bp.b[5]*z - Bp.y*Height/2;
		zz = Bp.b[6]*x + Bp.b[7]*y + Bp.b[8]*z + Depth/2.;
		register float d = (1-Bp.pf)/(1-Bp.pf*zz/Depth);
		xx = Width/2 + d*xx;	yy = Height/2 + d*yy;

		r[0] = (unsigned char)(255*p.r);
		r[1] = (unsigned char)(255*p.g);
		r[2] = (unsigned char)(255*p.b);
		register long i0=long(xx)+Width*(Height-1-long(yy));
		if(i0>=0 && i0<Width*Height && zz>Z[3*i0])
		{	Z[3*i0]=z;	memcpy(C+12*i0,r,4);	OI[i0]=-1;	}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::Finish()
{
	if(Quality==MGL_DRAW_DOTS)
	{
		mglStartThread(&mglCanvas::pxl_dotsdr,this,Pnt.size());
		mglStartThread(&mglCanvas::pxl_memcpy,this,Width*Height);
		mglStartThread(&mglCanvas::pxl_backgr,this,Width*Height);
		return;
	}
	static mglMatrix bp;
	if(Quality==MGL_DRAW_NONE)	return;
	if(Quality&MGL_DRAW_LMEM)	clr(MGL_FINISHED);
	if(memcmp(&Bp,&bp,sizeof(mglMatrix)) && !(Quality&MGL_DRAW_LMEM) && Prm.size()>0)
		clr(MGL_FINISHED);
	if(get(MGL_FINISHED))	return;	// nothing to do
/*	static bool working=false;
	if(working)	return;
	working = true;*/
	if(!(Quality&MGL_DRAW_LMEM) && Prm.size()>0)
	{
		PreparePrim(0);	bp=Bp;
		clr(MGL_FINISHED);
		mglStartThread(&mglCanvas::pxl_primdr,this,Prm.size());
	}
	size_t n=Width*Height;
	BDef[3] = (Flag&3)!=2 ? 0:255;
	if(Quality&MGL_DRAW_NORM)	mglStartThread(&mglCanvas::pxl_combine,this,n);
	else 			mglStartThread(&mglCanvas::pxl_memcpy,this,n);
	BDef[3] = 255;
	mglStartThread(&mglCanvas::pxl_backgr,this,n);
	set(MGL_FINISHED);
//	working = false;
}
//-----------------------------------------------------------------------------
void mglCanvas::ClfZB(bool force)
{
	if(!force && (Quality&MGL_DRAW_LMEM))	return;
	register long n=Width*Height;
	memset(C,0,12*n);	memset(OI,0,n*sizeof(int));
#pragma omp parallel for
	for(long i=0;i<3*n;i++)	Z[i] = -1e20f;
	clr(MGL_FINISHED);
}
//-----------------------------------------------------------------------------
void mglCanvas::Clf(mglColor Back)
{
	Fog(0);		PDef = 0xffff;	pPos = 0;	StartAutoGroup(NULL);
	Pnt.clear();	Prm.clear();	Ptx.clear();	Glf.clear();
	Sub.clear();	Leg.clear();	Grp.clear();	Act.clear();
	pnt_col.clear();	prm_col.clear();

#pragma omp critical(txt)
	{
		Txt.clear();	Txt.reserve(3);
		mglTexture t1(MGL_DEF_PAL,-1), t2(MGL_DEF_SCH,1);
		MGL_PUSH(Txt,t1,mutexTxt);
		MGL_PUSH(Txt,t2,mutexTxt);
	}

//	if(Back==NC)		Back = mglColor(1,1,1);
	if((Flag&3)==2)	Back = mglColor(0,0,0);
	if(Back!=NC)
	{	BDef[0]=Back.r*255;	BDef[1]=Back.g*255;BDef[2]=Back.b*255;	BDef[3]=0;	}
	ClfZB(true);
}
//-----------------------------------------------------------------------------
void mglCanvas::pxl_other(long id, long n, const void *p)
{
	const mglCanvas *gr = (const mglCanvas *)p;
	if(!gr)	return;
	if(Quality&MGL_DRAW_NORM)
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
		for(long k=id;k<n;k+=mglNumThr)
		{
			register long i = k%Width, j = Height-1-(k/Width);
			pnt_plot(i,j,gr->Z[3*k+2],gr->C+12*k+8,gr->OI[k]);
			pnt_plot(i,j,gr->Z[3*k+1],gr->C+12*k+4,gr->OI[k]);
			pnt_plot(i,j,gr->Z[3*k],gr->C+12*k,gr->OI[k]);
		}
	else
#if !MGL_HAVE_PTHREAD
#pragma omp parallel for
#endif
		for(long k=id;k<n;k+=mglNumThr)
		{
			register long i = k%Width, j = Height-1-(k/Width);
			pnt_plot(i,j,gr->Z[3*k],gr->C+12*k,gr->OI[k]);
		}
}
//-----------------------------------------------------------------------------
void mglCanvas::Combine(const mglCanvas *gr)
{
	if(Width!=gr->Width || Height!=gr->Height)	return;	// wrong sizes
	mglStartThread(&mglCanvas::pxl_other,this,Width*Height,gr);
}
//-----------------------------------------------------------------------------
void mglCanvas::pnt_plot(long x,long y,mreal z,const unsigned char ci[4], int obj_id)
{
	long i0=x+Width*(Height-1-y);
	if(ci[3]==0)	return;
	unsigned char *cc = C+12*i0, c[4];
	memcpy(c,ci,4);
	float *zz = Z+3*i0, zf = FogDist*(z/Depth-0.5-FogDz);
	if(zf<0)	// add fog
	{
		int d = int(255.f-255.f*exp(5.f*zf));
		unsigned char cb[4] = {BDef[0], BDef[1], BDef[2], (unsigned char)d};
		if(d==255)	return;
		combine(c,cb);
	}
	if(Quality&MGL_DRAW_NORM)
	{
		if(z>=zz[1])	// shift point on slice down and paste new point
		{
			zz[2] = zz[1];	combine(cc+8,cc+4);
			if(z>=zz[0])
			{	zz[1] = zz[0];	zz[0] = z;	OI[i0]=obj_id;
				memcpy(cc+4,cc,4);	memcpy(cc,c,4);		}
			else	{	zz[1] = z;	memcpy(cc+4,c,4);	}
		}
		else
		{
			if(z>=zz[2])	// shift point on slice down and paste new point
			{	zz[2] = z;	combine(cc+8,c);	}
			else		// point below the background
			{	combine(c,cc+8);	memcpy(cc+8,c,4);	}
		}
	}
	else
	{
		if(z>=zz[0])	// point upper the background
		{	zz[0]=z;	memcpy(cc,c,4);		OI[i0]=obj_id;	}
	}
}
//-----------------------------------------------------------------------------
unsigned char* mglCanvas::col2int(const mglPnt &p,unsigned char *r, int obj_id)
{
	if(!r)	return r;
	if(p.a<=0)	{	memset(r,0,4);	return r;	}
	register float b0=0,b1=0,b2=0, ar,ag,ab;
	ar = ag = ab = AmbBr;

//	if(get(MGL_ENABLE_LIGHT) && mgl_isnum(p.u))
	if(mgl_isnum(p.u+p.v+p.w))
	{
		float d0,d1,d2,nn;
		register long i;
		for(i=0;i<10;i++)
		{
			const mglLight &ll=light[i];
			if(!ll.n)	continue;
			if(mgl_isnan(ll.q.x))		// source at infinity
			{
				nn = 2*(p.u*ll.p.x+p.v*ll.p.y+p.w*ll.p.z) / (p.u*p.u+p.v*p.v+p.w*p.w+1e-6);
				d0 = ll.p.x - p.u*nn;
				d1 = ll.p.y - p.v*nn;
				d2 = ll.p.z - p.w*nn;
				nn = 1 + d2/sqrt(d0*d0+d1*d1+d2*d2+1e-6);

				nn = exp(-ll.a*nn)*ll.b*2;
				b0 += nn*ll.c.r;
				b1 += nn*ll.c.g;
				b2 += nn*ll.c.b;
			}
			else		// diffuse and specular light
			{
				d0 = ll.q.x-p.x;	// direction to light source
				d1 = ll.q.y-p.y;
				d2 = ll.q.z-p.z;
				nn = 1+(d0*ll.p.x+d1*ll.p.y+d2*ll.p.z)/sqrt(d0*d0+d1*d1+d2*d2+1e-6);
				float bb = exp(-3*ll.a*nn);	nn = bb*DifBr*2;
				ar += nn*ll.c.r;
				ag += nn*ll.c.g;
				ab += nn*ll.c.b;

				nn = 2*(p.u*d0+p.v*d1+p.w*d2) / (p.u*p.u+p.v*p.v+p.w*p.w+1e-6);
				d0 -= p.u*nn;	d1 -= p.v*nn;	d2 -= p.w*nn;
				nn = 1 + d2/sqrt(d0*d0+d1*d1+d2*d2+1e-6);

				nn = exp(-ll.a*nn)*bb*ll.b*2;
				b0 += nn*ll.c.r;
				b1 += nn*ll.c.g;
				b2 += nn*ll.c.b;
			}
		}
		b0 += (ar>1 ? 1:ar)*p.r;	// diffuse light
		b1 += (ag>1 ? 1:ag)*p.g;
		b2 += (ab>1 ? 1:ab)*p.b;
		b0 = b0<1 ? b0 : 1;			// normalize components
		b1 = b1<1 ? b1 : 1;
		b2 = b2<1 ? b2 : 1;
	}
	else
	{	b0=p.r;	b1=p.g;	b2=p.b;	}
	// try to highlight faces
	if(obj_id==HighId)	{	b0*=0.7;	b1*=0.7;	b2*=0.7;	}
	r[0] = (unsigned char)(255*b0);
	r[1] = (unsigned char)(255*b1);
	r[2] = (unsigned char)(255*b2);
//	r[3] = get(MGL_ENABLE_ALPHA) ? (unsigned char)(255*p.a) : 255;
	r[3] = (unsigned char)((Quality&MGL_DRAW_NORM)?255*p.a:255);
	return r;
}
//-----------------------------------------------------------------------------
/// color mixing: color c1 is under color c2 !!!
void mglCanvas::combine(unsigned char *c1, const unsigned char *c2)
{
	if(!c2[3])	return;
	register unsigned int a1=c1[3], a2=c2[3],b1=255-a2;
	if(a1==0 || a2==255)	{	memcpy(c1,c2,4);	return; }
	if((Flag&3)==0)
	{
		c1[0] = (c1[0]*b1 + c2[0]*a2)/256;
		c1[1] = (c1[1]*b1 + c2[1]*a2)/256;
		c1[2] = (c1[2]*b1 + c2[2]*a2)/256;
		c1[3] = (unsigned char)(a2+a1*b1/255);
	}
	else if((Flag&3)==1)
	{
		c1[0] = (unsigned char)((255-a1*(255-c1[0])/256)*(255-a2*(255-c2[0])/256)/256);
		c1[1] = (unsigned char)((255-a1*(255-c1[1])/256)*(255-a2*(255-c2[1])/256)/256);
		c1[2] = (unsigned char)((255-a1*(255-c1[2])/256)*(255-a2*(255-c2[2])/256)/256);
		c1[3] = 255;
	}
	else if((Flag&3)==2)
	{
		unsigned int b2,b3;
		b1 = (c1[0]*a1 + c2[0]*a2)/256;		c1[0] = b1<255 ? b1 : 255;
		b2 = (c1[1]*a1 + c2[1]*a2)/256;		c1[1] = b2<255 ? b2 : 255;
		b3 = (c1[2]*a1 + c2[2]*a2)/256;		c1[2] = b3<255 ? b3 : 255;
		c1[3] = a1+a2>255? 255 : a1+a2;
	}
}
//-----------------------------------------------------------------------------
unsigned char **mglCanvas::GetRGBLines(long &w, long &h, unsigned char *&f, bool alpha)
{
	unsigned char **p;
	Finish();
	p = (unsigned char **)malloc(Height * sizeof(unsigned char *));
	long d = (alpha ? 4:3)*Width;
	unsigned char *gg = (alpha?G4:G);
#pragma omp parallel for
	for(long i=0;i<Height;i++)	p[i] = gg + d*i;
	w = Width;	h = Height;		f = 0;
	return p;
}
//-----------------------------------------------------------------------------
bool visible(long i, long j, const unsigned char m[8], mreal pw, int a)	// Check if pixel visible
{
	register float c = mgl_cos[(a+360)%360], s = mgl_cos[(a+450)%360];
	register int ii = long(0.5+(i*c+j*s)/pw)%8, jj = long(0.5+(j*c-i*s)/pw)%8;
	if(ii<0)	ii+=8;	if(jj<0)	jj+=8;
	return m[jj] & (1<<ii);
}
//-----------------------------------------------------------------------------
/* Bilinear interpolation r(u,v) = r0 + (r1-r0)*u + (r2-r0)*v + (r3+r0-r1-r2)*u*v
	is used (where r is one of {x,y,z,R,G,B,A}. Variables u,v are determined
	for each point (x,y) and selected one pair which 0<u<1 and 0<v<1.*/
void mglCanvas::quad_draw(const mglPnt &p1, const mglPnt &p2, const mglPnt &p3, const mglPnt &p4, const mglDrawReg *d)
{
	if(!(Quality&3))
	{
//		fast_draw(p1,p2,d);	fast_draw(p1,p3,d);
//		fast_draw(p4,p2,d);	fast_draw(p4,p3,d);
		fast_draw(p1,p4,d);	fast_draw(p2,p3,d);
		return;
	}
	unsigned char r[4];
	long y1,x1,y2,x2;
	float dd,dsx,dsy;
	mglPnt d1=p2-p1, d2=p3-p1, d3=p4+p1-p2-p3, p;

	x1 = long(fmin(p1.x<p2.x?p1.x:p2.x, p3.x<p4.x?p3.x:p4.x));	// bounding box
	y1 = long(fmin(p1.y<p2.y?p1.y:p2.y, p3.y<p4.y?p3.y:p4.y));
	x2 = long(fmax(p1.x>p2.x?p1.x:p2.x, p3.x>p4.x?p3.x:p4.x));
	y2 = long(fmax(p1.y>p2.y?p1.y:p2.y, p3.y>p4.y?p3.y:p4.y));
	x1=x1>d->x1?x1:d->x1;	x2=x2<d->x2?x2:d->x2;
	y1=y1>d->y1?y1:d->y1;	y2=y2<d->y2?y2:d->y2;
	if(x1>x2 || y1>y2)	return;

	dd = d1.x*d2.y-d1.y*d2.x;
	dsx =-4*(d2.y*d3.x - d2.x*d3.y)*d1.y;
	dsy = 4*(d2.y*d3.x - d2.x*d3.y)*d1.x;

	if((d1.x==0 && d1.y==0) || (d2.x==0 && d2.y==0) || !(Quality&MGL_DRAW_NORM))
	{	trig_draw(p1,p2,p4,true,d);	trig_draw(p1,p3,p4,true,d);	return;	}

	mglPoint n1 = mglPoint(p2.x-p1.x,p2.y-p1.y,p2.z-p1.z)^mglPoint(p3.x-p1.x,p3.y-p1.y,p3.z-p1.z);
	mglPoint n2 = mglPoint(p2.x-p4.x,p2.y-p4.y,p2.z-p4.z)^mglPoint(p3.x-p4.x,p3.y-p4.y,p3.z-p4.z);
	mglPoint nr = (n1+n2)*0.5;

	float x0 = p1.x, y0 = p1.y;
	for(long i=x1;i<=x2;i++)	for(long j=y1;j<=y2;j++)
	{
		if(!visible(i,j,d->m, d->PenWidth,d->angle))	continue;
		register float xx = (i-x0), yy = (j-y0), s;
		s = dsx*xx + dsy*yy + (dd+d3.y*xx-d3.x*yy)*(dd+d3.y*xx-d3.x*yy);
		if(s<0)	continue;	// no solution
		s = sqrt(s);
		register float qu = d3.x*yy - d3.y*xx + dd + s, u=-1;
		register float qv = d3.y*xx - d3.x*yy + dd + s, v=-1;
//		if(qu && qv)
		{
			u = 2.f*(d2.y*xx - d2.x*yy)/qu;
			v = 2.f*(d1.x*yy - d1.y*xx)/qv;
		}
		if(u*(1.f-u)<0.f || v*(1.f-v)<0.f)	// first root bad
		{
			qu = d3.x*yy - d3.y*xx + dd - s;
			qv = d3.y*xx - d3.x*yy + dd - s;
			u = v = -1.f;
//			if(qu && qv)
			{
				u = 2.f*(d2.y*xx - d2.x*yy)/qu;
				v = 2.f*(d1.x*yy - d1.y*xx)/qv;
			}
			if(u*(1.f-u)<0.f || v*(1.f-v)<0.f)	continue;	// second root bad
		}
		p = p1+d1*u+d2*v+d3*(u*v);
		if(mgl_isnan(p.u) && mgl_isnum(p.v))
		{	p.u = nr.x;	p.v = nr.y;	p.w = nr.z;	}
		pnt_plot(i,j,p.z,col2int(p,r,d->ObjId),d->ObjId);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::quad_pix(long i, long j, const mglPnt &p1, const mglPnt &p2, const mglPnt &p3, const mglPnt &p4, const mglDrawReg *d)
{
	if(!visible(i,j,d->m, d->PenWidth,d->angle))	return;
	mglPnt d1=p2-p1, d2=p3-p1, d3=p4+p1-p2-p3;
	register float dd = d1.x*d2.y-d1.y*d2.x;
	register float dsx =-4*(d2.y*d3.x - d2.x*d3.y)*d1.y;
	register float dsy = 4*(d2.y*d3.x - d2.x*d3.y)*d1.x;
	register float xx = (i-p1.x), yy = (j-p1.y), s;
	s = dsx*xx + dsy*yy + (dd+d3.y*xx-d3.x*yy)*(dd+d3.y*xx-d3.x*yy);
	if(s<0)	return;	// no solution
	s = sqrt(s);
	register float qu = d3.x*yy - d3.y*xx + dd + s, u=-1;
	register float qv = d3.y*xx - d3.x*yy + dd + s, v=-1;
	if(qu && qv)
	{
		u = 2.f*(d2.y*xx - d2.x*yy)/qu;
		v = 2.f*(d1.x*yy - d1.y*xx)/qv;
	}
	if(u*(1.f-u)<0.f || v*(1.f-v)<0.f)	// first root bad
	{
		qu = d3.x*yy - d3.y*xx + dd - s;
		qv = d3.y*xx - d3.x*yy + dd - s;
		u = v = -1.f;
		if(qu && qv)
		{
			u = 2.f*(d2.y*xx - d2.x*yy)/qu;
			v = 2.f*(d1.x*yy - d1.y*xx)/qv;
		}
		if(u*(1.f-u)<0.f || v*(1.f-v)<0.f)	return;	// second root bad
	}
	mglPnt p = p1+d1*u+d2*v+d3*(u*v);
	if(mgl_isnan(p.u) && mgl_isnum(p.v))
	{
		mglPoint n1 = mglPoint(p2.x-p1.x,p2.y-p1.y,p2.z-p1.z)^mglPoint(p3.x-p1.x,p3.y-p1.y,p3.z-p1.z);
		mglPoint n2 = mglPoint(p2.x-p4.x,p2.y-p4.y,p2.z-p4.z)^mglPoint(p3.x-p4.x,p3.y-p4.y,p3.z-p4.z);
		p.u = (n1.x+n2.x)*0.5;
		p.v = (n1.y+n2.y)*0.5;
		p.w = (n1.z+n2.z)*0.5;
	}
	unsigned char r[4];
	pnt_plot(i,j,p.z,col2int(p,r,d->ObjId),d->ObjId);
}
//-----------------------------------------------------------------------------
/* Linear interpolation r(u,v) = r0 + (r1-r0)*u + (r2-r0)*v is used, where r is
	one of {x,y,z,R,G,B,A}. Variables u,v are determined for each point (x,y).
	Point plotted is u>0 and v>0 and u+v<1.*/
void mglCanvas::trig_draw(const mglPnt &p1, const mglPnt &p2, const mglPnt &p3, bool anorm, const mglDrawReg *d)
{
	if(!(Quality&3) && anorm)
	{
		fast_draw(p1,p2,d);	fast_draw(p1,p3,d);
		fast_draw(p2,p3,d);	return;
	}
	unsigned char r[4];
	long y1,x1,y2,x2;
	float dxu,dxv,dyu,dyv;
	mglPnt d1=p2-p1, d2=p3-p1, p;

	dxu = d2.x*d1.y - d1.x*d2.y;
	if(fabs(dxu)<1e-5)	return;		// points lies on the same line
	dyv =-d1.x/dxu;	dxv = d1.y/dxu;
	dyu = d2.x/dxu;	dxu =-d2.y/dxu;

	x1 = long(fmin(p1.x<p2.x?p1.x:p2.x, p3.x));	// bounding box
	y1 = long(fmin(p1.y<p2.y?p1.y:p2.y, p3.y));
	x2 = long(fmax(p1.x>p2.x?p1.x:p2.x, p3.x));
	y2 = long(fmax(p1.y>p2.y?p1.y:p2.y, p3.y));
	x1=x1>d->x1?x1:d->x1;	x2=x2<d->x2?x2:d->x2;
	y1=y1>d->y1?y1:d->y1;	y2=y2<d->y2?y2:d->y2;
	if(x1>x2 || y1>y2)	return;
	// default normale
	mglPoint nr = mglPoint(p2.x-p1.x,p2.y-p1.y,p2.z-p1.z)^mglPoint(p3.x-p1.x,p3.y-p1.y,p3.z-p1.z);
	float x0 = p1.x, y0 = p1.y;
	if(Quality&MGL_DRAW_NORM)	for(long i=x1;i<=x2;i++)	for(long j=y1;j<=y2;j++)
	{
		if(!visible(i,j,d->m, d->PenWidth,d->angle))	continue;
		register float xx = (i-x0), yy = (j-y0);
		register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;
		if(u<0 || v<0 || u+v>1)	continue;
		p = p1+d1*u+d2*v;
		if(mgl_isnan(p.u) && mgl_isnum(p.v) && anorm)
		{	p.u = nr.x;	p.v = nr.y;	p.w = nr.z;	}
		pnt_plot(i,j,p.z,col2int(p,r,d->ObjId),d->ObjId);
	}
	else	for(long i=x1;i<=x2;i++)	for(long j=y1;j<=y2;j++)
	{
		if(!visible(i,j,d->m, d->PenWidth,d->angle))	continue;
		register float xx = (i-x0), yy = (j-y0);
		register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;
		if(u<0 || v<0 || u+v>1)	continue;
		pnt_plot(i,j,p1.z,col2int(p1,r,d->ObjId),d->ObjId);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::trig_pix(long i, long j, const mglPnt &p1, const mglPnt &p2, const mglPnt &p3, bool anorm, const mglDrawReg *d)
{
	if(!visible(i,j,d->m, d->PenWidth,d->angle))	return;
	mglPnt d1=p2-p1, d2=p3-p1;
	register float dd = d2.x*d1.y - d1.x*d2.y;
	if(fabs(dd)<1e-5)	return;		// points lies on the same line
	register float dyv =-d1.x/dd, dxv = d1.y/dd, dyu = d2.x/dd, dxu =-d2.y/dd;
	register float xx = (i-p1.x), yy = (j-p1.y);
	register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;
	if(u<0 || v<0 || u+v>1)	return;
	mglPnt p = p1+d1*u+d2*v;
	if(mgl_isnan(p.u) && mgl_isnum(p.v) && anorm)
	{	mglPoint nr = mglPoint(p2.x-p1.x,p2.y-p1.y,p2.z-p1.z)^mglPoint(p3.x-p1.x,p3.y-p1.y,p3.z-p1.z);
		p.u = nr.x;	p.v = nr.y;	p.w = nr.z;	}
	unsigned char r[4];
	pnt_plot(i,j,p.z,col2int(p,r,d->ObjId),d->ObjId);
}
//-----------------------------------------------------------------------------
//#define mgl_sline(c,x)	(unsigned char)((c)/cosh(x))
inline unsigned char mgl_sline(unsigned char c,float x)
{	x*=x/2;	return (unsigned char)((c)/(1+x+x*x/5));	}
void mglCanvas::line_draw(const mglPnt &p1, const mglPnt &p2, const mglDrawReg *dr)
{
	if((Quality&3)<2)	{	fast_draw(p1,p2,dr);	return;	}
	unsigned char r[4];
	long y1,x1,y2,x2;

	float pw=dr->PenWidth, dxu,dxv,dyu,dyv,dd,dpw=3;
	float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces

	if(dr->ObjId==HighId)	{	pw *= 2;	dpw=2;	}
	mglPnt d=p2-p1, p;
	bool hor = fabs(d.x)>fabs(d.y);

	x1 = long(p1.x<p2.x?p1.x:p2.x);	y1 = long(p1.y<p2.y?p1.y:p2.y);	// bounding box
	x2 = long(p1.x>p2.x?p1.x:p2.x);	y2 = long(p1.y>p2.y?p1.y:p2.y);
	x1 -= pw+3.5;	x2 += pw+3.5;
	y1 -= pw+3.5;	y2 += pw+3.5;
	x1=x1>dr->x1?x1:dr->x1;	x2=x2<dr->x2?x2:dr->x2;
	y1=y1>dr->y1?y1:dr->y1;	y2=y2<dr->y2?y2:dr->y2;
	dd = hypot(d.x, d.y);
	if(x1>x2 || y1>y2 || dd<1e-5)	return;

	dxv = d.y/dd;	dyv =-d.x/dd;
	dxu = d.x/dd;	dyu = d.y/dd;

	if(hor)	for(long i=x1;i<=x2;i++)
	{
		y1 = int(p1.y+d.y*(i-p1.x)/d.x - pw - 3.5);
		y2 = int(p1.y+d.y*(i-p1.x)/d.x + pw + 3.5);
		y1=y1>dr->y1?y1:dr->y1;	y2=y2<dr->y2?y2:dr->y2;
		if(y1>y2)	continue;
		for(long j=y1;j<=y2;j++)
		{
			register float xx = (i-p1.x), yy = (j-p1.y);
			register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;	v = v*v;
			if(u<0)			v += u*u;
			else if(u>dd)	v += (u-dd)*(u-dd);
			if(v>pw*pw)		continue;
			if(!(dr->PDef & ( 1<<long(fmod(dr->pPos+u/pw/1.5, 16)) ) ))	continue;
			p = p1+d*(u/dd);	col2int(p,r,dr->ObjId);
			r[3] = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
			pnt_plot(i,j,p.z+dz,r,dr->ObjId);
		}
	}
	else	for(long j=y1;j<=y2;j++)
	{
		x1 = int(p1.x+d.x*(j-p1.y)/d.y - pw - 3.5);
		x2 = int(p1.x+d.x*(j-p1.y)/d.y + pw + 3.5);
		x1=x1>dr->x1?x1:dr->x1;	x2=x2<dr->x2?x2:dr->x2;
		if(x1>x2)	continue;

		for(long i=x1;i<=x2;i++)
		{
			register float xx = (i-p1.x), yy = (j-p1.y);
			register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;	v = v*v;
			if(u<0)			v += u*u;
			else if(u>dd)	v += (u-dd)*(u-dd);
			if(v>pw*pw)		continue;
			if(!(dr->PDef & (1<<long(fmod(dr->pPos+u/pw/1.5, 16)))))		continue;
			p = p1+d*(u/dd);	col2int(p,r,dr->ObjId);
			r[3] = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
			pnt_plot(i,j,p.z+dz,r,dr->ObjId);
		}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pnt_fast(long x,long y,mreal z,const unsigned char ci[4], int obj_id)
{
	register long i0=x+Width*(Height-1-y);
	if(ci[3]!=0 && z>Z[3*i0])	// point upper the background
	{	Z[3*i0]=z;	memcpy(C+12*i0,ci,4);	OI[i0]=obj_id;	}
}
//-----------------------------------------------------------------------------
void mglCanvas::fast_draw(const mglPnt &p1, const mglPnt &p2, const mglDrawReg *dr)
{
	if(p1.x==p2.x && p1.y==p2.y) return;
	mglPnt d=p2-p1;
	unsigned char r[4];	col2int(p1,r,dr->ObjId);
	long y1,x1,y2,x2;

	bool hor = fabs(d.x)>fabs(d.y);

	x1 = long(p1.x<p2.x?p1.x:p2.x);	y1 = long(p1.y<p2.y?p1.y:p2.y);	// bounding box
	x2 = long(p1.x>p2.x?p1.x:p2.x);	y2 = long(p1.y>p2.y?p1.y:p2.y);
	x1=x1>dr->x1?x1:dr->x1;	x2=x2<dr->x2?x2:dr->x2;
	y1=y1>dr->y1?y1:dr->y1;	y2=y2<dr->y2?y2:dr->y2;
	if(x1>x2 || y1>y2)	return;
	float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces

	if(hor)	for(long i=x1;i<=x2;i++)
	{
		register long c = long(p1.y+d.y*(i-p1.x)/d.x);
		if(c>=y1 && c<=y2)
			pnt_fast(i, c, p1.z+d.z*(i-p1.x)/d.x+dz, r,dr->ObjId);
	}
	else	for(long i=y1;i<=y2;i++)
	{
		register long c = long(p1.x+d.x*(i-p1.y)/d.y);
		if(c>=x1 && c<=x2)
			pnt_fast(c, i, p1.z+d.z*(i-p1.y)/d.y+dz, r,dr->ObjId);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::line_pix(long i, long j, const mglPnt &p1, const mglPnt &p2, const mglDrawReg *dr)
{
	register float xx = (i-p1.x), yy = (j-p1.y);
	mglPnt d=p2-p1;
	register float dd = hypot(d.x, d.y);
	register float dxv = d.y/dd, dyv =-d.x/dd, dxu = d.x/dd, dyu = d.y/dd;
	register float u = dxu*xx+dyu*yy, v = dxv*xx+dyv*yy;	v = v*v;
	if(u<0)			v += u*u;
	else if(u>dd)	v += (u-dd)*(u-dd);
	register float pw=dr->PenWidth, dpw=3;
	if(dr->ObjId==HighId)	{	pw *= 2;	dpw=2;	}
	if(v>pw*pw || !(dr->PDef & ( 1<<long(fmod(dr->pPos+u/pw/1.5, 16)) ) ))	return;
	mglPnt p = p1+d*(u/dd);
	unsigned char r[4];
	col2int(p,r,dr->ObjId);
	r[3] = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
	register float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces
	pnt_plot(i,j,p.z+dz,r,dr->ObjId);
}
//-----------------------------------------------------------------------------
void mglCanvas::pnt_draw(const mglPnt &p, const mglDrawReg *dr)
{
//	if(k<0 || !dr)	return;
	float pw=3*dr->PenWidth,dpw=3;
	if(dr->ObjId==HighId)	{	pw *= 2;	dpw=2;	}
	unsigned char cs[4], cc;
	col2int(p,cs,dr->ObjId);	cc = cs[3];
	if(cc==0)	return;
	long s = long(5.5+fabs(pw));
	long i1=fmax(-s,dr->x1-p.x),i2=fmin(s,dr->x2-p.x), j1=fmax(-s,dr->y1-p.y),j2=fmin(s,dr->y2-p.y);
	if(!(Quality&3))	for(long j=j1;j<=j2;j++)	for(long i=i1;i<=i2;i++)	// fast draw
	{
		register float v = i*i+j*j;
		if(v>1+(pw-1)*(pw-1)/4)	continue;
		pnt_plot(p.x+i,p.y+j,p.z,cs,dr->ObjId);
	}
	else	for(long j=j1;j<=j2;j++)	for(long i=i1;i<=i2;i++)
	{
		register float v = i*i+j*j;
		cs[3] = v<(pw-1)*(pw-1)/4 ? cc : mgl_sline(cc,dpw*(sqrt(v)+(1-pw)/2));
		pnt_plot(p.x+i,p.y+j,p.z,cs,dr->ObjId);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::pnt_pix(long i, long j, const mglPnt &p, const mglDrawReg *dr)
{
	register float pw=3*dr->PenWidth,dpw=3;
	if(dr->ObjId==HighId)	{	pw *= 2;	dpw=2;	}
	unsigned char cs[4];
	col2int(p,cs,dr->ObjId);
	register float xx = (i-p.x), yy = (j-p.y), v = xx*xx+yy*yy;
	if(cs[3]==0 || v>(5.5+pw)*(5.5+pw))	return;
	if(v<(pw-1)*(pw-1)/4)	cs[3] = mgl_sline(cs[3],dpw*(sqrt(v)+(1-pw)/2));
	pnt_plot(i,j,p.z,cs,dr->ObjId);
}
//-----------------------------------------------------------------------------
void mglCanvas::mark_draw(const mglPnt &q, char type, mreal size, mglDrawReg *d)
{
	unsigned char cs[4], ca;	col2int(q,cs,d->ObjId);	ca = cs[3];// = size>0 ? 255 : 255*q.t;
	mreal ss=fabs(size), pw=1,dpw=3;

	if(type=='.' || ss==0)
	{
		if(d)	pw = 3*(ss?ss:sqrt(font_factor/400));
		register mreal dd = pw+3.5;
		long x1 = long(q.x-dd), y1 = long(q.y-dd);	// bounding box
		long x2 = long(q.x+dd), y2 = long(q.y+dd);
		x1=x1>d->x1?x1:d->x1;	x2=x2<d->x2?x2:d->x2;
		y1=y1>d->y1?y1:d->y1;	y2=y2<d->y2?y2:d->y2;
		if(x1>x2 || y1>y2)	return;

		if(d->ObjId==HighId)	{	pw *= 2;	dpw=2;	}
		for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
		{
			register float dx=i-q.x, dy=j-q.y, v=dx*dx+dy*dy;
			register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
			cs[3] = ca*sum/255;
			pnt_plot(i,j,q.z+1,cs,d->ObjId);
		}
	}
	else
	{
		if(d)
		{
			d->PDef = MGL_SOLID_MASK;	d->angle = 0;
			pw = d->PenWidth*fabs(50*size);
			if(pw<1)	pw=1;
		}
		if(!strchr("xsSoO",type))	ss *= 1.1;
		if(d->ObjId==HighId)	{	pw *= 2;	dpw=2;	}

		register mreal dd = ss+pw+3.5;
		long x1 = long(q.x-dd), y1 = long(q.y-dd);	// bounding box
		long x2 = long(q.x+dd), y2 = long(q.y+dd);
		x1=x1>d->x1?x1:d->x1;	x2=x2<d->x2?x2:d->x2;
		y1=y1>d->y1?y1:d->y1;	y2=y2<d->y2?y2:d->y2;
		if(x1>x2 || y1>y2)	return;

		switch(type)
		{
		case 'P':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = (dx-ss)*(dx-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dy)-ss;	v = (dx+ss)*(dx+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dy)-ss;	v = dx*dx+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy-ss)*(dy-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy+ss)*(dy+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = dy*dy+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case '+':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = dx*dx+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = dy*dy+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'X':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = (dx-ss)*(dx-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dy)-ss;	v = (dx+ss)*(dx+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy-ss)*(dy-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy+ss)*(dy+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));

				u = fabs(dx+dy)-2*ss;	v = dx-dy;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx-dy)-2*ss;	v = dx+dy;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));

				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'x':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dx+dy)-2*ss;	v = dx-dy;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx-dy)-2*ss;	v = dx+dy;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'S':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u = fabs(dy)-ss;	if(u<0)	u=0;
				v = fabs(dx)-ss;	if(v<0)	v=0;	v = u*u+v*v;
				register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 's':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = (dx-ss)*(dx-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dy)-ss;	v = (dx+ss)*(dx+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy-ss)*(dy-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx)-ss;	v = (dy+ss)*(dy+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'D':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u = fabs(dx-dy)-ss;	if(u<0)	u=0;
				v = fabs(dx+dy)-ss;	if(v<0)	v=0;	v = u*u+v*v;
				register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'd':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dx+dy)-ss;	v = (dx-dy-ss)*(dx-dy-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx+dy)-ss;	v = (dx-dy+ss)*(dx-dy+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx-dy)-ss;	v = (dx+dy-ss)*(dx+dy-ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(dx-dy)-ss;	v = (dx+dy+ss)*(dx+dy+ss)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'Y':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy+ss/2)-ss/2;	v = dx*dx+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.87*dx+0.5*dy-ss/2)-ss/2;	v = (0.5*dx-0.87*dy)*(0.5*dx-0.87*dy)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(-0.87*dx+0.5*dy-ss/2)-ss/2;	v = (0.5*dx+0.87*dy)*(0.5*dx+0.87*dy)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case '*':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = dx*dx+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.87*dx+0.5*dy)-ss;	v = (0.5*dx-0.87*dy)*(0.5*dx-0.87*dy)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(-0.87*dx+0.5*dy)-ss;	v = (0.5*dx+0.87*dy)*(0.5*dx+0.87*dy)+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'T':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u=dy/1.5+ss/3;	v=(dx+ss-u)/2;
				if(u>0 && v>0 && u+v<ss)	cs[3]=ca;
				else
				{
					register int sum=0;
					u = fabs(dx)-ss;	v = dy+ss/2;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dx+0.83*dy)-0.9*ss;	v = 0.83*dx-0.55*dy+0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dx-0.83*dy)-0.9*ss;	v = 0.83*dx+0.55*dy-0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				}
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case '^':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dx)-ss;	v = dy+ss/2;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dx+0.83*dy)-0.9*ss;	v = 0.83*dx-0.55*dy+0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dx-0.83*dy)-0.9*ss;	v = 0.83*dx+0.55*dy-0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'V':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u=-dy/1.5+ss/3;	v=(dx+ss-u)/2;
				if(u>0 && v>0 && u+v<ss)	cs[3]=ca;
				else
				{
					register int sum=0;
					u = fabs(dx)-ss;	v = dy-ss/2;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dx+0.83*dy)-0.9*ss;	v = 0.83*dx-0.55*dy-0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dx-0.83*dy)-0.9*ss;	v = 0.83*dx+0.55*dy+0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				}
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'v':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dx)-ss;	v = dy-ss/2;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dx+0.83*dy)-0.9*ss;	v = 0.83*dx-0.55*dy-0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dx-0.83*dy)-0.9*ss;	v = 0.83*dx+0.55*dy+0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'L':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u=-dx/1.5+ss/3;	v=(dy+ss-u)/2;
				if(u>0 && v>0 && u+v<ss)	cs[3]=ca;
				else
				{
					register int sum=0;
					u = fabs(dy)-ss;	v = dx-ss/2;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dy+0.83*dx)-0.9*ss;	v = 0.83*dy-0.55*dx-0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dy-0.83*dx)-0.9*ss;	v = 0.83*dy+0.55*dx+0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				}
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case '<':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = dx-ss/2;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dy+0.83*dx)-0.9*ss;	v = 0.83*dy-0.55*dx-0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dy-0.83*dx)-0.9*ss;	v = 0.83*dy+0.55*dx+0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'R':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				u=dx/1.5+ss/3;	v=(dy+ss-u)/2;
				if(u>0 && v>0 && u+v<ss)	cs[3]=ca;
				else
				{
					register int sum=0;
					u = fabs(dy)-ss;	v = dx+ss/2;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dy+0.83*dx)-0.9*ss;	v = 0.83*dy-0.55*dx+0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					u = fabs(0.55*dy-0.83*dx)-0.9*ss;	v = 0.83*dy+0.55*dx-0.55*ss;	v = v*v+(u<0?0:u*u);
					sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
					sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				}
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case '>':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v,u;
				register int sum=0;
				u = fabs(dy)-ss;	v = dx+ss/2;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dy+0.83*dx)-0.9*ss;	v = 0.83*dy-0.55*dx+0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				u = fabs(0.55*dy-0.83*dx)-0.9*ss;	v = 0.83*dy+0.55*dx-0.55*ss;	v = v*v+(u<0?0:u*u);
				sum += v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'O':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v;
				v = hypot(dx,dy)-ss;	v=v<0?0:v*v;
				register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'o':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v;
				v = hypot(dx,dy)-ss;	v=v*v;
				register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		case 'C':
			for(long j=y1;j<=y2;j++)	for(long i=x1;i<=x2;i++)
			{
				register float dx=i-q.x, dy=j-q.y, v;
				v = hypot(dx,dy)-ss;	v=v*v;
				register int sum = v<(pw-1)*(pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-pw)/2));
				v = dx*dx+dy*dy;
				sum += v<(2*pw-1)*(2*pw-1)/4 ? 255 : mgl_sline(255,dpw*(sqrt(v)+(1-2*pw)/2));
				sum = sum>255?255:sum;	cs[3] = ca*sum/255;		cs[3] = ca*sum/255;
				pnt_plot(i,j,q.z+1,cs,d->ObjId);
			}
			break;
		}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::mark_pix(long i, long j, const mglPnt &q, char type, mreal size, mglDrawReg *d)
{
	unsigned char cs[4];	col2int(q,cs,d->ObjId);	cs[3] = size>0 ? 255 : 255*q.t;
	mglPnt p0=q,p1=q,p2=q,p3=q;
	mreal ss=fabs(size);

	if(type=='.' || ss==0)
	{
		if(d)	d->PenWidth = ss?ss:sqrt(font_factor/400);
		pnt_pix(i,j,q,d);
	}
	else
	{
		if(d)
		{
			d->PDef = MGL_SOLID_MASK;	d->angle = 0;
			d->PenWidth*=fabs(50*size);
			if(d->PenWidth<1)	d->PenWidth=1;
		}
		if(!strchr("xsSoO",type))	ss *= 1.1;
		switch(type)
		{
		case 'P':
			p0.x = q.x-ss;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y-ss;
			p2.x = q.x+ss;	p2.y = q.y+ss;	p3.x = q.x-ss;	p3.y = q.y+ss;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p3,d);	line_pix(i,j,p3,p0,d);
		case '+':
			p0.x = q.x-ss;	p0.y = q.y;	p1.x = q.x+ss;	p1.y = q.y;	line_pix(i,j,p0,p1,d);
			p2.x = q.x;	p2.y = q.y-ss;	p3.x = q.x;	p3.y = q.y+ss;	line_pix(i,j,p2,p3,d);
			break;
		case 'X':
			p0.x = q.x-ss;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y-ss;
			p2.x = q.x+ss;	p2.y = q.y+ss;	p3.x = q.x-ss;	p3.y = q.y+ss;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p3,d);	line_pix(i,j,p3,p0,d);
		case 'x':
			p0.x = q.x-ss;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y+ss;	line_pix(i,j,p0,p1,d);
			p2.x = q.x+ss;	p2.y = q.y-ss;	p3.x = q.x-ss;	p3.y = q.y+ss;	line_pix(i,j,p2,p3,d);
			break;
		case 'S':
			p0.x = q.x-ss;	p0.y = q.y-ss;	p1.x = q.x-ss;	p1.y = q.y+ss;
			p2.x= q.x+ss;	p2.y= q.y+ss;	p3.x = q.x+ss;	p3.y = q.y-ss;
			quad_pix(i,j,p0,p1,p3,p2,d);
		case 's':
			p0.x = q.x-ss;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y-ss;
			p2.x = q.x+ss;	p2.y = q.y+ss;	p3.x = q.x-ss;	p3.y = q.y+ss;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p3,d);	line_pix(i,j,p3,p0,d);
			break;
		case 'D':
			p0.x = q.x;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y;
			p2.x= q.x;	p2.y= q.y+ss;	p3.x = q.x-ss;	p3.y = q.y;
			quad_pix(i,j,p0,p1,p3,p2,d);
		case 'd':
			p0.x = q.x;	p0.y = q.y-ss;	p1.x = q.x+ss;	p1.y = q.y;
			p2.x = q.x;	p2.y = q.y+ss;	p3.x = q.x-ss;	p3.y = q.y;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p3,d);	line_pix(i,j,p3,p0,d);
			break;
		case 'Y':
			p1.x = q.x;	p1.y = q.y-ss;	line_pix(i,j,q,p1,d);
			p2.x = q.x-0.8*ss;	p2.y = q.y+0.6*ss;	line_pix(i,j,q,p2,d);
			p3.x = q.x+0.8*ss;	p3.y = q.y+0.6*ss;	line_pix(i,j,q,p3,d);
			break;
		case '*':
			p0.x = q.x-ss;		p0.y = q.y;
			p1.x = q.x+ss;		p1.y = q.y;	line_pix(i,j,p0,p1,d);
			p0.x = q.x-0.6*ss;	p0.y = q.y-0.8*ss;
			p1.x = q.x+0.6*ss;	p1.y = q.y+0.8*ss;	line_pix(i,j,p0,p1,d);
			p0.x = q.x-0.6*ss;	p0.y = q.y+0.8*ss;
			p1.x = q.x+0.6*ss;	p1.y = q.y-0.8*ss;	line_pix(i,j,p0,p1,d);
			break;
		case 'T':
			p0.x = q.x-ss;	p0.y = q.y-ss/2;
			p1.x = q.x+ss;	p1.y = q.y-ss/2;
			p2.x= q.x;		p2.y= q.y+ss;
			trig_pix(i,j,p0,p1,p2,false,d);
		case '^':
			p0.x = q.x-ss;	p0.y = q.y-ss/2;
			p1.x = q.x+ss;	p1.y = q.y-ss/2;
			p2.x= q.x;		p2.y= q.y+ss;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p0,d);	break;
		case 'V':
			p0.x = q.x-ss;	p0.y = q.y+ss/2;
			p1.x = q.x+ss;	p1.y = q.y+ss/2;
			p2.x= q.x;		p2.y= q.y-ss;
			trig_pix(i,j,p0,p1,p2,false,d);
		case 'v':
			p0.x = q.x-ss;	p0.y = q.y+ss/2;
			p1.x = q.x+ss;	p1.y = q.y+ss/2;
			p2.x= q.x;		p2.y= q.y-ss;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p0,d);	break;
		case 'L':
			p0.x = q.x+ss/2;	p0.y = q.y+ss;
			p1.x = q.x+ss/2;	p1.y = q.y-ss;
			p2.x= q.x-ss;		p2.y= q.y;
			trig_pix(i,j,p0,p1,p2,false,d);
		case '<':
			p0.x = q.x+ss/2;	p0.y = q.y+ss;
			p1.x = q.x+ss/2;	p1.y = q.y-ss;
			p2.x= q.x-ss;		p2.y= q.y;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p0,d);	break;
		case 'R':
			p0.x = q.x-ss/2;	p0.y = q.y+ss;
			p1.x = q.x-ss/2;	p1.y = q.y-ss;
			p2.x= q.x+ss;		p2.y= q.y;
			trig_pix(i,j,p0,p1,p2,false,d);
		case '>':
			p0.x = q.x-ss/2;	p0.y = q.y+ss;
			p1.x = q.x-ss/2;	p1.y = q.y-ss;
			p2.x= q.x+ss;		p2.y= q.y;
			line_pix(i,j,p0,p1,d);	line_pix(i,j,p1,p2,d);
			line_pix(i,j,p2,p0,d);	break;
		case 'O':
			{
				register float xx = (i-q.x), yy = (j-q.y);
				register float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces
				if(xx*xx+yy*yy<ss*ss)	pnt_plot(i,j,q.z+dz,cs,d->ObjId);
				// TODO add edge smoothing here?
			}
		case 'o':
			{
				register float pw=d->PenWidth, dpw=2;
				register float xx = (i-q.x), yy = (j-q.y), v = hypot(xx,yy);
				v = (v-ss)*(v-ss);
				if(v>pw*pw)	return;
				if(v>(pw-1)*(pw-1)/4)	cs[3] = mgl_sline(cs[3],dpw*(sqrt(v)+(1-pw)/2));
				register float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces
				pnt_plot(i,j,q.z+dz,cs,d->ObjId);
			}
			break;
		case 'C':
			pnt_pix(i,j,q,d);
			{
				register float pw=d->PenWidth, dpw=2;
				register float xx = (i-q.x), yy = (j-q.y), v = hypot(xx,yy);
				v = (v-ss)*(v-ss);
				if(v>pw*pw)	return;
				if(v>(pw-1)*(pw-1)/4)	cs[3] = mgl_sline(cs[3],dpw*(sqrt(v)+(1-pw)/2));
				register float dz = Width>2 ? 1 : 1e-5*Width;		// provide additional height to be well visible on the surfaces
				pnt_plot(i,j,q.z+dz,cs,d->ObjId);
			}
			break;
		}
	}
}
//-----------------------------------------------------------------------------
// scale direction for new view/zoom
float mglCanvas::GetGlyphPhi(const mglPnt &q, float phi)
{
	float x,y,z,ll;
	if(q.sub)
	{	x = q.u;	y = q.v;	z = q.w;	}
	else
	{
		x = Bp.b[0]*q.u + Bp.b[1]*q.v + Bp.b[2]*q.w;
		y = Bp.b[3]*q.u + Bp.b[4]*q.v + Bp.b[5]*q.w;
		z = Bp.b[6]*q.u + Bp.b[7]*q.v + Bp.b[8]*q.w;

		register float dv = (1-Bp.pf)/(1-Bp.pf*q.z/Depth);
		register float c=Bp.pf/(1-Bp.pf)/Depth;
		x += (q.x-Width/2)*z*c*dv;
		y += (q.y-Height/2)*z*c*dv;
	}
	ll = x*x+y*y;
	if(ll < 1e-10)	return NAN;
	if(ll==ll && phi<1e4)
	{
		phi = -atan2(y,x)*180/M_PI;
		if(fabs(phi)>90) 	phi+=180;
	}
	else phi=0;
	return phi;
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_draw(const mglPrim &P, mglDrawReg *d)
{
	float phi = GetGlyphPhi(Pnt[P.n2],P.w);
	if(mgl_isnan(phi))	return;

	if(d)	{	d->PDef = MGL_SOLID_MASK;	d->angle = 0;	d->PenWidth=0.6;	}
	mglPnt p=Pnt[P.n1];
	// NOTE check this later for mglInPlot
	mreal pf=p.sub<0?1:sqrt((Bp.b[0]*Bp.b[0]+Bp.b[1]*Bp.b[1]+Bp.b[3]*Bp.b[3]+Bp.b[4]*Bp.b[4])/2), f = P.p*pf;

	mglMatrix M;
	M.b[0] = M.b[4] = M.b[8] = P.s;
	M.RotateN(phi,0,0,1);
	M.x=p.x;	M.y=p.y;	M.z=p.z;	M.pf = 1;
	p.u *= pf;	p.v *= pf;

	const mglGlyph &g = Glf[P.n4];
	if(P.n3&8)
	{
		if(!(P.n3&4))	glyph_line(&M,p,f,true, d);
		glyph_line(&M,p,f,false, d);
	}
	else
	{
		if(!(P.n3&4))	glyph_fill(&M,p,f,g, d);
		glyph_wire(&M,p,f,g, d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_fill(const mglMatrix *M, const mglPnt &pp, mreal f, const mglGlyph &g, const mglDrawReg *d)
{
	if(!g.trig || g.nt<=0)	return;
	mglPnt q0=pp, q1=pp, q2=pp;
	q0.u=q0.v=q1.u=q1.v=q2.u=q2.v=NAN;
	for(long ik=0;ik<g.nt;ik++)
	{
		register long ii = 6*ik;	mglPoint p;
		p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q0.x = p.x;	q0.y = p.y;	q0.z = p.z;
		ii+=2;	p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q1.x = p.x;	q1.y = p.y;	q1.z = p.z;
		ii+=2;	p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q2.x = p.x;	q2.y = p.y;	q2.z = p.z;
		trig_draw(q0,q1,q2,false,d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_wire(const mglMatrix *M, const mglPnt &pp, mreal f, const mglGlyph &g, const mglDrawReg *d)
{
	if(!g.line || g.nl<=0)	return;
	long il=0;
	mglPnt q0=pp, q1=pp;	q0.u=q0.v=q1.u=q1.v=NAN;
	mglPoint p1,p2;
//#pragma omp parallel for firstprivate(q0,q1,i1) private(p1,p2)
	for(long ik=0;ik<g.nl;ik++)
	{
		register long ii = 2*ik;
		if(g.line[ii]==0x3fff && g.line[ii+1]==0x3fff)	// line breakthrough
		{	il = ik+1;	continue;	}
		else if(ik==g.nl-1 || (g.line[ii+2]==0x3fff && g.line[ii+3]==0x3fff))
		{	// enclose the circle. May be in future this block should be commented
			p1 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);	ii=2*il;
			p2 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);
		}
		else
		{	// normal line
			p1 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);	ii+=2;
			p2 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);
		}
		PostScale(M,p1);	PostScale(M,p2);
		q0.x = p1.x;	q0.y = p1.y;	q0.z = p1.z;
		q1.x = p2.x;	q1.y = p2.y;	q1.z = p2.z;
		line_draw(q0,q1,d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_line(const mglMatrix *M, const mglPnt &pp, mreal f, bool solid, const mglDrawReg *d)
{
	mglPnt q0=pp,q1=pp,q2=pp,q3=pp;
	q0.u=q0.v=q1.u=q1.v=q2.u=q2.v=q3.u=q3.v=NAN;
	mglPoint p1,p2,p3,p4;

	mreal dy = 0.004;
	p1 = mglPoint(pp.u,pp.v-dy,0);	PostScale(M,p1);
	p2 = mglPoint(pp.u,pp.v+dy,0);	PostScale(M,p2);
	p3 = mglPoint(fabs(f)+pp.u,pp.v+dy,0);	PostScale(M,p3);
	p4 = mglPoint(fabs(f)+pp.u,pp.v-dy,0);	PostScale(M,p4);

	q0.x = p1.x;	q0.y = p1.y;	q0.z = p1.z;
	q1.x = p2.x;	q1.y = p2.y;	q1.z = p2.z;
	q2.x = p3.x;	q2.y = p3.y;	q2.z = p3.z;
	q3.x = p4.x;	q3.y = p4.y;	q3.z = p4.z;

	if(solid)	quad_draw(q0,q1,q3,q2,d);
	else
	{
		line_draw(q0,q1,d);	line_draw(q2,q1,d);
		line_draw(q0,q3,d);	line_draw(q2,q3,d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_pix(long i, long j, const mglPrim &P, mglDrawReg *d)
{
	float phi = GetGlyphPhi(Pnt[P.n2],P.w);
	if(mgl_isnan(phi))	return;

	if(d)	{	d->PDef = MGL_SOLID_MASK;	d->angle = 0;	d->PenWidth=1;	}
	mglPnt p=Pnt[P.n1];
	mreal pf=sqrt((Bp.b[0]*Bp.b[0]+Bp.b[1]*Bp.b[1]+Bp.b[3]*Bp.b[3]+Bp.b[4]*Bp.b[4])/2), f = P.p*pf;

	mglMatrix M;
	M.b[0] = M.b[4] = M.b[8] = P.s;
	M.RotateN(phi,0,0,1);
	M.x=p.x;	M.y=p.y;	M.z=p.z;	M.pf = 1;
	p.u *= pf;	p.v *= pf;

	const mglGlyph &g = Glf[P.n4];
	if(P.n3&8)
	{
		if(!(P.n3&4))	glyph_lpix(i,j,&M,p,f,true, d);
		glyph_lpix(i,j,&M,p,f,false, d);
	}
	else
	{
		if(!(P.n3&4))	glyph_fpix(i,j,&M,p,f,g, d);
		glyph_wpix(i,j,&M,p,f,g, d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_fpix(long i, long j, const mglMatrix *M, const mglPnt &pp, mreal f, const mglGlyph &g, const mglDrawReg *d)
{
	if(!g.trig || g.nt<=0)	return;
	mglPnt q0=pp, q1=pp, q2=pp;
	q0.u=q0.v=q1.u=q1.v=q2.u=q2.v=NAN;
	for(long ik=0;ik<g.nt;ik++)
	{
		register long ii = 6*ik;	mglPoint p;
		p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q0.x = p.x;	q0.y = p.y;	q0.z = p.z;
		ii+=2;	p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q1.x = p.x;	q1.y = p.y;	q1.z = p.z;
		ii+=2;	p = mglPoint(f*g.trig[ii]+pp.u,f*g.trig[ii+1]+pp.v,0);
		PostScale(M,p);	q2.x = p.x;	q2.y = p.y;	q2.z = p.z;
		trig_pix(i,j,q0,q1,q2,false,d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_wpix(long i, long j, const mglMatrix *M, const mglPnt &pp, mreal f, const mglGlyph &g, const mglDrawReg *d)
{
	if(!g.line || g.nl<=0)	return;
	long il=0;
	mglPnt q0=pp, q1=pp;	q0.u=q0.v=q1.u=q1.v=NAN;
	mglPoint p1,p2;
//#pragma omp parallel for firstprivate(q0,q1,i1) private(p1,p2)	// mostly useless here
	for(long ik=0;ik<g.nl;ik++)
	{
		register long ii = 2*ik;
		if(g.line[ii]==0x3fff && g.line[ii+1]==0x3fff)	// line breakthrough
		{	il = ik+1;	continue;	}
		else if(ik==g.nl-1 || (g.line[ii+2]==0x3fff && g.line[ii+3]==0x3fff))
		{	// enclose the circle. May be in future this block should be commented
			p1 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);	ii=2*il;
			p2 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);
		}
		else
		{	// normal line
			p1 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);	ii+=2;
			p2 = mglPoint(f*g.line[ii]+pp.u,f*g.line[ii+1]+pp.v,0);
		}
		PostScale(M,p1);	PostScale(M,p2);
		q0.x = p1.x;	q0.y = p1.y;	q0.z = p1.z;
		q1.x = p2.x;	q1.y = p2.y;	q1.z = p2.z;
		line_pix(i,j,q0,q1,d);
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::glyph_lpix(long i, long j, const mglMatrix *M, const mglPnt &pp, mreal f, bool solid, const mglDrawReg *d)
{
	mglPnt q0=pp,q1=pp,q2=pp,q3=pp;
	q0.u=q0.v=q1.u=q1.v=q2.u=q2.v=q3.u=q3.v=NAN;
	mglPoint p1,p2,p3,p4;

	mreal dy = 0.004;
	p1 = mglPoint(pp.u,pp.v-dy,0);	PostScale(M,p1);
	p2 = mglPoint(pp.u,pp.v+dy,0);	PostScale(M,p2);
	p3 = mglPoint(fabs(f)+pp.u,pp.v+dy,0);	PostScale(M,p3);
	p4 = mglPoint(fabs(f)+pp.u,pp.v-dy,0);	PostScale(M,p4);

	q0.x = p1.x;	q0.y = p1.y;	q0.z = p1.z;
	q1.x = p2.x;	q1.y = p2.y;	q1.z = p2.z;
	q2.x = p3.x;	q2.y = p3.y;	q2.z = p3.z;
	q3.x = p4.x;	q3.y = p4.y;	q3.z = p4.z;

	if(solid)	quad_pix(i,j,q0,q1,q3,q2,d);
	else
	{
		line_pix(i,j,q0,q1,d);	line_pix(i,j,q2,q1,d);
		line_pix(i,j,q0,q3,d);	line_pix(i,j,q2,q3,d);
	}
}
//-----------------------------------------------------------------------------
long mglCanvas::setPp(mglPnt &q, const mglPoint &p)
{
	q.xx=q.x=p.x;	q.yy=q.y=p.y;	q.zz=q.z=p.z;
	long k;
#pragma omp critical(pnt)
	{k=Pnt.size();	MGL_PUSH(Pnt,q,mutexPnt);}
	return k;
}
//-----------------------------------------------------------------------------
void mglCanvas::arrow_draw(long n1, long n2, char st, float ll)
{
	const mglPnt &p1=Pnt[n1], &p2=Pnt[n2];
	mglPnt q=p1; 	//q.u=q.v=q.w=0;

	mglPoint kl=mglPoint(p1.x-p2.x,p1.y-p2.y,p1.z-p2.z), kt, p0=mglPoint(p1.x,p1.y,p1.z), p;
	mreal d = hypot(kl.x,kl.y);
	if(d==0)	return;
	kl /= d;	kt = !kl;
	kl *= ll;	kt *= ll;

	Reserve(8);
	long k1,k2,k3,k4;

	switch(st)	// S,D -- cube, T -- sq.pyramid, I -- square, O -- sphere???, A,K,V -- cone???
	{
		case 'I':
			k1=setPp(q,p0+kt);	k2=setPp(q,p0-kt);	line_plot(k1,k2);	break;
		case 'D':
			k1=setPp(q,p0+kl);	k2=setPp(q,p0-kl);	k3=setPp(q,p0+kt);	k4=setPp(q,p0-kt);
			trig_plot(k1,k2,k3);	trig_plot(k1,k2,k4);	break;
		case 'S':
			k1=setPp(q,p0+kl+kt);	k2=setPp(q,p0+kl-kt);
			k3=setPp(q,p0-kl-kt);	k4=setPp(q,p0-kl+kt);
			quad_plot(k1,k2,k4,k3);	break;
		case 'T':
			k1=setPp(q,p0-kl+kt);	k2=setPp(q,p0-kl-kt);	k3=setPp(q,p0+kl);
			trig_plot(k1,k2,k3);	break;
		case 'K':
			k1=setPp(q,p0+kt);	k2=setPp(q,p0-kt);	line_plot(k1,k2);
		case 'A':
			k1=setPp(q,p0-2.*kl+kt);	k2=setPp(q,p0-2.*kl-kt);	k3=setPp(q,p0-1.5*kl);
			trig_plot(n1,k3,k1);	trig_plot(n1,k3,k2);	break;
		case 'V':
			k1=setPp(q,p0+2.*kl+kt);	k2=setPp(q,p0+2.*kl-kt);	k3=setPp(q,p0+1.5*kl);
			trig_plot(n1,k3,k1);	trig_plot(n1,k3,k2);	break;
		case 'O':	// let draw icosahedron
		{
			const int n = 12;	k1=setPp(q,p0+kl);
			for(int i=1;i<=n;i++)
			{
				mreal u = 2*i*M_PI/n;
				k2 = k1;	k1 = setPp(q,p0+kl*cos(u)+kt*sin(u));
				trig_plot(n1,k1,k2);
			}
			break;
		}
	}
}
//-----------------------------------------------------------------------------
void mglCanvas::arrow_plot_3d(long n1, long n2, char st, float ll)
{
	const mglPnt &p1=Pnt[n1], &p2=Pnt[n2];
	mglPnt q=p1; 	//q.u=q.v=q.w=0;

	mglPoint kl=mglPoint(p1.x-p2.x,p1.y-p2.y,p1.z-p2.z), kt, kz, p0=mglPoint(p1.x,p1.y,p1.z), p;
	if(kl.norm()==0)	return;
	kl.Normalize();	kt = !kl;	kz = kl^kt;
	kl *= ll;	kt *= ll;	kz *= ll;

	Reserve(8);
	long k1,k2,k3,k4,k5, k6,k7,k8;

	switch(st)	// S,D -- cube, T -- sq.pyramid, I -- square, O -- sphere???, A,K,V -- cone???
	{
		case 'I':
			k1=setPp(q,p0+kt);	k2=setPp(q,p0+kz);
			k3=setPp(q,p0-kt);	k4=setPp(q,p0-kz);
			quad_plot(k1,k2,k4,k3);	break;
		case 'D':
			k1=setPp(q,p0+kl);	k2=setPp(q,p0-kl);	k5=k3=setPp(q,p0+kt);
			k4=setPp(q,p0+kz);	trig_plot(k1,k3,k4);	trig_plot(k2,k3,k4);	k3=k4;
			k4=setPp(q,p0-kt);	trig_plot(k1,k3,k4);	trig_plot(k2,k3,k4);	k3=k4;
			k4=setPp(q,p0-kz);	trig_plot(k1,k3,k4);	trig_plot(k2,k3,k4);	k3=k4;
			trig_plot(k1,k3,k5);	trig_plot(k2,k3,k5);	break;
		case 'S':
			k1=setPp(q,p0+kl+kt);	k2=setPp(q,p0+kl+kz);	k3=setPp(q,p0+kl-kt);	k4=setPp(q,p0+kl-kz);
			k5=setPp(q,p0-kl+kt);	k6=setPp(q,p0-kl+kz);	k7=setPp(q,p0-kl-kt);	k8=setPp(q,p0-kl-kz);
			quad_plot(k1,k2,k4,k3);	quad_plot(k1,k2,k5,k6);	quad_plot(k3,k2,k7,k6);
			quad_plot(k1,k4,k5,k8);	quad_plot(k3,k4,k7,k8);	quad_plot(k5,k6,k8,k7);	break;
		case 'T':
			k1=setPp(q,p0-kl+kt);	k2=setPp(q,p0-kl+kz);	k3=setPp(q,p0-kl-kt);
			k4=setPp(q,p0-kl-kz);	k5=setPp(q,p0+kl);
			trig_plot(k1,k2,k5);	trig_plot(k2,k3,k5);
			trig_plot(k3,k4,k5);	trig_plot(k1,k4,k5);	break;
		case 'K':
			k1=setPp(q,p0+kt);	k2=setPp(q,p0+kz);
			k3=setPp(q,p0-kt);	k4=setPp(q,p0-kz);	quad_plot(k1,k2,k4,k3);
		case 'A':
			k1=setPp(q,p0-2.*kl+kt);	k2=setPp(q,p0-2.*kl+kz);	k3=setPp(q,p0-2.*kl-kt);
			k4=setPp(q,p0-2.*kl-kz);	k5=setPp(q,p0-1.5*kl);
			trig_plot(n1,k5,k1);	trig_plot(n1,k5,k2);
			trig_plot(n1,k5,k3);	trig_plot(n1,k5,k4);	break;
		case 'V':
			k1=setPp(q,p0+2.*kl+kt);	k2=setPp(q,p0+2.*kl+kz);	k3=setPp(q,p0+2.*kl-kt);
			k4=setPp(q,p0+2.*kl-kz);	k5=setPp(q,p0+1.5*kl);
			trig_plot(n1,k5,k1);	trig_plot(n1,k5,k2);
			trig_plot(n1,k5,k3);	trig_plot(n1,k5,k4);	break;
		case 'O':	// let draw icosahedron
		{
			const int n = 12, m = n/2;	Reserve(n*m);
			register long i,j;
			long *nn=new long[2*n], n1=setPp(q,p0+kl), n2=setPp(q,p0-kl);
			mreal u,v,rr;
			for(i=0;i<m;i++)	for(j=0;j<n;j++)
			{
				if(i>0 && i<m-1)
				{
					u = i*M_PI/(m-1.);	v = 2*M_PI*j/(n-1.)-1;	rr = sin(u);
					nn[j+n]=nn[j];	nn[j]=setPp(q,p0+kl*cos(u)+kt*rr*cos(v)+kz*rr*sin(v));
				}
				else if(i==0)	nn[j] = n1;
				else if(i==m-1)	{	nn[j+n]=nn[j];	nn[j]=n2;	}
				if(i*j>0)	quad_plot(nn[j-1], nn[j], nn[j+n-1], nn[j+n]);
			}
			delete []nn;	break;
		}
	}
}
//-----------------------------------------------------------------------------
