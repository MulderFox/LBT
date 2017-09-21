using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LBT.Services
{
    public class FileService
    {
        private const string InvoiceFolder = "Invoices";

        public static string SaveInvoiceFile(string text, string invoiceNumber)
        {
            string invoiceFolder = Path.Combine(HttpContext.Current.Server.MapPath("~/"), InvoiceFolder);
            if (!Directory.Exists(invoiceFolder))
            {
                Directory.CreateDirectory(invoiceFolder);
            }

            string filename = String.Format("Invoice - {0}.html", invoiceNumber);
            string filePath = Path.Combine(invoiceFolder, filename);

            File.WriteAllText(filePath, text, Encoding.UTF8);
            return filePath;
        }

        public static long DirSize(string sourceDir, bool recurse)
        {
            long size = 0;
            string[] fileEntries = Directory.GetFiles(sourceDir);

            foreach (string fileName in fileEntries)
            {
                Interlocked.Add(ref size, (new FileInfo(fileName)).Length);
            }

            if (recurse)
            {
                string[] subdirEntries = Directory.GetDirectories(sourceDir);

                Parallel.For<long>(0, subdirEntries.Length, () => 0, (i, loop, subtotal) =>
                {
                    if ((File.GetAttributes(subdirEntries[i]) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {
                        subtotal += DirSize(subdirEntries[i], true);
                        return subtotal;
                    }

                    return 0;
                },
                    x => Interlocked.Add(ref size, x)
                );
            }

            return size;
        }

        public static string GetAbsoluteFilePath(HttpServerUtilityBase server, string relativeFilePath)
        {
            string absoluteFilePath = server.MapPath(String.Format("~/{0}", relativeFilePath));
            return absoluteFilePath;
        }
    }
}