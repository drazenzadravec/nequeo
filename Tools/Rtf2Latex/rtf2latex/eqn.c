/*  MathType equation to LaTeX converter
 * 
 *  Ported 2000.04.03 by Steve Swanson, steve@mackichan.com
 *  Initial implementation by Jack Medd.  Originally part of
 *  RTF to LaTeX converter in Scientific WorkPlace (http://www.mackichan.com).
 * 
 *  The MathType equation format were described at
 *    http://www.mathtype.com/support/tech/MTEF4.htm
 *    http://www.mathtype.com/support/tech/MTEF5.htm
 *    http://www.mathtype.com/support/tech/MTEF_storage.htm
 *    http://www.mathtype.com/support/tech/encodings/mtcode.stm
 *  Various undocumented details determined by debugging and intuition.
 * 
 *  Access to these pages is available at
 *  http://web.archive.org/web/20010304110708/http://mathtype.com/support/tech/MTEF5.htm
 *  http://web.archive.org/web/20010304110708/http://mathtype.com/support/tech/MTEF4.htm
 *  http://web.archive.org/web/20010304110708/http://mathtype.com/support/tech/MTEF3.htm
 *  http://web.archive.org/web/20010304111449/http://mathtype.com/support/tech/MTEF_storage.htm
 *  http://web.archive.org/web/20021020115826/http://www.mathtype.com/support/tech/encodings/mtcode.stm
 */

# include       <stdint.h>
# include       <stdio.h>
# include       <string.h>
# include       <stdlib.h>
# include       <ctype.h>

# include       "rtf.h"
# include       "rtf2latex2e.h"
# include       "eqn.h"
# include       "eqn_support.h"

# define        DEBUG_PARSING     0
# define        DEBUG_TRANSLATION 0
# define        DEBUG_TEMPLATE    0
# define        DEBUG_FONT        0
# define        DEBUG_EMBELLS     0
# define        DEBUG_CHAR        0
# define        DEBUG_MODE        0
# define        DEBUG_SIZE        0
# define        DEBUG_JOIN        0
# define        DEBUG_LINE        0
# define        DEBUG_FUNCTION    0
# define        DEBUG_EQUATION    (DEBUG_PARSING || DEBUG_TRANSLATION || DEBUG_TEMPLATE)


static MT_OBJLIST *Eqn_GetObjectList(MTEquation * eqn, unsigned char *src, int *src_index, int num_objs);
static char *Eqn_TranslateObjects(MTEquation * eqn, MT_OBJLIST * the_list);
extern void hexdump (void *ptr, void *zero, uint32_t length, char *msg);

# define EQN_MODE_TEXT     0
# define EQN_MODE_INLINE   1
# define EQN_MODE_DISPLAY  2
# define EQN_MODE_EQNARRAY 3

/*  Various local data which used to be in rtf2latex.ini.  Initialized at bottom of file. */
extern char *Profile_FUNCTIONS[];
extern char *Profile_VARIABLES[];
extern char *Profile_PILEtranslation[];
extern char *Profile_MT_CHARSET_ATTS[];
extern char *Profile_CHARTABLE[];
extern char *Profile_TEMPLATES[];
extern char *Profile_TEMPLATES_5[];
extern char *Template_EMBELLS[];

char * typeFaceName[NUM_TYPEFACE_SLOTS] =
{
    "ZERO",
    "TEXT",
    "FUNCTION",
    "VARIABLE",
    "LCGREEK",
    "UCGREEK",
    "SYMBOL",
    "VECTOR",
    "NUMBER",
    "USER1",
    "USER2",
    "MTEXTRA",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "TEXT_FE",
    "EXPAND",
    "MARKER",
    "SPACE",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN",
    "UNKNOWN"
};

static
char *ToBuffer(char *src, char *buffer, uint32_t *off, uint32_t *lim)
{

    uint32_t zln = (uint32_t) strlen(src) + 1;

    if (*off + zln + 256 >= *lim) {
        char *newbuf;
        *lim = *off + zln + 1024;
        newbuf = (char *) malloc(*lim);
        strcpy(newbuf, buffer);
        free(buffer);
        buffer = newbuf;
    }

    strcpy(buffer + *off, src);
    *off += zln - 1;
    free(src);

    return buffer;
}

static void SetComment(EQ_STRREC * strs, int lev, char *src)
{
    strs[0].log_level = lev;
    strs[0].do_delete = 1;
    strs[0].ilk = Z_COMMENT;
    strs[0].is_line = 0;

    if (src) {
        uint32_t zln = (uint32_t) strlen(src) + 1;
        char *newbuf = (char *) malloc( (size_t) zln);
        strcpy(newbuf, src);
        strs[0].data = newbuf;
    } else
        strs[0].data = (char *) NULL;
}

static void print_tag(uint8_t tag, int src_index)
{
    switch (tag) {
    case 0:
        RTFMsg("[%03d] %-14s\n", src_index, "END");
        break;
    case 1:
        RTFMsg("[%03d] %-14s\n", src_index, "LINE");
        break;
    case 2:
        RTFMsg("[%03d] %-14s\n", src_index, "CHAR");
        break;
    case 3:
        RTFMsg("[%03d] %-14s\n", src_index, "TMPL");
        break;
    case 4:
        RTFMsg("[%03d] %-14s\n", src_index, "PILE");
        break;
    case 5:
        RTFMsg("[%03d] %-14s\n", src_index, "MATRIX");
        break;
    case 6:
        RTFMsg("[%03d] %-14s\n", src_index, "EMBELL");
        break;
    case 7:
        RTFMsg("[%03d] %-14s\n", src_index, "RULER");
        break;
    case 8:
        RTFMsg("[%03d] %-14s\n", src_index, "FONT_STYLE_DEF");
        break;
    case 9:
        RTFMsg("[%03d] %-14s\n", src_index, "SIZE");
        break;
    case 10:
        RTFMsg("[%03d] %-14s\n", src_index, "FULL");
        break;
    case 11:
        RTFMsg("[%03d] %-14s\n", src_index, "SUB");
        break;
    case 12:
        RTFMsg("[%03d] %-14s\n", src_index, "SUB2");
        break;
    case 13:
        RTFMsg("[%03d] %-14s\n", src_index, "SYM");
        break;
    case 14:
        RTFMsg("[%03d] %-14s\n", src_index, "SUBSYM");
        break;
    case 15:
        RTFMsg("[%03d] %-14s\n", src_index, "COLOR");
        break;
    case 16:
        RTFMsg("[%03d] %-14s\n", src_index, "COLOR_DEF");
        break;
    case 17:
        RTFMsg("[%03d] %-14s\n", src_index, "FONT_DEF");
        break;
    case 18:
        RTFMsg("[%03d] %-14s\n", src_index, "EQN_PREFS");
        break;
    case 19:
        RTFMsg("[%03d] %-14s\n", src_index, "ENCODING_DEF");
        break;
    default:
        RTFMsg("[%03d] %-14s\n", src_index, "FUTURE");
        break;
    }
}

/*
 * Nibble routines
 */
static unsigned char HiNibble(unsigned char x)
{
//    fprintf(stderr, "x=%d, high=%d, shifted=%d\n",x,(x & 0xF0),(x & 0xF0)/16);
    return (unsigned char) ((x & 0xF0)/16);
}

static unsigned char LoNibble(unsigned char x)
{
    return (x & 0x0F);
}

static void PrintNibble(unsigned char n)
{
    if (1)
        return;

    if (n <= 9)
        fprintf(stderr, "%d", n);
    else if (n == 0x0A)
        fprintf(stderr, ".");
    else if (n == 0x0B)
        fprintf(stderr, "-");
    else if (n == 0x0F)
        fprintf(stderr, " ");
    else
        RTFPanic("Bad nibble\n");
}

static void PrintNibbleDimension(unsigned char n)
{
    if (1)
        return;
    switch (n) {
    case 0:
        fprintf(stderr, "in ");
        break;
    case 1:
        fprintf(stderr, "cm ");
        break;
    case 2:
        fprintf(stderr, "pt ");
        break;
    case 3:
        fprintf(stderr, "pc ");
        break;
    case 4:
        fprintf(stderr, " %% ");
        break;
    default:
        RTFPanic("Bad nibble\n");
    }
}


static int SkipNibbles(unsigned char *p, int num)
{
    unsigned char hi, lo;
    int nbytes = 0;
    int count = 1;
    int new_str = 1;

    if (0) fprintf(stderr, " #%02d -- ", count);
    while (count <= num) {

        hi = HiNibble(*(p + nbytes));
        lo = LoNibble(*(p + nbytes));
        nbytes++;

        if (new_str)
            PrintNibbleDimension(hi);
        else
            PrintNibble(hi);

        new_str = 0;
        if (hi == 0x0F) {
            new_str = 1;
            count++;
            if (0) fprintf(stderr, " ---> total of %d bytes\n #%02d -- ", nbytes, count);
        }

        if (new_str)
            PrintNibbleDimension(lo);
        else
            PrintNibble(lo);

        new_str = 0;
        if (lo == 0x0F) {
            new_str = 1;
            count++;
            if (0) fprintf(stderr, " ---> total of %d bytes\n #%02d -- ", nbytes, count);
        }
    }
    if (0) fprintf(stderr, "\n");

    return nbytes;
}


/*  section contains strings of the form */
/*     key=data */
static
uint32_t GetProfileStr(char **section, char *key, char *data, int datalen)
{
    char **rover;
    size_t keylen;

	data[0] = '\0';
    if (key == NULL) return 0;
    keylen = strlen(key);
    
    for (rover = &section[0]; *rover; ++rover) {
        if (strncmp(*rover, key, keylen) == 0) {
            strncpy(data, *rover + keylen + 1, (size_t) (datalen - 1));    /*  skip over = (no check for white space */
            data[datalen - 1] = '\0';      /*  null terminate */
            return ((uint32_t) strlen(data));
        }
    }

    return 0;
}


/*  this mangles the contents of strs[].data, so just call once! */
static
char *Eqn_JoinStrings(MTEquation * eqn, EQ_STRREC * strs, int num_strs)
{
    char join[8192], buf[128];
    char *p, *substition_text, *marker, *thetex;
    char *vars[3];
    int i, j, id, is_pile;

    if (DEBUG_JOIN) {
        for (i=0; i<num_strs; i++)
            fprintf(stderr,"   is_line=%d, strs[%d].data=%s\n", strs[i].is_line, i, strs[i].data);
    }
    
    *join = '\0';

    for (i=0; i<num_strs; i++) {
    
        if (!strs[i].data) continue;
            
        if (strs[i].log_level > eqn->log_level) continue;
        
        if (strs[i].ilk != Z_TMPL) { 
            strcat(join, strs[i].data);
            continue;
        }

        /*  the current string is a TMPL and needs to be filled and added */
        /*  e.g., dat = "\sqrt[#2]{#1}" */
        
        p = strs[i].data;
        
        while (*p) {

            marker = strchr(p, '#');
            if (!marker) {
                strcat(join, p);
                break;
            }
            
            *marker = '\0';
            strcat(join, p);
            p = marker + 1;
                
/*  #1[L][STARTSUB][ENDSUB]  ... substitute text according to byzantine scheme */
            
            /* only #1, #2, and #3 are used */  
            id = *p - '1';
            if (id < 0 || id > 3) break;
            p++;
            
            /* Extract the bracketed items */
            /* vars[0]="L", vars[1]="STARTSUB", vars[2]="ENDSUB" */
            /* only vars[1] and vars[2] are used */
            
            vars[1] = NULL;
            vars[2] = NULL;
            j = 0;
            while (*p == '[') {     
                p++;
                marker = strchr(p, ']');
                if (!marker) break;
                *marker = '\0';
                vars[j++] = p;
                p = marker+1;
            }

            /*  This is pretty confusing. All the strs[] have an is_line flag */
            /*  only strs[] entries that have this flag set may be used for replacement */
            /*  The replacement text for #1 or #2 or #3 is the first, second, or third */
            /*  strs[] entry that has its flag set. */
            
            thetex = NULL;
            for (j=i+1; j<num_strs; j++) {
                if (strs[j].is_line == 0) continue;
                            
                if (id == 0) {
                    thetex = strs[j].data;
                    strs[j].log_level = 100; /*  mark this entry as used */
                    is_pile = (strs[j].is_line == 2) ? 1 : 0;
                    break;
                }
                id--;  /*  one string closer to our goal */
            }

            /*  this should not happen, but if it does, just go on to the next character */
            if (!thetex) continue;

            if (GetProfileStr(Profile_VARIABLES, vars[1], buf, 128)) {
                
                substition_text = buf;
                marker = strchr(buf, ',');

                if (is_pile)
                    substition_text = marker + 1;
                else
                    *marker = '\0';

                strcat(join, substition_text);
            }

            strcat(join, thetex);

            if (GetProfileStr(Profile_VARIABLES, vars[2], buf, 128)) {
                
                substition_text = buf;
                marker = strchr(buf, ',');

                if (is_pile)
                    substition_text = marker + 1;
                else
                    *marker = 0;

                strcat(join, substition_text);
            }
        }
    }

    for (i=0; i<num_strs; i++)
        if (strs[i].do_delete && strs[i].data) {
            free(strs[i].data);
            strs[i].data = NULL;
        }
            
    thetex = (char *) malloc(strlen(join)+1);
    strcpy(thetex, join);
    if (DEBUG_JOIN) fprintf(stderr,"final join='%s'\n", thetex);
    return thetex;
}

/*  delete routines */



static
void DeleteTabstops(MT_TABSTOP * the_list)
{
    MT_TABSTOP *del_node;

    while (the_list) {
        del_node = the_list;
        the_list = (MT_TABSTOP *) (the_list->next);
        free(del_node);
    }
}


static
void DeleteEmbells(MT_EMBELL * the_list)
{
    MT_EMBELL *del_node;
    while (the_list) {
        del_node = the_list;
        the_list = (MT_EMBELL *) the_list->next;
        free(del_node);
    }
}


