/*
 * tokenscan.h - token scanning library stuff
 */


/*
 * Flags for scanFlags field
 */

#define	tsNoConcatDelims	0x01	/* all delimiters are significant */


typedef	struct TSScanner	TSScanner;

struct TSScanner
{
	void	(*scanInit) (char *p);
	char	*(*scanScan) (void);
	char	*scanDelim;
	char	*scanQuote;
	char	*scanEscape;
	char	*scanEos;
	int	scanFlags;
};


void TSScanInit (char *p);
void TSSetScanner (TSScanner *p);
void TSGetScanner (TSScanner *p);
void TSSetScanPos (char *p);
char *TSGetScanPos (void);
int TSIsScanDelim (char c);
int TSIsScanQuote (char c);
int TSIsScanEscape (char c);
int TSIsScanEos (char c);
int TSTestScanFlags (int flags);
char	*TSScan (void);
