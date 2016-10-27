/*
 * RTF-to-LaTeX2e translation writer code.
 * (c) 2011 Scott Prahl
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

/* Once Only Initialization Stuff */

# include <stdint.h>
# include <stdio.h>
# include <string.h>
# include <stdlib.h>
# include <ctype.h>
//# include <unistd.h>

# include "rtf.h"
# include "tokenscan.h"
# include "cole.h"
# include "rtf2latex2e.h"
# include "init.h"

# define  PREF_FILE_NAME    "r2l-pref"
# define  STYLE_FILE_NAME   "r2l-map"
# define  CTRL_FILE_NAME    "rtf-ctrl"
# define  HEAD_FILE_NAME    "r2l-head"


int prefs[pLast];

char *outputMapFileName = NULL;
char *convertTableName = NULL;

const char *prefString[] = {
    "outputMapFileName",          /* 0  */
    "pageWidth",
    "pageLeft",
    "pageRight",
    "convertTextColor",
    "convertPageSize",            /* 5  */
    "convertTextSize",
    "convertTextForm",
    "convertParagraphStyle",
    "convertParagraphIndent",
    "convertInterParagraphSpace", /* 10 */
    "convertLineSpacing",
    "convertHypertext",
    "convertPict",
    "convertEquation",
    "convertAsDirectory",         /* 15 */
    "preambleFirstText",
    "preambleSecondText",
    "preambleDocClass",
    "convertTableName",
    "convertParagraphMargin",     /* 20 */
    "convertParagraphAlignment",
    "convertTextNoTab",
    "convertTableWidths",
    "convertTableAlignment",
    "convertTableBorders"         /* 25 */
};

char *preambleFirstText = NULL;
char *preambleSecondText = NULL;
char *preambleDocClass = NULL;
char *preambleUserText = NULL;
char *preambleEncoding = NULL;
char *preamblePackages = NULL;
char *preambleColorTable = NULL;
char *preambleOurDefs = NULL;

char *Style2LatexOpen[MAX_STYLE_MAPPINGS];
char *Style2LatexClose[MAX_STYLE_MAPPINGS];
char *Style2LatexStyle[MAX_STYLE_MAPPINGS];
int Style2LatexMapIndex[MAX_STYLE_MAPPINGS];

short symCharCode[CHAR_SET_SIZE];          /* Adobe Symbol Font */
short cp1250CharCode[CHAR_SET_SIZE];       /* code page 1250 */
short cp1251CharCode[CHAR_SET_SIZE];       /* code page 1251 */
short cp1252CharCode[CHAR_SET_SIZE];       /* code page 1252 */
short cp1254CharCode[CHAR_SET_SIZE];       /* code page 1254 */
short cp437CharCode[CHAR_SET_SIZE];        /* code page 437 */
short cp850CharCode[CHAR_SET_SIZE];        /* code page 850 */
short cpMacCharCode[CHAR_SET_SIZE];        /* mac character set */
short cpNextCharCode[CHAR_SET_SIZE];       /* NeXt character set */
short cp932CharCode[7915];       /* cp932 character set */
short cp932Index[7915];       /* cp932 character set index */
int cp932IndexSize=7915;

short cp936CharCode[21791];       /* cp936 (GB2312) character set */
short cp936Index[21791];       /* cp932 (GB2312) character set index */
int cp936IndexSize=21791;

