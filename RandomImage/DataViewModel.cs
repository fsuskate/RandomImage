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

        #region Methods

        public void NextRandomImage()
        {
            Random rand = new Random();
            int randomIndex = rand.Next(0, RandomImageList.Count);
            ImageSrc = RandomImageList[randomIndex].link;
            Description = RandomImageList[randomIndex].title;
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

        public List<ImgurJsonData> GetRandomImages()
        {
            try
            {
                return GetImgurImageStream()
                    .ReadImageDataFromStream()
                    .ConvertImageDataToImages();                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
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
                    randomImageList = GetRandomImages();
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
                    bitmapImage = ImageSrc.GetBitmapFromUri();
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


    }
}
