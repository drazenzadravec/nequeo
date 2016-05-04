// Crypto.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

ZEND_FUNCTION(Encrypt);    

/* compiled function list so Zend knows what's in this module */    
zend_function_entry CryptoExtModule_functions[] = {    
   ZEND_FE(Encrypt, NULL)    
    {NULL, NULL, NULL}    
};     

zend_module_entry CryptoExtModule_module_entry = {
#if ZEND_MODULE_API_NO >= 20010901
    STANDARD_MODULE_HEADER,
#endif
    "Crypto Module",					/* extension name */
    CryptoExtModule_functions,			/* extension function list */
    NULL,								/* extension-wide startup function */
    NULL,								/* extension-wide shutdown function */
    NULL,								/* per-request startup function */
    NULL,								/* per-request shutdown function */
    NULL,								/* information function */
#if ZEND_MODULE_API_NO >= 20010901
    PHP_CRYPTO_VERSION,				/* extension version number (string) */
#endif
    STANDARD_MODULE_PROPERTIES
};

/* implement standard "stub" routine to introduce ourselves to Zend */    
ZEND_GET_MODULE(CryptoExtModule)    

/* Encrypt function */    
/* This method takes 1 parameter, a long value, returns    
   the value multiplied by 2 */    
ZEND_FUNCTION(Encrypt){    
    long theValue = 0;    
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC,    
                              "l", &theValue) == FAILURE){    
        RETURN_STRING("Bad parameters!");    
    }    
    theValue *= 2;    
    RETURN_LONG(theValue);    
}