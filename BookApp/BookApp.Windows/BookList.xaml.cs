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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.ApplicationModel.Search;
using Windows.Storage.Streams;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace BookApp
{
    public sealed partial class BookList : Page
    {
        const string booksApi = "https://scottbookapi.herokuapp.com/api/books";

        public BookList()
        {
            this.InitializeComponent();
            this.DisplayBooks();
            ShowProgressRing();
        }

        public void ShowProgressRing()
        {
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (await GetBooksByTitle() != null)
                {
                    booksGroupedView.Source = await this.GetBooksByTitle();
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                }
                else
                {
                    ShowProgressRing();
                }
            }
            catch(Exception ex)
            {
                await ShowErrorMessage("The app couldn't load books. Please check your connection and try again");
            }
        }

        public async Task<ObservableCollection<Books>> GetBookLists()
        {
            ObservableCollection<Books> BooksList = new ObservableCollection<Books>();
            try
            {

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(booksApi + "/all/"));
                var result = await response.Content.ReadAsStringAsync();

                BooksList = JsonConvert.DeserializeObject<ObservableCollection<Books>>(result);

                return BooksList;
            }
            catch(Exception ex)
            {
                BooksList = null;
                return BooksList;
            }
        }

        private async Task<List<GroupBookList<object>>> GetBooksByTitle()
        {
            List<GroupBookList<object>> BookGroups = new List<GroupBookList<object>>();
            ObservableCollection<Books> bookList = await GetBookLists();

            var query = from book in bookList
                        orderby ((Books)book).title
                        group book by ((Books)book).category__name into books
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

            return BookGroups;
        }

        public async void DisplayBooks()
        {
            try
            {
                booksGroupedView.IsSourceGrouped = true;
                booksGroupedView.Source = await GetBookLists();
            }
            catch(Exception ex)
            {
                await ShowErrorMessage("The app couldn't load books. Please check your connection and try again");
            }
            
        }

        public async Task ShowErrorMessage(string message)
        {
            var popup = new Windows.UI.Popups.MessageDialog(message);
            popup.Commands.Add(new Windows.UI.Popups.UICommand("Try Again"));
            popup.Commands.Add(new Windows.UI.Popups.UICommand("Cancel"));

            popup.DefaultCommandIndex = 0;
            popup.CancelCommandIndex = 1;

            var results = await popup.ShowAsync();

            if(results.Label == "Try Again")
            {
                NavigationEventArgs e = null;
                OnNavigatedTo(e);
            }
            else
            {
                this.Frame.Navigate(typeof(HomePage));
            }
        }

        private void booksItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as Books;
            var bookID = book.id;
            string from = "main";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("from", from);
            parameters.Add("bookID", bookID.ToString());
            this.Frame.Navigate(typeof(BookDetail), parameters);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void searchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.QueryText))
            {
                this.Frame.Navigate(typeof(BookSearchResult), args.QueryText);
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
            this.Frame.Navigate(typeof(BookList), null);
        }
    }
}
