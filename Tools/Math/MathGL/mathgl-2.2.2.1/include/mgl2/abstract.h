/***************************************************************************
 * thread.h is part of Math Graphic Library
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
#ifndef _MGL_ABSTRACT_H_
#define _MGL_ABSTRACT_H_

#include "mgl2/define.h"
//-----------------------------------------------------------------------------
#ifdef __cplusplus
#include <string>
#include "mgl2/type.h"
//-----------------------------------------------------------------------------
class mglBase;
class mglData;
class mglDataC;
class mglParser;
class mglFormula;
class mglFormulaC;
class mglFont;
typedef mglBase* HMGL;
typedef mglData* HMDT;
typedef mglDataC* HADT;
typedef mglParser* HMPR;
typedef mglFormula* HMEX;
typedef mglFormulaC* HAEX;
//-----------------------------------------------------------------------------
#if MGL_NO_DATA_A
#define mglDataA mglData
typedef const mglData* HCDT;
#include "mgl2/data.h"
#else
//-----------------------------------------------------------------------------
/// Callback function for asking user a question. Result shouldn't exceed 1024.
extern MGL_EXPORT void (*mgl_ask_func)(const wchar_t *quest, wchar_t *res);
//-----------------------------------------------------------------------------
/// Abstract class for data array
class MGL_EXPORT mglDataA
{
public:
	virtual ~mglDataA()	{}
	virtual mreal v(long i,long j=0,long k=0) const = 0;
	virtual mreal vthr(long i) const = 0;
	virtual long GetNx() const = 0;
	virtual long GetNy() const = 0;
	virtual long GetNz() const = 0;
	inline long GetNN() const {	return GetNx()*GetNy()*GetNz();	}
	virtual mreal Maximal() const = 0;
	virtual mreal Minimal() const = 0;
	virtual mreal dvx(long i,long j=0,long k=0) const = 0;
//	{	return i>0 ? (i<GetNx()-1 ? (v(i+1,j,k)-v(i-1,j,k))/2 : v(i,j,k)-v(i-1,j,k)) : v(1,j,k)-v(0,j,k);	}
	virtual mreal dvy(long i,long j=0,long k=0) const = 0;
//	{	return j>0 ? (j<GetNy()-1 ? (v(i,j+1,k)-v(i,j-1,k))/2 : v(i,j,k)-v(i,j-1,k)) : v(i,1,k)-v(i,0,k);	}
	virtual mreal dvz(long i,long j=0,long k=0) const = 0;
//	{	return k>0 ? (k<GetNz()-1 ? (v(i,j,k+1)-v(i,j,k-1))/2 : v(i,j,k)-v(i,j,k-1)) : v(i,j,1)-v(i,j,0);	}
};
#endif
typedef const mglDataA* HCDT;
//-----------------------------------------------------------------------------
/// Structure for color ID
struct MGL_EXPORT mglColorID
{
	char id;
	mglColor col;
};
MGL_EXPORT extern mglColorID mglColorIds[31];
MGL_EXPORT extern std::string mglGlobalMess;	///< Buffer for receiving global messages
//-----------------------------------------------------------------------------
/// Brushes for mask with symbol "-+=;oOsS~<>jdD*^" correspondingly
extern uint64_t mgl_mask_val[16];
#define MGL_MASK_ID		"-+=;oOsS~<>jdD*^"
#define MGL_SOLID_MASK	0xffffffffffffffff
//-----------------------------------------------------------------------------
#else
typedef void *HMGL;
typedef void *HMDT;
typedef void *HADT;
typedef void *HMEX;
typedef void *HAEX;
typedef void *HMPR;
typedef const void *HCDT;
#endif

#if MGL_SRC
#define _Da_(d)	(*((const mglDataA *)(d)))
#define _DA_(a)	((const mglDataA *)*(a))
#define _GR_	((mglBase *)(*gr))
//#define _D_(d)	*((mglData *)*(d))
#define _DM_(a)	((mglData *)*(a))
#define _DT_	((mglData *)*d)
#endif

#endif
