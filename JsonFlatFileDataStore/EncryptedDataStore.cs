using System;

namespace JsonFlatFileDataStore
{
    public class EncryptedDataStore : DataStore
    {
        private readonly Func<string, string> _encryptJson;
        private readonly Func<string, string> _decryptJson;
        
        public EncryptedDataStore(string path, string encryptionKey, bool useLowerCamelCase = true, string keyProperty = null, bool reloadBeforeGetCollection = false) : base(path,useLowerCamelCase,keyProperty, reloadBeforeGetCollection)
        {
            var aes256 = new Aes256();
            _encryptJson = json => aes256.Encrypt(json, encryptionKey);
            _decryptJson = json => aes256.Decrypt(json, encryptionKey);
            Reload();
        }

        // Virtual methods are called from base-classes constructor, which is a bad practice. We have to use "guard" null checks and load data in own constructor.
        protected override string PrepareJsonForWriting(string json) => _encryptJson == null ? "{}" : _encryptJson(json);
        protected override string PrepareJsonForUsing(string json) => _decryptJson == null ? "{}" : _decryptJson(json);
    }
}