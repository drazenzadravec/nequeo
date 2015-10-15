/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Search.Microsoft
{
    /// <summary>
    /// ExpandableSearchResult
    /// </summary>
    public partial class ExpandableSearchResult
    {

        private Guid _ID;

        private Int64? _WebTotal;

        private Int64? _WebOffset;

        private Int64? _ImageTotal;

        private Int64? _ImageOffset;

        private Int64? _VideoTotal;

        private Int64? _VideoOffset;

        private Int64? _NewsTotal;

        private Int64? _NewsOffset;

        private Int64? _SpellingSuggestionsTotal;

        private String _AlteredQuery;

        private String _AlterationOverrideQuery;

        private System.Collections.ObjectModel.Collection<WebResult> _Web;

        private System.Collections.ObjectModel.Collection<ImageResult> _Image;

        private System.Collections.ObjectModel.Collection<VideoResult> _Video;

        private System.Collections.ObjectModel.Collection<NewsResult> _News;

        private System.Collections.ObjectModel.Collection<RelatedSearchResult> _RelatedSearch;

        private System.Collections.ObjectModel.Collection<SpellResult> _SpellingSuggestions;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? WebTotal
        {
            get
            {
                return this._WebTotal;
            }
            set
            {
                this._WebTotal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? WebOffset
        {
            get
            {
                return this._WebOffset;
            }
            set
            {
                this._WebOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? ImageTotal
        {
            get
            {
                return this._ImageTotal;
            }
            set
            {
                this._ImageTotal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? ImageOffset
        {
            get
            {
                return this._ImageOffset;
            }
            set
            {
                this._ImageOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? VideoTotal
        {
            get
            {
                return this._VideoTotal;
            }
            set
            {
                this._VideoTotal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? VideoOffset
        {
            get
            {
                return this._VideoOffset;
            }
            set
            {
                this._VideoOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? NewsTotal
        {
            get
            {
                return this._NewsTotal;
            }
            set
            {
                this._NewsTotal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? NewsOffset
        {
            get
            {
                return this._NewsOffset;
            }
            set
            {
                this._NewsOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? SpellingSuggestionsTotal
        {
            get
            {
                return this._SpellingSuggestionsTotal;
            }
            set
            {
                this._SpellingSuggestionsTotal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String AlteredQuery
        {
            get
            {
                return this._AlteredQuery;
            }
            set
            {
                this._AlteredQuery = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String AlterationOverrideQuery
        {
            get
            {
                return this._AlterationOverrideQuery;
            }
            set
            {
                this._AlterationOverrideQuery = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<WebResult> Web
        {
            get
            {
                return this._Web;
            }
            set
            {
                this._Web = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<ImageResult> Image
        {
            get
            {
                return this._Image;
            }
            set
            {
                this._Image = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<VideoResult> Video
        {
            get
            {
                return this._Video;
            }
            set
            {
                this._Video = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<NewsResult> News
        {
            get
            {
                return this._News;
            }
            set
            {
                this._News = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<RelatedSearchResult> RelatedSearch
        {
            get
            {
                return this._RelatedSearch;
            }
            set
            {
                this._RelatedSearch = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.ObjectModel.Collection<SpellResult> SpellingSuggestions
        {
            get
            {
                return this._SpellingSuggestions;
            }
            set
            {
                this._SpellingSuggestions = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class WebResult
    {

        private Guid _ID;

        private String _Title;

        private String _Description;

        private String _DisplayUrl;

        private String _Url;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String DisplayUrl
        {
            get
            {
                return this._DisplayUrl;
            }
            set
            {
                this._DisplayUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class ImageResult
    {

        private Guid _ID;

        private String _Title;

        private String _MediaUrl;

        private String _SourceUrl;

        private String _DisplayUrl;

        private Int32? _Width;

        private Int32? _Height;

        private Int64? _FileSize;

        private String _ContentType;

        private Thumbnail _Thumbnail;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String MediaUrl
        {
            get
            {
                return this._MediaUrl;
            }
            set
            {
                this._MediaUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String SourceUrl
        {
            get
            {
                return this._SourceUrl;
            }
            set
            {
                this._SourceUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String DisplayUrl
        {
            get
            {
                return this._DisplayUrl;
            }
            set
            {
                this._DisplayUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                this._Width = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? FileSize
        {
            get
            {
                return this._FileSize;
            }
            set
            {
                this._FileSize = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ContentType
        {
            get
            {
                return this._ContentType;
            }
            set
            {
                this._ContentType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Thumbnail Thumbnail
        {
            get
            {
                return this._Thumbnail;
            }
            set
            {
                this._Thumbnail = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class VideoResult
    {

        private Guid _ID;

        private String _Title;

        private String _MediaUrl;

        private String _DisplayUrl;

        private Int32? _RunTime;

        private Thumbnail _Thumbnail;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String MediaUrl
        {
            get
            {
                return this._MediaUrl;
            }
            set
            {
                this._MediaUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String DisplayUrl
        {
            get
            {
                return this._DisplayUrl;
            }
            set
            {
                this._DisplayUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? RunTime
        {
            get
            {
                return this._RunTime;
            }
            set
            {
                this._RunTime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Thumbnail Thumbnail
        {
            get
            {
                return this._Thumbnail;
            }
            set
            {
                this._Thumbnail = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class NewsResult
    {

        private Guid _ID;

        private String _Title;

        private String _Url;

        private String _Source;

        private String _Description;

        private DateTime? _Date;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                this._Source = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? Date
        {
            get
            {
                return this._Date;
            }
            set
            {
                this._Date = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class RelatedSearchResult
    {

        private Guid _ID;

        private String _Title;

        private String _BingUrl;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String BingUrl
        {
            get
            {
                return this._BingUrl;
            }
            set
            {
                this._BingUrl = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class SpellResult
    {

        private Guid _ID;

        private String _Value;

        /// <summary>
        /// 
        /// </summary>
        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Thumbnail
    {

        private String _MediaUrl;

        private String _ContentType;

        private Int32? _Width;

        private Int32? _Height;

        private Int64? _FileSize;

        /// <summary>
        /// 
        /// </summary>
        public String MediaUrl
        {
            get
            {
                return this._MediaUrl;
            }
            set
            {
                this._MediaUrl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ContentType
        {
            get
            {
                return this._ContentType;
            }
            set
            {
                this._ContentType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                this._Width = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64? FileSize
        {
            get
            {
                return this._FileSize;
            }
            set
            {
                this._FileSize = value;
            }
        }
    }
}