static short InitStyleTranslationMap(void)
{
    FILE *f;
    char buf[rtfBufSiz];
    char *styleString, *openString, *closeString;
    short i;
    TSScanner scanner;
    char *scanEscape;

    /* clobber current mapping */
    for (i = 0; i < MAX_STYLE_MAPPINGS; i++) {
        Style2LatexOpen[i] = NULL;
        Style2LatexClose[i] = NULL;
        Style2LatexStyle[i] = NULL;
        Style2LatexMapIndex[i] = -1;
    }

    f = RTFOpenLibFile(STYLE_FILE_NAME, "rb");
    if (!f) return 0;

    /*
     * Turn off scanner's backslash escape mechanism while reading
     * file.  Restore it later.
     */
    TSGetScanner(&scanner);
    scanEscape = scanner.scanEscape;
    scanner.scanEscape = "";
    TSSetScanner(&scanner);

    /* read file */

    i=0;
    while (fgets(buf, (int) sizeof(buf), f)) {
        if (buf[0] == '#') continue;     /* skip comment lines */

        TSScanInit(buf);
        
        styleString = TSScan();
        if (!styleString) continue;             /* skip blank lines */

        openString = TSScan();
        if (!openString) continue;              /* skip incomplete definition */

        closeString = TSScan();
        if (!closeString) continue;             /* skip incomplete definition */
        
        Style2LatexOpen[i] = strdup(openString);
        Style2LatexClose[i] = strdup(closeString);
        Style2LatexStyle[i] = strdup(styleString);
        i++;

        if (i>=MAX_STYLE_MAPPINGS) {
            RTFMsg("More than %d styles in preference file '%s'", MAX_STYLE_MAPPINGS, STYLE_FILE_NAME);
            RTFMsg("skipping the rest", STYLE_FILE_NAME);
            break;
        }
    }
    scanner.scanEscape = scanEscape;
    TSSetScanner(&scanner);
    fclose(f);

    return (1);
}


static int GetPreferenceNum(const char *name)
{
    short i;

    for (i = 0; i < pLast; i++)
        if (strnicmp(name, prefString[i], 26) == 0) return (i);

    fprintf(stderr,"Could not locate preference '%s'\n", name);

    return (-1);
}

static void setPref(const char *name, const char *value)
{
    int n = GetPreferenceNum(name);

    if (n == -1) return;
    
    if (n == pOutputMapFileName) {
    	if (outputMapFileName) free(outputMapFileName);
        outputMapFileName = strdup(value);
        return;
    }

    if (n == pPreambleFirstText) {
    	if (preambleFirstText) free(preambleFirstText);
        preambleFirstText = strdup(value);
        return;
    }

    if (n == pPreambleSecondText) {
    	if (preambleSecondText) free(preambleSecondText);
        preambleSecondText = strdup(value);
        return;
    }

    if (n == pPreambleDocClass) {
    	if (preambleDocClass) free(preambleDocClass);
        preambleDocClass = strdup(value);
        return;
    }

	if (n == pConvertTableName) {
		if (convertTableName) free(convertTableName);
		convertTableName = strdup(value);
	}

    if (n == pPageWidth || n == pPageLeft || n == pPageRight) {
        prefs[n] = (int) 20.0 * 72.0 * atof(value);
        return;
    }

    prefs[n] = strnicmp(value,"true", 5)==0 ? 1 : 0;
}

short ReadPrefFile(char *file)
{
    FILE *f;
    char buf[rtfBufSiz];
    char *name, *seq;
    TSScanner scanner;
    char *scanEscape;

    f = RTFOpenLibFile(file, "rb");
    if (!f) 
        return 0;

    /*
     * Turn off scanner's backslash escape mechanism while reading
     * file.  Restore it later.
     */
    TSGetScanner(&scanner);
    scanEscape = scanner.scanEscape;
    scanner.scanEscape = "";
    TSSetScanner(&scanner);

    while (fgets(buf, rtfBufSiz, f)) {
        if (buf[0] == '#') continue;    /* skip comment lines */

        TSScanInit(buf);
        name = TSScan();
        if (!name) continue;           /* skip blank lines */

        seq = TSScan();
        if (!seq) continue;            /* skip empty settings */

        setPref(name,seq);
    }
    
    scanner.scanEscape = scanEscape;
    TSSetScanner(&scanner);
    fclose(f);

    return 1;
}

