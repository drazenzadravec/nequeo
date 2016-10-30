/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using Nequeo.IO.Audio.Api.Interfaces;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyStoreProperty
    {
        private PropertyKey _PropertyKey;
        private PropVariant _PropValue;

        internal PropertyStoreProperty(PropertyKey key, PropVariant value)
        {
            _PropertyKey = key;
            _PropValue = value;
        }

        public PropertyKey Key
        {
            get
            {
                return _PropertyKey;
            }
        }

        public object Value
        {
            get
            {
                return _PropValue.Value;
            }
        }
    }
}
