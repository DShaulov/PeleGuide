using PeleGuide;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

public class FolderScanner
{
    public ObservableCollection<FileTreeItem> TreeItems { get; private set; }

    public FolderScanner()
    {
        TreeItems = new ObservableCollection<FileTreeItem>();
    }

    public void ScanFolder(string folderPath)
    {
        try
        {
            var rootItem = new FileTreeItem
            {
                Name = Path.GetFileName(folderPath),
                Type = "Folder",
                FullPath = folderPath
            };

            // Scan subfolders
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                ScanFolderRecursive(dir, rootItem);
            }

            // Scan PDF files in root folder
            foreach (var file in Directory.GetFiles(folderPath, "*.pdf"))
            {
                rootItem.Items.Add(new FileTreeItem
                {
                    Name = Path.GetFileName(file),
                    Type = "PDF",
                    FullPath = file
                });
            }

            TreeItems.Clear();
            TreeItems.Add(rootItem);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error scanning folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ScanFolderRecursive(string folderPath, FileTreeItem parentItem)
    {
        try
        {
            var folderItem = new FileTreeItem
            {
                Name = Path.GetFileName(folderPath),
                Type = "Folder",
                FullPath = folderPath
            };

            // Scan subfolders
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                ScanFolderRecursive(dir, folderItem);
            }

            // Scan PDF files
            foreach (var file in Directory.GetFiles(folderPath, "*.pdf"))
            {
                folderItem.Items.Add(new FileTreeItem
                {
                    Name = Path.GetFileName(file),
                    Type = "PDF",
                    FullPath = file
                });
            }

            // Only add folder if it contains items (subfolders or PDFs)
            if (folderItem.Items.Count > 0)
            {
                parentItem.Items.Add(folderItem);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error scanning folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}