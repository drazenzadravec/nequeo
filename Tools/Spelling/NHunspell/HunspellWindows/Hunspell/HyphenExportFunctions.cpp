#include <stdlib.h> /* for NULL, malloc */
#include <stdio.h>  /* for fprintf */
#include <string.h> /* for strdup */

#ifdef UNX
#include <unistd.h> /* for exit */
#endif

#define noVERBOSE

// #include "hnjalloc.h"
#include <windows.h>
#include <locale.h>
#include <mbctype.h>
#include "hyphen/hyphen.h"
#include "NHunspellExtensions.h"
#include "EncodingToCodePage.h"
#define DLLEXPORT extern "C" __declspec( dllexport )


class NHyphen
{
	HyphenDict * dictionary;

  // The methods aren't multi threaded, reentrant or whatever so a encapsulated buffer can be used for performance reasons
	void * marshalBuffer;
	size_t marshalBufferSize;

	void * wordBuffer;
	size_t wordBufferSize;

	void * affixBuffer;
	size_t affixBufferSize;

public:
	UINT CodePage;

	inline NHyphen(const char *dict) 
	{
		marshalBuffer = 0;
		marshalBufferSize = 0;

		wordBuffer = 0;
		wordBufferSize = 0;

		dictionary = hnj_hyphen_load(dict);
        CodePage = EncodingToCodePage( dictionary->cset );

	}


	inline ~NHyphen()
	{
		if( marshalBuffer != 0 )
			free( marshalBuffer );

		if( wordBuffer != 0 )
			free( wordBuffer );

		if( dictionary != 0 )
			hnj_hyphen_free( dictionary );
	}

	
	inline int Hyphenate(  const char *word, int word_size, char * hyphens,
        char *hyphenated_word, char *** rep, int ** pos, int ** cut )
	{
		return hnj_hyphen_hyphenate2(dictionary,word,word_size,hyphens,hyphenated_word, rep, pos, cut); 
	}

	
	// Buffer for marshaling string lists back to .NET
	void * GetMarshalBuffer(size_t size) 
	{
		if(size < marshalBufferSize )
			return marshalBuffer;

		if( marshalBufferSize == 0 )
		{
			size += 128;
			marshalBuffer = malloc( size );
		}
		else
		{
			size += 256;
			marshalBuffer = realloc(marshalBuffer, size );
		}
		
		marshalBufferSize = size;
		return marshalBuffer;

	}
	
	// Buffer for the mutibyte representation of a word
	void * GetWordBuffer(size_t size) 
	{
		if(size < wordBufferSize )
			return wordBuffer;

		if( wordBufferSize == 0 )
		{
			size += 32;
			wordBuffer = malloc( size );
		}
		else
		{
			size += 64;
			wordBuffer = realloc(wordBuffer, size );
		}
		
		wordBufferSize = size;
		return wordBuffer;
	}


	inline char * GetWordBuffer( wchar_t * unicodeString )
	{
		size_t buffersize = WideCharToMultiByte(CodePage,0,unicodeString,-1,0,0,0,0); 
		char * buffer = (char *) GetWordBuffer( buffersize );
		WideCharToMultiByte(CodePage,0,unicodeString,-1,buffer,buffersize,0,0);
		return buffer;
	}




};

inline char * AllocMultiByteBuffer( wchar_t * unicodeString )
{
	size_t buffersize = WideCharToMultiByte(CP_UTF8,0,unicodeString,-1,0,0,0,0); 
	char * buffer = (char *) malloc( buffersize );
	WideCharToMultiByte(CP_UTF8,0,unicodeString,-1,buffer,buffersize,0,0);
	return buffer;
}




/************************* Export Functions **************************************************************/

DLLEXPORT void * HyphenInit(void * dictionaryBuffer, size_t dictionaryBufferSize)
{

	MemoryBufferInformation dictionaryBufferInfo;
	dictionaryBufferInfo.magic = magicConstant;
	dictionaryBufferInfo.buffer = dictionaryBuffer;
	dictionaryBufferInfo.bufferSize = dictionaryBufferSize;

	void * result = new NHyphen((const char *) &dictionaryBufferInfo ); 
	return result;
}

DLLEXPORT void HyphenFree(NHyphen * handle )
{
	delete handle;
}