static
void DeleteObjectList(MT_OBJLIST * the_list)
{
    MT_OBJLIST *del_node;
    MT_LINE *line;
    MT_CHAR *charptr;

    while (the_list) {

        del_node = the_list;
        the_list = (void *) the_list->next;

        switch (del_node->tag) {

        case LINE:{
                line = (MT_LINE *) del_node->obj_ptr;
                if (line) {
                    if (line->ruler) {
                        DeleteTabstops(line->ruler->tabstop_list);
                        free(line->ruler);
                    }

                    if (line->object_list)
                        DeleteObjectList(line->object_list);
                    free(line);
                }
            }
            break;

        case CHAR:{
                charptr = (MT_CHAR *) del_node->obj_ptr;
                if (charptr) {
                    if (charptr->embellishment_list)
                        DeleteEmbells(charptr->embellishment_list);
                    free(charptr);
                }
            }
            break;

        case TMPL:{
                MT_TMPL *tmpl = (MT_TMPL *) del_node->obj_ptr;
                if (tmpl) {
                    if (tmpl->subobject_list)
                        DeleteObjectList(tmpl->subobject_list);
                    free(tmpl);
                }
            }
            break;

        case PILE:{
                MT_PILE *pile = (MT_PILE *) del_node->obj_ptr;
                if (pile) {
                    if (pile->line_list)
                        DeleteObjectList(pile->line_list);
                    free(pile);
                }
            }
            break;

        case MATRIX:{
                MT_MATRIX *matrix = (MT_MATRIX *) del_node->obj_ptr;
                if (matrix) {
                    if (matrix->element_list)
                        DeleteObjectList(matrix->element_list);
                    free(matrix);
                }
            }
            break;

        case EMBELL:
            break;

        case RULER:{
                MT_RULER *ruler = (MT_RULER *) del_node->obj_ptr;
                if (ruler) {
                    if (ruler->tabstop_list)
                        DeleteTabstops(ruler->tabstop_list);
                    free(ruler);
                }
            }
            break;

        case FONT:{
                MT_FONT *font = (MT_FONT *) del_node->obj_ptr;
                if (font) {
                    if (font->zname)
                        free(font->zname);
                    free(font);
                }
            }
            break;

        case SIZE:
        case FULL:
        case SUB:
        case SUB2:
        case SYM:
        case SUBSYM:{
                MT_SIZE *size = (MT_SIZE *) del_node->obj_ptr;
                if (size) {
                    free(size);
                }
            }
            break;

        default:
            break;
        }

        free(del_node);
    }                           /*   while ( the_list ) */
}

void Eqn_Destroy(MTEquation * eqn)
{
    if (eqn->o_list) {
        DeleteObjectList(eqn->o_list);
        eqn->o_list = NULL;
    }
    if (eqn->atts_table) {
        free(eqn->atts_table);
        eqn->atts_table = NULL;
    }
    if (eqn->m_latex_start) {
        free(eqn->m_latex_start);
        eqn->m_latex_start = NULL;
    }
    if (eqn->m_latex_end) {
        free(eqn->m_latex_end);
        eqn->m_latex_end = NULL;
    }
    if (eqn->m_latex) {
        free(eqn->m_latex);
        eqn->m_latex = NULL;
    }
    
}

static void setMathMode(MTEquation * eqn, int mode)
{
    char s[50];
    if (DEBUG_MODE || g_input_file_type==TYPE_EQN) fprintf(stderr,"old=%d, new=%d\n",eqn->m_mode,mode);
    
    switch (mode) {
    case EQN_MODE_TEXT:
        switch (eqn->m_mode) {
        case EQN_MODE_TEXT:
            break;
        case EQN_MODE_INLINE:
            strcpy(s,eqn_end_inline);
            if (eqn->m_latex_end) free(eqn->m_latex_end);
            eqn->m_latex_end = strdup(s);
            break;     
        case EQN_MODE_DISPLAY:
            strcpy(s,eqn_end_display);
            if (eqn->m_latex_end) free(eqn->m_latex_end);
            eqn->m_latex_end = strdup(s);
            break;          
        case EQN_MODE_EQNARRAY:
            strcpy(s,"\\end{eqnarray}\n");
            if (eqn->m_latex_end) free(eqn->m_latex_end);
            eqn->m_latex_end = strdup(s);
            break;
        }
        break;
        
    case EQN_MODE_INLINE:
        switch (eqn->m_mode) {
        case EQN_MODE_TEXT:
            if (eqn->m_latex_start) free(eqn->m_latex_start);
            eqn->m_latex_start = strdup(eqn_start_inline);
            break;
        case EQN_MODE_INLINE:
            break;     
        case EQN_MODE_DISPLAY:
            fprintf(stderr,"Bizarre case of switching from display to inline??\n");
            break;          
        case EQN_MODE_EQNARRAY:
            fprintf(stderr,"Bizarre case of switching from eqnarray to inline??\n");
            break;
        }
        break;
   
    case EQN_MODE_DISPLAY:
        switch (eqn->m_mode) {
        case EQN_MODE_TEXT:
            if (eqn->m_latex_start) free(eqn->m_latex_start);
            eqn->m_latex_start = strdup(eqn_start_display);
            break;
        case EQN_MODE_INLINE:
            fprintf(stderr,"Bizarre case of switching from inline to display??\n");
            break;     
        case EQN_MODE_DISPLAY:
            break;          
        case EQN_MODE_EQNARRAY:
            fprintf(stderr,"Bizarre case of switching from eqnarry to display??\n");
            break;
        }
        break;
        
    case EQN_MODE_EQNARRAY:
        switch (eqn->m_mode) {
        case EQN_MODE_TEXT:
            if (eqn->m_latex_start) free(eqn->m_latex_start);
            eqn->m_latex_start = strdup("\\begin{eqnarray}");
            break;
        case EQN_MODE_INLINE:
            fprintf(stderr,"Bizarre case of switching from inline to eqnarry??\n");
            break;     
        case EQN_MODE_DISPLAY:
            fprintf(stderr,"Bizarre case of switching from display to eqnarry??\n");
            break;          
        case EQN_MODE_EQNARRAY:
            break;
        }
        break;
    }
    
    eqn->m_mode = mode;
    
/*    if (strlen(s))
    	return strdup(s);
    else
    	return NULL;*/
}

static int GetAttribute(MTEquation * eqn, unsigned char *src, uint8_t *attrs)
{
    if (eqn->m_mtef_ver < 5) {
        *attrs = HiNibble(*src);
        return 1;
    }

    *attrs = *(src + 1);
    return 2;
}

static
int GetNudge(unsigned char *src, int16_t *x, int16_t *y)
{

    int nudge_length;
    int16_t tmp;

    unsigned char b1 = *src;
    unsigned char b2 = *(src + 1);


    if (b1 == 128 && b2 == 128) {
        tmp  = (int16_t) *(src + 2);
        tmp |= (int16_t) *(src + 3) << 8;
        *x = tmp;
        tmp  = (int16_t) *(src + 4);
        tmp |= (int16_t) *(src + 5) << 8;
        *y = tmp;
        nudge_length = 6;
    } else {
        *x = b1;
        *y = b2;
        nudge_length = 2;
    }

    if (g_input_file_type==TYPE_EQN) fprintf(stderr, "nudge gotten size=%d",nudge_length);

    return nudge_length;
}

static MT_RULER *Eqn_inputRULER(MTEquation * eqn, unsigned char *src, int *src_index)
{
    MT_RULER *new_ruler;
    MT_TABSTOP *head, *curr, *new_stop;
    uint8_t i, num_stops, tag;

    /* if we arrived here from LINE, then there does not seem to be
       a proper RULER tag.  Skip the tag only if it is a RULER */
    tag = *(src + *src_index) & 0x0F;
    if (tag == 7)
        (*src_index)++;             /*  step over ruler tag */

    head = NULL;
    num_stops = *(src + *src_index);
    (*src_index)++;
    
    for (i=0; i < num_stops; i++) {
        new_stop = (MT_TABSTOP *) malloc(sizeof(MT_TABSTOP));
        new_stop->next = NULL;
        new_stop->type = *(src + *src_index);
        (*src_index)++;
        
        new_stop->offset = *(src + *src_index);
        (*src_index)++;
        new_stop->offset |= *(src + *src_index) << 8;
        (*src_index)++;

        if (head)
            curr->next = (struct MT_TABSTOP *) new_stop;
        else
            head = new_stop;
        curr = new_stop;
    }

    new_ruler = (MT_RULER *) malloc(sizeof(MT_RULER));
    new_ruler->n_stops = num_stops;
    new_ruler->tabstop_list = head;

    return new_ruler;
}


/*
    * record type (1)
    * options
    * [nudge] if mtefOPT_NUDGE is set
    * [line spacing] if mtefOPT_LINE_LSPACE is set (16-bit integer)
    * [RULER record] if mtefOPT_LP_RULER is set
    * object list contents of line (a single pile, characters and templates, or nothing)
*/

static MT_LINE *Eqn_inputLINE(MTEquation * eqn, unsigned char *src,
                       int *src_index)
{
    unsigned char attrs;
    MT_LINE *new_line = (MT_LINE *) malloc(sizeof(MT_LINE));
    new_line->nudge_x = 0;
    new_line->nudge_y = 0;
    new_line->line_spacing = 0;
    new_line->ruler = NULL;
    new_line->object_list = NULL;

    *src_index += GetAttribute(eqn, src+*src_index, &attrs);

    if (DEBUG_LINE) fprintf(stderr, "LINE options  = 0x%02x\n", attrs);

    if (attrs & xfLMOVE)
        *src_index += GetNudge(src + *src_index, &new_line->nudge_x, &new_line->nudge_y);

    if (attrs & xfLSPACE) {
        new_line->line_spacing = *(src + *src_index);
        (*src_index)++;
    }
    
    if (attrs & xfRULER)
        new_line->ruler = Eqn_inputRULER(eqn, src, src_index);

    if (!(attrs & xfNULL))
        new_line->object_list = Eqn_GetObjectList(eqn, src, src_index, 0);

    return new_line;
}


static MT_EMBELL *Eqn_inputEMBELL(MTEquation * eqn, unsigned char *src, int *src_index)
{
    unsigned char attrs, tag;
    MT_EMBELL *head = NULL;
    MT_EMBELL *new_embell = NULL;
    MT_EMBELL *curr = NULL;
    tag = EMBELL;

    while (tag == EMBELL) {
        new_embell = (MT_EMBELL *) malloc(sizeof(MT_EMBELL));
        new_embell->next = NULL;
        new_embell->nudge_x = 0;
        new_embell->nudge_y = 0;

        *src_index += GetAttribute(eqn, src+*src_index, &attrs);
        
        if (attrs & xfLMOVE)
            *src_index += GetNudge(src + *src_index, &new_embell->nudge_x, &new_embell->nudge_y);
        
        new_embell->embell = *(src + *src_index);
        if (DEBUG_EMBELLS  || g_input_file_type==TYPE_EQN) 
            fprintf(stderr, "[%-3d] EMBELL --- embell=%d\n", *src_index, (int) new_embell->embell);
        (*src_index)++;

        if (head)
            curr->next = (struct MT_EMBELL *) new_embell;
        else
            head = new_embell;
        curr = new_embell;

        if (eqn->m_mtef_ver == 5)
            tag = *(src + *src_index);
        else
            tag = LoNibble(*(src + *src_index));
    }

    (*src_index)++;             /*  advance over end byte */

    return head;
}


/*
    * attributes
    * [nudge] if xfLMOVE is set
    * [typeface] typeface value (see FONT below) + 128
    * [character] 16-bit character value (encoding depends on typeface)
    * [embellishment list] if xfEMBELL is set (embellishments)
*/

static MT_CHAR *Eqn_inputCHAR(MTEquation * eqn, unsigned char *src, int *src_index)
{
    unsigned char attrs;
    MT_CHAR *new_char = (MT_CHAR *) malloc(sizeof(MT_CHAR));
    new_char->nudge_x = 0;
    new_char->nudge_y = 0;
    new_char->mtchar = 0;
    new_char->bits16 = 0;
    new_char->embellishment_list = (MT_EMBELL *) NULL;

    *src_index += GetAttribute(eqn, src+*src_index, &attrs);
    new_char->atts = attrs;

    if (new_char->atts & CHAR_NUDGE)
        *src_index += GetNudge(src + *src_index, &new_char->nudge_x, &new_char->nudge_y);

    new_char->typeface = *(src + *src_index);
    (*src_index)++;

    if (eqn->m_mtef_ver < 5) {
        new_char->character = *(src + *src_index);
        (*src_index)++;
        if (eqn->m_platform == PLATFORM_WIN) {
			new_char->character |= *(src + *src_index) << 8;
			(*src_index)++;
        }

    } else {
    
        /* nearly always have a 16 bit MT character */
        if (!(new_char->atts & CHAR_ENC_NO_MTCODE)) {
            new_char->character = *(src + *src_index);
            (*src_index)++;
            new_char->character |= *(src + *src_index) << 8;
            (*src_index)++;
        }
        
        /*  02 05 84 bc 03 6d 06 00  11 00 02 04 86  */
        if (new_char->atts & CHAR_ENC_CHAR_8) {
            new_char->character = *(src + *src_index);
            (*src_index)++;
        }
        
        if (new_char->atts & CHAR_ENC_CHAR_16) {
            new_char->bits16 = *(src + *src_index);
            (*src_index)++;
            new_char->bits16 |= *(src + *src_index) << 8;
            (*src_index)++;
        }
    }

    if (g_input_file_type==TYPE_EQN || DEBUG_CHAR) {
        fprintf(stderr, "          '%c' or 0x%04x,", (int)new_char->character, (unsigned int) new_char->character);
        fprintf(stderr, " typeface = %d mtchar=%u, 16bit=%u ", 
                  (int) new_char->typeface-128, (unsigned int) new_char->mtchar, (unsigned int) new_char->bits16);
        fprintf(stderr, " attr = 0x%02x \n", (unsigned int) new_char->atts);
    }

    if (eqn->m_mtef_ver == 5) {
        if (new_char->atts & CHAR_EMBELL)
            new_char->embellishment_list = Eqn_inputEMBELL(eqn, src, src_index);
    } else {
        if (new_char->atts & xfEMBELL)
            new_char->embellishment_list = Eqn_inputEMBELL(eqn, src, src_index);
    }


    return new_char;
}

