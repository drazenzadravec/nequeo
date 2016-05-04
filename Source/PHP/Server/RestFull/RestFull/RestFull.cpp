// RestFull.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "PhpCommon.h"
#include "CTransit.h"

/* List of all avaliable functions */
ZEND_FUNCTION(WebInvoke);   
ZEND_FUNCTION(WebInvokeRequest); 
ZEND_FUNCTION(WebInvokeRequestGet); 

/* Compiled function list so Zend knows what's in this module */    
zend_function_entry NequeoRestFullExt_functions[] = 
{    
   ZEND_FE(WebInvoke, NULL)
   ZEND_FE(WebInvokeRequest, NULL)
   ZEND_FE(WebInvokeRequestGet, NULL)
   {NULL, NULL, NULL}
};     

/* The main entry point into to module */
zend_module_entry NequeoRestFullExt_module_entry = 
{
	#if ZEND_MODULE_API_NO >= 20010901
		STANDARD_MODULE_HEADER,
	#endif
		"Nequeo RestFull Extension",			/* extension name */
		NequeoRestFullExt_functions,			/* extension function list */
		NULL,									/* extension-wide startup function */
		NULL,									/* extension-wide shutdown function */
		NULL,									/* per-request startup function */
		NULL,									/* per-request shutdown function */
		NULL,									/* information function */
	#if ZEND_MODULE_API_NO >= 20010901
		PHP_RESTFUL_VERSION,					/* extension version number (string) */
	#endif
		STANDARD_MODULE_PROPERTIES
};

/* Implement standard "stub" routine to introduce ourselves to Zend */    
ZEND_GET_MODULE(NequeoRestFullExt)

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="message">The service channel message.</param>
ZEND_FUNCTION(WebInvoke)
{    
    char* request;
	int requestLen;
	char* requestRawData;
	int requestRawDataLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "ss", 
		&request, &requestLen, &requestRawData, &requestRawDataLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!");    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->WebInvoke(request, requestRawData);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data);  
}

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="message">The service channel message.</param>
/// <returns>The result.</returns>
ZEND_FUNCTION(WebInvokeRequest)
{    
    char* request;
	int requestLen;
	char* requestRawData;
	int requestRawDataLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "ss", 
		&request, &requestLen, &requestRawData, &requestRawDataLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!");    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->WebInvokeRequest(request, requestRawData);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data);  
}

/// <summary>
/// Web invoke request.
/// </summary>
/// <param name="message">The service channel message.</param>
/// <returns>The result.</returns>
ZEND_FUNCTION(WebInvokeRequestGet)
{    
    char* request;
	int requestLen;

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", 
		&request, &requestLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!");    
    }    
    
	// Create a new transit class to the
	// other Cpp and managed types.
	CTransit* transit = new CTransit();
	char* data = transit->WebInvokeRequestGet(request);
	
	// Delete the type.
	delete transit;
	
	// Return the string value.
    RETURN_STRING(data);  
}