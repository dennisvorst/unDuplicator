using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unDuplicator
{

    class FolderFactory
    {
        /* static stuff */
        static long _totalFolders;

        /* folder contains files and subfolders */
        private List<FileFactory> _fileCollection;
        private List<FolderFactory> _subfolderCollection;
        private long _subfoldersCount;

        /* properties */
        private string _name;
        private string _sourcePath;

        public FolderFactory(string folder)
        {
            /* check the folder */
            if (folder == "") { throw new ArgumentNullException(); }
            if (!Directory.Exists(folder)) { throw new DirectoryNotFoundException(); }

            /* init */
            FolderFactory._totalFolders++;

            /* get the folder properties */
            DirectoryInfo di = new DirectoryInfo(folder);
            Name = di.Name;
            SourcePath = di.FullName;

            /* process the folder data */
            _fileCollection = _getFileCollection();
            _subfolderCollection = _getSubfolderCollection();
        }

        /* get the folders in the current directory */
        private List<FolderFactory> _getSubfolderCollection()
        {
            /* have we done this before ? */
            if (_subfolderCollection == null)
            {
                _subfolderCollection = new List<FolderFactory>();
            }
            if (!_subfolderCollection.Any())
            {
                /* get the folders */
                string[] folders = Directory.GetDirectories(SourcePath, "*", System.IO.SearchOption.TopDirectoryOnly);
                /* process each folder */
                foreach (string folder in folders)
                {
                    /* turn it in to a factory */
                    FolderFactory ff = new FolderFactory(folder);
                    /* add it to the subfolders */
                    _subfolderCollection.Add(ff);
                }
            }
            return _subfolderCollection;
        }

        /* get the files in the current directory */
        private List<FileFactory> _getFileCollection()
        {
            if (_fileCollection == null)
            {
                _fileCollection = new List<FileFactory>();
            }
            if (!_fileCollection.Any())
            {
                string[] files = Directory.GetFiles(SourcePath, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    FileFactory ff = new FileFactory(file);
                    _fileCollection.Add(ff);
                }
            }
            return _fileCollection;
        }

        /* process the subfolders  */
        public List<FolderFactory> GetSubFolders(List<FolderFactory> folders)
        {
            foreach (FolderFactory subfolder in _subfolderCollection)
            {
                folders.Add(subfolder);
                folders = subfolder.GetSubFolders(folders);
                _subfoldersCount++;
            }
            return folders;
        }

        /* get the files in the folder tree */
        public List<FileFactory> GetFiles(List<FileFactory> allFiles)
        {
            foreach (FileFactory file in _fileCollection)
            {
                allFiles.Add(file);
            }
            foreach (FolderFactory folder in _subfolderCollection)
            {
                allFiles = folder.GetFiles(allFiles);
            }
            return allFiles;
        }

        /* properties */
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }
        public static long TotalFolders
        {
            get { return _totalFolders; }
            set { _totalFolders = value; }
        }
    }
}