DLLEXPORT void * HyphenHyphenate(NHyphen * handle, wchar_t * word )
{
	size_t wordChars = wcslen( word );
	char * word_buffer = handle->GetWordBuffer(word);
	int wordBufferSize = strlen( word_buffer );

	char * hyphenPoints = new char[wordBufferSize + 5];
	char * hyphenatedWord = new char[wordBufferSize * 2];

	char **rep = 0;
	int *pos = 0;
	int *cut = 0;

	handle->Hyphenate(word_buffer,wordBufferSize,hyphenPoints,hyphenatedWord,&rep,&pos,&cut);

	size_t hyphenatedWordWChars= MultiByteToWideChar(handle->CodePage,0,hyphenatedWord,-1,0,0);

	// Marshal Buffer Layout:
	// Pointer to hyphenated word (in Data)
	// Pointer to hyphenation points array (in Data)
	// Pointer to REP Data (wchar_t ptr Array)
	// Pointer to POS Data (int Array)
	// Pointer to CUT Data (int Array)
	// Data - Hyphenated Word
	// Data - Hyphenation Points


	size_t bufferSize = sizeof (wchar_t *); // ptr to hyphenated word
	bufferSize += sizeof(BYTE **); // pointer to hyphenation points array
	bufferSize += sizeof(wchar_t **); // pointer to REP Data
	bufferSize += sizeof(int *); // pointer to POS Data
	bufferSize += sizeof(int *); // pointer to CUT Data

	bufferSize += hyphenatedWordWChars * sizeof( wchar_t); // hyphenated word
	bufferSize += wordChars * sizeof( BYTE); // hyphenation points;
	if( rep != 0 )
	{
		bufferSize += wordChars * sizeof( wchar_t *); // REP Data
		bufferSize += wordChars * sizeof( int); // POS Data
		bufferSize += wordChars * sizeof( int); // CUT Data

		for( int index = 0; index < wordChars; ++index )
		{
			if( rep[index] != 0 )
			{
  				size_t repTextChars= MultiByteToWideChar(handle->CodePage,0,rep[index],-1,0,0);
				bufferSize += repTextChars * sizeof(wchar_t);
			}
		}
	}


	BYTE * marshalBuffer = (BYTE *) handle->GetMarshalBuffer( bufferSize );
	BYTE * currentBufferPos = marshalBuffer;

	wchar_t ** hyphenatedWordPtr = (wchar_t ** ) currentBufferPos;
    currentBufferPos += sizeof( wchar_t * );

	BYTE ** hyphenationPointsPtr = (BYTE ** ) currentBufferPos;
    currentBufferPos += sizeof( BYTE ** );

	wchar_t *** repPtr = (wchar_t *** ) currentBufferPos;
    currentBufferPos += sizeof( wchar_t *** );

	int ** posPtr = (int ** ) currentBufferPos;
    currentBufferPos += sizeof( int ** );

	int ** cutPtr = (int ** ) currentBufferPos;
    currentBufferPos += sizeof( int ** );

	// Hyphenated Word
	*hyphenatedWordPtr = (wchar_t *) currentBufferPos;
	MultiByteToWideChar(handle->CodePage,0,hyphenatedWord,-1,*hyphenatedWordPtr,hyphenatedWordWChars);
    currentBufferPos += sizeof( wchar_t ) * hyphenatedWordWChars;

	// Hyphenation Points
	*hyphenationPointsPtr = (BYTE *) currentBufferPos;
	currentBufferPos += wordChars * sizeof( BYTE);

	if (rep)
	{
		*repPtr = (wchar_t ** ) currentBufferPos;
		currentBufferPos += wordChars * sizeof(wchar_t ** );

		*posPtr = (int *) currentBufferPos;
		currentBufferPos += wordChars * sizeof(int *);

		*cutPtr = (int *) currentBufferPos;
		currentBufferPos += wordChars * sizeof(int *);
	}
	else
	{
		*repPtr = 0;
		*posPtr = 0;
		*cutPtr = 0;
	}

	int wideCharIndex = 0;
	for( int multByteIndex = 1; multByteIndex < wordBufferSize; ++ multByteIndex)
	{
		// Multibyte Start Bytes 0XXXXXXX oder 11XXXXXX ( Follow Bytes : 10XXXXXX )
		if( BYTE( word_buffer[multByteIndex] ) < 0x80 || BYTE( word_buffer[multByteIndex] ) >= 0xC0 )
		{
			(*hyphenationPointsPtr)[wideCharIndex] = hyphenPoints[multByteIndex -1] - '0';

			if( rep )
			{
				if( rep[multByteIndex] )
				{
					size_t repTextChars= MultiByteToWideChar(handle->CodePage,0,rep[multByteIndex],-1,0,0);
					wchar_t * repTextBuffer = (wchar_t *) currentBufferPos;
					currentBufferPos += repTextChars * sizeof(wchar_t);
					(*repPtr)[wideCharIndex] = repTextBuffer;
   				    MultiByteToWideChar(handle->CodePage,0,rep[multByteIndex],-1,repTextBuffer,repTextChars);

				}
				else 
					(*repPtr)[wideCharIndex] = 0;

				(*posPtr)[wideCharIndex] = pos[multByteIndex];
				(*cutPtr)[wideCharIndex] = cut[multByteIndex];

			}
			++wideCharIndex;
		}
	}

    if (rep)
	{
		for (int i = 0; i < wordChars - 1; i++) 
		{
			if (rep[i]) 
				free(rep[i]);
		}

		free(rep);
		free(pos);
		free(cut);
	}

	return marshalBuffer;
}