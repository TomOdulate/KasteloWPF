using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tao.CredentialStore;

namespace Kastelo.Controls
{
    /// <summary>
    /// Interaction logic for ucViewApplications.xaml
    /// </summary>
    public partial class UcViewApplications
    {
        public ObservableCollection<Tao.CredentialStore.App> MyApplications;

        public UcViewApplications()
        {
            InitializeComponent();
            TreeView.DataContext = MyApplications;
            TreeView.ItemsSource = MyApplications;
            MyApplications = new ObservableCollection<Tao.CredentialStore.App>();
        }

        #region Events
        public event PropertyChangedEventHandler KasteloPropertyChanged;        
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
            {
                RemoveControlsFromGridColumn(1);
                return;
            }

            LoadControlIntoGrid(e.NewValue.GetType(), e.NewValue);
            DeleteButtonText.Text = e.NewValue.GetType().Name == "App" ? "Delete Application" : "Delete Credential";
        }
        #endregion        

        #region Commands
        private void AddApplication_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void AddApplicaiton_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var dlg = new InputDialog("Enter an application name", "New Application");
            if (dlg.ShowDialog() != true) return;

            if (dlg.Answer.Trim() == string.Empty) return;

            if (!MyApplications.Any(x => x.Name == dlg.Answer))
            {
                var app = new Tao.CredentialStore.App(dlg.Answer);
                var parentWindow = (MainWindow)Window.GetWindow(this);
                if (parentWindow != null)
                    app.Credentials.CollectionChanged += parentWindow.OnApplications_CollectionChanged;
                MyApplications.Add(app);
            }
            else
                MessageBox.Show(string.Format("Application {0} already exists", dlg.Answer));
        }
        private void AddCredential_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (TreeView.SelectedItem == null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        private void AddCredential_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var dlg = new InputDialog("Enter a user name", "New Credential");
            if (dlg.ShowDialog() != true) return;
            if (dlg.Answer.Trim() == string.Empty) return;

            var item = TreeView.SelectedItem;
            switch (item.GetType().Name)
            {
                case "App":
                    // Ensure the username doesn't already exist
                    var selectedItem = (Tao.CredentialStore.App)item;
                    if (selectedItem.Credentials == null)
                    {
                        selectedItem.Credentials = new ObservableCredentialStore<Credential>();
                        var parentWindow = (MainWindow)Window.GetWindow(this);
                        if (parentWindow != null)
                            selectedItem.Credentials.CollectionChanged += parentWindow.OnApplications_CollectionChanged;
                    }
                    var apps = MyApplications.First(x => x.Name == ((Tao.CredentialStore.App)item).Name);
                    if (apps.Credentials.Any(x => x.Username == dlg.Answer))
                    {
                        MessageBox.Show("credential already exists");
                    }
                    else
                    {                                                                      
                        apps.Credentials.Add(new Credential(dlg.Answer, "", "", "") { Application = selectedItem.Name });
                    }
                    break;
                case "Credential":
                    // Ensure the username doesn't already exist
                    var app = MyApplications.First(x => x.Name == ((Credential)item).Application);
                    if (app.Credentials.Any(x => x.Username == dlg.Answer))
                    {
                        MessageBox.Show("credential already exists");
                    }
                    else
                    {
                        app.Credentials.Add(new Credential(dlg.Answer, "", "", "") { Application = app.Name });
                    }
                    break;
            }
        }
        private void DeleteItem_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (TreeView.SelectedItem == null) return;

            var strType = TreeView.SelectedItem.GetType().Name == "App" ? "Application" : "Credential";
            var dlgResult = MessageBox.Show(string.Format("Are you sure you want to delete this {0}?", strType)
                , string.Format("Delete {0}", strType)
                , MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (dlgResult == MessageBoxResult.Cancel) return;

            if (TreeView.SelectedItem.GetType().Name == "App")
            {
                var app = TreeView.SelectedItem as Tao.CredentialStore.App;
                MyApplications.Remove(app);
            }
            else
            {
                if (TreeView.SelectedItem.GetType().Name == "Credential")
                {
                    var credential = (Credential)TreeView.SelectedItem;
                    var app = MyApplications.First(a => a.Name == credential.Application);
                    app.Credentials.Remove(credential);
                }
            }
        }
        private void DeleteItem_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
                e.CanExecute = true;
        }
        private void ExitCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ExitCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void SaveCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            var parentWindow = (MainWindow)Window.GetWindow(this);
            if (parentWindow != null)
                e.CanExecute = parentWindow.RequiresSave;
        }
        private void SaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var parentWindow = (MainWindow)Window.GetWindow(this);
            parentWindow?.SaveFile();
        }

        #endregion

        #region Methods
        private void LoadControlIntoGrid(Type type, object data)
        {
            RemoveControlsFromGridColumn(1);

            // Display new userControl in Grid
            switch (type.Name)
            {
                case "App":
                    var applicationControl = new UcEditApplication {DataContext = data};
                    Grid.SetColumn(applicationControl, 1);
                    GridMain.Children.Add(applicationControl);
                    break;
                case "Credential":
                    var ucEditCredential = new UcEditCredential {DataContext = data};
                    Grid.SetColumn(ucEditCredential, 1);
                    GridMain.Children.Add(ucEditCredential);
                    break;
            }
        }
        private void RemoveControlsFromGridColumn(int column)
        {
            foreach (UIElement control in GridMain.Children)
            {
                if (Grid.GetColumn(control) == column)
                {
                    if (control.DependencyObjectType.Name != "DockPanel")
                    {
                        GridMain.Children.Remove(control);
                        break;
                    }
                }
            }
        }
        public void SetDataSource(ObservableCollection<Tao.CredentialStore.App> app)
        {
            MyApplications = app;
            LoadControlIntoGrid(app.GetType(), app);
            TreeView.DataContext = MyApplications;
            TreeView.ItemsSource = MyApplications;            
        }
        #endregion

        protected virtual void OnKasteloPropertyChanged(PropertyChangedEventArgs e)
        {
            KasteloPropertyChanged?.Invoke(this, e);
        }
    }
}