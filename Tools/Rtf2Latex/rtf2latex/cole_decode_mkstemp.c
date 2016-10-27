/*
   OLEdecode - Decode Microsoft OLE files into its components.
   Copyright (C) 1998, 1999  Andrew Scriven <andy.scriven@research.natpower.co.uk>

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

   *Extremely* modified by Arturo Tena <arturo@directmail.org>
   Modified by Ujwal S. Sathyam for rtf2latex2e <setlur@bigfoot.com>
 */

#include <stdio.h>
#include <stdarg.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <assert.h>

#include "cole.h"
#include "cole_support.h"

#define ENTRYCHUNK 20  /* root_list and sbd_list entries added, must be at least 1 */

#define MIN(a,b) ((a)<(b) ? (a) : (b))

static int reorder_pps_tree(pps_entry * root_pps, uint16_t level);
static void ends(void);

#ifdef COLE_VERBOSE
static void verbosePPSTree(pps_entry * pps_list, uint32_t root_pps, int level);
#endif

static FILE *input, *sbfile;
static uint8_t *Block, *Blockx, *BDepot, *SDepot, *Root;
static uint32_t num_of_pps;
static uint32_t *sbd_list, *root_list;
static pps_entry *pps_list;

/*
   Create a OLE stream tree from a file.
   Input: char *Olefilename        = File to be decoded (ie. .xsl, .doc, .ppt).
   .      pps_entry ** stream_list = The stream tree.
   .      uint32_t * root          = The number of root dir in stream_list.
   .      uint8_t **_BDepot, 
   .      uint8_t **_SDepot, 
   .      FILE **_sbfile, 
   .      char **_sbfilename,
   .      FILE **_input,            = Exposes internals, read only.
   .      uint16_t max_level        = The maximum level on stream tree in which
   .                                 streams will be extracted. 0 means extract all.
   Output: 0 = Sucess.
   .       4 = Couldn't open OLEfilename file (can use perror).
   .       8 = OLEfilename file seems to contain plain text, not OLE file.
   .       9 = OLEfilename is a binary file, but it have not OLEfile format.
   .       5 = Error reading from file, means OLEfilename file has a faulty
   .           OLE file format (UPDATE: not always).
   .       6 = Error removing temporal files.  <-- this is never returned now
   .       7 = Error creating temporal files, can use perror.
   .       10 = Error allocating memory, there's no more memory.
 */

