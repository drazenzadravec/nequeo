/*
 * rtfprep
 *
 * Prepare some preliminary files needed for RTF Tools build.
 *
 * This program reads the file rtf-controls, which lists control word
 * tokens and their major and minor macro names.  It produces a file
 * rtf-ctrldef.h containing a #define for each macro, and a file
 * rtf-ctrl listing all control words and their major and minor numbers.
 * rtf-ctrldef.h is included by rtf.h.  rtf-ctrl is read by the code
 * in reader.c when a translator starts up.
 *
 * The program also reads standard-names, which lists standard character
 * names.  It produces a file rtf-namedef.h containing a #define for
 * each character name, and a file stdcharnames.h containing all the
 * names in C string form.  rtf-namedef.h is included by rtf.h.
 * stdcharnames.h is included by reader.c when reader.c is compiled.
 *
 * The code for generating rtf-namedef.h and stdcharnames.h from
 * standard-names is based on code originally written by Wally Wedel,
 * U S WEST Advanced Technologies.
 *
 * 05 Apr 95 Paul DuBois dubois@primate.wisc.edu
 *
 * Change history:
 * 05 Apr 95
 * - Created.
 */

/*
 * Error messages that may be produced by the program:
 *
 * Too many fields in line
 * - An input line contains to many fields.  Recompile with a bigger
 * value of maxFld.  (But more likely you have a strange input line.)
 * Too many controls
 * - There are too many control words to fit in the control structure.
 * Recompile with a bigger value of maxCtrl.
 * Token buffer exceeded
 * - The token text buffer size has been exceeded.  Compile with a bigger
 * value of maxTokBuf.
 */

# include	<stdio.h>
# include	<stdlib.h>
# include	<string.h>

# include	"tokenscan.h"

# define	bufSiz	1024
# define	maxTokBuf (10 * bufSiz)
# define	maxFld	20
# define	maxCtrl	1000
# define	maxMajors	100

# define	ctrlInputFile	"rtf-controls"
# define	rtfCtrlDef		"rtf-ctrldef.h"
# define	rtfCtrlTab		"rtf-ctrl"
# define	nameInputFile	"standard-names"
# define	rtfNameDef		"rtf-namedef.h"
# define	rtfNameTab		"stdcharnames.h"


typedef struct RTFCtrl	RTFCtrl;

struct RTFCtrl
{
	short	major;	/* major number */
	short	minor;	/* minor number */
	char	*str;	/* symbol name */
};

static char	*fld[maxFld];
static char rememberFld[maxMajors][maxMajors];
static short rememberMajor[maxMajors];
static short defined = -1;
static short	nFlds;

static char	buf[bufSiz];
static char	buf2[bufSiz];

static short	curMajor = 0;
static short	curMinor = 0;
static short	majorCount = 0;

static char	tokBuf[maxTokBuf];
static char	*tokBufEnd = tokBuf;

static RTFCtrl	ctrl[maxCtrl];
static short	nCtrls = 0;

static short	nNames = 0;

/*
 * Tokenize rest of input buffer following first word
 */

static void Tokenize (void)
{
char	*p;

	nFlds = 0;
	while ((p = TSScan ()) != (char *) NULL)
	{
		if (nFlds >= maxFld)	/* shouldn't happen */
		{
			fprintf (stderr, "Too many fields in line: %s\n", buf2);
			exit (1);
		}
		fld[nFlds++] = p;
	}
}


int Defined (void)
{
int i = 0;
	if (majorCount == 0) return (0);
	for (i = 0; i < majorCount; i++)
	{
		if (strcmp(rememberFld[i], fld[0]) == 0) 
		{
			defined = rememberMajor[i];
			return (1);
		}
	}
return (0);
}

static void Unescape (char *p1)
{
char	*p2, c;

	/* reprocess string to remove embedded escapes */
	p2 = p1;
	while ((c = *p1++) != '\0')
	{
		/*
		 * Escaped character.  Default is to use next
		 * character unmodified, but \n and \r are
		 * turned into linefeed and carriage return.
		 */
		if (c == '\\')
		{
			c = *p1++;
			switch (c)
			{
			case 'n':
				c = '\n';
				break;
			case 'r':
				c = '\r';
				break;
			}
		}
		*p2++ = c;
	}
	*p2 = '\0';
}

