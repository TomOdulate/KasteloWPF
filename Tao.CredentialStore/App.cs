using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Remoting.Channels;

namespace Tao.CredentialStore
{
    [Serializable]
    public class App : IItemStore, IStoreableItem, INotifyPropertyChanged
    {
        #region Private Properties
        private string _name;
        private string _notes;
        private DateTime _created;
        private DateTime _lastUpdated;
        [NonSerialized] private ObservableCredentialStore<Credential> _credentials;
        public List<Credential> _credentialList = null; // Used only for serialise / Deserialise
        #endregion

        #region Getters / Setters        
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                LastUpdated = DateTime.Now;
                if(Credentials!=null)
                {
                    foreach (var credential in Credentials)
                        credential.Application = value;
                }                
                OnPropertyChanged("Name");
            }
        }
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged("Notes");
                LastUpdated = DateTime.Now;
            }
        }        
        public DateTime Created
        {
            get { return _created; }
            set
            {
                _created = value;
                LastUpdated = DateTime.Now;
                OnPropertyChanged("Created");
            }
        }
        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                OnPropertyChanged("LastUpdated");                
            }
        }
        public ObservableCredentialStore<Credential> Credentials
        {
            get { return _credentials;}
            set
            {
                _credentials = value;
                OnCredentials_KasteloPropertyChanged(this, new PropertyChangedEventArgs("Credentials"));
            }
        }
        #endregion

        #region Events
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler ApplicationKasteloPropertyChanged;
        
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnCredentials_KasteloPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplicationKasteloPropertyChanged?.Invoke(sender, e);
        }
        #endregion

        #region Constructors
        public App()
        {
            Name = "New"; 
            Credentials = new ObservableCredentialStore<Credential>();
            Created = LastUpdated = DateTime.Now;
        }
        public App(string name)
        {
            Name = name;
            Credentials = new ObservableCredentialStore<Credential>();            
            Created = LastUpdated = DateTime.Now;
        }

        public bool Add(IStoreableItem item)
        {
            Credentials.Add(item as Credential);
            return true;
        }
        #endregion

        #region Methods
        public bool Remove(IStoreableItem item)
        {
            return Credentials.Remove(item as Credential);
        }
        public int Count()
        {
            return Credentials.Count;
        }
        #endregion
    }
}
