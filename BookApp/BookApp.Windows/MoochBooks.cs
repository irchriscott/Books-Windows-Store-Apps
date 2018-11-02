using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookApp
{
    public class MoochBooks
    {
        public string id { set; get; }
        public string Author { set; get; }
        public string Binding { set; get; }
        public string DetailPageURL { set; get; }
        public string EditorialReview_Content { set; get; }
        public string EditorialReview_Source { set; get; }
        public string ISBN { set; get; }
        [System.ComponentModel.DefaultValue("https://islandpress.org/sites/default/files/400px%20x%20600px-r01BookNotPictured.jpg")]
        public string LargeImage_URL { set; get; } = "https://islandpress.org/sites/default/files/400px%20x%20600px-r01BookNotPictured.jpg";
        public string ListPrice_CurrencyCode { set; get; }
        public string ListPrice_FormattedPrice { set; get; }
        public string MediumImage_URL { set; get; } = "https://islandpress.org/sites/default/files/400px%20x%20600px-r01BookNotPictured.jpg";
        public string NumberOfPages { set; get; }
        public string PublicationDate { set; get; }
        public string Publisher { set; get; }
        public string SmallImage_URL { set; get; }
        public string store { set; get; }
        public string Title { set; get; }
        [System.ComponentModel.DefaultValue("Others")]
        public string[] Topics { set; get; } = { "Others" };
    }
}
