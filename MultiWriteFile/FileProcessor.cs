using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MultiWriteFile
{
    internal class FileProcessor
    {
        public string FileName { get; set; }

        private int _lines = 0;

        public int Lines
        {
            get { return _lines; }
        }

        public FileProcessor(string filePath)
        {
            if (string.IsNullOrEmpty(filePath.Trim()))
            {
                throw new ArgumentNullException("filePath", "File path cannot be null or empty");
            }

            if(filePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException("filePath", "File path contains invalid characters");
            }

            if(string.IsNullOrEmpty(Path.GetDirectoryName(filePath)) || string.IsNullOrEmpty(Path.GetFileName(filePath)))
            {
                throw new ArgumentException("filePath", "File path does not refer to a valid file path");
            }

            FileName = filePath;

            InitFile();
        }

        public void WriteLIne(string text)
        {
            using (var streamWriter = new StreamWriter(FileName, true))
            {
                streamWriter.WriteLine(text);
                streamWriter.Flush();
            }
            
            //increment the lines counter if the write succeeds in a thread safe manner
            Interlocked.Increment(ref _lines);
        }

        private void InitFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            //Create (replace if file exists) the file
            File.Create(FileName).Close();
        }
    }
}
