using AsmResolver;
using AsmResolver.IO;
using AsmResolver.PE.Win32Resources;
using Kaitai;
using SharpShell.Attributes;
using SharpShell.Helpers;
using SharpShell.SharpThumbnailHandler;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Drawing.Imaging.ImageLockMode;
using static System.Drawing.Imaging.PixelFormat;

namespace PetzThumbnails
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".pet")]
    public class PetzThumbnailHandler : SharpThumbnailHandler
    {
        protected override Bitmap GetThumbnailImage(uint width)
        {
            var headerBytes = new byte[] { 0x42, 0x4d, 0xf6, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x04, 0x00, 0x00 };
            SelectedItemStream.Seek(-13704, SeekOrigin.End);
            using (var mem = new MemoryStream())
            {
                mem.Write(headerBytes, 0, headerBytes.Length);
                SelectedItemStream.CopyTo(mem);
                var bitmap = new Bitmap(mem);
                var transcolor = bitmap.Palette.Entries[253];
                bitmap.MakeTransparent(transcolor);
                return bitmap;

            }
        }
    }

    public class Helper
    {
        public static readonly Bitmap palette = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PetzThumbnails.PALETTE.bmp"));
        public static Bitmap GetThumbnail(byte[] bytes, string type)
        {
            var flhname = "restinga";
            if (type == "clo")
            {
                flhname = "awaya";
            }
            var asm = AsmResolver.PE.PEImage.FromBytes(bytes);
            var resourceTypes = asm.Resources.Entries.Where(x => x.Name == "FLM" || x.Name == "FLH");
            bool includeaway = type == "clo" || resourceTypes.Count() == 2;
            resourceTypes = resourceTypes.SelectMany(x => (x as IResourceDirectory).Entries)
                .Where(x => !x.Name.ToLower().Contains("away") || includeaway);
            resourceTypes = resourceTypes.SelectMany(x => (x as IResourceDirectory).Entries);

            BinaryStreamReader flh = resourceTypes.Where(x => x.ParentDirectory.ParentDirectory.Name == "FLH").Select(x => (x as IResourceData).CreateReader()).First();
            BinaryStreamReader flm = resourceTypes.Where(x => x.ParentDirectory.ParentDirectory.Name == "FLM").Select(x => (x as IResourceData).CreateReader()).First();

            var kaitaiflh = new Flh(new KaitaiStream(flh.ReadToEnd()));
            var frame = kaitaiflh.Frames.FirstOrDefault(x => x.Name.ToLower().Contains(flhname) && (x.Flags & 2) != 0);
            if (frame == null)
            {
                frame = kaitaiflh.Frames.First(x => (x.Flags & 2) != 0);
            }

            var bitmap = new Bitmap(frame.X2 - frame.X1, frame.Y2 - frame.Y1, Format8bppIndexed);

            int bitmapWidth = bitmap.Width;
            var offset = frame.Offset;
            if (bitmap.Width % 4 != 0)
            {
                bitmapWidth = bitmap.Width + 4 - (bitmap.Width % 4);
            }
            int size = bitmapWidth * bitmap.Height;
            flm.RelativeOffset = offset;
            byte[] bmp = flm.ReadSegment((uint)size).ToArray();
            var thelock = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), WriteOnly, Format8bppIndexed);
            bitmap.Palette = palette.Palette;
            Marshal.Copy(bmp, 0, thelock.Scan0, bmp.Length);
            bitmap.UnlockBits(thelock);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bitmap.MakeTransparent(palette.Palette.Entries[253]);

            return bitmap;
        }
    }


    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".toy")]
    public class ToyThumbnailHandler : SharpThumbnailHandler
    {
        protected override Bitmap GetThumbnailImage(uint width)
        {
            byte[] data = new byte[SelectedItemStream.Length];
            SelectedItemStream.Read(data, 0, data.Length);
            return Helper.GetThumbnail(data, "toy");
        }

    }

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".clo")]
    public class CloThumbnailHandler : SharpThumbnailHandler
    {
        protected override Bitmap GetThumbnailImage(uint width)
        {
            byte[] data = new byte[SelectedItemStream.Length];
            SelectedItemStream.Read(data, 0, data.Length);
            return Helper.GetThumbnail(data, "clo");
        }
    }

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".dog")]
    [COMServerAssociation(AssociationType.FileExtension, ".cat")]
    public class BreedThumbnailHandler : SharpThumbnailHandler
    {
        protected override Bitmap GetThumbnailImage(uint width)
        {
            var headerBytes = new byte[] { 0x42, 0x4d, 0xf6, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x04, 0x00, 0x00 };
            byte[] data = new byte[SelectedItemStream.Length];
            SelectedItemStream.Read(data, 0, data.Length);
            var asm = AsmResolver.PE.PEImage.FromBytes(data);
            var bmpResourceDir = (IResourceDirectory)asm.Resources.Entries.Where(x => x.Name == "BMP").First();
            bmpResourceDir = (IResourceDirectory)bmpResourceDir.Entries.First();
            var bmpResource = (IResourceData)bmpResourceDir.Entries.First();
            var bmp = bmpResource.Contents.WriteIntoArray();
            using (var mem = new MemoryStream())
            {
                mem.Write(bmp, 0, bmp.Length);
                var bitmap = new Bitmap(mem);
                bitmap.MakeTransparent(bitmap.Palette.Entries[253]);
                return bitmap;
            }
        }
    }
}
