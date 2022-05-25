using System;
using System.Collections.ObjectModel;
using System.IO;

namespace PhotoViewer
{
    public class ImageInfo
    {
        public string Name { get; }
        public string Path { get; }
        public ImageInfo(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
    public class ImagesRepository
    {
        public ObservableCollection<ImageInfo> Images { get; } = new();

        public void GetImages(string folder)
        {
            Images.Clear();
            var di = new DirectoryInfo(folder);
            var files = di.GetFiles("*.jpg");

            foreach (FileInfo file in files)
            {
                Images.Add(new ImageInfo(file.Name, file.FullName));
            }
        }
    }
}
