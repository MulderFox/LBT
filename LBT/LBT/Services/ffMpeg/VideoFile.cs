using System;
using System.IO;

namespace LBT.Services.ffMpeg
{
    public class VideoFile
    {
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        private string _path;

        public TimeSpan Duration { get; set; }
        public double BitRate { get; set; }
        public string AudioFormat { get; set; }
        public string VideoFormat { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string RawInfo { get; set; }
        public bool InfoGathered { get; set; }

        public VideoFile(string path)
        {
            _path = path;
            Initialize();
        }

        private void Initialize()
        {
            InfoGathered = false;
            
            //first make sure we have a value for the video file setting
            if (String.IsNullOrEmpty(_path))
            {
                throw new Exception("Could not find the location of the video file");
            }

            //Now see if the video file exists
            if (!File.Exists(_path))
            {
                throw new Exception("The video file " + _path + " does not exist.");
            }
        }
    }
}