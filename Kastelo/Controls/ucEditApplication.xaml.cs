using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Kastelo.Controls
{
    /// <summary>
    /// Interaction logic for ucEditApplication.xaml
    /// </summary>
    public partial class UcEditApplication : INotifyPropertyChanged
    {
        private bool _editMode;

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
        private void SetMode(bool mode)
        {
            EditDone.Content = mode ? "Done" : "Edit";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands
        private void CancelEdit_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditMode;
        }
        private void CancelEdit_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Application.Undo();
            Notes.Undo();
            EditMode = false;
        }
        private void DoneEdit_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            EditMode = !EditMode;
            Application.Focus();                        
        }
        private void DoneEdit_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        public UcEditApplication()
        {
            InitializeComponent();
        }

        private void SelectText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
                ((TextBox)sender).SelectAll();
        }

    }
}
