using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace InventorySystemCore
{
    public class Tools
    {
        /// <summary>
        /// Creates Rfc2898DeriveBytes password hash.
        /// </summary>
        /// <param name="password">User password.</param>
        /// <returns>Hashed password.</returns>
        public string CreatePassHash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
        /// <summary>
        /// Check if password can be translated to stored password.
        /// </summary>
        /// <param name="password">User entered password.</param>
        /// <param name="passhash">Stored password hash.</param>
        public void CheckPassword(string password, string passhash)
        {
            byte[] hashBytes = Convert.FromBase64String(passhash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    throw new UnauthorizedAccessException();
        }
        private Image imageToPrint;
        private void PrintPage(object o, PrintPageEventArgs e)
        {
            System.Drawing.Point loc = new System.Drawing.Point(100, 100);
            e.Graphics.DrawImage(imageToPrint, loc);
        }
        /// <summary>
        /// Barcode generation and possible printing.
        /// </summary>
        /// <param name="barcodes">String array of barcodes to generate.</param>
        /// <param name="print">Option to print barcodes to default printer.</param>
        public void GenerateBarcodes(string[] barcodes, bool print = false)
        {
            Directory.CreateDirectory("C:/ProgramData/InventorySystem/Barcodes"); // If there is no data/configuration path.
            Directory.Delete("C:/ProgramData/InventorySystem/Barcodes", true);
            Directory.CreateDirectory("C:/ProgramData/InventorySystem/Barcodes");
            List<Image> images = new List<Image>();
            for (int i = 0; i<barcodes.Length; i++)
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                b.IncludeLabel = true;
                Font font = new Font("Arial", 16, System.Drawing.FontStyle.Bold);
                b.LabelFont = font;
                Image image = b.Encode(BarcodeLib.TYPE.CODE39, barcodes[i], Color.Black, Color.White, 290, 120);
                images.Add(image);
                image.Save("C:/ProgramData/InventorySystem/Barcodes/" + barcodes[i] + ".png");
            }
            if (print)
            {
                PrintDocument pd = new PrintDocument();
                foreach (Image image in images)
                {
                    imageToPrint = image;
                    pd.PrintPage += PrintPage;
                    pd.Print();
                }
            }
        }
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();
                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
    public class Emailing
    {
        private InfoStoring CredInfo;
        private string Activation;
        public Emailing()
        {
            CredInfo = new InfoStoring();
        }
        public Emailing(string password, string senderEmail)
        {
            CredInfo = new InfoStoring(password, senderEmail);
        }
        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/en-US/f75a6ebb-32cf-4ed5-ab03-4a3f07bc214e/how-to-generate-a-code-for-the-email-confirmation-when-user-register-into-my-site?forum=aspmvc
        /// </summary>
        /// <param name="email">Correct email where activation code will be sent to.</param>
        public void Register(string email)
        {
            Random rand = new Random();
            // Characters we will use to generate this random string.
            char[] allowableChars = "ABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray();
            // Start generating the random string.
            string activationCode = "";
            for (int i = 0; i <= 6 - 1; i++)
            {
                activationCode += allowableChars[rand.Next(allowableChars.Length - 1)];
            }
            // Return the random string in upper case.
            Activation = activationCode.ToUpper();
            SendEmail("Epasta Pievienošana", $"Jūsu Aktivizācijas kods: {Activation}", email);
        }
        /// <summary>
        /// Checks activation code that was sent to the email.
        /// </summary>
        /// <param name="Received">User received activation code.</param>
        /// <returns>Whatever code is correct or no.</returns>
        public bool IsRegistrated(string Received)
        {
            return Activation == Received;
        }
        /// <summary>
        /// Simple email sending with subject and message.
        /// </summary>
        /// <param name="subject">Subject of the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="toEmail">Correct emeil where to send email.</param>
        public void SendEmail(string subject, string message, string toEmail)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(CredInfo.GetSenderEmail(), CredInfo.GetPassword()),
                EnableSsl = true,
            };
            smtpClient.Send(CredInfo.GetSenderEmail(), toEmail, subject, message);
        }
        /// <summary>
        /// Resave sender credentials to configuration file.
        /// </summary>
        public void Resave()
        {
            CredInfo.StoreInfo();
        }
    }
    /// <summary>
    /// Settings file creator class.
    /// </summary>
    public class InfoStoring
    {
        class cred
        {
            public string Password { get; set; }
            public string SenderEmail { get; set; }
        }
        cred Cred;
        static string filePath = "C:/ProgramData/InventorySystem/";
        static string fileName = "Settings.json";
        Exception EmailSettingsExc = new Exception("Sūtīšanas epasta configurācijas kļūda!");
        /// <summary>
        /// Default constructor tries to read configuration from file.
        /// </summary>
        public InfoStoring()
        {
            if(File.Exists(filePath + fileName))
            {
                string Reading = File.ReadAllText(filePath + fileName);
                Cred = JsonSerializer.Deserialize<cred>(Reading);
            }
            else
            {
                throw EmailSettingsExc;
            }
            if(Cred.Password == null || Cred.SenderEmail == null)
            {
                throw EmailSettingsExc;
            }
        }
        /// <summary>
        /// When there is no information stored new credentials are saved.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <param name="senderEmail">Correct sender email.</param>
        public InfoStoring(string password, string senderEmail)
        {
            Cred = new cred
            {
                Password = password,
                SenderEmail = senderEmail
            };
        }
        /// <summary>
        /// Save information in settings file.
        /// </summary>
        public void StoreInfo()
        {
            Directory.CreateDirectory(filePath); // If there is no data/configuration path.
            File.Delete(filePath + fileName); // If old configuration file exists.
            string Writing = JsonSerializer.Serialize(Cred);
            File.WriteAllText(filePath + fileName, Writing);
        }
        /// <summary>
        /// Get sender email from settings.
        /// </summary>
        /// <returns>Sender email.</returns>
        public string GetSenderEmail()
        {
            return Cred.SenderEmail;
        }
        /// <summary>
        /// Get sender email password from settings.
        /// </summary>
        /// <returns>Sender email password.</returns>
        public string GetPassword()
        {
            return Cred.Password;
        }
    }
    public class Notifications
    {
        public void I1()
        {
            MessageBox.Show("Būvdarbu objekts ir reģistrēts");
        }
        public void I2()
        {
            MessageBox.Show("Informācija par būvdarbu objektu ir nomainīta");
        }
        public void I3()
        {
            MessageBox.Show("Būvdarbu objekts ir izdzēsts");
        }
        public void I4()
        {
            MessageBox.Show("Parole iestatīta uz noklusējumu 0000");
        }
        public void I5()
        {
            MessageBox.Show("Darbinieks ir reģistrēts");
        }
        public void I6()
        {
            MessageBox.Show("Informācija par darbinieku ir nomainīta");
        }
        public void I7()
        {
            MessageBox.Show("Darbinieks ir izdzēsts");
        }
        public void I8()
        {
            MessageBox.Show("Inventārs ir reģistrēts");
        }
        public void I9()
        {
            MessageBox.Show("Informācija par inventāru ir nomainīta");
        }
        public void I10()
        {
            MessageBox.Show("Inventārs ir norakstīts");
        }
        public void I11()
        {
            MessageBox.Show("Atlasītā inventāra svītrkodi tiek ģenerēti/drukāti");
        }
        public void I12()
        {
            MessageBox.Show("Administrators ir reģistrēts");
        }
        public void I13()
        {
            MessageBox.Show("Ziņu e-pasts ir reģistrēts");
        }
        public void I14()
        {
            MessageBox.Show("Parole ir nomainīta");
        }
        public void I15()
        {
            MessageBox.Show("Paņemts");
        }
        public void I16()
        {
            MessageBox.Show("Nodots");
        }
        public void I17()
        {
            MessageBox.Show("Bojājuma ziņojums pieņemts");
        }

        public MessageBoxResult A1()
        {
            return MessageBox.Show("Vai vēlaties dzēst būvdarbu objektu?", "Izdzēst?", MessageBoxButton.YesNo);
        }
        public MessageBoxResult A2()
        {
            return MessageBox.Show("Šajā būvdarbu objektā vēl ir inventārs.\nVai atgriezt visu šo inventāru un dzēst darbinieku?", "Atgriezt?", MessageBoxButton.YesNo);
        }
        public MessageBoxResult A3()
        {
            return MessageBox.Show("Vai vēlaties dzēst darbinieku?", "Izdzēst?", MessageBoxButton.YesNo);
        }
        public MessageBoxResult A4()
        {
            return MessageBox.Show("Pie šī darbinieka vēl ir inventārs.\nVai atgriezt visu šo inventāru un dzēst darbinieku?", "Atgriezt?", MessageBoxButton.YesNo);
        }
        public MessageBoxResult A5()
        {
            return MessageBox.Show("Vai vēlaties norakstīt inventāru?", "Izdzēst?", MessageBoxButton.YesNo);
        }
        public MessageBoxResult A6()
        {
            return MessageBox.Show("Šo inventāru bija paņēmis cits darbinieks. Vai atgriezt?", "Atgriezt?", MessageBoxButton.YesNo);
        }

        public void E1()
        {
            MessageBox.Show("Nav norādīts darba objekta nosaukums");
        }
        public void E2()
        {
            MessageBox.Show("Darba objekta nosaukums nedrīkst būt garāks par 35 simboliem");
        }
        public void E3()
        {
            MessageBox.Show("Nav norādīts vārds");
        }
        public void E4()
        {
            MessageBox.Show("Vārds nedrīkst būt garāks par 20 simboliem");
        }
        public void E5()
        {
            MessageBox.Show("Nav norādīts uzvārds");
        }
        public void E6()
        {
            MessageBox.Show("Uzvārds nedrīkst būt garāks par 20 simboliem");
        }
        public void E7()
        {
            MessageBox.Show("Parole nav garumā no 4-10 simboliem");
        }
        public void E8()
        {
            MessageBox.Show("Nav norādīts inventāra nosaukums");
        }
        public void E9()
        {
            MessageBox.Show("Inventāra nosaukums nedrīkst būt garāks par 35 simboliem");
        }
        public void E10()
        {
            MessageBox.Show("Nav norādīts inventāra modelis");
        }
        public void E11()
        {
            MessageBox.Show("Inventāra modelis nedrīkst būt garāks par 35 simboliem");
        }
        public void E12()
        {
            MessageBox.Show("Nav norādīts inventāra ražotājs");
        }
        public void E13()
        {
            MessageBox.Show("Inventāra ražotājs nedrīkst būt garāks par 35 simboliem");
        }
        public void E14()
        {
            MessageBox.Show("Nepareiza administratora pieeja");
        }
        public void E15()
        {
            MessageBox.Show("Nav ievadīts e-pasts");
        }
        public void E16()
        {
            MessageBox.Show("E-pasts neatbilst standartam");
        }
        public void E17()
        {
            MessageBox.Show("Ievadītais kods nav pareizs");
        }
        public void E18()
        {
            MessageBox.Show("Nepareiza darbinieka pieeja");
        }
        public void E19()
        {
            MessageBox.Show("Paroles nav vienādas");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Entered ID</param>
        /// <returns></returns>
        public void E20(int id)
        {
            MessageBox.Show($"Inventārs ar {id} $is neeksistē");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Entered ID</param>
        /// <returns></returns>
        public void E21(int id)
        {
            MessageBox.Show($"Inventārs ar ID {id} jau ir paņemts");
        }
    }
}
