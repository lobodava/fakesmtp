using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Web.WebPages;
using netDumbster.smtp;

namespace FakeSmtp.Models
{
	[DataContract]
	public class Email
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string From { get; set; }
		
		[DataMember]
		public string To { get; set; }
		
		[DataMember]
		public string Cc { get; set; }
		
		[DataMember]
		public string Bcc { get; set; }
		
		[DataMember]
		public DateTime SentDate { get; set; }

		[DataMember]
		public string Subject { get; set; }
		
		[DataMember]
		public string Body { get; set; }

		[DataMember]
		public Boolean IsBodyHtml { get; set; }

		[DataMember]
		public string Importance { get; set; }

		[DataMember]
		public List<Attachment> Attachments { get; set; }

		[DataMember]
		public string RawData { get; set; }
		

		public Email(SmtpMessage smtpMessage, int index, bool withoutRawData = false)
		{
			MailMessage mailMessage = Helpers.MailMessageMimeParser.ParseMessage(new StringReader(smtpMessage.Data));

			Id = index;

		    From = mailMessage.From.Address;
		    To = String.Join("; ", mailMessage.To);
            Cc  = String.Join("; ", mailMessage.CC);
            Bcc = GetBcc(smtpMessage, mailMessage);

            SentDate = mailMessage.Headers["Date"].AsDateTime();

            Subject = mailMessage.Subject;

			Body = mailMessage.Body;
			
			RawData = withoutRawData ? "" : smtpMessage.Data;

            IsBodyHtml = mailMessage.IsBodyHtml;

			switch (smtpMessage.Importance)
			{
				case "high":
					Importance = "High";
					break;
				case "low":
					Importance = "Low";
					break;
				default:
					Importance = "Normal";
					break;
			}

			Attachments = new List<Attachment>();

			for (var i = 0; i < mailMessage.Attachments.Count; i++)
			{
				var attachment  = new Attachment
				{
					Id = i + 1,
					Name = mailMessage.Attachments[i].ContentType.Name,
					ContentStream = (MemoryStream) mailMessage.Attachments[i].ContentStream
				};

				attachment.SetSize(attachment.ContentStream.Capacity);

				Attachments.Add(attachment);
			}
		}

		private string GetBcc(SmtpMessage smtpMessage, MailMessage mailMessage )
		{
			String[] toArray = mailMessage.To.Select(to => to.Address).ToArray();
			String[] ccArray = mailMessage.CC.Select(cc => cc.Address).ToArray();
			var bccList = new List<string>();

			foreach (var to in smtpMessage.ToAddresses)
			{
				if (!toArray.Contains(to.Address) && !ccArray.Contains(to.Address))
				{
					bccList.Add(to.Address);
				}
			}

			return (bccList.Count == 0) ? null : String.Join("; ", bccList.ToArray());
		}
	}

}