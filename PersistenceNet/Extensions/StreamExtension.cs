using System.Drawing;

namespace PersistenceNet.Extensions
{
    public static class StreamExtension
    {
        public static string ByteToBase64(this byte[] arrByte)
        {
            if (arrByte is null)
                return string.Empty;

            return Convert.ToBase64String(arrByte);
        }

        public static byte[] CreateThumbnail(this byte[] value, int large)
        {
            if (value == null || value.Length == 0)
                return null;

            byte[] ReturnedThumbnail;

            using (MemoryStream StartMemoryStream = new(), NewMemoryStream = new())
            {
                StartMemoryStream.Write(value, 0, value.Length);

#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
                Bitmap startBitmap = new(StartMemoryStream);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma

                int newHeight;
                int newWidth;
                double HW_ratio;
#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
                if (startBitmap.Height > startBitmap.Width)
                {
                    newHeight = large;
                    HW_ratio = (double)((double)large / (double)startBitmap.Height);
                    newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                }
                else
                {
                    newWidth = large;
                    HW_ratio = (double)((double)large / (double)startBitmap.Width);
                    newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                }
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma

#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
                Bitmap newBitmap = new(newWidth, newHeight);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma

                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma

                ReturnedThumbnail = NewMemoryStream.ToArray();
            }

            return ReturnedThumbnail;
        }

        static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
            Bitmap resizedImage = new(width, height);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma

#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma
            }
            return resizedImage;
        }
    }
}