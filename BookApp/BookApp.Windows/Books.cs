using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookApp
{
    public class Books
    {
        public int id { set; get; }
        public long isbn { set; get; }
        public string title { set; get; }
        public string author { set; get; }
        public string description { set; get; }
        public string category__name { set; get; }
        public string imageurl { set; get; }
        public string pdfurl { set; get; }
        public string detailsurl { set; get; }
        public Nullable<int> pages { set; get; }
        public string published_date { set; get; }
        public string publisher { set; get; }
    }
}
