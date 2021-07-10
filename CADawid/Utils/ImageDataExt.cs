using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Utils
{
    public static class ImageDataExt
    {
        public static unsafe System.Drawing.Bitmap ToBitmap(this ImageData bitmap)
        {
            fixed (byte* p = bitmap.Data)
            {
                return new System.Drawing.Bitmap(bitmap.ImageSize.Width, bitmap.ImageSize.Height, bitmap.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, new IntPtr(p));
            }
        }
    }
}
