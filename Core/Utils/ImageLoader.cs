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

        public static async void ImageLoad(byte[] buffer)
        {
            using var fs = new FileStream("D:/IMG_3479-Pano.jpg", FileMode.Create);
            await fs.WriteAsync(buffer);


        }

    }
}

