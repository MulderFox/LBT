using System;
using System.IO;

namespace LBT.Services
{
    public static class HttpResponseBaseService
    {
        public enum ContentType
        {
            Txt,
            Pdf,
            Mp4,
            Xlsx,
            Docx,
            Pptx,
            Ogg
        }

        public static string GetContentType(string filePath, out ContentType contentType)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                contentType = ContentType.Txt;
                return "text/plain";
            }

            string extension = Path.GetExtension(filePath);
            extension = String.IsNullOrEmpty(extension) ? String.Empty : extension.ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    contentType = ContentType.Pdf;
                    return "application/pdf";

                case ".mp4":
                    contentType = ContentType.Mp4;
                    return "video/mp4";

                case ".xlsx":
                    contentType  = ContentType.Xlsx;
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case ".docx":
                    contentType = ContentType.Docx;
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case ".pptx":
                    contentType = ContentType.Pptx;
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";

                case ".ogg":
                    contentType = ContentType.Ogg;
                    return "video/ogg";

                default:
                    throw new ArgumentOutOfRangeException(extension);
            }
        }

        public static string GetContentType(string filePath = null)
        {
            ContentType contentType;
            string strContentType = GetContentType(filePath, out contentType);
            return strContentType;
        }
    }
}