/*
 * Read in a file describing the relation between the standard character set
 * and an RTF translator's corresponding output sequences.  Each line consists
 * of a standard character name and the output sequence for that character.
 *
 * thisOutMap is an array of strings into which the sequences should be placed.
 * It should be declared like this in the calling program:
 *
 *      char *thisOutMap[rtfSC_MaxChar];
 *
 * reinit should be non-zero if outMap should be initialized before reading the
 * file, zero otherwise.  (This allows the map to be constructed by reading
 * several files.)  It's assumed that any existing strings in the map were
 * allocated by RTFStrSave().  The map is initialized BEFORE any attempt is
 * made to read the file.
 *
 * If the filename is an absolute pathname, look in the specified location
 * only.  Otherwise try to find the file in the current directory or the
 * library directory.
 */

short RTFReadOutputMap(char *file, char *thisOutMap[], short reinit)
{
    FILE *f;
    char buf[rtfBufSiz];
    char *name, *seq;
    short stdCode;
    short i;
    size_t size;
    TSScanner scanner;
    char *scanEscape;
    char *fn = "RTFReadOutputMap";

    /* clobber current mapping */

    if (reinit) {
        for (i = 0; i < rtfSC_MaxChar; i++) {
            RTFFree(thisOutMap[i]);
            thisOutMap[i] = NULL;
        }
    }

    if ((f = RTFOpenLibFile(file, "rb")) == NULL)
        return (0);

    /*
     * Turn off scanner's backslash escape mechanism while reading
     * file.  Restore it later.
     */
    TSGetScanner(&scanner);
    scanEscape = scanner.scanEscape;
    scanner.scanEscape = "";
    TSSetScanner(&scanner);

    /* (over) allocate space for preambleEncoding */
	fseek(f, 0, SEEK_END);
	size = ftell(f); 
	fseek(f, 0, SEEK_SET);
	preambleEncoding = malloc(size);
	preambleEncoding[0] = '\0';

    /* read file */
    while (fgets(buf, rtfBufSiz, f)) {
        if (buf[0] == '#')      /* skip comment lines */
            continue;
        TSScanInit(buf);
        
        /* check for any special requirements */
        if (buf[0] == '%') {
            if (strcmp((name = TSScan()), "%") == 0)
                continue;       /* skip blank lines */
            strcat(preambleEncoding, ++name);
            strcat(preambleEncoding, "\n");
            continue;
        }
        
        if ((name = TSScan()) == NULL)
            continue;           /* skip blank lines */
            
        if ((stdCode = RTFStdCharCode(name)) < 0) {
            RTFMsg("%s: unknown character name: %s\n", fn, name);
            continue;
        }
        
        if ((seq = TSScan()) == NULL) {
            RTFMsg("%s: malformed output sequence line for character %s\n",
                   fn, name);
            continue;
        }
        
        if ((seq = RTFStrSave(seq)) == NULL)
            RTFPanic("%s: out of memory", fn);
        thisOutMap[stdCode] = seq;
    }
    scanner.scanEscape = scanEscape;
    TSSetScanner(&scanner);
    fclose(f);
    return (1);
}

/*
 * One time initialization of user preferences from r2l-pref
 */
static void InitUserPrefs(void)
{
    prefs[pPageWidth] = 8.5 * 20 * 72;  /*twips*/
    prefs[pPageLeft] = 1.0 * 20 * 72;
    prefs[pPageRight] = 1.0 * 20 * 72;
    prefs[pConvertTextColor] = true;
    prefs[pConvertPageSize] = false;
    prefs[pConvertTextForm] = true;
    prefs[pConvertTextSize] = false;
    prefs[pConvertParagraphStyle] = true;
    prefs[pConvertParagraphIndent] = true;
    prefs[pConvertInterParagraphSpace] = true;
    prefs[pConvertLineSpacing] = true;
    prefs[pConvertHypertext] = true;
    prefs[pConvertPict] = true;
    prefs[pConvertEquation] = true;
    prefs[pConvertAsDirectory] = false;
    prefs[pConvertParagraphMargin] = true;
    prefs[pConvertParagraphAlignment] = true;
    prefs[pConvertTextNoTab] = false;

    outputMapFileName = strdup("latex-encoding");
    convertTableName = strdup("tabular");

    /* read preferences */
    if (ReadPrefFile(PREF_FILE_NAME) == 0) {
        RTFMsg("Cannot read preferences file %s", PREF_FILE_NAME);
        RTFMsg("Using hard-coded defaults", PREF_FILE_NAME);
    	return;
    }
}

