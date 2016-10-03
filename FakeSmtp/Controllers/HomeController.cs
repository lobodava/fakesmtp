using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using FakeSmtp.Models;
using FakeSmtp.Repositories;


namespace FakeSmtp.Controllers
{
	[HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Headers(int? pageSize, int? pageNumber)
	    {
			var model = GetPagedEmails(pageSize, pageNumber);

			return View(model);
	    }

		public ActionResult Messages(int? pageSize, int? pageNumber)
		{
			var model = GetPagedEmails(pageSize, pageNumber);
			return View(model);
	    }

		private List<Email> GetPagedEmails(int? pageSize, int? pageNumber)
		{
			var session = System.Web.HttpContext.Current.Session;

			var sessionPageSize = (int) ( pageSize ?? System.Web.HttpContext.Current.Session["PageSize"] ?? 10 );
			var currentPageNumber =  (int) ( pageNumber ?? System.Web.HttpContext.Current.Session["PageNumber"] ?? 1 );

			if (sessionPageSize == 0 || MvcApplication.ReceivedEmails.Count() < sessionPageSize * (currentPageNumber - 1))
			{
				currentPageNumber = 1;
			}
			
			session["PageSize"] = sessionPageSize;
			session["PageNumber"] = currentPageNumber;
			
			ViewBag.PageSize = sessionPageSize;
			ViewBag.PageNumber = currentPageNumber;

			sessionPageSize = (sessionPageSize == 0) ? int.MaxValue : sessionPageSize;

			ViewBag.PageAnchors = MessageRepository.GetPageAnchors(MvcApplication.ReceivedEmails.Count(), sessionPageSize, currentPageNumber);
			
			var model = MessageRepository.GetReceivedEmails(sessionPageSize, currentPageNumber);

			ViewBag.TotalCount = MvcApplication.ReceivedEmails.Count();
			ViewBag.OnPageCount = model.Count;
			
			return model;
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
		    ViewBag.MaximumLimit = MvcApplication.MaximumLimit;
		    ViewBag.EmailCount = MvcApplication.ReceivedEmails.Count();;
		    ViewBag.ServerName = Request.ServerVariables.Get("SERVER_NAME");

			return View();
	    }

		public ActionResult Start(int? port, int? limit)
		{
			MessageRepository.Start(port, limit);

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

			var emailId = MvcApplication.ReceivedEmails.Count();

			return RedirectToAction("Message", new {id = emailId});
		}


		public ActionResult TestEmailPlus()
		{
			MessageRepository.SendTestEmailPlus();
			
			var emailId = MvcApplication.ReceivedEmails.Count(); 

			return RedirectToAction("Message", new {id = emailId});
		}

		public ActionResult TestEmail500()
		{
			for (var i = 0; i < 500; i++)
			{
				MessageRepository.SendTestEmail();
			}

			return RedirectToAction("Messages", new { pageNumber = 1 });
		}


		public ActionResult Error()
		{
			Server.ClearError();
			return View();
		}
    }
}







// ((List<Email>) System.Web.HttpContext.Current.Application["ReceivedEmails"]).Count;