"""Nequeo exception provider."""

import exceptions

# Network exceptions
class NetworkException(exceptions.Exception):
    """Network exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Permission exceptions
class PermissionException(exceptions.Exception):
    """Permission exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Async exceptions
class AsyncException(exceptions.Exception):
    """Async exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Empty string exceptions
class EmptyStringException(exceptions.Exception):
    """Empty string exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Invalid file exceptions
class InvalidFileException(exceptions.Exception):
    """Invalid file exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Invalid length exceptions
class InvalidLengthException(exceptions.Exception):
    """Invalid length exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;

# Invalid path exceptions
class InvalidPathException(exceptions.Exception):
    """Invalid path exception error handler."""

    def __init__(self, args, code = 0):
        """Constructor
           args: string, the error description.
           code: int, the error code.
        """
        self.args = args
        self.code = code

    code = 0;
    """Error code"""

    def getCode(self):
        """Gets the error code."""
        return self.code;