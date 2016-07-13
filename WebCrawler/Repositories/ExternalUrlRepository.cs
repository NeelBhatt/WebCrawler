using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Interfaces;

namespace WebCrawler.Repositories
{
    /// <summary>
    /// Class for External Urls.
    /// </summary>
    public class ExternalUrlRepository : IRepository
    {
        /// <summary>
        /// List of external Urls.
        /// </summary>
        List<string> _listOfExternalUrl;

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public ExternalUrlRepository()
        {
            _listOfExternalUrl = new List<string>();
        }

        /// <summary>
        /// List to gather Urls.
        /// </summary>
        public List<string> List
        {
            get
            {
                return _listOfExternalUrl;
            }
        }

        /// <summary>
        /// Method to add new Url.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(string entity)
        {
            _listOfExternalUrl.Add(entity);
        }
    }
}
