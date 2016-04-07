using System;
using System.Windows;

namespace Kastelo.Controls
{
    /// <summary>
    /// Interaction logic for ucStart.xaml
    /// </summary>
    public partial class UcStart
    {
        public event EventHandler LoadClick;
        public event EventHandler NewClick;
        public event EventHandler ExitClick;

        public UcStart()
        {
            InitializeComponent();
        }

        private void OnNew_Click(object sender, RoutedEventArgs e)
        {
            NewClick?.Invoke(this, new RoutedEventArgs());
        }

        private void OnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadClick?.Invoke(this, new RoutedEventArgs());
        }

        private void OnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitClick?.Invoke(this, new RoutedEventArgs());
        }
    }
}
