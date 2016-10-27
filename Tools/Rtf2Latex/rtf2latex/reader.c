/*
 * RTF-to-LaTeX2e translation reader code.
 */

# include       <stdio.h>
# include       <ctype.h>
# include       <string.h>
# include       <stdarg.h>
# include       <stdlib.h>
# include       <stdint.h>

# include       "tokenscan.h"

# define        rtfInternal
# include       "rtf.h"
# undef         rtfInternal

# include       "rtf2latex2e.h"
# include       "init.h"
extern char     texMapQualifier[];

/*
 * Return pointer to new element of type t, or NULL
 * if no memory available.
 */

# define        New(t)  ((t *) RTFAlloc (sizeof (t)))

static void _RTFGetToken(void);
static void _RTFGetToken2(void);
static short GetChar(void);
static void ReadFontTbl(void);
static void ReadStyleSheet(void);
static void ReadInfoGroup(void);
static void ReadPictGroup(void);
static void ReadObjGroup(void);
static void Lookup(char *s);

void DebugMessage(void);

void ExamineToken(char *title);

/*
 * Public variables (listed in rtf.h)
 */

short rtfClass;
short rtfMajor;
short rtfMinor;
int32_t rtfParam;
char *rtfTextBuf = NULL;
short rtfTextLen;

int32_t rtfLineNum;
short rtfLinePos;
short rtfTokenIndex;


/*
 * Private stuff
 */

static short pushedChar;        /* pushback char if read too far */

static short pushedClass;       /* pushed token info for RTFUngetToken() */
static short pushedMajor;
static short pushedMinor;
static int32_t pushedParam;
static char *pushedTextBuf = NULL;

static short prevChar;
static short bumpLine;


static RTFFont *fontList = NULL;    /* these lists MUST be */
static RTFColor *colorList = NULL; /* initialized to NULL */
static RTFStyle *styleList = NULL;


static FILE *rtffp;

static char *inputName = NULL;
static char *outputName = NULL;

static int RTFBraceLevel(int level)
{
    static int braceLevel;
    if (level == -99)
        return braceLevel;

    braceLevel = level;
    return braceLevel;
}

static void RTFSetBraceLevel(int level) 
{
    (void) RTFBraceLevel(level);
}

int RTFGetBraceLevel(void)
{
    return RTFBraceLevel(-99);
}

/*
 * This array is used to map standard character names onto their numeric codes.
 * The position of the name within the array is the code.
 * stdcharnames.h is generated in the ../h directory.
 */

static char *stdCharName[] = {
# include       "stdcharnames.h"
    NULL
};


/*
 * These arrays are used to map RTF input character values onto the standard
 * character names represented by the values.  Input character values are
 * used as indices into the arrays to produce standard character codes.
 */


short *genCharCode = NULL;
short *curCharCode = NULL;

int defaultFontNumber;

/*
 * Initialize the reader.  This may be called multiple times,
 * to read multiple files.  The only thing not reset is the input
 * stream; that must be done with RTFSetStream().
 */
void RTFInit(void)
{
    short i;
    RTFColor *cp;
    RTFFont *fp;
    RTFStyle *sp;
    RTFStyleElt *eltList, *ep;

    rtfClass = -1;
    pushedClass = -1;
    pushedChar = EOF;

    rtfLineNum = 0;
    rtfLinePos = 0;
    prevChar = EOF;
    bumpLine = 0;

    if (!rtfTextBuf) {  /* initialize text buffers */
        rtfTextBuf = RTFAlloc(rtfBufSiz);
        pushedTextBuf = RTFAlloc(rtfBufSiz);
        if (!rtfTextBuf || !pushedTextBuf) {
            RTFPanic("Cannot allocate text buffers.");
            exit(1);
        }
        rtfTextBuf[0] = '\0';
        pushedTextBuf[0] = '\0';
    }

//    RTFFree(inputName);
//    RTFFree(outputName);
//    inputName = outputName = NULL;

    for (i = 0; i < rtfMaxClass; i++)
        RTFSetClassCallback(i, NULL);
    for (i = rtfMinDestination; i <= rtfMaxDestination; i++)
        RTFSetDestinationCallback(i, NULL);

    /* install built-in destination readers */
    RTFSetDestinationCallback(rtfFontTbl, ReadFontTbl);
    RTFSetDestinationCallback(rtfStyleSheet, ReadStyleSheet);
    RTFSetDestinationCallback(rtfInfo, ReadInfoGroup);
    RTFSetDestinationCallback(rtfPict, ReadPictGroup);
    RTFSetDestinationCallback(rtfObject, ReadObjGroup);

    RTFSetReadHook(NULL);

    /* dump old lists if necessary */

    while (fontList != NULL) {
        fp = fontList->rtfNextFont;
        RTFFree(fontList->rtfFName);
        RTFFree((char *) fontList);
        fontList = fp;
    }
    while (colorList != NULL) {
        cp = colorList->rtfNextColor;
        RTFFree((char *) colorList);
        colorList = cp;
    }
    while (styleList != NULL) {
        sp = styleList->rtfNextStyle;
        eltList = styleList->rtfSSEList;
        while (eltList != NULL) {
            ep = eltList->rtfNextSE;
            RTFFree(eltList->rtfSEText);
            RTFFree((char *) eltList);
            eltList = ep;
        }
        RTFFree(styleList->rtfSName);
        RTFFree((char *) styleList);
        styleList = sp;
    }

    genCharCode = cp1252CharCode;
    curCharCode = cp1252CharCode;
    RTFInitStack();
}


/*
 * Set the reader's input stream to the given stream.  Can
 * be used to redirect to other than the default (stdin).
 */
void RTFSetStream(FILE *stream)
{
    rtffp = stream;
}


/*
 * Set or get the input or output file name.  These are never guaranteed
 * to be accurate, only insofar as the calling program makes them so.
 */
void RTFSetInputName(char *name)
{
    inputName = strdup(name);
    if (!inputName)
        RTFPanic("RTFSetInputName: out of memory");
}


char *RTFGetInputName(void)
{
    return (inputName);
}


void RTFSetOutputName(char *name)
{
    outputName = strdup(name);
    if (!outputName)
        RTFPanic("RTFSetOutputName: out of memory");
}


char *RTFGetOutputName(void)
{
    return (outputName);
}


/* ----------------------------------------------------------------------
 * Callback table manipulation routines
 *
 * Install or return a writer callback for a token class
 */


static RTFFuncPtr ccb[rtfMaxClass];     /* class callbacks */


void RTFSetClassCallback(short class, RTFFuncPtr callback)
{
    if (class >= 0 && class < rtfMaxClass)
        ccb[class] = callback;
}


RTFFuncPtr RTFGetClassCallback(short class)
{
    if (class >= 0 && class < rtfMaxClass)
        return (ccb[class]);
    return (NULL);
}


/*
 * Install or return a writer callback for a destination type
 */

static RTFFuncPtr dcb[rtfNumDestinations];      /* destination callbacks */


void RTFSetDestinationCallback(short dest, RTFFuncPtr callback)
{
    if (dest >= rtfMinDestination && dest <= rtfMaxDestination)
        dcb[dest - rtfMinDestination] = callback;
}


RTFFuncPtr RTFGetDestinationCallback(short dest)
{
    if (dest >= rtfMinDestination && dest <= rtfMaxDestination)
        return (dcb[dest - rtfMinDestination]);
    return (NULL);
}

/* ---------------------------------------------------------------------- */

