/*
 * header file for RTF-to-LaTeX2e translation writer code.
 * (c) 1999 Ujwal S. Sathyam
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

# define    WRAP_LIMIT     80
# define    PACKAGES        9
# define    UNINITIALIZED -99

#ifndef boolean
typedef unsigned char boolean;
#endif

# define    New(t)  ((t *) RTFAlloc (sizeof (t)))
# define    Swap(a, b) {int tmp;\
                        tmp = a;\
                        a = b;\
                        b = tmp;}

#if defined(UNIX)
    # define    PATH_SEP    '/'
    # define    ENV_SEP     ':'
#else 
    # define    PATH_SEP    '\\'
    # define    ENV_SEP     ';'
#endif

#ifndef false
    #define false 0
#endif

#ifndef true
    #define true 1
#endif

void    WriterInit (void);
int BeginLaTeXFile (void);
void EndLaTeXFile (void);

void RTFSetOutputStream (FILE *ofp);

enum pictType {unknownPict, pict, wmf, gif, jpeg, bin, png, emf};
enum objClass {unknownObjClass, EquationClass, WordPictureClass, MSGraphChartClass};
enum shapeObjectClass {unknownShapeObject, shapeObject, standardObject, shapePicture, shapeObjText};
enum {left, center, right};
enum {singleSpace, oneAndAHalfSpace, doubleSpace};
typedef enum {tinySize, scriptSize, footNoteSize, smallSize, normalSize, 
            largeSize, LargeSize, LARGESize, giganticSize, GiganticSize} fontSize;
enum cellVerticalMergeFlag {mergeNone, mergeTop, mergeAbove};

typedef struct 
{
    int count;
    int type;
    int32_t width;
    int32_t height;
    int32_t goalWidth;
    int32_t goalHeight;
    float scaleX;
    float scaleY;
    int     llx;
    int     lly;
    int     urx;
    int     ury;
    char    *name;
    int     cropTop;
    int     cropBottom;
    int     cropLeft;
    int     cropRight;
} pictureStruct;

typedef struct
{
    int count;
} equationStruct;


typedef struct
{
    int bold;
    int italic;
    int underlined;
    int dbUnderlined;
    int shadowed;
    int allCaps;
    int smallCaps;
    int subScript;
    int superScript;
    int fontSize;
    int foreColor;
    int backColor;
    int fontNumber;
    int mathRoman;
    short *charCode;
    int unicodeSkipN;
} textStyleStruct;

typedef struct
{
    int alignment;
    int lineSpacing;
    int firstIndent;
    int leftIndent;
    int rightIndent;
    int spaceBefore;
    int spaceAfter;
    int extraIndent;
    int styleIndex;
} parStyleStruct;

typedef struct cellStruct
{
    int row;
    int col;
    int originalLeft;
    int originalRight;
    int columnSpan;
    int index;
    int verticalMerge;   
    boolean leftBorder;
    boolean rightBorder;
    boolean topBorder;
    boolean bottomBorder;
    struct cellStruct *nextCell;
} cellStruct;

typedef struct
{
    boolean inside;
    int         rows;
    int         cols;
    int         cellCount;
    int         leftEdge;
    cellStruct  *theCell;
    int         cellsInRow[rtfBufSiz];
    int         *rightColumnBorders;
    int         cellMergePar;
    boolean     multiRow;
    boolean     limboCellLeftBorder;
    boolean     limboCellRightBorder;
    boolean     limboCellTopBorder;
    boolean     limboCellBottomBorder;
} tableStruct;

typedef struct
{
    int left;
    int top;
    int bottom;
    int right;
} shapeStruct;


short ReadPrefFile (char *file);

extern parStyleStruct paragraph, paragraphWritten;
extern textStyleStruct textStyle, textStyleWritten;

extern char *preambleFirstText;
extern char *preambleSecondText;
extern char *preambleDocClass;
extern char *preambleUserText;
extern char *preambleEncoding;
extern char *preamblePackages;
extern char *preambleColorTable;
extern char *preambleOurDefs;
