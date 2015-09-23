/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Nequeo Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://Nequeo.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.ComponentModel;
using System.Windows;

namespace Nequeo.Wpf.Wpf.Markup
{
  [Browsable( false )]
  [EditorBrowsable( EditorBrowsableState.Never )]
  public sealed class NequeoResourceDictionary : ResourceDictionary
  {
    #region NequeoSource Property

    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly" )]
    public string NequeoSource
    {

      get
      {
        return m_NequeoSource;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "NequeoSource" );

        m_NequeoSource = value;

        string[] parsedArguments = m_NequeoSource.Split( new char[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries );

        if( parsedArguments.Length != 2 )
          throw new ArgumentException( "Invalid URI format.", "NequeoSource" );

        string uriString =  parsedArguments[ 0 ] + 
          ";;;" + parsedArguments[ 1 ];

        this.Source = new Uri( uriString, UriKind.RelativeOrAbsolute );
      }
    }

    private string m_NequeoSource = string.Empty;

    #endregion
  }
}
