/*
   Support - Provides some big and little endian abstraction functions,
             besides another things.
   Copyright (C) 1999  Roberto Arturo Tena Sanchez <arturo@directmail.org>

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
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307  USA

  Some code was from Caolan, but I have replaced all the code,
  now all code here is mine, so I changed copyright announce in cole-1.0.0.
     Arturo Tena
*/

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>

uint16_t fil_sreadU16(uint8_t * in)
{
    uint16_t ret;
#ifdef WORDS_BIGENDIAN
    *((uint8_t *) (&ret)) = *(in + 1);
    *(((uint8_t *) (&ret)) + 1) = *in;
#else
    ret = *((uint16_t *) in);
#endif
    return ret;
}

uint32_t fil_sreadU32(uint8_t * in)
{
    uint32_t ret;
#ifdef WORDS_BIGENDIAN
    *(((uint8_t *) (&ret)) + 3) = *in;
    *(((uint8_t *) (&ret)) + 2) = *(in + 1);
    *(((uint8_t *) (&ret)) + 1) = *(in + 2);
    *((uint8_t *) (&ret)) = *(in + 3);
#else
    ret = *((uint32_t *) in);
#endif
    return ret;
}

#define MIN(a,b) ((a)<(b) ? (a) : (b))

int
__cole_extract_file(FILE ** file, char **filename, uint32_t size, uint32_t pps_start,
                    uint8_t * BDepot, uint8_t * SDepot, FILE * sbfile,
                    FILE * inputfile)
{
    FILE *ret;
    uint16_t BlockSize, Offset;
    uint8_t *Depot;
    FILE *infile;
    long FilePos;
    size_t bytes_to_copy;
    uint8_t Block[0x0200];
    char template_name[]="rtf2latex-ex-XXXXXX";
    int temp_fd;

    *filename = malloc(strlen(template_name)+1);
    if (*filename == NULL)
        return 1;

	temp_fd = mkstemp(*filename);
	
    if (*filename == NULL) {
        free(*filename);
        return 2;
    }
    
    ret = fdopen(temp_fd, "w+b");
    *file = ret;
    if (ret == NULL) {
        free(*filename);
        return 3;
    }
    
    if (size >= 0x1000) {
        /* read from big block depot */
        Offset = 1;
        BlockSize = 0x0200;
        infile = inputfile;
        Depot = BDepot;
    } else {
        /* read from small block file */
        Offset = 0;
        BlockSize = 0x40;
        infile = sbfile;
        Depot = SDepot;
    }

    while (pps_start < (uint32_t) 0xfffffffd) {
        FilePos = (long) ((pps_start + Offset) * BlockSize);
        if (FilePos < 0) {
            fclose(ret);
            remove(*filename);
            free(*filename);
            return 4;
        }
        
        bytes_to_copy = MIN(BlockSize, size);
        if (fseek(infile, FilePos, SEEK_SET)) {
            fclose(ret);
            remove(*filename);
            free(*filename);
            return 4;
        }
        
        (void) fread(Block, bytes_to_copy, 1, infile);
        if (ferror(infile)) {
            fclose(ret);
            remove(*filename);
            free(*filename);
            return 5;
        }
        
        (void) fwrite(Block, bytes_to_copy, 1, ret);
        if (ferror(ret)) {
            fclose(ret);
            remove(*filename);
            free(*filename);
            return 6;
        }
        
        pps_start = fil_sreadU32(Depot + (pps_start * 4));
        size -= MIN(BlockSize, size);
        if (size == 0)
            break;
    }

    return 0;
}

/*
 * hex dump memory
 *
 * ptr    : location in memory to be shown
 * zero   : zero location ... only affects offsets shown in left column
 * length : number of bytes to display
 * msg    : optional message, can be NULL
 */
void hexdump(void *ptr, void *zero, uint32_t length, char *msg)
{
    unsigned char *pm, *m, *start;
    char buff[18];
    long offset;

    if (!ptr) {
        fprintf(stderr,"Cannot show memory because pointer is NULL\n");
        return;
    }

    m = (unsigned char *) ptr;
    start = (zero) ? (unsigned char *) zero : m;

    buff[8]  = ' ';
    buff[17] = '\0';

    if (msg)
        printf("%s from 0x%p length 0x%08x (%d bytes)\n", msg, m, (unsigned int) length, (int) length);

    for (pm = m; (uint32_t) (pm - m) < length; pm++) {

        /* print offset every 16 bytes */
        offset = (pm - m) % 16;
        if (offset == 0)
            printf("%08lx  ", (unsigned long) ((pm - m) + (m - start)));

        /* write char in the right column buffer */
        buff[offset + (offset < 8 ? 0 : 1)] = (isprint(*pm) ? (char) *pm : '.');

        /* print next char */
        if (!((pm - m + 1) % 16))
            /* print right column */
            printf("%02x  %s\n", *pm, buff);
        else if (!((pm - m + 1) % 8))
            printf("%02x  ", *pm);
        else
            printf("%02x ", *pm);
    }

    offset = (pm - m) % 16;
    if (offset) {
        int i;
        for (i = 0; i < (16 - offset) * 3 - 1; i++)
            printf(" ");
        if (offset != 8)
            buff[offset] = 0;
        printf("  %s\n", buff);
    }
}