/*
 * Routines to handle mapping of RTF character sets
 * onto standard characters.
 *
 * RTFStdCharCode(name) given char name, produce numeric code
 * RTFStdCharName(code) given char code, return name
 * RTFMapChar(c)        map input (RTF) char code to std code
 *
 */



/* ---------------------------------------------------------------------- */

/*
 * Token reading routines
 */


/*
 * Read the input stream, invoking the writer's callbacks
 * where appropriate.
 */

void RTFRead(void)
{
    while (RTFGetToken() != rtfEOF) {
/*      ExamineToken(); */
        RTFRouteToken();
    }
}

/*
 * Route a token.  If it's a destination for which a reader is
 * installed, process the destination internally, otherwise
 * pass the token to the writer's class callback.
 */
void RTFRouteToken(void)
{
    RTFFuncPtr p;

    if (rtfClass < 0 || rtfClass >= rtfMaxClass) {      /* watchdog */
        RTFPanic("Unknown class %d: %s (reader malfunction)",
                 rtfClass, rtfTextBuf);
    }
    if (RTFCheckCM(rtfControl, rtfDestination)) {
        /* invoke destination-specific callback if there is one */

        if ((p = RTFGetDestinationCallback(rtfMinor))
            != NULL) {
            (*p) ();
            return;
        }
    }
    /* invoke class callback if there is one */
    if ((p = RTFGetClassCallback(rtfClass)) != NULL)
        (*p) ();
}

#define NO_EXECUTE 0
#define EXECUTE 1

/*
 * Skip to the end of the current group.  When this returns,
 * writers that maintain a state stack may want to call their
 * state unstacker; global vars will still be set to the group's
 * closing brace.
 */

static void RTFDoGroup(int execute, int level)
{
    if (level<=0) {fprintf(stderr,"trying doGroup at braceLevel 0??\n"); return;}

    while (RTFGetToken() != rtfEOF) {

        if (RTFGetBraceLevel() < level) 
            return;

        if (execute) RTFRouteToken();
    }
}

void RTFSkipGroup(void)
{
    int level = RTFGetBraceLevel();
    RTFDoGroup(NO_EXECUTE,level);
}

void RTFExecuteGroup(void)
{
    int level = RTFGetBraceLevel();
    RTFDoGroup(EXECUTE,level);
}

void RTFSkipToLevel(int level)
{
    RTFDoGroup(NO_EXECUTE,level);
}

static int RTFToToken(int class, int major, int minor, int execute)
{
    short level = RTFGetBraceLevel();

    if (level==0) {fprintf(stderr,"trying to skip to something a braceLevel 0??\n"); return 0;}

    while (RTFGetToken() != rtfEOF) {

//        ExamineToken("RTFToToken");
        if (RTFCheckCMM(class,major,minor))
            return 1;

        if (RTFGetBraceLevel() < level)
            return 0;

        if (execute) RTFRouteToken();
    }
    return 0;
}

int RTFSkipToToken(int class, int major, int minor)
{
    return RTFToToken(class, major, minor, NO_EXECUTE);
}

int RTFExecuteToToken(int class, int major, int minor)
{
    return RTFToToken(class, major, minor, EXECUTE);
}

void RTFExecuteParentheses(void)
{
    short level = RTFGetBraceLevel();
    int parens = 1;

    if (level==0) {fprintf(stderr,"trying to execute parens at braceLevel 0??\n"); return;}

    while (RTFGetToken() != rtfEOF) {

    	if (rtfClass == rtfText) {
    		if (rtfMajor == '(') parens++;
    		if (rtfMajor == ')') parens--;
    	}
    	
        if (parens == 0) return;

        if (RTFGetBraceLevel() < level) return;

        RTFRouteToken();
    }
}

/* 
 * return the next (non-space) word delimited by whitespace
 */
char *RTFGetTextWord(void)
{
    char word[256];
    int len = 0;
    short level = RTFGetBraceLevel();

    /* Skip over spaces */
    while (rtfClass != rtfText || (rtfClass == rtfText && rtfTextBuf[0] == ' ')) {
        RTFGetToken();
        if (rtfClass == rtfEOF) return NULL;
        if (level > RTFGetBraceLevel()) return NULL;
    }

    /* Collect the word */
    while (rtfClass == rtfText) {
        word[len] = rtfTextBuf[0];
        len++;
        if (RTFGetToken() == rtfEOF) return NULL;
        if (level > RTFGetBraceLevel()) break;
        if (rtfTextBuf[0] == ' ') break;
        if (len >= 256) break;
    }
    word[len]='\0';
    return strdup(word);
}

/* 
 * return text to next unmatched closing brace
 */
char *RTFGetFieldContents(void)
{
    char fieldContents[1024];
    int i;
    int len = 0;
    int braceCount = 0;

    /* Collect the word */
    while (rtfClass == rtfText || rtfClass == rtfGroup || rtfClass == rtfControl) {
        if (RTFGetToken() == rtfEOF) return NULL;

		i=0;
		while (rtfTextBuf[i]) {
        	fieldContents[len] = rtfTextBuf[i];
            len++;
            i++;
        	if (len >= 1023) break;
        }
        
        if (rtfClass == rtfControl) {
        	fieldContents[len] = ' ';
        	len++;
        }
        
        if (rtfClass == rtfGroup && rtfMajor == 0) braceCount++;
        if (rtfClass == rtfGroup && rtfMajor == 1) braceCount--;

    	fieldContents[len]='\0';
        if (braceCount < 0) break;
        if (len >= 1024) break;
    }
    fieldContents[len]='\0';
    return strdup(fieldContents);
}

/*
 * Read one token.  Call the read hook if there is one.  The
 * token class is the return value.  Returns rtfEOF when there
 * are no more tokens.
 */

short RTFGetToken(void)
{
    RTFFuncPtr p;

    for (;;) {
        _RTFGetToken();
        DebugMessage();
        if ((p = RTFGetReadHook()) != NULL)
            (*p) ();            /* give read hook a look at token */

        /* Silently discard newlines, carriage returns, nulls.  */
        if (!(rtfClass == rtfText  && (rtfMajor == '\n' || rtfMajor == '\r')))
            break;
    }
    return (rtfClass);
}

short RTFGetNonWhiteSpaceToken(void)
{
    for (;;) {
        RTFGetToken();
        if (rtfClass != rtfText) break;
        if (rtfMajor != ' ' && rtfMajor != '\n' && rtfMajor != '\r') break;
    }
    return (rtfClass);
}

/*
 * Install or return a token reader hook.
 */

static RTFFuncPtr readHook;


void RTFSetReadHook(RTFFuncPtr f)
{
    readHook = f;
}


RTFFuncPtr RTFGetReadHook(void)
{
    return (readHook);
}


void RTFUngetToken(void)
{
    if (pushedClass >= 0)       /* there's already an ungotten token */
        RTFPanic("cannot unget two tokens");
    if (rtfClass < 0)
        RTFPanic("no token to unget");
    pushedClass = rtfClass;
    pushedMajor = rtfMajor;
    pushedMinor = rtfMinor;
    pushedParam = rtfParam;
    (void) strcpy(pushedTextBuf, rtfTextBuf);
}


short RTFPeekToken(void)
{
    _RTFGetToken();
    RTFUngetToken();
    return (rtfClass);
}

/* 
 * slow, slow lookup because binary search fails for me
 */
