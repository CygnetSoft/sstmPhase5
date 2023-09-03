using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SSTM.Helpers.Common
{
    public class SecurityMethods
    {
        private static readonly string password = "7x!A%D*G-KaPdSgVkYp3s6v9y/B?E(H+MbQeThWmZq4t7w!z%C&F)J@NcRfUjXn2";

        //public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        //{
        //    byte[] encryptedBytes = null;

        //    // Set your salt here, change it to meet your flavor:
        //    byte[] saltBytes = passwordBytes;
        //    // Example:
        //    //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (RijndaelManaged AES = new RijndaelManaged())
        //        {
        //            AES.KeySize = 256;
        //            AES.BlockSize = 128;

        //            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
        //            AES.Key = key.GetBytes(AES.KeySize / 8);
        //            AES.IV = key.GetBytes(AES.BlockSize / 8);

        //            AES.Mode = CipherMode.CBC;

        //            using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
        //                cs.Close();
        //            }
        //            encryptedBytes = ms.ToArray();
        //        }
        //    }

        //    return encryptedBytes;
        //}

        //public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        //{
        //    byte[] decryptedBytes = null;
        //    // Set your salt here to meet your flavor:
        //    byte[] saltBytes = passwordBytes;
        //    // Example:
        //    //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (RijndaelManaged AES = new RijndaelManaged())
        //        {
        //            AES.KeySize = 256;
        //            AES.BlockSize = 128;

        //            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
        //            AES.Key = key.GetBytes(AES.KeySize / 8);
        //            AES.IV = key.GetBytes(AES.BlockSize / 8);

        //            AES.Mode = CipherMode.CBC;

        //            using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
        //                cs.Close();
        //            }
        //            decryptedBytes = ms.ToArray();
        //        }
        //    }

        //    return decryptedBytes;
        //}

        //public static string Encrypt(string text, byte[] passwordBytes)
        //{
        //    byte[] originalBytes = Encoding.UTF8.GetBytes(text);
        //    byte[] encryptedBytes = null;

        //    // Hash the password with SHA256
        //    passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        //    // Getting the salt size
        //    int saltSize = GetSaltSize(passwordBytes);
        //    // Generating salt bytes
        //    byte[] saltBytes = GetRandomBytes(saltSize);

        //    // Appending salt bytes to original bytes
        //    byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
        //    for (int i = 0; i < saltBytes.Length; i++)
        //    {
        //        bytesToBeEncrypted[i] = saltBytes[i];
        //    }
        //    for (int i = 0; i < originalBytes.Length; i++)
        //    {
        //        bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
        //    }

        //    encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

        //    return Convert.ToBase64String(encryptedBytes);
        //}

        //public static string Decrypt(string decryptedText, byte[] passwordBytes)
        //{
        //    byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);

        //    // Hash the password with SHA256
        //    passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        //    byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

        //    // Getting the size of salt
        //    int saltSize = GetSaltSize(passwordBytes);

        //    // Removing salt bytes, retrieving original bytes
        //    byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
        //    for (int i = saltSize; i < decryptedBytes.Length; i++)
        //    {
        //        originalBytes[i - saltSize] = decryptedBytes[i];
        //    }

        //    return Encoding.UTF8.GetString(originalBytes);
        //}

        //public static int GetSaltSize(byte[] passwordBytes)
        //{
        //    var key = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000);
        //    byte[] ba = key.GetBytes(2);
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < ba.Length; i++)
        //    {
        //        sb.Append(Convert.ToInt32(ba[i]).ToString());
        //    }
        //    int saltSize = 0;
        //    string s = sb.ToString();
        //    foreach (char c in s)
        //    {
        //        int intc = Convert.ToInt32(c.ToString());
        //        saltSize = saltSize + intc;
        //    }

        //    return saltSize;
        //}

        //public static byte[] GetRandomBytes(int length)
        //{
        //    byte[] ba = new byte[length];
        //    RNGCryptoServiceProvider.Create().GetBytes(ba);
        //    return ba;
        //}

        //private static byte[] GetPasswordBytes()
        //{
        //    // The real password characters is stored in System.SecureString
        //    // Below code demonstrates on converting System.SecureString into Byte[]
        //    // Credit: http://social.msdn.microsoft.com/Forums/vstudio/en-US/f6710354-32e3-4486-b866-e102bb495f86/converting-a-securestring-object-to-byte-array-in-net

        //    byte[] ba = null;

        //    if (password.Length == 0)
        //        ba = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        //    else
        //    {
        //        var secureString = new SecureString();
        //        password.ToList().ForEach(item => secureString.AppendChar(item));
        //        secureString.MakeReadOnly();

        //        // Convert System.SecureString to Pointer
        //        IntPtr unmanagedBytes = Marshal.SecureStringToGlobalAllocAnsi(secureString);
        //        try
        //        {
        //            // You have to mark your application to allow unsafe code
        //            // Enable it at Project's Properties > Build
        //            unsafe
        //            {
        //                byte* byteArray = (byte*)unmanagedBytes.ToPointer();

        //                // Find the end of the string
        //                byte* pEnd = byteArray;
        //                while (*pEnd++ != 0) { }
        //                // Length is effectively the difference here (note we're 1 past end) 
        //                int length = (int)((pEnd - byteArray) - 1);

        //                ba = new byte[length];

        //                for (int i = 0; i < length; ++i)
        //                {
        //                    // Work with data in byte array as necessary, via pointers, here
        //                    byte dataAtIndex = *(byteArray + i);
        //                    ba[i] = dataAtIndex;
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            // This will completely remove the data from memory
        //            Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
        //        }
        //    }

        //    return System.Security.Cryptography.SHA256.Create().ComputeHash(ba);
        //}

        public static string StaticEncrypt(string content)
        {
            try
            {
                byte[] contentBytes = Encoding.Unicode.GetBytes(content);
                using (Aes encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(contentBytes, 0, contentBytes.Length);
                                cs.Close();
                            }
                            content = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }

                content = content.Replace('+', ' ');
            }
            catch (Exception ex) {
                string e = ex.Message;
                content = "In Valid";
            }

            return content;
        }

        public static string StaticDecrypt(string cipherText)
        {
            try
            {
                cipherText = cipherText.Replace(' ', '+');

                var cipherBytes = Convert.FromBase64String(cipherText);
                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex) {
                string e = ex.Message;
                cipherText = "In Valid";
            }

            return cipherText;
        }
        /// <summary>
        /// Plain Text and Json Encryption.
        /// </summary>
        /// <param name="base_text"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string JsonSstmEncrypt(string b)
        {
            try
            {
                string j = string.Empty;
                long k = 328477227719771;
                for (int i = 0; i < b.Count(); i++)
                {
                    long z = char.ConvertToUtf32(b, i);
                    if (z > 96 && z < 123)
                    {
                        z += k % 26;
                        if (z > 122)
                        {
                            z = (z - 122) + 96;
                        }
                        else if (z < 97)
                        {
                            z = (z - 97) + 123;
                        }
                    }
                    if (z > 64 && z < 91)
                    {
                        z += k % 26;
                        if (z > 90)
                        {
                            z = (z - 90) + 64;
                        }
                        else if (z < 65)
                        {
                            z = (z - 65) + 91;
                        }
                    }
                    j += Convert.ToChar(z);
                }
                byte[] f = Encoding.UTF8.GetBytes(j);
                j = Convert.ToBase64String(f);
                b = j;
            }
            catch (Exception ex)
            {
                b = ex.Message.ToString();
            }
            return b;
        }
        public static string JsonSstmDecrypt(string s)
        {
            try
            {
                string r = string.Empty;
                long k = 328477227719771;
                if (!string.IsNullOrEmpty(s))
                {
                    byte[] e = Convert.FromBase64String(s);
                    s = Encoding.UTF8.GetString(e);
                }
                for (int i = 0; i < s.Count(); i++)
                {
                    long c = char.ConvertToUtf32(s, i);
                    if (c > 96 && c < 123)
                    {
                        c += k % 26;
                        if (c > 122)
                        {
                            c = (c - 122) + 96;
                        }
                        else if (c < 97)
                        {
                            c = (c - 97) + 123;
                        }
                    }
                    if (c > 64 && c < 91)
                    {
                        c += k % 26;
                        if (c > 90)
                        {
                            c = (c - 90) + 64;
                        }
                        else if (c < 65)
                        {
                            c = (c - 65) + 91;
                        }
                    }
                    r += Convert.ToChar(c);
                }
                s = r;
            }
            catch (Exception ex)
            {
                s = ex.Message.ToString();
            }
            return s;
        }
    }
}