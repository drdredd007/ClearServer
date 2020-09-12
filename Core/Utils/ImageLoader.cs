using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ClearServerCore.Core.Utils
{
    class ImageLoader
    {

        public static async void ImageLoad(string b64Content)
        {
            byte[] buffer = Convert.FromBase64String(b64Content.Split(',')[1]);

            using (FileStream fs = new FileStream("D:/testimg64.png", FileMode.Create))
            {
               await fs.WriteAsync(buffer);
            }

        }

    }
}

