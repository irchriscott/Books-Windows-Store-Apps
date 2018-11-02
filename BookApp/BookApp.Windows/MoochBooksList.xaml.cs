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
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace BookApp
{
    public sealed partial class MoochBooksList : Page
    {
        const string bookmoochBaseApi = "http://api.bookmooch.com/api/";

        public MoochBooksList()
        {
            this.InitializeComponent();
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
                if (await GetBooksByTitle(GenerateQuery()) != null)
                {
                    moochGroupedItems.Source = await this.GetBooksByTitle(GenerateQuery());
                    VisualStateManager.GoToState(this, "ResultsFound", true);
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
                    OnNavigatedTo(e);
                }
                else
                {
                    this.Frame.Navigate(typeof(HomePage));
                }
            }
        }

        public async Task<ObservableCollection<MoochBooks>> GetMoochBooks(string queryText)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(bookmoochBaseApi + "search?txt=" + queryText + "&db=bm&o=json"));
                var result = await response.Content.ReadAsStringAsync();

                ObservableCollection<MoochBooks> booksList = JsonConvert.DeserializeObject<ObservableCollection<MoochBooks>>(result);

                return booksList;
            }
            catch(TaskCanceledException ex)
            {
                ShowProgressRing();
                return null;
            }
        }

        public async Task<List<MoochBooks>> LoadBookmoochBooks()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(bookmoochBaseApi);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("appliction/json"));
                var response = await client.GetAsync(bookmoochBaseApi + "recent?since=1&o=json");
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(List<MoochBooks>));

                var memorystream = new MemoryStream(Encoding.UTF8.GetBytes(result));
                var data = (List<MoochBooks>)serializer.ReadObject(memorystream);

                List<MoochBooks> bookslist = JsonConvert.DeserializeObject<List<MoochBooks>>(result);

                return bookslist;
            }
            catch(TaskCanceledException ex)
            {
                ShowProgressRing();
                return null;
            }
        }

        private async Task<List<GroupBookList<object>>> GetBooksByTitle(string queryText)
        {
            List<GroupBookList<object>> BookGroups = new List<GroupBookList<object>>();
            ObservableCollection<MoochBooks> bookList = await GetMoochBooks(queryText);

            var query = from book in bookList
                        orderby ((MoochBooks)book).Title
                        group book by ((MoochBooks)book).Topics[0] into books
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

        public string GenerateQuery()
        {
            Random rnd = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };

            string word = "";
            int requestedLength = 2;

            while (word.Length < requestedLength)
            {
                if (requestedLength != 1)
                {
                    string consonant = GetRandomLetter(rnd, consonants);

                    if (consonant == "q" && word.Length + 3 <= requestedLength)
                    {
                        word += "qu";
                    }
                    else
                    {
                        while (consonant == "q")
                        {
                            consonant = GetRandomLetter(rnd, consonants);
                        }

                        if (word.Length + 1 <= requestedLength)
                        {
                            word += consonant;
                        }
                    }
                }

                if (word.Length + 1 <= requestedLength)
                {
                    word += GetRandomLetter(rnd, vowels);
                }
            }

            return word;
        }

        private static string GetRandomLetter(Random rnd, string[] letters)
        {
            return letters[rnd.Next(0, letters.Length - 1)];
        }

        private void googleBooks_Checked(object sender, RoutedEventArgs e)
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HomePage), null);
        }

        private void booksItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as MoochBooks;
            string bookID = book.id;
            string from = "mooch";

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("bookID", bookID);
            parameter.Add("from", from);
            parameter.Add("queryText", GenerateQuery());
            this.Frame.Navigate(typeof(BookDetail), parameter);
        }

        private async void searchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.QueryText))
            {
                this.Frame.Navigate(typeof(MoochBookSearchResult), args.QueryText);
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
            this.Frame.Navigate(typeof(MoochBooksList), parameter);
        }
    }
}
