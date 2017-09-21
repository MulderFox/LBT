using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace LBT.Helpers
{
    public class InternetResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public XDocument XDocument { get; set; }

        public string ExceptionMessage { get; set; }
    }

    public enum InternetRequestType
    {
        OnlyStatusCode,
        ProcessXDocument
    }

    public static class InternetRequest
    {
        public static InternetResponse SendRequest(string url, InternetRequestType requestType)
        {
            if (String.IsNullOrEmpty(url))
                throw new Exception(String.Format("Cannot send request for empty URL."));

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = Properties.Settings.Default.RequestTimeout;
            request.UserAgent = Properties.Settings.Default.WebTitle;

            var response = new InternetResponse();
            try
            {
                using (var httpWebResponse = (HttpWebResponse)request.GetResponse())
                {
                    response.HttpStatusCode = httpWebResponse.StatusCode;

                    switch (requestType)
                    {
                        case InternetRequestType.OnlyStatusCode:
                            break;

                        case InternetRequestType.ProcessXDocument:
                            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                                break;

                            using (Stream stream = httpWebResponse.GetResponseStream())
                            {
                                if (stream == null)
                                    break;

                                response.XDocument = XDocument.Load(stream);
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("requestType");
                    }
                }
            }
            catch (Exception e)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ExceptionMessage = e.Message;
            }

            return response;
        }
    }
}