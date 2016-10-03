using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using FakeSmtp.Models;
using netDumbster.smtp;

namespace FakeSmtp
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static SimpleSmtpServer SmtpServer {get;set;}
		public static bool IsSmtpServerOn {get;set;}
		public static int MaximumLimit {get;set;}
		public static List<Email> ReceivedEmails {get;set;}


		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RouteConfig.RegisterRoutes(RouteTable.Routes);


			ReceivedEmails = new List<Email>();

			StartSmtpServer(5000, 1000);
		}

		public static void StartSmtpServer(int port, int limit)
		{
			if (ReceivedEmails.Count > limit)
			{
				ReceivedEmails.RemoveRange(limit - 1, ReceivedEmails.Count - limit);
			}
			
			SmtpServer = SimpleSmtpServer.Start(port);
			IsSmtpServerOn = true;
			MaximumLimit = limit;

			SmtpServer.MessageReceived += SmtpServer_MessageReceived;
		}

		public static void StopSmtpServer()
		{
			SmtpServer.MessageReceived -= SmtpServer_MessageReceived;
			SmtpServer.ClearReceivedEmail();
			SmtpServer.Stop();
			IsSmtpServerOn = false;
		}
		
		private static void SmtpServer_MessageReceived(object sender, MessageReceivedArgs e)
		{
			if (ReceivedEmails.Count == MaximumLimit)
			{
				ReceivedEmails.RemoveAt(ReceivedEmails.Count - 1);
			}

			var newEmailId = (ReceivedEmails.Count == 0) ? 1 : ReceivedEmails[0].Id + 1;
			
			ReceivedEmails.Insert(0, new Email(e.Message, newEmailId));

			SmtpServer.ClearReceivedEmail();
		}
		
		protected void Application_End()
		{
			SmtpServer.Stop();
			IsSmtpServerOn = false;
		}
	}
}