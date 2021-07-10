using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Utils
{
    public static class BitmapExt
    {
        public static byte[] ToByteArray(this System.Drawing.Bitmap bitmap)
        {

            System.Drawing.Imaging.BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        public static ImageData ToImageData(this System.Drawing.Bitmap bitmap)
        {
            var rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size);
            var bitLock = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgra32, bitmap.Size); // must be permuted format in compare to original bitmap (video encoding related?)
            bitmap.UnlockBits(bitLock);
            return bitmapData;
        }
    }
}
