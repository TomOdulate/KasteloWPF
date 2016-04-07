using Kastelo.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Tao.CredentialStore;

namespace Kastelo
{
    class Helper
    {
        public static bool IsSetupNeeded()
        {
            return Settings.Default.IsFirstRun;
        }
        public static ObservableCredentialStore<Tao.CredentialStore.App> ApplicationFactory()
        {
            var store = new ApplicationStore("tmp", ReadKeyFromSettings(),ReadVectorFromSettings());
            for (var i = 1; i < 10; i++)
                store.Applications.Add(new Tao.CredentialStore.App("Application" + i));            

            foreach (var app in store.Applications)
            {
                for (var i = 1; i < 15; i++)
                    app.Credentials.Add(new Credential("Username " + i, app.Name + " password " + i,
                                                        app.Name, String.Format("Notes for 'credential {0}'", i)));

                app.Notes = String.Format("Here are some notes for application '{0}'", app.Name);
            }
            return store.Applications;
        }
        public static void ResetSettingsVector()
        {
            byte[] iv = { 187, 201, 13, 144, 55, 116, 79, 18, 45, 10, 121, 44, 3, 124, 152, 164 };
            Settings.Default.IV = Convert.ToBase64String(iv);
            Console.WriteLine(Convert.ToBase64String(iv));
            Settings.Default.Save();            
        }
        public static byte[] ReadVectorFromSettings()
        {
            return Convert.FromBase64String(Settings.Default.IV);
        }
        public static byte[] ReadKeyFromSettings()
        {
            return Convert.FromBase64String(Settings.Default.Key);
        }
        public static void SaveKeyToSettings(byte[] newKey)
        {
            Settings.Default.Key = Convert.ToBase64String(newKey);
            Settings.Default.Save();
        }
        public static byte[] GetHashFromString(string cleartext)
        {
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(Encoding.ASCII.GetBytes(cleartext));
        }
        public static FileDialog GetFileDialog(bool load, string filename = "")
        {
            var fd = load ? (FileDialog)new OpenFileDialog() : new SaveFileDialog();
            fd.AddExtension = true;
            fd.FileName = filename == "" ? "Store.bin" : filename;
            fd.DefaultExt = ".bin";
            fd.CheckPathExists = true;
            fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            fd.Filter = "Kastelo File|*.bin|All Files|*.*";
            fd.RestoreDirectory = true;
            return fd;
        }
        public static void ExportAKey(byte[] key, string fullFilePath)
        {
            var fStream = File.Create(fullFilePath);
            fStream.Write(key, 0, key.Length);
            fStream.Flush();
            fStream.Close();
        }
        public static byte[] ImportAKey(string fullFilePath)
        {
            var key = new byte[32];
            var fStream = File.OpenRead(fullFilePath);
            fStream.Read(key, 0, 32);
            fStream.Flush();
            fStream.Close();
            return key;
        }
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(serializableObject.GetType());
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }
            T objectOut;
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            var xmlString = xmlDocument.OuterXml;

            using (var read = new StringReader(xmlString))
            {
                var outType = typeof(T);

                var serializer = new XmlSerializer(outType);
                using (XmlReader reader = new XmlTextReader(read))
                {
                    objectOut = (T)serializer.Deserialize(reader);
                    reader.Close();
                }

                read.Close();
            }

            return objectOut;
        }
    }
}
