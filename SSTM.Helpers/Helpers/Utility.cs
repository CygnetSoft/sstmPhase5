using System;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace SSTM.Helpers.Helpers
{
    public class Utility
    {
        #region Class Properties Declaration
        public static readonly string destinationPath = AppDomain.CurrentDomain.BaseDirectory;

        //public static readonly string dbConnectionString = @"Data Source=18.140.245.42\SQLEXPRESS,50290;Initial Catalog=SSTM; User ID=sstmAdmin; Password=$$#/\/\2o!gDb;Connection Timeout=0; Max Pool Size=1000;";
        public static readonly string dbConnectionString = ConfigurationManager.ConnectionStrings["SSTMDbContext"].ToString();
        public static readonly TimeZoneInfo sgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
        public static readonly DateTime currentDateTimeStamp = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, sgTimeZone);

        public static readonly string cryptoKey = "7x!A%D*G-KaPdSgVkYp3s6v9y/B?E(H+MbQeThWmZq4t7w!z%C&F)J@NcRfUjXn2";
        #endregion

        public static void WriteToFile(string text)
        {
            string subFolder = Path.Combine(destinationPath, "Courses Materials");

            if (!Directory.Exists(subFolder))
                Directory.CreateDirectory(subFolder);

            using (StreamWriter writer = new StreamWriter(Path.Combine(subFolder, "SSTMDownloads.text"), true))
            {
                writer.WriteLine(string.Format(text, currentDateTimeStamp.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

        public static string StaticEncrypt(string content)
        {
            try
            {
                byte[] contentBytes = Encoding.Unicode.GetBytes(content);
                using (Aes encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(cryptoKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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
            catch (Exception) { content = "In Valid"; }

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
                    var pdb = new Rfc2898DeriveBytes(cryptoKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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
            catch (Exception) { cipherText = "In Valid"; }

            return cipherText;
        }

        public static string GetMacAddress()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            return mac;
        }
    }
}