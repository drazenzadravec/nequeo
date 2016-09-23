// stdafx.cpp : source file that includes just the standard includes
// NequeoHttpBoostServer.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

#include "server_http.hpp"
#include "server_https.hpp"
#include "client_http.hpp"
#include "client_https.hpp"

typedef Nequeo::Net::Http::Boost::Server<Nequeo::Net::Http::Boost::HTTP> InternalHttpServer;
typedef Nequeo::Net::Http::Boost::Server<Nequeo::Net::Http::Boost::HTTPS> InternalSecureHttpServer;

typedef Nequeo::Net::Http::Boost::Client<Nequeo::Net::Http::Boost::HTTP> InternalHttpClient;
typedef Nequeo::Net::Http::Boost::Client<Nequeo::Net::Http::Boost::HTTPS> InternalSecureHttpClient;