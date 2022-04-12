using System;
using System.Security.Cryptography;
using System.Text;

public static class SecuredValueHashComputer
{
    private const string _secret = "ZaC273ysbEXiM8715484z480O";
    

    public static SHA1 sha1 { get; set; }



    static SecuredValueHashComputer()
    {
        sha1 = new SHA1CryptoServiceProvider();
    }



    public static string GetHash(string value)
    {
        return Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(value + _secret)));
    }


    public static bool ValidateValue(string value, string hash)
    {
        return GetHash(value).Equals(hash);
    }
}