static int find932Index(short value)
{
    int i;

    for (i=0; i<cp932IndexSize; i++)
        if (cp932Index[i] == value) return i;

    //short *ptr = (short *)bsearch(&value, cp932Index, cp932IndexSize, sizeof(short), shortcmp);

   // if (ptr) return ptr-cp932Index;

    return -1;
}

static void RTFSet932Token(unsigned char firstByte)
{
    int index932;
    unsigned int value;
    char *s1, *s2;

    /* ASCII ... leave alone */
    if (firstByte<0x80) {
        rtfMinor = cp1252CharCode[firstByte];
        return;
    }

    /* this should not happen */
    if (firstByte==0x80 || firstByte==0xA0 || firstByte==0xFD || firstByte==0xFE || firstByte==0xFF) {
        rtfClass = rtfUnknown;
        rtfParam = rtfNoParam;
        rtfTextBuf[0] = '\0';
        return;
    }

    /* one byte character */
    if (0xA0<firstByte && firstByte<0xE0) {
        value = firstByte;
        index932 = find932Index(value);
    } else {
        s1 = strdup(rtfTextBuf);
        _RTFGetToken2();
        s2 = strdup_together(s1,rtfTextBuf);
        value = firstByte*256+rtfMajor;
        index932 = find932Index(value);
        strcpy(rtfTextBuf,s2);
        free(s2);
        free(s1);
    }

    if (index932 == -1) {
        rtfMinor = '?';
    } else {
        rtfClass = rtfControl;
        rtfMajor = rtfDestination;
        rtfMinor = rtfUnicodeFake;  
        rtfParam = cp932CharCode[index932];
    }
//  fprintf(stderr,"value is %u, index is %d, index[%d]=%u, charcode[%d]=%u\n", value, index932, index932, (unsigned short) cp932Index[index932],index932, (unsigned short) cp932CharCode[index932]);
}

/* 
 * slow, slow lookup because binary search fails for me
 */
static int find936Index(short value)
{
    int i;

    for (i=0; i<cp936IndexSize; i++)
        if (cp936Index[i] == value) return i;

    //short *ptr = (short *)bsearch(&value, cp936Index, cp936IndexSize, sizeof(short), shortcmp);

   // if (ptr) return ptr-cp936Index;

    return -1;
}

static void RTFSet936Token(unsigned char firstByte)
{   
    int index936;
    unsigned int value;
    char *s1, *s2;

    /* ASCII ... leave alone */
    if (firstByte<0x80) {
        rtfMinor = cp1252CharCode[firstByte];
        return;
    }

    s1 = strdup(rtfTextBuf);
    _RTFGetToken2();
    s2 = strdup_together(s1,rtfTextBuf);
    value = firstByte*256+rtfMajor;
    index936 = find936Index(value);
    strcpy(rtfTextBuf,s2);
    free(s2);
    free(s1);

    if (index936 == -1) {
        rtfMinor = '?';
    } else {
        rtfClass = rtfControl;
        rtfMajor = rtfDestination;
        rtfMinor = rtfUnicodeFake;
        rtfParam = cp936CharCode[index936];
    }
//  fprintf(stderr,"value is %u, index is %d, index[%d]=%u, charcode[%d]=%u\n", value, index936, index936, (unsigned short) cp936Index[index936],index936, (unsigned short) cp936CharCode[index936]);
}

static void _RTFGetToken(void)
{
    /* first check for pushed token from RTFUngetToken() */

    if (pushedClass >= 0) {
        rtfClass = pushedClass;
        rtfMajor = pushedMajor;
        rtfMinor = pushedMinor;
        rtfParam = pushedParam;
        (void) strcpy(rtfTextBuf, pushedTextBuf);
        rtfTextLen = (short) strlen(rtfTextBuf);
        pushedClass = -1;
        return;
    }

    /*
     * Beyond this point, no token is ever seen twice, which is
     * important, e.g., for making sure no "}" pops the font stack twice.
     */

    _RTFGetToken2();

    if (rtfClass == rtfText) {   /* map RTF char to standard code */

        if (curCharCode==cp932CharCode) {
            RTFSet932Token(rtfMajor);
            return;
        }

        if (curCharCode==cp936CharCode) {
            RTFSet936Token(rtfMajor);
            return;
        }

        rtfMinor = RTFMapChar(rtfMajor);
        return;
    }

    if (RTFCheckCMM(rtfControl, rtfCharAttr, rtfFontNum)) {
        RTFFont *fp = RTFGetFont(rtfFontNum);
        if (fp) curCharCode = fp->rtfFCharCode;
        return;
    }

    /* \cchs indicates any characters not belonging to the default document character
     * set and tells which character set they do belong to. Macintosh character sets 
     * are represented by values greater than 255. The values for N correspond to the 
     * values for the \ fcharset control word.
     */
    if (RTFCheckCMM(rtfControl, rtfCharAttr, rtfCharCharSet)) {

        if (rtfParam>255) {
            curCharCode = cp1252CharCode;
            return;
        }

        switch (rtfParam) {
        case 1:
            curCharCode = genCharCode;
            break;
        case 2:
            curCharCode = symCharCode;
            break;
        default:
            curCharCode = cp1252CharCode;
            break;
        }
        return;
    }

    if (rtfClass == rtfGroup) {
        switch (rtfMajor) {
        case rtfBeginGroup:
            RTFPushStack();
            break;
        case rtfEndGroup:
            RTFPopStack();
            break;
        }
        return;
    }
}

/* this shouldn't be called anywhere but from _RTFGetToken() */

