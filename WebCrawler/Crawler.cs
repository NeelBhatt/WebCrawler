using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WebCrawler.Interfaces;
using WebCrawler.Logging;
using WebCrawler.Repositories;

namespace WebCrawler
{
    /// <summary>
    /// Main Crawler class.
    /// </summary>
    public class Crawler
    {
        #region Private Fields

        /// <summary>
        /// external URL repository
        /// </summary>
        private IRepository _externalUrlRepository;

        /// <summary>
        /// Other URL repository
        /// </summary>
        private IRepository _otherUrlRepository;

        /// <summary>
        /// Failed URL repository
        /// </summary>
        private IRepository _failedUrlRepository;

        /// <summary>
        /// Current page URL repository
        /// </summary>
        private IRepository _currentPageUrlRepository; 

        /// <summary>
        /// List of Pages.
        /// </summary>
        private static List<Page> _pages = new List<Page>();

        /// <summary>
        /// List of exceptions.
        /// </summary>
        private static List<string> _exceptions = new List<string>();          

        /// <summary>
        /// Is current page or not
        /// </summary>
        private bool isCurrentPage = true;

        #endregion

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public Crawler(IRepository externalUrlRepository, IRepository otherUrlRepository, IRepository failedUrlRepository, IRepository currentPageUrlRepository)
        {
            _externalUrlRepository = externalUrlRepository;

            _otherUrlRepository = otherUrlRepository;

            _failedUrlRepository = failedUrlRepository;

            _currentPageUrlRepository = currentPageUrlRepository; 
        }

        /// <summary>
        /// Initializing the crawling process.
        /// </summary>
        public void InitializeCrawl()
        {
            CrawlPage(ConfigurationManager.AppSettings["url"]);
        }

        /// <summary>
        /// Initializing the reporting process.
        /// </summary>
        public void InitilizeCreateReport()
        {
            var stringBuilder = Reporting.CreateReport(_externalUrlRepository, _otherUrlRepository, _failedUrlRepository, _currentPageUrlRepository, _pages, _exceptions);

            Logging.Logging.WriteReportToDisk(stringBuilder.ToString());

            System.Diagnostics.Process.Start(ConfigurationManager.AppSettings["logTextFileName"].ToString());

            Environment.Exit(0);
        }

        /// <summary>
        /// Crawls a page.
        /// </summary>
        /// <param name="url">The url to crawl.</param>
        private void CrawlPage(string url)
        {
            if (!PageHasBeenCrawled(url))
            {
                var htmlText = GetWebText(url);
               
                var linkParser = new LinkParser();
               
                var page = new Page();
                page.Text = htmlText;
                page.Url = url;                

                _pages.Add(page);
                    
                linkParser.ParseLinks(page, url);

                //Add data to main data lists
                if (isCurrentPage)
                {
                    AddRangeButNoDuplicates(_currentPageUrlRepository.List, linkParser.ExternalUrls);
                }
               
                AddRangeButNoDuplicates(_externalUrlRepository.List, linkParser.ExternalUrls);                                               
                AddRangeButNoDuplicates(_otherUrlRepository.List, linkParser.OtherUrls);
                AddRangeButNoDuplicates(_failedUrlRepository.List, linkParser.BadUrls);      

                foreach (string exception in linkParser.Exceptions)
                    _exceptions.Add(exception);

                isCurrentPage = false;
                //Crawl all the links found on the page.
                foreach (string link in _externalUrlRepository.List)
                {                  
                    string formattedLink = link;
                    try
                    {
                        formattedLink = FixPath(url, formattedLink);

                        if (formattedLink != String.Empty)
                        {
                            CrawlPage(formattedLink);
                        }
                    }
                    catch (Exception exc)
                    {
                        _failedUrlRepository.List.Add(formattedLink + " (on page at url " + url + ") - " + exc.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Fixes a path. Makes sure it is a fully functional absolute url.
        /// </summary>
        /// <param name="originatingUrl">The url that the link was found in.</param>
        /// <param name="link">The link to be fixed up.</param>
        /// <returns>A fixed url that is fit to be fetched.</returns>
        public static string FixPath(string originatingUrl, string link)
        {
            string formattedLink = String.Empty;

            if (link.IndexOf("../") > -1)
            {
                formattedLink = ResolveRelativePaths(link, originatingUrl);
            }
            else if (originatingUrl.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) > -1
                && link.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) == -1 && !link.Contains("http:"))
            {
                formattedLink = originatingUrl.Substring(0, originatingUrl.LastIndexOf("/") + 1) + link;
            }
            else if (link.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) == -1)
            {
                formattedLink = link; //ConfigurationManager.AppSettings["url"].ToString() + 
            }

            return formattedLink;
        }

        /// <summary>
        /// Needed a method to turn a relative path into an absolute path. And this seems to work.
        /// </summary>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="originatingUrl">The url that contained the relative url.</param>
        /// <returns>A url that was relative but is now absolute.</returns>
        public static string ResolveRelativePaths(string relativeUrl, string originatingUrl)
        {
            string resolvedUrl = String.Empty;

            string[] relativeUrlArray = relativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] originatingUrlElements = originatingUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfFirstNonRelativePathElement = 0;
            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (relativeUrlArray[i] != "..")
                {
                    indexOfFirstNonRelativePathElement = i;
                    break;
                }
            }

            int countOfOriginatingUrlElementsToUse = originatingUrlElements.Length - indexOfFirstNonRelativePathElement - 1;
            for (int i = 0; i <= countOfOriginatingUrlElementsToUse - 1; i++)
            {
                if (originatingUrlElements[i] == "http:" || originatingUrlElements[i] == "https:")
                    resolvedUrl += originatingUrlElements[i] + "//";
                else
                    resolvedUrl += originatingUrlElements[i] + "/";
            }

            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (i >= indexOfFirstNonRelativePathElement)
                {
                    resolvedUrl += relativeUrlArray[i];

                    if (i < relativeUrlArray.Length - 1)
                        resolvedUrl += "/";
                }
            }

            return resolvedUrl;
        }

        /// <summary>
        /// Checks to see if the page has been crawled.
        /// </summary>
        /// <param name="url">The url that has potentially been crawled.</param>
        /// <returns>Boolean indicating whether or not the page has been crawled.</returns>
        public static bool PageHasBeenCrawled(string url)
        {
            foreach (Page page in _pages)
            {
                if (page.Url == url)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Merges a two lists of strings.
        /// </summary>
        /// <param name="targetList">The list into which to merge.</param>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private static void AddRangeButNoDuplicates(List<string> targetList, List<string> sourceList)
        {
            foreach (string str in sourceList)
            {
                if (!targetList.Contains(str))
                    targetList.Add(str);
            }
        }

        /// <summary>
        /// Gets the response text for a given url.
        /// </summary>
        /// <param name="url">The url whose text needs to be fetched.</param>
        /// <returns>The text of the response.</returns>
        public static string GetWebText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "A Web Crawler";

            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            StreamReader reader = new StreamReader(stream);
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }      
    }
}
