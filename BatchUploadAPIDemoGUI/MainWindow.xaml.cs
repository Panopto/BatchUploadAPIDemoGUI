using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.ComponentModel;
using System.Timers;

namespace BatchUploadAPIDemoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool selfSigned = true; // Target server is a self-signed server
        private static bool hasBeenInitialized = false;
        private static long DEFAULT_PARTSIZE = 1048576; // Size of each upload in the multipart upload process
        private static System.Timers.Timer timer;
        private static string fileLocationType = "none";

        public MainWindow()
        {
            InitializeComponent();

            if (selfSigned)
            {
                // For self-signed servers
                EnsureCertificateValidation();
            }
        }

        /// <summary>
        /// Opens a file chooser and allows the user to pick which file to upload
        /// </summary>
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".mp4";
            dlg.Filter = "MP4 File (.mp4)|*.mp4";
            dlg.Multiselect = true;

            DialogResult result = dlg.ShowDialog();

            if (result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                string[] filenames = dlg.FileNames;
                foreach (string filename in filenames)
                {
                    FileInput.Text = filename;
                    Add_Click(null, null);
                }
            }
        }

        /// <summary>
        /// Uploads the selected file to designated server and folder on click
        /// </summary>
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            LockAllFields();
            Status.Content = "Uploading...";

            Common.SetServer(Server.Text);

            timer = new System.Timers.Timer(1500);
            timer.Elapsed += DispatchUpload;
            
            timer.Start();
        }

        /// <summary>
        /// Disabling all fields
        /// </summary>
        private void LockAllFields()
        {
            Server.IsEnabled = false;
            UserID.IsEnabled = false;
            UserPassword.IsEnabled = false;
            FolderID.IsEnabled = false;
            Upload.IsEnabled = false;
            Browse.IsEnabled = false;
            HTTP.IsEnabled = false;
            FTP.IsEnabled = false;
            Disk.IsEnabled = false;
            FileInput.IsEnabled = false;
            FileSourceID.IsEnabled = false;
            FileSourceKey.IsEnabled = false;

            foreach (StackPanel sp in Files.Children)
            {
                WrapPanel wp = sp.Children[0] as WrapPanel;
                System.Windows.Controls.Button bt = wp.Children[1] as System.Windows.Controls.Button;
                bt.IsEnabled = false;
            }
        }

        /// <summary>
        /// Enabling all fields that are originally enabled
        /// </summary>
        private void FreeAllFields()
        {
            Server.IsEnabled = true;
            UserID.IsEnabled = true;
            UserPassword.IsEnabled = true;
            FolderID.IsEnabled = true;
            Upload.IsEnabled = true;
            Browse.IsEnabled = true;
            HTTP.IsEnabled = true;
            FTP.IsEnabled = true;
            Disk.IsEnabled = true;

            if (fileLocationType != "disk")
            {
                FileInput.IsEnabled = true;
                FileSourceID.IsEnabled = true;
                FileSourceKey.IsEnabled = true;
            }

            foreach (StackPanel sp in Files.Children)
            {
                WrapPanel wp = sp.Children[0] as WrapPanel;
                System.Windows.Controls.Button bt = wp.Children[1] as System.Windows.Controls.Button;
                bt.IsEnabled = true;
            }
        }

        /// <summary>
        /// Calls the upload method and handles any error
        /// </summary>
        private void DispatchUpload(Object source, ElapsedEventArgs e)
        {
            timer.Stop();

            Action uploadMethod = ProcessBackground;
            Dispatcher.BeginInvoke(uploadMethod);
        }

        /// <summary>
        /// Sets up BackgroundWorker for uploading in background and starts BackgroundWorker
        /// </summary>
        private void ProcessBackground()
        {
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(ProcessUpload);
            bgw.ProgressChanged += new ProgressChangedEventHandler(UpdateStatus);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UploadComplete);

            string[] filePaths = new string[Files.Children.Count];
            int i = 0;
            foreach (StackPanel sp in Files.Children)
            {
                WrapPanel wp = sp.Children[0] as WrapPanel;
                filePaths[i] = (wp.Children[0] as System.Windows.Controls.TextBox).Text;
                i++;
            }

            object[] args = new object[] { filePaths, UserID.Text, UserPassword.Password, FolderID.Text, FileSourceID.Text, FileSourceKey.Password };

            bgw.RunWorkerAsync(args);
        }

        /// <summary>
        /// Methods to process uploads; updates upload status for each file
        /// </summary>
        /// <param name="sender">BackgroundWorker Object</param>
        /// <param name="e">Arguments necessary to start upload</param>
        private void ProcessUpload(object sender, DoWorkEventArgs e)
        {
            bool hasError = false;
            BackgroundWorker bgw = sender as BackgroundWorker;
            object[] args = e.Argument as object[];

            int i = 0;
            int size = (args[0] as string[]).Length;
            foreach (string filePath in (args[0] as string[]))
            {
                try
                {
                    string fileName = RemoteFileAccess.GetFileName(filePath);
                    string actualFilePath = GetTempFilePath(filePath, args[4] as string, args[5] as string);

                    UploadAPIWrapper.UploadFile(
                        args[1] as string,
                        args[2] as string,
                        args[3] as string,
                        fileName,
                        actualFilePath,
                        DEFAULT_PARTSIZE);
                }
                catch (Exception ex) // single file error handling and status
                {
                    hasError = true;
                    bgw.ReportProgress(i * 100 / size, i + ";" + ex.Message);
                    i++;
                    continue;
                }

                bgw.ReportProgress(i * 100 / size, i + ";" + "Upload Successful");
                
                i++;
            }

            RemoteFileAccess.CleanUp();

            // Handle overall status
            if (!hasError)
            {
                if (size == 0)
                {
                    bgw.ReportProgress(100, -1 + ";" + "No File(s) Selected");
                }
                else
                {
                    bgw.ReportProgress(i * 100 / size, -1 + ";" + "Upload Successful");
                }
            }
            else
            {
                bgw.ReportProgress(i * 100 / size, -1 + ";" + "Upload Failed");
            }
        }

        /// <summary>
        /// Updates status of upload to UI
        /// </summary>
        /// <param name="sender">BackgroundWorker Object</param>
        /// <param name="e">Arguments used to update status</param>
        private void UpdateStatus(object sender, ProgressChangedEventArgs e)
        {
            int index = Convert.ToInt16((e.UserState as string).Split(';')[0]);
            string msg = (e.UserState as string).Split(';')[1];

            if (index == -1)
            {
                Status.Content = msg;
            }
            else
            {
                System.Windows.Controls.Label singleStatus = (Files.Children[index] as StackPanel).Children[1] as System.Windows.Controls.Label;
                singleStatus.Content = msg;
            }
        }

        /// <summary>
        /// Clean up BackgroundWorker job; Free fields in UI
        /// </summary>
        /// <param name="sender">BackgoundWorker Object</param>
        /// <param name="e">Arguments to be used</param>
        private void UploadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            FreeAllFields();
        }

        /// <summary>
        /// Event handler for deleting selected file
        /// </summary>
        /// <param name="sender">Object that starts this event</param>
        /// <param name="e">Arguments holder</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button bt = sender as System.Windows.Controls.Button;
            WrapPanel wp = bt.Parent as WrapPanel;
            StackPanel sp = wp.Parent as StackPanel;

            Files.Children.Remove(sp);
        }

        /// <summary>
        /// Changes UI input according to file location field selected
        /// </summary>
        /// <param name="sender">Object that changed and started this event</param>
        /// <param name="e">Arguments holder</param>
        private void FileLocation_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton rb = sender as System.Windows.Controls.RadioButton;
            string newType = rb.Name.ToLower();

            if (fileLocationType.Equals(newType))
            {
                return;
            }

            // Restore to default
            ClearMode();

            // Disk selection
            if (newType.Equals("disk"))
            {
                DiskMode();

                FileInput.Text = "C:\\\\";

                Browse.Content = "Browse";
                Browse.Click -= Add_Click;
                Browse.Click += Browse_Click;

                Files.Children.RemoveRange(0, Files.Children.Count);
            }
            else // HTTP and FTP selection
            {
                if (fileLocationType == "disk")
                {
                    Browse.Click -= Browse_Click;
                }
                else
                {
                    Browse.Click -= Add_Click;
                }

                if (newType.Equals("ftp"))
                {
                    FileInput.Text = "ftp://";
                    FTPMode();
                }
                else
                {
                    FileInput.Text = "http://";
                    HTTPMode();
                }

                Browse.Content = "Add";
                Browse.Click += Add_Click;

                Files.Children.RemoveRange(0, Files.Children.Count);
            }

            fileLocationType = newType;
        }

        /// <summary>
        /// Restore UI to defualt state
        /// </summary>
        private void ClearMode()
        {
            FileSourceID.IsEnabled = false;
            FileSourceKey.IsEnabled = false;
            FileInput.IsEnabled = false;
            Browse.IsEnabled = false;
            ChosenFileViewer.IsEnabled = false;

            FileSourceID.Visibility = System.Windows.Visibility.Hidden;
            FileSourceKey.Visibility = System.Windows.Visibility.Hidden;
            FileInput.Visibility = System.Windows.Visibility.Hidden;
            Browse.Visibility = System.Windows.Visibility.Hidden;
            ChosenFileViewer.Visibility = System.Windows.Visibility.Hidden;

            SourceIDLabel.Visibility = System.Windows.Visibility.Hidden;
            SourceKeyLabel.Visibility = System.Windows.Visibility.Hidden;
            SourceFileLabel.Visibility = System.Windows.Visibility.Hidden;

            SourceFileLabel.Margin = new Thickness(10.0, 115.0, 0, 0);
            FileInput.Margin = new Thickness(134.0, 115.0, 0, 0);
            Browse.Margin = new Thickness(410.0, 115.0, 0, 0);
            ChosenFileViewer.Margin = new Thickness(134.0, 150.0, 0, 0);
            ChosenFileViewer.Height = 122;
        }

        /// <summary>
        /// Set UI configuration to select file from disk and network
        /// </summary>
        private void DiskMode()
        {
            Browse.IsEnabled = true;
            ChosenFileViewer.IsEnabled = true;

            FileInput.Visibility = System.Windows.Visibility.Visible;
            Browse.Visibility = System.Windows.Visibility.Visible;
            ChosenFileViewer.Visibility = System.Windows.Visibility.Visible;

            SourceFileLabel.Visibility = System.Windows.Visibility.Visible;

            SourceFileLabel.Margin = new Thickness(10.0, 45.0, 0, 0);
            FileInput.Margin = new Thickness(134.0, 45.0, 0, 0);
            Browse.Margin = new Thickness(410.0, 45.0, 0, 0);
            ChosenFileViewer.Margin = new Thickness(134.0, 80.0, 0, 0);
            ChosenFileViewer.Height = 192;
        }

        /// <summary>
        /// Set UI configuration to select file from HTTP address
        /// </summary>
        private void HTTPMode()
        {
            FileInput.IsEnabled = true;
            Browse.IsEnabled = true;
            ChosenFileViewer.IsEnabled = true;

            FileInput.Visibility = System.Windows.Visibility.Visible;
            Browse.Visibility = System.Windows.Visibility.Visible;
            ChosenFileViewer.Visibility = System.Windows.Visibility.Visible;

            SourceFileLabel.Visibility = System.Windows.Visibility.Visible;

            SourceFileLabel.Margin = new Thickness(10.0, 45.0, 0, 0);
            FileInput.Margin = new Thickness(134.0, 45.0, 0, 0);
            Browse.Margin = new Thickness(410.0, 45.0, 0, 0);
            ChosenFileViewer.Margin = new Thickness(134.0, 80.0, 0, 0);
            ChosenFileViewer.Height = 192;
        }

        /// <summary>
        /// Set UI configuration to select file from FTP server address
        /// </summary>
        private void FTPMode()
        {
            FileSourceID.IsEnabled = true;
            FileSourceKey.IsEnabled = true;
            FileInput.IsEnabled = true;
            Browse.IsEnabled = true;
            ChosenFileViewer.IsEnabled = true;

            FileSourceID.Visibility = System.Windows.Visibility.Visible;
            FileSourceKey.Visibility = System.Windows.Visibility.Visible;
            FileInput.Visibility = System.Windows.Visibility.Visible;
            Browse.Visibility = System.Windows.Visibility.Visible;
            ChosenFileViewer.Visibility = System.Windows.Visibility.Visible;

            SourceIDLabel.Visibility = System.Windows.Visibility.Visible;
            SourceKeyLabel.Visibility = System.Windows.Visibility.Visible;
            SourceFileLabel.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Add another file to the queue of files to be uploaded
        /// </summary>
        /// <param name="sender">Object that starts this event</param>
        /// <param name="e">Arguments holder</param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox box = new System.Windows.Controls.TextBox();
            box.Style = (Style)this.Resources["ChosenTextBox"];
            box.Text = FileInput.Text;

            System.Windows.Controls.Button bt = new System.Windows.Controls.Button();
            bt.Style = (Style)this.Resources["ChosenButton"];

            System.Windows.Controls.WrapPanel wp = new WrapPanel();
            wp.Style = (Style)this.Resources["ChosenWrapPanel"];

            System.Windows.Controls.Label status = new System.Windows.Controls.Label();
            status.Style = (Style)this.Resources["SingleStatus"];

            System.Windows.Controls.StackPanel sp = new StackPanel();
            sp.Style = (Style)this.Resources["SingleFileGroup"];

            wp.Children.Add(box);
            wp.Children.Add(bt);

            sp.Children.Add(wp);
            sp.Children.Add(status);

            Files.Children.Add(sp);
        }

        /// <summary>
        /// Obtain a local copy of file at given URI and return its path
        /// </summary>
        /// <param name="uri">Target file URI</param>
        /// <param name="ftpID">FTP server credential</param>
        /// <param name="ftpKey">FTP server credential</param>
        /// <returns>Path to local copy of file</returns>
        private string GetTempFilePath(string uri, string ftpID, string ftpKey)
        {
            if (fileLocationType.Equals("disk"))
            {
                return uri;
            }
            else if (fileLocationType.Equals("http"))
            {
                return RemoteFileAccess.GetHTTPFile(uri);
            }
            else if (fileLocationType.Equals("ftp"))
            {
                return RemoteFileAccess.GetFTPFile(uri, ftpID, ftpKey);
            }
            else 
            {
                return null;
            }
        }

        //========================= Needed to use self-signed servers

        /// <summary>
        /// Ensures that our custom certificate validation has been applied
        /// </summary>
        public static void EnsureCertificateValidation()
        {
            if (!hasBeenInitialized)
            {
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(CustomCertificateValidation);
                hasBeenInitialized = true;
            }
        }

        /// <summary>
        /// Ensures that server certificate is authenticated
        /// </summary>
        private static bool CustomCertificateValidation(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }
    }
}
