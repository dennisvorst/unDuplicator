using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace unDuplicator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        /** Button event */
        private void btnGo_Click(object sender, EventArgs e)
        {
            if (_SourceIsEmpty())
            {
                return;
            }

            /* setup the logfile */
            string logfile = "Unduplicator.log";
            using (StreamWriter outputFile = new StreamWriter(logfile))
            {
                /** Start logging */
                outputFile.WriteLine("Logging start");
                outputFile.WriteLine("=============");


                /* process the folders */
                outputFile.WriteLine("Getting all the folder content");
                List<FolderFactory> allFolders = new List<FolderFactory>();
                List<FileFactory> allFiles = new List<FileFactory>();
                foreach (string item in listBoxIn.Items)
                {
                    /* analyse the folder */
                    FolderFactory ff = new FolderFactory(item);
                    allFolders.Add(ff);
                    allFolders = ff.GetSubFolders(allFolders);

                    allFiles = ff.GetFiles(allFiles);
                }

                /* log how much we are processing */
                outputFile.WriteLine($"Processing {FileFactory.TotalFiles} files");
                outputFile.WriteLine($"Processing {FolderFactory.TotalFolders} folders");
                outputFile.WriteLine($"Processing {FileFactory.TotalBytes} bytes");

                /* start looking for duplicate files */
                foreach (FileFactory file in allFiles)
                {
                    file.GetDuplicates(allFiles);
                }

                /* look for identical folders */


                /* log identical files */
                outputFile.WriteLine("Identifying single duplicate files");
                outputFile.WriteLine("-------------------------");
                foreach (FileFactory file in allFiles)
                {
                    List<FileFactory> duplicates = file.GetDuplicates(new List<FileFactory>());
                    if (duplicates.Count > 0)
                    {
                        outputFile.WriteLine($"Not Moving {file.Source} To {file.Destination} ");

                        foreach (FileFactory duplicate in duplicates)
                        {
                            outputFile.WriteLine($"Moving {duplicate.Source} To {duplicate.Destination} ");
                        }
                        outputFile.WriteLine("-------------------------");
                    }
                }
            }
            /** show the log file */
            System.Diagnostics.Process.Start(logfile);
        }

        private bool _SourceIsEmpty()
        {
            if ((listBoxIn.Items.Count == 0))
            {
                MessageBox.Show("There are no folders to be processed.");
                return true;
            }
            return false;
        }


        /** Drag and drop events */
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] folders = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string folder in folders)
            {
                /* check if it is a valid folder on the system */
                if (Directory.Exists(folder))
                {
                    /* only when it doesnot exist already */
                    bool isFound = false;
                    foreach (String item in listBoxIn.Items)
                    {
                        if (item == folder)
                        {
                            isFound = true;
                        }
                    }

                    /* if it doesnot exist add it. */
                    if (!isFound)
                    {
                        listBoxIn.Items.Add(folder);
                    }
                }
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            /* reset the listbox */
            listBoxIn.Items.Clear();
        }
    }
}