static MT_TMPL *Eqn_inputTMPL(MTEquation * eqn, unsigned char *src, int *src_index)
{
    unsigned char attrs;
    MT_TMPL *new_tmpl = (MT_TMPL *) malloc(sizeof(MT_TMPL));
    new_tmpl->nudge_x = 0;
    new_tmpl->nudge_y = 0;

    *src_index += GetAttribute(eqn, src+*src_index, &attrs);

    if (attrs & xfLMOVE)
        *src_index += GetNudge(src + *src_index, &new_tmpl->nudge_x, &new_tmpl->nudge_y);

    new_tmpl->selector = *(src + *src_index);
    (*src_index)++;

    new_tmpl->variation = *(src + *src_index);
    (*src_index)++;

    if (eqn->m_mtef_ver == 5 && (new_tmpl->variation & 0x80) ) {
        new_tmpl->variation &= 0x7F;
        new_tmpl->variation |=  *(src + *src_index) << 8;
        (*src_index)++;
        RTFMsg("Two byte variation!\n");
    }

    new_tmpl->options = *(src + *src_index);
    (*src_index)++;

//	if (eqn->m_mtef_ver == 3 && 18 <= new_tmpl->selector && new_tmpl->selector <= 20)
	//	new_tmpl->variation = new_tmpl->variation >> 8;

    if ( (DEBUG_PARSING && DEBUG_TEMPLATE) || g_input_file_type==TYPE_EQN) 
        fprintf(stderr, "TMPL : read sel=%2d var=0x%04x (%d.%d)\n", 
        (int) new_tmpl->selector, (unsigned int) new_tmpl->variation, (int) new_tmpl->selector, (int) new_tmpl->variation);

    new_tmpl->subobject_list = Eqn_GetObjectList(eqn, src, src_index, 0);

    return new_tmpl;
}


static MT_PILE *Eqn_inputPILE(MTEquation * eqn, unsigned char *src,
                       int *src_index)
{
    unsigned char attrs;
    MT_PILE *new_pile = (MT_PILE *) malloc(sizeof(MT_PILE));
    new_pile->nudge_x = 0;
    new_pile->nudge_y = 0;
    new_pile->ruler = NULL;

    *src_index += GetAttribute(eqn, src+*src_index, &attrs);

    if (attrs & xfLMOVE)
        *src_index += GetNudge(src + *src_index, &new_pile->nudge_x, &new_pile->nudge_y);

    new_pile->halign = *(src + *src_index);
    (*src_index)++;

    new_pile->valign = *(src + *src_index);
    (*src_index)++;

    if (attrs & xfRULER)
        new_pile->ruler = Eqn_inputRULER(eqn, src, src_index);

    new_pile->line_list = Eqn_GetObjectList(eqn, src, src_index, 0);

    return new_pile;
}


static MT_MATRIX *Eqn_inputMATRIX(MTEquation * eqn, unsigned char *src,
                           int *src_index)
{
    uint8_t attrs, i, bytes;

    MT_MATRIX *new_matrix = (MT_MATRIX *) malloc(sizeof(MT_MATRIX));
    new_matrix->nudge_x = 0;
    new_matrix->nudge_y = 0;

    *src_index += GetAttribute(eqn, src+*src_index, &attrs);

    if (attrs & xfLMOVE)
        *src_index += GetNudge(src + *src_index, &new_matrix->nudge_x, &new_matrix->nudge_y);

    new_matrix->valign = *(src + *src_index);
    (*src_index)++;
    new_matrix->h_just = *(src + *src_index);
    (*src_index)++;
    new_matrix->v_just = *(src + *src_index);
    (*src_index)++;
    new_matrix->rows = *(src + *src_index);
    (*src_index)++;
    new_matrix->cols = *(src + *src_index);
    (*src_index)++;

    /* row partition consists of (rows+1) two-bit values */
    bytes = (2 * (new_matrix->rows + 1) + 7) / 8;
    for (i=0; i<bytes; i++) {
        new_matrix->row_parts[i] = *(src + *src_index);
        (*src_index)++;
    }

    /* col partition consists of (cols+1) two-bit values */
    bytes = (2 * (new_matrix->cols + 1) + 7) / 8;
    for (i=0; i<bytes; i++) {
        new_matrix->col_parts[i] = *(src + *src_index);
        (*src_index)++;
    }

    new_matrix->element_list = Eqn_GetObjectList(eqn, src, src_index, 0);

    return new_matrix;
}

static MT_FONT *Eqn_inputFONT(MTEquation * eqn, unsigned char *src,
                       int *src_index)
{
    uint32_t zln;
    MT_FONT *new_font = (MT_FONT *) malloc(sizeof(MT_FONT));

    (*src_index)++;             /*  step over the tag */

    new_font->tface = *(src + *src_index);
    (*src_index)++;
    new_font->style = *(src + *src_index);
    (*src_index)++;

    zln = (uint32_t) strlen((char *) src + *src_index) + 1;
    new_font->zname = (char *) malloc(sizeof(char) * zln);
    strcpy((char *) new_font->zname, (char *) src + *src_index);
    *src_index += zln;

    return new_font;
}

static MT_SIZE *Eqn_inputSIZE(MTEquation * eqn, unsigned char *src,
                       int *src_index)
{
    unsigned char tag, option;
    MT_SIZE *new_size = (MT_SIZE *) malloc(sizeof(MT_SIZE));
    new_size->dsize = 0;

    /* also works in MTEF5 because all supported tags are less than 16 */
    tag = *(src + *src_index) & 0x0F;
    (*src_index)++;

    /* FULL or SUB or SUB2 or SYM or SUBSYM */
    if (tag >= FULL && tag <= SUBSYM) {
//    fprintf(stderr,"tag= %d\n",tag);
        new_size->type  = tag;
        new_size->lsize = tag - FULL;
        return new_size;
    } 
    
    option = *(src + *src_index);
    (*src_index)++;
    
    /* large dsize */
    if (option == 100) {
        new_size->type = option;
        new_size->lsize = *(src + *src_index);
        (*src_index)++;
        new_size->dsize = *(src + *src_index);
        (*src_index)++;
        new_size->dsize += (*(src + *src_index) << 8);
        (*src_index)++;
        return new_size;
    }
    
    /* explicit point size */
    if (option == 101) {
        new_size->type = option;
        new_size->lsize = *(src + *src_index);
        (*src_index)++;
        new_size->lsize += (*(src + *src_index) << 8);
        (*src_index)++;
        return new_size;
    } 
    
    /* -128 < dsize < 128 */
    new_size->type = 0;
    new_size->lsize = option;
    new_size->dsize = *(src + *src_index) - 128;
    (*src_index)++;
    return new_size;
}


/*
 * Convert MT equation into internal form
 * at each Eqn_inputXXX call, the src_index is set to the XXX tag
 * so that the hi nibble can be used to obtain the options in 
 * versions 1-4 of MTEF
*/
static MT_OBJLIST *Eqn_GetObjectList(MTEquation * eqn, unsigned char *src, int *src_index, int num_objs)
{
    static int subroutine_depth = 0;
    unsigned char c, size, curr_tag;
    int i,id;
    int tally = 0;
    MT_OBJLIST *head = (MT_OBJLIST *) NULL;
    MT_OBJLIST *curr;
    void *new_obj;

    ++subroutine_depth;
    if (eqn->m_mtef_ver == 5)
        curr_tag = *(src + *src_index);
    else
        curr_tag = LoNibble(*(src + *src_index));

    while (curr_tag != 0xFF) {

        new_obj = (void *) NULL;

if (DEBUG_PARSING || g_input_file_type==TYPE_EQN) {
        print_tag(curr_tag, *src_index);
        hexdump(src+*src_index, src+*src_index, 16, NULL);
}

        switch (curr_tag) {
        case END:
            (*src_index)++;
            subroutine_depth--;
            return head;
            break;

        case LINE:
            new_obj = (void *) Eqn_inputLINE(eqn, src, src_index);
            break;
        case CHAR:
            new_obj = (void *) Eqn_inputCHAR(eqn, src, src_index);
            break;
        case TMPL:
            new_obj = (void *) Eqn_inputTMPL(eqn, src, src_index);
            break;
        case PILE:
            new_obj = (void *) Eqn_inputPILE(eqn, src, src_index);
            break;
        case MATRIX:
            new_obj = (void *) Eqn_inputMATRIX(eqn, src, src_index);
            break;

        case EMBELL:
            new_obj = (void *) Eqn_inputEMBELL(eqn, src, src_index);
            break;

        case RULER:
            new_obj = (void *) Eqn_inputRULER(eqn, src, src_index);
            break;
        case FONT:
            new_obj = (void *) Eqn_inputFONT(eqn, src, src_index);
            break;

        case SIZE:
        case FULL:
        case SUB:
        case SUB2:
        case SYM:
        case SUBSYM:
            new_obj = (void *) Eqn_inputSIZE(eqn, src, src_index);
            break;

        case COLOR_DEF:
            break;

        case FONT_DEF:
            (*src_index)++;
            id = *(src + *src_index);
            (*src_index)++;
            if (DEBUG_FONT) fprintf(stderr,"          ");
            while ((c = *(src + *src_index))) { 
                if (DEBUG_FONT) fprintf(stderr,"%c",c);
                (*src_index)++;
            }
            if (DEBUG_FONT) fprintf(stderr," ==> %d\n",id);
            (*src_index)++;
            tally--;
            break;

        case EQN_PREFS:
            (*src_index)++;     /* skip tag */
            (*src_index)++;     /* options */

            size = *(src + *src_index); /* sizes[] */
            if (0) fprintf(stderr, " size array has %d entries\n", size);
            (*src_index)++;
            (*src_index) += SkipNibbles(src + *src_index, size);

            size = *(src + *src_index); /* spaces[] */
            (*src_index)++;
            (*src_index) += SkipNibbles(src + *src_index, size);

            size = *(src + *src_index); /* styles[] */
            (*src_index)++;
            if (0) fprintf(stderr, " style array has %d entries\n", size);
            for (i = 0; i < size; i++) {
                c = *(src + *src_index);
                (*src_index)++;
                if (c) {
                   /* c = *(src + *src_index); */
                    (*src_index)++;
                }
            }
            tally--;
            break;

        case ENCODING_DEF:
            (*src_index)++;     /* skip tag */
            while ((c = *(src + *src_index))) {
                if (0) fprintf(stderr, "%c", c);
                (*src_index)++;
            }
            if (0) fprintf(stderr, "\n");
            (*src_index)++;     /* skip NULL */
            tally--;
            break;

        default:
            (*src_index)++;     /* skip tag */
            size = *(src + *src_index);
            (*src_index)++;
            size |= *(src + *src_index) << 8;
            
            //Need to include a change to how the *src_index tag is incremented. 
            //Although the MTEF v.5 documentation says otherwise, it appears the 
            //index should be incremented by size-1, not size. 
            (*src_index) += size; 

            fprintf(stderr, "Future tag = 0x%02x with size %d\n",curr_tag,size);
            fprintf(stderr,"--> ignoring!\n");
            tally--;
            break;
        }

        if (new_obj) {
            MT_OBJLIST *new_node =
                (MT_OBJLIST *) malloc(sizeof(MT_OBJLIST));
            new_node->next = NULL;
            new_node->tag = curr_tag;
            new_node->obj_ptr = new_obj;

            if (head)
                curr->next = (void *) new_node;
            else
                head = new_node;
            curr = new_node;
        }

        if (eqn->m_mtef_ver == 5)
            curr_tag = *(src + *src_index);
        else
            curr_tag = LoNibble(*(src + *src_index));

        tally++;
 
//        fprintf(stderr,"depth=%d number of objects/total = %d/%d\n", subroutine_depth, tally, num_objs);
        
        if (tally == num_objs) {
    		subroutine_depth--;
            return head;
        }

    }                           /*  while loop thru MathType Objects */

    (*src_index)++;             /*  step over end byte */

    subroutine_depth--;
    return head;
}

static
void Eqn_LoadCharSetAtts(MTEquation * eqn, char **table)
{
    char key[16];
    char buff[16];
    uint8_t slot = 1;
    uint32_t zln;

    eqn->atts_table = malloc(sizeof(MT_CHARSET_ATTS) * NUM_TYPEFACE_SLOTS);

    while (slot <= NUM_TYPEFACE_SLOTS) {
        snprintf(key, 16, "%d", (int) slot + 128);
        zln = GetProfileStr(table, key, buff, 16);
        if (zln) {
            eqn->atts_table[slot - 1].mathattr = buff[0] - '0';
            eqn->atts_table[slot - 1].do_lookup = buff[2] - '0';
            eqn->atts_table[slot - 1].use_codepoint = buff[4] - '0';
        } else {
            eqn->atts_table[slot - 1].mathattr = 1;
            eqn->atts_table[slot - 1].do_lookup = 0;
            eqn->atts_table[slot - 1].use_codepoint = 1;
        }

        slot++;
    }
}

