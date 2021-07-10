using FFMediaToolkit.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Utils
{
    public static class Texture2DExt
    {
        public static void Save(this Texture2D texture, SharpDX.Direct3D11.Device device, string path)
        {
            SharpDX.WIC.ImagingFactory2 f = new ImagingFactory2();
            var bitmap = texture.ToBitmap(device, f);
            using (var s = new System.IO.StreamWriter(path))
            {
                s.BaseStream.Position = 0;
                using (var bitmapEncoder = new PngBitmapEncoder(f, s.BaseStream))
                {
                    using (var bitmapFrameEncode = new BitmapFrameEncode(bitmapEncoder))
                    {
                        bitmapFrameEncode.Initialize();
                        bitmapFrameEncode.SetSize(bitmap.Size.Width, bitmap.Size.Height);
                        var pixelFormat = PixelFormat.FormatDontCare;
                        bitmapFrameEncode.SetPixelFormat(ref pixelFormat);
                        bitmapFrameEncode.WriteSource(bitmap);
                        bitmapFrameEncode.Commit();
                        bitmapEncoder.Commit();
                    }
                }
            }
            bitmap.Dispose();
        }

        public static T[,] ReadAsArray<T>(this Texture2D texture, SharpDX.Direct3D11.Device device) where T : unmanaged
        {
            var textureCopy = new Texture2D(device, new Texture2DDescription
            {
                Width = (int)texture.Description.Width,
                Height = (int)texture.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = texture.Description.Format,
                Usage = ResourceUsage.Staging,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            });
            device.ImmediateContext.CopyResource(texture, textureCopy);

            DataStream dataStream;
            var dataBox = device.ImmediateContext.MapSubresource(
                textureCopy,
                0,
                0,
                MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);

            int w = texture.Description.Width;
            int h = texture.Description.Height;
            T[,] arr = new T[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    T val;
                    unsafe
                    {
                        val = *(T*)(dataBox.DataPointer + (y * dataBox.RowPitch) + x * sizeof(int)).ToPointer();
                    }
                    arr[x, y] = val;

                }
            }

            device.ImmediateContext.UnmapSubresource(textureCopy, 0);
            textureCopy.Dispose();

            return arr;
        }

        public static Bitmap ToBitmap(this Texture2D texture, SharpDX.Direct3D11.Device device)
        {
            SharpDX.WIC.ImagingFactory2 f = new ImagingFactory2();
            return texture.ToBitmap(device, f);
        }

        public static Bitmap ToBitmap(this Texture2D texture, SharpDX.Direct3D11.Device device, ImagingFactory2 f)
        {
            var textureCopy = new Texture2D(device, new Texture2DDescription
            {
                Width = (int)texture.Description.Width,
                Height = (int)texture.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = texture.Description.Format,
                Usage = ResourceUsage.Staging,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            });
            device.ImmediateContext.CopyResource(texture, textureCopy);

            DataStream dataStream;
            var dataBox = device.ImmediateContext.MapSubresource(
                textureCopy,
                0,
                0,
                MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);

            var dataRectangle = new DataRectangle
            {
                DataPointer = dataStream.DataPointer,
                Pitch = dataBox.RowPitch
            };

            

            var bitmap = new Bitmap(
                f,
                textureCopy.Description.Width,
                textureCopy.Description.Height,
                PixelFormat.Format32bppBGRA,
                dataRectangle);
            device.ImmediateContext.UnmapSubresource(textureCopy, 0);
            textureCopy.Dispose();

            return bitmap;
        }

        public static System.Drawing.Bitmap ToDrawingBitmap(this Texture2D texture, SharpDX.Direct3D11.Device device)
        {
            var textureCopy = new Texture2D(device, new Texture2DDescription
            {
                Width = (int)texture.Description.Width,
                Height = (int)texture.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = texture.Description.Format,
                Usage = ResourceUsage.Staging,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            });
            device.ImmediateContext.CopyResource(texture, textureCopy);

            DataStream dataStream;
            var dataBox = device.ImmediateContext.MapSubresource(
                textureCopy,
                0,
                0,
                MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);

            int w = texture.Description.Width;
            int h = texture.Description.Height;

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //T[,] arr = new T[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Vector4_B8G8R8A8 val;
                    unsafe
                    {
                        val = *(Vector4_B8G8R8A8*)(dataBox.DataPointer + (y * dataBox.RowPitch) + x * sizeof(int)).ToPointer();
                    }
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(val.a, val.r, val.g, val.b));
                }
            }

            device.ImmediateContext.UnmapSubresource(textureCopy, 0);
            textureCopy.Dispose();
            return bitmap;
        }

    }
}
