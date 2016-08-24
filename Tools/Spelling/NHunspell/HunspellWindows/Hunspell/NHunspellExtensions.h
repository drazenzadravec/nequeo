#pragma once

// FILE Replacements to work with memory instead
const unsigned long magicConstant = 0xFE0FF07F;

struct MemoryBufferInformation
{
	unsigned long magic; // Trapping misconfigured FILE * replacements by checking magic number
	void * buffer;
	size_t bufferSize;
};



FILE * fopenMemory ( const char * filename, const char * mode );
int fcloseMemory ( FILE * stream );
char *fgetsMemory(char *str, int n, FILE *stream);

#define fopen( filename, mode ) fopenMemory(filename, mode)
#define fclose( stream ) fcloseMemory(stream)
#define fgets( str, n, stream) fgetsMemory( str, n, stream )