static void _RTFGetToken2(void)
{
    short sign;
    short c;

    /* initialize token vars */

    rtfClass = rtfUnknown;
    rtfParam = rtfNoParam;
    rtfTextLen = 0;
    rtfTextBuf[0] = '\0';

    /* get first character, which may be a pushback from previous token */

    if (pushedChar != EOF) {
        c = pushedChar;
        rtfTextBuf[rtfTextLen++] = c;
        rtfTextBuf[rtfTextLen] = '\0';
        pushedChar = EOF;
    } else if ((c = GetChar()) == EOF) {
        rtfClass = rtfEOF;
        return;
    }

    if (c == '{') {
        rtfClass = rtfGroup;
        rtfMajor = rtfBeginGroup;
        return;
    }

    if (c == '}') {
        rtfClass = rtfGroup;
        rtfMajor = rtfEndGroup;
        return;
    }

    if (c != '\\') {
        /*
         * Two possibilities here:
         * 1) ASCII 9, effectively like \tab control symbol
         * 2) literal text char
         */
        if (c == '\t') {        /* ASCII 9 */
            rtfClass = rtfControl;
            rtfMajor = rtfSpecialChar;
            rtfMinor = rtfTab;
        } else {
            rtfClass = rtfText;
            rtfMajor = c;
        }
        return;
    }

    /* get the character following the backslash */
    c = GetChar();

    /* \<newline>text         ---> \par text */
    /* \<newline><spaces>text ---> \text */
    if (c == '\n' || c == '\r') {
        while (c == '\n' || c == '\r')  
            c = GetChar();

        if (1||c != ' ') {
            pushedChar = c;
            strcpy(rtfTextBuf,"\\par");
            Lookup("\\par"); 
            return;
        }

        while (c == ' ') 
            c = GetChar();

        rtfTextBuf[1] = c;
        rtfTextBuf[2] = '\0';
        rtfTextLen = 2;
    }


    if (c == EOF) {
        /* early eof, whoops (class is rtfUnknown) */
        return;
    }

    if (!isalpha(c)) {
        /*
         * Four possibilities here:
         * 1) hex encoded text char, e.g., \'d5, \'d3
         * 2) in word EQ field, e.g, \\a, \\b, \\f ...
         * 3) special escaped text char, e.g., \{, \}
         * 3) control symbol, e.g., \_, \-, \|, \<10>
         */
        if (c == '\'') {        /* hex char */
            short c2;

            if ((c = GetChar()) != EOF && (c2 = GetChar()) != EOF) {
                /* should do isxdigit check! */
                rtfClass = rtfText;
                rtfMajor = RTFCharToHex(c) * 16 + RTFCharToHex(c2);
                return;
            }
            /* early eof, whoops (class is rtfUnknown) */
            return;
        }

        if (insideEquation && c == '\\') {
            rtfClass = rtfControl;

    		rtfMinor = GetChar();
			if (rtfMinor == '\\') {  /* this handles \\\{ and the like */
    			rtfMinor = GetChar();
				rtfMajor = rtfEquationFieldLiteral;
				return;
			}

            rtfMajor = rtfEquationFieldCmd;

			/* special escaping for characters not used as syntax */
			if (rtfMinor == ',' || rtfMinor == '(' || rtfMinor == ')' || 
			                       rtfMajor == '[' || rtfMajor == ']')
            	rtfMajor = rtfEquationFieldLiteral;
            return;
        }

        /* escaped char */
        /*if (index (":{}\\", c) != NULL)  */
        if (c == ':' || c == '{' || c == '}' || c == '\\') {
            rtfClass = rtfText;
            rtfMajor = c;
            return;
        }

        /* control symbol */
        Lookup(rtfTextBuf);     /* sets class, major, minor */
        return;
    }

    /* aquire control word */
    while (isalpha(c) && c != EOF)
        c = GetChar();

    /*
     * At this point, the control word is all collected, so the
     * major/minor numbers are determined before the parameter
     * (if any) is scanned.  There will be one too many characters
     * in the buffer, though, so fix up before and restore after
     * looking up.
     */

    if (c != EOF)
        rtfTextBuf[rtfTextLen - 1] = '\0';

    Lookup(rtfTextBuf);         /* sets class, major, minor */

    if (c != EOF)
        rtfTextBuf[rtfTextLen - 1] = c;

    /*
     * Should be looking at first digit of parameter if there
     * is one, unless it's negative.  In that case, next char
     * is '-', so need to gobble next char, and remember sign.
     */

    sign = 1;
    if (c == '-') {
        sign = -1;
        c = GetChar();
    }

    if (c != EOF && isdigit(c)) {
        rtfParam = 0;
        while (isdigit(c)) {    /* gobble parameter */
            rtfParam = rtfParam * 10 + c - '0';
            if ((c = GetChar()) == EOF)
                break;
        }
        rtfParam *= sign;
    }
    /*
     * If control symbol delimiter was a blank, gobble it.
     * Otherwise the character is first char of next token, so
     * push it back for next call.  In either case, delete the
     * delimiter from the token buffer.
     */
    if (c != EOF) {
        if (c != ' ')
            pushedChar = c;
        rtfTextBuf[--rtfTextLen] = '\0';
    }
}


/*
 * Read the next character from the input.  This handles setting the
 * current line and position-within-line variables.  Those variable are
 * set correctly whether lines end with CR, LF, or CRLF (the last being
 * the tricky case).
 *
 * bumpLine indicates whether the line number should be incremented on
 * the *next* input character.
 */

static short GetChar(void)
{
    short c;
    short oldBumpLine;

    if ((c = getc(rtffp)) != EOF) {
        rtfTextBuf[rtfTextLen++] = c;
        rtfTextBuf[rtfTextLen] = '\0';
    }
   /* fputc(c,stderr); */
    if (prevChar == EOF)
        bumpLine = 1;
    oldBumpLine = bumpLine;     /* non-zero if prev char was line ending */
    bumpLine = 0;
    if (c == '\r')
        bumpLine = 1;
    else if (c == '\n') {
        bumpLine = 1;
        if (prevChar == '\r')   /* oops, previous \r wasn't */
            oldBumpLine = 0;    /* really a line ending */
    }
    ++rtfLinePos;
    if (oldBumpLine) {          /* were we supposed to increment the *//* line count on this char? */
        ++rtfLineNum;
        rtfLinePos = 1;
    }
    prevChar = c;
    return (c);
}


/*
 * Synthesize a token by setting the global variables to the
 * values supplied.  Typically this is followed with a call
 * to RTFRouteToken().
 *
 * If a param value other than rtfNoParam is passed, it becomes
 * part of the token text.
 */

void RTFSetToken(short class, short major, short minor, int32_t param, char *text)
{
    rtfClass = class;
    rtfMajor = major;
    rtfMinor = minor;
    rtfParam = param;
    if (param == rtfNoParam)
        (void) strcpy(rtfTextBuf, text);
    else
        snprintf(rtfTextBuf, rtfBufSiz, "%s%d", text, (int) param);
    rtfTextLen = strlen(rtfTextBuf);
}


/*
 * Given a standard character name (a string), find its code (a number).
 * Return -1 if name is unknown.
 */

short RTFStdCharCode(char *name)
{
    short i;

    for (i = 0; i < rtfSC_MaxChar; i++) {
        if (strcmp(name, stdCharName[i]) == 0)
            return (i);
    }
    return (-1);
}


/*
 * Given a standard character code (a number), find its name (a string).
 * Return NULL if code is unknown.
 */

char *RTFStdCharName(short code)
{
    if (code < 0 || code >= rtfSC_MaxChar)
        return (NULL);
    return (stdCharName[code]);
}


/*
 * Given an RTF input character code, find standard character code.
 * The translator should read the appropriate charset maps when it finds a
 * charset control.  However, the file might not contain one.  In this
 * case, no map will be available.  When the first attempt is made to
 * map a character under these circumstances, RTFMapChar() assumes ANSI
 * and reads the map as necessary.
 */

short RTFMapChar(short c)
{
    if (c < 0 || c >= CHAR_SET_SIZE)
        return (rtfSC_nothing);

    return (curCharCode[c]);
}


/* ---------------------------------------------------------------------- */

/*
 * Special destination readers.  They gobble the destination so the
 * writer doesn't have to deal with them.  That's wrong for any
 * translator that wants to process any of these itself.  In that
 * case, these readers should be overridden by installing a different
 * destination callback.
 *
 * NOTE: The last token read by each of these reader will be the
 * destination's terminating '}', which will then be the current token.
 * That '}' token is passed to RTFRouteToken() - the writer has already
 * seen the '{' that began the destination group, and may have pushed a
 * state; it also needs to know at the end of the group that a state
 * should be popped.
 *
 * It's important that rtf.h and the control token lookup table list
 * as many symbols as possible, because these destination readers
 * unfortunately make strict assumptions about the input they expect,
 * and a token of class rtfUnknown will throw them off easily.
 */


/*
 * Read { \fonttbl ... } destination.  Old font tables don't have
 * braces around each table entry; try to adjust for that.
 */

