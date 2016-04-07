using System;
using System.Windows;

namespace Kastelo
{
    public partial class InputDialog
    {
        public string Answer => TxtAnswer.Text; 
        private bool _useHidden;       

        public InputDialog(string question,string title = "", string defaultAnswer = "", bool useHiddenChars = false)
        {
            _useHidden = useHiddenChars;
            InitializeComponent();
            this.Icon = null;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            if (title.Length > 0)
                Title = title;

            if (useHiddenChars)
            {
                TxtAnswer.Visibility = Visibility.Collapsed;
                TxtAnswerHidden.Visibility = Visibility.Visible;
                TxtAnswerHidden.Focus();
            }
            else
            {
                TxtAnswerHidden.Visibility = Visibility.Collapsed;
                TxtAnswer.Focus();
            }

            LblQuestion.Text = question;
            TxtAnswer.Text = defaultAnswer;            
        }
        
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (_useHidden)
                TxtAnswer.Text = TxtAnswerHidden.Password;
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TxtAnswer.SelectAll();
            TxtAnswer.Focus();
        }        
    }
}

