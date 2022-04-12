using System;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CondoClub.Regras {
    /// <summary>
    /// Classe com funções utilitárias de apoio ao componente de regras
    /// </summary>
    public class Util {


        #region Tratamento de Erro


        public enum ImportanciaErro {
            Normal,
            Critico
        }


        public static string TratamentoErro(ImportanciaErro? importancia, string origem, Exception erro, UtilizadorAutenticado utilizador) {

            String erroMsg = erro.ToString();

            EscreveTabelaErros(Convert.ToInt16(importancia), origem, erroMsg, utilizador);

            if (importancia == ImportanciaErro.Critico) {
                string msg = String.Format("{0}  - Utilizador: {1}", erroMsg, utilizador);
                EnviaEmailErro(msg.Replace("\r\n", ""));
            }

            return erro.Message.Replace("\r\n", "");
        }


        private static void EscreveTabelaErros(short importancia, string origemErro, string descricao, UtilizadorAutenticado utilizador)
        {
            try {
                using (BD.Context ctx = new BD.Context()) {
                    ctx.LogErro.AddObject(new BD.LogErro() {
                        Importancia = importancia,
                        Origem = origemErro,
                        Descricao = descricao,
                        DataHora = DateTime.Now,
                        UtilizadorID = (utilizador != null ? utilizador.ID : (long?)null)
                    });
                    ctx.SaveChanges();
                }
            }
            catch { }
        }


        private static bool EnviaEmailErro(string msg) {
            try {
                MailMessage ErrorMail = new MailMessage(ConfigurationManager.AppSettings["AppEmail"],
                   ConfigurationManager.AppSettings["SysAdminEmail"],
                   ConfigurationManager.AppSettings["ErrorEmailSubject"],
                   msg);

                ErrorMail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                AutenticaSMTP(smtp, ConfigurationManager.AppSettings["AppEmail"]);
                smtp.Send(ErrorMail);
                return true;
            }
            catch {
                return false;
            }
        }


        #endregion


        #region Email


        internal static bool EnviaEmail(string remetente, string destinatarios, string assunto, string mensagem, bool html, bool useCondoClubTemplate, UtilizadorAutenticado utilizador) {
            try {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["MaqProd"])) {

                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], Int32.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                    AutenticaSMTP(smtp, remetente);
                    
                    string[] destinatariosArray = destinatarios.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    MailMessage mailMsg;
                    if (!html || !useCondoClubTemplate) {
                        mailMsg = new MailMessage();
                        mailMsg.From = new MailAddress(remetente);
                        mailMsg.Subject = assunto;
                        mailMsg.Body = mensagem;
                        mailMsg.IsBodyHtml = html;

                        foreach (string dest in destinatariosArray) {
                            mailMsg.To.Clear();
                            mailMsg.To.Add(dest);
                            smtp.Send(mailMsg);
                        }

                    } else {
                        mailMsg = Email.GeraHTMLEmailTemplateBase(assunto, mensagem, remetente);
                        foreach (string dest in destinatariosArray) {
                            mailMsg.To.Clear();
                            mailMsg.To.Add(dest);
                            smtp.Send(mailMsg);
                        }
                    }
                    
                }
                return true;
            } catch (Exception ex) {
                Util.TratamentoErro(ImportanciaErro.Normal, "Regras.Util.EnviaEmail", ex, utilizador);
                return false;
            }
        }


        internal static void EnviaEmailAssincrono(string remetente, string destinatarios, string assunto, string mensagem, bool html, bool useCondoClubTemplate, UtilizadorAutenticado utilizador) {
            Task.Factory.StartNew((culture) => {

                System.Threading.Thread.CurrentThread.CurrentUICulture = (System.Globalization.CultureInfo)culture;
                System.Threading.Thread.CurrentThread.CurrentCulture = (System.Globalization.CultureInfo)culture;

                Util.EnviaEmail(remetente, destinatarios, assunto, mensagem, html, useCondoClubTemplate, utilizador);
            }, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }


        private static void AutenticaSMTP(SmtpClient smtp, string remetente) {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["AutenticaSMTP"])) {
                SimpleSolutions.wcSecure.Decifra decifra = new SimpleSolutions.wcSecure.Decifra();
                string smtpUser = decifra.Decifrar(ConfigurationManager.AppSettings["SmtpUser"]);
                string smtpPswd = decifra.Decifrar(ConfigurationManager.AppSettings["SmtpPswd"]);
                smtp.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPswd);
                //apenas se for necessário https
                //smtp.EnableSsl = true;
            }
        }


        #endregion

        #region Funções Diversas


        public static void LogTarefa(string descricao) {
            try {
                using (BD.Context ctx = new BD.Context()) {
                    ctx.LogTarefa.AddObject(new BD.LogTarefa() {
                        Descricao = descricao,
                        DataHora = DateTime.Now });
                    ctx.SaveChanges();
                }
            }
            catch { }
        }


        #endregion


        #region Cifra e Decifra


        public static string Cifra(string valor) {
            return new SimpleSolutions.wcSecure.Cifra().Cifrar(valor);
        }


        public static string Decifra(string valor) {
            return new SimpleSolutions.wcSecure.Decifra().Decifrar(valor);
        }


        public static string UrlEncode(string url) {
            return url.Replace("+", "_m_").Replace("/","_b_").Replace(" ", "_");
        }


        public static string UrlDecode(string url) {
            return url.Replace("_m_", "+").Replace("_b_", "/").Replace("_", " ");
        }


        #endregion


        #region Recuperação de Password


        internal static bool EnviaRecuperacaoPassword(
            string emailEmpresa, string nomeEmpresa, 
            string destinatarioEmail, string destinatarioNome, 
            string assunto, string mensagem, 
            bool html, string origemErro, string userName) {

            try {
                MailAddress remetente;

                string remetenteEmail = "", remetenteNome = "";

                remetenteEmail = emailEmpresa;

                remetenteNome = nomeEmpresa;

                remetente = new MailAddress(remetenteEmail, remetenteNome);

                MailMessage mailmsg;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                AutenticaSMTP(smtp, remetenteEmail);

                mailmsg = new MailMessage();
                mailmsg.From = remetente;
                mailmsg.Subject = assunto;
                mailmsg.Body = mensagem;
                mailmsg.IsBodyHtml = html;
                mailmsg.To.Add(new MailAddress(destinatarioEmail, destinatarioNome));
                smtp.Send(mailmsg);
                return true;
            }
            catch (Exception ex) {
                Regras.Util.TratamentoErro(null, "Regras.Util", ex, null);
                return false;
            }
        }


        #endregion


        #region Redimensionar Imagem


        public static Stream ResizeStream(Stream stream, ImageFormat imageFormat, int maxWidth, int maxHeight) {
            using (Bitmap original = new Bitmap(stream)) {
                Size MaxSizeImage = new Size(maxWidth, maxHeight);

                if (original.Width <= MaxSizeImage.Width && original.Height <= MaxSizeImage.Height) {
                    stream.Position = 0;
                    return stream;
                }

                using (Bitmap resized = ResizeAspectRatio(original, MaxSizeImage)) {
                    MemoryStream ms = new MemoryStream();
                    resized.Save(ms, imageFormat);
                    ms.Position = 0;
                    return ms;
                }
            }
        }


        private static Bitmap ResizeAspectRatio(Bitmap original, Size max) {
            Size size = GetAspectRatioResize(new Size(original.Width, original.Height), max);
            return ResizeBitmap(original, size.Width, size.Height);
        }


        private static Size GetAspectRatioResize(Size original, Size max) {
            Size size = new Size();
            size.Width = max.Width;
            size.Height = (original.Height * size.Width) / original.Width;
            if (size.Height > max.Height) {
                size.Height = max.Height;
                size.Width = (original.Width * size.Height) / original.Height;
            }
            return size;
        }


        private static Bitmap ResizeBitmap(Bitmap original, int width, int height) {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, width, height);
            }
            return bmp;
        }


        #endregion
    }
}