using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
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
    public sealed partial class ReadLocalBook : Page
    {
        public ReadLocalBook()
        {
            this.InitializeComponent();
        }

        public void ShowProgressRing()
        {
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        public void HideProgressRing()
        {
            VisualStateManager.GoToState(this, "ResultsFound", true);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter != null)
                {
                    try
                    {
                        var args = e.Parameter as Windows.ApplicationModel.Activation.IActivatedEventArgs;

                        if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.File)
                        {
                            var fileArgs = args as Windows.ApplicationModel.Activation.FileActivatedEventArgs;
                            string strFilePath = fileArgs.Files[0].Path;
                            StorageFile file = (StorageFile)fileArgs.Files[0];
                            pageTitle.Text = fileArgs.Files[0].Name;
                            LoadPdfFileAsync(file);
                            backButton.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch(Exception ex)
                    {
                        StorageFile file = e.Parameter as StorageFile;
                        if (file != null)
                        {
                            pageTitle.Text = file.DisplayName;
                            LoadPdfFileAsync(file);
                        }
                    }
                }
                base.OnNavigatedTo(e);
            }
            catch (Exception ex)
            {
                var popup = new Windows.UI.Popups.MessageDialog("The Program doesn't support this type of file. It only opens PDF files");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
            }
        }

        private async void LoadPdfFileAsync(StorageFile bookUrl)
        {
            try
            {
                ObservableCollection<BookPdf> bookPages = new ObservableCollection<BookPdf>();
                PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(bookUrl);
                int count = 0;
                popUp.IsOpen = true;
                progressLoader.Maximum = (int)pdfDocument.PageCount;
                pageCount.Text = pdfDocument.PageCount.ToString() + " Pages";
                progressStatus.Text = "Loading Pages...";
                for (int pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
                {
                    var pdfPage = pdfDocument.GetPage((uint)pageIndex);
                    if (pdfPage != null)
                    {
                        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
                        StorageFile pngFile = await tempFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".png", CreationCollisionOption.ReplaceExisting);

                        if (pngFile != null && pdfPage != null)
                        {
                            IRandomAccessStream randomStream = await pngFile.OpenAsync(FileAccessMode.ReadWrite);

                            await pdfPage.RenderToStreamAsync(randomStream);
                            await randomStream.FlushAsync();
                            randomStream.Dispose();
                            pdfPage.Dispose();

                            count++;
                            progressLoader.Value = pageIndex;
                            int progress = (100 * pageIndex) / (int)pdfDocument.PageCount;

                            downloadSize.Text = String.Format("{0} of {1} pages loaded - {2} % complete.", pageIndex, pdfDocument.PageCount, progress);
                            bookPages.Add(new BookPdf { Id = pageIndex.ToString(), PageNumber = pageIndex.ToString(), ImagePath = pngFile.Path });
                        }
                    }
                }
                if (progressLoader.Value >= 99 || count >= (int)pdfDocument.PageCount - 1)
                {
                    progressStatus.Text = "Pages Loaded";
                    popUp.IsOpen = false;
                }
                bookPagesView.ItemsSource = bookPages;
            }
            catch(Exception ex)
            {
                var popup = new Windows.UI.Popups.MessageDialog("The Program doesn't support this type of file. It only opens PDF files");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
                
                if(results.Label == "OK")
                {
                    this.Frame.GoBack();
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