/* parse the equation header and fill the fields of eqn */
int Eqn_Create(MTEquation * eqn, unsigned char *eqn_stream, int eqn_size)
{
    int src_index = 0;

    eqn->indent[0] = '%';
    eqn->indent[1] = 0;
    eqn->o_list = NULL;
    eqn->atts_table = NULL;
    eqn->m_mode = EQN_MODE_TEXT;
    eqn->m_inline = 0;
    eqn->m_latex_start = NULL;
    eqn->m_latex_end = NULL;
    eqn->m_latex = NULL;
    eqn->m_mtef_ver = 0;

	if (g_input_file_type != TYPE_RAWEQN) {
    	eqn->m_mtef_ver = eqn_stream[0];
		src_index++;
	} else {
		fprintf(stderr, "skipping 0x %02X %02X %02X %02X\n", eqn_stream[0], eqn_stream[1], eqn_stream[2], eqn_stream[3]);
		src_index+=4;
	}
		
    switch (eqn->m_mtef_ver) {
    case 0:
        eqn->m_mtef_ver = 5;
        eqn->m_product = 0;
        eqn->m_version = 0;
        eqn->m_version_sub = 0;
        break;
    case 1:
    case 101:
        eqn->m_platform = (eqn->m_mtef_ver == 101) ? 1 : 0;
        eqn->m_product = 0;
        eqn->m_version = 1;
        eqn->m_version_sub = 0;
        break;

    case 2:
    case 3:
    case 4:
        eqn->m_platform = eqn_stream[src_index++];
        eqn->m_product = eqn_stream[src_index++];
        eqn->m_version = eqn_stream[src_index++];
        eqn->m_version_sub = eqn_stream[src_index++];
        break;

    case 5:
        eqn->m_platform = eqn_stream[src_index++];
        eqn->m_product = eqn_stream[src_index++];
        eqn->m_version = eqn_stream[src_index++];
        eqn->m_version_sub = eqn_stream[src_index++];

        /* the application key is a null terminated string */
        while (eqn_stream[src_index]) {
//            fprintf(stderr, "%d", eqn_stream[src_index]);
            src_index++;
            if (src_index == eqn_size) {
                RTFMsg("The Application Key for the Equation is screwy!");
                return (false);
            }
        }
        /*  fprintf(stderr, "\n"); */
        src_index++;

        eqn->m_inline = eqn_stream[src_index++];
        break;

    default:
        RTFMsg("* Unsupported MathType Equation Binary Format (MTEF=%d)\n",
               eqn->m_mtef_ver);
        return (false);
    }

    if (g_input_file_type==TYPE_EQN || DEBUG_EQUATION) {
        fprintf(stderr,"* MTEF ver = %d\n", eqn->m_mtef_ver);
        fprintf(stderr,"* Platform = %s\n", (eqn->m_platform == PLATFORM_WIN) ? "Win" : "Mac");
        fprintf(stderr,"* Product  = %s\n", (eqn->m_product) ? "MathType" : "EqnEditor");
        fprintf(stderr,"* Version  = %d.%d\n", eqn->m_version, eqn->m_version_sub);
        fprintf(stderr,"* Type     = %s (ignored because it is unreliable)\n", eqn->m_inline ? "inline" : "display");
    }
    
    eqn->m_atts_table = Profile_MT_CHARSET_ATTS;
    Eqn_LoadCharSetAtts(eqn, Profile_MT_CHARSET_ATTS);
    eqn->m_char_table = Profile_CHARTABLE;

    /*  We expect a SIZE then a LINE or PILE */
    eqn->o_list = Eqn_GetObjectList(eqn, eqn_stream, &src_index, 2);

    return (true);
}

/*  formatting routines.  convert internal form to LaTeX */



static
char *Eqn_TranslateRULER(MTEquation * eqn, MT_RULER * ruler)
{
    char buf[128];
    EQ_STRREC strs[2];
    int num_strs = 0;

    if (eqn->log_level >= 2) {
        snprintf(buf, 128, "\n%sRULER\n", eqn->indent);
        SetComment(strs, 2, buf);
        num_strs++;
    }

    strcat(eqn->indent, "  ");

    /*  no translation implemented yet */

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}

static
char *Eqn_TranslateLINE(MTEquation * eqn, MT_LINE * line)
{
    char buf[128];
    char *thetex;
    EQ_STRREC strs[3];

    int num_strs = 0;
    if (eqn->log_level >= 2) {
        snprintf(buf, 128, "\n%sLINE\n", eqn->indent);
        SetComment(strs, 2, buf);
        num_strs++;
    }

    strcat(eqn->indent, "  ");

    if (line->ruler) {
//    fprintf(stderr,"LINE---ruler\n");
        strs[num_strs].log_level = 0;
        strs[num_strs].do_delete = 1;
        strs[num_strs].ilk = Z_TEX;
        strs[num_strs].is_line = 0;
        strs[num_strs].data = Eqn_TranslateRULER(eqn, line->ruler);
        num_strs++;
    }

    if (line->object_list) {
//    fprintf(stderr,"LINE---object list\n");
        strs[num_strs].log_level = 0;
        strs[num_strs].do_delete = 1;
        strs[num_strs].ilk = Z_TEX;
        strs[num_strs].is_line = 0;
        strs[num_strs].data = Eqn_TranslateObjects(eqn, line->object_list);
        num_strs++;
    }

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    thetex = Eqn_JoinStrings(eqn, strs, num_strs);

    if (g_input_file_type==TYPE_EQN || DEBUG_LINE) fprintf(stderr,"LINE='%s'\n",thetex);
    return thetex;
}

/*  Character translation, MathType to TeX, using inifile data */

static
int Eqn_GetTexChar(MTEquation * eqn, EQ_STRREC * strs, MT_CHAR * thechar, int *math_attr)
{
    MT_CHARSET_ATTS set_atts;
    MT_EMBELL *embells;
    char buff[256];
    int num_strs = 0; 

    char *ztex = (char *) NULL;
    char zch = 0;
    char *zdata = (char *) NULL;

    if (DEBUG_CHAR) fprintf(stderr,"in GetTeXChar seeking eqn->atts_table[%d]\n",(int) thechar->typeface - 129);

    strs[0].log_level = 0;
    strs[0].do_delete = 1;
    strs[0].ilk = Z_TEX;
    strs[0].is_line = 0;
    strs[0].data = NULL;

    if (thechar->typeface >= 129 && thechar->typeface < 129 + NUM_TYPEFACE_SLOTS) {
        set_atts = eqn->atts_table[thechar->typeface - 129];
    } else {                    /*  unexpected charset */
        char buffer[16];
        char key[16];
        uint32_t zln;
        snprintf(key, 16, "%d", (int) thechar->typeface);
        zln = GetProfileStr(eqn->m_atts_table, key, buffer, 16);
        if (zln) {
            set_atts.mathattr = buffer[0] - '0';
            set_atts.do_lookup = buffer[2] - '0';
            set_atts.use_codepoint = buffer[4] - '0';
        } else {
            set_atts.mathattr = 1;
            set_atts.do_lookup = 1;
            set_atts.use_codepoint = 1;
        }
    }

    *math_attr = set_atts.mathattr;
    if (set_atts.do_lookup) {
        uint32_t zln;
        char key[16];           /*  132.65m */

        snprintf(key, 16, "%d.%d", (int)thechar->typeface, (int)thechar->character);
        if (DEBUG_CHAR) fprintf(stderr, "looking up char in table[%d] as %d.%d\n", 
        (int)thechar->typeface - 129, (int) thechar->typeface, (int) thechar->character);

        if (*math_attr == 3) {
            if (eqn->m_mode == EQN_MODE_TEXT)
                strcat(key, "t");
            else
                strcat(key, "m");
            *math_attr = 0;
        }

        zln = GetProfileStr(eqn->m_char_table, key, buff, 256);
        if (zln) {
            ztex = (char *) malloc(zln + 1);
            strcpy(ztex, buff);
        }
    }

    if (*math_attr == MA_FORCE_MATH && eqn->m_mode == EQN_MODE_TEXT) {
        setMathMode(eqn, eqn->m_inline ? EQN_MODE_INLINE : EQN_MODE_DISPLAY); 

    } else if (*math_attr == MA_FORCE_TEXT) {
        setMathMode(eqn, EQN_MODE_TEXT);
	}
    if (!ztex && set_atts.use_codepoint) {
        if (thechar->character >= 32 && thechar->character <= 127) {
            zch = (char) thechar->character;
            if (thechar->character == 38) {
                snprintf(buff,20,"\\&");
                ztex = (char *) malloc(strlen(buff) + 1);
                strcpy(ztex, buff);
            }
            if (thechar->typeface == 135) {
                snprintf(buff,20,"\\mathbf{%c}", zch);
                ztex = (char *) malloc(strlen(buff) + 1);
                strcpy(ztex, buff);
            }
        }
    }

    if (ztex) {
        zdata = ztex;
    } else if (zch) {
        zdata = (char *) malloc(2);
        zdata[0] = zch;
        zdata[1] = 0;
    }

    if (!zdata) 
        return num_strs;

    embells = thechar->embellishment_list;
    
    while (embells) {
        char template[128];
        *template = '\0';
        
        /* template will in the form "math template,text template" */
        if (embells->embell>1 && embells->embell<37)
            strcpy(template, Template_EMBELLS[embells->embell]);
        
        if (DEBUG_EMBELLS || g_input_file_type==TYPE_EQN) 
            fprintf(stderr,"Yikes --- Template_EMBELLS[%d]='%s'!\n", (int) embells->embell,template);

        if (strlen(template)) {     /*  only bother if there is a character */
            char *join, *t_ptr, *j_ptr;

            t_ptr = strchr(template, ',');
            
            if (!t_ptr) {
                RTFPanic("Malformed EMBELL Template!\n");
                exit(1);
            }
            
            /* set string to first or second half of template depending on mode */
            if (eqn->m_mode != EQN_MODE_TEXT) {
                *t_ptr = '\0';
                t_ptr = template;
            } else 
                t_ptr++;

            join = (char *) malloc(strlen(t_ptr) + strlen(zdata) + 16);
            j_ptr = join;

            if (DEBUG_EMBELLS || g_input_file_type==TYPE_EQN) fprintf(stderr,"Yikes --- replacement template is '%s'!\n",t_ptr);
            
            /* replace %1 in template with zdata */
            while (*t_ptr) {        
                if (*t_ptr == '%') {
                    t_ptr+=2;             /* skip over %1 */
                    strcpy(j_ptr, zdata);
                    j_ptr += strlen(zdata);
                } else {
                    *j_ptr = *t_ptr;
                    j_ptr++;
                    t_ptr++;
                }
            }
            *j_ptr = '\0';
            free(zdata);
            zdata = join;
        }
        if (DEBUG_EMBELLS || g_input_file_type==TYPE_EQN) fprintf(stderr,"Yikes --- after replacement strs[0].data is '%s'!\n",zdata);

        embells = (MT_EMBELL *) embells->next;
    }

    strs[0].data = zdata;
    num_strs++;
    return num_strs;
}

static
char *Eqn_TranslateCHAR(MTEquation * eqn, MT_CHAR * thechar)
{
    EQ_STRREC strs[4];
    int num_strs = 0;
    int math_attr;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sCHAR : atts=%d,typeface=%d,char=%c,%d\n",
                eqn->indent, (int)thechar->atts, (int)thechar->typeface,
                (char) thechar->character, (int)thechar->character);
        SetComment(strs, 2, buf);
        num_strs++;
    }
    if (DEBUG_CHAR || g_input_file_type==TYPE_EQN) 
        fprintf(stderr, "CHAR : atts=%d,typeface=%3d=%10s, char='%c' = 0x%04x = %d\n",
                (int)thechar->atts, (int)thechar->typeface, typeFaceName[thechar->typeface-128],
                (char) thechar->character, (unsigned int)thechar->character, (int)thechar->character);
/*
    SetComment(strs + num_strs, 100, (char *) NULL);
    num_strs++;
*/
    strcat(eqn->indent, "  ");

    num_strs += Eqn_GetTexChar(eqn, strs + num_strs, thechar, &math_attr);

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}


static
char *Eqn_TranslateFUNCTION(MTEquation * eqn, MT_OBJLIST * curr_node,
                            int *advance)
{
    char nom[16];
    char tex_func[128];
    EQ_STRREC strs[4];
    int num_strs = 0;
    uint32_t zlen;
    char *zdata;

    /*  step through object list to gather the function name */
    *advance = 0;
    nom[*advance] = '\0';
    while (curr_node && curr_node->tag == CHAR) {
        MT_CHAR *charptr = (MT_CHAR *) curr_node->obj_ptr;
        if (!charptr || charptr->typeface != 130 || !isalpha((int) charptr->character)) 
            break;
        
        nom[*advance] = (char) charptr->character;
        curr_node = (MT_OBJLIST *) curr_node->next;
        (*advance)++;
    }
    nom[*advance] = '\0';

    if (*advance == 0) return NULL;
    
    if (g_input_file_type==TYPE_EQN || DEBUG_FUNCTION) fprintf(stderr,"FUNC : name = '%s'\n", nom);
    
    zlen = GetProfileStr(Profile_FUNCTIONS, nom, tex_func, 128);

    /* no function translation found, just emit these characters */
    if (!zlen || !tex_func[0])
        snprintf(tex_func,128,"\\mathrm{%s}", nom);

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sFUNCTION : %s\n", eqn->indent, nom);
        SetComment(strs, 2, buf);
        num_strs++;
    }

/*  place_holder for $ if needed
    SetComment(strs + num_strs, 100, (char *) NULL);    
    num_strs++;
*/
    strs[num_strs].log_level = 0;
    strs[num_strs].do_delete = 1;
    strs[num_strs].ilk = Z_TEX;
    strs[num_strs].is_line = 0;
    zdata = (char *) malloc(strlen(tex_func) + 1);
    strcpy(zdata, tex_func);
    strs[num_strs].data = zdata;
    num_strs++;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}


static
char *Eqn_TranslateTEXTRUN(MTEquation * eqn, MT_OBJLIST * curr_node,
                           int *advance)
{
    /*  Gather the tex run */

    int di = 0;
    char run[256];
    EQ_STRREC strs[4];
    int num_strs = 0;
    char tbuff[256];
    uint32_t zlen;
    char *zdata;
    char *ndl;

    *advance = 0;

    while (curr_node && (curr_node->tag == CHAR || curr_node->tag == SIZE)) {
        if (curr_node->tag == CHAR) {
            MT_CHAR *charptr = (MT_CHAR *) curr_node->obj_ptr;
            if (charptr && charptr->typeface == 129) {
                run[di++] = (char) charptr->character;
                curr_node = (MT_OBJLIST *) curr_node->next;
                (*advance)++;
            } else
                break;
        } else {
            curr_node = (MT_OBJLIST *) curr_node->next;
            (*advance)++;
        }
    }
    run[di] = 0;

    if (eqn->log_level >= 2) {
        char buf[256];
        snprintf(buf, 256, "\n%sTEXTRUN : %s\n", eqn->indent, run);
        SetComment(strs, 2, buf);
        num_strs++;
    }

    strs[num_strs].log_level = 0;
    strs[num_strs].do_delete = 1;
    strs[num_strs].ilk = Z_TEX;
    strs[num_strs].is_line = 0;

    zlen = GetProfileStr(Profile_TEMPLATES, "TextInMath", tbuff, 256);
    if (zlen <= 0)
        strcpy(tbuff, "\\text{#1}");

    zdata = malloc(di + zlen);

    ndl = strchr(tbuff, '#');
    if (ndl) {
        *ndl = 0;
        strcpy(zdata, tbuff);
    } else
        *zdata = 0;
    strcat(zdata, run);
    if (ndl) {
        ndl += 2;
        strcat(zdata, ndl);
    }

    strs[num_strs].data = zdata;
    num_strs++;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}

