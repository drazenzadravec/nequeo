/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :
*  Purpose :
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "pch.h"
#include "StspDefs.h"

namespace
{
    // Function validating just a type of the data.
    template <VARTYPE vt>
    bool ValidateDataType(PROPVARIANT *pValue)
    {
        return (pValue->vt == vt);
    }

    // Function validating a BLOB apart from the type it checks the size as well.
    bool ValidateBlob(PROPVARIANT *pValue)
    {
        return ValidateDataType<MF_ATTRIBUTE_BLOB>(pValue) && pValue->caub.cElems < 128;
    }

    // Description of an attribute for validation
    struct AttributeValidationDescriptor
    {
        REFGUID guidKey;
        bool (*IsValid)(PROPVARIANT *pValue);
    };

    // Description of an media type for validation
    struct MediaTypeValidationDescriptor
    {
        GUID guidSubtype;
        bool fVideo;
        size_t cAttributes;
        const AttributeValidationDescriptor *attributes;
    };

    // Valid attibutes for H.264 decoder.
    const AttributeValidationDescriptor H264ValidAttributes[] = {
        {MF_MT_FRAME_SIZE, ValidateDataType<MF_ATTRIBUTE_UINT64>},
        {MF_MT_AVG_BITRATE, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_MPEG_SEQUENCE_HEADER, ValidateBlob},
        {MF_MT_MPEG2_PROFILE, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_FRAME_RATE, ValidateDataType<MF_ATTRIBUTE_UINT64>},
        {MF_MT_PIXEL_ASPECT_RATIO, ValidateDataType<MF_ATTRIBUTE_UINT64>},
        {MF_MT_INTERLACE_MODE, ValidateDataType<MF_ATTRIBUTE_UINT32>},
    };

    // Valid attributes for AAC decoder
    const AttributeValidationDescriptor AACValidAttributes[] = {
        {MF_MT_AUDIO_AVG_BYTES_PER_SECOND, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AVG_BITRATE, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AUDIO_BLOCK_ALIGNMENT, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AUDIO_NUM_CHANNELS, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_COMPRESSED, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AUDIO_SAMPLES_PER_SECOND, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AUDIO_PREFER_WAVEFORMATEX, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_USER_DATA, ValidateDataType<MF_ATTRIBUTE_BLOB>},
        {MF_MT_FIXED_SIZE_SAMPLES, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AAC_PAYLOAD_TYPE, ValidateDataType<MF_ATTRIBUTE_UINT32>},
        {MF_MT_AUDIO_BITS_PER_SAMPLE, ValidateDataType<MF_ATTRIBUTE_UINT32>},    
    };

    // List of valid media types.
    const MediaTypeValidationDescriptor ValidMediaTypes[] = {
        {MFVideoFormat_H264, true, _countof(H264ValidAttributes), H264ValidAttributes},
        {MFAudioFormat_AAC, false, _countof(AACValidAttributes), AACValidAttributes}
    };

    // Finds media type desciptor based on major type and subtype.
    const MediaTypeValidationDescriptor *FindMediaTypeDescriptor(REFGUID guidMajorType, REFGUID guidSubtype)
    {
        for (UINT32 nIndex = 0; nIndex < _countof(ValidMediaTypes); ++nIndex)
        {
            if (ValidMediaTypes[nIndex].guidSubtype == guidSubtype &&
                ((guidMajorType == MFMediaType_Video && ValidMediaTypes[nIndex].fVideo) ||
                (guidMajorType == MFMediaType_Audio && !ValidMediaTypes[nIndex].fVideo)))
            {
                return &ValidMediaTypes[nIndex];
            }
        }

        return nullptr;
    }

    // Checks if attribute is valid for a given media type. If it returns FALSE attrubute is optional and shouldn't be used during communication
    // between sink and source, if it returns error it means that attribute is correct but its value is somehow corrupted.
    bool IsAttributeValid(const MediaTypeValidationDescriptor *validationDescriptor, REFGUID guidKey, PROPVARIANT *pValue)
    {
        bool isValid = false;
        
        for (UINT32 nIndex = 0; nIndex < validationDescriptor->cAttributes; ++nIndex)
        {
            if (validationDescriptor->attributes[nIndex].guidKey == guidKey)
            {
                if (validationDescriptor->attributes[nIndex].IsValid(pValue))
                {
                    isValid = true;
                }
                else
                {
                   ThrowIfError(MF_E_INVALIDMEDIATYPE);
                }
                break;
            }
        }

        return isValid;
    }
}

// Filters media type's attribute before they can be sent over the network.
void Nequeo::Media::Communication::FilterOutputMediaType(IMFMediaType *pSourceMediaType, IMFMediaType *pDestinationMediaType)
{
    HRESULT hr = S_OK;
    GUID guidMajorType;
    GUID guidSubtype;
    const MediaTypeValidationDescriptor *typeDescriptor = nullptr;

    ThrowIfError(pSourceMediaType->GetMajorType(&guidMajorType));

    ThrowIfError(pSourceMediaType->GetGUID(MF_MT_SUBTYPE, &guidSubtype));

    typeDescriptor = FindMediaTypeDescriptor(guidMajorType, guidSubtype);

    // Not a valid media type
    if (typeDescriptor == nullptr)
    {
        Throw(MF_E_INVALIDMEDIATYPE);
    }

    UINT32 cAttributes = 0;
    pSourceMediaType->GetCount(&cAttributes);

    // Remove optional and not necessary attributes.
    for (UINT32 nIndex = 0; nIndex < cAttributes; ++nIndex)
    {
        GUID guidKey;
        PROPVARIANT val;
        Exception ^error = nullptr;
        PropVariantInit(&val);
        ThrowIfError(pSourceMediaType->GetItemByIndex(nIndex, &guidKey, &val));
        try
        {
            if (IsAttributeValid(typeDescriptor, guidKey, &val))
            {
                // Copy only attributes that can be sent otherwise skip it.
                ThrowIfError(pDestinationMediaType->SetItem(guidKey, val));
            }
        }
        catch(Exception ^exc)
        {
            error = exc;
        }
            
        PropVariantClear(&val);
        if (error != nullptr)
        {
            throw error;
        }
    }
}

// Used to validate media type after receiving it from the network.
void Nequeo::Media::Communication::ValidateInputMediaType(REFGUID guidMajorType, REFGUID guidSubtype, IMFMediaType *pMediaType)
{
    const MediaTypeValidationDescriptor *typeDescriptor = FindMediaTypeDescriptor(guidMajorType, guidSubtype);

    if (typeDescriptor == nullptr)
    {
        Throw(MF_E_INVALIDMEDIATYPE);
    }

    UINT32 cAttributes = 0;
    pMediaType->GetCount(&cAttributes);

    for (UINT32 nIndex = 0; nIndex < cAttributes; ++nIndex)
    {
        Exception ^error = nullptr;
        GUID guidKey;
        PROPVARIANT val;
        PropVariantInit(&val);

        ThrowIfError(pMediaType->GetItemByIndex(nIndex, &guidKey, &val));
        try
        {
            if (!IsAttributeValid(typeDescriptor, guidKey, &val))
            {
                ThrowIfError(MF_E_INVALIDMEDIATYPE);
            }
        }
        catch(Exception ^exc)
        {
            error = exc;
        }

        PropVariantClear(&val);
        if (error != nullptr)
        {
            throw error;
        }
    }
}
