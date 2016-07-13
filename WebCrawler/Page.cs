using System;

namespace WebCrawler
{
    /// <summary>
    /// Page class
    /// </summary>
    public class Page
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Page() { }

        #endregion
        #region Private Instance Fields

        private int _size;
        private string _text;
        private string _url;
        private int _viewstateSize;

        #endregion
        #region Public Properties

        public int Size
        {
            get { return _size; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _size = value.Length;
            }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }  
        #endregion
    }
}