/*
 * Read in a file describing an RTF character set map.  Lines consist of pairs
 * associating character names with character values.
 *
 * If the filename is an absolute pathname, look in the specified location
 * only.  Otherwise try to find the file in the current directory or library.
 */

static short RTFReadCharSetMap(char *file, short *stdCodeArray)
{
    FILE *f;
    char buf[rtfBufSiz];
    char *name, *p;
    short stdCode;
    short radix;
    short value;
    short i;
    TSScanner scanner;
    char *scanEscape;
    char *fn = "RTFReadCharSetMap";

    if ((f = RTFOpenLibFile(file, "rb")) == NULL)
        return (0);

    /* clobber current mapping */

    for (i = 0; i < CHAR_SET_SIZE; i++) 
        stdCodeArray[i] = rtfSC_nothing;

    /*
     * Turn off scanner's backslash escape mechanism while reading
     * charset file.  Restore it later.
     */
    TSGetScanner(&scanner);
    scanEscape = scanner.scanEscape;
    scanner.scanEscape = "";
    TSSetScanner(&scanner);

    /* read file */

    while (fgets(buf, (int) sizeof(buf), f) != NULL) {
        if (buf[0] == '#')      /* skip comment lines */
            continue;
        TSScanInit(buf);
        if ((name = TSScan()) == NULL)
            continue;           /* skip blank lines */
        if ((stdCode = RTFStdCharCode(name)) < 0) {
            RTFPanic("%s: unknown character name: %s", fn, name);
            continue;
        }
        if ((p = TSScan()) == NULL) {
            RTFPanic("%s: malformed charset map line for character %s",
                     fn, name);
            continue;
        }
        if (p[1] == '\0')       /* single char - use ascii value */
            value = p[0];
        else {
            radix = 10;
            if (p[0] == '0' && (p[1] == 'x' || p[1] == 'X')) {
                radix = 16;
                p += 2;
            }
            value = 0;
            while (*p != '\0')
                value = value * radix + RTFCharToHex(*p++);
        }
        if (value >= CHAR_SET_SIZE) {
            RTFMsg("%s: character value %d for %s too high\n",
                   fn, value, name);
            RTFPanic("maximum value is %d", CHAR_SET_SIZE - 1);
        }
        stdCodeArray[value] = stdCode;
    }
    scanner.scanEscape = scanEscape;
    TSSetScanner(&scanner);
    fclose(f);

    return (1);
}

static int read932CharCodes(void)
{
    FILE *f;
    int a,b;
    int i;

    if ((f = RTFOpenLibFile("cp932raw.txt", "rb")) == NULL)
        return (0);

	i=0;
	while (!feof(f)) {
		fscanf(f,"%d %d", &a, &b);
		cp932Index[i]= (short)a;
		cp932CharCode[i]=(short)b;
		i++;
	}
	cp932IndexSize = i-1;
	fclose(f);
	return 1;
}

static int read936CharCodes(void)
{
    FILE *f;
    int a,b;
    int i;

    if ((f = RTFOpenLibFile("cp936raw.txt", "rb")) == NULL)
        return (0);

	i=0;
	while (!feof(f)) {
		fscanf(f,"%d %d", &a, &b);
		cp936Index[i]= (short)a;
		cp936CharCode[i]=(short)b;
		i++;
	}
	cp936IndexSize = i-1;
	fclose(f);
	return 1;
}

/*
 * Initialize all character sets needed to interpret the RTF data
 */
