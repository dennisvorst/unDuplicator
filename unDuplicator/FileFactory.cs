using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unDuplicator
{
    class FileFactory
    {
        /* static stuff */
        static long _totalFiles;
        static long _totalBytes;

        /* properties */
        private List<FileFactory> _duplicates;
        private string _destination;
        private string _extension;
        private string _filename;
        private bool _matched;
        private long _size;
        private string _source;
        private string _sourcePath;

        public FileFactory(string path)
        {
            if (path == "") { throw new ArgumentNullException(); }
            if (!File.Exists(path)) { throw new FileNotFoundException(); }

            /* add the file properties */
            FileInfo fi = new FileInfo(path);
            Filename = fi.Name;
            Extension = fi.Extension;
            Size = fi.Length;
            Source = path;

            /* update the static */
            _totalFiles++;
            _totalBytes += Size;
        }

        public List<FileFactory> GetDuplicates(List<FileFactory> haystack)
        {
            if (_duplicates == null)
            {
                _duplicates = new List<FileFactory>();
            }
            if (!_duplicates.Any())
            {
                /* match to filename and filesize */
                var SortedList = from file in haystack
                                 where file.Source != Source
                                    && file.Size == Size
                                    && file.Extension == Extension
                                 //where file.SourcePath != SourcePath
                                 //&& file.Filename == Filename
                                 //&& file._creationTime == _creationTime
                                 //&& file._creationTime == _creationTime
                                    && file.Matched == false
                                 select file;

                foreach (FileFactory item in SortedList)
                {
                    //                    if (IsBinaryIdentical(item))
                    //                    {
                    /* found a duplicate */
                    _duplicates.Add(item);

                    /* set the source and the destination for the first file as the samen */
                    Destination = Source;
                    Matched = true;

                    /* update the duplicate */
                    item.Matched = true;
                    item.Destination = Source;
                    //                    }
                }

                /* before we proceed make sure the destination of all the files is the same. */
            }
            return _duplicates;
        }

        /* from https://stackoverflow.com/questions/968935/compare-binary-files-in-c-sharp */
        /// <summary>
        /// Binary comparison of two files
        /// </summary>
        /// <param name="fileName1">the file to compare</param>
        /// <param name="fileName2">the other file to compare</param>
        /// <returns>a value indicateing weather the file are identical</returns>
        public bool IsBinaryIdentical(FileFactory file)
        {
            FileInfo info1 = new FileInfo(Source);
            FileInfo info2 = new FileInfo(file.Source);
            bool same = info1.Length == info2.Length;
            if (same)
            {
                using (FileStream fs1 = info1.OpenRead())
                using (FileStream fs2 = info2.OpenRead())
                using (BufferedStream bs1 = new BufferedStream(fs1))
                using (BufferedStream bs2 = new BufferedStream(fs2))
                {
                    for (long i = 0; i < info1.Length; i++)
                    {
                        if (bs1.ReadByte() != bs2.ReadByte())
                        {
                            same = false;
                            break;
                        }
                    }
                }
            }

            return same;
        }

        /* properties */
        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public bool Matched
        {
            get { return _matched; }
            set { _matched = value; }
        }

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }
        public static long TotalBytes
        {
            get { return _totalBytes; }
            set { _totalBytes = value; }
        }
        public static long TotalFiles
        {
            get { return _totalFiles; }
            set { _totalFiles = value; }
        }
    }
}