static void ReadFontTbl(void)
{
    RTFFont *fp = NULL;
    char buf[rtfBufSiz], *bp;
    short old = -1;
    char *fn = "ReadFontTbl";

    for (;;) {
        (void) RTFGetToken();

        if (RTFCheckCM(rtfGroup, rtfEndGroup))
            break;

        if (rtfClass==rtfText && rtfMinor == rtfSC_space)
            continue;

        if (old < 0) {          /* first entry - determine tbl type */
            if (RTFCheckCMM(rtfControl, rtfCharAttr, rtfFontNum))
                old = 1;        /* no brace */
            else if (RTFCheckCM(rtfGroup, rtfBeginGroup))
                old = 0;        /* brace */
            else                /* can't tell! */
                RTFPanic("%s: Cannot determine format", fn);
        }

        if (old == 0) {         /* need to find "{" here */
            if (!RTFCheckCM(rtfGroup, rtfBeginGroup))
                RTFPanic("%s: xmissing \"{\"", fn);
            (void) RTFGetToken();       /* yes, skip to next token */
        }

        fp = New(RTFFont);
        if (!fp) {
            RTFPanic("%s: cannot allocate font entry", fn);
            exit(1);
        }

        fp->rtfNextFont = fontList;
        fontList = fp;

        fp->rtfFName = NULL;
        fp->rtfFAltName = NULL;
        fp->rtfFNum = -1;
        fp->rtfFFamily = 0;
        fp->rtfFCharSet = -1;
        fp->rtfFPitch = 0;
        fp->rtfFType = 0;
        fp->rtfFCharCode = genCharCode;
        fp->rtfFCodePage = 0;

        while (rtfClass != rtfEOF && !RTFCheckCM(rtfText, ';')) {

           if (rtfClass == rtfControl) {

                switch (rtfMajor) {
                default:
                    /* ignore token but do not announce it */
                    /*RTFMsg("%s: unknown token \"%s\"\n", fn, rtfTextBuf); */
                    break;

                case rtfFontFamily:
                    fp->rtfFFamily = rtfMinor;
                    break;

                case rtfCharAttr:
                   switch (rtfMinor) {
                    default:
                        break;  /* ignore unknown? */
                    case rtfFontNum:
                        fp->rtfFNum = rtfParam;
                        break;
                    }
                    break;

                case rtfFontAttr:
                    switch (rtfMinor) {

                    case rtfThemeflomajor:
                    case rtfThemefhimajor:
                    case rtfThemefdbmajor:
                    case rtfThemefbimajor:
                    case rtfThemeflominor:
                    case rtfThemefhiminor:
                    case rtfThemefdbminor:
                    case rtfThemefbiminor:
                    default:
                        break;  /* ignore unknown? */
                    case rtfFontCharSet:
                        fp->rtfFCharSet = rtfParam;
                        if (rtfParam == 128 || rtfParam == 78)  /* Japanese */
                            fp->rtfFCharCode = cp932CharCode;
                        if (rtfParam == 134 || rtfParam == 80)  /* Chinese GB2312 */
                            fp->rtfFCharCode = cp936CharCode;
                        break;
                    case rtfFontPitch:
                        fp->rtfFPitch = rtfParam;
                        break;
                    case rtfFontCodePage:
                        fp->rtfFCodePage = rtfParam;
                        break;
                    case rtfFTypeNil:
                    case rtfFTypeTrueType:
                        fp->rtfFType = rtfParam;
                        break;
                    }
                    break;
                }

            } else if (RTFCheckCM(rtfGroup, rtfBeginGroup)) {   /* dest */
                RTFSkipGroup(); /* ignore for now */

            } else if (rtfClass == rtfText) {   /* font name */
                /* read until a semi-colon is encountered */
                bp = buf;
                while (rtfClass != rtfEOF && !RTFCheckCM(rtfText, ';')) {
                    *bp++ = rtfMajor;
                    (void) RTFGetToken();
                }
                *bp = '\0';

                /*fprintf(stderr,"%05d fontname=%s\n",fp->rtfFNum, buf); */

                fp->rtfFName = RTFStrSave(buf);     
                if (fp->rtfFName == NULL)
                    RTFPanic("%s: cannot allocate font name", fn);
				
				/* Symbol font is unlike all others*/
                if (strnicmp(fp->rtfFName,"Symbol", 255)==0)
                    fp->rtfFCharCode = symCharCode;

                /* already have next token ';' don't read another at bottom of loop */
               continue;
            } else {
                /* ignore token but and don't announce it */
                /*RTFMsg("%s: unknown token \"%s\"\n", fn, rtfTextBuf);*/
            }

            (void) RTFGetToken();
       }

        if (old == 0) {         /* need to see "}" here */
            (void) RTFGetToken();
            if (!RTFCheckCM(rtfGroup, rtfEndGroup))
                RTFPanic("%s: missing '}' at end of \\fonttbl", fn);
        }
    }

    if (fp == NULL || fp->rtfFNum == -1)
        RTFPanic("File does not contain a valid font table");

/*
 * Could check other pieces of structure here, too, I suppose.
 */
    for (fp = fontList; fp != NULL; fp = fp->rtfNextFont) {
        if (fp->rtfFName == NULL)
            fp->rtfFName = strdup("noName");
//      fprintf(stderr, "Font %3d, cs=%3d, lookup=%p, name='%s'\n", fp->rtfFNum, fp->rtfFCharSet, fp->rtfFCharCode, fp->rtfFName);
    }

    RTFRouteToken();            /* feed "}" back to router */

    if (defaultFontNumber > -1) {
        RTFFont *fp1 = RTFGetFont(defaultFontNumber);
        if (fp1) curCharCode = fp1->rtfFCharCode;
    }

}

/*
 * The color table entries have color values of -1 if
 * the default color should be used for the entry (only
 * a semi-colon is given in the definition, no color values).
 * There will be a problem if a partial entry (1 or 2 but
 * not 3 color values) is given.  The possibility is ignored
 * here.
 */

void ReadColorTbl(void)
{
    RTFColor *cp;
    short cnum = 0;
    char *fn = "ReadColorTbl";

    for (;;) {
        (void) RTFGetToken();
        if (RTFCheckCM(rtfGroup, rtfEndGroup))
            break;

        cp = New(RTFColor);
        if (!cp) {
            RTFPanic("%s: cannot allocate color entry", fn);
            exit(1);
        }

        cp->rtfCNum = cnum++;
        cp->rtfCRed = cp->rtfCGreen = cp->rtfCBlue = -1;
        cp->rtfNextColor = colorList;
        colorList = cp;
        while (RTFCheckCM(rtfControl, rtfColorName)) {
            switch (rtfMinor) {
            case rtfRed:
                cp->rtfCRed = rtfParam;
                break;
            case rtfGreen:
                cp->rtfCGreen = rtfParam;
                break;
            case rtfBlue:
                cp->rtfCBlue = rtfParam;
                break;

            case rtfTextOne:
            case rtfShade:
            case rtfTint:
            case rtfColorHyperlink:
            case rtfColorAccent:
            case rtfColorTextTwo:
                break;
            }
            RTFGetToken();
        }
        /*
         * Normally a semicolon should terminate a color table entry,
         * but some writers write the last entry without one, so that
         * the entry is followed by the "}" that terminates the table.
         * Allow for that possibility here.
         */
        if (RTFCheckCM(rtfGroup, rtfEndGroup))
            break;
        if (!RTFCheckCM(rtfText, (short) ';'))
            RTFPanic("%s: malformed entry", fn);
    }
    RTFRouteToken();            /* feed "}" back to router */
}