int
__OLEdecode(char *OLEfilename, pps_entry ** stream_list, size_t * root,
            uint8_t ** _BDepot, uint8_t ** _SDepot, FILE ** _sbfile,
            char **_sbfilename, FILE ** _input, uint16_t max_level)
{
    int c;
    uint32_t num_bbd_blocks, num_xbbd_blocks, bl, i, j, len;
    uint8_t *s, *p, *t;
    long FilePos;      /*long because second argument of fseek is long */
    char template_name[]="rtf2latex-sb-XXXXXX";

    /* initialize static variables */
    input = sbfile = NULL;
    Block = Blockx = BDepot = SDepot = Root = NULL;
    pps_list = NULL;
    num_of_pps = 0;
    root_list = sbd_list = NULL;
    *stream_list = NULL;

    /* open input file */
    verbose("open input file");
    verboseS(OLEfilename);
    test_exitf(OLEfilename != NULL, 4, ends());
    input = fopen(OLEfilename, "rb");
    test_exitf(input != NULL, 4, ends());
    *_input = input;

    /* fast check type of file */
    verbose("fast testing type of file");
    test_exitf((c = getc(input)) != EOF, 5, ends());
    test_exitf(ungetc(c, input) != EOF, 5, ends());
    test_exitf(c == 0xd0, 9, ends());

    /* read header block */
    verbose("read header block");
    Block = (uint8_t *) malloc(0x0200);
    test_exitf(Block != NULL, 10, ends());
    (void) fread(Block, 0x0200, 1, input);
    test_exitf(!ferror(input), 5, ends());

    /* really check type of file */
    rewind(input);
    verbose("testing type of file");
    
    verboseU32(fil_sreadU32(Block));
    test_exitf(fil_sreadU32(Block) != (uint32_t) 0xd0cf11e0, 9, ends());
    verboseU32(fil_sreadU32(Block + 0x04));
    test_exitf(fil_sreadU32(Block + 0x04) != (uint32_t) 0xa1b11ae1, 9, ends());

    /* read big block depot */
    verbose("read big block depot (bbd)");
    num_bbd_blocks = fil_sreadU32(Block + 0x2c);
    num_xbbd_blocks = fil_sreadU32(Block + 0x48);
    verboseU32(num_bbd_blocks);
    verboseU32(num_xbbd_blocks);
    BDepot = malloc(0x0200 * (num_bbd_blocks + num_xbbd_blocks));
    test_exitf(BDepot != NULL, 10, ends());
    *_BDepot = BDepot;
    s = BDepot;
    
    assert(num_bbd_blocks <= (0x0200 / 4 - 1) * num_xbbd_blocks + (0x0200 / 4) - 19);
    
    /* the first 19 uint32_t in header does not belong to bbd_list */
    for (i = 0; i < MIN(num_bbd_blocks, 0x0200 / 4 - 19); i++) {
        FilePos = (long) (0x0200 * (1 + fil_sreadU32(Block + 0x4c + (i * 4))));
        assert(FilePos >= 0);
        test_exitf(!fseek(input, FilePos, SEEK_SET), 5, ends());
        (void) fread(s, 0x0200, 1, input);
        test_exitf(!ferror(input), 5, ends());
        s += 0x0200;
    }

    Blockx = (uint8_t *) malloc(0x0200);
    test_exitf(Blockx != NULL, 10, ends());
    bl = fil_sreadU32(Block + 0x44);
    for (i = 0; i < num_xbbd_blocks; i++) {
        FilePos = (long) (0x0200 * (1 + bl));
        assert(FilePos >= 0);
        test_exitf(!fseek(input, FilePos, SEEK_SET), 5, ends());
        (void) fread(Blockx, 0x0200, 1, input);
        test_exitf(!ferror(input), 5, ends());

        for (j = 0; j < 0x0200 / 4 - 1; j++) {
            if (fil_sreadU32(Blockx + (j * 4)) == (uint32_t) 0xfffffffe ||
                fil_sreadU32(Blockx + (j * 4)) == (uint32_t) 0xfffffffd ||
                fil_sreadU32(Blockx + (j * 4)) == (uint32_t) 0xffffffff)
                break;
                
            FilePos = (long) (0x0200 * (1 + fil_sreadU32(Blockx + (j * 4))));
            assert(FilePos >= 0);
            test_exitf(!fseek(input, FilePos, SEEK_SET), 5, ends());
            (void) fread(s, 0x0200, 1, input);
            test_exitf(!ferror(input), 5, ends());
            s += 0x0200;
        }

        bl = fil_sreadU32(Blockx + 0x0200 - 4);
    }
    verboseU8Array(BDepot, (num_bbd_blocks + num_xbbd_blocks), 0x0200);

    /* extract the sbd block list */
    verbose("extract small block depot (sbd) block list");
    sbd_list = malloc(ENTRYCHUNK * 4);
    test_exitf(sbd_list != NULL, 10, ends());
    sbd_list[0] = fil_sreadU32(Block + 0x3c);
    
    /* -2 == 0xfffffffe (as uint32_t) */
    for (len = 1; sbd_list[len - 1] != (uint32_t) 0xfffffffe; len++) {
        test_exitf(len != 0, 5, ends());        /* means file is too big */
        
        /* if memory allocated in sbd_list is all used, allocate more memory */
        if (!(len % ENTRYCHUNK)) {
            uint32_t *newspace;
            newspace = realloc(sbd_list, (1 + len / ENTRYCHUNK) * ENTRYCHUNK * 4);
            test_exitf(newspace != NULL, 10, ends());
            sbd_list = newspace;
        }
        sbd_list[len] = fil_sreadU32(BDepot + (sbd_list[len - 1] * 4));
        verboseU32 (len); 
        verboseU32 (sbd_list[0]); 
        verboseU32 (sbd_list[1]); 
        if (sbd_list[len] != (uint32_t) 0xfffffffe)
            test_exitf(sbd_list[len] <= num_bbd_blocks * 0x0200 - 4, 5,
                       ends());
        test_exitf(sbd_list[len] != (uint32_t) 0xfffffffd
                   && sbd_list[len] != (uint32_t) 0xffffffff, 5, ends());
    }
    
    len--;
    verboseU32Array(sbd_list, len + 1);
    
    /* read in small block depot, if there's any small block */
    if (len == 0) {
        SDepot = NULL;
        verbose ("no small block depot (sbd): there's no small blocks");
    } else {
        verbose("reading small block depot (sbd)");
        SDepot = malloc(0x0200 * len);
        test_exitf(SDepot != NULL, 10, ends());
        s = SDepot;
        for (i = 0; i < len; i++) {
            FilePos = (long) (0x0200 * (1 + sbd_list[i]));
            assert(FilePos >= 0);
            test_exitf(!fseek(input, FilePos, SEEK_SET), 5, ends());
            (void) fread(s, 0x0200, 1, input);
            test_exitf(!ferror(input), 5, ends());
            s += 0x200;
        }
        verboseU8Array(SDepot, len, 0x0200);
    }
    *_SDepot = SDepot;


    /* extract the root block list */
    verbose("extracting root block depot (root) block list");
    root_list = malloc(ENTRYCHUNK * 4);
    test_exitf(root_list != NULL, 10, ends());
    root_list[0] = fil_sreadU32(Block + 0x30);
    for (len = 1; root_list[len - 1] != (uint32_t) 0xfffffffe; len++) {
        test_exitf(len != 0, 5, ends());        /* means file is too long */
        
        /* if memory allocated in root_list is all used, allocate more memory */
        if (!(len % ENTRYCHUNK)) {
            uint32_t *newspace;
            newspace = realloc(root_list, (1 + len / ENTRYCHUNK) * ENTRYCHUNK * 4);
            test_exitf(newspace != NULL, 10, ends());
            root_list = newspace;
        }
        root_list[len] = fil_sreadU32(BDepot + (root_list[len - 1] * 4));
        verboseU32(root_list[len]);
        test_exitf(root_list[len] != (uint32_t) 0xfffffffd 
                   && root_list[len] != (uint32_t) 0xffffffff, 5, ends());
    }
    len--;
    verboseU32Array(root_list, len + 1);
    
    /* read in root block depot */
    verbose("reading root block depot (Root)");
    Root = malloc(0x0200 * len);
    test_exitf(Root != NULL, 10, ends());
    s = Root;
    for (i = 0; i < len; i++) {
        FilePos = (long) (0x0200 * (root_list[i] + 1));
        assert(FilePos >= 0);
        test_exitf(!fseek(input, FilePos, SEEK_SET), 5, ends());
        (void) fread(s, 0x0200, 1, input);
        test_exitf(!ferror(input), 5, ends());
        s += 0x200;
    }
    verboseU8Array(Root, len, 0x0200);

    /* assign space for pps list */
    verbose("allocating pps list");
    num_of_pps = len * 4;       /* each sbd block have 4 pps */
    *stream_list = pps_list = malloc(num_of_pps * sizeof(pps_entry));
    test_exitf(pps_list != NULL, 10, ends());
    
    /* read pss entry details and look out for "Root Entry" */
    verbose("reading pps entries");
    for (i = 0; i < num_of_pps; i++) {
        uint16_t size_of_name;

        s = Root + (i * 0x80);

        /* read the number */
        pps_list[i].ppsnumber = i;

        /* read the name */
        size_of_name = MIN((uint16_t) 0x40, fil_sreadU16(s + 0x40));
        pps_list[i].name[0] = 0;
        if (size_of_name == 0)
            continue;
        for (p = (uint8_t *) pps_list[i].name, t = s; t < s + size_of_name; t++)
            *p++ = *t++;

        /* read the pps type */
        pps_list[i].type = *(s + 0x42);
        if (pps_list[i].type == 5) {
            assert(i == 0);
            *root = i;          /* this pps is the root */
        }

        pps_list[i].previous = fil_sreadU32(s + 0x44);
        pps_list[i].next     = fil_sreadU32(s + 0x48);
        pps_list[i].dir      = fil_sreadU32(s + 0x4c);
        pps_list[i].start    = fil_sreadU32(s + 0x74);
        pps_list[i].size     = fil_sreadU32(s + 0x78);
        pps_list[i].seconds1 = fil_sreadU32(s + 0x64);
        pps_list[i].seconds2 = fil_sreadU32(s + 0x6c);
        pps_list[i].days1    = fil_sreadU32(s + 0x68);
        pps_list[i].days2    = fil_sreadU32(s + 0x70);
    }

#ifdef COLE_VERBOSE
    {
        uint32_t i;
        printf("before reordering pps tree\n");
        printf("pps    type    prev     next      dir start   level size     name\n");
        
        for (i = 0; i < num_of_pps; i++) {
            if (!pps_list[i].name[0]) {
                printf(" -\n");
                continue;
            }
            printf("%08x ", pps_list[i].ppsnumber);
            printf("%2d ",  pps_list[i].type);
            printf("%08x ", pps_list[i].previous);
            printf("%08x ", pps_list[i].next);
            printf("%08x ", pps_list[i].dir);
            printf("%08x ", pps_list[i].start);
            printf("%04x ", pps_list[i].level);
            printf("%08x ", pps_list[i].size);
            printf("'%c", !isprint(pps_list[i].name[0]) ? ' ' : pps_list[i].name[0]);
            printf("%s'\n", pps_list[i].name + 1);
        }
    }
#endif

    /* go through the tree made with pps entries, and reorder it so only the
       next link is used (move the previous-link-children to the last visited
       next-link-children) */
    test_exitf(reorder_pps_tree(&pps_list[*root], 0), 9, ends());

#ifdef COLE_VERBOSE
    {
        uint32_t i;
        printf("after reordering pps tree\n");
        printf("pps    type    prev     next      dir start   level size     name\n");
        
        for (i = 0; i < num_of_pps; i++) {
            if (!pps_list[i].name[0]) {
                printf(" -\n");
                continue;
            }
            printf("%08x ", pps_list[i].ppsnumber);
            printf("%2d ",  pps_list[i].type);
            printf("%08x ", pps_list[i].previous);
            printf("%08x ", pps_list[i].next);
            printf("%08x ", pps_list[i].dir);
            printf("%08x ", pps_list[i].start);
            printf("%04x ", pps_list[i].level);
            printf("%08x ", pps_list[i].size);
            printf("'%c", !isprint(pps_list[i].name[0]) ? ' ' : pps_list[i].name[0]);
            printf("%s'\n", pps_list[i].name + 1);
        }
    }

    verbosePPSTree(pps_list, *root, 0);
#endif


    /* generates pps real files */
    /* NOTE: by this moment, the pps tree,
       wich is made with pps_list entries, is reordered */
    verbose("create pps files");
    {
        uint8_t *Depot;
        FILE *OLEfile, *infile;
        uint16_t BlockSize, Offset;
        size_t bytes_to_read;
        uint32_t pps_size, pps_start;

        assert(num_of_pps >= 1);
        /* i < 1 --- before i < num_of_pps --- changed so not to generate the
           real files by now --- cole 2.0.0 */
        /* may be later we can rewrite this code in order to only extract the
           sbfile, may be using __cole_extract_file call to avoid duplicated
           code --- cole 2.0.0 */
           
        for (i = 0; i < 1; i++) {
            pps_list[i].filename[0] = 0;

            /* storage pps and non-valid-pps (except root) does not need files */
            /* because FlashPix file format have a root of type 5 but with no name,
               we must to check if the non-valid-pps is root */
            if (pps_list[i].type == 1 || (!pps_list[i].name[0] && pps_list[i].type != 5))
                continue;
            /* pps that have level > max_level will not be extracted */
            if (max_level != 0 && pps_list[i].level > max_level)
                continue;

            pps_size = pps_list[i].size;
            pps_start = pps_list[i].start;

			/* FIXME can pps_start point to a block larger than sbfile input ? */

        	assert(pps_list[i].type == 5);
            /* create the new file */
            /* root entry, sbfile must be generated */
            {
            	int temp_fd;
                if (SDepot == NULL) {
                    /* if there are no small blocks, do not generate sbfile */
                    *_sbfilename = NULL;
                    *_sbfile = NULL;
                    break;
                }
                assert(i == *root);
                assert(i == 0);
                *_sbfilename = malloc(strlen(template_name)+1);
                strcpy(*_sbfilename, template_name);
                test_exitf(*_sbfilename != NULL, 10, ends());
                temp_fd = mkstemp(*_sbfilename);
                test_exitf(*_sbfilename[0], 7, ends());
                sbfile = OLEfile = fdopen(temp_fd, "wb+");
                *_sbfile = sbfile;
                test_exitf(OLEfile != NULL, 7, ends());
                verboseS(*_sbfilename);
            } 

            if (pps_size >= 0x1000 /*is in bbd */  ||
                OLEfile == sbfile /*is root */ ) {
                /* read from big block depot */
                Offset = 1;
                BlockSize = 0x0200;
                assert(input != NULL);
                assert(BDepot != NULL);
                infile = input;
                Depot = BDepot;
            } else {
                /* read from small block file */
                Offset = 0;
                BlockSize = 0x40;
                assert(sbfile != NULL);
                assert(SDepot != NULL);
                infile = sbfile;
                Depot = SDepot;
            }

            /* -2 signed long int == 0xfffffffe unsinged long int */
            while (pps_start != (uint32_t) 0xfffffffe) {
#ifdef COLE_VERBOSE
                printf("reading pps %08x block %08x from %s\n",
                       pps_list[i].ppsnumber, pps_start,
                       Depot ==
                       BDepot ? "big block depot" : "small block depot");
#endif
                FilePos = (long) (pps_start + Offset) * BlockSize;
                assert(FilePos >= 0);
                bytes_to_read = MIN(BlockSize, pps_size);
                fseek(infile, FilePos, SEEK_SET);
                (void) fread(Block, bytes_to_read, 1, infile);
                test_exitf(!ferror(infile), 5, ends());
                (void) fwrite(Block, bytes_to_read, 1, OLEfile);
                test_exitf(!ferror(infile), 5, ends());
                pps_start = fil_sreadU32(Depot + (pps_start * 4));
                pps_size -= MIN(BlockSize, pps_size);
                if (pps_size == 0)
                    pps_start = 0xfffffffe;
            }
            
            /* if small block file generated, rewind because we will read it later */
            if (OLEfile == sbfile)
                rewind(OLEfile);       
            /*else if (!fclose (OLEfile)) */
            /* close the pps file */
            /* don't know what to do here */
            /*; */
        }
    }
    ends();
    return 0;
}


