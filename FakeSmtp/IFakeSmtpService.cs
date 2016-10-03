using System.Collections.Generic;
using System.ServiceModel;
using FakeSmtp.Models;

namespace FakeSmtp
{
	[ServiceContract]
	public interface IFakeSmtpService
	{
		[OperationContract]
		List<Email> GetReceivedEmails();

		[OperationContract]
		Email GetEmailById(int emailId);
		
		[OperationContract]
		string GetRawDataById(int emailId);
		
		[OperationContract]
		byte[] GetAttachmentBytesById(int emailId, int attachmentId);

		[OperationContract]
		void Start(int? port, int? limit);

		[OperationContract]
		void Stop();

		[OperationContract]
		void Clear();

		[OperationContract]
		void SendTestEmail();

		[OperationContract]
		void SendTestEmailPlus();
	}
}