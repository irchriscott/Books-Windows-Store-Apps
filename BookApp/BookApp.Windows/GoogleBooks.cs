using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookApp
{
    public class GoogleBooks
    {
        public string kind { set; get; }
        public int totalItems { set; get; }
        public ObservableCollection<Items> items { set; get; }

        public GoogleBooks()
        {
            this.items = new ObservableCollection<Items>();
        }
    }

    public class Items
    {
        public string kind { set; get; }
        public string id { set; get; }
        public string etag { set; get; }
        public string selfLink { set; get; }
        public VolumeInfo volumeInfo { set; get; }
        public SaleInfo saleInfo { set; get; }
        public AccessInfo accessInfo { set; get; }
    }

    public class VolumeInfo
    {
        public string title { set; get; }
        public string subtitle { set; get; }
        public string[] authors { set; get; } = { "Unknown" };
        public string publisher { set; get; }
        public string publishedDate { set; get; }
        public string description { set; get; }
        public IndustryIdentifiers[] industryIdentifiers { set; get; }
        public int pageCount { set; get; }
        public Dimensions dimensions { set; get; }
        public string printType { set; get; }
        [System.ComponentModel.DefaultValue("Others")]
        public string[] categories { set; get; } = { "Others" };
        public double averageRating { set; get; }
        public int ratingsCount { set; get; }
        public string contentVersion { set; get; }
        public ImageLinks imageLinks { set; get; }
        public string language { set; get; }
        public string previewLink { set; get; }
        public string infoLink { set; get; }
        public string canonicalVolumeLink { set; get; }
    }

    public class IndustryIdentifiers
    {
        public string type { set; get; }
        public string identifier { set; get; }
    }
    public class Dimensions
    {
        public string height { set; get; }
        public string width { set; get; }
        public string thickness { set; get; }
    }
    public class ImageLinks
    {
        public string smallThumbnail { set; get; }
        public string thumbnail { set; get; }
        public string small { set; get; }
        public string medium { set; get; }
        public string large { set; get; }
        public string extraLarge { set; get; }
    }

    public class SaleInfo
    {
        public string country { set; get; }
        public string saleability { set; get; }
        public DateTime onSaleDate { set; get; }
        public Boolean isEbook { set; get; }
        public ListPrice listPrice { set; get; }
        public RetailPrice retailPrice { set; get; }
        public string buyLink { set; get; }
    }

    public class ListPrice
    {
        public double amount { set; get; }
        public string currencyCode { set; get; }
    }
    public class RetailPrice
    {
        public double amount { set; get; }
        public string currencyCode { set; get; }
    }

    public class AccessInfo
    {
        public string country { set; get; }
        public string viewability { set; get; }
        public Boolean embeddable { set; get; }
        public Boolean publicDomain { set; get; }
        public string textToSpeechPermission { set; get; }
        public Epub epub { set; get; }
        public PDF pdf { set; get; }
        public string webReaderLink { set; get; }
        public string accessViewStatus { set; get; }
    }
    
    public class Epub
    {
        public Boolean isAvailable { set; get; }
        public string downloadLink { set; get; }
        public string acsTokenLink { set; get; }
    }

    public class PDF
    {
        public Boolean isAvailable { set; get; }
        public string downloadLink { set; get; }
        public string acsTokenLink { set; get; }
    }
}