#if 0
/*  Line positions are stored in a 2-bit pieces in the bits
 *  array.  This extracts the right one and returns true if
 *  a dashed, dotted, or solid line is present.
 */
static
int HasHVLine(int line_num, unsigned char *bits)
{

    int rv = 0;
    int byte_num = line_num / 4;
    int shift = (line_num % 4) * 2;
    int mask = 0x03 << shift;

    if (byte_num < MATR_MAX)
        rv = (bits[byte_num] & mask) ? 1 : 0;

    return rv;
}
#endif

/*  The current implementation ignores the vertical and
 *  horizontal lines ... it is not clear if it makes any
 *  sense to use the tabular environment to include this
 *  feature
 */
static
char *Eqn_TranslateMATRIX(MTEquation * eqn, MT_MATRIX * matrix)
{
    size_t buf_limit = 8192;
    char *thetex, *cell;
    char *col_align = "c";
    uint8_t j,col,row;
    MT_OBJLIST *obj_list;

    thetex = malloc(buf_limit);
    *thetex = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sStart MATRIX\n", eqn->indent);
        strcat(thetex, buf);
    }

    strcat(eqn->indent, "  ");

    strcat(thetex, "\n\\begin{array}");

    /*  set the vertical alignment of the matrix */
    if (matrix->valign == 0)
        strcat(thetex, "[t]");
    else if (matrix->valign == 2)
        strcat(thetex, "[b]");
        
    /*  set alignment for all columns */
    if (matrix->h_just == MT_LEFT)
        col_align = "l";
    else if (matrix->h_just == MT_RIGHT)
        col_align = "r";

    strcat(thetex, "{");
    for (col=0; col<matrix->cols; col++)
        strcat(thetex, col_align);
    strcat(thetex, "}\n");

    obj_list = matrix->element_list;
    for (row = 0; row < matrix->rows; row++) {

        if (row > 0) 
            strcat(thetex, " \\\\\n");
        
        for (col=0; col<matrix->cols; col++) {
        
            if (col>0) strcat(thetex, " & ");

            while (obj_list && obj_list->tag != LINE) 
                obj_list = (MT_OBJLIST *) obj_list->next;

            /* no column element found ... finish the line and exit */
            if (!obj_list || obj_list->tag != LINE) {
                for (j=col; j<matrix->cols; j++)
                    strcat(thetex, " & ");
                break;
            }
                
            cell = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);
            strcat(thetex, cell);
            free(cell);

            obj_list = (MT_OBJLIST *) obj_list->next;
        }
    }                           /*  loop down rows */

    strcat(thetex, "\n\\end{array}\n");

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sEnd MATRIX\n", eqn->indent);
        strcat(thetex, buf);
    }
    return thetex;
}

static
int Is_RelOp(MT_CHAR * charptr)
{

    int rv = 0;

    if (charptr->typeface == 134) {

        if (charptr->character == 60 || charptr->character == 62
            || charptr->character == 163 || charptr->character == 179
            || charptr->character == 185 || charptr->character == 186
            || charptr->character == 187 || charptr->character == 64
            || charptr->character == 61 || charptr->character == 181)
            rv = 1;

    } else if (charptr->typeface == 139) {
        if (charptr->character == 112 || charptr->character == 102
            || charptr->character == 60 || charptr->character == 62)
            rv = 1;
    }

    return rv;
}


static
char *Eqn_TranslateEQNARRAY(MTEquation * eqn, MT_PILE * pile)
{
	char *rv;
    uint32_t buf_limit = 8192;
    MT_OBJLIST *obj_list;
    int curr_row = 0;
    int right_only = 0;
    char *data;
    uint32_t b_off;

    rv = malloc(buf_limit);
    *rv = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sStart EQNARRAY\n", eqn->indent);
        strcat(rv, buf);
    }

    strcat(eqn->indent, "  ");

	setMathMode(eqn, EQN_MODE_EQNARRAY);

    obj_list = pile->line_list;

    while (obj_list) {          /*   loop down lines */

        if (curr_row)
            strcat(rv, " \\\\\n");

        while (obj_list && obj_list->tag != LINE) {     /*  could be a SIZE */
            obj_list = (MT_OBJLIST *) obj_list->next;
        }

        if (obj_list && obj_list->tag == LINE) {

/*  locate the "relop" in the object_list of the current line */

            MT_OBJLIST *left_node = (MT_OBJLIST *) NULL;
            MT_OBJLIST *right_node = (MT_OBJLIST *) NULL;

            MT_LINE *line = (MT_LINE *) obj_list->obj_ptr;
            MT_OBJLIST *curr_node = line->object_list;
            while (curr_node) {
                if (curr_node->tag == CHAR) {   /*  check for a relop */
                    if (Is_RelOp((MT_CHAR *) curr_node->obj_ptr))
                        break;
                }
                left_node = curr_node;
                curr_node = (MT_OBJLIST *) curr_node->next;
            }

/*  handle left side */

            if (left_node) {
                if (curr_node) {
                    left_node->next = NULL;
                    data = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);

                    b_off = (uint32_t) strlen(rv);
                    rv = ToBuffer(data, rv, &b_off, &buf_limit);        /*  strcat( rv,data ); */

                    left_node->next = (void *) curr_node;
                } else {
                    right_node = left_node;
                    right_only = 1;
                }
            }
            strcat(rv, " & ");

/*  handle the relop */

            if (curr_node) {
                char *data2 = Eqn_TranslateCHAR(eqn, (MT_CHAR *) curr_node->obj_ptr);
                strcat(rv, data2);
                free(data2);
                right_node = (MT_OBJLIST *) curr_node->next;
            }
            strcat(rv, " & ");

/*  handle right side */

            if (right_node) {
                char *data2;
                if (right_only)
                    data2 = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);
                else
                    data2 = Eqn_TranslateObjects(eqn, right_node);

                b_off = (uint32_t) strlen(rv);
                rv = ToBuffer(data2, rv, &b_off, &buf_limit);    /*  strcat( rv,data ); */
            }

            obj_list = (MT_OBJLIST *) obj_list->next;
        } else
            break;

        curr_row++;
    }                           /*  loop down thru lines */

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "%sEnd EQNARRAY\n", eqn->indent);
        strcat(rv, buf);
    }

    return rv;
}


static
char *Eqn_TranslateTABULAR(MTEquation * eqn, MT_PILE * pile)
{
    MT_OBJLIST *obj_list;
    size_t buf_limit = 8192;
    uint8_t row;
    uint32_t head_len;
    char *thetex, *line;
    
    thetex = (char *) malloc(buf_limit);

    /* fprintf(stderr, "PILE : Translating Tabular PILE\n"); */
    *thetex = '\0';

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sStart TABULAR\n", eqn->indent);
        strcat(thetex, buf);
    }

    strcat(eqn->indent, "  ");

    strcat(thetex, "\n\\begin{array}");

    switch (pile->valign) {
        case PVA_TOP:
            strcat(thetex, "[t]");
            break;
        case PVA_BOTTOM:
            strcat(thetex, "[b]");
            break;
        case PVA_CENTER:
        case PVA_CENTERING:
        case PVA_MATH:
            break;
    }

    switch (pile->halign) {
        case MT_PILE_LEFT:
            strcat(thetex, "{l}\n");
            break;
        case MT_PILE_RIGHT:
            strcat(thetex, "{r}\n");
            break;
        case MT_PILE_CENTER:
        case MT_PILE_OPERATOR:
        case MT_PILE_DECIMAL:
            strcat(thetex, "{r}\n");
            break;
    }

    head_len = (uint32_t) strlen(thetex);

    row = 1;
    for (obj_list = pile->line_list; obj_list != NULL; obj_list = (MT_OBJLIST *)obj_list->next) {

        if (obj_list->tag != LINE) 
            continue;
        
        line = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);
        
        if (strlen(line)>0) {
            if (row > 1) strcat(thetex, " \\\\\n");  /*  end previous row */
            strcat(thetex, line);
            row++;
        }
        free(line);
    }

    if (row==2 && strlen(thetex) > head_len) 
        strcat(thetex, "\n");
    strcat(thetex, "\\end{array}\n");

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sEnd TABULAR\n", eqn->indent);
        strcat(thetex, buf);
    }

    return thetex;
}


static
char *Eqn_TranslatePILEtoTARGET(MTEquation * eqn, MT_PILE * pile, char *targ_nom)
{
    MT_OBJLIST *obj_list;
    char ini_line[256];
    uint32_t dlen = 0;
    /*int forces_math = 1;*/
    /*int forces_text = 0;*/
    char *head = "";
    char *line_sep = " \\\\ ";
    char *tail = "";
    uint32_t buf_limit = 8192;
    int curr_row = 0;

    char *rv = (char *) malloc(buf_limit);
    *rv = 0;

    if (targ_nom && *targ_nom)
        dlen = GetProfileStr(Profile_PILEtranslation, targ_nom, ini_line, 256);

    /*   ini_line  =  "TextOnly,\begin{env}, \\,\end{env}" */

    if (dlen) {
        char *rover = ini_line;
        if (*rover == 'T') {
           /* forces_math = 0; */
           /* forces_text = 1; */
        }
        rover = strchr(rover, ',');     /*  end math/text force flag */
        if (rover && *(rover + 1)) {
            rover++;            /*  start of head */
            if (*rover != ',')
                head = rover;
            rover = strchr(rover, ','); /*  end of head */
            if (rover) {
                *rover = 0;
                rover++;        /*  start of line_sep */
                if (*rover && *(rover + 1)) {
                    if (*rover != ',')
                        line_sep = rover;
                    rover = strchr(rover, ','); /*  end of line_sep */
                    if (rover) {
                        *rover = 0;
                        rover++;        /*  start of tail */
                        if (*rover)
                            if (*rover != ',')
                                tail = rover;
                    }
                }
            }
        }
    }

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sStart PILE in TMPL field\n", eqn->indent);
        strcat(rv, buf);
    }

    strcat(eqn->indent, "  ");

    strcat(rv, "\n");
    strcat(rv, head);
    strcat(rv, "\n");

    obj_list = pile->line_list;

    while (obj_list) {          /*   loop down thru lines */

        if (curr_row) {
            strcat(rv, line_sep);
            strcat(rv, "\n");
        }

        while (obj_list && obj_list->tag != LINE) {     /*  could be a SIZE */
            obj_list = (MT_OBJLIST *) obj_list->next;
        }

        if (obj_list && obj_list->tag == LINE) {
            char *data = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);

            uint32_t b_off = (uint32_t) strlen(rv);
            rv = ToBuffer(data, rv, &b_off, &buf_limit);

            obj_list = (MT_OBJLIST *) obj_list->next;
        } else
            break;

        curr_row++;
    }                           /*  loop down thru lines */

    /*  put "\n\\end{array*}\n" */

    strcat(rv, "\n");
    strcat(rv, tail);
    strcat(rv, "\n");

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sEnd PILE in TMPL field\n", eqn->indent);
        strcat(rv, buf);
    }

    return rv;
}


static
char *Eqn_TranslateSIZE(MTEquation * eqn, MT_SIZE * size)
{

    EQ_STRREC strs[2];

    int num_strs = 0;
    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sSIZE\n", eqn->indent);
        SetComment(strs, 2, buf);
        num_strs++;
    }
    if (DEBUG_SIZE) fprintf(stderr, "\n%sSIZE type=%d, lsize=%d, dsize=%d\n", eqn->indent, size->type, size->lsize, size->dsize);

    strcat(eqn->indent, "  ");

    /*  not translation implemented yet */

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}


static
char *Eqn_TranslateFONT(MTEquation * eqn, MT_FONT * font)
{

    EQ_STRREC strs[2];
    int num_strs = 0;

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128,"\n%sFONT\n", eqn->indent);
        SetComment(strs, 2, buf);
        num_strs++;
    }

    strcat(eqn->indent, "  ");

    /*  no translation implemented yet */

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}

static
void GetPileType(char *the_template, int arg_num, char *targ_nom)
{
    int di = 0;
    char *ptr;
    char tok[4];
    snprintf(tok, 4, "#%d", arg_num);       /* #2 */

    ptr = strstr(the_template, tok);
    if (ptr && *(ptr + 2) == '[') {
        ptr += 3;
        while (*ptr != ']' && di < 32)
            targ_nom[di++] = *ptr++;
    }

    targ_nom[di] = 0;
}


static
int Eqn_GetTmplStr(MTEquation * eqn, uint8_t selector, uint16_t variation, EQ_STRREC * strs)
{
    char key[16];                /*  key = "20.1" */
    char ini_line[256];
    char *tmpl_ptr;
    int result;
    int num_strs = 0;           /*  this becomes the return value */

    snprintf(key, 16, "%d.%d", (int)selector, (int)variation); 

    if (eqn->m_mtef_ver==5)
        result = GetProfileStr(Profile_TEMPLATES_5, key, ini_line, 256);
    else
        result = GetProfileStr(Profile_TEMPLATES, key, ini_line, 256);

	if (!result) {
	    fprintf(stderr, "TMPL key='%s' not found in version %d Profile_TEMPLATE\n",key,eqn->m_mtef_ver);
        exit(1);
	}
	
//	fprintf(stderr,"ini_line='%s'\n", ini_line);
    tmpl_ptr = strchr(ini_line, ',');

    if (eqn->log_level >= 2) {
        char buf[512];
        snprintf(buf, 512, "\n%sTMPL : %s=!%s!\n", eqn->indent, key, ini_line);
        SetComment(strs, 2, buf);
        num_strs++;
    }
    if (0) fprintf(stderr, "\n%sTMPL : %s=!%s!\n", eqn->indent, key, ini_line);

    if (tmpl_ptr)
        *tmpl_ptr++ = 0;

    if (tmpl_ptr && *tmpl_ptr) {
        char *ztmpl;
        strs[num_strs].log_level = 0;
        strs[num_strs].do_delete = 1;
        strs[num_strs].ilk = Z_TMPL;
        strs[num_strs].is_line = 0;
        ztmpl = (char *) malloc(strlen(tmpl_ptr) + 1);
        strcpy(ztmpl, tmpl_ptr);
        strs[num_strs].data = ztmpl;
        num_strs++;
    }

    return num_strs;
}


