using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;

namespace RandomImage
{
    public class DataViewModel : INotifyPropertyChanged
    {
        private const string clientId = "f609571ab85f649";
        private const string authUrl = "https://api.imgur.com/3/gallery/random/random/0";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Methods

        public void NextRandomImage()
        {
            Random rand = new Random();
            int randomIndex = rand.Next(0, RandomImageList.Count);

            if (RandomImageList != null)
            {
                ImageSrc = RandomImageList[randomIndex].link;
                Description = RandomImageList[randomIndex].title;
            }           
        }

        protected Stream GetImgurImageStream()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl);
            if (httpWebRequest != null)
            {
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Client-ID " + clientId);

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    // No need to close the connection using httpWebResponse.Close() since we will close the 
                    // response stream later.
                    return httpWebResponse.GetResponseStream();                   
                }
            }
            return null;
        }

        protected string ReadImageDataFromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new InvalidDataException("Null stream passed to ReadImageDataFromStream");
            }

            StringBuilder strBuilder = new StringBuilder();
            StreamReader readStream = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            Char[] readBuffer = new Char[1024];
            int count = readStream.Read(readBuffer, 0, 1024);
            while (count > 0)
            {
                strBuilder.Append(readBuffer);
                count = readStream.Read(readBuffer, 0, 1024);
            }
            readStream.Close();
            stream.Close();

            return strBuilder.ToString().Substring(9); 
        }

        protected void ConvertImageDataToImages(string strImageData)
        {
            if (string.IsNullOrEmpty(strImageData))
            {
                throw new InvalidDataException("Empty image data passed to ConvertImageDataToImages");
            }

            string[] images = strImageData.Split('}');
            foreach (string imageStr in images)
            {
                string temp = imageStr + "}";
                if (temp[0] == ',') temp = temp.Substring(1);

                ImgurJsonData imgurObj = null;
                try
                {
                    imgurObj = JsonConvert.DeserializeObject<ImgurJsonData>(temp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (imgurObj != null)
                {
                    RandomImageList.Add(imgurObj);
                }
            }
        }

        public void GetRandomImages()
        {
            try
            {
                ConvertImageDataToImages(ReadImageDataFromStream(GetImgurImageStream()));                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Properties

        private List<ImgurJsonData> randomImageList;
        public List<ImgurJsonData> RandomImageList
        {
            get
            {
                if (randomImageList == null)
                {
                    randomImageList = new List<ImgurJsonData>();
                }
                return randomImageList;
            }
        }

        private string descripton = "";
        public string Description
        {
            get
            {
                return descripton;
            }

            set
            {
                if (value != descripton)
                {
                    descripton = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        private BitmapImage bitmapImage = null;
        public BitmapImage Bitmap
        {
            get
            {
                if (bitmapImage == null)
                {
                    bitmapImage = GetBitmapFromUri();
                }
                return bitmapImage;
            }
        }

        private string imageSrc = "http://upload.wikimedia.org/wikipedia/commons/thumb/e/e9/Imgur_logo.svg/800px-Imgur_logo.svg.png";
        public string ImageSrc
        {
            get
            {
                return imageSrc;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    imageSrc = value;
                    OnPropertyChanged("ImageSrc");

                    bitmapImage = null;
                    OnPropertyChanged("Bitmap");
                }
            }
        }
        #endregion

        protected BitmapImage GetBitmapFromUri()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(ImageSrc, UriKind.Absolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
