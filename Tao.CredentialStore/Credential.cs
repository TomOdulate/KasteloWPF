using System;
using System.ComponentModel;

namespace Tao.CredentialStore
{
    [Serializable]
    public class Credential : IStoreableItem, INotifyPropertyChanged
    {
        #region Private properties
        private string _username;
        private string _password;
        private string _comment;
        private DateTime _created;
        private DateTime _lastUpdated;
        #endregion

        #region Getters / Setters
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
                LastUpdated = DateTime.Now;
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
                LastUpdated = DateTime.Now;
            }
        }
        public DateTime Created
        {
            get { return _created; }
            set
            {
                _created = value;
                OnPropertyChanged("Created");
                LastUpdated = DateTime.Now;
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
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
                LastUpdated = DateTime.Now;
            }
        }
        public string Application { get; set; }
        #endregion

        #region Events   
        [field: NonSerialized]       
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler CredentialsKasteloPropertyChanged;
            
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CredentialsKasteloPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructors
        public Credential()
        {

        }
        public Credential(Credential credential)
        {
            Username = credential.Username;
            Password = credential.Password;
            Created = LastUpdated = DateTime.Now;
            Comment = credential.Comment;
        }
        public Credential(string username, string password, string comment = "")
        {
            Created = LastUpdated = DateTime.Now;
            Username = username;
            Password = password;
            Comment = comment;
        }
        public Credential(string username, string password, string application, string comment = "")
        {
            Created = LastUpdated = DateTime.Now;
            Username = username;
            Password = password;
            Comment = comment;
            Application = application;
        }
        #endregion
    }
}
