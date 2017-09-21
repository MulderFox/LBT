using System.Web;
using LBT.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Regex = System.Text.RegularExpressions.Regex;

namespace LBT.Services.ffMpeg
{
    public class Converter
    {
        public string FfExe
        {
            get { return _ffExe; }
            set { _ffExe = value; }
        }
        private string _ffExe;

        public string WorkingPath
        {
            get { return _workingPath; }
            set { _workingPath = value; }
        }
        private string _workingPath;

        public Converter(HttpServerUtilityBase server)
        {
            Initialize(server);
        }

        public Converter(HttpServerUtilityBase server, string ffmpegExePath)
        {
            _ffExe = ffmpegExePath;
            Initialize(server);
        }

        public static Image LoadImageFromFile(string fileName)
        {
            Image image;
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var imageBytes = new byte[fileStream.Length];
                fileStream.Read(imageBytes, 0, imageBytes.Length);

                image = Image.FromStream(new MemoryStream(imageBytes));
                // ReSharper disable RedundantAssignment
                imageBytes = null;
                // ReSharper restore RedundantAssignment
            }

            GC.Collect();

            return image;
        }

        public static MemoryStream LoadMemoryStreamFromFile(string fileName)
        {
            MemoryStream ms;
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);

                ms = new MemoryStream(fileBytes);
            }

            GC.Collect();

            return ms;
        }

        public VideoFile GetVideoInfo(MemoryStream inputFile, string filename)
        {
            string tempfile = Path.Combine(WorkingPath, Guid.NewGuid() + Path.GetExtension(filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();

            var vf = new VideoFile(tempfile);

            GetVideoInfo(vf);

            try
            {
                File.Delete(tempfile);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            return vf;
        }

        public VideoFile GetVideoInfo(string inputPath)
        {
            var vf = new VideoFile(inputPath);
            GetVideoInfo(vf);
            return vf;
        }

        public void GetVideoInfo(VideoFile input)
        {
            //set up the parameters for video info
            string Params = String.Format("-i \"{0}\"", input.Path);
            string output = RunProcess(Params);
            input.RawInfo = output;

            //get duration
            var re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            Match m = re.Match(input.RawInfo);

            if (m.Success)
            {
                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new[] { ':', '.' });
                if (timepieces.Length == 4)
                {
                    input.Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }

            //get audio bit rate
            re = new Regex("[B|b]itrate:.((\\d|:)*)");
            m = re.Match(input.RawInfo);
            double kb = 0.0;
            if (m.Success)
            {
                Double.TryParse(m.Groups[1].Value, out kb);
            }
            input.BitRate = kb;

            //get the audio format
            re = new Regex("[A|a]udio:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.AudioFormat = m.Value;
            }

            //get the video format
            re = new Regex("[V|v]ideo:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.VideoFormat = m.Value;
            }

            //get the video format
            re = new Regex("(\\d{2,3})x(\\d{2,3})");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                int width; int height;
                int.TryParse(m.Groups[1].Value, out width);
                int.TryParse(m.Groups[2].Value, out height);
                input.Width = width;
                input.Height = height;
            }
            input.InfoGathered = true;
        }

        public OutputPackage ConvertToFLV(MemoryStream inputFile, string filename)
        {
            string tempfile = Path.Combine(WorkingPath, Guid.NewGuid() + Path.GetExtension(filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();

            var vf = new VideoFile(tempfile);
            OutputPackage oo = ConvertToFLV(vf);

            try
            {
                File.Delete(tempfile);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            return oo;
        }

        public OutputPackage ConvertToFLV(string inputPath)
        {
            var vf = new VideoFile(inputPath);
            OutputPackage oo = ConvertToFLV(vf);
            return oo;
        }

        public OutputPackage ConvertToFLV(VideoFile input)
        {
            if (!input.InfoGathered)
            {
                GetVideoInfo(input);
            }
            var outputPackage = new OutputPackage();

            //set up the parameters for getting a previewimage
            string filename = Guid.NewGuid() + ".jpg";

            //divide the duration in 3 to get a preview image in the middle of the clip
            //instead of a black image from the beginning.
            var secs = (int)Math.Round(TimeSpan.FromTicks(input.Duration.Ticks / 3).TotalSeconds, 0);

            string finalpath = Path.Combine(WorkingPath, filename);

            //These are the parameters for setting up a preview image that must be passed to ffmpeg.
            //Note that we are asking for a jpeg image at our specified seconds.
            string Params = String.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, secs);

            string output = RunProcess(Params);

            outputPackage.RawOutput = output;

            //Ok, so hopefully we now have a preview file.  If the file
            //did not get created properly, try again at the first frame.
            if (File.Exists(finalpath))
            {
                //load that file into our output package and attempt to delete the file
                //since we no longer need it.
                outputPackage.PreviewImage = LoadImageFromFile(finalpath);
                try
                {
                    File.Delete(finalpath);
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                }
            }
            else
            {
                //try running again at frame 1 to get something
                Params = String.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, 1);
                output = RunProcess(Params);

                outputPackage.RawOutput = output;

                if (File.Exists(finalpath))
                {
                    outputPackage.PreviewImage = LoadImageFromFile(finalpath);
                    try
                    {
                        File.Delete(finalpath);
                    }
                    catch (Exception e)
                    {
                        Logger.SetLog(e);
                    }
                }
            }

            filename = Guid.NewGuid() + ".flv";
            finalpath = Path.Combine(WorkingPath, filename);

            //Now we are going to actually create the converted file.  Note that we are asking for
            //a video at 22khz 64bit.  This can be changed by a couple quick alterations to this line,
            //or by extending out this class to offer multiple different conversions.
            Params = String.Format("-i {0} -y -ar 22050 -ab 64 -f flv {1}", input.Path, finalpath);
            RunProcess(Params);

            //Check to see if our conversion file exists and then load the converted
            //file into our output package.  If the file does exist and we are able to
            //load it into our output package, we can delete the work file.
            if (File.Exists(finalpath))
            {
                outputPackage.VideoStream = LoadMemoryStreamFromFile(finalpath);
                try
                {
                    File.Delete(finalpath);
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                }
            }
            return outputPackage;
        }

        private void Initialize(HttpServerUtilityBase server)
        {
            //first make sure we have a value for the ffexe file setting
            if (String.IsNullOrEmpty(_ffExe))
            {
                object exeLocation = ConfigurationManager.AppSettings["ffmpeg:ExeLocation"];
                if (exeLocation == null)
                {
                    throw new Exception("Could not find the location of the ffmpeg exe file.  The path for ffmpeg.exe " +
                    "can be passed in via a constructor of the ffmpeg class (this class) or by setting in the app.config or web.config file.  " +
                    "in the appsettings section, the correct property name is: ffmpeg:ExeLocation");
                }

                if (String.IsNullOrEmpty(exeLocation.ToString()))
                {
                    throw new Exception("No value was found in the app setting for ffmpeg:ExeLocation");
                }

                _ffExe = exeLocation.ToString();
            }

            //Now see if ffmpeg.exe exists
            string ffExePath = GetWorkingFile(server);
            if (String.IsNullOrEmpty(ffExePath))
            {
                //ffmpeg doesn't exist at the location stated.
                throw new Exception("Could not find a copy of ffmpeg.exe");
            }

            _ffExe = ffExePath;

            //now see if we have a temporary place to work
            if (!string.IsNullOrEmpty(_workingPath))
                return;

            object workingPath = ConfigurationManager.AppSettings["ffmpeg:WorkingPath"];
            _workingPath = workingPath != null ? workingPath.ToString() : String.Empty;
            _workingPath = FileService.GetAbsoluteFilePath(server, _workingPath);
        }

        private string GetWorkingFile(HttpServerUtilityBase server)
        {
            //try the stated directory
            string absoluteFfExeFilePath = FileService.GetAbsoluteFilePath(server, _ffExe);
            if (File.Exists(absoluteFfExeFilePath))
            {
                return absoluteFfExeFilePath;
            }

            //oops, that didn't work, try the base directory
            //well, now we are really unlucky, let's just return null
            return File.Exists(Path.GetFileName(_ffExe)) ? Path.GetFileName(_ffExe) : null;
        }

        private string RunProcess(string parameters)
        {
            var oInfo = new ProcessStartInfo(_ffExe, parameters)
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            };

            string output = String.Empty;
            try
            {
                using (Process proc = Process.Start(oInfo))
                {
                    proc.WaitForExit();

                    using (StreamReader srOutput = proc.StandardError)
                    {
                        output = srOutput.ReadToEnd();
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return output;
        }
    }
}