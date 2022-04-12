using System;
using System.Collections.Generic;

namespace CondoClub.Web {

    public class ContentTypeConfig {

        private static Dictionary<String, String> ContentTypes;

        public static string GetContentType(string extensao) {
            if (ContentTypes.ContainsKey(extensao))
                return ContentTypes[extensao];
            else
                return "application/txt";
        }

        public static void RegisterContentTypes() {
            ContentTypes = new Dictionary<string, string>();
            ContentTypes.Add("gif", "image/gif");
            ContentTypes.Add("jpg", "image/jpg");
            ContentTypes.Add("jpeg", "image/jpeg");
            ContentTypes.Add("png", "image/png");
            ContentTypes.Add("pdf", "application/pdf");
            ContentTypes.Add("doc", "application/msword");
            ContentTypes.Add("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            ContentTypes.Add("xls", "application/vnd.ms-excel");
            ContentTypes.Add("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            ContentTypes.Add("pps", "application/vnd.ms-powerpoint");
            ContentTypes.Add("ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            ContentTypes.Add("ppt", "application/vnd.ms-powerpoint");
            ContentTypes.Add("pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            ContentTypes.Add("txt", "application/txt");
            ContentTypes.Add("7z", "application/x-7z-compressed");
            ContentTypes.Add("zip", "application/zip");
            ContentTypes.Add("rar", "application/rar");
            ContentTypes.Add("mp3", "audio/mpeg");
            ContentTypes.Add("odt", "application/vnd.oasis.opendocument.text");
            ContentTypes.Add("ods", "application/vnd.oasis.opendocument.spreadsheet");
        }
    }
}