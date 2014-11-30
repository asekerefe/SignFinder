using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignFinder
{
    /*
     * SearchData
     * 
     * Keeps properties of the search operation
     * 
     * written by Alican Şekerefe
     */
    class SearchData
    {
        private string path;
        private bool includeSubDirectories;
        private int maxBytesToCheck;
        private List<string> signaturesToSearch = null;

        public SearchData(string path, List<string> signaturesToSearch, bool includeSubDirectories, int maxBytesToCheck)
        {
            this.path = path;
            this.signaturesToSearch = signaturesToSearch;
            this.includeSubDirectories = includeSubDirectories;
            this.maxBytesToCheck = maxBytesToCheck;
        }

        public string getPath()
        {
            return this.path;
        }

        public bool includesSubDirectories()
        {
            return this.includeSubDirectories;
        }

        public List<string> getSignatures()
        {
            return this.signaturesToSearch;
        }

        public int getMaxBytesToCheck()
        {
            return this.maxBytesToCheck;
        }
    }
}
