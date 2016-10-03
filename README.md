# Fake Smtp -  a web interface for the fake smtp server

##The problem solved

When your system has data with real email addresses and you need to test email delivery functionality... it is very dangerous to do this with real smtp server. You can substitute your real SMTP Server with Fake Smtp server and no one email message leaves your development environment.

##The project

Fake Smtp — is ASP.NET project that you can deploy to IIS or run on a local machine.

The base for this project is [netDumpster](https://github.com/cmendible/netDumbster) library and its [NuGet package](http://www.nuget.org/packages/netDumbster), as well as the class [MailMessageMimeParser](http://mimeparser.codeplex.com/SourceControl/latest#MimeParser/MailMessageMimeParser.cs) of [mimeparser.codeplex.com](http://mimeparser.codeplex.com) project.

When the web site starts, it creates a new smtp server (Fake Smtp server) with host name of the hosting web server. The new smtp server starts listening the port which can be configured on the setting page (5000 by default). 

When a email message is sent to Fake Smtp server host and port, Fake Smtp server receives the message, keeps it in memory and sends it nowhere further.

The Fake Smtp site is a web interface for Fake Smtp server. The site allows seeing:

* Headers of received email messages in a table.
* The list of email messages in a compact view, with possibility to read the email message body and download attachments.
* Every email message in full view including raw data which was received by Fake Smtp server.

The setting page allows to:

* See a name or address for a host of the Fake Smtp server
* Assign a port for the Fake Smtp server to listen and receive email messages.
* Start the Fake Smtp server with the giving port.
* Stop the Fake Smtp server.
* Clear (delete) early received email messages from memory of the Fake Smtp server.
* Send test email message to the Fake Smtp server (with or without attachment).
* The regular email message can be sent to the Fake Smtp server using, for instance, the following C# code. Host and Port values can be taken from the setting page:

```C#
var email = new System.Net.Mail.MailMessage("from@mail.com", "to1@mail.com, to2@mail.com", "Subject", "Message");
using (var smtpClient = new System.Net.Mail.SmtpClient {Host = "localhost", Port = 5000})
{
    smtpClient.Send(email);
}
```

##Web Interface screenshots

###Setting page

![Fake Smtp Setting Page](https://github.com/lobodava/fakesmtp/blob/master/ScreenShots/FakeSmtp1.png)

---

###Page with a list of expanded messages

![Fake Smtp Setting Page](https://github.com/lobodava/fakesmtp/blob/master/ScreenShots/FakeSmtp2.png)

---

###Page with a list of message headers

![Fake Smtp Setting Page](https://github.com/lobodava/fakesmtp/blob/master/ScreenShots/FakeSmtp3.png)

---

###One message page with a view of raw data

![Fake Smtp Setting Page](https://github.com/lobodava/fakesmtp/blob/master/ScreenShots/FakeSmtp4.png)

## About
This project was created as part of the **open source contribution program** in [Artezio](http://www.artezio.com/) company.
