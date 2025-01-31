using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeleGuide
{
    public class FileTreeItem
    {
        public string Name { get; set; }
        public string Type { get; set; } // "Folder" or "PDF"
        public string FullPath { get; set; }
        public ObservableCollection<FileTreeItem> Items { get; set; }

        public FileTreeItem()
        {
            Items = new ObservableCollection<FileTreeItem>();
        }
    }
}
