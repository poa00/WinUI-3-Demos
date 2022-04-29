using System.Collections.ObjectModel;
using System.IO;

namespace PhotoViewer
{
    public class ImageInfo
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public string FullName
        {
            get { return System.IO.Path.Combine(Path, Name);}
        }
        public ImageInfo(string path, string name)
        {
            Path = path;
            Name = name;
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
                Images.Add(new ImageInfo(file.DirectoryName, file.Name));
            }
        }
    }    
}