static void InitCharSets(void)
{
    if (!RTFReadCharSetMap("rtf-encoding.cp1250", cp1250CharCode))
        RTFPanic("Cannot read character mapping for code page 1250!\n");

    if (!RTFReadCharSetMap("rtf-encoding.cp1251", cp1251CharCode))
        RTFPanic("Cannot read character mapping for code page 1252!\n");

    if (!RTFReadCharSetMap("rtf-encoding.cp1252", cp1252CharCode))
        RTFPanic("Cannot read character mapping for code page 1252!\n");

    if (!RTFReadCharSetMap("rtf-encoding.cp1254", cp1254CharCode))
        RTFPanic("Cannot read character mapping for code page 1254!\n");

    if (!RTFReadCharSetMap("rtf-encoding.mac", cpMacCharCode))
        RTFPanic("Cannot read character mapping for applemac.map!\n");

    if (!RTFReadCharSetMap("rtf-encoding.cp437", cp437CharCode))
        RTFPanic("Cannot read character mapping for code page 437!\n");

    if (!RTFReadCharSetMap("rtf-encoding.cp850", cp850CharCode))
        RTFPanic("Cannot read character mapping for code page 850!\n");

    if (!RTFReadCharSetMap("rtf-encoding.next", cpNextCharCode))
        RTFPanic("Cannot read character mapping for code page NeXt!\n");

    if (!RTFReadCharSetMap("rtf-encoding.symbolfont", symCharCode))
        RTFPanic("Cannot read character mapping for Adobe symbol code page!\n");
        
    read932CharCodes();
    read936CharCodes();
}

static void InitUserText(void)
{
    FILE *f;
    char buf[rtfBufSiz];
    size_t size;

    f = RTFOpenLibFile(HEAD_FILE_NAME, "rb");
    if (f) {

    	/* (over) allocate and read user-defined preamble text */
		fseek(f, 0, SEEK_END);
		size = ftell(f); 
		fseek(f, 0, SEEK_SET);
		preambleUserText = malloc(size);
		preambleUserText[0] = '\0';
	
		while (fgets(buf, (int) sizeof(buf), f) != NULL) {
			if (buf[0] == '#') continue;  /* skip comment lines */
			if (buf[0] == '\0') continue; /* skip blank lines */
			strcat(preambleUserText,buf);
		}
		fclose(f);
    }
}

/*
 * Read in control word lookup table.  Only need to do this once.
 */

static void InitControlWordLookupTable(void)
{
    FILE *f;
    RTFCtrl *rp;
    char buf[rtfBufSiz];
    char *tokBuf, *tokBufEnd;
    short tokBytes;
    short line = 0, i;
    char *p1, *p2, *p3, c;
    TSScanner scanner;
    char *scanEscape;
    char *fn = "LookupInit";

    if ((f = RTFOpenLibFile(CTRL_FILE_NAME, "rb")) == NULL)
        RTFPanic("%s: cannot open %s file.", fn, CTRL_FILE_NAME);

    /*
     * Turn off scanner's backslash escape mechanism while reading
     * file.  Restore it later.
     */
    TSGetScanner(&scanner);
    scanEscape = scanner.scanEscape;
    scanner.scanEscape = "";
    TSSetScanner(&scanner);

    while (fgets(buf, (int) sizeof(buf), f) ) {
        if (buf[0] == '#')      /* skip comments */
            continue;
        ++line;
        TSScanInit(buf);
        p1 = TSScan();
        p2 = TSScan();
        p3 = TSScan();
        /*
         * First non-comment line contains number of control words and
         * number of bytes (including terminating nulls) needed to store
         * control word tokens.
         */
        if (line == 1) {
            if (p2 == NULL)    /* malformed */
                break;
            nCtrls = atoi(p1);
            tokBytes = atoi(p2);
            rtfCtrl = (RTFCtrl **) RTFAlloc(nCtrls * sizeof(RTFCtrl *));
            if (!rtfCtrl) {
                RTFPanic("%s: out of memory.", fn);
                exit(1);
            }

            for (i = 0; i < nCtrls; i++) {
                rp = (RTFCtrl *) RTFAlloc(sizeof(RTFCtrl));
                if (!rp) {
                    RTFPanic("%s: out of memory.", fn);
                    exit(1);
                }
                rtfCtrl[i] = rp;
            }

            /*
             * Allocate a buffer into which to copy all the tokens
             */
            tokBuf = RTFAlloc(tokBytes);
            if (!tokBuf) {
                RTFPanic("%s: out of memory.", fn);
                exit(1);
            }
            tokBufEnd = tokBuf;
        } else {
            if (p3 == NULL)    /* malformed */
                break;
            if (line - 1 > nCtrls)      /* malformed */
                break;
            rp = rtfCtrl[line - 2];
            rp->major = atoi(p1);
            rp->minor = atoi(p2);
            rp->str = tokBufEnd;
            rp->index = line - 2;

            /* process string to remove embedded escapes */
            p1 = p3;
            while ((c = *p1++) != '\0') {
                /*
                 * Escaped character.  Default is to use next
                 * character unmodified, but \n and \r are
                 * turned into linefeed and carriage return.
                 */
                if (c == '\\') {
                    c = *p1++;
                    if (c == 'n') c = '\n';
                    if (c == 'r') c = '\r';
                }
                *tokBufEnd++ = c;
            }
            *tokBufEnd++ = '\0';
        }
    }
    scanner.scanEscape = scanEscape;
    TSSetScanner(&scanner);
    (void) fclose(f);

    if (rtfCtrl == NULL || line - 1 != nCtrls)
        RTFPanic("%s: %s file contents malformed.", fn, CTRL_FILE_NAME);
}

