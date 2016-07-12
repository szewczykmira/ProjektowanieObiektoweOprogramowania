using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_5
{
    public class SmtpFacade
    {
        private NetworkCredential _credential;
        private SmtpClient _client;

        public SmtpFacade(string server, int port)
        {
            this._client = new SmtpClient(server, port);
        }

        public SmtpFacade(string server, int port, string account, string password)
            : this(server, port)
        {
            this._credential = new NetworkCredential(account, password);
            this._client.Credentials = this._credential;
        }

        public void Send(string From, string To,
                        string Subject, string Body,
                        Stream AttachmentContent = null, string AttachmentMimeType = null)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(To);
            msg.From = new MailAddress(From);
            msg.Subject = Subject;
            msg.Body = Body;
            Attachment att;
            if (AttachmentContent != null)
            {
                if (AttachmentMimeType == null)
                    att = new Attachment(AttachmentContent, "");
                else
                    att = new Attachment(AttachmentContent, new ContentType(AttachmentMimeType));
                msg.Attachments.Add(att);
            }
            this._client.Send(msg);
        }
    }

}