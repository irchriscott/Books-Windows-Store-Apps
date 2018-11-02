using BookApp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.Networking.BackgroundTransfer;
using System.Threading;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace BookApp
{
    public sealed partial class BookReadView : Page
    {
        public BookReadView()
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
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter = e.Parameter as Dictionary<string, string>;

                    if (parameter["bookUrl"] != null)
                    {
                        string bookUrl = parameter["bookUrl"];
                        pageTitle.Text = parameter["title"];

                        Uri fileTarget = new Uri(bookUrl.Trim());
                        StorageFile file = await KnownFolders.DocumentsLibrary.CreateFileAsync(pageTitle.Text.Trim() + ".pdf", CreationCollisionOption.GenerateUniqueName);

                        CancellationTokenSource cancellationToken = new CancellationTokenSource();

                        BackgroundDownloader downloader = new BackgroundDownloader();
                        DownloadOperation download = downloader.CreateDownload(fileTarget, file);
                        Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);

                        progressStatus.Text = "Initializing ....";
                        await download.StartAsync().AsTask(cancellationToken.Token, progress);

                        if (download.Progress.BytesReceived >= download.Progress.TotalBytesToReceive)
                        {
                            StorageFile bookPdf = await KnownFolders.DocumentsLibrary.GetFileAsync(pageTitle.Text.Trim() + ".pdf");
                            LoadPdfFileAsync(file);
                            HideProgressRing();
                        }
                        else
                        {
                            ShowProgressRing();
                        }
                    }
                }
                base.OnNavigatedTo(e);
            }
            catch(TaskCanceledException ex)
            {
                var popup = new Windows.UI.Popups.MessageDialog("Cant load the file. The program might have failed to open the file  or you are not connected");
                progressStatus.Text = "Download canceled.";
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();

                if(results.Label == "Ok")
                {
                    this.Frame.GoBack();
                }

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
                
                if(progressLoader.Value >= 99 || count >= (int)pdfDocument.PageCount - 1)
                {
                    progressStatus.Text = "Pages Loaded";
                    popUp.IsOpen = false;
                }
                bookPagesView.ItemsSource = bookPages;
            }
            catch(Exception ex)
            {
                var popup = new Windows.UI.Popups.MessageDialog("Cant load the file. The program might have failed to open the file  or you are not connected");
                popup.Commands.Add(new Windows.UI.Popups.UICommand("Ok"));
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var results = await popup.ShowAsync();
            }
        }

        private void progressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
            downloadSize.Text = String.Format("{0} of {1} kb. downloaded - {2} % complete.", downloadOperation.Progress.BytesReceived / 1024, downloadOperation.Progress.TotalBytesToReceive / 1024, progress);
            progressLoader.Value = progress;
            switch (downloadOperation.Progress.Status)
            {
                case BackgroundTransferStatus.Running:
                    {
                        progressStatus.Text = "Downloading...";
                        break;
                    }
                case BackgroundTransferStatus.PausedByApplication:
                    {
                        progressStatus.Text = "Download paused.";
                        break;
                    }
                case BackgroundTransferStatus.PausedCostedNetwork:
                    {
                        progressStatus.Text = "Download paused because of metered connection.";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        progressStatus.Text = "No network detected. Please check your internet connection.";
                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        progressStatus.Text = "An error occured while downloading.";
                        break;
                    }
            }
            if (progress >= 100)
            {
                progressStatus.Text = "Download complete.";
                downloadOperation = null;
                popUp.IsOpen = false;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
