using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace RandomImage
{
    public static class Extensions
    {
        public static string ReadImageDataFromStream(this Stream stream)
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

        public static List<ImgurJsonData> ConvertImageDataToImages(this string strImageData)
        {
            if (string.IsNullOrEmpty(strImageData))
            {
                throw new InvalidDataException("Empty image data passed to ConvertImageDataToImages");
            }

            List<ImgurJsonData> imgurJsonDataList = new List<ImgurJsonData>();
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
                    imgurJsonDataList.Add(imgurObj);
                }
            }
            return imgurJsonDataList;
        }

        public static BitmapImage GetBitmapFromUri(this string imageSrc)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imageSrc, UriKind.Absolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