static
char *Eqn_TranslateTMPL(MTEquation * eqn, MT_TMPL * tmpl)
{
    EQ_STRREC strs[10];
    char *the_template;
    int tally = 1;
    int num_strs;
    MT_OBJLIST *obj_list;

    if (eqn->m_mode == EQN_MODE_TEXT) 
        setMathMode(eqn, eqn->m_inline ? EQN_MODE_INLINE : EQN_MODE_DISPLAY); 

    if (eqn->m_mtef_ver == 5 && (tmpl->selector != 9))
        tmpl->variation &= 0x000f;

    if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN)
        fprintf(stderr, "TMPL : selector = %d, variation=0x%04x (%d.%d)\n", 
               (int) tmpl->selector, (unsigned int) tmpl->variation, (int) tmpl->selector, (int) tmpl->variation);

    num_strs = Eqn_GetTmplStr(eqn, tmpl->selector, tmpl->variation, strs);

    the_template = strs[num_strs - 1].data;
    
    if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN) 	
    	fprintf(stderr,"TMPL : num_strs=%d, strs[%d].data='%s'\n",num_strs,num_strs-1,the_template);

    strcat(eqn->indent, "  ");

    obj_list = tmpl->subobject_list;

    if (obj_list && obj_list->tag == CHAR) {

/*    translateCHAR( (MT_CHAR*)obj_list->obj_ptr ); */

        obj_list = (MT_OBJLIST *) obj_list->next;
    }

    if (obj_list && obj_list->tag >= SIZE && obj_list->tag <= SUBSYM) {

        obj_list = (MT_OBJLIST *) obj_list->next;
    }

    while (obj_list) {
        if (obj_list->tag == LINE) {
            if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN) fprintf(stderr,"TMPL : LINE object\n");
            strs[num_strs].log_level = 0;
            strs[num_strs].do_delete = 1;
            strs[num_strs].ilk = Z_TEX;
            strs[num_strs].is_line = 1;
            strs[num_strs].data = Eqn_TranslateLINE(eqn, (MT_LINE *) obj_list->obj_ptr);
            if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN) fprintf(stderr,"TMPL : strs[%d].data='%s'\n",num_strs,strs[num_strs].data);
            num_strs++;
            tally++;
        } else if (obj_list->tag == PILE) {     /*  This one is DIFFICULT!! */
            char targ_nom[32];
            if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN) fprintf(stderr,"TMPL : PILE object, targ_nom=%s\n",targ_nom);
            strs[num_strs].log_level = 0;
            strs[num_strs].do_delete = 1;
            strs[num_strs].ilk = Z_TEX;
            strs[num_strs].is_line = 2;

            GetPileType(the_template, tally, targ_nom);

            strs[num_strs].data = Eqn_TranslatePILEtoTARGET(eqn, (MT_PILE *) obj_list-> obj_ptr, targ_nom);
            if (DEBUG_TEMPLATE || g_input_file_type==TYPE_EQN) fprintf(stderr,"PILE : strs[%d].data='%s'\n",num_strs,strs[num_strs].data);

            num_strs++;
            tally++;
        }

        obj_list = (MT_OBJLIST *) obj_list->next;
    }

    /* There may be a SIZE at the end of the list */

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}


static
char *Eqn_TranslatePILE(MTEquation * eqn, MT_PILE * pile)
{
    EQ_STRREC strs[2];
    int num_strs = 0;

/*  fprintf(stderr, "PILE : Translating PILE\n"); */

    if (eqn->log_level >= 2) {
        char buf[128];
        snprintf(buf, 128, "\n%sPILE\n", eqn->indent);
        SetComment(strs, 2, buf);
        num_strs++;
    }

    strcat(eqn->indent, "  ");

    strs[num_strs].log_level = 0;
    strs[num_strs].do_delete = 1;
    strs[num_strs].ilk = Z_TEX;
    strs[num_strs].is_line = 0;

    setMathMode(eqn, EQN_MODE_DISPLAY); 

    if (pile->halign == MT_PILE_OPERATOR)
        strs[num_strs].data = Eqn_TranslateEQNARRAY(eqn, pile);
    else
        strs[num_strs].data = Eqn_TranslateTABULAR(eqn, pile);
    num_strs++;

    eqn->indent[strlen(eqn->indent) - 2] = 0;

    return Eqn_JoinStrings(eqn, strs, num_strs);
}

static
char *Eqn_TranslateObjects(MTEquation * eqn, MT_OBJLIST * the_list)
{

    char *zcurr;
    char *rv = (char *) malloc(1024);
    uint32_t di = 0;
    uint32_t lim = 1024;

    MT_OBJLIST *curr_node;
    *rv = 0;

    if (DEBUG_TRANSLATION || g_input_file_type==TYPE_EQN) fprintf(stderr,"new object list\n");
    while (the_list) {

        curr_node = the_list;
        the_list = (void *) the_list->next;

        zcurr = (char *) NULL;

        if (DEBUG_TRANSLATION || g_input_file_type==TYPE_EQN) print_tag(curr_node->tag, 0);
        
        switch (curr_node->tag) {

        case LINE:{
                MT_LINE *line = (MT_LINE *) curr_node->obj_ptr;
                if (line)
                    zcurr = Eqn_TranslateLINE(eqn, line);
            }
            break;

        case CHAR:{
                int advance = 0;
                MT_CHAR *charptr = (MT_CHAR *) curr_node->obj_ptr;
                if (!charptr) break;
            
                if (charptr->typeface == 130) {     /*  auto_recognize functions */
                    zcurr = Eqn_TranslateFUNCTION(eqn, curr_node, &advance);
                    while (advance > 1) {
                        the_list = (MT_OBJLIST *) the_list->next;
                        advance--;
                    }
                } else if (charptr->typeface == 129 && eqn->m_mode != EQN_MODE_TEXT) {    /*  text in math */
                    zcurr = Eqn_TranslateTEXTRUN(eqn, curr_node, &advance);
                    while (advance > 1) {
                        the_list = (MT_OBJLIST *) the_list->next;
                        advance--;
                    }
                }
                if (!advance)
                    zcurr = Eqn_TranslateCHAR(eqn, charptr);
            }
            break;

        case TMPL:{
                MT_TMPL *tmpl = (MT_TMPL *) curr_node->obj_ptr;
                if (tmpl)
                    zcurr = Eqn_TranslateTMPL(eqn, tmpl);
            }
            break;

        case PILE:{
                MT_PILE *pile = (MT_PILE *) curr_node->obj_ptr;
                if (pile)
                    zcurr = Eqn_TranslatePILE(eqn, pile);
            }
            break;

        case MATRIX:{
                MT_MATRIX *matrix = (MT_MATRIX *) curr_node->obj_ptr;
                if (matrix)
                    zcurr = Eqn_TranslateMATRIX(eqn, matrix);
            }
            break;

        case EMBELL:
            break;

        case RULER:{
                MT_RULER *ruler = (MT_RULER *) curr_node->obj_ptr;
                if (ruler)
                    zcurr = Eqn_TranslateRULER(eqn, ruler);
            }
            break;

        case FONT:{
                MT_FONT *font = (MT_FONT *) curr_node->obj_ptr;
                if (font)
                    zcurr = Eqn_TranslateFONT(eqn, font);
            }
            break;

        case SIZE:
        case FULL:
        case SUB:
        case SUB2:
        case SYM:
        case SUBSYM:{
                MT_SIZE *size = (MT_SIZE *) curr_node->obj_ptr;
                if (size)
                    zcurr = Eqn_TranslateSIZE(eqn, size);
            }
            break;

        default:
            break;
        }

        if (zcurr)
            rv = ToBuffer(zcurr, rv, &di, &lim);

    }                           /*   while ( the_list ) */

    return rv;
}

void Eqn_TranslateObjectList(MTEquation * eqn, FILE * outfile,
                             int loglevel)
{
    char *ztex;

    eqn->out_file = outfile;
    eqn->log_level = loglevel;

    if (eqn->log_level == 2)
        fputs("%Begin Equation\n", eqn->out_file);

    if (DEBUG_TRANSLATION || g_input_file_type==TYPE_EQN) fprintf(stderr,"new equation\n");
    
    ztex = Eqn_TranslateObjects(eqn, eqn->o_list);

	if (eqn->m_latex_start && ztex && *ztex) {
		eqn->m_latex = ztex;
		if (!eqn->m_latex_end) setMathMode(eqn, EQN_MODE_TEXT);
		return;
	} 
	
	if (eqn->m_latex_start) free(eqn->m_latex_start);
	eqn->m_latex_start = NULL;
	if (eqn->m_latex) free(eqn->m_latex);
	eqn->m_latex = NULL;
	if (eqn->m_latex_end) free(eqn->m_latex_end);
	eqn->m_latex_end = NULL;
}

/* [FUNCTIONS] */
char *Profile_FUNCTIONS[] = {
    "Pr=\\Pr ",
    "arccos=\\arccos ",
    "arcsin=\\arcsin ",
    "arctan=\\arctan ",
    "arg=\\arg ",
    "cos=\\cos ",
    "cosh=\\cosh ",
    "cot=\\cot ",
    "coth=\\coth ",
    "csc=\\csc ",
    "deg=\\deg ",
    "det=\\det ",
    "dim=\\dim ",
    "exp=\\exp ",
    "gcd=\\gcd ",
    "hom=\\hom ",
    "inf=\\inf ",
    "ker=\\ker ",
    "lim=\\lim ",
    "liminf=\\liminf ",
    "limsup=\\limsup ",
    "ln=\\ln ",
    "log=\\log ",
    "max=\\max ",
    "min=\\min ",
    "sec=\\sec ",
    "sin=\\sin ",
    "sinh=\\sinh ",
    "sup=\\sup ",
    "tan=\\tan ",
    "tanh=\\tanh ",
    "mod=\\mathop{\\rm mod} ",
    "glb=\\mathop{\\rm glb} ",
    "lub=\\mathop{\\rm lub} ",
    "int=\\mathop{\\rm int} ",
    "Im=\\mathop{\\rm Im} ",
    "Re=\\mathop{\\rm Re} ",
    "var=\\mathop{\\rm var} ",
    "cov=\\mathop{\\rm cov} ",
    0
};

/* [VARIABLES] */
/* VARIABLE_NAME,MATH_VERSION,TEXT_VERSION */
char *Profile_VARIABLES[] = {
    "STARTSUB=_{,\\Sb ",
    "ENDSUB=}, \\endSb ",
    "STARTSUP=^{,\\Sp ",
    "ENDSUP=}, \\endSp ",
    0
};

/* [PILEtranslation] */
char *Profile_PILEtranslation[] = {
    "MATHDEFAULT=MathForce,\\begin{array}{l}, \\\\,\\end{array}",
    "TEXTDEFAULT=TextOnly,\\begin{tabular}{l}, \\\\,\\end{tabular}",
    "L=,, \\ ,",
    "M=MathForce,\\begin{array}{l}, \\\\,\\end{array}",
    "X=TextForce,\\begin{texttest}, \\\\LINE_END,\\end{texttest}",
    "Y=MathForce,\\begin{mathtest}, \\\\LINE_END,\\end{mathtest}",
    0
};


/* typeface=attributes,do_lookup,use_codepoint */
/* attributes --> 0=MA_NONE, 1=MA_FORCE_MATH, 2=MA_FORCE_TEXT, 3=2 translations */
char *Profile_MT_CHARSET_ATTS[] = {
    "129=2,0,1", /* TEXT     */
    "130=1,1,1", /* FUNCTION */
    "131=1,0,1", /* VARIABLE */
    "132=1,1,0", /* LCGREEK  */
    "133=1,1,0", /* UCGREEK  */
    "134=1,1,1", /* SYMBOL   */
    "135=1,0,1", /* VECTOR   */
    "136=1,0,1", /* NUMBER   */
    "137=1,1,0",
    "138=1,1,0",
    "139=1,1,1", /* MTEXTRA */
    "140=1,1,0",
    "141=1,1,0",
    "142=1,1,0",
    "143=1,1,0",
    "144=1,1,0",
    "145=1,1,0",
    "146=1,1,0",
    "147=1,1,0",
    "148=1,1,0",
    "149=1,1,0", 
    "150=1,1,0", /* EXPAND */
    "151=1,1,0", /* MARKER */
    "152=3,1,0", /* SPACE  */
    "153=1,1,0",
    0
};

