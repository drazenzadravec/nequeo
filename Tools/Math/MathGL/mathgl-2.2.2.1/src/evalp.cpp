/***************************************************************************
 * evalp.cpp is part of Math Graphic Library
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
#include <time.h>
#include <ctype.h>
#include "mgl2/parser.h"
#if MGL_HAVE_GSL
#include <gsl/gsl_sf.h>
#include <gsl/gsl_errno.h>
#endif
//-----------------------------------------------------------------------------
std::wstring mgl_trim_ws(const std::wstring &str);
int mglFormulaError;
mglData MGL_NO_EXPORT mglFormulaCalc(std::wstring string, mglParser *arg, const mglVar *head);
//-----------------------------------------------------------------------------
void mglApplyFunc(mglData &d, double (*func)(double))
{
	long n = d.nx*d.ny*d.nz;
#pragma omp parallel for
	for(long i=0;i<n;i++)	d.a[i] = func(d.a[i]);
}
//-----------------------------------------------------------------------------
mglData mglApplyOper(std::wstring a1, std::wstring a2, mglParser *arg, const mglVar *head, double (*func)(double,double))
{
	const mglData &a = mglFormulaCalc(a1,arg,head), &b = mglFormulaCalc(a2,arg,head);
	long n = mgl_max(a.nx,b.nx), m = mgl_max(a.ny,b.ny), l = mgl_max(a.nz,b.nz);
	mglData r(n, m, l);
	if(b.nx*b.ny*b.nz==1)
#pragma omp parallel for
		for(long i=0;i<a.nx*a.ny*a.nz;i++)	r.a[i] = func(a.a[i],b.a[0]);
	else if(a.nx*a.ny*a.nz==1)
#pragma omp parallel for
		for(long i=0;i<b.nx*b.ny*b.nz;i++)	r.a[i] = func(a.a[0],b.a[i]);
	else if(a.nx==b.nx && b.ny==a.ny && b.nz==a.nz)
#pragma omp parallel for
		for(long i=0;i<n*m*l;i++)	r.a[i] = func(a.a[i], b.a[i]);
	else if(a.nx==b.nx && b.ny*b.nz==1)
#pragma omp parallel for collapse(2)
		for(long j=0;j<a.ny*a.nz;j++)	for(long i=0;i<n;i++)
			r.a[i+n*j] = func(a.a[i+n*j], b.a[i]);
	else if(a.nx==b.nx && a.ny*a.nz==1)
#pragma omp parallel for collapse(2)
		for(long i=0;i<n;i++)	for(long j=0;j<b.ny*b.nz;j++)
			r.a[i+n*j] = func(a.a[i], b.a[i+n*j]);
	else if(a.nx==b.nx && b.ny==a.ny && b.nz==1)
#pragma omp parallel for collapse(3)
		for(long k=0;k<a.nz;k++)	for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			r.a[i+n*(j+m*k)] = func(a.a[i+n*(j+m*k)], b.a[i+n*j]);
	else if(a.nx==b.nx && b.ny==a.ny && a.nz==1)
#pragma omp parallel for collapse(3)
		for(long k=0;k<b.nz;k++)	for(long j=0;j<m;j++)	for(long i=0;i<n;i++)
			r.a[i+n*(j+m*k)] = func(a.a[i+n*j], b.a[i+n*(j+m*k)]);
	return r;
}
//-----------------------------------------------------------------------------
bool mglCheck(std::wstring str)
{
	register long s = 0,i,n=str.length();
	for(i=0;i<n;i++)
	{
		if(str[i]=='(')	s++;
		if(str[i]==')') s--;
		if(s<0)	return false;
	}
	return (s==0) ? true : false;
}
//-----------------------------------------------------------------------------
int mglFindInText(std::wstring str,const char *lst)
{
	register long l=0,r=0,i;//,j,len=strlen(lst);
	for(i=str.length()-1;i>=0;i--)
	{
		if(str[i]=='(') l++;
		if(str[i]==')') r++;
		if(l==r && strchr(lst,str[i]))	return i;
	}
	return -1;
}
//-----------------------------------------------------------------------------
double MGL_NO_EXPORT cand(double a,double b);//	{return a&&b?1:0;}
double MGL_NO_EXPORT cor(double a,double b);//	{return a||b?1:0;}
double MGL_NO_EXPORT ceq(double a,double b);//	{return a==b?1:0;}
double MGL_NO_EXPORT clt(double a,double b);//	{return a<b?1:0;}
double MGL_NO_EXPORT cgt(double a,double b);//	{return a>b?1:0;}
double MGL_NO_EXPORT add(double a,double b);//	{return a+b;}
double MGL_NO_EXPORT sub(double a,double b);//	{return a-b;}
double MGL_NO_EXPORT mul(double a,double b);//	{return a&&b?a*b:0;}
double MGL_NO_EXPORT del(double a,double b);//	{return b?a/b:NAN;}
double MGL_NO_EXPORT ipw(double a,double b);//	{return mgl_ipow(a,int(b));}
double MGL_NO_EXPORT llg(double a,double b);//	{return log(a)/log(b);}
//double MGL_NO_EXPORT asinh(double x);//	{	return log(x+sqrt(x*x+1));	}
//double MGL_NO_EXPORT acosh(double x);//	{	return x>1 ? log(x+sqrt(x*x-1)) : NAN;	}
//double MGL_NO_EXPORT atanh(double x);//	{	return fabs(x)<1 ? log((1+x)/(1-x))/2 : NAN;	}
double MGL_NO_EXPORT gslEllE(double a,double b);//	{return gsl_sf_ellint_E(a,b,GSL_PREC_SINGLE);}
double MGL_NO_EXPORT gslEllF(double a,double b);//	{return gsl_sf_ellint_F(a,b,GSL_PREC_SINGLE);}
double MGL_NO_EXPORT gslLegP(double a,double b);//	{return gsl_sf_legendre_Pl(int(a),b);}
//-----------------------------------------------------------------------------
// It seems that standard wcstombs() have a bug. So, I replace by my own.
void MGL_EXPORT mgl_wcstombs(char *dst, const wchar_t *src, int size)
{
	register int j;
	for(j=0;j<size-1 && src[j]!=0;j++)
		dst[j] = src[j]<0x7f ? src[j] : ' ';
	dst[j] = 0;
}
//-----------------------------------------------------------------------------
const mglVar *FindVar(const mglVar *head, std::wstring &name)
{
	const mglVar *v=head;
	while(v)
	{
		if(v->s==name)	return v;
		v = v->next;
	}
	return 0;
}
//-----------------------------------------------------------------------------
/// Parse string and substitute the script argument
// All numbers are presented as mglData(1). Do boundary checking.
// NOTE: In any case where number is required the mglData::a[0] is used.
// String flag is binary 0x1 -> 'x', 0x2 -> 'y', 0x4 -> 'z'
// NOTE: the speed is not a goal (mglFormula is faster). It is true interpreter!
mglData MGL_NO_EXPORT mglFormulaCalc(std::wstring str, mglParser *arg, const mglVar *head)
{
#if MGL_HAVE_GSL
	gsl_set_error_handler_off();
#endif
	mglData res;
	if(str.empty() || mglFormulaError)	return res;	// nothing to parse
	if(!head && arg)	head = arg->DataList;
	str = mgl_trim_ws(str);
	long n,len=str.length();
	if(str[0]=='(' && mglCheck(str.substr(1,len-2)))	// remove braces
	{	str = str.substr(1,len-2);	len-=2;	}
	if(str[0]=='[')	// this is manual subdata
	{
		mglData a1;
		long i, j, br=0,k;
		bool ar=true,mt=false;
		for(i=1,j=1;i<len-1;i++)
		{
			if(str[i]=='[')	br++;
			if(str[i]==']' && br>0)	br--;
			if(str[i]==',' && !br)
			{
				a1=mglFormulaCalc(str.substr(j,i-j), arg, head);
				if(j==1)
				{	res = a1;	ar = (a1.nx==1);	mt = (a1.nx>1 && a1.ny==1);	}
				else
				{
					if(ar)		// res 1d array
					{	k = res.nx;	res.Insert('x',k);	res.Put(a1,k);	}
					else if(mt)	// res 2d array
					{	k = res.ny;	res.Insert('y',k);	res.Put(a1,-1,k);	}
					else		// res 3d array
					{	k = res.nz;	res.Insert('z',k);	res.Put(a1,-1,-1,k);	}
				}
				j=i+1;
			}
		}
		a1=mglFormulaCalc(str.substr(j,i-j), arg, head);
		if(j==1)
		{	res = a1;	ar = (a1.nx==1);	mt = (a1.nx>1 && a1.ny==1);	}
		else
		{
			if(ar)		// res 1d array
			{	k = res.nx;	res.Insert('x',k);	res.Put(a1,k);	}
			else if(mt)	// res 2d array
			{	k = res.ny;	res.Insert('y',k);	res.Put(a1,-1,k);	}
			else		// res 3d array
			{	k = res.nz;	res.Insert('z',k);	res.Put(a1,-1,-1,k);	}
		}
		return res;
	}

	n=mglFindInText(str,"&|");				// lowest priority -- logical
	if(n>=0)
		return mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, str[n]=='|'?cor:cand);
	n=mglFindInText(str,"<>=");				// low priority -- conditions
	if(n>=0)
		return mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, str[n]=='<'?clt:(str[n]=='>'?cgt:ceq));
	n=mglFindInText(str,"+-");				// normal priority -- additions
	if(n>=0 && (n<2 || str[n-1]!='e' || (str[n-2]!='.' && !isdigit(str[n-2]))))
		return mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, str[n]=='+'?add:sub);
	n=mglFindInText(str,"*/");				// high priority -- multiplications
	if(n>=0)
		return mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, str[n]=='*'?mul:del);
	n=mglFindInText(str,"@");				// high priority -- combine
	if(n>=0)
		return mglFormulaCalc(str.substr(0,n),arg, head).Combine(mglFormulaCalc(str.substr(n+1),arg, head));
	n=mglFindInText(str,"^");				// highest priority -- power
	if(n>=0)
		return mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, ipw);
	n=mglFindInText(str,":");				// highest priority -- array
	if(n>=0 && str.compare(L":"))
	{
		const mglData &a1=mglFormulaCalc(str.substr(0,n), arg, head);
		const mglData &a2=mglFormulaCalc(str.substr(n+1), arg, head);
		res.Create(abs(int(a2.a[0]+0.5)-int(a1.a[0]+0.5))+1);
		res.Fill(a1.a[0], a2.a[0]);
		return res;
	}
	n=mglFindInText(str,".");				// highest priority -- suffixes
	if(n>=0)
	{
		mreal x,y,z,k,v=NAN;
		mglData d = mglFormulaCalc(str.substr(0,n), arg, head);
		const std::wstring &p=str.substr(n+1);
		if(!p.compare(L"a"))			v = d.a[0];
		else if(!p.compare(L"fst"))	{	long i=-1,j=-1,l=-1;	v = d.Find(0,i,j,l);	}
		else if(!p.compare(L"lst"))	{	long i=-1,j=-1,l=-1;	v = d.Last(0,i,j,l);	}
		else if(!p.compare(L"nx"))	v=d.nx;
		else if(!p.compare(L"ny"))	v=d.ny;
		else if(!p.compare(L"nz"))	v=d.nz;
		else if(!p.compare(L"max"))	v=d.Maximal();
		else if(!p.compare(L"min"))	v=d.Minimal();
		else if(!p.compare(L"pmax"))	{	v=d.Maximal();	v = v>0?v:0;	}
		else if(!p.compare(L"pmin"))	{	v=d.MinimalPos();	}
		else if(!p.compare(L"nmax"))	{	v=d.MaximalNeg();	}
		else if(!p.compare(L"nmin"))	{	v=d.Minimal();	v = v<0?v:0;	}
		else if(!p.compare(L"sum"))	v=d.Momentum('x',x,y);
		else if(!p.compare(L"mx"))	{	d.Maximal(x,y,z);	v=x/d.nx;	}
		else if(!p.compare(L"my"))	{	d.Maximal(x,y,z);	v=y/d.ny;	}
		else if(!p.compare(L"mz"))	{	d.Maximal(x,y,z);	v=z/d.nz;	}
		else if(!p.compare(L"ax"))	{	d.Momentum('x',x,y);	v=x/d.nx;	}
		else if(!p.compare(L"ay"))	{	d.Momentum('y',x,y);	v=x/d.ny;	}
		else if(!p.compare(L"az"))	{	d.Momentum('z',x,y);	v=x/d.nz;	}
		else if(!p.compare(L"wx"))	{	d.Momentum('x',x,y);	v=y/d.nx;	}
		else if(!p.compare(L"wy"))	{	d.Momentum('y',x,y);	v=y/d.ny;	}
		else if(!p.compare(L"wz"))	{	d.Momentum('z',x,y);	v=y/d.nz;	}
		else if(!p.compare(L"sx"))	{	d.Momentum('x',x,y,z,k);	v=z/d.nx;	}
		else if(!p.compare(L"sy"))	{	d.Momentum('y',x,y,z,k);	v=z/d.ny;	}
		else if(!p.compare(L"sz"))	{	d.Momentum('z',x,y,z,k);	v=z/d.nz;	}
		else if(!p.compare(L"kx"))	{	d.Momentum('x',x,y,z,k);	v=k/d.nx;	}
		else if(!p.compare(L"ky"))	{	d.Momentum('y',x,y,z,k);	v=k/d.ny;	}
		else if(!p.compare(L"kz"))	{	d.Momentum('z',x,y,z,k);	v=k/d.nz;	}
		else if(!p.compare(L"aa"))	{	d.Momentum('a',x,y);	v=x;	}
		else if(!p.compare(L"wa"))	{	d.Momentum('a',x,y);	v=y;	}
		else if(!p.compare(L"sa"))	{	d.Momentum('a',x,y,z,k);v=z;	}
		else if(!p.compare(L"ka"))	{	d.Momentum('a',x,y,z,k);v=k;	}
		// if this is valid suffix when finish parsing (it can be mreal number)
		if(mgl_isfin(v))	{	res.a[0] = v;	return res;	}
	}
	for(n=0;n<len;n++)	if(str[n]=='(')	break;
	if(n>=len)		// this is number or variable
	{
		const mglVar *v = FindVar(head, str);
		mglNum *f = arg?arg->FindNum(str.c_str()):0;
		if(v)	res = v;
		else if(f)	res.a[0] = f->d;
		else if(!str.compare(L"rnd"))	res.a[0] = mgl_rnd();
		else if(!str.compare(L"nan"))	res.a[0] = NAN;
		else if(!str.compare(L"pi"))	res.a[0] = M_PI;
		else if(!str.compare(L"on"))	res.a[0] = 1;
		else if(!str.compare(L"off"))	res.a[0] = 0;
		else if(!str.compare(L":"))		res.a[0] = -1;
		else res.a[0] = wcstod(str.c_str(),0);	// this is number
		return res;
	}
	else
	{
		std::wstring nm = str.substr(0,n);
		str = str.substr(n+1,len-n-2);	len -= n+2;
		const mglVar *v = FindVar(head, nm);
//		mglVar *v = arg->FindVar(nm.c_str());
		if(!v && !nm.compare(0,7,L"jacobi_"))	nm = nm.substr(7);
		if(v)	// subdata
		{
			if(str[0]=='\'' && str[len-1]=='\'')	// this is column call
			{
				char *buf = new char[len];
				mgl_wcstombs(buf, str.substr(1).c_str(), len-1);	buf[len-1]=0;
				res=v->Column(buf);	delete []buf;
			}
			else
			{
				long m;
				mglData a1, a2, a3;
				a1.a[0] = a2.a[0] = a3.a[0] = -1;
				n=mglFindInText(str,",");
				if(n>0)
				{
					m=mglFindInText(str.substr(0,n),",");
					if(m>0)
					{
						str[m]=0;
						a1 = mglFormulaCalc(str.substr(0,m), arg, head);
						a2 = mglFormulaCalc(str.substr(m+1,n-m-1), arg, head);
						a3 = mglFormulaCalc(str.substr(n+1), arg, head);
					}
					else
					{
						a1 = mglFormulaCalc(str.substr(0,n), arg, head);
						a2 = mglFormulaCalc(str.substr(n+1), arg, head);
					}
				}
				else	a1 = mglFormulaCalc(str, arg, head);
				res = v->SubData(a1,a2,a3);
			}
		}
		else if(nm[0]=='a')	// function
		{
			if(!nm.compare(L"asin"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,asin);	}
			else if(!nm.compare(L"acos"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,acos);	}
			else if(!nm.compare(L"atan"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,atan);	}
			else if(!nm.compare(L"arg"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(n+1),str.substr(0,n),arg, head, atan2);
			}
			else if(!nm.compare(L"abs"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,fabs);	}
				else	res = mglApplyOper(str.substr(n+1),str.substr(0,n),arg, head, hypot);
			}
#if MGL_HAVE_GSL
			else if(!nm.compare(L"ai") || !nm.compare(L"airy_ai"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_airy_Ai(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"airy_dai"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_airy_Ai_deriv(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"airy_bi"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_airy_Bi(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"airy_dbi"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_airy_Bi_deriv(res.a[i],GSL_PREC_SINGLE);	}
		}
		else if(nm[0]=='b')
		{
			if(!nm.compare(L"beta"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_beta);
			}
			else if(!nm.compare(L"bi"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_airy_Bi(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"bessel_i"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Inu);
			}
			else if(!nm.compare(L"bessel_j"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Jnu);
			}
			else if(!nm.compare(L"bessel_k"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Knu);
			}
			else if(!nm.compare(L"bessel_y"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Ynu);
			}
#endif
		}
		else if(nm[0]=='c')
		{
			if(!nm.compare(L"cos"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,cos);	}
			else if(!nm.compare(L"cosh") || !nm.compare(L"ch"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,cosh);	}
#if MGL_HAVE_GSL
			else if(!nm.compare(L"ci"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_Ci);	}
#endif
		}
		else if(nm[0]=='e')
		{
			if(!nm.compare(L"exp"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,exp);	}
#if MGL_HAVE_GSL
			else if(!nm.compare(L"erf"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_erf);	}
//			else if(!nm.compare(L"en"))	Kod=EQ_EN;	// NOTE: not supported
			else if(!nm.compare(L"ee") || !nm.compare(L"elliptic_ec"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_ellint_Ecomp(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"ek") || !nm.compare(L"elliptic_kc"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = gsl_sf_ellint_Kcomp(res.a[i],GSL_PREC_SINGLE);	}
			else if(!nm.compare(L"e") || !nm.compare(L"elliptic_e"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gslEllE);
			}
			else if(!nm.compare(L"elliptic_f"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gslEllF);
			}

			else if(!nm.compare(L"ei"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_expint_Ei);	}
			else if(!nm.compare(L"e1"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_expint_E1);	}
			else if(!nm.compare(L"e2"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_expint_E2);	}
			else if(!nm.compare(L"eta"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_eta);	}
			else if(!nm.compare(L"ei3"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_expint_3);	}
#endif
		}
		else if(nm[0]=='l')
		{
			if(!nm.compare(L"log"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, llg);
			}
			else if(!nm.compare(L"lg"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,log10);	}
			else if(!nm.compare(L"ln"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,log);	}
#if MGL_HAVE_GSL
			else if(!nm.compare(L"li2"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_dilog);	}
			else if(!nm.compare(L"legendre"))
			{
				n=mglFindInText(str,",");
				if(n<=0)	mglFormulaError=true;
				else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gslLegP);
			}
#endif
		}
		else if(nm[0]=='s')
		{
			if(!nm.compare(L"sqrt"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,sqrt);	}
			else if(!nm.compare(L"sin"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,sin);	}
			else if(!nm.compare(L"step"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)	res.a[i] = res.a[i]>0?1:0;	}
			else if(!nm.compare(L"sign"))
			{	res=mglFormulaCalc(str, arg, head);
#pragma omp parallel for
				for(long i=0;i<res.nx*res.ny*res.nz;i++)
					res.a[i] = res.a[i]>0?1:(res.a[i]<0?-1:0);	}
			else if(!nm.compare(L"sinh") || !nm.compare(L"sh"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,sinh);	}
#if MGL_HAVE_GSL
			else if(!nm.compare(L"si"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_Si);	}
			else if(!nm.compare(L"sinc"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_sinc);	}
#endif
		}
		else if(nm[0]=='t')
		{
			if(!nm.compare(L"tg") || !nm.compare(L"tan"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,tan);	}
			else if(!nm.compare(L"tanh") || !nm.compare(L"th"))
			{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,tanh);	}
		}
		else if(!nm.compare(L"pow"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, pow);
		}
		else if(!nm.compare(L"mod"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, fmod);
		}
		else if(!nm.compare(L"int"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,floor);	}
#if MGL_HAVE_GSL
		else if(!nm.compare(L"i"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Inu);
		}
		else if(!nm.compare(L"j"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Jnu);
		}
		else if(!nm.compare(L"k"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Knu);
		}
		else if(!nm.compare(L"y"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gsl_sf_bessel_Ynu);
		}
		else if(!nm.compare(L"f"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(0,n),str.substr(n+1),arg, head, gslEllF);
		}
		else if(!nm.compare(L"hypot"))
		{
			n=mglFindInText(str,",");
			if(n<=0)	mglFormulaError=true;
			else	res = mglApplyOper(str.substr(n+1),str.substr(0,n),arg, head, hypot);
		}
		else if(!nm.compare(L"gamma"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_gamma);	}
		else if(!nm.compare(L"w0"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_lambert_W0);	}
		else if(!nm.compare(L"w1"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_lambert_Wm1);	}
		else if(!nm.compare(L"psi"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_psi);	}
		else if(!nm.compare(L"zeta"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_zeta);	}
		else if(!nm.compare(L"z"))
		{	res=mglFormulaCalc(str, arg, head);	mglApplyFunc(res,gsl_sf_dawson);	}
#endif
	}
	return res;
}
//-----------------------------------------------------------------------------
void MGL_EXPORT mgl_wcslwr(wchar_t *str)
{
	register size_t l=mgl_wcslen(str);
	for(size_t k=0;k<l;k++)
		str[k] = (str[k]>='A' && str[k]<='Z') ? str[k]+'a'-'A' : str[k];
}
//-----------------------------------------------------------------------------
