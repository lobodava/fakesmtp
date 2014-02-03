using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using FakeSmtp.Repositories;
using netDumbster.smtp;

namespace FakeSmtp.Controllers
{
	[HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			//return RedirectToAction("Settings");
			return View();
        }

		public ActionResult Headers()
	    {
			var model = MessageRepository.GetReceivedEmails();
			
			return View(model);
	    }

	    public ActionResult Messages()
	    {
		    var model = MessageRepository.GetReceivedEmails();
			
			return View(model);
	    }

	    public ActionResult Message(int id)
	    {
			var model = MessageRepository.GetEmailById(id);
			
			return View(model);
	    }

		public FileResult Download(int id, int attachmentId)
		{
			var email = MessageRepository.GetEmailById(id);
			
			var attacment = email.Attachments.First(a => a.Id == attachmentId);

			byte[] fileBytes = attacment.ContentStream.ToArray();
			string fileName = attacment.Name;

			return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
		}


	    public ActionResult Settings()
	    {
		    ViewBag.Port = MvcApplication.SmtpServer.Port;
		    ViewBag.IsSmtpServerOn = MvcApplication.IsSmtpServerOn;
		    ViewBag.EmailCount = MvcApplication.SmtpServer.ReceivedEmailCount;
		    ViewBag.ServerName = Request.ServerVariables.Get("SERVER_NAME");

			return View();
	    }

		public ActionResult Start(int port)
		{
			MessageRepository.Start(port);

			return RedirectToAction("Settings");
		}

		public ActionResult Stop()
		{
			MessageRepository.Stop();

			return RedirectToAction("Settings");
		}

		public ActionResult Clear()
		{
			MessageRepository.Clear();
			
			return RedirectToAction("Settings");
		}


		public ActionResult TestEmail()
		{
			MessageRepository.SendTestEmail();

			return RedirectToAction("Message", new {id = MvcApplication.SmtpServer.ReceivedEmailCount});
		}


		public ActionResult TestEmailPlus()
		{
			MessageRepository.SendTestEmailPlus();

			return RedirectToAction("Message", new {id = MvcApplication.SmtpServer.ReceivedEmailCount});
		}

		public ActionResult Error()
		{
			Server.ClearError();
			return View();
		}
    }
}
