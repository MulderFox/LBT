using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LBT.Services
{
    /// <summary>
    /// Yet another developer blog: Range Requests in ASP.NET MVC – RangeFileResult
    /// </summary>
    /// <see cref="http://tpeczek.blogspot.cz/2011/10/range-requests-in-aspnet-mvc.html"/>
    public abstract class RangeFileResult : ActionResult
    {
        public string ContentType { get; private set; }

        public string FileName { get; private set; }

        public DateTime FileModificationDate { get; private set; }

        private DateTime HttpModificationDate { get; set; }

        public long FileLength { get; private set; }

        private string EntityTag { get; set; }

        private long[] RangesStartIndexes { get; set; }

        private long[] RangesEndIndexes { get; set; }

        private bool RangeRequest { get; set; }

        private bool MultipartRequest { get; set; }

        private static readonly char[] CommaSplitArray = new[] { ',' };
        private static readonly char[] DashSplitArray = new[] { '-' };
        private static readonly string[] HttpDateFormats = new[] { "r", "dddd, dd-MMM-yy HH':'mm':'ss 'GMT'", "ddd MMM d HH':'mm':'ss yyyy" };

        protected RangeFileResult(string contentType, string fileName, DateTime modificationDate, long fileLength)
        {
            if (String.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("contentType");

            ContentType = contentType;
            FileName = fileName;
            FileLength = fileLength;
            FileModificationDate = modificationDate;
            //Modification date for header values comparisons purposes
            HttpModificationDate = modificationDate.ToUniversalTime();
            HttpModificationDate = new DateTime(HttpModificationDate.Year, HttpModificationDate.Month, HttpModificationDate.Day, HttpModificationDate.Hour, HttpModificationDate.Minute, HttpModificationDate.Second, DateTimeKind.Utc);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //Generate entity tag
            EntityTag = GenerateEntityTag(context);
            //Get ranges from request
            GetRanges(context.HttpContext.Request);

            //If all validations are successful
            if (!ValidateRanges(context.HttpContext.Response) ||
                !ValidateModificationDate(context.HttpContext.Request, context.HttpContext.Response) ||
                !ValidateEntityTag(context.HttpContext.Request, context.HttpContext.Response))
                return;

            //Set common headers
            context.HttpContext.Response.AddHeader("Last-Modified", FileModificationDate.ToString("r"));
            context.HttpContext.Response.AddHeader("ETag", String.Format("\"{0}\"", EntityTag));
            context.HttpContext.Response.AddHeader("Accept-Ranges", "bytes");

            //If this is not range request
            if (!RangeRequest)
            {
                //Set standard headers
                context.HttpContext.Response.AddHeader("Content-Length", FileLength.ToString(CultureInfo.InvariantCulture));
                context.HttpContext.Response.ContentType = ContentType;
                //Set status code
                context.HttpContext.Response.StatusCode = 200;

                //If this is not HEAD request
                if (!context.HttpContext.Request.HttpMethod.Equals("HEAD"))
                {
                    //Write entire file to response
                    WriteEntireEntity(context.HttpContext.Response);
                }
            }
            //If this is range request
            else
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

                //Compute and set content length
                context.HttpContext.Response.AddHeader("Content-Length", GetContentLength(boundary).ToString(CultureInfo.InvariantCulture));

                //If this is not multipart request
                if (!MultipartRequest)
                {
                    //Set content range and type
                    context.HttpContext.Response.AddHeader("Content-Range", String.Format("bytes {0}-{1}/{2}", RangesStartIndexes[0], RangesEndIndexes[0], FileLength));
                    context.HttpContext.Response.ContentType = ContentType;
                }
                //Otherwise
                else
                {
                    //Set proper content type
                    context.HttpContext.Response.ContentType = String.Format("multipart/byteranges; boundary={0}", boundary);
                }

                //Set status code
                context.HttpContext.Response.StatusCode = 206;

                //If this not HEAD request
                if (context.HttpContext.Request.HttpMethod.Equals("HEAD"))
                    return;

                //For each requested range
                for (int i = 0; i < RangesStartIndexes.Length; i++)
                {
                    //If this is multipart request
                    if (MultipartRequest)
                    {
                        //Write additional multipart info
                        context.HttpContext.Response.Write(String.Format("--{0}\r\n", boundary));
                        context.HttpContext.Response.Write(String.Format("Content-Type: {0}\r\n", ContentType));
                        context.HttpContext.Response.Write(String.Format("Content-Range: bytes {0}-{1}/{2}\r\n\r\n", RangesStartIndexes[i], RangesEndIndexes[i], FileLength));
                    }

                    //Write range (with multipart separator if required)
                    if (!context.HttpContext.Response.IsClientConnected)
                        return;

                    if (!MultipartRequest)
                    {
                        WriteEntityRange(context.HttpContext.Response, RangesStartIndexes[i], RangesEndIndexes[i]);
                    }
                    else
                    {
                        WriteMultipartEntityRange(context.HttpContext.Response, RangesStartIndexes[i], RangesEndIndexes[i]);
                        context.HttpContext.Response.Write("\r\n");

                        try
                        {
                            context.HttpContext.Response.Flush();
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        // ReSharper restore EmptyGeneralCatchClause
                        {
                        }
                    }
                }

                //If this is multipart request
                if (MultipartRequest)
                {
                    context.HttpContext.Response.Write(String.Format("--{0}--", boundary));                    
                }
            }
        }

        protected abstract void WriteEntireEntity(HttpResponseBase response);

        protected abstract void WriteEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex);

        protected abstract void WriteMultipartEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex);

        protected virtual string GenerateEntityTag(ControllerContext context)
        {
            //Generate entity tag based on file name and modification date
            byte[] entityTagBytes = Encoding.ASCII.GetBytes(String.Format("{0}|{1}", FileName, FileModificationDate));
            string entityTag = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(entityTagBytes));
            return entityTag;
        }

        //Helper method for getting HTTP headers values
        private string GetHeader(HttpRequestBase request, string header, string defaultValue = "")
        {
            string requestHeader = String.IsNullOrEmpty(request.Headers[header]) ? defaultValue : request.Headers[header].Replace("\"", String.Empty);
            return requestHeader;
        }

        private void GetRanges(HttpRequestBase request)
        {
            //Get "Range" header from request
            string rangesHeader = GetHeader(request, "Range");
            //Get "If-Range" header from request
            string ifRangeHeader = GetHeader(request, "If-Range", EntityTag);
            DateTime ifRangeHeaderDate;
            bool isIfRangeHeaderDate = DateTime.TryParseExact(ifRangeHeader, HttpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out ifRangeHeaderDate);

            //If there is no "Range" header,
            //or the entity tag from "If-Range" header does not match this entity tag,
            //or the modification date is greater than modification date from "If-Range" header
            if (String.IsNullOrEmpty(rangesHeader) || (!isIfRangeHeaderDate && ifRangeHeader != EntityTag) || (isIfRangeHeaderDate && HttpModificationDate > ifRangeHeaderDate))
            {
                //Return entire file
                RangesStartIndexes = new long[] { 0 };
                RangesEndIndexes = new[] { FileLength - 1 };
                RangeRequest = false;
                MultipartRequest = false;
            }
            //Otherwise
            else
            {
                //Split "Range" header value into ranges
                string[] ranges = rangesHeader.Replace("bytes=", String.Empty).Split(CommaSplitArray);

                RangesStartIndexes = new long[ranges.Length];
                RangesEndIndexes = new long[ranges.Length];
                RangeRequest = true;
                MultipartRequest = (ranges.Length > 1);

                //Get the star and end index for the range 
                for (int i = 0; i < ranges.Length; i++)
                {
                    string[] currentRange = ranges[i].Split(DashSplitArray);

                    if (String.IsNullOrEmpty(currentRange[1]))
                    {
                        RangesEndIndexes[i] = FileLength - 1;                        
                    }
                    else
                    {
                        RangesEndIndexes[i] = Int64.Parse(currentRange[1]);                        
                    }

                    if (String.IsNullOrEmpty(currentRange[0]))
                    {
                        RangesStartIndexes[i] = FileLength - 1 - RangesEndIndexes[i];
                        RangesEndIndexes[i] = FileLength - 1;
                    }
                    else
                    {
                        RangesStartIndexes[i] = Int64.Parse(currentRange[0]);                        
                    }
                }
            }
        }

        private bool ValidateRanges(HttpResponseBase response)
        {
            if (FileLength > Int32.MaxValue)
            {
                response.StatusCode = 413;
                return false;
            }

            if (RangesStartIndexes.Where((t, i) => t > FileLength - 1 || RangesEndIndexes[i] > FileLength - 1 || t < 0 || RangesEndIndexes[i] < 0 || RangesEndIndexes[i] < t).Any())
            {
                response.StatusCode = 400;
                return false;
            }

            return true;
        }

        private bool ValidateModificationDate(HttpRequestBase request, HttpResponseBase response)
        {
            //First validate "If-Modified-Since" header
            string modifiedSinceHeader = GetHeader(request, "If-Modified-Since");
            if (!String.IsNullOrEmpty(modifiedSinceHeader))
            {
                DateTime modifiedSinceDate;
                DateTime.TryParseExact(modifiedSinceHeader, HttpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out modifiedSinceDate);

                if (HttpModificationDate <= modifiedSinceDate)
                {
                    response.StatusCode = 304;
                    return false;
                }
            }

            //Then validate "If-Unmodified-Since" or "Unless-Modified-Since"
            string unmodifiedSinceHeader = GetHeader(request, "If-Unmodified-Since", GetHeader(request, "Unless-Modified-Since"));
            if (!String.IsNullOrEmpty(unmodifiedSinceHeader))
            {
                DateTime unmodifiedSinceDate;
                DateTime.TryParseExact(unmodifiedSinceHeader, HttpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out unmodifiedSinceDate);

                if (HttpModificationDate > unmodifiedSinceDate)
                {
                    response.StatusCode = 412;
                    return false;
                }
            }

            return true;
        }

        private bool ValidateEntityTag(HttpRequestBase request, HttpResponseBase response)
        {
            //Get "If-Match" header from request
            string matchHeader = GetHeader(request, "If-Match");

            //If header exists and it's value is different from "*"
            if (!String.IsNullOrEmpty(matchHeader) && matchHeader != "*")
            {
                //Split header value into list of etity tags
                string[] entitiesTags = matchHeader.Split(CommaSplitArray);
                int entitieTagIndex;
                for (entitieTagIndex = 0; entitieTagIndex < entitiesTags.Length; entitieTagIndex++)
                {
                    if (EntityTag == entitiesTags[entitieTagIndex])
                        break;
                }

                //If our entity tag wasn't found
                if (entitieTagIndex >= entitiesTags.Length)
                {
                    //Set proper response status code
                    response.StatusCode = 412;
                    return false;
                }
            }

            //Get "If-None-Match" header from request
            string noneMatchHeader = GetHeader(request, "If-None-Match");

            //If header exists
            if (!String.IsNullOrEmpty(noneMatchHeader))
            {
                //If header value equals "*"
                if (noneMatchHeader == "*")
                {
                    //Set proper response status code
                    response.StatusCode = 412;
                    return false;
                }

                //Split header value into list of etity tags
                string[] entitiesTags = noneMatchHeader.Split(CommaSplitArray);
                foreach (string entityTag in entitiesTags.Where(et => EntityTag == et))
                {
                    //Set proper response status code
                    response.AddHeader("ETag", String.Format("\"{0}\"", entityTag));
                    response.StatusCode = 304;
                    return false;
                }
            }

            return true;
        }

        //Helper method for computing content length
        private int GetContentLength(string boundary)
        {
            int contentLength = 0;
            for (int i = 0; i < RangesStartIndexes.Length; i++)
            {
                contentLength += Convert.ToInt32(RangesEndIndexes[i] - RangesStartIndexes[i]) + 1;

                if (MultipartRequest)
                {
                    contentLength += boundary.Length +
                                     ContentType.Length +
                                     RangesStartIndexes[i].ToString(CultureInfo.InvariantCulture).Length +
                                     RangesEndIndexes[i].ToString(CultureInfo.InvariantCulture).Length +
                                     FileLength.ToString(CultureInfo.InvariantCulture).Length + 49;                    
                }
            }

            if (MultipartRequest)
            {
                contentLength += boundary.Length + 4;                
            }

            return contentLength;
        }
    }
}