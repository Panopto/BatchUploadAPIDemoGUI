using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.SharePoint.Client;

namespace BatchUploadAPIDemoGUI
{
    class RemoteFileAccess
    {
        private static string folder = null; // Storage location of downloaded files, Will be deleted at the end of the program
        private static string userName = null;
        private static string userPassword = null;

        /// <summary>
        /// Downloads the file located at given URI to disk for upload
        /// </summary>
        /// <param name="uri">URI of file</param>
        /// <returns>Path on disk of downloaded file</returns>
        public static string GetHTTPFile(string uri)
        {
            string filePath = GetTempPath(uri);

            WebClient wc = new WebClient();
            wc.DownloadFile(uri, filePath);

            return filePath;
        }

        /// <summary>
        /// Downloads the file located on a FTP server at given URI
        /// </summary>
        /// <param name="uri">URI of file</param>
        /// <returns>Path on disk of downloaded file</returns>
        public static string GetFTPFile(string uri, string userID, string userKey)
        {
            string filePath = GetTempPath(uri);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            if (userName == null || userPassword == null)
            {
                userName = userID;
                userPassword = userKey;
            }

            request.Credentials = new NetworkCredential(userName, userPassword);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            System.IO.FileStream fs = System.IO.File.Create(filePath);
            responseStream.CopyTo(fs);

            fs.Close();
            responseStream.Close();
            response.Close();

            return filePath;
        }

        /// <summary>
        /// Cleans up temporary files that are downloaded
        /// </summary>
        public static void CleanUp()
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            userName = null;
            userPassword = null;
            folder = null;
        }

        /// <summary>
        /// Produce a temporary file path for file to be downloaded
        /// </summary>
        /// <param name="uri">URI of file to be downloaded</param>
        /// <returns>Storage path on disk of file to be downloaded</returns>
        private static string GetTempPath(string uri)
        {
            if (folder == null)
            {
                DateTime dt = DateTime.Now;
                folder = ".\\temp_" + dt.Year + "_" + dt.Month + "_" + dt.Day + "_" + dt.Hour + "_" + dt.Minute + "_" + dt.Second;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = GetFileName(uri);

            return Path.Combine(folder, fileName);
        }

        /// <summary>
        /// Gets the name of file in the give URI
        /// </summary>
        /// <param name="uri">URI of file to obtain name from</param>
        /// <returns>Name of file in URI</returns>
        public static string GetFileName(string uri)
        {
            string[] splitted = uri.Split(new char[] { '/', '\\' });
            return splitted[splitted.Length - 1];
        }
        
        /// <summary>
        /// Prompts user on console for password
        /// </summary>
        /// <returns>Password obtained from user</returns>
        private static string GetPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            return pass;
        }
    }
}
