#pragma once

#ifdef NEQUEOANDROIDPJSIP_EXPORTS
#define EXPORT_NEQUEO_ANDROID_PJSIP_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_ANDROID_PJSIP_API __declspec(dllimport) 
#endif