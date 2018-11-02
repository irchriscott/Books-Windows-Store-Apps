using BookApp.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// TODO: Connect the Search Results Page to your in-app search.
// The Search Results Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace BookApp
{
    public sealed partial class GoogleBookSearchResult : Page
    {
        public Dictionary<String, IEnumerable<Items>> SearchResult { get; set; }
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        const string googleBaseApi = "https://www.googleapis.com/books/v1/";

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public GoogleBookSearchResult()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "loadingData", true);
        }

        protected async override void OnNavigatedTo (NavigationEventArgs args)
        {
            var queryText = args.Parameter.ToString().ToLower() as String;

            GoogleBooks booksList = await GetGoogleBookList(queryText);

            if(booksList != null)
            {
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

                int countBooks = BookGroups.Count;
                var totalItems = 0;
                var searchFilters = new List<Filter>();
                SearchResult = new Dictionary<string, IEnumerable<Items>>();

                if (countBooks > 0)
                {
                    foreach (var groups in BookGroups)
                    {
                        IEnumerable<Items> matchItems = booksItems.Where(book => book.volumeInfo.title.ToLower().Contains(queryText) && book.volumeInfo.categories[0] == (string)groups.Group);
                        int itemsNumber = matchItems.Count<Items>();

                        totalItems = totalItems + itemsNumber;
                    }

                    searchFilters.Add(new Filter("All", totalItems, true));


                    foreach (var groups in BookGroups)
                    {
                        IEnumerable<Items> matchItems = booksItems.Where(book => book.volumeInfo.title.ToLower().Contains(queryText) && book.volumeInfo.categories[0] == (string)groups.Group);
                        int itemsNumber = matchItems.Count<Items>();

                        if(itemsNumber > 0)
                        {
                            SearchResult.Add(groups.Group.ToString(), matchItems);
                            searchFilters.Add(new Filter(groups.Group.ToString(), itemsNumber, false));
                            totalItems = totalItems + itemsNumber;
                        }
                    }
                }
                else
                {
                    VisualStateManager.GoToState(this, "NoResultsFound", true);
                }

                this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
                this.DefaultViewModel["Filters"] = searchFilters;
                this.DefaultViewModel["ShowFilters"] = searchFilters.Count > 1;
            }
            else
            {
                var popup = new Windows.UI.Popups.MessageDialog("Couldnt fetch books. Plaease check your internet connection.");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Try Again"));
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Cancel"));

                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;

                var results = await popup.ShowAsync();

                if (results.Label == "Try Again")
                {
                    OnNavigatedTo(args);
                }
                else
                {
                    this.Frame.GoBack();
                }
            }
        }

        public async Task<GoogleBooks> GetGoogleBookList(string queryText)
        {
            try
            {
                Random random = new Random();
                int startIndex = random.Next(1, 10);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(googleBaseApi + "volumes?q=" + queryText + "&maxResults=40&startIndex=" + startIndex));
                var result = await response.Content.ReadAsStringAsync();

                GoogleBooks googleBooks = JsonConvert.DeserializeObject<GoogleBooks>(result);

                return googleBooks;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            var filter = (sender as FrameworkElement).DataContext;

            if (filtersViewSource.View != null)
            {
                filtersViewSource.View.MoveCurrentTo(filter);
            }

            var selectedFilter = filter as Filter;
            if (selectedFilter != null)
            {
                selectedFilter.Active = true;

                var groupName = selectedFilter.Name;

                if (groupName.Equals("All"))
                {
                    List<Items> result = new List<Items>();

                    foreach (var group in SearchResult)
                    {
                        result.AddRange(group.Value);
                        this.defaultViewModel["Results"] = result;
                    }
                }
                else
                {
                    this.DefaultViewModel["Results"] = new List<Items>(SearchResult[groupName]);
                }

                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        private sealed class Filter : INotifyPropertyChanged
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Group;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Group"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Group"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Group
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
            {
                if (object.Equals(storage, value)) return false;

                storage = value;
                this.OnPropertyChanged(propertyName);
                return true;
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var eventHandler = this.PropertyChanged;
                if (eventHandler != null)
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void resultsGridView_ItemClick(object sender, ItemClickEventArgs e)
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
    }
}
