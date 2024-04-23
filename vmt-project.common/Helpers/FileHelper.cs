using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.common.Helpers
{
    public static class FileHelper
    {
        public static string GetTypeFromUrl(string url)
        {
            var splitdots = url.Split('.');
            string extension = splitdots.Last();
            string[] imageExtensions = {
                "PNG", "JPG", "JPEG", "JFIF", "PJPEG","PJP","GIF"
            };
            string[] videoExtensions = {
                "MP4", "3gpp"
            };
            string[] audioExtensions = {
                "aac", "amr", "mpeg", "ogg"
            };
            string[] docExtensions = {
                "doc", "docx", "odt", "pdf","xls","xlsx", "ods", "ppt", "pptx", "txt"
            };
            string[] stickerExtensions = {
                "webp"
            };
            if (imageExtensions.Select(m => m.ToLower()).Contains(extension.ToLower()))
            {
                return "image";
            }
            if (videoExtensions.Select(m => m.ToLower()).Contains(extension.ToLower()))
            {
                return "video";
            }
            if (audioExtensions.Select(m => m.ToLower()).Contains(extension.ToLower()))
            {
                return "audio";
            }
            if (docExtensions.Select(m => m.ToLower()).Contains(extension.ToLower()))
            {
                return "document";
            }

            return "";
        }
    }
}
