using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using FakeSmtp.Models;

namespace FakeSmtp.Repositories
{
	public class MessageRepository
	{

		public static List<Email> GetReceivedEmails()
		{
			return MvcApplication.ReceivedEmails;
		}

		public static List<Email> GetReceivedEmails(int pageSize, int pageNumber)
		{
			return MvcApplication.ReceivedEmails.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
		}

		public static Email GetEmailById(int id, bool withoutRawData = false )
		{
			var emails = MvcApplication.ReceivedEmails;
			
			var count = emails.Count();

			if (0 < count && 0 < id && id <= count)
			{
				return emails[count - id];
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

		public static string GetRawDataById(List<Email> emails, int id )
		{
			var count = emails.Count();

			if (0 < count && 0 < id && id <= count)
			{
				return emails[count - id].RawData;
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

		public static void Start(int? port, int? limit)
		{
			if (port <= 0) port = 5000;
			if (limit <= 0) limit = 1000;

			MvcApplication.StartSmtpServer(port ?? 5000, limit ?? 1000);
		}

		public static void Stop()
		{
			MvcApplication.StopSmtpServer();
		}


		public static void Clear()
		{
			MvcApplication.ReceivedEmails.Clear();
			MvcApplication.SmtpServer.ClearReceivedEmail();
		}

		public static List<PageAnchor> GetPageAnchors(int count, int pageSize, int currentPageNumber)
		{
			var pageAnchors = new List<PageAnchor>();
			
			var pageCount = (pageSize == 0) ? 1 : (int) Math.Ceiling((decimal)count / pageSize);

			
			if (pageCount == 0) 
			{
				pageAnchors.Add(new PageAnchor{ PageNumber = 1, PageLabel = "1" });
				
			}
			
			else if (pageCount < 8) 
			{
				for (var i = 1; i <= pageCount; i++) {
					pageAnchors.Add(new PageAnchor{ PageNumber = i, PageLabel = i.ToString() });
				}
			} 
			else {
				if (currentPageNumber <= 4) 
				{
					for (var i = 1; i <= 5; i++) {
						pageAnchors.Add(new PageAnchor{ PageNumber = i, PageLabel = i.ToString() });
					}
					pageAnchors.Add(new PageAnchor{ PageNumber = 6, PageLabel = "..." });
					pageAnchors.Add(new PageAnchor{ PageNumber = pageCount, PageLabel = "" + pageCount });
				} 
				else if (pageCount - currentPageNumber <= 3) 
				{
					pageAnchors.Add(new PageAnchor{ PageNumber = 1, PageLabel = "1"});
					pageAnchors.Add(new PageAnchor{ PageNumber = pageCount - 5, PageLabel = "..." });
					for (var i = pageCount - 4; i <= pageCount; i++) {
						pageAnchors.Add(new PageAnchor{ PageNumber = i, PageLabel = "" + i });
					}
				} 
				else 
				{
					pageAnchors.Add(new PageAnchor{ PageNumber = 1, PageLabel = "1"});
					
					pageAnchors.Add(new PageAnchor{ PageNumber = currentPageNumber - 2, PageLabel = "..." });
					pageAnchors.Add(new PageAnchor{ PageNumber = currentPageNumber - 1, PageLabel = "" + (currentPageNumber - 1)});
					pageAnchors.Add(new PageAnchor{ PageNumber = currentPageNumber, PageLabel = "" + currentPageNumber});
					pageAnchors.Add(new PageAnchor{ PageNumber = currentPageNumber + 1, PageLabel = "" + (currentPageNumber + 1)});
					pageAnchors.Add(new PageAnchor{ PageNumber = currentPageNumber + 2, PageLabel = "..."});
					
					pageAnchors.Add(new PageAnchor{ PageNumber = pageCount, PageLabel = "" + pageCount});
				}
			
			}
			return pageAnchors;

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

	public class PageAnchor
	{
		public int PageNumber { get; set; }
		public string PageLabel { get; set; }
	}
	
}