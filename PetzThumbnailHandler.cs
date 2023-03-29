using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

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
}
