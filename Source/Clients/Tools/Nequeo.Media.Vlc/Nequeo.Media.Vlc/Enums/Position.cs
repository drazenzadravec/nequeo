//    nVLC
//    
//    Author:  Roman Ginzburg
//
//    nVLC is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    nVLC is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.
//     
// ========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Media.Vlc.Enums
{
    /// <summary>
    /// Position on the video screen
    /// </summary>
    [Flags]
    public enum Position
    {
        /// <summary>
        /// 
        /// </summary>
        Center = 0,
        /// <summary>
        /// 
        /// </summary>
        Left = 1,
        /// <summary>
        /// 
        /// </summary>
        Right = 2,
        /// <summary>
        /// 
        /// </summary>
        Top = 4,
        /// <summary>
        /// 
        /// </summary>
        Bottom = 8,
        /// <summary>
        /// 
        /// </summary>
        TopRight = Top | Right,
        /// <summary>
        /// 
        /// </summary>
        TopLeft = Top | Left,
        /// <summary>
        /// 
        /// </summary>
        BottomRight = Bottom | Right,
        /// <summary>
        /// 
        /// </summary>
        BottomLeft = Bottom | Left
    }
}
