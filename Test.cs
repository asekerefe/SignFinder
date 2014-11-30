using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignFinder
{
    class Test
    {
        public static void Main()
        {
            SignFinder sf = new SignFinder(onSearchFinished, onDirectorySearchStarted, onFileSearchStarted, onFileFound);

            sf.start(
            "D:\\<directory-name>",
            //search files that contains 'PDF' signature
            new List<string>() { "PDF" },
            //include subdirectories
            true);
        }

        private static void onSearchFinished(List<string> files)
        {
            Console.WriteLine("results:");
            for (int i = 0; i < files.Count; i++)
                Console.WriteLine("file -> " + files[i]);

            Console.WriteLine(files.Count);
        }

        private static void onDirectorySearchStarted(string dirPath)
        {
            Console.WriteLine("checking dir -> " + dirPath);
        }

        private static void onFileSearchStarted(string filePath)
        {
            Console.WriteLine("checking file -> " + filePath);
        }

        private static void onFileFound(string filePath)
        {
            Console.WriteLine("signature matched found -> " + filePath);
        }
    }
}