/* [CHARTABLE] */
char *Profile_CHARTABLE[] = {
    "130.91=\\lbrack ",
    "130.95=\\_ ",        /* needed to properly escape '_' */
    "131.95=\\_ ",
    "132.74=\\vartheta ", /* encoding for lowercase greek */
    "132.86=\\varsigma ",
    "132.97=\\alpha ",
    "132.98=\\beta ",
    "132.99=\\chi ",
    "132.100=\\delta ",
    "132.101=\\epsilon ",
    "132.102=\\phi ",
    "132.103=\\gamma ",
    "132.104=\\eta ",
    "132.105=\\iota ",
    "132.106=\\varphi ",
    "132.107=\\kappa ",
    "132.108=\\lambda ",
    "132.109=\\mu ",
    "132.110=\\nu ",
    "132.111=\\o ",
    "132.112=\\pi ",
    "132.113=\\theta ",
    "132.114=\\rho ",
    "132.115=\\sigma ",
    "132.116=\\tau ",
    "132.117=\\upsilon ",
    "132.118=\\varpi ",
    "132.119=\\omega ",
    "132.120=\\xi ",
    "132.121=\\psi ",
    "132.122=\\zeta ",
    "132.182=\\partial ",
    "132.945=\\alpha ",
    "132.946=\\beta ",
    "132.967=\\chi ",
    "132.948=\\delta ",
    "132.949=\\epsilon ",
    "132.966=\\phi ",
    "132.947=\\gamma ",
    "132.951=\\eta ",
    "132.953=\\iota ",
    "132.981=\\varphi ",
    "132.954=\\kappa ",
    "132.955=\\lambda ",
    "132.956=\\mu ",
    "132.957=\\nu ",
    "132.959=\\o ",
    "132.960=\\pi ",
    "132.952=\\theta ",
    "132.961=\\rho ",
    "132.963=\\sigma ",
    "132.964=\\tau ",
    "132.965=\\upsilon ",
    "132.969=\\omega ",
    "132.958=\\xi ",
    "132.968=\\psi ",
    "132.950=\\zeta ",
    "132.977=\\vartheta ",
    "132.962=\\varsigma ",
    "132.982=\\varpi ",
    "133.65=A",            /* encoding for upper case greek */
    "133.66=B",
    "133.67=X",
    "133.68=\\Delta ",
    "133.69=E",
    "133.70=\\Phi ",
    "133.71=\\Gamma ",
    "133.72=H",
    "133.73=I",
    "133.75=K",
    "133.76=\\Lambda ",
    "133.77=M",
    "133.78=N",
    "133.79=O",
    "133.80=\\Pi ",
    "133.81=\\Theta ",
    "133.82=P",
    "133.83=\\Sigma ",
    "133.84=T",
    "133.85=Y",
    "133.87=\\Omega ",
    "133.88=\\Xi ",
    "133.89=\\Psi ",
    "133.90=Z",
    "133.913=A",
    "133.914=B",
    "133.935=X",
    "133.916=\\Delta ",
    "133.917=E",
    "133.934=\\Phi ",
    "133.915=\\Gamma ",
    "133.919=H",
    "133.921=I",
    "133.922=K",
    "133.923=\\Lambda ",
    "133.924=M",
    "133.925=N",
    "133.927=O",
    "133.928=\\Pi ",
    "133.920=\\Theta ",
    "133.929=P",
    "133.931=\\Sigma ",
    "133.932=T",
    "133.933=Y",
    "133.937=\\Omega ",
    "133.926=\\Xi ",
    "133.936=\\Psi ",
    "133.918=Z",
    "134.34=\\forall ",    /* symbol font encoding */
    "134.36=\\exists ",
    "134.39=\\ni ",
    "134.42=*",
    "134.43=+",
    "134.45=-",
    "134.61==",
    "134.64=\\cong ",
    "134.92=\\therefore ",
    "134.94=\\bot ",
    "134.97=\\alpha ",
    "134.98=\\beta ",
    "134.99=\\chi ",
    "134.100=\\delta ",
    "134.101=\\epsilon ",
    "134.102=\\phi ",
    "134.103=\\gamma ",
    "134.104=\\eta ",
    "134.105=\\iota ",
    "134.106=\\varphi ",
    "134.107=\\kappa ",
    "134.108=\\lambda ",
    "134.109=\\mu ",
    "134.110=\\nu ",
    "134.112=\\pi ",
    "134.113=\\theta ",
    "134.114=\\rho ",
    "134.115=\\sigma ",
    "134.116=\\tau ",
    "134.117=\\upsilon ",
    "134.118=\\varpi ",
    "134.119=\\omega ",
    "134.120=\\xi ",
    "134.121=\\psi ",
    "134.122=\\zeta ",
    "134.163=\\leq ",
    "134.165=\\infty ",
    "134.171=\\leftrightarrow ",
    "134.172=\\leftarrow ",
    "134.173=\\uparrow ",
    "134.174=\\rightarrow ",
    "134.175=\\downarrow ",
    "134.176=^\\circ ",
    "134.177=\\pm ",
    "134.179=\\geq ",
    "134.180=\\times ",
    "134.181=\\propto ",
    "134.182=\\partial ",
    "134.183=\\bullet ",
    "134.184=\\div ",
    "134.185=\\neq ",
    "134.186=\\equiv ",
    "134.187=\\approx ",
    "134.191=\\hookleftarrow ",
    "134.192=\\aleph ",
    "134.193=\\Im ",
    "134.194=\\Re ",
    "134.195=\\wp ",
    "134.196=\\otimes ",
    "134.197=\\oplus ",
    "134.198=\\emptyset ",
    "134.199=\\cap ",
    "134.200=\\cup ",
    "134.201=\\supset ",
    "134.202=\\supseteq ",
    "134.203=\\nsubset ",
    "134.204=\\subset ",
    "134.205=\\subseteq ",
    "134.206=\\in ",
    "134.207=\\notin ",
    "134.208=\\angle ",
    "134.209=\\nabla ",
    "134.213=\\prod ",
    "134.215=\\cdot ",
/*    "134.215=\\times ", */
    "134.216=\\neg ",
    "134.217=\\wedge ",
    "134.218=\\vee ",
    "134.219=\\Leftrightarrow ",
    "134.220=\\Leftarrow ",
    "134.221=\\Uparrow ",
    "134.222=\\Rightarrow ",
    "134.223=\\Downarrow ",
    "134.224=\\Diamond ",
    "134.225=\\langle ",
    "134.229=\\Sigma ",
    "134.241=\\rangle ",
    "134.242=\\smallint ",
    "134.247=\\div ",
    "134.8722=-",
    "134.8804=\\leq ",
    "134.8805=\\geq ",
    "134.8800=\\neq ",
    "134.8801=\\equiv ",
    "134.8776=\\approx ",
    "134.8773=\\cong ",
    "134.8733=\\propto ",
    "134.8727=\\ast ",
    "134.8901=\\cdot ",
    "134.8226=\\bullet ",
    "134.8855=\\otimes ",
    "134.8853=\\oplus ",
    "134.9001=\\langle ",
    "134.9002=\\rangle ",
    "134.8594=\\rightarrow ",
    "134.8592=\\leftarrow ",
    "134.8596=\\leftrightarrow ",
    "134.8593=\\uparrow ",
    "134.8595=\\downarrow ",
    "134.8658=\\Rightarrow ",
    "134.8656=\\Leftarrow ",
    "134.8660=\\Leftrightarrow ",
    "134.8657=\\Uparrow ",
    "134.8659=\\Downarrow ",
    "134.8629=\\hookleftarrow ",
    "134.8756=\\therefore ",
    "134.8717=\\backepsilon ",
    "134.8707=\\exists ",
    "134.8704=\\forall ",
    "134.172=\\neg ",
    "134.8743=\\wedge ",
    "134.8744=\\vee ",
    "134.8712=\\in ",
    "134.8713=\\notin ",
    "134.8746=\\cup ",
    "134.8745=\\cap ",
    "134.8834=\\subset ",
    "134.8835=\\supset ",
    "134.8838=\\subseteq ",
    "134.8839=\\supseteq ",
    "134.8836=\\not\\subset ",
    "134.8709=\\emptyset ",
    "134.8706=\\partial ",
    "134.8711=\\nabla ",
    "134.8465=\\Im ",
    "134.8476=\\Re ",
    "134.8501=\\aleph ",
    "134.8736=\\angle ",
    "134.8869=\\bot ",
    "134.8900=\\lozenge ",
    "134.8734=\\infty ",
    "134.8472=\\wp ",
    "134.8747=\\smallint",
    "134.8721=\\sum ",
    "134.8719=\\prod ",
    "139.58=\\sim ",              /* MT Extra encoding */
    "139.59=\\simeq ",
    "139.60=\\vartriangleleft ",
    "139.61=\\ll ",
    "139.62=\\vartriangleright ",
    "139.63=\\gg ",
    "139.66=\\doteq ",
    "139.67=\\coprod ",
    "139.68=\\lambdabar ",
    "139.73=\\bigcap ",
    "139.75=\\ldots ",
    "139.76=\\cdots ",
    "139.77=\\vdots ",
    "139.78=\\ddots ",
    "139.79=\\ddots ",
    "139.81=\\because ",
    "139.85=\\bigcup ",
    "139.97=\\mapsto ",
    "139.98=\\updownarrow ",
    "139.99=\\Updownarrow ",
    "139.102=\\succ ",
    "139.104=\\hbar ",
    "139.108=\\ell ",
    "139.109=\\mp ",
    "139.111=\\circ ",
    "139.112=\\prec ",
    "139.8230=\\ldots ",
    "139.8943=\\cdots ",
    "139.8942=\\vdots ",
    "139.8944=\\ddots ",
    "139.8945=\\ddots ",
    "139.8826=\\prec ",
    "139.8827=\\succ ",
    "139.8882=\\vartriangleleft ",
    "139.8883=\\vartriangleright ",
    "139.8723=\\mp ",
    "139.8728=\\circ ",
    "139.8614=\\longmapsto ",
    "139.8597=\\updownarrow ",
    "139.8661=\\Updownarrow ",
    "139.4746=\\bigcup ",
    "139.4745=\\bigcap ",
    "139.8757=\\because ",
    "139.8467=\\ell ",
    "139.8463=\\hbar ",
    "139.411=\\lambdabar ",
    "139.8720=\\coprod ",
    "151.60160m={}",
    "151.60160t={}",
    "152.1m={}",
    "152.1t={}",
    "152.8m=\\/",
    "152.8t=\\/",
    "152.2m=\\,",
    "152.2t=\\thinspace ",
    "152.4m=\\;",
    "152.4t=\\ ",
    "152.5m=\\quad ",
    "152.5t=\\quad ",
    "152.60161m=\\/",
    "152.60161t=\\/",
    "152.61168m=@,",
    "152.61168t=",
    "152.60162m=\\,",
    "152.60162t=\\thinspace ",
    "152.60164m=\\;",
    "152.60164t=\\ ",
    "152.60165m=\\quad ",
    "152.60165t=\\quad ",
    "152.61186m=\\, ",
    "152.61186t=\\thinspace ",
    0
};

