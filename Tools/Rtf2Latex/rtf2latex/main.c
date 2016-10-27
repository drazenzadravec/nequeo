/*
 * RTF-to-LaTeX2e translation driver code.
 * (c) 1999 Ujwal S. Sathyam
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

#include    <stdio.h>
#include    <string.h>
#include    <stdlib.h>
#include    <ctype.h>
#include    <stdarg.h>
#include    <stdint.h>
#include    <sys/stat.h>

#include     "rtf.h"
#include     "mygetopt.h"
#include     "rtf2latex2e.h"
#include     "eqn.h"
#include     "init.h"

static const char* VERSION = "16.26.1.1";

boolean ConvertRawEquationFile(char *rawFileName); /* from writer.c */

FILE         *ifp, *ofp;

char  *g_library_path        = NULL;
int   g_little_endian        = 0;
int   g_debug_level          = 0;
int   g_object_width         = 0;
int   g_create_new_directory = 0;

int   g_eqn_insert_image     = 0;
int   g_eqn_keep_file        = 0;
int   g_eqn_insert_name      = 0;
int   g_shouldIncludePreamble= 1;

enum INPUT_FILE_TYPE g_input_file_type;

/* Figure out endianness of machine.  Needed for OLE & graphics support */
static void 
SetEndianness(void)
{
    unsigned int    endian_test = (unsigned int) 0xaabbccdd;
    unsigned char   endian_test_char = *(unsigned char *) &endian_test;
    if (endian_test_char == 0xdd)
        g_little_endian = 1;
}


static void 
print_version(void)
{
    fprintf(stdout, "rtf2latex2e %s\n\n", VERSION);
    fprintf(stdout, "Copyright (C) 2012 Free Software Foundation, Inc.\n");
    fprintf(stdout, "This is free software; see the source for copying conditions.  There is NO\n");
    fprintf(stdout, "warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.\n\n");
}

static void 
print_usage(void)
{
    fprintf(stdout, "rtf2latex2e %s - convert RTF to LaTeX.\n", VERSION);
    fprintf(stdout, "Options:\n");
    fprintf(stdout, "  -h               display help\n");
    fprintf(stdout, "  -b               best attempt at matching RTF formatting\n");
    fprintf(stdout, "  -f               fractional document, omit latex preamble\n");
    fprintf(stdout, "  -D               make a new directory for latex and extracted images\n");
    fprintf(stdout, "  -e #             equation conversion options\n");
    fprintf(stdout, "      -e 1              convert to latex\n");
    fprintf(stdout, "      -e 2              insert image\n");
    fprintf(stdout, "      -e 4              keep intermediate eqn file\n");
    fprintf(stdout, "      -e 8              insert eqn file name in latex document\n");
    fprintf(stdout, "      -e 16             delimit eqns by \\(...\\) and \\[...\\]\n");
    fprintf(stdout, "  -n               natural latex formatting ... easiest to edit\n");
    fprintf(stdout, "  -p #             paragraph conversion options\n");
    fprintf(stdout, "      -p 1              'heading 1' style -> '\\section{}'\n");
    fprintf(stdout, "      -p 2              convert indenting\n");
    fprintf(stdout, "      -p 4              convert space between paragraphs\n");
    fprintf(stdout, "      -p 8              convert line spacing\n");
    fprintf(stdout, "      -p 16             convert margins\n");
    fprintf(stdout, "      -p 32             convert alignment\n");
    fprintf(stdout, "  -P path          path to preferred preference directory\n");
    fprintf(stdout, "  -t #             text conversion options\n");
    fprintf(stdout, "      -t 1              convert text size\n");
    fprintf(stdout, "      -t 2              convert text color\n");
    fprintf(stdout, "      -t 4              convert text formatting\n");
    fprintf(stdout, "      -t 8              replace tabs with spaces\n");
    fprintf(stdout, "  -T #             table conversion options\n");
    fprintf(stdout, "      -T 1              keep column widths\n");
    fprintf(stdout, "      -T 2              keep column alignment\n");
    fprintf(stdout, "  -v               version information\n");
    fprintf(stdout, "\n");
    fprintf(stdout, "Examples:\n");
    fprintf(stdout, "  rtf2latex2e foo              convert foo.rtf to foo.tex\n");
    fprintf(stdout, "  rtf2latex2e -n foo           minimal mark-up\n");
    fprintf(stdout, "  rtf2latex2e -e 15 foo        help identify failed equation conversion\n");
    fprintf(stdout, "  rtf2latex2e foo-eqn003.eqn   debug third equation (after above command)\n");
    fprintf(stdout, "  rtf2latex2e -D foo           put foo.tex and images in foo-latex dir\n");
    fprintf(stdout, "  rtf2latex2e foo.rtfd         convert to foo.rtfd/TXT.tex\n");
    fprintf(stdout, "\n");
    fprintf(stdout, "Preference Directory:\n");
    fprintf(stdout, "  Default  = '%s'\n", (PREFS_DIR) ? PREFS_DIR : "not defined");
    fprintf(stdout, "  $RTFPATH = '%s'\n", (getenv("RTFPATH")) ? getenv("RTFPATH") : "not defined");
    fprintf(stdout, "\n");
    fprintf(stdout, "Report bugs at http://sourceforge.net/projects/rtf2latex2e/\n\n");
    exit(1);
}

