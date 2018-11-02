using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BookApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookWebView : Page
    {
        public delegate void LoadCompletedEventHandler(object sender, NavigationEventArgs e);
        public BookWebView()
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
                string linkUrl = parameter["link"].ToString();
                Uri linkUri = new Uri(linkUrl);
                bookWebView.Navigate(linkUri);

                bookWebView.LoadCompleted += new Windows.UI.Xaml.Navigation.LoadCompletedEventHandler(bookWebView_LoadCompleted);

                string from = parameter["from"].ToString();
                string title = parameter["title"].ToString();
                if(from == "buy")
                {
                    pageTitle.Text = "Buy " + title;
                }
                else if(from == "details")
                {
                    pageTitle.Text = "Details for " + title;
                }
            }
            base.OnNavigatedTo(e);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void bookWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            HideProgressRing();
        }
    }
}
