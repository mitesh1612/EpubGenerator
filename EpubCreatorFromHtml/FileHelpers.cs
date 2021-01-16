using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EpubCreatorFromHtml
{
    public class FileHelpers
    {
        /// <summary>
        /// Create a folder using the given name and path.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="folderPath">Path of the folder.</param>
        public static void CreateFolderGivenNamePath(string folderName, string folderPath = ".")
        {
            var joinedPath = Path.Join(folderPath, folderName);
            DirectoryInfo di = new DirectoryInfo(joinedPath);
            if (di.Exists)
            {
                Logger.LogToConsole($"{folderName} Directory already exists. Not creating it.");
            }
            
            di.Create();
            Logger.LogToConsole($"{folderName} Directory created successfully.");
        }

        /// <summary>
        /// Create an empty file with given name and path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">Path of the file.</param>
        public static void CreateEmptyFileGivenNamePath(string fileName, string filePath = ".")
        {
            var joinedPath = Path.Join(filePath, fileName);
            if (File.Exists(joinedPath))
            {
                Logger.LogToConsole($"{fileName} already exists. Not creating it.");
            }

            File.Create(joinedPath);
            Logger.LogToConsole($"{fileName} file created successfully.");
        }

        /// <summary>
        /// Writes the given string content to given file name and create the file if it doesn't exist.
        /// </summary>
        /// <param name="fileContent">Content to write.</param>
        /// <param name="fileName">Name of the file to write to.</param>
        /// <param name="filePath">Path of the file to write to.</param>
        public static void CreateFileAndAddContent(string fileContent, string fileName, string filePath = ".")
        {
            var joinedPath = Path.Join(filePath, fileName);
            File.WriteAllText(joinedPath, fileContent);
            Logger.LogToConsole($"Given content written to file {fileName} successfully.");
        }

        /// <summary>
        /// Copy files from source to destination.
        /// </summary>
        /// <param name="sourceFileName">Source File Path.</param>
        /// <param name="destFileName">Destination File Path.</param>
        public static void CopyFile(string sourceFileName, string destFileName)
        {
            try
            {
                File.Copy(sourceFileName, destFileName, true);
                Logger.LogToConsole($"{sourceFileName} copied to {destFileName} successfully.");
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }


        /// <summary>
        /// Get the full file path of a file.
        /// GetFullFilePath("a.txt","folderA/folderB") -> folderA/folderB/a.txt
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="remainingPath">Location of file.</param>
        /// <returns></returns>
        public static string GetFullFilePath(string fileName, string remainingPath)
        {
            return Path.Join(remainingPath, fileName);
        }
    }
}
