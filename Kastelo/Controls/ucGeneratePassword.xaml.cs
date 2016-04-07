using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Tao.CredentialStore;

namespace Kastelo.Controls
{
    public partial class UcGeneratePassword : INotifyPropertyChanged
    {
        #region Properties
        private string _newPassword;
        private bool _bAlpha = true;
        private bool _bNumeric = true;
        private bool _bPunctuation = true;
        private bool _bBrackets = true;
        private bool _bSpecials1 = true;
        private bool _bSpecials2;

        #region Allowable Character Definitions
        private readonly string _strAlpha = Properties.Resources.CharsAlpha;
        private readonly string _strNumeric = Properties.Resources.CharsNumeric;
        private readonly string _strPunctuation = Properties.Resources.CharsPunct;
        private readonly string _strBrackets = Properties.Resources.CharsBrackets;
        private readonly string _strSpecials1 = Properties.Resources.CharsSpecial1;
        private readonly string _strSpecials2 = Properties.Resources.CharsSpecial2;
        #endregion
        
        #endregion

        #region Getters / Setters
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                OnPropertyChanged(new PropertyChangedEventArgs("NewPassword"));
            }
        }
        public bool Alpha
        {
            get { return _bAlpha; }
            set
            {
                _bAlpha = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Alpha"));
            }
        }
        public bool Numeric
        {
            get { return _bNumeric; }
            set
            {
                _bNumeric = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Numeric"));
            }
        }
        public bool Punctuation
        {
            get { return _bPunctuation; }
            set
            {
                _bPunctuation = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Punctuation"));
            }
        }
        public bool Brackets
        {
            get { return _bBrackets; }
            set
            {
                _bBrackets = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Brackets"));
            }
        }
        public bool Specials1
        {
            get { return _bSpecials1; }
            set
            {
                _bSpecials1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Specials1"));
            }
        }
        public bool Specials2
        {
            get
            {
                return _bSpecials2;
            }
            set
            {
                _bSpecials2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Specials2"));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CancelClick;
        public event EventHandler DoneClick;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var additionalChars = string.Empty;
            if (OptOther.IsChecked == true)
                additionalChars = OptExampleOther.Text;

            try
            {
                NewPassword = PasswordGenerator.GeneratePassword(
                    GenerateAllowableCharsArray(additionalChars)
                        , PasswordLength.Value ?? default(int));
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show(Properties.Resources.No_Chars_Selected_Msg
                    , Properties.Resources.No_Chars_Selected_Msg_Caption
                    , MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void OnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClick?.Invoke(this, e);                
        }
        private void OnDone_Click(object sender, RoutedEventArgs e)
        {
            DoneClick?.Invoke(this, e);
        }
        private void PasswordText_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PasswordLength.Value = PasswordTextBox.Text.Length;
            SetPasswordStrengthMeter((int)
                PasswordGenerator.CalculatePasswordStrength(PasswordTextBox.Text));
        }
        #endregion

        public UcGeneratePassword()
        {
            InitializeComponent();
            DataContext = this;
            OptExampleAlpha.Text        = "[a-z]";
            OptExampleNumeric.Text      = _strNumeric;
            OptExampleBrackets.Text     = _strBrackets;
            OptExamplePunctuation.Text  = _strPunctuation;
            OptExampleSpecials1.Text    = _strSpecials1;
            OptExampleSpecials2.Text    = _strSpecials2;

            NewPassword = PasswordGenerator.GeneratePassword(
                GenerateAllowableCharsArray(), PasswordLength.Value ?? 10);
         }
        private void SetPasswordStrengthMeter(int strength)
        {
            var brush = Brushes.Red;
            var msg  = "Weak";
            
            if (strength > 1600)
            {
                msg = "Very Strong";
                brush = Brushes.LawnGreen;
            }
            else
            {
                if (strength > 1000)
                {
                    msg = "Strong";
                    brush = Brushes.YellowGreen;
                }
                else
                {
                    if (strength > 600)
                    {
                        msg = "Good";
                        brush = Brushes.Yellow;
                    }
                    else
                    {
                        if (strength > 450)
                        {
                            msg = "Average";
                            brush = Brushes.Orange;
                        }
                        else
                        {
                            if (strength > 350)
                            {
                                msg = "Not Great";
                                brush = Brushes.Coral;
                            }
                        }
                    }
                }
            }

            PasswordStrengthContainer.Background = brush;
            PasswordStrength.Text = msg;
        }
        private List<char> GenerateAllowableChars(string additionalChars = "")
        {
            var allowableChars = new List<char>();
            if(additionalChars != string.Empty) allowableChars.AddRange(additionalChars.ToCharArray());
            if (_bAlpha) allowableChars.AddRange(_strAlpha.ToCharArray());
            if (_bNumeric) allowableChars.AddRange(_strNumeric.ToCharArray());
            if (_bPunctuation) allowableChars.AddRange(_strPunctuation.ToCharArray());
            if (_bBrackets) allowableChars.AddRange(_strBrackets.ToCharArray());
            if (_bSpecials1) allowableChars.AddRange(_strSpecials1.ToCharArray());
            if (_bSpecials2) allowableChars.AddRange(_strSpecials2.ToCharArray());

            if (allowableChars.Count < 1)
                throw new ArgumentNullException("additionalChars");

            if (additionalChars.Length > 0)
            {
                foreach (var c in additionalChars.Trim().ToCharArray()
                    .Where(c => !allowableChars.Contains(c)))
                {
                    allowableChars.Add(c);
                }
            }

            return allowableChars;
        }
        private char[] GenerateAllowableCharsArray(string additionalChars = "")
        {
            return GenerateAllowableChars(additionalChars).ToArray();
        }
    }

    public class ConvertCheckBoxToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "True" ? "Visible" : "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Convert back not implemented.");
        }
    }


}