static void InitLatexEncoding(void)
{
    if (RTFReadOutputMap(outputMapFileName, outMap, 1) == 0)
        RTFPanic("Cannot read output map %s", outputMapFileName);
}

/*
 * Copy src to string dst of size siz.  At most siz-1 characters
 * will be copied.  Always NUL terminates (unless siz == 0).
 * Returns strlen(src); if retval >= siz, truncation occurred.
 */
static size_t my_strlcpy(char *dst, const char *src, size_t siz)
{
        char *d = dst;
        const char *s = src;
        size_t n = siz;

        /* Copy as many bytes as will fit */
        if (n != 0) {
                while (--n != 0) {
                        if ((*d++ = *s++) == '\0')
                                break;
                }
        }

        /* Not enough room in dst, add NUL and traverse rest of src */
        if (n == 0) {
                if (siz != 0)
                        *d = '\0';                /* NUL-terminate dst */
                while (*s++)
                        ;
        }

        return(s - src - 1);        /* count does not include NUL */
}

/*
 * Appends src to string dst of size siz (unlike strncat, siz is the
 * full size of dst, not space left).  At most siz-1 characters
 * will be copied.  Always NUL terminates (unless siz <= strlen(dst)).
 * Returns strlen(src) + MIN(siz, strlen(initial dst)).
 * If retval >= siz, truncation occurred.
 */
static size_t my_strlcat(char *dst, const char *src, size_t siz)
{
        char *d = dst;
        const char *s = src;
        size_t n = siz;
        size_t dlen;

        /* Find the end of dst and adjust bytes left but don't go past end */
        while (n-- != 0 && *d != '\0')
                d++;
        dlen = d - dst;
        n = siz - dlen;

        if (n == 0)
                return(dlen + strlen(s));
        while (*s != '\0') {
                if (n != 1) {
                        *d++ = *s;
                        n--;
                }
                s++;
        }
        *d = '\0';

        return(dlen + (s - src));        /* count does not include NUL */
}

/******************************************************************************
 purpose:  returns a new string consisting of s+t
******************************************************************************/
char *strdup_together(const char *s, const char *t)
{
    char *both;
    size_t siz;
    
    if (s == NULL) {
        if (t == NULL)
            return NULL;
        return strdup(t);
    }
    if (t == NULL)
        return strdup(s);

    if (0) fprintf(stderr, "'%s' + '%s'", s, t);
    siz = strlen(s) + strlen(t) + 1;
    both = (char *) malloc(siz);

    if (both == NULL)
        fprintf(stderr, "Could not allocate memory for both strings.");

    my_strlcpy(both, s, siz);
    my_strlcat(both, t, siz);

    return both;
}

