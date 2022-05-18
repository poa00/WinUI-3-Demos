using System.Collections.ObjectModel;
using System.IO;

namespace PhotoViewer
{
    public class ImageInfo
    {
        public string Path { get; }
        public string Name { get; }

        public string FullName { get; }
        public ImageInfo(string path, string name, string fullName)
        {
            Path = path;
            Name = name;
            FullName = fullName;
        }
    }
    public class ImagesRepository
    {
        public ObservableCollection<ImageInfo> Images { get; } = new ObservableCollection<ImageInfo>();

        public void GetImages(string folder)
        {
            Images.Clear();
            var di = new DirectoryInfo(folder);
            var files = di.GetFiles("*.jpg");

            foreach (FileInfo file in files)
            {
                Images.Add(new ImageInfo(file.DirectoryName, file.Name, file.FullName));
            }
        }
    }
}