/* correlates the user defined conversion with a styles "name" */
static short Style2LatexItem(char *name)
{
    int i;

    for (i = 0; i < MAX_STYLE_MAPPINGS; i++) {
        if (!Style2LatexStyle[i]) return -1; 

        if (strnicmp(name, Style2LatexStyle[i], 255) == 0)
            return i;
    }
    return (-1);
}


/*
 * The "Normal" style definition doesn't contain any style number,
 * all others do.  Normal style is given style rtfNormalStyleNum.
 */

static void ReadStyleSheet(void)
{
    RTFStyle *sp;
    RTFStyleElt *sep, *sepLast;
    char buf[rtfBufSiz], *bp;
    char *fn = "ReadStyleSheet";

    for (;;) {
        (void) RTFGetToken();

        if (RTFCheckCM(rtfGroup, rtfEndGroup))
            break;

        sp = New(RTFStyle);
        if (!sp) {
            RTFPanic("%s: cannot allocate stylesheet entry", fn);
            exit(1);
        }

        sp->rtfSName = NULL;
        sp->rtfSNum = -1;
        sp->rtfSType = rtfParStyle;
        sp->rtfSAdditive = 0;
        sp->rtfSBasedOn = rtfNoStyleNum;
        sp->rtfSNextPar = -1;
        sp->rtfSSEList = sepLast = NULL;
        sp->rtfNextStyle = styleList;
        sp->rtfExpanding = 0;
        styleList = sp;
        if (!RTFCheckCM(rtfGroup, rtfBeginGroup))
            RTFPanic("%s: missing \"{\"", fn);
        for (;;) {
            (void) RTFGetToken();
            if (rtfClass == rtfEOF || RTFCheckCM(rtfText, ';'))
                break;

            if (rtfClass == rtfControl) {
                if (RTFCheckMM(rtfSpecialChar, rtfOptDest))
                    continue;   /* ignore "\*" */

                if (RTFCheckMM(rtfParAttr, rtfStyleNum)) {
                    sp->rtfSNum = rtfParam;
                    sp->rtfSType = rtfParStyle;
                    continue;
                }
                if (RTFCheckMM(rtfCharAttr, rtfCharStyleNum)) {
                    sp->rtfSNum = rtfParam;
                    sp->rtfSType = rtfCharStyle;
                    continue;
                }
                if (RTFCheckMM(rtfSectAttr, rtfSectStyleNum)) {
                    sp->rtfSNum = rtfParam;
                    sp->rtfSType = rtfSectStyle;
                    continue;
                }
                if (RTFCheckMM(rtfStyleAttr, rtfBasedOn)) {
                    sp->rtfSBasedOn = rtfParam;
                    continue;
                }
                if (RTFCheckMM(rtfStyleAttr, rtfAdditive)) {
                    sp->rtfSAdditive = 1;
                    continue;
                }
                if (RTFCheckMM(rtfStyleAttr, rtfNext)) {
                    sp->rtfSNextPar = rtfParam;
                    continue;
                }
                if (RTFCheckMM(rtfStyleAttr, rtfTableStyleNum)) {
                    sp->rtfSNum = rtfParam;
                    sp->rtfSType = rtfTableStyle;
                    continue;
                }

                sep = New(RTFStyleElt);
                if (!sep) {
                    RTFPanic("%s: cannot allocate style element", fn);
                    exit(1);
                }

                sep->rtfSEClass = rtfClass;
                sep->rtfSEMajor = rtfMajor;
                sep->rtfSEMinor = rtfMinor;
                sep->rtfSEParam = rtfParam;
                sep->rtfSEText = RTFStrSave(rtfTextBuf);
                if (! sep->rtfSEText)
                    RTFPanic("%s: cannot allocate style element text", fn);

                if (sepLast == NULL)
                    sp->rtfSSEList = sep;       /* first element */
                else            /* add to end */
                    sepLast->rtfNextSE = sep;
                sep->rtfNextSE = NULL;
                sepLast = sep;
            } else if (RTFCheckCM(rtfGroup, rtfBeginGroup)) {
                /*
                 * This passes over "{\*\keycode ... }, among
                 * other things. A temporary (perhaps) hack.
                 */
                RTFSkipGroup();
                continue;
            } else if (rtfClass == rtfText) {   /* style name */
                bp = buf;
                while (rtfClass == rtfText) {
                    if (rtfMajor == ';') {
                        /* put back for "for" loop */
                        (void) RTFUngetToken();
                        break;
                    }
                    *bp++ = rtfMajor;
                    (void) RTFGetToken();
                }
                *bp = '\0';
                if ((sp->rtfSName = RTFStrSave(buf)) == NULL)
                    RTFPanic("%s: cannot allocate style name", fn);
            } else {            /* unrecognized */

                /* ignore token and do not announce it
                RTFMsg ("%s: unknown token \"%s\"\n", fn, rtfTextBuf);
                */

            }
        }
        (void) RTFGetToken();

        while (!RTFCheckCM(rtfGroup, rtfEndGroup)) {
            RTFGetToken();
            if (rtfClass == rtfEOF) {
                RTFPanic("%s: Stylesheet malformed!\n", fn);
                break;
            }
        }

        /*
         * Check over the style structure.  A name is a must.
         * If no style number was specified, check whether it's the
         * Normal style (in which case it's given style number
         * rtfNormalStyleNum).  Note that some "normal" style names
         * just begin with "Normal" and can have other stuff following,
         * e.g., "Normal,Times 10 point".  Ugh.
         *
         * Some German RTF writers use "Standard" instead of "Normal".
         */
        if (sp->rtfSName == NULL)
            RTFPanic("%s: missing style name", fn);

        if (sp->rtfSNum < 0) {
            if (strncmp(buf, "Normal", 6) != 0 && strncmp(buf, "Standard", 8) != 0) {
                RTFMsg("%s: style is '%s'\n",fn, buf);
                RTFMsg("%s: number is %d\n",fn, sp->rtfSNum);
                RTFPanic("%s: missing style number", fn);
            }
            sp->rtfSNum = rtfNormalStyleNum;
        }

        /* this provides direct access to the right style mapping without
           needing to compare all the style names every time we want to know
           If there is no mapping, then the index is just set to -1 */
        if (sp->rtfSNum < MAX_STYLE_MAPPINGS)
            Style2LatexMapIndex[sp->rtfSNum] = Style2LatexItem(sp->rtfSName);

        if (sp->rtfSNextPar == -1)      /* if \snext not given, */
            sp->rtfSNextPar = sp->rtfSNum;      /* next is itself */
    }
    RTFRouteToken();            /* feed "}" back to router */
}


static void ReadInfoGroup(void)
{
    RTFSkipGroup();
    RTFRouteToken();            /* feed "}" back to router */
}


static void ReadPictGroup(void)
{
    RTFSkipGroup();
    RTFRouteToken();            /* feed "}" back to router */
}


static void ReadObjGroup(void)
{
    RTFSkipGroup();
    RTFRouteToken();            /* feed "}" back to router */
}

/* ---------------------------------------------------------------------- */

/*
 * Routines to return pieces of stylesheet, or font or color tables.
 * References to style 0 are mapped onto the Normal style.
 */


RTFStyle *RTFGetStyle(short num)
{
    RTFStyle *s;

    if (num == -1)
        return (styleList);
    for (s = styleList; s != NULL; s = s->rtfNextStyle) {
        if (s->rtfSNum == num)
            break;
    }
    return (s);                 /* NULL if not found */
}


