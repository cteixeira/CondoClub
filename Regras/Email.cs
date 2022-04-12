using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using System.Net.Mime;
using System.Configuration;

namespace CondoClub.Regras {

    public class Email {

        public static MailMessage GeraHTMLEmailTemplateBase(string EmailSubject, string EmailBody, string remetente) {
            List<string> destinatarios = new List<string>();
            return GeraHTMLEmailTemplateBase(EmailSubject, EmailBody, remetente, destinatarios);
        }

        public static MailMessage GeraHTMLEmailTemplateBase(string EmailSubject, string EmailBody, string remetente, string Destinatario) {
            List<string> destinatarios = new List<string>();
            destinatarios.Add(Destinatario);
            return GeraHTMLEmailTemplateBase(EmailSubject, EmailBody, remetente, destinatarios);
        }

        public static MailMessage GeraHTMLEmailTemplateBase(string EmailSubject, string EmailBody, string remetente, List<string> Destinatarios) {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(remetente);
            if (Destinatarios != null && Destinatarios.Count() > 0) {
                foreach (string email in Destinatarios.Distinct()) {
                    msg.Bcc.Add(email);
                }
            }
            msg.Subject = EmailSubject;
            string body = HTMLTemplateBase(EmailBody);

            msg.IsBodyHtml = false;
            AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            LinkedResource lr = new LinkedResource(Assembly.GetExecutingAssembly().GetManifestResourceStream("CondoClub.Regras.Images.logo_grande.png"));
            lr.ContentId = "logo";
            av.LinkedResources.Add(lr);
            msg.AlternateViews.Add(av);
            return msg;
        }

        private static string HTMLTemplateBase(string EmailBody) {
            StringBuilder sbHTML = new StringBuilder("<html><head></head><body>");
            sbHTML.Append("<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td style='text-align: center;'>");
            sbHTML.Append("<table cellspacing='0' cellpadding='0' style='font-family: Verdana, Arial; font-size: 14px;margin: 20px auto 0px auto; border: solid 1px #CCCCCC; background-color: White;color:#444444;font-weight:bold;line-height:25px; width:800px;'>");
            sbHTML.Append("<tr><td align='center' style='padding-top:5px;'><img src=\"cid:logo\"></td></tr>");
            sbHTML.Append("<tr><td align='left' style='padding: 50px 20px 20px 20px;'>");
            sbHTML.Append(EmailBody.Replace("<a ", "<a style='color:#F07D00;text-decoration:none;' "));
            sbHTML.Append("<br />");
            sbHTML.Append("</td></tr>");
            sbHTML.Append("<tr style='background-color:#E6E6E6;height:50px;'><td  style='padding-left:10px;' valign='middle'>");
            sbHTML.AppendFormat("<p style='padding:10px;'><strong>X.imob</strong>&nbsp;&copy; - Todos os direitos reservados<p>", Resources.Email.TodosDireitosReservados);
            sbHTML.Append("</td></tr>");
            sbHTML.Append("</table>");
            sbHTML.Append("</td></tr></table>");
            sbHTML.Append("</body></html>");
            return sbHTML.ToString();
        }

    }
}
