using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FakeSmtp.Models;
using FakeSmtp.Repositories;

namespace FakeSmtp
{
	public class FakeSmtpService : IFakeSmtpService
	{
		public List<Email> GetReceivedEmails()
		{
			return MessageRepository.GetReceivedEmails(true);
		}

		public Email GetEmailById(int emailId)
		{
			return MessageRepository.GetEmailById(emailId, true);
		}

		public string GetRawDataById(int emailId)
		{
			return MessageRepository.GetRawDataById(emailId);
		}

		public byte[] GetAttachmentBytesById(int emailId, int attachmentId)
		{
			return MessageRepository.GetAttachmentBytesById(emailId, attachmentId);
		}
		
		public void Start(int port)
		{
			MessageRepository.Start(port);
		}

		public void Stop()
		{
			MessageRepository.Stop();
		}

		public void Clear()
		{
			MessageRepository.Clear();
		}

		public void SendTestEmail()
		{
			MessageRepository.SendTestEmail();
		}

		public void SendTestEmailPlus()
		{
			MessageRepository.SendTestEmailPlus();
		}
	}
}
