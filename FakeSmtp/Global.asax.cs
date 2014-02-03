using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using netDumbster.smtp;

namespace FakeSmtp
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		public static SimpleSmtpServer SmtpServer {get;set;}
		public static bool IsSmtpServerOn {get;set;}
		
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RouteConfig.RegisterRoutes(RouteTable.Routes);

			SmtpServer = SimpleSmtpServer.Start(5000);
			IsSmtpServerOn = true;
		}

		protected void Application_End()
		{
			SmtpServer.Stop();
			IsSmtpServerOn = false;
		}
	}
}