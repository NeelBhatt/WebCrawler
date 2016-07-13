using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Interfaces;

namespace WebCrawler.Repositories
{
    /// <summary>
    /// Class for Failed Urls.
    /// </summary>
    public class FailedUrlRepository : IRepository
    {
        /// <summary>
        /// List of failed Urls.
        /// </summary>
        List<string> _listOfFailedUrl;

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public FailedUrlRepository()
        {
            _listOfFailedUrl = new List<string>();
        }

        /// <summary>
        /// List to gather Urls.
        /// </summary>
        public List<string> List
        {
            get
            {
                return _listOfFailedUrl;
            }
        }

        /// <summary>
        /// Method to add new Url.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(string entity)
        {
            _listOfFailedUrl.Add(entity);
        }
    }
}
