#ifndef _EQN_H

#define _EQN_H

# define DISPLAY_EQUATION 0
# define INLINE_EQUATION 1

char *eqn_start_inline;
char *eqn_end_inline;
char *eqn_start_display;
char *eqn_end_display;

struct EQN_OLE_FILE_HDR {
    uint16_t   cbHdr;     /* length of header, sizeof(EQNOLEFILEHDR) = 28 bytes */
    uint32_t   version;   /* hiword = 2, loword = 0 */
    uint16_t   format;
    uint32_t   size;
    uint32_t   reserved1;
    uint32_t   reserved2;
    uint32_t   reserved3;
    uint32_t   reserved4;
};

typedef struct {
    int mathattr;
    int do_lookup;
    int use_codepoint;
} MT_CHARSET_ATTS;

typedef struct {
    struct MT_OBJLIST *next;
    uint8_t tag;
    void *obj_ptr;
} MT_OBJLIST;

typedef struct {
    MT_OBJLIST *o_list;
    FILE *out_file;

    char indent[128];
    int log_level;
    
    MT_CHARSET_ATTS *atts_table;
    char **m_atts_table;
    char **m_char_table;

    int m_mtef_ver;
    int m_platform;
    int m_product;
    int m_version;
    int m_version_sub;
    int m_inline;
    int m_mode;
    char *m_latex_start;
    char *m_latex;
    char *m_latex_end;
} MTEquation;

int Eqn_Create(MTEquation * eqn, unsigned char *eqn_stream, int eqn_size);
void Eqn_Destroy(MTEquation * eqn);
void Eqn_TranslateObjectList(MTEquation * eqn, FILE * outfile, int loglevel);
boolean ConvertEquationFile(char *objectFileName);

#endif
