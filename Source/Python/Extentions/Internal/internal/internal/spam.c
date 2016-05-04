#include <Python.h>

static PyObject *SpamError;

static PyObject* spam_system(PyObject *self, PyObject *args)
{
	const char *command;
	int sts;

	if (!PyArg_ParseTuple(args, "s", &command))
		return NULL;

	sts = system(command);

	if (sts < 0) 
	{
		PyErr_SetString(SpamError, "System command failed");
		return NULL;
	}
	return PyLong_FromLong(sts);
}

static PyMethodDef SpamMethods[] =
{
	{"system", spam_system, METH_VARARGS, "Execute a shell command."},
	{NULL, NULL, 0, NULL}        /* Sentinel */
};

static struct PyModuleDef examplemodule = {
	PyModuleDef_HEAD_INIT,
	"spam",
	"example module doc string",
	-1,
	SpamMethods,
	NULL,
	NULL,
	NULL,
	NULL
};

PyMODINIT_FUNC PyInit_spam(void)
{
	PyObject *m;

	m = PyModule_Create(&examplemodule);
	if (m == NULL)
		return;

	SpamError = PyErr_NewException("spam.error", NULL, NULL);
	Py_INCREF(SpamError);
	PyModule_AddObject(m, "error", SpamError);

	return m;
}
