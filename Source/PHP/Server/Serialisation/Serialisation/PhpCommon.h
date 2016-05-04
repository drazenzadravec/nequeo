#pragma once

#define PHP_SERIALISATION_VERSION "1.0"
#define PHP_COMPILER_ID "VC14"

#ifndef _PHPCOMMON_
#define _PHPCOMMON_

/* PHP Extension headers */    
/* include zend win32 config first */    
#include "zend_config.w32.h"    

/* include standard header */    
#include "php.h"
#include "php_globals.h"
#include "ext/standard/info.h"
#include "ext/standard/php_standard.h"

#include "CSerialisation.h"

/* Declare serialisation structure container. */
typedef struct _serialiseInstance serialiseInstance, *serialiseInstancePtr;

/* Serialisation instance struct */
struct _serialiseInstance {
	// Standard properties.
	zend_object std;

	// The serialisation instance.
    Serialse* serialse;

	// Error code.
	int send_errors;
};

/* Declare globals */
ZEND_BEGIN_MODULE_GLOBALS(serialise)
	zend_bool  use_serialise_error_handler;
	char*      error_code;
	zval*      error_object;
ZEND_END_MODULE_GLOBALS(serialise)

#define SERIALISE_GLOBAL(v) (serialise_globals.v)

#define SERIALISATION_BEGIN_CODE() \
	zend_bool _old_handler = SERIALISE_GLOBAL(use_serialise_error_handler);\
	char* _old_error_code = SERIALISE_GLOBAL(error_code);\
	zval* _old_error_object = SERIALISE_GLOBAL(error_object);\
	SERIALISE_GLOBAL(use_serialise_error_handler) = 1;\
	SERIALISE_GLOBAL(error_code) = "Serialisation";\
	SERIALISE_GLOBAL(error_object) = this_ptr;

#define SERIALISATION_END_CODE() \
	SERIALISE_GLOBAL(use_serialise_error_handler) = _old_handler;\
	SERIALISE_GLOBAL(error_code) = _old_error_code;\
	SERIALISE_GLOBAL(error_object) = _old_error_object;\

#define FETCH_THIS_INSTANCE(insta) \
	{ \
		zval **tmp; \
		if (zend_hash_find(Z_OBJPROP_P(this_ptr),"instance", sizeof("instance"), (void **)&tmp) != FAILURE) { \
			insta = (serialiseInstancePtr)zend_fetch_resource(tmp TSRMLS_CC, -1, "instance", NULL, 1, le_instance); \
		} else { \
			insta = NULL; \
		} \
	}

ZEND_DECLARE_MODULE_GLOBALS(serialise)

#ifdef va_copy
#define call_old_error_handler(error_num, error_filename, error_lineno, format, args) \
{ \
	va_list copy; \
	va_copy(copy, args); \
	old_error_handler(error_num, error_filename, error_lineno, format, copy); \
	va_end(copy); \
}
#else
#define call_old_error_handler(error_num, error_filename, error_lineno, format, args) \
{ \
	old_error_handler(error_num, error_filename, error_lineno, format, args); \
}
#endif

#endif