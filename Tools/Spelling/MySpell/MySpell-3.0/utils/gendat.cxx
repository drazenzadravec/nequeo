#include <cctype>
#include <cstring>
#include <cstdlib>
#include <cstdio>
#include <locale.h>

using namespace std;

int 
main(int argc, char** argv)
{
  char * enc="en_US.ISO-8859-1";

  if (argv[1]) {
    enc =  strdup(argv[1]);
  }


  fprintf(stdout,"%s\n",enc);
       
  char * lp = setlocale(LC_ALL, enc);
  if (lp) 
      fprintf(stdout,"locale returned %s\n",lp);

  for (int i=0; i < 256; i++) {
  
    fprintf(stdout,"0x%02x, 0x%02x, 0x%02x, 0x%02x\n",(unsigned char)i,isupper((unsigned char)i),
            tolower((unsigned char)i), toupper((unsigned char)i));
  } 

  return 0;
}