/* reorder pps tree, from tree structure to a linear one,
 * and write the level numbers, returns zero if OLE format fails,
 * returns no zero if success 
 *
 * not sure if it is safe declare last_next_link_visited
 * inside reorder_pps_tree function 
 *
 * in next, previous and dir link, 0xffffffff means NULL
 */
static uint32_t *last_next_link_visited;

int reorder_pps_tree(pps_entry * node, uint16_t level)
{
    node->level = level;

    /* reorder subtrees, if there is any */
    if (node->dir != (uint32_t) 0xffffffff) {
        if (node->dir > num_of_pps || !pps_list[node->dir].name[0])
            return 0;
        if (!reorder_pps_tree(&pps_list[node->dir], level + 1))
            return 0;
    }

    /* reorder next-link subtree, saving the most next link visited */
    if (node->next != (uint32_t) 0xffffffff) {
        if (node->next > num_of_pps || !pps_list[node->next].name[0])
            return 0;
        else if (!reorder_pps_tree(&pps_list[node->next], level))
            return 0;
    } else
        last_next_link_visited = &node->next;

    /* move the prev child to the next link and reorder it, if there's any */
    if (node->previous != (uint32_t) 0xffffffff) {
        if (node->previous > num_of_pps || !pps_list[node->previous].name[0])
            return 0;

        *last_next_link_visited = node->previous;
        node->previous = (uint32_t) 0xffffffff;
        if (!reorder_pps_tree(&pps_list[*last_next_link_visited], level))
            return 0;
    }
    return 1;
}

