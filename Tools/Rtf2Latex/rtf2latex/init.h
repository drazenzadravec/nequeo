#ifndef _RTF_INIT

#define _RTF_INIT

/* These should exactly match order in prefString[] in init.c */
enum prefName {
    pOutputMapFileName,           /* 0  */
    pPageWidth,
    pPageLeft,
    pPageRight,
    pConvertTextColor,
    pConvertPageSize,             /* 5  */
    pConvertTextSize,
    pConvertTextForm,
    pConvertParagraphStyle,
    pConvertParagraphIndent,
    pConvertInterParagraphSpace,  /* 10 */
    pConvertLineSpacing,
    pConvertHypertext,
    pConvertPict,
    pConvertEquation,
    pConvertAsDirectory,          /* 15 */
    pPreambleFirstText,
    pPreambleSecondText,
    pPreambleDocClass,
    pConvertTableName,
    pConvertParagraphMargin,     /* 20 */
    pConvertParagraphAlignment,
    pConvertTextNoTab,
    pConvertTableWidths,
    pConvertTableAlignment,
    pConvertTableBorders,        /* 25 */
    pLast
};

typedef struct RTFCtrl RTFCtrl;

struct RTFCtrl {
    short major;                /* major number */
    short minor;                /* minor number */
    short index;                /* index of token */
    char *str;                  /* symbol name */
};


extern RTFCtrl **rtfCtrl;
extern short nCtrls;

extern const char *prefString[];
extern int prefs[pLast];
extern char *convertTableName;

FILE *OpenLibFile(char *name, char *mode);
char *strdup_together(const char *s, const char *t);
char *append_file_to_path(char *path, char *file);
void InitConverter(void);

# define MAX_STYLE_MAPPINGS     255
extern char *Style2LatexOpen[MAX_STYLE_MAPPINGS];
extern char *Style2LatexClose[MAX_STYLE_MAPPINGS];
extern char *Style2LatexStyle[MAX_STYLE_MAPPINGS];
extern int Style2LatexMapIndex[MAX_STYLE_MAPPINGS];

# define        CHAR_SET_SIZE             256
extern short symCharCode[CHAR_SET_SIZE];          /* Adobe Symbol Font */
extern short cp1250CharCode[CHAR_SET_SIZE];       /* code page 1250 */
extern short cp1251CharCode[CHAR_SET_SIZE];       /* code page 1252 */
extern short cp1252CharCode[CHAR_SET_SIZE];       /* code page 1252 */
extern short cp1254CharCode[CHAR_SET_SIZE];       /* code page 1254 */
extern short cp437CharCode[CHAR_SET_SIZE];        /* code page 437 */
extern short cp850CharCode[CHAR_SET_SIZE];        /* code page 850 */
extern short cpMacCharCode[CHAR_SET_SIZE];        /* mac character set */
extern short cpNextCharCode[CHAR_SET_SIZE];       /* NeXt character set */

extern short cp932CharCode[];       /* cp932 character set */
extern short cp932Index[];       /* cp932 character set index */
extern int cp932IndexSize;

extern short cp936CharCode[];       /* cp932 character set */
extern short cp936Index[];       /* cp932 character set index */
extern int cp936IndexSize;

extern short *genCharCode;
extern short *curCharCode;

extern char *outMap[rtfSC_MaxChar];

extern char *g_library_path;

# define  PREFS_DIR	"pref"

#endif
