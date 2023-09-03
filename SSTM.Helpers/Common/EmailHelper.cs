using SSTM.Models.EmailModel;
using System;
using System.Net;
using System.Net.Mail;

namespace SSTM.Helpers.Common
{
    public class EmailHelper
    {
        public static string SendMail(EmailModel model)
        {
            try
            {
                var from = UtilityHelper.Decrypt(model.From);

                if (model.To == null && model.Cc == null && model.Bcc == null)
                    return "Please enter mail recipients.";
                else
                {
                    var message = new MailMessage();
                    message.From = new MailAddress(from, from);

                    if (model.To != null)
                    {
                        string[] toEmails = model.To.Split(';');
                        foreach (string toEmail in toEmails)
                        {
                            if (toEmail.Trim() != "")
                                message.To.Add(toEmail.Trim());
                        }
                    }

                    if (model.Cc != null)
                    {
                        string[] ccEmails = model.Cc.Split(';');
                        foreach (string ccEmail in ccEmails)
                        {
                            if (ccEmail.Trim() != "")
                                message.CC.Add(ccEmail.Trim());
                        }
                    }

                    if (model.Bcc != null)
                    {
                        string[] bccEmails = model.Bcc.Split(';');
                        foreach (string bccEmail in bccEmails)
                        {
                            if (bccEmail.Trim() != "")
                                message.Bcc.Add(bccEmail.Trim());
                        }
                    }

                    message.ReplyTo = new MailAddress(from);
                    message.Subject = model.Subject;
                    message.Body = model.Message;
                    message.IsBodyHtml = true;

                    if (model.Attachments != null && model.Attachments != "")
                    {
                        string[] attachments = model.Attachments.Split(';');
                        foreach (string attachment in attachments)
                        {
                            if (attachment.Trim() != null)
                                message.Attachments.Add(new Attachment(attachment.Trim()));
                        }
                    }

                    NetworkCredential networkCredential = new NetworkCredential(UtilityHelper.Decrypt(model.SMTPEmail), UtilityHelper.Decrypt(model.SMTPPassword));

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = UtilityHelper.Decrypt(model.SMTPHost);
                    smtpClient.Port = Convert.ToInt32(UtilityHelper.Decrypt(model.SMTPPort));
                    smtpClient.EnableSsl = Convert.ToBoolean(UtilityHelper.Decrypt(model.EnableSsl));
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = networkCredential;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    smtpClient.Send(message);

                    return "sent";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}