static enum INPUT_FILE_TYPE identify_filename(char *name)
{
    int len;
    
    if (!name) return TYPE_UNKNOWN;
    
    len = strlen(name);
    
    if (len > 5 && strnicmp(name+len-5,".rtfd", strlen(name) + 5)==0)
        return TYPE_RTFD;
        
    if (len > 4 && strnicmp(name+len-4,".rtf", strlen(name) + 4)==0)
        return TYPE_RTF;
        
    if (len > 4 && strnicmp(name+len-4,".eqn", strlen(name) + 4)==0)
        return TYPE_EQN;
        
    if (len > 4 && strnicmp(name+len-4,".rqn", strlen(name) + 4)==0)
        return TYPE_RAWEQN;

    return TYPE_UNKNOWN;
}


static char * establish_filename(char * name)
{
    FILE *fp;
    char *s;
    int len;
    
    g_input_file_type = TYPE_UNKNOWN;
    if (!name) return NULL;
    
    /* try .rtfd case first because simple fopen() test below misses this case */
    len = strlen(name) - 5;
    if (len > 0 && strnicmp(name + len,".rtfd", strlen(name) + len) == 0) {
        s = append_file_to_path(name, "TXT.rtf");
        fp = fopen(name, "rb");
        if (fp) {
            fclose(fp);
            g_input_file_type = TYPE_RTFD;
            return s;
        }
        free(s);
        fprintf(stderr,"* RTFD directory found, but no TXT.rtf file inside!\n");
        return NULL;
    }
    
    fp = fopen(name, "rb");
    if (fp) {
        fclose(fp);
        g_input_file_type = identify_filename(name);
        return strdup(name);
    }
    
    s = strdup_together(name, ".rtf");
    fp = fopen(s, "rb");
    if (fp) {
        fclose(fp);
        g_input_file_type = TYPE_RTF;
        return s;
    }

    free(s);
    s = strdup_together(name, ".rtfd");
    fp = fopen(s, "rb");
    if (fp) {
        fclose(fp);
        free(s);
        s = strdup_together(name, ".rtfd/TXT.rtf");
        fp = fopen(s, "rb");
        if (fp) {
            fclose(fp);
            g_input_file_type = TYPE_RTFD;
            return s;
        }
        
        return s;
        fprintf(stderr,"* RTFD directory found, but no TXT.rtf file inside!\n");
        return NULL;
    }

    free(s);
    s = strdup_together(name, ".eqn");
    fp = fopen(s, "rb");
    if (fp) {
        fclose(fp);
        g_input_file_type = TYPE_EQN;
        return s;
    }

    free(s);
    return NULL;
}


