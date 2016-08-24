#include "hunspell/hunspell.hxx"
#include <windows.h>
#include <locale.h>
#include <mbctype.h>
#include "NHunspellExtensions.h"
#include "EncodingToCodePage.h"
#define DLLEXPORT extern "C" __declspec( dllexport )


class NHunspell: public Hunspell
{
  // The methods aren't multi threaded, reentrant or whatever so a encapsulated buffer can be used for performance reasons
	void * marshalBuffer;
	size_t marshalBufferSize;

	void * wordBuffer;
	size_t wordBufferSize;

	void * word2Buffer;
	size_t word2BufferSize;

	void * affixBuffer;
	size_t affixBufferSize;

public:

	UINT CodePage;


	inline NHunspell(const char *affpath, const char *dpath, const char * key = 0) : Hunspell(affpath,dpath,key) 
	{
		marshalBuffer = 0;
		marshalBufferSize = 0;

		wordBuffer = 0;
		wordBufferSize = 0;
		
		word2Buffer = 0;
		word2BufferSize = 0;

		affixBuffer = 0;
		affixBufferSize = 0;

		char * encoding = get_dic_encoding();
        CodePage = EncodingToCodePage( encoding );
	}

	inline ~NHunspell()
	{
		if( marshalBuffer != 0 )
			free( marshalBuffer );

		if( wordBuffer != 0 )
			free( wordBuffer );

		if( word2Buffer != 0 )
			free( word2Buffer );

		if( affixBuffer != 0 )
			free( affixBuffer );	
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

		// Buffer for the mutibyte representation of a word
	void * GetWord2Buffer(size_t size) 
	{
		if(size < word2BufferSize )
			return word2Buffer;

		if( word2BufferSize == 0 )
		{
			size += 32;
			word2Buffer = malloc( size );
		}
		else
		{
			size += 64;
			word2Buffer = realloc(wordBuffer, size );
		}
		
		word2BufferSize = size;
		return word2Buffer;
	}

	inline char * GetWordBuffer( wchar_t * unicodeString )
	{
		size_t buffersize = WideCharToMultiByte(CodePage,0,unicodeString,-1,0,0,0,0); 
		char * buffer = (char *) GetWordBuffer( buffersize );
		WideCharToMultiByte(CodePage,0,unicodeString,-1,buffer,buffersize,0,0);
		return buffer;
	}

	inline char * GetWord2Buffer( wchar_t * unicodeString )
	{
		size_t buffersize = WideCharToMultiByte(CodePage,0,unicodeString,-1,0,0,0,0); 
		char * buffer = (char *) GetWord2Buffer( buffersize );
		WideCharToMultiByte(CodePage,0,unicodeString,-1,buffer,buffersize,0,0);
		return buffer;
	}

	// Buffer for the mutibyte representation of an affix
	void * GetAffixBuffer(size_t size) 
	{
		if(size < affixBufferSize )
			return affixBuffer;

		if( affixBufferSize == 0 )
		{
			size += 32;
			affixBuffer = malloc( size );
		}
		else
		{
			size += 64;
			affixBuffer = realloc(affixBuffer, size );
		}
		
		affixBufferSize = size;
		return affixBuffer;
	}



	inline char * GetAffixBuffer( wchar_t * unicodeString )
	{
		size_t buffersize = WideCharToMultiByte(CodePage,0,unicodeString,-1,0,0,0,0); 
		char * buffer = (char *) GetAffixBuffer( buffersize );
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

DLLEXPORT NHunspell * HunspellInit(void * affixBuffer, size_t affixBufferSize, void * dictionaryBuffer, size_t dictionaryBufferSize, wchar_t * key)
{
	char * key_buffer = 0;
	if( key != 0 )
		key_buffer = AllocMultiByteBuffer(key);

	MemoryBufferInformation affixBufferInfo;
	affixBufferInfo.magic = magicConstant;
	affixBufferInfo.buffer = affixBuffer;
	affixBufferInfo.bufferSize = affixBufferSize;

	MemoryBufferInformation dictionaryBufferInfo;
	dictionaryBufferInfo.magic = magicConstant;
	dictionaryBufferInfo.buffer = dictionaryBuffer;
	dictionaryBufferInfo.bufferSize = dictionaryBufferSize;


	NHunspell * result = new NHunspell((const char *) &affixBufferInfo, (const char *) &dictionaryBufferInfo,key_buffer); 

	if( key_buffer != 0 )
		free( key_buffer );

	return result;
}


DLLEXPORT void HunspellFree(NHunspell * handle )
{
	delete handle;
}

DLLEXPORT bool HunspellAdd(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = handle->GetWordBuffer(word);
	int success = handle->add(word_buffer);
	return success != 0;
}

DLLEXPORT bool HunspellRemove(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = handle->GetWordBuffer(word);
	int success = handle->remove(word_buffer);
	return success != 0;
}


DLLEXPORT bool HunspellAddWithAffix(NHunspell * handle, wchar_t * word, wchar_t * example )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	char * example_buffer = ((NHunspell *) handle)->GetWord2Buffer(example);
	bool success =  handle->add_with_affix(word_buffer,example_buffer) != 0;
	return success;
}

DLLEXPORT bool HunspellSpell(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	bool correct = ((NHunspell *) handle)->spell(word_buffer) != 0;
	return correct;
}

DLLEXPORT void * HunspellSuggest(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	char ** wordList;
	int  wordCount = ((NHunspell *) handle)->suggest(&wordList, word_buffer);

	// Cacculation of the Marshalling Buffer. 
    // Layout: {Pointers- zero terminated}{Wide Strings - zero terminated}
	size_t bufferSize = (wordCount + 1) * sizeof (wchar_t **);
	for( int i = 0; i < wordCount; ++ i )
		bufferSize += MultiByteToWideChar(handle->CodePage,0,wordList[i],-1,0,0) * sizeof( wchar_t);
		
		
	BYTE * marshalBuffer = (BYTE *) ((NHunspell *) handle)->GetMarshalBuffer( bufferSize );

	wchar_t ** pointer = (wchar_t ** ) marshalBuffer;
	wchar_t * buffer = (wchar_t * ) (marshalBuffer + (wordCount + 1) * sizeof (wchar_t **));
	for( int i = 0; i < wordCount; ++ i )
	{
		*pointer = buffer;
		size_t wcsSize = MultiByteToWideChar(handle->CodePage,0,wordList[i],-1,0,0);
		MultiByteToWideChar(handle->CodePage,0,wordList[i],-1,buffer,wcsSize);
		
		// Prepare pointers for the next string
		buffer += wcsSize;
		++pointer;
	}

	// Zero terminate the pointer list
	*pointer = 0;

	((NHunspell *) handle)->free_list(&wordList, wordCount);	


	return marshalBuffer;
}


DLLEXPORT void * HunspellAnalyze(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	char ** morphList;
	int  morphCount = ((NHunspell *) handle)->analyze(&morphList, word_buffer);

	// Cacculation of the Marshalling Buffer. Layout: {Pointers- zero terminated}{Wide Strings - zero terminated}
	size_t bufferSize = (morphCount + 1) * sizeof (wchar_t **);
	for( int i = 0; i < morphCount; ++ i )
		bufferSize += MultiByteToWideChar(handle->CodePage,0,morphList[i],-1,0,0) * sizeof( wchar_t);
		
		
	BYTE * marshalBuffer = (BYTE *) ((NHunspell *) handle)->GetMarshalBuffer( bufferSize );

	wchar_t ** pointer = (wchar_t ** ) marshalBuffer;
	wchar_t * buffer = (wchar_t * ) (marshalBuffer + (morphCount + 1) * sizeof (wchar_t **));
	for( int i = 0; i < morphCount; ++ i )
	{
		*pointer = buffer;
		size_t wcsSize = MultiByteToWideChar(handle->CodePage,0,morphList[i],-1,0,0);
		MultiByteToWideChar(handle->CodePage,0,morphList[i],-1,buffer,wcsSize);
		
		// Prepare pointers for the next string
		buffer += wcsSize;
		++pointer;
	}

	// Zero terminate the pointer list
	*pointer = 0;

	((NHunspell *) handle)->free_list(&morphList, morphCount);	


	return marshalBuffer;
}

DLLEXPORT void * HunspellStem(NHunspell * handle, wchar_t * word )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	char ** stemList;
	int  stemCount = ((NHunspell *) handle)->stem(&stemList, word_buffer);

	// Cacculation of the Marshalling Buffer. Layout: {Pointers- zero terminated}{Wide Strings - zero terminated}
	size_t bufferSize = (stemCount + 1) * sizeof (wchar_t **);
	for( int i = 0; i < stemCount; ++ i )
		bufferSize += MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,0,0) * sizeof( wchar_t);
		
		
	BYTE * marshalBuffer = (BYTE *) ((NHunspell *) handle)->GetMarshalBuffer( bufferSize );

