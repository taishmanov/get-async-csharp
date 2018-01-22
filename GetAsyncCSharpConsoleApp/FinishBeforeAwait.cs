using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetAsyncCSharpConsoleApp
{
    /// <summary>
    /// This is example class. The aim is to 
    ///  - start background I/O tasks
    ///  - do some job on main UI thread without affecting background tasks
    ///  - show how background task may finish before awaiting the task
    /// </summary>
    public class FinishBeforeAwait
    {
        /// <summary>
        /// This method calls 2 async methods with different execution times and waits until both is complete. 
        /// </summary>
        /// <returns></returns>
        public async Task AccessTheWebAsync()
        {
            var someUri = "http://google.com";
            //call async method 
            Task fastDownloadTask = FastDownloadPageAsync(someUri);
            
            //call async method
            Task lazyDownloadTask = LazyDownloadPageAsync(someUri);

            // Do some job on main thread
            // We expect fastDownloadTask will finish during DoIndependentWork() execution.
            DoIndependentWork();
            
            Program.WriteToConsole("Let's wait both of tasks.");
            
            // waits all of the suspended tasks have completed.
            // program waits lazyDownloadTask even if fastDownloadTask finished before this line.
            await Task.WhenAll(lazyDownloadTask, fastDownloadTask);

            Program.WriteToConsole("All tasks have finished.");
        }

        /// <summary>
        /// Downloads the page content after 10 sec
        /// </summary>
        /// <param name="uri">uri of the web page</param>
        /// <returns>task</returns>
        public async Task LazyDownloadPageAsync(string uri)
        {
            HttpClient client = new HttpClient();

            Program.WriteToConsole("LazyDownloadPage: I will download the page, but after 10 sec.");
            await Task.Delay(10000);

            Program.WriteToConsole("LazyDownloadPage: I am starting downloading.");
            var pageContent = await client.GetStringAsync(uri);

            Program.WriteToConsole(string.Format("LazyDownloadPage: I have finished. Content Length is: {0}", pageContent.Length));
            
        }

        /// <summary>
        /// Immediatly starts the download
        /// </summary>
        /// <param name="uri">uri if the web page</param>
        /// <returns>task</returns>
        public async Task FastDownloadPageAsync(string uri)
        {
            HttpClient client = new HttpClient();

            Program.WriteToConsole("FastDownloadPage: I am starting downloading.");

            var pageContent = await client.GetStringAsync(uri);

            Program.WriteToConsole(string.Format("FastDownloadPage: I have finished. Content Length is: {0}", pageContent.Length));
            
        }


        /// <summary>
        /// Does some job in main UI Thread.
        /// </summary>
        public void DoIndependentWork()
        {
            Program.WriteToConsole("DoIndependentWork: I can do here something 7 sec.");
            Thread.Sleep(7000);
            Program.WriteToConsole("DoIndependentWork: I have finished.");
        }

        /*
         * My result:
         * 
        14:59:40.7302133:FastDownloadPage: I am starting downloading.
        14:59:40.8152153:LazyDownloadPage: I will download the page, but after 10 sec.
        14:59:40.8172147:DoIndependentWork: I can do here something 7 sec.
        14:59:41.0522143:FastDownloadPage: I have finished. Content Length is: 46263
        14:59:47.8182647:DoIndependentWork: I have finished.
        14:59:47.8182647:Let's wait both of tasks.
        14:59:50.8172787:LazyDownloadPage: I am starting downloading.
        14:59:51.1632829:LazyDownloadPage: I have finished. Content Length is: 46308
        14:59:51.1642826:All tasks have finished.
         
         */

    }
}
