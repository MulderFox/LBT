using System;
using System.IO;
using System.Web;

namespace LBT.Services
{
    public class RangeFilePathResult : RangeFileResult
    {
        private const int BufferSize = 0x1000;

        public RangeFilePathResult(string contentType, string fileName, DateTime modificationDate, long fileLength)
            : base(contentType, fileName, modificationDate, fileLength)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
        }

        protected override void WriteEntireEntity(HttpResponseBase response)
        {
            try
            {
                response.TransmitFile(FileName);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        protected override void WriteEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex)
        {
            int length = Convert.ToInt32(rangeEndIndex - rangeStartIndex) + 1;

            try
            {
                response.TransmitFile(FileName, rangeStartIndex, length);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        protected override void WriteMultipartEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex)
        {
            using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(rangeStartIndex, SeekOrigin.Begin);

                int bytesRemaining = Convert.ToInt32(rangeEndIndex - rangeStartIndex) + 1;
                var buffer = new byte[BufferSize];

                while (bytesRemaining > 0)
                {
                    int bytesRead = stream.Read(buffer, 0, BufferSize < bytesRemaining ? BufferSize : bytesRemaining);
                    response.OutputStream.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                }
            }
        }
    }
}