RTFFont *RTFGetFont(short num)
{
    RTFFont *f;

    if (num == -1)
        return (fontList);
    for (f = fontList; f != NULL; f = f->rtfNextFont) {
        if (f->rtfFNum == num) {
            /* check for NULL font name contributed by Keith Refson */
            if (f->rtfFName == NULL)
                f->rtfFName = "noName";
            break;
        }
    }
    return (f);                 /* NULL if not found */
}


RTFColor *RTFGetColor(short num)
{
    RTFColor *c;

    if (num == -1)
        return (colorList);
    for (c = colorList; c != NULL; c = c->rtfNextColor) {
        if (c->rtfCNum == num)
            break;
    }
    return (c);                 /* NULL if not found */
}


/* ---------------------------------------------------------------------- */


/*
 * Expand style n, if there is such a style.
 */

void RTFExpandStyle(short n)
{
    RTFStyle *s;
    RTFStyleElt *se;

    if (n == -1 || (s = RTFGetStyle(n)) == NULL) {
        RTFMsg("¥ n is %d, returning without expanding style\n", n);
        return;
    }
    if (s->rtfExpanding != 0)
        RTFPanic("Style expansion loop, style %d", n);
    s->rtfExpanding = 1;        /* set expansion flag for loop detection */
    /*
     * Expand "based-on" style (unless it's the same as the current
     * style -- Normal style usually gives itself as its own based-on
     * style).  Based-on style expansion is done by synthesizing
     * the token that the writer needs to see in order to trigger
     * another style expansion, and feeding to token back through
     * the router so the writer sees it.
     */
    if (n != s->rtfSBasedOn) {
        RTFSetToken(rtfControl, rtfParAttr, rtfStyleNum,
                    s->rtfSBasedOn, "\\s");
        RTFRouteToken();
    }
    /*
     * Now route the tokens unique to this style.  RTFSetToken()
     * isn't used because it would add the param value to the end
     * of the token text, which already has it in.
     */
    for (se = s->rtfSSEList; se != NULL;
         se = se->rtfNextSE) {
        rtfClass = se->rtfSEClass;
        rtfMajor = se->rtfSEMajor;
        rtfMinor = se->rtfSEMinor;
        rtfParam = se->rtfSEParam;
        (void) strcpy(rtfTextBuf, se->rtfSEText);
        rtfTextLen = strlen(rtfTextBuf);
        RTFRouteToken();
    }
    s->rtfExpanding = 0;        /* done - clear expansion flag */
}


/* ---------------------------------------------------------------------- */

/*
 * Control symbol lookup routines
 */


RTFCtrl **rtfCtrl = NULL;
short nCtrls;



/*
 * Determine major and minor number of control token.  If it's
 * not found, the class turns into rtfUnknown.
 *
 * Algorithm uses a binary search to locate the token.  It assumes
 * the tokens are stored in the table in sorted order.
 */

static void Lookup(char *s)
{
    RTFCtrl *rp;
    char c1, c2;
/* short        i; */
    short LookupIndex, result;
    short lower, upper;

    ++s;                        /* skip over the leading \ character */
    c1 = *s;                    /* save first char for comparisons */

    lower = 0;
    upper = nCtrls - 1;

    for (;;) {
        LookupIndex = (lower + upper) / 2;
        rp = rtfCtrl[LookupIndex];
        /*
         * Do quick check against first character to avoid function
         * call if possible.  If character matches, then call strcmp().
         */
        c2 = rp->str[0];
        result = (c1 - c2);
        if (result == 0)
            result = strcmp(s, rp->str);
        if (result == 0) {
            rtfClass = rtfControl;
            rtfMajor = rp->major;
            rtfMinor = rp->minor;
            rtfTokenIndex = rp->index;
            return;
        }
        if (lower >= upper)     /* can't subdivide range further, */
            break;              /* so token wasn't found */
        if (result < 0)
            upper = LookupIndex - 1;
        else
            lower = LookupIndex + 1;
    }
    rtfClass = rtfUnknown;
}

void DebugMessage(void)
{
    if (0 || g_debug_level > 0)
        if (strcmp(rtfCtrl[rtfTokenIndex]->str,"objdata"))
            fprintf(stderr, "%s (%d,%d,%d)\n",rtfCtrl[rtfTokenIndex]->str,rtfClass,rtfMajor,rtfMinor);
}
/* ---------------------------------------------------------------------- */

/*
 * Memory allocation routines
 */


/*
 * Return pointer to block of size bytes, or NULL if there's
 * not enough memory available.
 *
 * This is called through RTFAlloc(), a define which coerces the
 * argument to int32_t.  This avoids the persistent problem of allocation
 * failing and causing mysterious crashes under THINK C when an argument
 * of the wrong size is passed.
 */

char * RTFAlloc(size_t size)
{
    char * memory = malloc(size);

    if (!memory) {
        RTFPanic("Cannot allocate needed memory\n");
        exit(1);
    }
    return memory;
}


/*
 * Saves a string on the heap and returns a pointer to it.
 */


char *RTFStrSave(char *s)
{
    char *p;

    if ((p = RTFAlloc(strlen(s) + 1)) == NULL)
        return (NULL);
    return (strcpy(p, s));
}


void RTFFree(char *p)
{
    if (p != NULL)
        free(p);
}


/* ---------------------------------------------------------------------- */


/*
 * Token comparison routines
 */

short RTFCheckCM(short class, short major)
{
    return (rtfClass == class && rtfMajor == major);
}


short RTFCheckCMM(short class, short major, short minor)
{
    return (rtfClass == class && rtfMajor == major && rtfMinor == minor);
}


short RTFCheckMM(short major, short minor)
{
    return (rtfMajor == major && rtfMinor == minor);
}


/* ---------------------------------------------------------------------- */


short RTFCharToHex(char c)
{
    if (isupper(c))
        c = tolower(c);
    if (isdigit(c))
        return (c - '0');       /* '0'..'9' */
    return (c - 'a' + 10);      /* 'a'..'f' */
}


short RTFHexToChar(short i)
{
    if (i < 10)
        return (i + '0');
    return (i - 10 + 'a');
}

/* string must start with 0x or 0X */
int RTFHexStrToInt(char * s)
{
    int i,x;
    if (!s)
        return 0;
    if (strlen(s)<3)
        return 0;
    if (s[0] != '0' || (s[1] != 'x' && s[1] != 'X'))
        return 0;
    i=2;
    x=0;
    while (s[i] != '\0') {
        x = x*16 + RTFCharToHex(s[i]);
        i++;
    }
    return x;
}



/* ---------------------------------------------------------------------- */

/*
 * Open a library file.
 */


static FILE *(*libFileOpen) (char *file, char *mode) = NULL;

void RTFSetOpenLibFileProc(FILE * (*proc) (char *file, char *mode))
{
    libFileOpen = proc;
}


FILE *RTFOpenLibFile(char *file, char *mode)
{
    if (libFileOpen == NULL)
        return (NULL);
    return ((*libFileOpen) (file, mode));
}


/* ---------------------------------------------------------------------- */

/*
 * Print message.  Default is to send message to stderr
 * but this may be overridden with RTFSetMsgProc().
 *
 * Message should include linefeeds as necessary.  If the default
 * function is overridden, the overriding function may want to
 * map linefeeds to another line ending character or sequence if
 * the host system doesn't use linefeeds.
 */

static void DefaultMsgProc(char *s)
{
    fprintf(stderr, "%s", s);
}

static RTFFuncMsgPtr msgProc = DefaultMsgProc;

