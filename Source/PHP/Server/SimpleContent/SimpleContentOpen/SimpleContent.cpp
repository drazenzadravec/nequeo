// SimpleContent.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

ZEND_FUNCTION(DoubleUp);    

/* compiled function list so Zend knows what's in this module */    
zend_function_entry CustomExtModule_functions[] = {    
   ZEND_FE(DoubleUp, NULL)    
    {NULL, NULL, NULL}    
};     

zend_module_entry CustomExtModule_module_entry = {
#if ZEND_MODULE_API_NO >= 20010901
    STANDARD_MODULE_HEADER,
#endif
    "CustomExt Module",					/* extension name */
    CustomExtModule_functions,			/* extension function list */
    NULL,								/* extension-wide startup function */
    NULL,								/* extension-wide shutdown function */
    NULL,								/* per-request startup function */
    NULL,								/* per-request shutdown function */
    NULL,								/* information function */
#if ZEND_MODULE_API_NO >= 20010901
    PHP_SIMPLECONTENT_VERSION,				/* extension version number (string) */
#endif
    STANDARD_MODULE_PROPERTIES
};

/* implement standard "stub" routine to introduce ourselves to Zend */    
ZEND_GET_MODULE(CustomExtModule)    

/* DoubleUp function */    
/* This method takes 1 parameter, a long value, returns    
   the value multiplied by 2 */    
ZEND_FUNCTION(DoubleUp){    
    long theValue = 0;    
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC,    
                              "l", &theValue) == FAILURE){    
        RETURN_STRING("Bad parameters!");    
    }    
    theValue *= 2;    
    RETURN_LONG(theValue);    
}
