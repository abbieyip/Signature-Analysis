/**
* Sample Application – Signature Analysis 
*
* Write a command line application (preferably in C#) that does the following: 
*    Takes two inputs and a flag
*        A directory that contains the files to be analyzed
*        A path for the output file (including file name and extension)
*        A flag to determine whether or not to include subdirectories contained
*           (and all subsequently embedded subdirectories) within the input
*           directory ([a.] above)
*    Processes each of the files in the directory (and subdirectories if the 
*       flag is present)
*    Determines using a file signature if a given file is a PDF or a JPG
*           JPG files start with 0xFFD8
*           PDF files start with 0x25504446
*    For each file that is a PDF or a JPG, creates an entry in the output CSV
*       containing the following information
*           The full path to the file
*           The actual file type (PDF or JPG)
*           The MD5 hash of the file contents
*/

using System;
using System.IO;
using System.Collections;

namespace SignatureAnalysis
{
    class Program
    {
        /**
         * Name: Main method
         * Parameters: None
         * Description: Prompts user for a directory, csv file path, and to 
         * include subdirectories or not; fills in csv with each file path, 
         * file type, and hash of the file contents
         */
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a directory:");
            string dir = Console.ReadLine();
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory does not exist. Please enter a valid directory.");
                dir = Console.ReadLine();
                while (!Directory.Exists(dir))
                {
                    Console.WriteLine("Directory does not exist. Please enter a valid directory.");
                    dir = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter a file path:");
            string path = Console.ReadLine();
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist. Please enter a valid file path.");
                path = Console.ReadLine();
                while (!File.Exists(path))
                {
                    Console.WriteLine("File does not exist. Please enter a valid file path.");
                    path = Console.ReadLine();
                }
            }

            Program pg = new Program();
            Console.WriteLine("Include subdirectories? (Y/N)");
            if ((Console.ReadLine()).Equals("Y"))
            {
                pg.processDirectory(dir, path, true);
            }
            else
            {
                pg.processDirectory(dir, path, false);

            }
        }

        public void processDirectory(string directory, string csvFile, bool sub)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string path in files)
            {
                processFile(path, csvFile);
            }

            if (sub == true)
            {
                string[] directories = Directory.GetDirectories(directory);
                foreach (string subdir in directories)
                {
                    processDirectory(subdir, csvFile, true);
                }
            }

        }

        public void processFile(string filePath, string csvFile)
        {
            Console.WriteLine("it works!");

        }
    }
}
