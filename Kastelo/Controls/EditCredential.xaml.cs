using System.Windows;
using System.Windows.Controls;
using Tao.CredentialStore;

namespace Kastelo.Controls
{
    /// <summary>
    /// Interaction logic for EditCredential.xaml
    /// </summary>    
    
    public partial class EditCredential
    {
        readonly UcGeneratePassword _ucGenerate = new UcGeneratePassword();
        public EditCredential()
        {
            InitializeComponent();
            var uc = new Credential("Username", "password", "comment");
            _ucGenerate.CancelClick += UcGenerate_Cancel_Click;
            _ucGenerate.Visibility = Visibility.Collapsed;
            _ucGenerate.DoneClick += UcGenerate_Done_Click;
            ButtonStackPanel.Children.Add(_ucGenerate);
            DataContext = uc;
        }

        private void UcGenerate_Done_Click(object sender, System.EventArgs e)
        {
            var uc = (UcGeneratePassword)sender;
            Password.Text = uc.NewPassword;
            UcGenerate_Cancel_Click(null, null);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {   
            foreach(Control b in ButtonStackPanel.Children)
                if(b.GetType() == typeof(Button))
                    b.Visibility = Visibility.Hidden;

            _ucGenerate.HorizontalAlignment = HorizontalAlignment.Center;
            _ucGenerate.Visibility = Visibility.Visible;            
        }

        private void UcGenerate_Cancel_Click(object sender, System.EventArgs e)
        {
            // Hide the generate password usercontrol
            ButtonStackPanel.Children[3].Visibility = Visibility.Collapsed;

            // Reshow the buttons
            foreach (Control b in ButtonStackPanel.Children)
                if(b.Visibility == Visibility.Hidden)
                    b.Visibility = Visibility.Visible;
        }
    }
}

