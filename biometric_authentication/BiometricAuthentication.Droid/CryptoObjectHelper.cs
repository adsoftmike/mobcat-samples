﻿using System;
using Android.Security.Keystore;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Util;
using Java.Security;
using Javax.Crypto;

namespace BiometricAuthentication.Droid
{
    public class CryptoObjectHelper
    {
        static readonly string TAG = "X:" + typeof(CryptoObjectHelper).Name;
        static readonly string KEY_NAME = "BasicFingerPrintSample.FingerprintManagerAPISample.sample_fingerprint_key";
        static readonly string KEYSTORE_NAME = "AndroidKeyStore";
        static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;

        static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;

        readonly KeyStore _keystore;

        public CryptoObjectHelper()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }

        public FingerprintManagerCompat.CryptoObject BuildCryptoObject()
        {
            Cipher cipher = CreateCipher();
            return new FingerprintManagerCompat.CryptoObject(cipher);
        }

        /// <summary>
        ///     Creates the cipher.
        /// </summary>
        /// <returns>The cipher.</returns>
        /// <param name="retry">If set to <c>true</c>, recreate the key and try again.</param>
        Cipher CreateCipher(bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(TRANSFORMATION);
            try
            {
                cipher.Init(CipherMode.EncryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                Log.Debug(TAG, "The key was invalidated, creating a new key.");
                _keystore.DeleteEntry(KEY_NAME);
                if (retry)
                {
                    CreateCipher(false);
                }
                else
                {
                    throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        /// <summary>
        ///     Will get the key from the Android keystore, creating it if necessary.
        /// </summary>
        /// <returns></returns>
        IKey GetKey()
        {
            if (!_keystore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            IKey secretKey = _keystore.GetKey(KEY_NAME, null);
            return secretKey;
        }

        /// <summary>
        ///     Creates the Key for fingerprint authentication.
        /// </summary>
        void CreateKey()
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            KeyGenParameterSpec keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
            Log.Debug(TAG, "New key created for fingerprint authentication.");
        }
    }
}
