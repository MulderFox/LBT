using System;
using System.Web;

namespace LBT.Services
{
    public class RangeFileContentResult : RangeFileResult
    {
        /// <summary>
        /// Gets the binary content to send to the response.
        /// </summary>
        public byte[] FileContents { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RangeFileContentResult class.
        /// </summary>
        /// <param name="fileContents">The byte array to send to the response.</param>
        /// <param name="contentType">The content type to use for the response.</param>
        /// <param name="fileName">The file name to use for the response.</param>
        /// <param name="modificationDate">The file modification date to use for the response.</param>
        /**
         * <remarks>
         * The <paramref name="modificationDate"/> parameter is used internally while creating ETag and Last-Modified headers. Those headers might by used by client in order to verify that the same entity is being requested in separated partial requests and for caching purposes. Because of that it is important that the value passed to this parameter is consitant and reflects the actual state of entity during its entire lifetime.
         * </remarks>
         */
        public RangeFileContentResult(byte[] fileContents, string contentType, string fileName, DateTime modificationDate)
            : base(contentType, fileName, modificationDate, fileContents.Length)
        {
            if (fileContents == null)
                throw new ArgumentNullException("fileContents");

            FileContents = fileContents;
        }

        /// <summary>
        /// Writes the entire file to the response.
        /// </summary>
        /// <param name="response">The response from context within which the result is executed.</param>
        protected override void WriteEntireEntity(HttpResponseBase response)
        {
            response.OutputStream.Write(FileContents, 0, FileContents.Length);
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
            response.OutputStream.Write(FileContents, Convert.ToInt32(rangeStartIndex), Convert.ToInt32(rangeEndIndex - rangeStartIndex) + 1);
        }
    }
}