/* [TEMPLATES] */
char *Profile_TEMPLATES[] = {
    "0.0=fence: angle-both,\\left\\langle #1[M]\\right\\rangle ",
    "0.1=fence: angle-left only,\\left\\langle #1[M]\\right. ",
    "0.2=fence: angle-right only,\\left. #1[M]\\right\\rangle ",
    "1.0=fence: paren-both,\\left( #1[M]\\right) ",
    "1.1=fence: paren-left only,\\left( #1[M]\\right. ",
    "1.2=fence: paren-right only,\\left. #1[M]\\right) ",
    "2.0=fence: brace-both,\\left\\{ #1[M]\\right\\} ",
    "2.1=fence: brace-left only,\\left\\{ #1[M]\\right. ",
    "2.2=fence: brace-right only,\\left. #1[M]\\right\\} ",
    "3.0=fence: brack-both,\\left[ #1[M]\\right] ",
    "3.1=fence: brack-left only,\\lef]t[ #1[M]\\right. ",
    "3.2=fence: brack-right only,\\left. #1[M]\\right] ",
    "4.0=fence: bar-both,\\left| #1[M]\\right| ",
    "4.1=fence: bar-left only,\\left| #1[M]\\right. ",
    "4.2=fence: bar-right only,\\left. #1[M]\\right| ",
    "5.0=fence: dbar-both,\\left\\| #1[M]\\right\\| ",
    "5.1=fence: dbar-left only,\\left\\| #1[M]\\right. ",
    "5.2=fence: dbar-right only,\\left. #1[M]\\right\\| ",
    "6.0=fence: floor,\\left\\lfloor #1[M]\\right\\rfloor ",
    "7.0=fence: ceiling,\\left\\lceil #1[M]\\right\\rceil ",
    "8.0=fence: LBLB,\\left[ #1[M]\\right[ ",
    "9.0=fence: RBRB,\\left] #1[M]\\right] ",
    "10.0=fence: RBLB,\\left] #1[M]\\right[ ",
    "11.0=fence: LBRP,\\left[ #1[M]\\right) ",
    "12.0=fence: LPRB,\\left( #1[M]\\right] ",
    "13.0=root: sqroot,\\sqrt{#1[M]} ",
    "13.1=root: nthroot,\\sqrt[#2[M]]{#1[M]} ",
    "14.0=fract: ffract,\\frac{#1[M]}{#2[M]} ",
    "14.1=fract: pfract,\\frac{#1[M]}{#2[M]} ",
    "15.0=script: super,#1[L][STARTSUP][ENDSUP] ",
    "15.1=script: sub,#1[L][STARTSUB][ENDSUB] ",
    "15.2=script: subsup,#1[L][STARTSUB][ENDSUB]#2[L][STARTSUP][ENDSUP] ",
    "16.0=ubar: subar,\\underline{#1[M]} ",
    "16.1=ubar: dubar,\\underline{\\underline{#1[M]}} ",
    "17.0=obar: sobar,\\overline{#1[M]} ",
    "17.1=obar: dobar,\\overline{\\overline{#1[M]}} ",
    "18.0=larrow: box on top,\\stackrel{#1[M]}{\\longleftarrow} ",
    "18.1=larrow: box below ,\\stackunder{#1[M]}{\\longleftarrow} ",
    "19.0=rarrow: box on top,\\stackrel{#1[M]}{\\longrightarrow} ",
    "19.1=rarrow: box below ,\\stackunder{#1[M]}{\\longrightarrow} ",
    "20.0=barrow: box on top,\\stackrel{#1[M]}{\\longleftrightarrow} ",
    "20.1=barrow: box below ,\\stackunder{#1[M]}{\\longleftrightarrow} ",
    "21.0=integrals: single - no limits,\\int #1[M] ",
    "21.1=integrals: single - lower only,\\int\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "21.2=integrals: single - both,\\int\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "21.3=integrals: contour - no limits,\\oint #1[M] ",
    "21.4=integrals: contour - lower only,\\oint\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "22.0=integrals: double - no limits ,\\iint #1[M] ",
    "22.1=integrals: double - lower only,\\iint\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "22.2=integrals: area - no limits ,\\iint #1[M] ",
    "22.3=integrals: area - lower only,\\iint\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "23.0=integrals: triple - no limits ,\\iiint #1[M] ",
    "23.1=integrals: triple - lower only,\\iiint\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "23.2=integrals: volume - no limits ,\\iiint #1[M] ",
    "23.3=integrals: volume - lower only,\\iiint\\nolimits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "24.0=integrals: single - sum style - both,\\int\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "24.1=integrals: single - sum style - lower only,\\int\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "24.2=integrals: contour - sum style - lower only,\\oint\\limits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "25.0=integrals: area - sum style - lower only,\\iint\\limits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "25.1=integrals: double - sum style - lower only,\\iint\\limits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "26.0=integrals: volume - sum style - lower only,\\iiint\\limits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "26.1=integrals: triple - sum style - lower only,\\iiint\\limits#2[L][STARTSUB][ENDSUB] #1[M] ",
    "27.0=horizontal braces: upper,\\stackrel{#2[M]}{\\overbrace{#1[M]}} ",
    "28.0=horizontal braces: lower,\\stackunder{#2[M]}{\\underbrace{#1[M]}} ",
    "29.0=sum: limits top/bottom - lower only,\\sum\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "29.1=sum: limits top/bottom - both,\\sum\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "29.2=sum: no limits,\\sum #1[M] ",
    "30.0=sum: limits right - lower only,\\sum\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "30.1=sum: limits right - both,\\sum\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "31.0=product: limits top/bottom - lower only,\\dprod\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "31.1=product: limits top/bottom - both,\\dprod\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "31.2=product: no limits,\\dprod #1[M] ",
    "32.0=product: limits right - lower only,\\dprod\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "32.1=product: limits right - both,\\dprod\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "33.0=coproduct: limits top/bottom - lower only,\\dcoprod\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "33.1=coproduct: limits top/bottom - both,\\dcoprod\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "33.2=coproduct: no limits,\\dcoprod #1[M] ",
    "34.0=coproduct: limits right - lower only,\\dcoprod\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "34.1=coproduct: limits right - both,\\dcoprod\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "35.0=union: limits top/bottom - lower only,\\dbigcup\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "35.1=union: limits top/bottom - both,\\dbigcup\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "35.2=union: no limits,\\dbigcup #1[M] ",
    "36.0=union: limits right - lower only,\\dbigcup\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "36.1=union: limits right - both,\\dbigcup\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "37.0=intersection: limits top/bottom - lower only,\\dbigcap\\limits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "37.1=intersection: limits top/bottom - both,\\dbigcap\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "37.2=intersection: no limits,\\dbigcap #1[M] ",
    "38.0=intersection: limits right - lower only,\\dbigcap\\nolimits#2[L][STARTSUB][ENDSUB]#1[M] ",
    "38.1=intersection: limits right - both,\\dbigcap\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "39.0=limit: upper,#1 #2[L][STARTSUP][ENDSUP] ",
    "39.1=limit: lower,#1 #2[L][STARTSUB][ENDSUB] ",
    "39.2=limit: both,#1 #2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP] ",
    "40.0=long divisionW,",
    "40.1=long divisionWO,",
    "41.0=slash fraction: normal,\\frac{#1[M]}{#2[M]} ",
    "41.1=slash fraction: baseline,#1[M]/#2[M] ",
    "41.2=slash fraction: subscript-sized,\\frac{#1[M]}{#2[M]} ",
    "42.0=INTOP: upper,",
    "42.1=INTOP: lower,",
    "42.2=INTOP: both,",
    "43.0=SUMOP: upper,",
    "43.1=SUMOP: lower,",
    "43.2=SUMOP: both,",
    "44.0=leadingSUPER,",
    "44.1=leadingSUB,",
    "44.2=leadingSUBSUP,",
    "45.0=Dirac: both,\\left\\langle #1[M]\\right.\\left| #2[M]\\right\\rangle ",
    "45.1=Dirac: left,\\left\\langle #1[M]\\right| ",
    "45.2=Dirac: right,\\left| #1[M]\\right\\rangle ",
    "46.0=under arrow: left,\\underleftarrow{#1[M]} ",
    "46.1=under arrow: right,\\underrightarrow{#1[M]} ",
    "46.2=under arrow: both,\\underleftrightarrow{#1[M]} ",
    "47.0=over arrow: left,\\overleftarrow{#1[M]} ",
    "47.1=over arrow: right,\\overrightarrow{#1[M]} ",
    "47.2=over arrow: both,\\overleftrightarrow{#1[M]} ",
    "TextInMath=\\text{#1} ",
    0
};

/* [TEMPLATES] */
char *Profile_TEMPLATES_5[] = {
    "0.1=fence: angle-left only,\\left\\langle #1[M]\\right. ",
    "0.2=fence: angle-right only,\\left. #1[M]\\right\\rangle ",
    "0.3=fence: angle-both,\\left\\langle #1[M]\\right\\rangle ",
    "1.1=fence: paren-left only,\\left( #1[M]\\right. ",
    "1.2=fence: paren-right only,\\left. #1[M]\\right) ",
    "1.3=fence: paren-both,\\left( #1[M]\\right) ",
    "2.1=fence: brace-left only,\\left\\{ #1[M]\\right. ",
    "2.2=fence: brace-right only,\\left. #1[M]\\right\\} ",
    "2.3=fence: brace-both,\\left\\{ #1[M]\\right\\} ",
    "3.1=fence: brack-left only,\\lef]t[ #1[M]\\right. ",
    "3.2=fence: brack-right only,\\left. #1[M]\\right] ",
    "3.3=fence: brack-both,\\left[ #1[M]\\right] ",
    "4.1=fence: bar-left only,\\left| #1[M]\\right. ",
    "4.2=fence: bar-right only,\\left. #1[M]\\right| ",
    "4.3=fence: bar-both,\\left| #1[M]\\right| ",
    "5.1=fence: dbar-left only,\\left\\| #1[M]\\right. ",
    "5.2=fence: dbar-right only,\\left. #1[M]\\right\\| ",
    "5.3=fence: dbar-both,\\left\\| #1[M]\\right\\| ",
    "6.1=fence: floor,\\left\\lfloor #1[M]\\right. ",
    "6.2=fence: floor,\\left. #1[M]\\right\\rfloor ",
    "6.3=fence: floor,\\left\\lfloor #1[M]\\right\\rfloor ",
    "7.1=fence: ceiling,\\left\\lceil #1[M]\\right. ",
    "7.2=fence: ceiling,\\left. #1[M]\\right\\rceil ",
    "7.3=fence: ceiling,\\left\\lceil #1[M]\\right\\rceil ",
    "8.0=fence: LBLB,\\left[ #1[M]\\right[ ",
    "9.0=fence: LPLP,\\left( #1[M]\\right( ",
    "9.1=fence: RPLP,\\left) #1[M]\\right( ",
    "9.2=fence: LBLP,\\left[ #1[M]\\right( ",
    "9.3=fence: RBLP,\\left] #1[M]\\right( ",
    "9.16=fence: LPRP,\\left( #1[M]\\right) ",
    "9.17=fence: RPRP,\\left) #1[M]\\right) ",
    "9.18=fence: LBRP,\\left[ #1[M]\\right) ",
    "9.19=fence: RBRP,\\left] #1[M]\\right) ",
    "9.32=fence: LPLB,\\left( #1[M]\\right[ ",
    "9.33=fence: RPLB,\\left) #1[M]\\right[ ",
    "9.34=fence: LBLB,\\left[ #1[M]\\right[ ",
    "9.35=fence: RBLB,\\left] #1[M]\\right[ ",
    "9.48=fence: LPRB,\\left( #1[M]\\right] ",
    "9.49=fence: RPRB,\\left) #1[M]\\right] ",
    "9.50=fence: LBRB,\\left[ #1[M]\\right] ",
    "9.51=fence: RBRB,\\left] #1[M]\\right] ",
    "10.0=root: sqroot,\\sqrt{#1[M]} ",
    "10.1=root: nthroot,\\sqrt[#2[M]]{#1[M]} ",
    "11.0=fract: tmfract,\\frac{#1[M]}{#2[M]} ",
    "11.1=fract: smfract,\\frac{#1[M]}{#2[M]} ",
    "11.2=fract: slfract,{#1[M]}/{#2[M]} ",
    "11.3=fract: slfract,{#1[M]}/{#2[M]} ",
    "11.4=fract: slfract,{#1[M]}/{#2[M]} ",
    "11.5=fract: smfract,\\frac{#1[M]}{#2[M]} ",
    "11.6=fract: slfract,{#1[M]}/{#2[M]} ",
    "11.7=fract: slfract,{#1[M]}/{#2[M]} ",
    "12.0=ubar: subar,\\underline{#1[M]} ",
    "12.1=ubar: dubar,\\underline{\\underline{#1[M]}} ",
    "13.0=obar: sobar,\\overline{#1[M]} ",
    "13.1=obar: dobar,\\overline{\\overline{#1[M]}} ",
    "14.0=larrow: box on top,\\stackrel{#1[M]}{\\longleftarrow} ",
    "14.1=larrow: box below ,\\stackunder{#1[M]}{\\longleftarrow} ",
    "14.0=rarrow: box on top,\\stackrel{#1[M]}{\\longrightarrow} ",
    "14.1=rarrow: box below ,\\stackunder{#1[M]}{\\longrightarrow} ",
    "14.0=barrow: box on top,\\stackrel{#1[M]}{\\longleftrightarrow} ",
    "14.1=barrow: box below ,\\stackunder{#1[M]}{\\longleftrightarrow} ",
    "15.0=integrals: single - no limits,\\int #1[M] ",
    "15.1=integrals: single - both,\\int\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "15.2=integrals: double - both,\\iint\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "15.3=integrals: triple - both,\\iiint\\nolimits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "15.4=integrals: contour - no limits,\\oint #1[M] ",
    "15.8=integrals: contour - no limits,\\oint #1[M] ",
    "15.12=integrals: contour - no limits,\\oint #1[M] ",
    "16.0=sum: limits top/bottom - both,\\sum\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "17.0=product: limits top/bottom - both,\\dprod\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "18.0=coproduct: limits top/bottom - both,\\dcoprod\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "19.0=union: limits top/bottom - both,\\dbigcup\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "20.0=intersection: limits top/bottom - both,\\dbigcap\\limits#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "21.0=integrals: single - both,\\int#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",
    "22.0=sum: single - both,\\sum#2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP]#1[M] ",    
    "23.0=limit: both,#1 #2[L][STARTSUB][ENDSUB]#3[L][STARTSUP][ENDSUP] ",
    "24.0=horizontal brace: lower,\\stackunder{#2[M]}{\\underbrace{#1[M]}} ",
    "24.1=horizontal brace: upper,\\stackrel{#2[M]}{\\overbrace{#1[M]}} ",
    "25.0=horizontal brace: lower,\\stackunder{#2[M]}{\\underbrace{#1[M]}} ",
    "25.1=horizontal brace: upper,\\stackrel{#2[M]}{\\overbrace{#1[M]}} ",
    "25.0=hbracket,",
    "26.0=limi",
    "27.0=script: sub,#1[L][STARTSUB][ENDSUB] ",
    "27.1=script: sub,#1[L][STARTSUB][ENDSUB] ",
    "28.0=script: super,#2[L][STARTSUP][ENDSUP] ",
    "28.1=script: super,#2[L][STARTSUP][ENDSUP] ",
    "29.0=script: subsup,#1[L][STARTSUB][ENDSUB]#2[L][STARTSUP][ENDSUP] ",
    "30.0=limi",
    "31.0=limi",
    "32.0=limi",
    "33.0=limi",
    "34.0=limi",
    "35.0=limi",
    "36.0=limi",
    "37.0=limi",
    "TextInMath=\\text{#1} ",
    0
};

/* [EMBELLS] */
/* ;format is "math template,text template" (different from all the above) */
char *Template_EMBELLS[] = {
                "",
                "",
/* embDOT       */ "\\dot{%1} ,\\.%1 ",
/* embDDOT      */ "\\ddot{%1} ,\\\"%1 ",
/* embTDOT      */ "\\dddot{%1} ,%1 ",
/* embPRIME     */ "%1' ,%1 ",
/* embDPRIME    */ "%1'' ,%1 ",
/* embBPRIME    */ "\\backprime %1 , %1",
/* embTILDE     */ "\\tilde{%1} ,\\~%1 ",
/* embHAT       */ "\\hat{%1} ,\\^%1 ",
/* embNOT       */ "\\not %1 ,\\NEG %1 ",
/* embRARROW    */ "\\vec{%1} ,%1 ",
/* embLARROW    */ "\\overleftarrow1{%1} ,%1 ",
/* embBARROW    */ "\\overleftrightarrow{%1} ,%1 ",
/* embR1ARROW   */ "\\overrightarrow{%1} ,%1 ",
/* embL1ARROW   */ "\\overleftarrow{%1} ,%1 ",
/* embMBAR      */ "\\underline{%1} ,%1 ",
/* embOBAR      */ "\\bar{%1} ,\\=%1 ",
/* embTPRIME    */ "%1''' ,",
/* embFROWN     */ "\\widehat{%1} ,%1 ",
/* embSMILE     */ "\\breve{%1} ,%1 ",
/* embX_BARS    */ "{%1} ,%1 ",
/* embUP_BAR    */ "{%1} ,%1 ",
/* embDOWN_BAR  */ "{%1} ,%1 ",
/* emb4DOT      */ "{%1} ,%1 ",
/* embU_1DOT    */ "\\d{%1} ,\\d{%1} ",
/* embU_2DOT    */ "{%1} ,%1 ",
/* embU_3DOT    */ "{%1} ,%1 ",
/* embU_4DOT    */ "{%1} ,%1 ",
/* embU_BAR     */ "{%1} ,%1 ",
/* embU_TILDE   */ "{%1} ,%1 ",
/* embU_FROWN   */ "{%1} ,%1 ",
/* embU_SMILE   */ "{%1} ,%1 ",
/* embU_RARROW  */ "{%1} ,%1 ",
/* embU_LARROW  */ "{%1} ,%1 ",
/* embU_BARROW  */ "{%1} ,%1 ",
/* embU_R1ARROW */ "{%1} ,%1 ",
/* embU_L1ARROW */ "{%1} ,%1 ",
    0
};
