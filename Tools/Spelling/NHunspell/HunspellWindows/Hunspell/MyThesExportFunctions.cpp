#include <stdlib.h> /* for NULL, malloc */
#include <stdio.h>  /* for fprintf */
#include <string.h> /* for strdup */

#ifdef UNX
#include <unistd.h> /* for exit */
#endif

#define noVERBOSE

#include <windows.h>
#include "mythes/mythes.hxx"
#include "EncodingToCodePage.h"
#include "NHunspellExtensions.h"


#define DLLEXPORT extern "C" __declspec( dllexport )


class NMyThes : public MyThes
{
	// The methods aren't multi threaded, reentrant or whatever so a encapsulated buffer can be used for performance reasons
	void * marshalBuffer;
	size_t marshalBufferSize;

	void * wordBuffer;
	size_t wordBufferSize;

	void * affixBuffer;
	size_t affixBufferSize;

public:
	UINT CodePage;

	inline NMyThes(const char *idxpath, const char *datpath) : MyThes(idxpath, datpath) 
	{
		marshalBuffer = 0;
		marshalBufferSize = 0;

		wordBuffer = 0;
		wordBufferSize = 0;
		char * encoding = get_th_encoding();
		CodePage = EncodingToCodePage( encoding );
	}


	inline ~NMyThes()
	{
		if( marshalBuffer != 0 )
			free( marshalBuffer );

		if( wordBuffer != 0 )
			free( wordBuffer );

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

DLLEXPORT void * MyThesInit(void * idxBuffer, size_t idxBufferSize, void * datBuffer, size_t datBufferSize)
{

	MemoryBufferInformation idxBufferInfo;
	idxBufferInfo.magic = magicConstant;
	idxBufferInfo.buffer = idxBuffer;
	idxBufferInfo.bufferSize = idxBufferSize;

	MemoryBufferInformation datBufferInfo;
	datBufferInfo.magic = magicConstant;
	datBufferInfo.buffer = datBuffer;
	datBufferInfo.bufferSize = datBufferSize;


	void * result = new NMyThes((const char *) & idxBufferInfo, (const char *) & datBufferInfo ); 
	return result;
}

DLLEXPORT void MyThesFree(NMyThes * handle )
{
	delete handle;
}


DLLEXPORT void * MyThesLookup(NMyThes * handle, wchar_t * word )
{
	size_t wordChars = wcslen( word );
	char * word_buffer = handle->GetWordBuffer(word);
	int wordBufferSize = strlen( word_buffer );


	mentry * meanings;
	DWORD count = handle->Lookup(word_buffer, wordBufferSize, &meanings );


	// Marshal Buffer Layout
	// Meanings Count m (Allocated size void * to match IntPtr)
	// m * Ptr to Meaning Data
	// Meaning Data / String Data

	// Meaning Data Layout
	// Synonyms Count s (Allocated size void * to match IntPtr)
	// Ptr to Meaning Description
	// s* Ptr to Synonym string


	if (count) 
	{
		mentry * meaning = meanings;
		size_t bufferSize = sizeof(void ** ); // Meanings Count
		for (int  i=0; i < count; i++) 
		{
			bufferSize += sizeof(void ** ); // Ptr to Meaning Data
			bufferSize += sizeof(void ** ); // Synonyms Count
			bufferSize += sizeof(void ** ); // Ptr to Meanings Description 
			bufferSize += meaning->count * sizeof(void ** ); // Synonyms Strings

			size_t meaningDescriptionWChars= MultiByteToWideChar(handle->CodePage,0,meaning->defn,-1,0,0);
			bufferSize += meaningDescriptionWChars * sizeof(wchar_t); // Syononym String size (+ ending 0)

			// synonyms += meaning->count;
			for (int j=0; j < meaning->count; j++) 
			{
				char * synonym = meaning->psyns[j];
				size_t synonymWordWChars= MultiByteToWideChar(handle->CodePage,0,synonym,-1,0,0);
				bufferSize += synonymWordWChars * sizeof(wchar_t); // Syononym String size (+ ending 0)
			}
			++meaning;
		}

		BYTE * marshalBuffer = (BYTE *) handle->GetMarshalBuffer( bufferSize );
        BYTE * currentBufferPos = marshalBuffer;

		*((void **) currentBufferPos) = (void *) count;
		currentBufferPos += sizeof( void * );	
		void ** meaningPtr = (void **) currentBufferPos;
		currentBufferPos += sizeof( void **) * count;
		meaning = meanings;
		for (int  i=0; i < count; i++) 
		{   
			void ** currentMeaning = (void **) currentBufferPos;
			currentBufferPos += sizeof( void * ); // Count;
			currentBufferPos += sizeof( void ** ) * (meaning->count + 1); // Ptr

			*meaningPtr = (void *) currentMeaning;

			*currentMeaning = (void *) meaning->count;

			++currentMeaning;
	
			*currentMeaning = (void *) currentBufferPos;
			++currentMeaning;
			size_t meaningDescriptionWChars= MultiByteToWideChar(handle->CodePage,0,meaning->defn,-1,0,0);
            MultiByteToWideChar(handle->CodePage,0,meaning->defn,-1,(wchar_t *) currentBufferPos,meaningDescriptionWChars);
			currentBufferPos += meaningDescriptionWChars * sizeof(wchar_t); 

			for (int j=0; j < meaning->count; j++) 
			{
				*currentMeaning = (void *) currentBufferPos;
				++currentMeaning;
				char * synonym = meaning->psyns[j];
				size_t synonymWordWChars= MultiByteToWideChar(handle->CodePage,0,synonym,-1,0,0);
				MultiByteToWideChar(handle->CodePage,0,synonym,-1,(wchar_t *) currentBufferPos,synonymWordWChars);
				currentBufferPos += synonymWordWChars * sizeof(wchar_t); 
			}

            ++meaningPtr;
			++meaning;
		}
		handle->CleanUpAfterLookup(&meanings,count);
		return marshalBuffer;
	} 

	return 0;
}
