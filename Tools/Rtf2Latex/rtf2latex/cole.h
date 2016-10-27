/*
   cole - A free C OLE library.
   Copyright 1998, 1999  Roberto Arturo Tena Sanchez

   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef __COLE_H
#define __COLE_H

#include <stdint.h>
#include <stdio.h>

enum _COLERRNO {
    COLE_EFIRST,            /* Leave this at first place */
    COLE_EMEMORY,           /* Failed malloc(3) or realloc(3) */
    COLE_EOPENFILE,         /* Failed fopen(3) */
    COLE_ECLOSEFILE,        /* Failed fclose(3) */
    COLE_EREAD,             /* Failed fread(3) */
    COLE_EWRITE,            /* Failed fwrite(3) */
    COLE_EREMOVE,           /* Failed remove(3) */
    COLE_ETMPNAM,           /* Failed tmpnam(3) */
    COLE_ESEEK,             /* Failed fseek(3) */
    COLE_EERRNO,            /* Some system call failed */
    COLE_ENOFILESYSTEM,     /* File is not a filesystem */
    COLE_EINVALIDFILESYSTEM,/* Filesystem is broken, it's invalid */
    COLE_EISNOTDIR,         /* Directory entry is not a directory */
    COLE_EISNOTFILE,        /* Directory entry is not a file */
    COLE_EFILENOTFOUND,     /* Doesn't exist a file with the given name */
    COLE_EOF,               /* End of file has been reached */
    COLE_EMEMBERISNOTDIR,   /* A member of the filename is not a directory */
    COLE_EBROKENFILENAME,   /* The filename is not right written */
    COLE_EFILENAMEISNOTFILE,/* Filename is not a file */
    COLE_EFSEEKDELTA,       /* Delta argument is not valid */
    COLE_EFSEEKFLAG,        /* Flag argument is not valid */
    COLE_EUNKNOWN,          /* An unknown error ocurred, can be a bug */
    COLE_ELAST              /* Leave this at last place */
};

typedef enum _COLERRNO COLERRNO;

enum _COLE_SEEK_FLAG
{
    COLE_SEEK_SET,
    COLE_SEEK_FORWARD,
    COLE_SEEK_BACKWARD,
    COLE_SEEK_END
};
typedef enum _COLE_SEEK_FLAG COLE_SEEK_FLAG;

typedef     struct _COLEFS      COLEFS;
typedef     struct _COLEDIR     COLEDIR;
typedef     struct _COLEDIRENT  COLEDIRENT;
typedef     struct _COLEFILE    COLEFILE;

typedef     int (COLE_RECURSE_DIR_FUNC)       (COLEDIR *coledir, void *info, COLERRNO *colerrno);
typedef     int (COLE_RECURSE_DIRENT_FUNC)    (COLEDIRENT *coledirentry, void *info, COLERRNO *colerrno);
typedef     int (COLE_RECURSE_VISIT_DIR_FUNC) (COLEDIR *coledir, void *info);
typedef     void (COLE_LOCATE_ACTION_FUNC)    (COLEDIRENT *coledirentry, void *info);

/* ***********
 * filesystem
 * ***********/
void     cole_perror        (const char *s, COLERRNO colerrno, char *filename);

COLEFS * cole_mount      (char *filename, COLERRNO *colerrno);
int      cole_umount     (COLEFS *colefilesystem, COLERRNO *colerrno);
void     cole_print_tree (COLEFS *colefilesystem, COLERRNO *colerrno);

int      cole_locate_filename   (COLEFS *colefilesystem,
                    char *filename,
                    void *info,
                    COLE_LOCATE_ACTION_FUNC *action,
                    COLERRNO *colerrno);
                    
int      cole_recurse_tree  (COLEFS *colefilesystem,
                    void *info,
                    COLE_RECURSE_DIR_FUNC *inroot,
                    COLE_RECURSE_DIRENT_FUNC *indirentry,
                    COLE_RECURSE_DIR_FUNC *indir,
                    COLE_RECURSE_DIR_FUNC *outdir,
                    COLE_RECURSE_VISIT_DIR_FUNC *visitdir,
                    COLERRNO *colerrno);


COLEFILE *   cole_fopen          (COLEFS *colefilesystem, char *filename, COLERRNO *colerrno);
COLEFILE *   cole_fopen_direntry (COLEDIRENT *coledirentry, COLERRNO *colerrno);
int          cole_fclose         (COLEFILE *colefile, COLERRNO *colerrno);
uint32_t     cole_fsize          (COLEFILE *colefile);
uint32_t     cole_fread          (COLEFILE *colefile, void *ptr, uint32_t size, COLERRNO *colerrno);
int          cole_feof           (COLEFILE *colefile);
uint32_t     cole_ftell          (COLEFILE *colefile);
int          cole_fseek          (COLEFILE *colefile, uint32_t delta, COLE_SEEK_FLAG direction, COLERRNO *colerrno);
int          cole_frewind        (COLEFILE *colefile, COLERRNO *colerrno);


COLEDIR *    cole_opendir_rootdir    (COLEFS *colefilesystem, COLERRNO *colerrno);
COLEDIR *    cole_opendir_direntry   (COLEDIRENT *coledirentry, COLERRNO *colerrno);
int          cole_closedir           (COLEDIR *coledir, COLERRNO *colerrno);
COLEDIRENT * cole_visiteddirentry    (COLEDIR *coledir);
COLEDIRENT * cole_nextdirentry       (COLEDIR *coledir);
char *       cole_dir_getname        (COLEDIR *coledir);
uint32_t     cole_dir_getsize        (COLEDIR *coledir);
uint32_t     cole_dir_getdays1       (COLEDIR *coledir);
uint32_t     cole_dir_getsec1        (COLEDIR *coledir);
uint32_t     cole_dir_getdays2       (COLEDIR *coledir);
uint32_t     cole_dir_getsec2        (COLEDIR *coledir);
char *       cole_direntry_getname   (COLEDIRENT *coledirentry);
uint32_t     cole_direntry_getsize   (COLEDIRENT *coledirentry);
uint32_t     cole_direntry_getdays1  (COLEDIRENT *coledirentry);
uint32_t     cole_direntry_getsec1   (COLEDIRENT *coledirentry);
uint32_t     cole_direntry_getdays2  (COLEDIRENT *coledirentry);
uint32_t     cole_direntry_getsec2   (COLEDIRENT *coledirentry);
int          cole_direntry_isdir     (COLEDIRENT *coledirentry);
int          cole_direntry_isfile    (COLEDIRENT *coledirentry);

#endif
