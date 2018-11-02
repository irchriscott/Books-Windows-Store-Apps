using BookApp.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

    public sealed partial class BookSearchResult : Page
    {

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public Dictionary<String, IEnumerable<Books>> SearchResult { get; set; }
        const string booksApi = "https://scottbookapi.herokuapp.com/api/books";


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public BookSearchResult()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "loadingData", true);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            BookList instanceBooks = new BookList();
            ObservableCollection<Books> bookList = await instanceBooks.GetBookLists();

            var queryText = args.Parameter.ToString().ToLower() as String;

            var requestedBooks = await GetBookSearchResult(queryText);
            if(requestedBooks != null)
            {
                int countBooks = requestedBooks.Count;

                var totalItems = 0;
                var searchFilters = new List<Filter>();
                SearchResult = new Dictionary<string, IEnumerable<Books>>();

                if (countBooks > 0)
                {
                    foreach (var groups in requestedBooks)
                    {
                        IEnumerable<Books> matchItems = bookList.Where(book => book.title.ToLower().Contains(queryText) && book.category__name == (string)groups.Group);
                        int itemsNumber = matchItems.Count<Books>();

                        totalItems = totalItems + itemsNumber;
                    }

                    searchFilters.Add(new Filter("All", totalItems, true));

                    foreach (var groups in requestedBooks)
                    {
                        IEnumerable<Books> matchItems = bookList.Where(book => book.title.ToLower().Contains(queryText) && book.category__name == (string)groups.Group);
                        int itemsNumber = matchItems.Count<Books>();

                        if (itemsNumber > 0)
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

        private async Task<List<GroupBookList<object>>> GetBookSearchResult(string searchTerm)
        {
            try
            {
                BookList instanceBooks = new BookList();
                List<GroupBookList<object>> BookGroups = new List<GroupBookList<object>>();
                ObservableCollection<Books> bookList = await instanceBooks.GetBookLists();

                var query = from book in bookList
                            where ((Books)book).title.ToLower().Contains(searchTerm.ToLower())
                            orderby ((Books)book).title
                            group book by ((Books)book).category__name into books
                            select new { GroupName = books.Key, Items = books, Number = books.Count() };

                foreach (var books in query)
                {
                    GroupBookList<object> info = new GroupBookList<object>();
                    info.Group = books.GroupName;
                    info.ItemsNumber = books.Number;

                    foreach (var book in books.Items)
                    {
                        info.Add(book);
                    }

                    BookGroups.Add(info);
                }

                return BookGroups;
            }
            catch(TaskCanceledException ex)
            {
                return null;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
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
                    List<Books> result = new List<Books>();

                    foreach(var group in SearchResult)
                    {
                        result.AddRange(group.Value);
                        this.defaultViewModel["Results"] = result;
                    }
                }
                else
                {
                    this.DefaultViewModel["Results"] = new List<Books>(SearchResult[groupName]);
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
    }
}
