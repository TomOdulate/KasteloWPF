using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace Tao.CredentialStore
{
    [Serializable]
    public class ApplicationStore : PasswordGenerator, IItemStore, INotifyPropertyChanged
    {
        #region Private properties        
        protected static byte[] Key;
        protected byte[] Vector;
        private string _name;
        private string _notes;
        private byte[] _hash;        
        private DateTime _lastUpdated;
        private DateTime _created;
        [NonSerialized]        
        private ObservableCredentialStore<App> _applications;
        private List<App> _applicationList;     // Used only for serialisation & Deserialisation
        #endregion

        #region Getters / Setters
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
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
            }
        }
        public byte[] Hash
        {
            get { return _hash; }
            set
            {
                _hash = value;
                OnPropertyChanged("Hash");
            }
        }        
        public ObservableCredentialStore<App> Applications
        {
            get { return _applications; }
            set
            {
                _applications = value;
                OnApplications_KasteloPropertyChanged(this, new PropertyChangedEventArgs("Applications"));
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
        public DateTime Created
        {
            get { return _created; }
            set
            {
                _created = value;
                OnPropertyChanged("Created");
            }
        }
        #endregion

        #region Events
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler ApplicationsKasteloPropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnApplications_KasteloPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplicationsKasteloPropertyChanged?.Invoke(sender, e);
        }
        #endregion

        #region Constructors
        public ApplicationStore(string name, byte[] key, byte[] iVector)
        {
            Name = name;
            Applications = new ObservableCredentialStore<App>();            
            Created = LastUpdated = DateTime.Now;
            Key = key;
            Vector = iVector;
        }      
        public ApplicationStore(ApplicationStore store, byte[] key, byte[] iVector)
        {
            Name = store.Name;
            Notes = store.Notes;
            Applications = store.Applications;            
            Hash = store.Hash;
            Created = store.Created;
            LastUpdated = store.LastUpdated;
            Key = key;
            Vector = iVector;
        }
        public ApplicationStore()
        {
            Applications = new ObservableCredentialStore<App>();
        }
        #endregion

        #region Methods
        public virtual bool Add(IStoreableItem item)
        {
            if (item.GetType() != typeof (App)) return false;
            var newItem = (App)item;
            if (Applications.Contains(newItem)) //.Exists(x => newItem != null && x.Name == newItem.Name))
                return false;

            Applications.Add(newItem);
            return true;
        }
        public bool Remove(IStoreableItem item)
        {
            return Applications.Remove(item as App);
        }
        public int Count()
        {
            return Applications.Count;
        }
        public static byte[] EncryptObjectToBytes(ApplicationStore sourceObject, byte[] iv)
        {
            byte[] encrypted;
            using (var algorithm = Aes.Create())
            {
                // ReSharper disable once PossibleNullReferenceException
                algorithm.Key = Key;
                algorithm.IV = iv;
                algorithm.Padding = PaddingMode.PKCS7;

                // Create a encryptor to perform the stream transform.
                ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        var swEncrypt = new BinaryFormatter();
                        //Write all data to the stream.
                        swEncrypt.Serialize(csEncrypt, sourceObject);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        public static ApplicationStore DecryptObjectFromBytes(byte[] cipherText, byte[] iv)
        {
            // Declare the string used to hold the decrypted text.
            ApplicationStore plainobject;

            // Create an Rijndael object with the specified key and IV.
            using (var algorithm = Aes.Create())
            {
                // ReSharper disable once PossibleNullReferenceException
                algorithm.Key = Key;
                algorithm.IV = iv;
                algorithm.Padding = PaddingMode.PKCS7;

                // Create a decrytor to perform the stream transform.
                var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            var binaryFormatter = new BinaryFormatter();
                            plainobject = (ApplicationStore)binaryFormatter.Deserialize(srDecrypt.BaseStream);
                        }
                    }
                }
            }
            return plainobject;
        }
        public bool Save(string filePath, byte[] iv)
        {
            // ObservableCollection does not serialise event handlers correctly,
            // so using a 'temporary' List<T> for serialisation instead!
            _applicationList = new List<App>(Applications);
            for (var i = 0; i < Applications.Count; i++)
                _applicationList[i]._credentialList = new List<Credential>(Applications[i].Credentials);

            // Encrypt this object
            var encryptedBytes = EncryptObjectToBytes(this, iv);

            // Write these encrypted bytes to a file.
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(encryptedBytes.ToArray(),0,encryptedBytes.Length);
                return true;
            }
        }
        public void Load(string filePath, byte[] iv, byte[] hashBytes)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            // Create a byte array from the file contents & decrypt it.
            var encryptedBytes = File.ReadAllBytes(filePath);
            var decryptedAppStore = DecryptObjectFromBytes(encryptedBytes, iv);

            // Check password
            if (String.CompareOrdinal(Encoding.ASCII.GetString(hashBytes)
                , Encoding.ASCII.GetString(decryptedAppStore.Hash)) != 0)
            {
                throw new AuthenticationException("Incorrect password.");
            }   

            // Update this object with the values from the decrypted object.
            Name = decryptedAppStore.Name;
            Notes = decryptedAppStore.Notes;
            Hash = decryptedAppStore.Hash;
            
            // Recreate the ObservableCollection<T> from the serialised List<T>
            // since serialising an ObservableCollection is problematic!
            for (var i = 0; i < decryptedAppStore._applicationList.Count; i++)
            {
                Applications.Add(decryptedAppStore._applicationList[i]);
                for (var j = 0; j < decryptedAppStore._applicationList[i]._credentialList.Count; j++)
                {
                    if(Applications[i].Credentials==null)
                        Applications[i].Credentials = new ObservableCredentialStore<Credential>();

                    Applications[i].Credentials.Add(decryptedAppStore._applicationList[i]._credentialList[j]);
                }
            }
            _applicationList = null;
        }
        #endregion
    }    
}