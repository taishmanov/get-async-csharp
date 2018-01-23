using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetAsyncCSharpWpfApp
{
    /// <summary>
    /// Interaction logic for LeaveMainUIContext.xaml
    /// </summary>
    public partial class LeaveMainUIContext : Window
    {
        public LeaveMainUIContext()
        {
            InitializeComponent();
        }

        private async void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            string someFileUri = "http://msdn.microsoft.com";

            // await whole method execution, even if it leaves current conext during execution.
            await DownloadPageAsync(someFileUri);

            infoTextBox.Text = "Web page is downloaded and saved!";
        }

        private async Task DownloadPageAsync(string uri)
        {
            var fileName = string.Format("Downloaded-{0}.html", Guid.NewGuid().ToString("N"));

            infoTextBox.Text = "Downloading...";

            var fileContents = await GetWebPageContentAsync(uri);

            infoTextBox.Text = "Page is downloaded and now let's save it...";

            await Task.Delay(0).ConfigureAwait(false); //we left the current UI context, we're on the thread pool. 
            
            //infoTextBox.Text = "Saving..."; // Another context and infoTextBox is not accessible...

            await SaveToFileAsync(fileName, fileContents).ConfigureAwait(false);
            // The second call to ConfigureAwait(false) is not *required*, but it is Good Practice.


            // Use .ConfigureAwait(true) to get back to main UI context
            // If SynchronizationContext.Current is not null, then you are on main UI Context
        }

        private async Task<string> GetWebPageContentAsync(string uri)
        {
            await Task.Delay(2000);

            using (var client = new HttpClient()) {
                return await client.GetStringAsync(uri);
            }
        }

        private async Task SaveToFileAsync(string fileName, string fileContent)
        {
            // infoTextBox.Text = "Saving..."; // Caller may leave UI context, so it may raise error.
            await Task.Delay(5000);

            using (StreamWriter writer = File.CreateText(fileName))
            {
                await writer.WriteAsync(fileContent);
            }
        }
    }
}
