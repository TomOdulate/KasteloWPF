using Kastelo.Properties;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Kastelo.Controls
{
    public partial class UcSetupKey
    {
        public int KeySize = 32;
        private const int Interval = 400;
        private List<byte> _myList = new List<byte>(0);
        private readonly SolidColorBrush _brushGreen = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush _brushGray = new SolidColorBrush(Colors.Gray);
        private int _count;
        private int _maxcount;
        private bool _started;

        public UcSetupKey()
        {
            InitializeComponent();
            MyProgressBar.Maximum = KeySize;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(_started)
                KeyCanvas.Background = _brushGreen;

            e.Handled = true;
            _maxcount++;           
            
            if (_started && _maxcount % Interval == 0 )
            {
                var point = e.GetPosition(KeyCanvas);
                if (_myList.Count < KeySize)
                {
                    if (_count % 2 == 0)
                        _myList.Add((byte)point.X);
                    else
                        _myList.Add((byte)point.Y);

                    MyProgressBar.Value = _myList.Count;
                    _count++;
                }
                else
                {
                    _started = false;
                    KeyCanvas.Background = _brushGray;
                    Helper.SaveKeyToSettings(_myList.ToArray());
                    Helper.ResetSettingsVector();
                    MessageBox.Show("Finished", "Setup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    Settings.Default.IsFirstRun = false;
                    Settings.Default.Save();

                    // Unload this control;
                    var parentWindow = (MainWindow)Window.GetWindow(this);
                    if (parentWindow != null)
                    {
                        parentWindow.MainGrid.Children.Clear();
                        parentWindow.MainMenu.IsEnabled = true;
                    }
                }                    
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _myList = new List<byte>(0);
            _count = 0;
            _maxcount = 0;
            MyProgressBar.Value = 0;
            _started = true;
            KeyCanvas.Focus();            
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            KeyCanvas.Background = _brushGray;
        }
        private void UcSetupKey_OnLoaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MainWindow)Window.GetWindow(this);
            if (parentWindow != null)
                parentWindow.MainMenu.IsEnabled = false;
        }
    }
}