static char * short_name(char *path)
{
    char *s;
    s = strrchr(path,PATH_SEP);
    if (s == NULL) 
        return strdup(path);
    else
        return strdup(s+1);
}

/* initial name for translated file will end in .ltx.  Later
   the file will be rewritten to a name that ends with .tex.
   the windows rename() function is horrible and so we do this
   dance instead (see EndLatexFile in writer.c)
 */
static char * make_output_filename(char * name)
{
    char *s, *dir, *file, *out;
    if (!name) return NULL;
    
    /* always create a new folder for RTFD files */
    if (g_input_file_type == TYPE_RTFD) {

        dir = malloc(strlen(name)+strlen("-latex"));
        strcpy(dir,name);
        s = strstr(dir, ".rtfd");
        if (!s) 
        	s = strstr(dir, ".RTFD");

        if (s)
        	strcpy(s,"-latex");
        else
        	strcat(dir,"-latex");
        
        mkdir(dir, 0755);
        out = append_file_to_path(dir,"TXT.ltx");
        return out;
    }

    if (!g_create_new_directory || g_input_file_type == TYPE_EQN || g_input_file_type == TYPE_RAWEQN) {
        s = strdup(name);
        strcpy(s+strlen(s)-4, ".ltx");
        return s;
    } 
    
    if (g_input_file_type == TYPE_RTF || g_input_file_type == TYPE_RTFD) {
        file = short_name(name);        
        strcpy(file+strlen(file)-4, ".ltx");

        dir = malloc(strlen(name)+strlen("-latex"));
        strcpy(dir,name);
        strcpy(dir+strlen(dir)-4,"-latex");
        
        mkdir(dir, 0755);
        out = append_file_to_path(dir,file);
        return out;
    }
    
    return NULL;
}

void ExamineToken(void);

