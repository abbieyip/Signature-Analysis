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
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Globalization;

namespace SignatureAnalysis
{
    class Program
    {
        string directoryMsg = "Directory does not exist. " +
            "Please enter a valid directory.";
        string fileMsg = "File does not exist. " +
            "Please enter a valid file path.";
        /**
         * Name: Main method
         * Parameters: None
         * Description: Prompts user for a directory, csv file path, and to 
         * include subdirectories or not; fills in csv with each file path, 
         * file type, and hash of the file contents
         */
        static void Main(string[] args)
        {
            Program pg = new Program();
            Console.WriteLine("Enter a directory:");
            string dir = Console.ReadLine();
            // entered invalid directory
            if (!Directory.Exists(dir))
            {
                Console.WriteLine(pg.directoryMsg);
                dir = Console.ReadLine();
                while (!Directory.Exists(dir))
                {
                    Console.WriteLine(pg.directoryMsg);
                    dir = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter a csv file path:");
            string path = Console.ReadLine();
            // entered invalid file path
            if (!File.Exists(path))
            {
                Console.WriteLine(pg.fileMsg);
                path = Console.ReadLine();
                while (!File.Exists(path))
                {
                    Console.WriteLine(pg.fileMsg);
                    path = Console.ReadLine();
                }
            }

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

        /**
         * Name: processDirectory
         * Parameters: directory name, csvFile path, boolean to include subdir
         * Description: Iterates through given directory's files and optional
         * subdirectories
         * Return: N/A
         */
        public void processDirectory(string directory, string csvFile,
            bool sub)
        {
            string[] files = Directory.GetFiles(directory);
            // iterates through each file
            foreach (string path in files)
            {
                string signature = processFile(path, csvFile);
                string hashValue = findHash(path);
                fillCSV(csvFile, path, signature, hashValue);

            }

            // checks whether to enter subdirectories
            if (sub == true)
            {
                string[] directories = Directory.GetDirectories(directory);
                // iterates through each subdirectory
                foreach (string subdir in directories)
                {
                    // recursive call
                    processDirectory(subdir, csvFile, true);
                }
            }

        }

        /**
         * Name: processFile
         * Parameters: file path name, csv file path name
         * Description: Determines using the file signature if the file is a 
         * PDF or a JPG
         * Return: N/A
         */
        public string processFile(string filePath, string csvFile)
        {
            // creates stream to open and read file
            FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[8];
            using (fs)
            {
                BinaryReader br = new BinaryReader(fs);
                using (br)
                {
                    // reads first four bytes of the file
                    buffer = br.ReadBytes(4);
                }
            }
            // converts bits into desired string format
            string bitStr = BitConverter.ToString(buffer);
            string signature = bitStr.Replace("-", "");
            Console.WriteLine(signature);
            // pdf signature
            if (signature.Equals("25504446")) {
                return "PDF";
            }
            // jpg signature
            else if ((signature.Remove(4)).Equals("FFD8"))
            {
                return "JPG";
            }
            // unsupported file type
            else
            {
                return "Unsupported file type";
            }
        }

        /**
         * Name: findHash
         * Parameters: file path name
         * Description: Given a file path, finds the MD5 hash of the file
         * contents
         * Return: MD5 Hash in string format
         */
        public string findHash(string path)
        {
            MD5 mD5 = MD5.Create();
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            byte[] hashData = mD5.ComputeHash(fileBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
            {
                sb.Append((hashData[i]).ToString());
            }
            return sb.ToString();
        }

        /**
         * Name: fillCSV
         * Parameters: csv file path, file path, file type, file hash
         * Description: Fills the given csv file with the given file's 
         * attributes
         * Return: N/A
         */
        public void fillCSV(string csvPath, string filePath, string fileType,
            string hashValue)
        {
            // seperates columns
            string seperator = ",";
            StringBuilder sb = new StringBuilder();

            // creates new row filled with file's properties
            string[][] fileProperties = new string[][]
            {
                new string[]
                {
                    filePath, fileType, hashValue
                }
            };

            // appends file properties to the given csv file 
            for (int i = 0; i < fileProperties.GetLength(0); i++)
            {
                sb.AppendLine(string.Join(seperator, fileProperties[i]));

                File.AppendAllText(csvPath, sb.ToString());
            }
        }
    }

}
