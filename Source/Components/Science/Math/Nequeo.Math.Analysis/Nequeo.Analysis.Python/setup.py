###########################################################################
# ALGLIB 3.10.0 (source code generated 2015-08-19)
# Copyright (c) Sergey Bochkanov (ALGLIB project).
# 
# >>> SOURCE LICENSE >>>
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation (www.fsf.org); either version 2 of the 
# License, or (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# A copy of the GNU General Public License is available at
# http://www.fsf.org/licensing/licenses
# >>> END OF LICENSE >>>
##########################################################################

from distutils.core import setup
import os
import sys
import ctypes
import shutil

#
# first, we need to copy shared libraries from core directory
#
if sys.platform=="win32" or sys.platform=="cygwin":
    #
    # we are running under windows
    #
    libnames = ["alglib.dll", "alglib"+str(ctypes.sizeof(ctypes.c_void_p)*8)+".dll"]
    targetname = "alglib.dll"
else:
    libnames = ["alglib.so"]
    targetname = "alglib.so"
libname = ""
for s in libnames:
    if os.path.exists(os.path.join("core",s)):
        libname = s
        break
if libname=="":
    sys.stdout.write("ALGLIB installer: unable to detect ALGLIB shared library\n")
    sys.exit(1)
shutil.copyfile(os.path.join("core",libname), targetname)

setup(
    name        =   'alglib',
    description =   'ALGLIB for Python: numerical library',
    author      =   'ALGLIB Project',
    url         =   'http://www.alglib.net/',
    license     =   "GPL 2+ (commercial license available for purchase)",
    py_modules  =   ['xalglib'],
    data_files  =   [('', [targetname])]
    )
