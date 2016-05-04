// Serialisation.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "PhpCommon.h"

// Instance indicator.
static int le_instance = 0;

// Define internal functions
static void delete_instance(void *instance);
static void serialise_error_handler(int error_num, const char *error_filename, const uint error_lineno, const char *format, va_list args);

// Define the class entry point
static zend_class_entry* serialise_class_entry;

// Define the error handler function pointer.
static void (*old_error_handler)(int, const char *, const uint, const char*, va_list);

// Define the serialisation class name when user creates a new instance.
#define PHP_SERIALISATION_CLASSNAME "Serialse"

// Define the PHP initialisation functions.
PHP_RINIT_FUNCTION(serialise);
PHP_MINIT_FUNCTION(serialise);
PHP_MSHUTDOWN_FUNCTION(serialise);
PHP_MINFO_FUNCTION(serialise);

// List of all avaliable functions
PHP_FUNCTION(use_serialise_error_handler);

// Serialisation Functions
PHP_METHOD(Serialse, Serialse);
PHP_METHOD(Serialse, SerialiseXml);
PHP_METHOD(Serialse, SerialiseJson);
PHP_METHOD(Serialse, DeserialiseXml);
PHP_METHOD(Serialse, DeserialiseJson);

// Define the constructor initialser.
#define SERIALISATION_CTOR(class_name, func_name, arginfo, flags) PHP_ME(class_name, func_name, arginfo, flags)

// Define the function parameter list
ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_serialisation, 0, 0, 0)
ZEND_END_ARG_INFO()

ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_serialisexml, 0, 0, 1)
	ZEND_ARG_INFO(0, value)
ZEND_END_ARG_INFO()

ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_serialisejson, 0, 0, 1)
	ZEND_ARG_INFO(0, value)
ZEND_END_ARG_INFO()

ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_deserialisexml, 0, 0, 1)
	ZEND_ARG_INFO(0, value)
ZEND_END_ARG_INFO()

ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_deserialisejson, 0, 0, 1)
	ZEND_ARG_INFO(0, value)
ZEND_END_ARG_INFO()

ZEND_BEGIN_ARG_INFO_EX(arginfo_serialisation_use_serialise_error_handler, 0, 0, 0)
	ZEND_ARG_INFO(0, handler)
ZEND_END_ARG_INFO()

// Compiled function list for the module.
static const zend_function_entry serialiseerror_functions[] = {
	PHP_FE(use_serialise_error_handler, 	arginfo_serialisation_use_serialise_error_handler)
	{NULL, NULL, NULL}
};

// Compiled function serialisation list for the instance.
static const zend_function_entry serialise_functions[] = 
{    
	SERIALISATION_CTOR(Serialse, Serialse, 	arginfo_serialisation_serialisation, 0)
	PHP_ME(Serialse, SerialiseXml, 			arginfo_serialisation_serialisexml, 0)
	PHP_ME(Serialse, SerialiseJson, 		arginfo_serialisation_serialisejson, 0)
	PHP_ME(Serialse, DeserialiseXml, 		arginfo_serialisation_deserialisexml, 0)
	PHP_ME(Serialse, DeserialiseJson, 		arginfo_serialisation_deserialisejson, 0)
   {NULL, NULL, NULL}
};     

// The main entry point into to module.
zend_module_entry serialise_module_entry = 
{
	#if ZEND_MODULE_API_NO >= 20010901
		STANDARD_MODULE_HEADER,
	#endif
		"Nequeo Serialisation Extension",		/* extension name */
		serialiseerror_functions,				/* extension function list */
		PHP_MINIT(serialise),					/* extension-wide startup function */
		PHP_MSHUTDOWN(serialise),				/* extension-wide shutdown function */
		PHP_RINIT(serialise),					/* per-request startup function */
		NULL,									/* per-request shutdown function */
		PHP_MINFO(serialise),					/* information function */
	#if ZEND_MODULE_API_NO >= 20010901
		PHP_SERIALISATION_VERSION,				/* extension version number (string) */
	#endif
		STANDARD_MODULE_PROPERTIES
};

// Implement standard "stub" routine to introduce ourselves to Zend.
ZEND_GET_MODULE(serialise)    

///	<summary>
///	PHP shoutdown.
///	</summary>
PHP_MSHUTDOWN_FUNCTION(serialise)
{
	zend_error_cb = old_error_handler;
	UNREGISTER_INI_ENTRIES();
	return SUCCESS;
}

///	<summary>
///	PHP release function
///	</summary>
PHP_RINIT_FUNCTION(serialise)
{
	SERIALISE_GLOBAL(use_serialise_error_handler) = 0;
	SERIALISE_GLOBAL(error_code) = NULL;
	SERIALISE_GLOBAL(error_object) = NULL;
	return SUCCESS;
}

