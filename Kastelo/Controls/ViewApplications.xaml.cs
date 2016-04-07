using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Tao.CredentialStore;

namespace Kastelo.Controls
{
    /// <summary>
    /// Interaction logic for ViewApplications.xaml
    /// </summary>
    public partial class ViewApplications
    {
        private ObservableCollection<Tao.CredentialStore.App> _myList;
        private readonly EditCredential _editCredential;
        
        public ViewApplications()
        {
            InitializeComponent();
            var myList = CreateApplication();
            _editCredential = new EditCredential();
            ApplicationListBox.DataContext = myList;
            ApplicationListBox.ItemsSource = myList;
        }
        
        protected ObservableCollection<Tao.CredentialStore.App> CreateApplication()
        {
            _myList = new ObservableCollection<Tao.CredentialStore.App>();
            for (int i = 1; i < 10; i++)
                _myList.Add(new Tao.CredentialStore.App("Application" + i));


            foreach (var app in _myList)
            {
                for (var i = 1; i < 15; i++)
                    app.Credentials.Add(new Credential("Username " + i, app.Name + " password " + i));

                app.Notes = string.Format("Here are some notes for application{0}", app.Name);
            }

            return _myList;
        }

        private void ApplicationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var obj = e.AddedItems[0] as Tao.CredentialStore.App;
                if (obj == null) return;
                CredentialsListBox.DataContext = obj.Credentials;
                CredentialsListBox.ItemsSource = obj.Credentials;
                ApplicationNotesTextBlock.Text = obj.Notes;
                ApplicationName.Text = obj.Name;
            }
            else
            {

            }


        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            _editCredential.DataContext = CredentialsListBox.SelectedItem;

            StackPanel stk = (StackPanel)((Button)sender).Parent;
            DockPanel a = (DockPanel)stk.Parent;
            Border b = (Border)a.Parent;
            StackPanel c = (StackPanel)b.Parent;
            c.Children.Clear();
            c.Children.Add(_editCredential);
            _editCredential.Width = c.ActualWidth;
            //Window b = new GeneratePasswordWindow();
            //b.ShowDialog();
        }

        private void Copy_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedCredential = CredentialsListBox.SelectedItem as Credential;
            var selectedApplication = ApplicationListBox.SelectedItem as Tao.CredentialStore.App;
            if (selectedCredential != null && selectedApplication != null)
                Clipboard.SetText(selectedCredential.Password);
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedCredential = CredentialsListBox.SelectedItem as Credential;
            var selectedApplication = ApplicationListBox.SelectedItem as Tao.CredentialStore.App;

            var a = _myList.IndexOf(selectedApplication);
            _myList[a].Credentials.Remove(selectedCredential);            
        }
    }
}
