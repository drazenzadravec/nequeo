/* mktex.h: MiKTeX Maker Library

   Copyright (C) 1998-2016 Christian Schenk

   This file is part of the MiKTeX Maker Library.

   The MiKTeX Maker Library is free software; you can redistribute it
   and/or modify it under the terms of the GNU General Public License
   as published by the Free Software Foundation; either version 2, or
   (at your option) any later version.

   The MiKTeX Maker Library is distributed in the hope that it will be
   useful, but WITHOUT ANY WARRANTY; without even the implied warranty
   of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with the MiKTeX Maker Library; if not, write to the Free
   Software Foundation, 59 Temple Place - Suite 330, Boston, MA
   02111-1307, USA. */

#if defined(_MSC_VER)
#  pragma once
#endif

#include <miktex/Definitions>

#if !defined(MKTEXAPI)
#  define MKTEXAPI(func) extern "C" MIKTEXDLLIMPORT int MIKTEXCEECALL func
#endif

MKTEXAPI(makebase) (int argc, const char ** argv);
MKTEXAPI(makefmt) (int argc, const char ** argv);
MKTEXAPI(makemem) (int argc, const char ** argv);
MKTEXAPI(makemf) (int argc, const char ** argv);
MKTEXAPI(makepk) (int argc, const char ** argv);
MKTEXAPI(maketfm) (int argc, const char ** argv);