///	<summary>
///	PHP start function.
///	</summary>
PHP_MINIT_FUNCTION(serialise)
{
	zend_class_entry ce;

	// CSerialisation class
	INIT_CLASS_ENTRY(ce, PHP_SERIALISATION_CLASSNAME, serialise_functions);
	serialise_class_entry = zend_register_internal_class(&ce TSRMLS_CC);

	// Register list of destructors
	le_instance = register_list_destructors(delete_instance, NULL);

	// Set the error handler.
	old_error_handler = zend_error_cb;
	zend_error_cb = serialise_error_handler;

	return SUCCESS;
}

///	<summary>
///	PHP information function.
///	</summary>
PHP_MINFO_FUNCTION(serialise)
{
	php_info_print_table_start();
	php_info_print_table_end();
	DISPLAY_INI_ENTRIES();
}

///	<summary>
///	Serialise error handler
///	</summary>
PHP_FUNCTION(use_serialise_error_handler)
{
	// The bool indicating if an error handler is to be used.
	zend_bool handler = 1;

	ZVAL_BOOL(return_value, SERIALISE_GLOBAL(use_serialise_error_handler));
	if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "|b", &handler) == SUCCESS) {
		SERIALISE_GLOBAL(use_serialise_error_handler) = handler;
	}
}

///	<summary>
///	Serialise contructor.
///	</summary>
PHP_METHOD(Serialse, Serialse)
{
	serialiseInstancePtr instance = NULL;
	Serialse* ser = NULL;
	int ret;
	
	// Get begin code.
	SERIALISATION_BEGIN_CODE();

	// Allocate the memory for the instance.
	instance = (serialiseInstancePtr)emalloc(sizeof(serialiseInstance));
	memset(instance, 0, sizeof(serialiseInstance));
	
	// Assign the properies.
	instance->std.ce = serialise_class_entry;
	instance->send_errors = 1;

	// Create a new instance of the serialisation class.
	instance->serialse = new Serialse();
	
	// Allocate memory to the properties.
	ALLOC_HASHTABLE(instance->std.properties);
	zend_hash_init(instance->std.properties, 0, NULL, ZVAL_PTR_DTOR, 0);

	// Add the property list.
	ret = zend_list_insert(instance, le_instance TSRMLS_CC);
	add_property_resource(this_ptr, "instance", ret);

	// Get end code.
	SERIALISATION_END_CODE();
}

///	<summary>
///	Serialise error handler.
///	</summary>
/// <param name="value">The type to serialise.</param>
/// <returns>The a serialised xml.</returns>
PHP_METHOD(Serialse, SerialiseXml)
{
	serialiseInstancePtr instance = NULL;
	char* xml;
	int xmlLen;

	// Get begin code.
	SERIALISATION_BEGIN_CODE();
	FETCH_THIS_INSTANCE(instance);

	// Pass string and length, if incorrect number
	// of parameters.
    if (zend_parse_parameters(ZEND_NUM_ARGS() TSRMLS_CC, "s", &xml, &xmlLen) == FAILURE)
	{    
        RETURN_STRING("Bad parameters!", true);    
    }

	// Get the serialise instance.
	Serialse* serl = instance->serialse;
	char* data = serl->SerialiseXml(xml);

	// Get end code.
	SERIALISATION_END_CODE();

	// Return the value.
	RETURN_STRING(data, 1);
}

PHP_METHOD(Serialse, SerialiseJson)
{
	serialiseInstancePtr instance = NULL;
}

PHP_METHOD(Serialse, DeserialiseXml)
{
	serialiseInstancePtr instance = NULL;
}

PHP_METHOD(Serialse, DeserialiseJson)
{
	serialiseInstancePtr instance = NULL;
}

///	<summary>
///	Serialise error handler.
///	</summary>
/// <param name="error_num">The error number.</param>
/// <param name="error_filename">The error file name.</param>
/// <param name="error_lineno">The error line number.</param>
/// <param name="format">The error format.</param>
/// <param name="args">The list of arguments.</param>
/// <returns>The encrypted value.</returns>
static void serialise_error_handler(int error_num, const char *error_filename, const uint error_lineno, const char *format, va_list args)
{
}

///	<summary>
///	Delete the serialise resource.
///	</summary>
/// <param name="data">The instance structure.</param>
static void delete_instance(void *data)
{
	// Get the instance pointer.
	serialiseInstancePtr instance = (serialiseInstancePtr)data;

	// Delete the serialse instance
	delete instance->serialse;
	
	// Free the properties hashtable.
	zend_hash_destroy(instance->std.properties);
    FREE_HASHTABLE(instance->std.properties);

	// Free the instance pointer.
	instance->serialse = NULL;
	efree(instance);
}