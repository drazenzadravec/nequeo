// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN

#include <openssl/ssl.h>
#include <openssl/rand.h>
#include <openssl/crypto.h>
#include <openssl/err.h>
#if OPENSSL_VERSION_NUMBER >= 0x0907000L
#include <openssl/conf.h>
#endif
