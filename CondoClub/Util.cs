using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CondoClub.Web 
{

    public class Util
    {
        #region Campos

        const string _regexEmail = @"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$";
        const string _regexURL = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";

        public static int PasswordMinLength = 5;
        
        private static int _ficheiroMaxSize = Convert.ToInt32(ConfigurationManager.AppSettings["FicheiroMaxSize"]);

        #endregion


        #region Propriedades

        public static int FicheiroMaxSize { get { return _ficheiroMaxSize; } }

        #endregion


        public static string ConvertUrlsToLinks(string msg) 
        {
            if (string.IsNullOrEmpty(msg)) 
                return msg;

            Regex r = new Regex(_regexURL, RegexOptions.IgnoreCase);
            return r.Replace(msg, "<a href='$1' target='blank' style='text-decoration: underline;'>$1</a>").Replace("href='www", "href='http://www");
        }


        public static string ConvertEmailToMailTo(string email)
        {
            if(!string.IsNullOrEmpty(email) && Regex.IsMatch(email, _regexEmail))
                email = String.Format("<a href='mailto:{0}' style='text-decoration: underline;'>{0}</a>", email);

            return email;
        }


        public static string ConvertNumberToTel(string number)
        {
            if (!string.IsNullOrEmpty(number))
                number = String.Format("<a href='tel:{0}'>{0}</a>", number);

            return number;
        }


        public static string GetDeviceType(string ua) {
            string ret = "";
            // Check if user agent is a smart TV - http://goo.gl/FocDk
            if (Regex.IsMatch(ua, @"GoogleTV|SmartTV|Internet.TV|NetCast|NETTV|AppleTV|boxee|Kylo|Roku|DLNADOC|CE\-HTML", RegexOptions.IgnoreCase)) {
                ret = "tv";
            }
            // Check if user agent is a TV Based Gaming Console
            else if (Regex.IsMatch(ua, "Xbox|PLAYSTATION.3|Wii", RegexOptions.IgnoreCase)) {
                ret = "tv";
            }
            // Check if user agent is a Tablet
            else if ((Regex.IsMatch(ua, "iP(a|ro)d", RegexOptions.IgnoreCase) || (Regex.IsMatch(ua, "tablet", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "RX-34", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "FOLIO", RegexOptions.IgnoreCase)))) {
                ret = "tablet";
            }
            // Check if user agent is an Android Tablet
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Android", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Fennec|mobi|HTC.Magic|HTCX06HT|Nexus.One|SC-02B|fone.945", RegexOptions.IgnoreCase))) {
                ret = "tablet";
            }
            // Check if user agent is a Kindle or Kindle Fire
            else if ((Regex.IsMatch(ua, "Kindle", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Mac.OS", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase))) {
                ret = "tablet";
            }
            // Check if user agent is a pre Android 3.0 Tablet
            else if ((Regex.IsMatch(ua, @"GT-P10|SC-01C|SHW-M180S|SGH-T849|SCH-I800|SHW-M180L|SPH-P100|SGH-I987|zt180|HTC(.Flyer|\\_Flyer)|Sprint.ATP51|ViewPad7|pandigital(sprnova|nova)|Ideos.S7|Dell.Streak.7|Advent.Vega|A101IT|A70BHT|MID7015|Next2|nook", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "MB511", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "RUTEM", RegexOptions.IgnoreCase))) {
                ret = "tablet";
            }
            return ret;
        }


        public static int ConvertToKB(int bytes)
        {
            return (int)Math.Floor(bytes / 1024d);
        }


        public static int ConvertToMB(int bytes)
        {
            return bytes / 1048576;
        }


        public static string GetCssClassFileTypeIcon(string extensao)
        {
            switch (extensao)
            {
                case "7z": return "file-icon-7z";
                case "bmp": return "file-icon-bmp";
                case "doc": return "file-icon-doc";
                case "docx": return "file-icon-docx";
                case "gif": return "file-icon-gif";
                case "jpeg": return "file-icon-jpeg";
                case "jpg": return "file-icon-jpg";
                case "pdf": return "file-icon-pdf";
                case "png": return "file-icon-png";
                case "pps": return "file-icon-pps";
                case "ppsx": return "file-icon-ppsx";
                case "ppt": return "file-icon-ppt";
                case "pptx": return "file-icon-pptx";
                case "rar": return "file-icon-rar";
                case "txt": return "file-icon-txt";
                case "xls": return "file-icon-xls";
                case "xlsx": return "file-icon-xlsx";
                case "zip": return "file-icon-zip";
                default: return "file-icon-desconhecido";
            }
        }


        public static SelectList ConstroiDropDownDadosMestres(IEnumerable<object> listaObj, Type modelType, Type bdType)
        {
            List<Models.DadosMestre._Base> modelList = new List<Models.DadosMestre._Base>();
            ConstructorInfo modelContructor = modelType.GetConstructor(new Type[] { bdType });

            foreach (object obj in listaObj)
                modelList.Add((Models.DadosMestre._Base)modelContructor.Invoke(new object[] { obj }));

            if (modelList.Count == 1)
                return new SelectList(modelList, "ID", "Designacao", modelList[0].ID);
            else
                return new SelectList(modelList, "ID", "Designacao");
        }


        public static bool ValidarDestinatariosConvites(string s, out string erro)
        {
            if (string.IsNullOrEmpty(s))
            {
                erro = Resources.Erro.SemDestinatarios;
                return false;
            }

            string[] emails = s.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            return ValidarFormatoEmailsConvites(emails, out erro) && ValidarEmailsExistentes(emails, out erro);
        }


        public static bool ValidarEmail(string email)
        {
            return Regex.IsMatch(email, _regexEmail);
        }


        public static bool ConcatenarComNomeCondominio(long UtilizadorID, Regras.Enum.Perfil PerfilUtilizador) {

            if (UtilizadorID != ControladorSite.Utilizador.ID &&
               (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub || ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa)) {
                   if (PerfilUtilizador == Regras.Enum.Perfil.Síndico ||
                    PerfilUtilizador == Regras.Enum.Perfil.Morador ||
                    PerfilUtilizador == Regras.Enum.Perfil.Portaria ||
                    PerfilUtilizador == Regras.Enum.Perfil.Consulta) {
                        return true;
                }
            }
            return false;
        }


        public static bool ConcatenarComNomeEmpresa(long UtilizadorID, Regras.Enum.Perfil PerfilUtilizador) {

            if (UtilizadorID != ControladorSite.Utilizador.ID &&
             (ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.CondoClub || ControladorSite.Utilizador.Perfil == Regras.Enum.Perfil.Empresa)) {
                 if (PerfilUtilizador == Regras.Enum.Perfil.Empresa) {
                    return true;
                }
            }

            return false;

        }


        #region Cookies
        
        public static void SetCookie(HttpResponseBase response, long utilizadorID, long condominioID)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                "cookie",
                DateTime.Now,
                DateTime.Now.AddMinutes(60),
                false,
                utilizadorID + "|" + condominioID,
                FormsAuthentication.FormsCookiePath
            );
            response.Cookies.Add(new HttpCookie("cookie", FormsAuthentication.Encrypt(ticket)));
        }


        public static void ReadCookie(HttpContext context)
        {
            HttpCookie cookie = context.Request.Cookies["cookie"];

            if (cookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(cookie.Value);

                if (!string.IsNullOrEmpty(authTicket.UserData))
                {
                    string[] data = authTicket.UserData.Split('|');
                    long condominioID;

                    if (data[0].Equals(ControladorSite.Utilizador.ID.ToString()) && 
                        long.TryParse(data[1], out condominioID))
                    {
                        ControladorSite.Utilizador.CondominioID = condominioID;
                        ControladorSite.Utilizador.EntidadeNome = new Regras.Condominio().Abrir(
                            ControladorSite.Utilizador.CondominioID.Value, ControladorSite.Utilizador).Nome;
                        ControladorSite.Utilizador.PerfilOriginal = ControladorSite.Utilizador.Perfil;
                        ControladorSite.Utilizador.Perfil = Regras.Enum.Perfil.Síndico;
                        ControladorSite.Utilizador.Impersonating = true;
                    }
                }
            }
        }


        public static void DeleteCookie(HttpContextBase context)
        {
            HttpCookie cookie = context.Request.Cookies["cookie"];
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(cookie);
        }

        #endregion


        #region Métodos auxiliares

        private static bool ValidarFormatoEmailsConvites(string[] emails, out string erro)
        {
            erro = string.Empty;

            foreach (string email in emails)
            {
                if (!Util.ValidarEmail(email.Trim()))
                    erro += "<" + email + ">, ";
            }

            if (!string.IsNullOrEmpty(erro))
            {
                erro = String.Format(Resources.Erro.ConviteEmailsInvalidos, erro.Substring(0, erro.Length - 2));
                return false;
            }

            return true;
        }


        private static bool ValidarEmailsExistentes(string[] emails, out string erro)
        {
            erro = string.Empty;

            Regras.Utilizador regras = new Regras.Utilizador();

            foreach (string email in emails)
            {
                if (regras.EmailJaExistente(email.Trim()))
                    erro += "<" + email + ">, ";
            }

            if (!string.IsNullOrEmpty(erro))
            {
                erro = String.Format(Resources.Erro.ConviteEmailsRepetidos, erro.Substring(0, erro.Length - 2));
                return false;
            }

            return true;
        }

        #endregion
    }
}