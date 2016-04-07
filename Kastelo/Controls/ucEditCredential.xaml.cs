using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kastelo.Controls
{
    public partial class UcEditCredential : INotifyPropertyChanged
    {
        private bool _editMode;
        private string _oldPassword;

        public UcEditCredential()
        {
            InitializeComponent();
            GeneratePasswordControl.DoneClick += GeneratePasswordControl_Done_Click;
            GeneratePasswordControl.CancelClick += GeneratePasswordControl_Cancel_Click;
            Loaded += UcEditCredential_Loaded;
        }

        private void UcEditCredential_Loaded(object sender, RoutedEventArgs e)
        {
            _oldPassword = Password.Text;
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
        private void GeneratePasswordControl_Done_Click(object sender, EventArgs e)
        {
            GeneratePasswordStackPanel.Visibility = Visibility.Collapsed;            
            Password.Text = GeneratePasswordControl.PasswordTextBox.Text;
            Password.Focus();
        }
        private void GeneratePasswordControl_Cancel_Click(object sender, EventArgs e)
        {
            GeneratePasswordStackPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Commands
        private void CancelEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditMode;
        }
        private void CancelEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Username.Undo();
            if(string.IsNullOrEmpty(_oldPassword))
                Password.Undo(); 
            else
                Password.Text = _oldPassword;
            Comments.Undo();
            EditMode = false;
        }
        private void DoneEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void DoneEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditMode = !EditMode;
            Username.Focus();                        
        }
        private void GeneratePasswordButton_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditMode;
        }
        private void GeneratePasswordButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GeneratePasswordStackPanel.Visibility = Visibility.Visible;
            Password.Focus();
        }
        private void CopyPasswordButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(Password.Text);
        }
        private void CopyPasswordButton_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Password.Text.Length > 0)
                e.CanExecute = true;
        }
        #endregion

        #region Methods
        private void SetMode(bool mode)
        {
            EditDone.Content = mode ? "Done" : "Edit";
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                SetMode(value);
                OnPropertyChanged("EditMode");
            }
        }
        private void SelectText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
                ((TextBox)sender).SelectAll();
        }
        #endregion Methods        
    }
}