void RTFSetMsgProc(RTFFuncMsgPtr proc)
{
    msgProc = proc;
}


void RTFMsg(char *fmt, ...)
{
    char buf[rtfBufSiz];

    va_list args;
    va_start(args, fmt);
    vsnprintf(buf, rtfBufSiz, fmt, args);
    va_end(args);
    (*msgProc) (buf);
}

/* ---------------------------------------------------------------------- */

/*
 * Process termination.  Print error message and exit.  Also prints
 * current token, and current input line number and position within
 * line if any input has been read from the current file.  (No input
 * has been read if prevChar is EOF).
 */

static void DefaultPanicProc(char *s)
{
    fprintf(stderr, "%s", s);
    exit(1);
}

static RTFFuncMsgPtr panicProc = DefaultPanicProc;

void RTFSetPanicProc(RTFFuncMsgPtr proc)
{
    panicProc = proc;
}


void RTFPanic(char *fmt, ...)
{
    char buf[rtfBufSiz];

    va_list args;
    va_start(args, fmt);
    vsnprintf(buf, rtfBufSiz, fmt, args);
    va_end(args);
    (void) strcat(buf, "\n");
    if (prevChar != EOF && rtfTextBuf != NULL) {
        snprintf(buf + strlen(buf), rtfBufSiz-strlen(buf),
                "Last token read was \"%s\" near line %d, position %d.\n",
                rtfTextBuf, (int)rtfLineNum, (int)rtfLinePos);
    }
    (*panicProc) (buf);
}

/*
 Useful functions added by Ujwal Sathyam.
 */

/*
 This function does a simple initialization of the RTF reader. This is useful when
 the file cursor is moved around in the input stream. It basically makes the RTF
 reader forget what it has seen before.
 */

void RTFSimpleInit(void)
{
    rtfClass = -1;
    pushedClass = -1;
    pushedChar = EOF;
}


/*
 This function returns the last character the RTF reader removed from the input stream.
 */
short RTFPushedChar(void)
{
    return (pushedChar);
}

void RTFSetPushedChar(short lastChar)
{
    pushedChar = lastChar;
}

void RTFSetDefaultFont(int fontNumber)
{
    defaultFontNumber = fontNumber;
}

/*
 * Stack for keeping track of text style on group begin/end.  This is
 * necessary because group termination reverts the text style to the previous
 * value, which may implicitly change it.
 */

# define        MAX_STACK             100

short *charStyleStack[MAX_STACK];
parStyleStruct  parStyleStack[MAX_STACK];
textStyleStruct textStyleStack[MAX_STACK];

void RTFInitStack(void)
{
    RTFSetBraceLevel(0);
    paragraphWritten.firstIndent      = UNINITIALIZED;
    paragraphWritten.leftIndent       = UNINITIALIZED;
    paragraphWritten.lineSpacing      = UNINITIALIZED;
    paragraphWritten.alignment        = UNINITIALIZED;

    paragraph.firstIndent = 720;
    paragraph.leftIndent  = 0;
    paragraph.lineSpacing = 240;
    paragraph.alignment   = 0;
    textStyle.foreColor   = 0;
    textStyle.backColor   = 0;
    textStyle.fontSize    = normalSize;
    textStyle.charCode    = genCharCode;
    textStyle.fontNumber  = defaultFontNumber;
    textStyle.unicodeSkipN= 1;

    RTFPushStack();
}

void RTFPushStack(void)
{
    int level = RTFGetBraceLevel();
    charStyleStack[level] = curCharCode;
    parStyleStack[level] = paragraph;
    textStyleStack[level] = textStyle;

//    fprintf(stderr, "push [%d] alignment now written=%d, set=%d\n",braceLevel, paragraphWritten.alignment, paragraph.alignment);
    level++;
    if (level >= MAX_STACK)
        RTFMsg("Exceeding stack capacity of %d items\n",MAX_STACK);
    else
        RTFSetBraceLevel(level);
}

void RTFPopStack(void)
{
    int level = RTFGetBraceLevel();
    level--;

    if (level < 0) {
        RTFMsg("Too many '}'.  Stack Underflow\n");
        return;
    }

    RTFSetBraceLevel(level);
    curCharCode = charStyleStack[level];
    paragraph = parStyleStack[level];
    textStyle = textStyleStack[level];
}

extern FILE *ifp, *ofp;

void RTFParserState(int op)
{
    static size_t saved_file_position = 0;
    static int savedbraceLevel = 0;
    static char statePrevChar = 0;
    static short *saved_charStyleStack[MAX_STACK];
    static parStyleStruct  saved_parStyleStack[MAX_STACK];
    static textStyleStruct saved_textStyleStack[MAX_STACK];
    static parStyleStruct  saved_paragraphWritten;
    static textStyleStruct saved_textStyleWritten;
    static int savedpushedClass;
    static int savedpushedMajor;
    static int savedpushedMinor;
    static int savedpushedParam;
    static char savedpushedTextBuf[rtfBufSiz];
    static char savedTextBuf[rtfBufSiz];
    static int savedClass;
    static int savedMajor;
    static int savedMinor;
    static int savedParam;

    if (op == SAVE_PARSER) {
        memcpy(saved_charStyleStack, charStyleStack, MAX_STACK * sizeof(short));
        memcpy(saved_parStyleStack, parStyleStack, MAX_STACK * sizeof(parStyleStruct));
        memcpy(saved_textStyleStack, textStyleStack, MAX_STACK * sizeof(textStyleStruct));
        saved_paragraphWritten = paragraphWritten;
        saved_textStyleWritten = textStyleWritten;
        statePrevChar = RTFPushedChar();
        saved_file_position = ftell(ifp);
        savedbraceLevel = RTFGetBraceLevel();
        savedClass = rtfClass;
        savedMajor = rtfMajor;
        savedMinor = rtfMinor;
        savedParam = rtfParam;
        strcpy(savedTextBuf,rtfTextBuf);
        savedpushedClass = pushedClass;
        savedpushedMajor = pushedMajor;
        savedpushedMinor = pushedMinor;
        savedpushedParam = pushedParam;
        strcpy(savedpushedTextBuf,pushedTextBuf);
        return;
    }

    if (op == RESTORE_PARSER) {
        fseek(ifp, saved_file_position, 0);
        RTFSimpleInit();
        RTFSetPushedChar(statePrevChar);
        memcpy(charStyleStack, saved_charStyleStack, MAX_STACK * sizeof(short));
        memcpy(parStyleStack, saved_parStyleStack, MAX_STACK * sizeof(parStyleStruct));
        memcpy(textStyleStack, saved_textStyleStack, MAX_STACK * sizeof(textStyleStruct));
        RTFSetBraceLevel(savedbraceLevel);
        curCharCode = charStyleStack[savedbraceLevel];
        paragraph = parStyleStack[savedbraceLevel];
        textStyle = textStyleStack[savedbraceLevel];
        paragraphWritten = saved_paragraphWritten;
        textStyleWritten = saved_textStyleWritten;
        rtfClass = savedClass;
        rtfMajor = savedMajor;
        rtfMinor = savedMinor;
        rtfParam = savedParam;
        strcpy(rtfTextBuf,savedTextBuf);
        pushedClass = savedpushedClass;
        pushedMajor = savedpushedMajor;
        pushedMinor = savedpushedMinor;
        pushedParam = savedpushedParam;
        strcpy(pushedTextBuf,savedpushedTextBuf);
    }
}
