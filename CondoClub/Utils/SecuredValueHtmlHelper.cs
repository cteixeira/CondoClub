using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

public static class SecuredValueHtmlHelper
{
    private const string _hdf = "<input type='hidden' name='{0}' value='{1}'/>";


    public static MvcHtmlString SecuredHiddenField(this HtmlHelper htmlHelper, string name, object value)
    {
        StringBuilder html = new StringBuilder();
        html.Append(String.Format(_hdf, name, value));
        html.Append(GetHashFieldHtml(name, GetValueAsString(value)));
        return new MvcHtmlString(html.ToString());
    }


    #region Métodos auxiliares

    private static string GetValueAsString(object value)
    {
        return Convert.ToString(value, CultureInfo.CurrentCulture);
    }


    private static string GetHashFieldHtml(string name, string value)
    {
        return String.Format(_hdf, name + "Hash", SecuredValueHashComputer.GetHash(value));
    }

    #endregion
}