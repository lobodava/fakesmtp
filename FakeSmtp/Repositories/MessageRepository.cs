using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using FakeSmtp.Models;
using netDumbster.smtp;

namespace FakeSmtp.Repositories
{
	public class MessageRepository
	{

		public static List<Email> GetReceivedEmails(bool withoutRawData = true)
		{
			var emails = new List<Email>();

			var count = MvcApplication.SmtpServer.ReceivedEmail.Count();

			for (var index = 0; index < count  ; index++)
			{
				emails.Add(new Email(MvcApplication.SmtpServer.ReceivedEmail[index], count - index, withoutRawData));
			}

			return emails;
		}

		public static Email GetEmailById(int id, bool withoutRawData = false )
		{
			var count = MvcApplication.SmtpServer.ReceivedEmail.Count();

			if (0 < count && 0 < id && id <= count)
			{
				var smtpMessage = MvcApplication.SmtpServer.ReceivedEmail[count - id];

				return new Email(smtpMessage, id, withoutRawData);
			}

			return null;
		}

		public static string GetRawDataById(int id)
		{
			var count = MvcApplication.SmtpServer.ReceivedEmail.Count();

			if (0 < count && 0 < id && id <= count)
			{
				return MvcApplication.SmtpServer.ReceivedEmail[count - id].Data;
			}

			return null;
		}

		public static byte[] GetAttachmentBytesById(int emailId, int attachmentId)
		{
			var email = GetEmailById(emailId, true);
			
			if (email != null) {
				var attacment = email.Attachments.FirstOrDefault(a => a.Id == attachmentId);

				if (attacment != null)
				{
					return attacment.ContentStream.ToArray();
				}
			}

			return null;
		}

		public static void Start(int port)
		{
			MvcApplication.SmtpServer = SimpleSmtpServer.Start(port);
			MvcApplication.IsSmtpServerOn = true;
		}

		public static void Stop()
		{
			MvcApplication.SmtpServer.Stop();
			MvcApplication.IsSmtpServerOn = false;
		}


		public static void Clear()
		{
			MvcApplication.SmtpServer.ClearReceivedEmail();
		}
		
		#region [ Test Messages ]

		public static void SendTestEmail()
		{
			var email = CreateTestEmail();

			using (var sc = new SmtpClient {Host = "localhost", Port = MvcApplication.SmtpServer.Port})
			{
				sc.Send(email);
			}
		}
		
		public static void SendTestEmailPlus()
		{
			string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\","");
			
			var email = CreateTestEmail();

			email.Attachments.Add(new System.Net.Mail.Attachment(asmPath + @"\artezio.png"));

			using (var sc = new SmtpClient {Host = "localhost", Port = MvcApplication.SmtpServer.Port})
			{
				sc.Send(email);
			}
		}

		private static MailMessage CreateTestEmail()
		{
			var email = new MailMessage();

			email.BodyEncoding = Encoding.UTF8;

			email.From = new MailAddress("from@mail.com", "Notification from mail.com");

			email.To.Add("to1@mail.com");
			email.To.Add("to2@mail.com");

			email.CC.Add("cc1@mail.com");
			email.CC.Add("cc2@mail.com");

			email.Bcc.Add("bcc1@mail.com");
			email.Bcc.Add("bcc2@mail.com");

            email.Subject = "This is \"Lorem ipsum\" test email";
            email.Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce ut pretium velit, at hendrerit mauris. Nam sit amet purus ac diam luctus facilisis.\nNullam sagittis vestibulum orci, pulvinar placerat turpis mollis ut. Phasellus fermentum tempor magna, venenatis congue sem interdum eget.\nUt commodo pellentesque fermentum. Praesent suscipit et augue sit amet pulvinar.\nEtiam augue mi, feugiat nec lobortis sit amet, condimentum a nibh.\nInteger blandit lacus sed lectus venenatis, sit amet consequat felis rutrum.";
           
			email.IsBodyHtml = false;
            email.Priority = MailPriority.Normal;

			return email;
		}

		#endregion
	}
}