using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace MC_Launcher.Source
{
    public class AccessAPI
    {
        private string name = "";
        private string id = "";

        public string NAME { get { return name; } set { name = value; } }
        public string UUID { get { return id; } set { id = value; } }

        public async void GetUUID(string username)
        {
            string url = $"https://api.mojang.com/users/profiles/minecraft/{username}?at=0";

            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<UUID_OBJ>(response);

            name = data.name;
            id = data.id;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource GetSkinFromAPI(string uuid)
        {
            try
            {
                WebClient downloader = new WebClient();
                Stream st = downloader.OpenRead($"https://mc-heads.net/body/{uuid}/right");
                Bitmap img = Bitmap.FromStream(st) as Bitmap;

                var handle = img.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(handle);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class UUID_OBJ
        {
            public string name { get; set; }
            public string id { get; set; }
        }
    }
}
