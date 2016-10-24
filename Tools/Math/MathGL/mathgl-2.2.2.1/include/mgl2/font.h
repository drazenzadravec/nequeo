/***************************************************************************
 * font.h is part of Math Graphic Library
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
//-----------------------------------------------------------------------------
#ifndef _MGL_FONT_H_
#define _MGL_FONT_H_

#include "mgl2/define.h"
#include <vector>
//-----------------------------------------------------------------------------
#define MGL_FONT_BOLD		0x01000000	// This value is used binary
#define MGL_FONT_ITAL		0x02000000	// This value is used binary
#define MGL_FONT_BOLD_ITAL	0x03000000
#define MGL_FONT_WIRE		0x04000000
#define MGL_FONT_OLINE		0x08000000	// This value is used binary
#define MGL_FONT_ULINE		0x10000000
#define MGL_FONT_ZEROW		0x20000000	// internal codes
#define MGL_FONT_UPPER		0x40000000
#define MGL_FONT_LOWER		0x80000000
#define MGL_FONT_ROMAN		0xfcffffff
#define MGL_FONT_MASK		0x00ffffff
#define MGL_COLOR_MASK		0xffffff00
#define MGL_FONT_STYLE		0x3f000000
//-----------------------------------------------------------------------------
#ifdef WIN32	// a man ask to use built-in font under Windows
#define MGL_DEF_FONT_NAME	0
#else
#define MGL_DEF_FONT_NAME	"STIX"
#endif
//-----------------------------------------------------------------------------
struct MGL_EXPORT mglTeXsymb	{	unsigned kod;	const wchar_t *tex;	};
const float mgl_fgen = 4*14;
/// Get font color, style and align for internal parser
char mglGetStyle(const char *how, int *font, int *align=0);
class mglBase;
//-----------------------------------------------------------------------------
/// Class for font typeface and text plotting procedures
class MGL_EXPORT mglFont
{
public:
	mglBase *gr;	///< mglBase class used for drawing characters
	mglFont(const char *name=0, const char *path=0);
	~mglFont();
	bool parse;		///< Parse LaTeX symbols

	/// Load font data to memory. Normally used by constructor.
	bool Load(const char *base, const char *path=0);
	/// Free memory
	void Clear();
	/// Copy data from other font
	void Copy(mglFont *);
	/// Restore default font
	void Restore();
	/// Return true if font is loaded
	inline bool Ready() const	{	return numg!=0;	}

	/// Get height of text
	float Height(int font) const;
	/// Get height of text
	float Height(const char *how) const;
	/// Print text string for font specified by string
	float Puts(const char *str,const char *how,float col) const;
	/// Get width of text string for font specified by string
	float Width(const char *str,const char *how) const;
	/// Print text string for font specified by string
	float Puts(const wchar_t *str,const char *how,float col) const;
	/// Get width of text string for font specified by string
	float Width(const wchar_t *str,const char *how) const;

	/// Get internal code for symbol
	long Internal(unsigned s) const;
	/// Return number of glyphs
	inline unsigned GetNumGlyph() const	{	return numg;	};
	/// Return some of pointers
	inline const short *GetTr(int s, long j) const	{	return Buf+tr[s][j];	}
	inline const short *GetLn(int s, long j) const	{	return Buf+ln[s][j];	}
	inline int GetNt(int s, long j) const	{	return numt[s][j];	}
	inline int GetNl(int s, long j) const	{	return numl[s][j];	}
	inline short GetWidth(int s, long j) const	{	return width[s][j];	}
	inline float GetFact(int s) const		{	return fact[s];	}
protected:
	wchar_t *id;	///< Unicode ID for glyph
	int *tr[4];		///< Shift of glyph description by triangles (for solid font)
	int *ln[4];		///< Shift of glyph description by lines (for wire font)
	short *numt[4];	///< Number of triangles in glyph description (for solid font)
	short *numl[4];	///< Number of lines in glyph description (for wire font)
	short *width[4];///< Width of glyph for wire font
	float fact[4];	///< Divider for width of glyph
	unsigned numg;	///< Number of glyphs
	short *Buf;		///< Buffer for glyph descriptions
	long numb;		///< Buffer size

	/// Print text string for font specified by integer constant
	float Puts(const wchar_t *str,int font,int align, float col) const;
	/// Get width of text string for font specified by integer constant
	float Width(const wchar_t *str,int font=0) const;
	/// Replace TeX symbols by its UTF code and add font styles
	void Convert(const wchar_t *str, unsigned *res) const;

	/// Draw string recursively
	/* x,y - position, f - factor, style: 0x1 - italic, 0x2 - bold, 0x4 - overline, 0x8 - underline, 0x10 - empty (not draw) */
	float Puts(const unsigned *str, float x,float y,float f,int style,float col) const;
	/// Parse LaTeX command
	unsigned Parse(const wchar_t *s) const;
	/// Get symbol for character ch with given font style
	unsigned Symbol(char ch) const;
private:
	float get_ptr(long &i,unsigned *str, unsigned **b1, unsigned **b2,float &w1,float &w2, float f1, float f2, int st) const;
	bool read_data(const char *fname, float *ff, short *wdt, short *numl, int *posl, short *numt, int *post, std::vector<short> &buf);
	void main_copy();
	bool read_main(const char *fname, std::vector<short> &buf);
	void mem_alloc();
	bool read_def();
	void draw_ouline(int st, float x, float y, float f, float g, float ww, float ccol) const;
};
//-----------------------------------------------------------------------------
#endif
//-----------------------------------------------------------------------------
