using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Interfaces;

namespace WebCrawler.Logging
{
    /// <summary>
    /// Reporting class to create HTML report.
    /// </summary>
    public static class Reporting
    {
        /// <summary>
        /// Creates a report out of the data gathered.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder CreateReport(IRepository externalUrlRepository, IRepository otherUrlRepository, IRepository failedUrlRepository, IRepository currentPageUrlRepository, List<Page> pages, List<string> exceptions)
        {
            var sb = new StringBuilder();

            sb.Append("<html><head><title>Web Crawling Report</title><style>");
            sb.Append("table { border: solid 3px black; border-collapse: collapse; }");
            sb.Append("table tr th { font-weight: bold; padding: 3px; padding-left: 10px; padding-right: 10px; }");
            sb.Append("table tr td { border: solid 1px black; padding: 3px;}");
            sb.Append("h1, h2, p { font-family: Rockwell; }");
            sb.Append("p { font-family: Rockwell; font-size: smaller; }");
            sb.Append("h2 { margin-top: 45px; }");
            sb.Append("</style></head><body>");
            sb.Append("<h1>Crawl Report</h1>");

            sb.Append("<h2>Internal Urls - In Order Crawled</h2>");
            sb.Append("<p>These are the links found within the site. This is the order in which they were crawled.</p>");

            sb.Append("<table><tr><th>Sr. No.</th><th>Url</th></tr>");
            var counter = 1;
            foreach (var page in currentPageUrlRepository.List)
            {
                sb.Append("<tr><td>");
                sb.Append(counter++);              
                sb.Append("</td><td>");
                sb.Append(page);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>External Urls</h2>");
            sb.Append("<p>These are the links to the pages which are inside as well as outside the site which are crawled one by one.</p>");

            sb.Append("<table><tr><th> Sr. No. </th><th>Url</th></tr>");
            var counter2 = 1;
            foreach (string str in externalUrlRepository.List)
            {
                sb.Append("<tr><td>");
                sb.Append(counter2++);
                sb.Append("</td><td>");
                sb.Append(str);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>Other Urls</h2>");
            sb.Append("<p>These are the links to things on the site that are not html files (html, aspx, etc.), like images and css files. If you do not have permission to view some websites then this will fall in this category.</p>");

            sb.Append("<table><tr><th> Sr. No. </th><th>Url</th></tr>");
            var counter3 = 1;
            foreach (string str in failedUrlRepository.List)
            {
                sb.Append("<tr><td>");
                sb.Append(counter3++);
                sb.Append("</td><td>");              
                sb.Append(str);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>Bad Urls</h2>");
            sb.Append("<p>Any bad urls will be listed here.</p>");

            sb.Append("<table><tr><th> Sr. No. </th><th>Url</th></tr>");
            var counter4 = 1;
            if (failedUrlRepository.List.Count > 0)
            {
                foreach (string str in failedUrlRepository.List)
                {
                    sb.Append("<tr><td>");
                    sb.Append(counter4++);
                    sb.Append("</td><td>");                         
                    sb.Append(str);
                    sb.Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td>0</td><td>No bad urls.</td></tr>");
            }

            sb.Append("</table>");


            sb.Append("<h2>Exceptions</h2>");
            sb.Append("<p>Any exceptions that were thrown would be shown here.</p>");

            sb.Append("<table><tr><th> Sr. No. </th><th>Exception</th></tr>");
            var counter5 = 1;
            if (exceptions.Count > 0)
            {
                foreach (string str in exceptions)
                {
                    sb.Append("<tr><td>");
                    sb.Append(counter5++);
                    sb.Append("</td><td>");                     
                    sb.Append(str);
                    sb.Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td>0</td><td>No exceptions thrown.</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("</body></html>");
            return sb;
        }    
    }
}
