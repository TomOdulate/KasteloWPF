using System.Windows;
using System.Windows.Controls;
using Tao.CredentialStore;
using Kastelo.Controls;
using System;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Input;
using Kastelo.Properties;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Deployment.Application;
using System.Text;

namespace Kastelo
{
    public partial class MainWindow
    {
        ApplicationStore _appStore;             // The local store for the applicaion data.
        public bool RequiresSave { get; set; }  // Flag used to indicate if there are unsaved changes.

        public MainWindow()
        {
            InitializeComponent();            
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Closing += MainWindow_Closing;

            // Check for commandline argument to allow for plaintext import / export
            if (App.Args.Any(args => string.Compare(args.ToLower()
                , "allowplaintext", StringComparison.Ordinal) == 0))
                ImportExportMenu.Visibility = Visibility.Visible;

            // Set version label
            if (ApplicationDeployment.IsNetworkDeployed)
                VersionLabel.Text =
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            // If this is the first time run, setup an encryption key
            if (Helper.IsSetupNeeded())
                LoadControl(new UcSetupKey());
            else
                LoadPrevious();
        }
        
        private void LoadPrevious()
        {
            if (string.IsNullOrEmpty(Settings.Default.LastFile)) return;

            var dlgResult = MessageBox.Show("Would you like to reload the last used store",
                "Reload last store", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dlgResult != MessageBoxResult.Yes) return;

            LoadFile(Settings.Default.LastFile);
        }
        protected void LoadFile(string fileName)
        {
            try
            {
                // Prompt user for password & create a hash from it.
                var userInput = new InputDialog("Enter password", "Password Required", "", true);
                if (userInput.ShowDialog() != true) return;
                HashAlgorithm sha = new SHA1CryptoServiceProvider();
                var password = sha.ComputeHash(Encoding.ASCII.GetBytes(userInput.Answer));

                // Load the file, decrypt it & check users password
                var store = new ApplicationStore("tmp", Helper.ReadKeyFromSettings(), Helper.ReadVectorFromSettings());
                store.Load(fileName, Convert.FromBase64String(Settings.Default.IV), password);

                // Replace the event handlers & update local appstore.
                ReplaceHandlers(store);
                _appStore = store;

                // Now we have a decrypted store, display it by loading a user control.
                var uc = new UcViewApplications();
                uc.SetDataSource(store.Applications);
                LoadControl(uc);

                // Store the filepath so it can be auto loaded next time.
                Settings.Default.LastFile = fileName;
                Settings.Default.Save();
                RequiresSave = false;
                FilenameLabel.Text = fileName.Split('\\').Last();
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message, "Authentication Error", MessageBoxButton.OK
                    , MessageBoxImage.Hand);
            }
            catch (CryptographicException)
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("There was an error decrypting the file '{0}'"
                    , fileName.Split('\\').Last()));
                sb.AppendLine();
                sb.AppendLine("Possible reasons include.");
                sb.AppendLine("1: The file is not a valid Kastelo file.");
                sb.AppendLine("2: The file was encrypted using a different Encryption Key.");
                sb.AppendLine("3: The file was encrypted using an older verion of Kastelo");
                MessageBox.Show(sb.ToString(), "An error occurred", MessageBoxButton.OK
                    , MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Are you sure you opened the correct file?"
                    , "An error occurred", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        public void SaveFile()
        {
            var strMgr = _appStore;
            // Has a password been set for this store?
            if (strMgr.Hash == null || strMgr.Hash.Length == 0)
            {
                // Create a password has for this store.
                strMgr.Hash = Helper.GetHashFromString("password");
            }
            strMgr.Applications = _appStore.Applications;
            strMgr = ValidateBeforeSave(strMgr);

            var fd = Helper.GetFileDialog(false, new Uri(Settings.Default.LastFile).Segments.Last() ?? "");
            if (fd.ShowDialog() != true) return;
            
            var initialisationVector = Convert.FromBase64String(Settings.Default.IV);
            var success = strMgr.Save(fd.FileName, initialisationVector);
            if(success) RequiresSave = false;

            MessageBox.Show(success ? "Saved successfully" : "Save Failed!", "Save", MessageBoxButton.OK,
                            success ? MessageBoxImage.Information : MessageBoxImage.Exclamation);
        }

        #region Misc helper methods

        private void ReplaceHandlers(ApplicationStore appStore)
        {
            appStore.Applications.KasteloPropertyChanged += OnApplications_KasteloPropertyChanged;
            appStore.Applications.CollectionChanged += OnApplications_CollectionChanged;
            foreach (var application in appStore.Applications)
            {
                application.ApplicationKasteloPropertyChanged += OnApplications_KasteloPropertyChanged;
                if (application.Credentials != null)
                {
                    application.Credentials.CollectionChanged += OnApplications_CollectionChanged;
                    foreach (var credential in application.Credentials)
                    {
                        credential.CredentialsKasteloPropertyChanged += OnApplications_KasteloPropertyChanged;
                    }
                }
            }
        }
        private void LoadControl(UserControl control)
        {
            if (MainGrid.Children.Count > 1)
                MainGrid.Children.RemoveAt(1);
            MainGrid.Children.Add(control);            
        }
        private ApplicationStore ValidateBeforeSave(ApplicationStore store)
        {
            for (int i = 0; i < store.Applications.Count; i++)
            {
                if (store.Applications[i].Credentials == null)
                    store.Applications.Remove(store.Applications[i]);
                else
                    if (store.Applications[i].Credentials.Count == 0)
                        store.Applications.Remove(store.Applications[i]);
            }
            return store;
        }
        
        #endregion

        #region Event handlers
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RequiresSave)
            {
                var dlgResult = MessageBox.Show("There are unsaved changes that will be lost if you close the application now. Are you sure you want to exit?"
                    , "Exit without save?"
                    , MessageBoxButton.YesNo
                    , MessageBoxImage.Hand
                    , MessageBoxResult.No);
                if (dlgResult == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }
        public void OnApplications_KasteloPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RequiresSave = true;
        }
        public void OnApplications_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RequiresSave = true;
        }

        #region Commands
        public void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Window.Close();
        }
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RequiresSave)
            {
                var dlgSave = MessageBox.Show(
                    "There are changes that have not been saved! do you want to save before opening another file?"
                    , "Changes have not been saved!", MessageBoxButton.YesNo, MessageBoxImage.Stop, MessageBoxResult.Yes);

                if (dlgSave == MessageBoxResult.Yes)
                    SaveFile();
            }

            var dlg = new OpenFileDialog { Filter = "Kastelo File|*.bin|All Files|*.*" };
            var dlgOpen = dlg.ShowDialog();
            if (dlgOpen == true)
                LoadFile(dlg.FileName);
        }
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RequiresSave)
            {
                var dlgResult = MessageBox.Show("There are unsaved changes that will be lost if you close the application now. Are you sure you want to exit?"
                    , "Exit without save?"
                    , MessageBoxButton.YesNo
                    , MessageBoxImage.Hand
                    , MessageBoxResult.No);

                if (dlgResult == MessageBoxResult.No)
                    return;
            }

            _appStore = new ApplicationStore("tmp", Helper.ReadKeyFromSettings(), Helper.ReadVectorFromSettings());
            var uc = new UcViewApplications();
            uc.SetDataSource(_appStore.Applications);
            LoadControl(uc);
        }
        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RequiresSave;
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile();
        }
        private void SetPasswordCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_appStore != null) e.CanExecute = true;
        }
        private void SetPasswordCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var userFirstInput = new InputDialog("Enter a new password", "", "", true);
            if (userFirstInput.ShowDialog() == true)
            {
                var userSecondInput = new InputDialog("Confirm password", "", "", true);
                if (userSecondInput.ShowDialog() == true)
                {
                    if (userFirstInput.Answer == userSecondInput.Answer)
                    {
                        _appStore.Hash = Helper.GetHashFromString(userFirstInput.Answer);
                        RequiresSave = true;
                    }
                    else
                    {
                        MessageBox.Show("Passwords do not match", "Password changed failed"
                            , MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }
        private void ExportKey_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Settings.Default.Key != null)
                e.CanExecute = true;
        }
        private void ExportKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            var dlgresult = dlg.ShowDialog();
            if (dlgresult == true)
                Helper.ExportAKey(Helper.ReadKeyFromSettings(), dlg.FileName);
        }
        private void ImportKey_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ImportKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var dlgresult = dlg.ShowDialog();
            if (dlgresult == true)
                Helper.SaveKeyToSettings(Helper.ImportAKey(dlg.FileName));

        }
        private void ExportData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_appStore != null)
                e.CanExecute = true;
        }
        private void ExportData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            var dlgResult = dlg.ShowDialog();

            if (dlgResult == true)
                Helper.SerializeObject(_appStore.Applications, dlg.FileName);
        }
        private void ImportData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var dlgResult = dlg.ShowDialog();
            if (dlgResult != true) return;

            _appStore = new ApplicationStore("tmp", Helper.ReadKeyFromSettings(), Helper.ReadVectorFromSettings());
            _appStore.Applications = Helper.DeSerializeObject<ObservableCredentialStore<Tao.CredentialStore.App>>(dlg.FileName);
            var uc = new UcViewApplications();
            uc.SetDataSource(_appStore.Applications);
            LoadControl(uc);
            FilenameLabel.Text = dlg.FileName.Split('\\').Last();
            RequiresSave = true;
        }
        private void ImportData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #endregion
    }
}