int 
main(int argc, char **argv)
{
    char            c, *input_filename, *output_filename;
    int             fileCounter, cli_paragraph, cli_text, cli_equation, cli_table;
    int             g_eqn_delim_brackets;
    long            cursorPos;
    extern char    *optarg;
    extern int      optind;

    SetEndianness();
    cli_paragraph = -1;
    cli_text = -1;
    cli_equation = -1;
    cli_table = -1;

    g_eqn_delim_brackets = 0;
    eqn_start_inline = strdup("$");
    eqn_end_inline = strdup(" $");
    eqn_start_display = strdup(" $$");
    eqn_end_display = strdup("$$\n");

    while ((c = my_getopt(argc, argv, "bDe:fhnp:P:t:T:v")) != EOF) {
        switch (c) {

        case 'b':
            cli_paragraph = 63;
            cli_text = 7;
            cli_equation = 1;
            cli_table = 3;
            break;

        case 'D':
            g_create_new_directory = 1;
            break;

        case 'e':
            sscanf(optarg, "%d", &cli_equation);
            break;

        case 'f':
            g_shouldIncludePreamble = 0;
            break;

        case 'n':
            cli_paragraph = 33;
            cli_text = 12;
            cli_equation = 1;
            cli_table = 0;
            break;

        case 'p':
            sscanf(optarg, "%d", &cli_paragraph);
            break;

        case 'P':   /* -P path/to/pref */
            g_library_path = strdup(optarg);
            break;

        case 't':
            sscanf(optarg, "%d", &cli_text);
            break;

        case 'T':
            sscanf(optarg, "%d", &cli_table);
            break;

        case 'v':
        case 'V':
            print_version();
            return (0);

        case 'h':
        case '?':
        default:
            print_usage();
        }
    }

    argc -= optind;
    argv += optind;

    if (!argc) print_usage();
    InitConverter();

    if (cli_paragraph >= 0) {
        prefs[pConvertParagraphStyle]      = cli_paragraph & 1;
        prefs[pConvertParagraphIndent]     = cli_paragraph & 2;
        prefs[pConvertInterParagraphSpace] = cli_paragraph & 4;
        prefs[pConvertLineSpacing]         = cli_paragraph & 8;
        prefs[pConvertParagraphMargin]     = cli_paragraph & 16;
        prefs[pConvertParagraphAlignment]  = cli_paragraph & 32;
    }

    if (cli_text >= 0) {
        prefs[pConvertTextSize]  = cli_text & 1;
        prefs[pConvertTextColor] = cli_text & 2;
        prefs[pConvertTextForm]  = cli_text & 4;
        prefs[pConvertTextNoTab] = cli_text & 8;
    }

    if (cli_equation >= 0) {
        prefs[pConvertEquation] = cli_equation & 1;
        g_eqn_insert_image      = cli_equation & 2;
        g_eqn_keep_file         = cli_equation & 4;
        g_eqn_insert_name       = cli_equation & 8;
        g_eqn_delim_brackets    = cli_equation & 16;
    }
    if  (g_eqn_delim_brackets) {
        eqn_start_inline = strdup("\\(");
        eqn_end_inline = strdup(" \\)");
        eqn_start_display = strdup("\\[");
        eqn_end_display = strdup(" \\]\n");
        }
    
    if (cli_table >= 0) {
        prefs[pConvertTableWidths]    = cli_table & 1;
        prefs[pConvertTableAlignment] = cli_table & 2;
        prefs[pConvertTableBorders]   = cli_table & 4;
    }

    for (fileCounter = 0; fileCounter < argc; fileCounter++) {

        RTFInit();
        
        input_filename = establish_filename(argv[fileCounter]);
    	
    	if (g_input_file_type == TYPE_RTFD)
    		g_create_new_directory = 1;
    		
        if (g_input_file_type == TYPE_UNKNOWN) {
            if (!input_filename)
                RTFMsg("* Skipping non-existent file '%s'\n", argv[fileCounter]);
            else 
                RTFMsg("* Skipping non-RTF file '%s'\n", argv[fileCounter]);
            if (input_filename) free(input_filename);
            continue;
        }
        
        ifp = fopen(input_filename, "rb");
        if (!ifp) {
            RTFPanic("* Cannot open input file %s\n", input_filename);
            exit(1);
        }
        
        RTFSetInputName(input_filename);
        RTFSetStream(ifp);

        /* look at second token to check if input file is of type rtf */
        if (g_input_file_type != TYPE_EQN && g_input_file_type != TYPE_RAWEQN) {
            cursorPos = ftell(ifp);
            RTFGetToken();
            RTFGetToken();
            if (!RTFCheckCMM(rtfControl, rtfVersion, rtfVersionNum)) {
                RTFMsg("* Yikes! '%s' is not actually a RTF file!  Skipping....\n", input_filename);
                fclose(ifp);
                free(input_filename);
                continue;
            }
            fseek(ifp, cursorPos, 0);
        }
        RTFInit();
        
        output_filename = make_output_filename(input_filename);

        ofp = fopen(output_filename, "wb+");
        if (!ofp) {
            RTFMsg("Cannot open output file %s\n", output_filename);
            free(input_filename);
            free(output_filename);
            continue;
        }
        RTFSetOutputName(output_filename);
        RTFSetOutputStream(ofp);

        fprintf(stderr, "Processing %s\n", input_filename);
        if (BeginLaTeXFile()) {
            if (g_input_file_type == TYPE_EQN) 
                (void) ConvertEquationFile(input_filename);
            else if (g_input_file_type == TYPE_RAWEQN) {
                (void) ConvertRawEquationFile(input_filename);
            } else

                RTFRead();
            EndLaTeXFile(); /* closes ofp */
        }
        
        fclose(ifp);
        free(input_filename);
        free(output_filename);
    }

    return (0);
}
