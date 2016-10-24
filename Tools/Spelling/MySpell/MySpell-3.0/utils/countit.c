/* count letters in a munched word list (ignoring affix chars) */

#include <ctype.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>
#include <fcntl.h>

#define MAXDELEN 1024

char * mystrdup(const char *);

void mychomp(char *);

int main(int argc, char** argv)
{

  int i;

  FILE * wrdlst;

  char *wf;

  char ts[MAXDELEN];

  char *ap;
  unsigned char *wp;

  int cc[256];

  /* first parse the command line options */
  /* arg1 - wordlist */

  if (argv[1]) {
       wf = mystrdup(argv[1]);
  } else {
    fprintf(stderr,"correct syntax is:\n"); 
    fprintf(stderr,"countit word_list_file\n");
    exit(1);
  }

  /* open the wordlist */
  wrdlst = fopen(wf,"r");
  if (!wrdlst) {
    fprintf(stderr,"Error - could not open word list file\n");
    exit(1);
  }


  /* initialize all character counts to 0 */
  for (i=0; i<256; i++) cc[i] = 0;

  /* skip over the first line to skip hash table size */
  if (! fgets(ts, MAXDELEN-1,wrdlst)) return 2;

  while (fgets(ts,MAXDELEN-1,wrdlst)) {
    mychomp(ts);

    wp = (unsigned char *)ts;

    /* remove any affix char strings */
    ap = strchr(ts,'/');
    if (ap) *ap = '\0';
    
    while (*wp) {
      cc[*wp] = cc[*wp] + 1;
      wp++;
    }
  }


  /* now output the array counts for counts > 0 */
  for (i=0; i<256; i++) { 
     if (cc[i] > 0) {
        fprintf(stdout,"%d,%c\n",cc[i],i);
     }
  }

  return 0;
}


char * mystrdup(const char * s)
{
  char * d = NULL;
  if (s) {
    int sl = strlen(s);
    d = (char *) malloc(((sl+1) * sizeof(char)));
    if (d) memcpy(d,s,((sl+1)*sizeof(char)));
  }
  return d;
}


void mychomp(char * s)
{
  int k = strlen(s);
  if (k > 0) *(s+k-1) = '\0';
  if ((k > 1) && (*(s+k-2) == '\r')) *(s+k-2) = '\0';
}
