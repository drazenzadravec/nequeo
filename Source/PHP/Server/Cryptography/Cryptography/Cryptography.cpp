// Cryptography.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "PhpCommon.h"
#include "CTransit.h"

/* List of all avaliable functions */
ZEND_FUNCTION(Encrypt);   
ZEND_FUNCTION(Decrypt); 
ZEND_FUNCTION(GetHashcode);   
ZEND_FUNCTION(GetHashcodeMD5); 
ZEND_FUNCTION(GetHashcodeSHA1);

/* Compiled function list so Zend knows what's in this module */    
zend_function_entry NequeoCryptographyExt_functions[] = 
{    
   ZEND_FE(Encrypt, NULL)
   ZEND_FE(Decrypt, NULL)
   ZEND_FE(GetHashcode, NULL)
   ZEND_FE(GetHashcodeMD5, NULL)
   ZEND_FE(GetHashcodeSHA1, NULL)
   {NULL, NULL, NULL}
};     

/* The main entry point into to module */
zend_module_entry NequeoCryptographyExt_module_entry = 
{
	#if ZEND_MODULE_API_NO >= 20010901
		STANDARD_MODULE_HEADER,
	#endif
		"Nequeo Cryptography Extension",		/* extension name */
		NequeoCryptographyExt_functions,		/* extension function list */
		NULL,									/* extension-wide startup function */
		NULL,									/* extension-wide shutdown function */
		NULL,									/* per-request startup function */
		NULL,									/* per-request shutdown function */
		NULL,									/* information function */
	#if ZEND_MODULE_API_NO >= 20010901
		PHP_CRYPTOGRAPHY_VERSION,				/* extension version number (string) */
	#endif
		STANDARD_MODULE_PROPERTIES
};

/* Implement standard "stub" routine to introduce ourselves to Zend */    
ZEND_GET_MODULE(NequeoCryptographyExt)    

///	<summary>
///	Encrypt data.
///	</summary>
ZEND_FUNCTION(Encrypt)
{    
    char* encrypt;
	int nameLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", &encrypt, &nameLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->Encrypt(encrypt);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data, 1);  
}

///	<summary>
///	Decrypt data.
///	</summary>
ZEND_FUNCTION(Decrypt)
{      
	char* decrypt;
	int nameLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", &decrypt, &nameLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->Decrypt(decrypt);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data, 1);    
}

///	<summary>
///	Get hashcode for a specific code type.
///	</summary>
ZEND_FUNCTION(GetHashcode)
{      
	char* item;
	int itemLen;
	long codeType;

	// Pass string, length and hash code type, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "sl", &item, &itemLen, &codeType) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->GetHashcode(item, codeType);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data, 1);    
}

///	<summary>
///	Get hashcode MD5.
///	</summary>
ZEND_FUNCTION(GetHashcodeMD5)
{      
	char* item;
	int itemLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", &item, &itemLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->GetHashcodeMD5(item);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data, 1);    
}

///	<summary>
///	Get hashcode SHA1.
///	</summary>
ZEND_FUNCTION(GetHashcodeSHA1)
{      
	char* item;
	int itemLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", &item, &itemLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->GetHashcodeSHA1(item);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data, 1);    
}
