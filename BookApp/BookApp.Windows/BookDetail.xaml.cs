using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BookApp.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Html;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace BookApp
{
    public sealed partial class BookDetail : Page
    {
        const string booksApi = "https://scottbookapi.herokuapp.com/api/books";
        const string bookmoochBaseApi = "http://api.bookmooch.com/api/";
        BookList bookList = new BookList();
        int textMaxLength = 1200;

        public BookDetail()
        {
            this.InitializeComponent();
            ShowProgressRing();
        }

        public void ShowProgressRing()
        {
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        public void HideProgressRing()
        {
            VisualStateManager.GoToState(this, "ResultsFound", true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter = e.Parameter as Dictionary<string, string>;

                string from = parameter["from"];
                string bookID = parameter["bookID"];

                if(from == "main")
                {
                    GetSingleBook(Convert.ToInt32(bookID), e);
                }
                else if(from == "google")
                {
                    string selfLink = parameter["selfLink"];
                    GetGoogleSingleBook(selfLink, e);
                }
                else if(from == "mooch")
                {
                    string queryText = parameter["queryText"];
                    GetMoochSingleBook(bookID, e);
                }
            }
            base.OnNavigatedTo(e);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        public async void GetSingleBook(int bookID, NavigationEventArgs e)
        {
            try
            {
                ObservableCollection<Books> books = await bookList.GetBookLists();
                var singleBook = books.Single(book => book.id == bookID);

                if(singleBook != null)
                {
                    HideProgressRing();
                    bookTitle.Text = singleBook.title;
                    bookImage.Source = new BitmapImage(new Uri(singleBook.imageurl, UriKind.Absolute));
                    bookAuthor.Text = "by " + singleBook.author;
                    bookPublishedDate.Text = " :    " + singleBook.published_date;
                    bookPublisher.Text = " :    " + singleBook.publisher;
                    bookPages.Text = " :    " + singleBook.pages + " pages";
                    bookCategories.Text = " :    " + singleBook.category__name;

                    if (!string.IsNullOrEmpty(singleBook.description))
                    {
                        bookDescription.Text = HtmlUtilities.ConvertToText(singleBook.description.Length > textMaxLength ? singleBook.description.Substring(0, textMaxLength) + "...." : singleBook.description);
                    }
                    else
                    {
                        bookDescription.Text = " - ";
                    }

                    bookPdfUrl.Text = singleBook.pdfurl;
                    if (!string.IsNullOrEmpty(singleBook.detailsurl))
                    {
                        bookDetailsUrl.Text = singleBook.detailsurl;
                        bookReadDescription.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        bookDetailsUrl.Text = "";
                    }
                    
                    bookBuyUrl.Text = "";
                    bookCommand.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowProgressRing();
                    await ShowErrorMessage("Couldnt Load The Book. Try Later", e);
                }
            }
            catch(HttpRequestException ex)
            {
                ShowProgressRing();
                await ShowErrorMessage("The app couldn't load books. Please check your connection and try again", e);
            }
        }

        public async void GetGoogleSingleBook(string selfLink, NavigationEventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(selfLink));
                var result = await response.Content.ReadAsStringAsync();

                Items singleBook = JsonConvert.DeserializeObject<Items>(result);

                if(singleBook != null)
                {
                    HideProgressRing();
                    bookTitle.Text = singleBook.volumeInfo.title;
                    if(singleBook.volumeInfo.imageLinks.medium != null)
                    {
                        bookImage.Source = new BitmapImage(new Uri(singleBook.volumeInfo.imageLinks.medium, UriKind.Absolute));
                    }
                    else
                    {
                        if(singleBook.volumeInfo.imageLinks.small != null)
                        {
                            bookImage.Source = new BitmapImage(new Uri(singleBook.volumeInfo.imageLinks.small, UriKind.Absolute));
                        }
                        else
                        {
                            if(singleBook.volumeInfo.imageLinks.thumbnail != null)
                            {
                                bookImage.Source = new BitmapImage(new Uri(singleBook.volumeInfo.imageLinks.thumbnail, UriKind.Absolute));
                            }
                            else
                            {
                                bookImage.Source = new BitmapImage(new Uri("https://islandpress.org/sites/default/files/400px%20x%20600px-r01BookNotPictured.jpg", UriKind.Absolute));
                            }
                                
                        }
                    }

                    bookAuthor.Text =  "by " + singleBook.volumeInfo.authors[0];
                    if(singleBook.volumeInfo.authors.Length > 1)
                    {
                        bookAuthor.Text += " & " + singleBook.volumeInfo.authors[1];
                    }

                    bookPublishedDate.Text = " :    " + singleBook.volumeInfo.publishedDate;
                    bookPublisher.Text = " :    " + singleBook.volumeInfo.publisher;
                    bookPages.Text = " :    " + singleBook.volumeInfo.pageCount + " pages";
                    bookRatings.Value = singleBook.volumeInfo.averageRating;
                    bookRateCount.Text = "( " + singleBook.volumeInfo.ratingsCount.ToString() + " )";

                    if (!string.IsNullOrEmpty(singleBook.volumeInfo.description))
                    {
                        bookDescription.Text = HtmlUtilities.ConvertToText(singleBook.volumeInfo.description.Length > textMaxLength ? singleBook.volumeInfo.description.Substring(0, textMaxLength) + "....." : singleBook.volumeInfo.description);
                    }
                    else
                    {
                        bookDescription.Text = " - ";
                    }

                    bookCategories.Text = " :    " + singleBook.volumeInfo.categories[0];
                    if(singleBook.volumeInfo.categories.Length > 1)
                    {
                        bookCategories.Text += ", " + singleBook.volumeInfo.categories[1];
                    }
                    if(singleBook.accessInfo.pdf.isAvailable == true)
                    {
                        if (!string.IsNullOrEmpty(singleBook.accessInfo.pdf.downloadLink))
                        {
                            bookPdfUrl.Text = singleBook.accessInfo.pdf.downloadLink;
                        }
                        else if(!string.IsNullOrEmpty(singleBook.accessInfo.pdf.acsTokenLink))
                        {
                            bookPdfUrl.Text = singleBook.accessInfo.pdf.acsTokenLink;
                        }
                    }
                    else
                    {
                        bookPdfUrl.Text = "";
                        bookRead.Visibility = Visibility.Collapsed;
                    }
                    bookDetailsUrl.Text = singleBook.volumeInfo.previewLink;
                    bookBuyUrl.Text = singleBook.volumeInfo.infoLink;
                }
                else
                {
                    ShowProgressRing();
                    await ShowErrorMessage("Couldnt Load The Book. Try Later", e);
                }
            }
            catch(HttpRequestException ex)
            {
                ShowProgressRing();
                await ShowErrorMessage("The app couldn't load books. Please check your connection and try again", e);
            }
        }

        public async void GetMoochSingleBook(string bookID, NavigationEventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(bookmoochBaseApi + "asin?asins=" + bookID + "&o=json"));
                var result = await response.Content.ReadAsStringAsync();

                MoochBooks[] singleBooks = JsonConvert.DeserializeObject<MoochBooks[]>(result);
                var singleBook = singleBooks.Single(book => book.id == bookID);

                if (singleBook != null)
                {
                    HideProgressRing();
                    bookTitle.Text = singleBook.Title;
                    bookImage.Source = new BitmapImage(new Uri(singleBook.LargeImage_URL.Replace(@"\", ""), UriKind.Absolute));
                    bookAuthor.Text = " by " + singleBook.Author;
                    bookPublishedDate.Text = " :    " + singleBook.PublicationDate;
                    bookPublisher.Text = " :    " + singleBook.Publisher;
                    bookPages.Text = " :    " + singleBook.NumberOfPages + " pages";

                    if(!string.IsNullOrEmpty(singleBook.EditorialReview_Content))
                    {
                        bookDescription.Text = HtmlUtilities.ConvertToText(singleBook.EditorialReview_Content.Length > textMaxLength ? singleBook.EditorialReview_Content.Substring(0, textMaxLength) + "....." : singleBook.EditorialReview_Content);
                    }
                    else
                    {
                        bookDescription.Text = " - ";
                    }

                    bookCategories.Text = " :    " + singleBook.Topics[0];
                    if(singleBook.Topics.Length > 1)
                    {
                        bookCategories.Text += ", " + singleBook.Topics[1];
                    }
                    bookPdfUrl.Text = "";
                    bookDetailsUrl.Text = !string.IsNullOrEmpty(singleBook.DetailPageURL) ? "" : singleBook.DetailPageURL.Replace(@"\", "");
                    bookBuyUrl.Text = singleBook.DetailPageURL.Replace(@"\", "");
                    bookRead.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowProgressRing();
                    await ShowErrorMessage("Couldnt Load The Book. Try Later", e);
                }
            }
            catch (HttpRequestException ex)
            {
                ShowProgressRing();
                await ShowErrorMessage("The app couldn't load books. Please check your connection and try again", e);
            }
        }

        public async Task ShowErrorMessage(string message, NavigationEventArgs e)
        {
            var popup = new Windows.UI.Popups.MessageDialog(message);
            popup.Commands.Add(new Windows.UI.Popups.UICommand("Try Again"));
            popup.Commands.Add(new Windows.UI.Popups.UICommand("Cancel"));

            popup.DefaultCommandIndex = 0;
            popup.CancelCommandIndex = 1;

            var results = await popup.ShowAsync();

            if (results.Label == "Try Again")
            {
                OnNavigatedTo(e);
            }
            else
            {
                this.Frame.GoBack();
            }
        }

        private async void bookCommand_Click(object sender, RoutedEventArgs e)
        {
            string buyLink = bookBuyUrl.Text;
            string title = bookTitle.Text;
            if (!string.IsNullOrEmpty(buyLink))
            {
                string from = "buy";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("link", buyLink);
                parameter.Add("from", from);
                parameter.Add("title", title);
                this.Frame.Navigate(typeof(BookWebView), parameter);
            }
            else
            {
                var popup = new Windows.UI.Popups.MessageDialog("Cannot buy this book. The link was not provided");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
            }
        }

        private async void bookRead_Click(object sender, RoutedEventArgs e)
        {
            string bookUrl = bookPdfUrl.Text;
            string title = bookTitle.Text;

            if (!string.IsNullOrEmpty(bookUrl))
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("bookUrl", bookUrl);
                parameter.Add("title", title);
                this.Frame.Navigate(typeof(BookReadView), parameter);
            }
            else
            {
                var popup = new Windows.UI.Popups.MessageDialog("Cannot Open this book. The link was not provided");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
            }
        }

        private async void bookReadDescription_Click(object sender, RoutedEventArgs e)
        {
            string detailsLink = bookDetailsUrl.Text;
            string title = bookTitle.Text;
            if (!string.IsNullOrEmpty(detailsLink))
            {
                string from = "details";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("link", detailsLink);
                parameter.Add("from", from);
                parameter.Add("title", title);
                this.Frame.Navigate(typeof(BookWebView), parameter);
            }
            else
            {
                var popup = new Windows.UI.Popups.MessageDialog("Cannot read details of this book. The link was not provided");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
            }
        }
    }
}