	wchar_t ** pointer = (wchar_t ** ) marshalBuffer;
	wchar_t * buffer = (wchar_t * ) (marshalBuffer + (stemCount + 1) * sizeof (wchar_t **));
	for( int i = 0; i < stemCount; ++ i )
	{
		*pointer = buffer;
		size_t wcsSize = MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,0,0);
		MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,buffer,wcsSize);
		
		// Prepare pointers for the next string
		buffer += wcsSize;
		++pointer;
	}

	// Zero terminate the pointer list
	*pointer = 0;

	((NHunspell *) handle)->free_list(&stemList, stemCount);	


	return marshalBuffer;
}

DLLEXPORT void * HunspellGenerate(NHunspell * handle, wchar_t * word, wchar_t * word2 )
{
	char * word_buffer = ((NHunspell *) handle)->GetWordBuffer(word);
	char * word2_buffer = ((NHunspell *) handle)->GetWord2Buffer(word2);
	char ** stemList;
	int  stemCount = ((NHunspell *) handle)->generate(&stemList, word_buffer, word2_buffer);

	// Cacculation of the Marshalling Buffer. Layout: {Pointers- zero terminated}{Wide Strings - zero terminated}
	size_t bufferSize = (stemCount + 1) * sizeof (wchar_t **);
	for( int i = 0; i < stemCount; ++ i )
		bufferSize += MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,0,0) * sizeof( wchar_t);
		
		
	BYTE * marshalBuffer = (BYTE *) ((NHunspell *) handle)->GetMarshalBuffer( bufferSize );

	wchar_t ** pointer = (wchar_t ** ) marshalBuffer;
	wchar_t * buffer = (wchar_t * ) (marshalBuffer + (stemCount + 1) * sizeof (wchar_t **));
	for( int i = 0; i < stemCount; ++ i )
	{
		*pointer = buffer;
		size_t wcsSize = MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,0,0);
		MultiByteToWideChar(handle->CodePage,0,stemList[i],-1,buffer,wcsSize);
		
		// Prepare pointers for the next string
		buffer += wcsSize;
		++pointer;
	}

	// Zero terminate the pointer list
	*pointer = 0;

	((NHunspell *) handle)->free_list(&stemList, stemCount);	


	return marshalBuffer;
}
