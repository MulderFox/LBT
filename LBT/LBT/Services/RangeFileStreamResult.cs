﻿using System;
using System.IO;
using System.Web;

namespace LBT.Services
{
    public class RangeFileStreamResult : RangeFileResult
    {
        private const int BufferSize = 0x1000;

        /// <summary>
        /// Gets the stream that will be sent to the response.
        /// </summary>
        public Stream FileStream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RangeFileStreamResult class.
        /// </summary>
        /// <param name="fileStream">The stream to send to the response.</param>
        /// <param name="contentType">The content type to use for the response.</param>
        /// <param name="fileName">The file name to use for the response.</param>
        /// <param name="modificationDate">The file modification date to use for the response.</param>
        /**
         * <remarks>
         * The <paramref name="modificationDate"/> parameter is used internally while creating ETag and Last-Modified headers. Those headers might by used by client in order to verify that the same entity is being requested in separated partial requests and for caching purposes. Because of that it is important that the value passed to this parameter is consitant and reflects the actual state of entity during its entire lifetime.
         * </remarks>
         */
        public RangeFileStreamResult(Stream fileStream, string contentType, string fileName, DateTime modificationDate)
            : base(contentType, fileName, modificationDate, fileStream.Length)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            FileStream = fileStream;
        }

        /// <summary>
        /// Writes the entire file to the response.
        /// </summary>
        /// <param name="response">The response from context within which the result is executed.</param>
        protected override void WriteEntireEntity(HttpResponseBase response)
        {
            using (FileStream)
            {
                var buffer = new byte[BufferSize];

                while (true)
                {
                    int bytesRead = FileStream.Read(buffer, 0, BufferSize);
                    if (bytesRead == 0)
                        break;

                    response.OutputStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        /// <summary>
        /// Writes the file range to the response.
        /// </summary>
        /// <param name="response">The response from context within which the result is executed.</param>
        /// <param name="rangeStartIndex">Range start index</param>
        /// <param name="rangeEndIndex">Range end index</param>
        protected override void WriteEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex)
        {
            WriteMultipartEntityRange(response, rangeStartIndex, rangeEndIndex);

            try
            {
                response.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        protected override void WriteMultipartEntityRange(HttpResponseBase response, long rangeStartIndex, long rangeEndIndex)
        {
            using (FileStream)
            {
                FileStream.Seek(rangeStartIndex, SeekOrigin.Begin);

                int bytesRemaining = Convert.ToInt32(rangeEndIndex - rangeStartIndex) + 1;
                var buffer = new byte[BufferSize];

                while (bytesRemaining > 0)
                {
                    int bytesRead = FileStream.Read(buffer, 0, BufferSize < bytesRemaining ? BufferSize : bytesRemaining);
                    response.OutputStream.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                }
            }
        }
    }
}