#ifdef COLE_VERBOSE
/*
   Input: pps_list: stream list.
          root_pps: root pps.
          level:    how many levels will be extracted.
 */
void verbosePPSTree(pps_entry * pps_list, uint32_t start_entry, int level)
{
    uint32_t entry;
    int i;

    for (entry = start_entry; entry != (uint32_t) 0xffffffff; entry = pps_list[entry].next) {
        if (pps_list[entry].type == 2) {
            for (i = 0; i < level * 3; i++)
                printf(" ");
            printf("FILE %02x %8ul '%c%s'\n", 
                   (unsigned int) pps_list[entry].ppsnumber,
                   pps_list[entry].size,
                   !isprint(pps_list[entry].
                            name[0]) ? ' ' : pps_list[entry].name[0],
                   pps_list[entry].name + 1);
        } else {
            for (i = 0; i < level * 3; i++)
                printf(" ");
            printf("DIR  %02x '%c%s'\n", 
                   (unsigned int) pps_list[entry].ppsnumber,
                   !isprint(pps_list[entry].name[0]) ? ' ' : pps_list[entry].name[0],
                   pps_list[entry].name + 1);
            verbosePPSTree(pps_list, pps_list[entry].dir, level + 1);
        }
    }
}
#endif


/* free memory used (except the pps tree) */
void ends(void)
{
    if (Block) free(Block);
    if (Root) free(Root);
    if (sbd_list) free(sbd_list);
    if (root_list) free(root_list);
}
