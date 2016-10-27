#ifndef _EQN_SUPPORT_H

#define _EQN_SUPPORT_H

#define PLATFORM_MAC  0
#define PLATFORM_WIN  1

#define embDOT        2
#define embDDOT       3
#define embTDOT       4
#define embPRIME      5
#define embDPRIME     6
#define embBPRIME     7
#define embTILDE      8
#define embHAT        9
#define embNOT        10
#define embRARROW     11
#define embLARROW     12
#define embBARROW     13
#define embR1ARROW    14
#define embL1ARROW    15
#define embMBAR       16
#define embOBAR       17
#define embTPRIME     18
#define embFROWN      19
#define embSMILE      20

/*  embellishment object */
typedef struct {
    struct MT_EMBELL *next;
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t embell;
} MT_EMBELL;

#define MT_LEFT       0
#define MT_CENTER     1
#define MT_RIGHT      2
#define MT_OPERATOR   3
#define MT_DECIMAL    4

#define MT_PILE_LEFT       1
#define MT_PILE_CENTER     2
#define MT_PILE_RIGHT      3
#define MT_PILE_OPERATOR   4
#define MT_PILE_DECIMAL    5

#define TAB_LEFT      0
#define TAB_CENTER    1
#define TAB_RIGHT     2
#define TAB_EQUAL     3
#define TAB_DECIMAL   4

/*  tabstop object */
typedef struct {
    struct MT_TABSTOP *next;
    int16_t type;
    int16_t offset;
} MT_TABSTOP;

#define END     0
#define LINE    1
#define CHAR    2
#define TMPL    3
#define PILE    4
#define MATRIX  5
#define EMBELL  6
#define RULER   7
#define FONT    8
#define SIZE    9
#define FULL    10
#define SUB     11
#define SUB2    12
#define SYM     13
#define SUBSYM  14
#define COLOR 	15

#define COLOR_DEF 	 16
#define FONT_DEF 	 17
#define EQN_PREFS 	 18
#define ENCODING_DEF 19

#define xfLMOVE     0x08

#define xfAUTO      0x01
#define xfEMBELL    0x02

#define xfNULL      0x01
#define xfRULER     0x02
#define xfLSPACE    0x04


/*  ruler object */
typedef struct {
    int16_t n_stops;
    MT_TABSTOP *tabstop_list;
} MT_RULER;


/*  line (of math) object */
typedef struct {
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t line_spacing;
    MT_RULER *ruler;
    MT_OBJLIST *object_list;
} MT_LINE;


/*  character object */
typedef struct {
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t atts;
    uint8_t typeface;
    uint16_t mtchar;
    uint16_t bits16;
    uint16_t character;
    MT_EMBELL *embellishment_list;
} MT_CHAR;

/*  template object */
typedef struct {
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t selector;
    uint16_t variation;
    uint8_t options;
    MT_OBJLIST *subobject_list;
} MT_TMPL;

#define PHA_LEFT      1
#define PHA_CENTER    2
#define PHA_RIGHT     3
#define PHA_RELOP     4
#define PHA_DECIMAL   5

#define PVA_TOP       0
#define PVA_CENTER    1
#define PVA_BOTTOM    2
#define PVA_CENTERING 3
#define PVA_MATH      4

/*  pile object */
typedef struct {
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t halign;
    uint8_t valign;
    MT_RULER *ruler;
    MT_OBJLIST *line_list;
} MT_PILE;

#define MATR_MAX  16

/*  matrix object */
typedef struct {
    int16_t nudge_x;
    int16_t nudge_y;
    uint8_t valign;
    uint8_t h_just;
    uint8_t v_just;
    uint8_t rows;
    uint8_t cols;
    uint8_t row_parts[MATR_MAX];
    uint8_t col_parts[MATR_MAX];
    MT_OBJLIST *element_list;
} MT_MATRIX;


/*  font object */
typedef struct {
    int tface;
    int style;
    char *zname;
} MT_FONT;

#define szFULL          0
#define szSUB           1
#define szSUB2          2
#define szSYM           3
#define szSUBSYM        4
#define szUSER1         5
#define szUSER2         6

/*  size object */
typedef struct {
    int type;
    int lsize;
    int dsize;
} MT_SIZE;


/*  our equation object */
typedef struct {
    int log_level;
    int do_delete;
    int ilk;
    int is_line;
    char *data;
} EQ_STRREC;

#define NUM_TYPEFACE_SLOTS    32

#define Z_TEX         1
#define Z_COMMENT     2
#define Z_TMPL        3

#define MA_NONE         0
#define MA_FORCE_MATH   1
#define MA_FORCE_TEXT   2

#define CHAR_EMBELL         0x01
#define CHAR_FUNC_START     0x02
#define CHAR_ENC_CHAR_8     0x04
#define CHAR_NUDGE          0x08
#define CHAR_ENC_CHAR_16    0x10
#define CHAR_ENC_NO_MTCODE  0x20

#endif