int main (int argc, char *argv[])
{
FILE	*f;
FILE	*of1, *of2;
char	*p, c;
short	len;
short	i, j;
RTFCtrl	*cp, tmpCtrl;
TSScanner	scanner;
char		*scanEscape;

	fprintf (stderr, "Reading %s...\n", ctrlInputFile);
	if ((f = fopen (ctrlInputFile, "rb")) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open input file %s\n", ctrlInputFile);
		exit (1);
	}

	fprintf (stderr, "Writing %s...\n", rtfCtrlDef);
	if (freopen (rtfCtrlDef, "wb", stdout) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open output file %s\n", rtfCtrlDef);
		exit (1);
	}
	
	printf ("/* The following defines are automatically generated from %s */\n",
															ctrlInputFile);
	printf ("/* Do not edit. */\n\n");

	/*
	 * Turn off scanner's backslash escape mechanism while reading
	 * file.  Restore it later.
	 */
	TSGetScanner (&scanner);
	scanEscape = scanner.scanEscape;
	scanner.scanEscape = "";
	TSSetScanner (&scanner);

	/* read file */

	while (fgets (buf, (int) sizeof (buf), f) != (char *) NULL)
	{
		/* strip newline if present */
		len = strlen (buf);
		if (len > 0 && buf[len-1] == '\n')
			buf[len - 1] = '\0';
		/* make copy of line (token scanning modifies original) */
		(void) strcpy (buf2, buf);
		TSScanInit (buf);
		/* echo blank lines to define output */
		if ((p = TSScan ()) == (char *) NULL)
		{
			printf ("%s\n", buf2);
			continue;
		}
		/* ignore comments */
		if (strcmp (p, "comment") == 0)
			continue;
		/* process according to line type */
		if (strcmp (p, "major") == 0)
		{
			Tokenize ();
			if (majorCount < (maxMajors - 1)) 
			{
				strcpy(rememberFld[majorCount], fld[0]);
				rememberMajor[majorCount] = curMajor;
			}
			else fprintf (stderr, "RTFMajor limit exceeded\n");


			if (nFlds != 1)
				continue;
			if (Defined () == 0) 
			{
				printf ("# define\t%s\t%hd\n", fld[0], curMajor++);
				majorCount++;
				defined = -1;
			}
			/*curMinor = 0;*/
		}
		else if (strcmp (p, "minor") == 0)
		{
			Tokenize ();
			if (nFlds < 2)
				continue;
			printf ("# define\t\t%s\t%hd\n",
				fld[0], curMinor++);
			if (nCtrls + nFlds - 1 >= maxCtrl)
			{
				fprintf (stderr, "Too many controls\n");
				exit (1);
			}
			for (i = 1; i < nFlds; i++)
			{
				Unescape (fld[i]);
				if (tokBufEnd + strlen (fld[i]) + 1
					>= tokBuf + sizeof (tokBuf))
				{
					fprintf (stderr, "Token buffer exceeded.\n");
					exit (1);
				}
				if (defined > -1) ctrl[nCtrls].major = defined;
				else ctrl[nCtrls].major = curMajor - 1;
				ctrl[nCtrls].minor = curMinor - 1;
				ctrl[nCtrls].str = tokBufEnd;
				++nCtrls;
				strcpy (tokBufEnd, fld[i]);
				tokBufEnd += strlen (fld[i]) + 1;
			}
		}
		else
		{
			/* echo any unrecognized lines to #define output file */
			printf ("%s\n", buf2);
		}
	}
	scanner.scanEscape = scanEscape;
	TSSetScanner (&scanner);
	fclose(f);

	/*
	 * Sort entries by token text.  This program isn't run often
	 * so just use a bubble sort.
	 */

	for (i = 0; i < nCtrls - 1; i++)
	{
		for (j = i + 1; j < nCtrls; j++)
		{
			if (strcmp (ctrl[i].str, ctrl[j].str) > 0)
			{
				tmpCtrl = ctrl[i];
				ctrl[i] = ctrl[j];
				ctrl[j] = tmpCtrl;
			}
		}
	}

	fprintf (stderr, "Writing %s...\n", rtfCtrlTab);
	if (freopen (rtfCtrlTab, "wb", stdout) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open output file %s\n", rtfCtrlTab);
		exit (1);
	}

	fprintf (stderr, "number of control words: %hd\n", nCtrls);
	fprintf (stderr, "bytes of token space: %hd\n",  (short) (tokBufEnd - tokBuf));
	printf ("# number of control words and bytes of token space\n");
	printf ("%hd %hd\n", nCtrls, (short) (tokBufEnd - tokBuf));

	printf ("# control word table\n");

	for (i = 0; i < nCtrls; i++)
	{
		cp = &ctrl[i];
		printf ("%hd %hd \"", cp->major, cp->minor);
		p = cp->str;
		while ((c = *p++) != '\0')
		{
			switch (c)
			{
			default:
				putchar (c);
				break;
			case '\n':
				printf ("\\n");
				break;
			case '\r':
				printf ("\\r");
				break;
			case '"':
				printf ("\\%c", c);
				break;
			}
		}
		printf ("\"\n");
	}

	fprintf (stderr, "Reading %s...\n", nameInputFile);
	if ((f = fopen (nameInputFile, "rb")) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open input file %s\n", nameInputFile);
		exit (1);
	}

	fprintf (stderr, "Writing %s...\n", rtfNameDef);
	if ((of1 = fopen (rtfNameDef, "wb")) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open output file %s\n", rtfNameDef);
		exit (1);
	}
	fprintf (stderr, "Writing %s...\n", rtfNameTab);
	if ((of2 = fopen (rtfNameTab, "wb")) == (FILE *) NULL)
	{
		fprintf (stderr, "Cannot open output file %s\n", rtfNameTab);
		exit (1);
	}
	
	fprintf (of1, "/* The following defines are automatically generated from %s */\n",
															nameInputFile);
	fprintf (of1, "/* They must be sequential beginning from zero */\n");
	fprintf (of1, "/* Do not edit. */\n\n");

	while (fgets (buf, (int) sizeof (buf), f) != (char *) NULL)
	{
		/* strip newline if present */
		len = strlen (buf);
		if (len > 0 && buf[len-1] == '\n')
			buf[len - 1] = '\0';
		/* ignore comments and blank lines */
		if (buf[0] == '#' || buf[0] == '\0')
			continue;
		fprintf (of1, "#define rtfSC_%s\t%hd\n", buf, nNames);
		fprintf (of2, "\"%s\",\n", buf);
		nNames++;
	}
	fprintf (of1, "\n#define rtfSC_MaxChar\t%hd\n", nNames);
	fclose(f);
	fclose(of1);
	fclose(of2);

	exit (0);
}