char * append_file_to_path(char *path, char *file)
{
    char *s, *t;
    int len;
    
    if (file == NULL) return NULL;
        
    if (path == NULL) return strdup(file);
    
    len = strlen(path);
    if (path[len] == PATH_SEP)
        return strdup_together(path,file);
    
    /* need to insert PATH_SEP */
    s = malloc(len+2);
    strcpy(s,path);
    s[len]=PATH_SEP;
    s[len+1]='\0';
    t = strdup_together(s,file);
    free(s);
    return t;   
}


/****************************************************************************
 * purpose:  append path to .cfg file name and open
             return NULL upon failure,
             return filepointer otherwise
 ****************************************************************************/
static FILE    *
try_path(char *path, char *file, char *mode)
{
    char           *both;
    FILE           *fp = NULL;

    both = append_file_to_path(path, file);
    if (0) fprintf(stderr, "trying both='%s'\n\n", both);
    fp = fopen(both, mode);
    free(both);
    return fp;
}

/****************************************************************************
purpose: open library files by trying multiple paths
 ****************************************************************************/
FILE *OpenLibFile(char *name, char *mode)
{
    char           *env_path, *p, *p1;
    char           *lib_path;
    FILE           *fp;

    /* try path specified on the line */
    fp = try_path(g_library_path, name, mode);
    if (fp)
        return fp;

    /* try the environment variable RTFPATH */
    p = getenv("RTFPATH");
    if (p) {
        env_path = strdup(p);   /* create a copy to work with */
        p = env_path;
        while (p) {
            p1 = strchr(p, ENV_SEP);
            if (p1)
                *p1 = '\0';

            fp = try_path(p, name, mode);
            if (fp) {
                free(env_path);
                return fp;
            }
            p = (p1) ? p1 + 1 : NULL;
        }
        free(env_path);
    }
    
    /* last resort.  try PREFS_DIR from compile time */
    lib_path = strdup(PREFS_DIR);
    if (lib_path) {
        p = lib_path;
        while (p) {
            p1 = strchr(p, ENV_SEP);
            if (p1)
                *p1 = '\0';

            fp = try_path(p, name, mode);
            if (fp) {
                free(lib_path);
                return fp;
            }
            p = (p1) ? p1 + 1 : NULL;
        }
        free(lib_path);
    }
    /* failed ... give some feedback */

	fprintf(stderr, "Cannot find the rtf2latex2e support file '%s'\n", name);
	fprintf(stderr, "\n");
	fprintf(stderr, "The default install location for these files is\n");
	fprintf(stderr, "   /usr/local/share/rtf2latex2e/\n");
	fprintf(stderr, "When this program was compiled the location was\n");
	fprintf(stderr, "   %s\n", PREFS_DIR);
	fprintf(stderr, "\n");
	fprintf(stderr, "After locating the proper directory you can \n");
	fprintf(stderr, "   (1) define the environment variable $RTFPATH, *or*\n");
	fprintf(stderr, "   (2) use command line path option '-P /path/to/prefs' *or*\n");
	fprintf(stderr, "   (3) recompile rtf2latex2e (after modifying datadir in Makefile)\n");
	fprintf(stderr, "Giving up.  Please don't hate me.\n");
	exit(1);
    
    return NULL;
}


/*
 * Things that need to be initialized only once (even if
 * multiple files are being converted
 */
void InitConverter(void)
{
    RTFSetOpenLibFileProc(OpenLibFile); /* install routine for opening library files */
    
	InitCharSets();
	InitControlWordLookupTable();
	InitUserPrefs();
	
	InitUserText();
	InitLatexEncoding();

	InitStyleTranslationMap();
}

