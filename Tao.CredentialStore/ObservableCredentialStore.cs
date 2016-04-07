using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Tao.CredentialStore
{
    /// <summary>
    /// This class is used only to override the SetItem & InsertItem methods of the
    /// ObservableCollection<T> class.  Simply because after serialisation the 
    /// ObservableCollection<T> loses its event handler references.  This class
    /// adds the references when an item is inserted or set.
    /// </summary>
    /// <typeparam name="T">A storeable item of type Tao.CredentialStore.App or credential</typeparam>
    [Serializable]
    public class ObservableCredentialStore<T> : ObservableCollection<T>, INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler KasteloPropertyChanged;
        protected override event PropertyChangedEventHandler PropertyChanged;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            switch (item.GetType().Name)
            {
                case "App":
                    var app = item as Tao.CredentialStore.App;
                    app.PropertyChanged += OnPropertyChanged;
                    break;
                case "Credential":
                    var cred = item as Tao.CredentialStore.Credential;
                    cred.PropertyChanged += OnPropertyChanged;
                    break;
            }
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            switch (item.GetType().Name)
            {
                case "App":
                    var app = item as Tao.CredentialStore.App;
                    app.PropertyChanged += OnPropertyChanged;
                    break;
                case "Credential":
                    var cred = item as Tao.CredentialStore.Credential;
                    cred.PropertyChanged += OnPropertyChanged;
                    break;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            KasteloPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
            PropertyChanged?.Invoke(this, e);
        }
    }
}
