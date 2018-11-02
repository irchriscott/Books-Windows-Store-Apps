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
using System.Net;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace BookApp
{
    public sealed partial class GoogleBooksList : Page
    {
        const string googleBaseApi = "https://www.googleapis.com/books/v1/";

        public GoogleBooksList()
        {
            this.InitializeComponent();
            ShowProgressRing();
        }

        public void ShowProgressRing()
        {
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter = args.Parameter as Dictionary<string, string>;
                int index = Convert.ToInt32(parameter["index"]);
                string queryText = parameter["query"];

                GoogleBooks booksList = await GetGoogleBookLists(queryText, index);

                List<GroupBookList<object>> BookGroups = new List<GroupBookList<object>>();
                ObservableCollection<Items> booksItems = booksList.items;

                var query = from book in booksItems
                            orderby ((Items)book).volumeInfo.title
                            group book by ((Items)book).volumeInfo.categories[0] into books
                            select new { GroupName = books.Key, Items = books };

                foreach (var books in query)
                {
                    GroupBookList<object> info = new GroupBookList<object>();
                    info.Group = books.GroupName;

                    foreach (var book in books.Items)
                    {
                        info.Add(book);
                    }

                    BookGroups.Add(info);
                }
                if(BookGroups != null)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    groupedGoogleBooks.Source = BookGroups;
                }
                else
                {
                    ShowProgressRing();
                }
            }
            catch (Exception ex)
            {
                var popup = new Windows.UI.Popups.MessageDialog("The app couldn't load books. Please check your connection and try again");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Try Again"));
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Cancel"));

                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;

                var results = await popup.ShowAsync();

                if (results.Label == "Try Again")
                {
                    ShowProgressRing();
                    OnNavigatedTo(args);
                }
                else
                {
                    this.Frame.Navigate(typeof(HomePage));
                }
            }
        }

        private async Task<GoogleBooks> LoadGoogleBooks()
        {
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(googleBaseApi + "volumes?q=a&maxResults=40&&startIndex=100");
            postRequest.Method = "GET";
            postRequest.CookieContainer = new CookieContainer();

            HttpWebResponse postResponse = (HttpWebResponse)await postRequest.GetResponseAsync();
            string response = String.Empty;

            if (postResponse != null)
            {
                Stream postResponseStream = postResponse.GetResponseStream();
                StreamReader postStreamReader = new StreamReader(postResponseStream);
                response = await postStreamReader.ReadToEndAsync();
            }

            GoogleBooks booksList = JsonConvert.DeserializeObject<GoogleBooks>(response);

            return booksList;
        }

        public async Task<GoogleBooks> GetGoogleBookLists(string searchTerm, int startIndex)
        {
            ObservableCollection<GoogleBooks> BooksList = new ObservableCollection<GoogleBooks>();
            try
            {

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(googleBaseApi + "volumes?q=" + searchTerm + "&maxResults=40&startIndex=" + startIndex));
                var result = await response.Content.ReadAsStringAsync();

                GoogleBooks googleBooks = JsonConvert.DeserializeObject<GoogleBooks>(result);

                return googleBooks;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HomePage), null);
        }

        private void moochBooks_Checked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MoochBooksList), null);
        }

        private void booksItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Items;
            string bookID = book.id;
            string selfLink = book.selfLink;
            string from = "google";

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("bookID", bookID);
            parameter.Add("from", from);
            parameter.Add("selfLink", selfLink);

            this.Frame.Navigate(typeof(BookDetail), parameter);
        }

        private async void searchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.QueryText))
            {
                this.Frame.Navigate(typeof(GoogleBookSearchResult), args.QueryText);
            }
            else
            {
                var popup = new Windows.UI.Popups.MessageDialog("Search Term Is Empty. Fill in the field, Please");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("OK"));

                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;

                var results = await popup.ShowAsync();
            }
        }

        private void refreshButon_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int index = random.Next(0, 300);
            int charnum = random.Next(0, 26); 
            char query = (char)('a' + charnum);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("index", index.ToString());
            parameter.Add("query", query.ToString());
            this.Frame.Navigate(typeof(GoogleBooksList), parameter);
        }

        private void searchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {

        }

        public async Task<string> GetSmallImage(string selfLink)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(selfLink));
                var result = await response.Content.ReadAsStringAsync();

                Items singleBook = JsonConvert.DeserializeObject<Items>(result);

                return singleBook.volumeInfo.imageLinks.small;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void AssignImage(string selfLink)
        {
            Items items = new Items();
            items.volumeInfo.imageLinks.small = await GetSmallImage(selfLink);
        }
    }
}
