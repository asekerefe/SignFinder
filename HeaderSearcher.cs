using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace HeaderSearcher
{
    /*  
     * HeaderSearcher
     *
     * Core class for searching files
     * 
     * written by Alican Şekerefe
     */
    class HeaderSearcher
    {
        //callback for passing the search results as file names
        public delegate void OnSearchFinished(List<string> files);
        //callback for notifying that a new directory will be searched
        public delegate void OnDirectorySearchStarted(string dirPath);
        //callback for notifying that a new file will be read
        public delegate void OnFileSearchStarted(string filePath);
        //callback for notifying that a file has been matched
        public delegate void OnFileMatched(string filePath);

        //state keepers
        private bool searching = false;
        private Thread searchThread = null;
        private SearchData currentSearchData = null;
        private volatile bool abortSearch = false;

        //callbacks
        private OnSearchFinished finishedCallback = null;
        private OnDirectorySearchStarted directorySearchStartedCallback = null;
        private OnFileSearchStarted fileSearchStartedcallback = null;
        private OnFileMatched fileMatchedCallback = null;

        /// <summary>
        /// The constructor for getting callbacks. null values are acceptable
        /// </summary>
        /// <param name="finishedCallback"></param>
        /// <param name="directorySearchStartedCallback"></param>
        /// <param name="fileSearchStartedcallback"></param>
        /// <param name="fileMatchedCallback"></param>
        public HeaderSearcher(OnSearchFinished finishedCallback, OnDirectorySearchStarted directorySearchStartedCallback, OnFileSearchStarted fileSearchStartedcallback, OnFileMatched fileMatchedCallback)
        {
            this.finishedCallback = finishedCallback;
            this.directorySearchStartedCallback = directorySearchStartedCallback;
            this.fileSearchStartedcallback = fileSearchStartedcallback;
            this.fileMatchedCallback = fileMatchedCallback;
        }

        /// <summary>
        /// starts searching the files inside the path for the given header names.
        /// </summary>
        /// <param name="path">path to search</param>
        /// <param name="headersToSearch">header names the search inside a file</param>
        /// <param name="searchSubDirectories">will this search include the subdirectories?</param>
        /// <param name="maxBytesToCheck">how many bytes are going to be read in a file to match header name? needs to be kept small</param>
        /// <returns>true if search started</returns>
        public bool start(string path, List<string> headersToSearch, bool searchSubDirectories = false, int maxBytesToCheck = 20)
        {
            bool canStart = !searching;

            if (canStart)
            {
                uppercaseStrings(headersToSearch);
                abortSearch = false;
                searching = true;
                currentSearchData = new SearchData(path, headersToSearch, searchSubDirectories, maxBytesToCheck);
                //start searching files in a new thread
                ThreadStart ts = new ThreadStart(search);
                searchThread = new Thread(ts);
                searchThread.Start();
            }

            return canStart;
        }

        /// <summary>
        /// stops the serach operation
        /// </summary>
        public void stop()
        {
            if (searching)
                abortSearch = true;
        }

        /// <summary>
        /// root method for the search operation
        /// </summary>
        private void search()
        {
            List<string> files = new List<string>();
            //start searching from the root path
            searchDirectory(currentSearchData.getPath(), files);

            //clear state
            searching = false;
            currentSearchData = null;
            searchThread = null;

            //pass results through the callback
            if (fileMatchedCallback != null)
                finishedCallback(files);
        }

        /// <summary>
        /// searches the given directory
        /// </summary>
        /// <param name="path">path to search</param>
        /// <param name="files">file list to be used for adding matched files</param>
        private void searchDirectory(string path, List<string> files)
        {
            bool exit = false;

            //make notification for the directory
            if (directorySearchStartedCallback != null)
                directorySearchStartedCallback(path);

            string[] fileArr = Directory.GetFiles(path);
            List<string> headers = currentSearchData.getSearchHeaders();

            //iterate over each file found inside the path
            for (int i = 0; i < fileArr.Length; i++)
            {
                //make notification for the file
                if (fileSearchStartedcallback != null)
                    fileSearchStartedcallback(fileArr[i]);

                //open a stream and create a buffer for the specified amount of bytes
                FileStream fs = new FileStream(fileArr[i], FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                char[] buffer = new char[currentSearchData.getMaxBytesToCheck()];
                reader.Read(buffer, 0, buffer.Length);

                reader.Close();

                string target = new string(buffer);

                //search header names inside the target string
                for (int j = 0; j < headers.Count; j++)
                {
                    if (target.Contains(headers[j]))
                    {
                        //target matched. make notification
                        if (fileMatchedCallback != null)
                            fileMatchedCallback(fileArr[i]);
                        //add file and continue with the next one
                        files.Add(fileArr[i]);
                        break;
                    }
                }

                if ((bool)abortSearch)
                    exit = true;
            }

            if (!exit)
            {
                //if subdirectories are included, 
                //find their names and make a recursive call
                if (currentSearchData.includesSubDirectories())
                {
                    string[] subDirs = Directory.GetDirectories(path);
                    for (int i = 0; i < subDirs.Length; i++)
                        searchDirectory(subDirs[i], files);
                }
            }
        }

        /// <summary>
        /// makes given list of strings uppercase
        /// </summary>
        /// <param name="strings">string list</param>
        private void uppercaseStrings(List<string> strings)
        {
            for (int i = 0; i < strings.Count; i++)
                strings[i] = strings[i].ToUpper();
        }
    }
}
