using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BookApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var animation = this.Resources["animateText"] as Storyboard;
            if (animation != null)
            {
                animation.Begin();
            }

            if(!string.IsNullOrEmpty(e.Parameter as String))
            {
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                var toeastElement = notificationXml.GetElementsByTagName("text");
                toeastElement[0].AppendChild(notificationXml.CreateTextNode("Welcome to Free Library App"));
                toeastElement[1].AppendChild(notificationXml.CreateTextNode("Read and download books for Free"));
                toeastElement[2].AppendChild(notificationXml.CreateTextNode("In Google books and Mooch Books"));
                var imageElement = notificationXml.GetElementsByTagName("image");
                imageElement[0].Attributes[1].NodeValue = "Assets/SmallLogo.scale-100.png";
                var toastNotification = new ToastNotification(notificationXml);
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
        }

        private void open_library_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BookList), null);
        }

        private void open_paid_Click(object sender, RoutedEventArgs e)
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

        private async void open_book_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".pdf");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.SettingsIdentifier = "book_picker";
            filePicker.CommitButtonText = "Open Pdf File";

            StorageFile file = await filePicker.PickSingleFileAsync();
            if(file != null)
            {
                this.Frame.Navigate(typeof(ReadLocalBook), file);
            }
        }
    }
}
