#pragma once

#ifdef NEQUEOUWPPJSIP_EXPORTS
#define EXPORT_NEQUEO_UWP_PJSIP_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_UWP_PJSIP_API __declspec(dllimport) 
#endif