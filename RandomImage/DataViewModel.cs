using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace RandomImage
{
    public class DataViewModel : INotifyPropertyChanged
    {
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

        public string GenerateRamdomImageUrl(int length = 6)
        {
            string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string url = "http://imgur.com/";

            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                url += allChars[rand.Next(0, allChars.Length)];
            }

            return url += ".jpg";            
        }

        public string GetRandomImageUrl()
        {
            string randomImageUrl = GenerateRamdomImageUrl(5);

            //string randomImageUrl = "";
            //for (int i = 0; i < Retries; i++)
            //{
            //    randomImageUrl = GenerateRamdomImageUrl();

            //    try
            //    {
            //        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(randomImageUrl);
            //        httpWebRequest.Method = "HEAD";
            //        httpWebRequest.Timeout = 10000;

            //        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            //        if (httpWebResponse.ResponseUri.LocalPath == "/removed.png")
            //        {
            //            Console.WriteLine("\nBad image url");
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Exception: {0}", ex.Message );
            //        Console.WriteLine(" Url: {0}", randomImageUrl);
            //    }
            //}

            return randomImageUrl;
        }

        public void RandomImage()
        {
            //ImageSrc = GetRandomImageUrl();

            Random rand = new Random();
            ImageSrc = RandomImageList[rand.Next(0, RandomImageList.Count)];

            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            bm.UriSource = new Uri(ImageSrc, UriKind.Absolute);
            bm.EndInit();

            Bitmap = bm;            
        }

        public void DoImgurAuth()
        {
            string clientId = "";
            string authUrl = "https://api.imgur.com/3/gallery/random/random/0";
            
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Client-ID " + clientId);
                
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    RandomImageList = new List<string>();
                    StringBuilder strBuilder = new StringBuilder();
                    Stream stream = httpWebResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(stream, encode);
                    Char[] read = new Char[1024];
                    int count = readStream.Read(read, 0, 1024);
                    while (count > 0)
                    {
                        strBuilder.Append(read);
                        //Console.WriteLine(str);
                        count = readStream.Read(read, 0, 1024);
                    }

                    string str = strBuilder.ToString();
                    str = str.Substring(9);
                    string[] images = str.Split('}');

                    foreach (string imageStr in images)
                    {
                        string temp = imageStr + "}";
                        if (temp[0] == ',') temp = temp.Substring(1);
                        //Console.WriteLine(temp);

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
                            Console.WriteLine(imgurObj.link);
                            RandomImageList.Add(imgurObj.link);
                        }
                    }
                    
                    readStream.Close();
                }
                httpWebResponse.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Properties

        public List<string> RandomImageList
        {
            get;
            set;
        }

        public int Retries
        {
            get;
            set;
        }

        private BitmapImage bm = null;
        public BitmapImage Bitmap
        {
            get
            {
                if (bm == null)
                {
                    bm = new BitmapImage();
                    bm.BeginInit();
                    bm.UriSource = new Uri(imageSrc, UriKind.Absolute);
                    bm.EndInit();
                }
                return bm;
            }

            set
            {
                bm = value;
                OnPropertyChanged("Bitmap");
            }
        }

        private string imageSrc = "http://imgur.com/wZ3eK.jpg";
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
                }
            }
        }
        #endregion
    }
}
