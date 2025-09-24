using Raylib_cs;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace IceSaw2.Utilities
{
    public static class LoadEmbededImage
    {

        public static Image LoadImage(string Name)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();

            // Construct the resource path (namespace.subfolders.imageName.extension)
            // Example: If your project's default namespace is "MyProject" and the image is in a "Resources" folder,
            // and the image is named "myImage.png", the path would be "MyProject.Resources.myImage.png"
            string resourcePath = "IceSaw2.Assets."+Name;

            // Get the embedded resource stream
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Image embeddedImage = new Image();
            if (myStream != null)
            {
                byte[] imageBytes = new byte[myStream.Length];
                myStream.Read(imageBytes, 0, (int)myStream.Length);
                unsafe
                {
                    fixed (byte* unmanagedPointer = imageBytes)
                    {
                        // Now 'unmanagedPointer' points to the first element of 'managedByteArray'
                        embeddedImage = Raylib.LoadImageFromMemory(GetSBytePointer(".png"), unmanagedPointer, imageBytes.Length);
                    }
                }
                myStream.Dispose();
            }

            return embeddedImage;
        }

        private unsafe static sbyte* GetSBytePointer(string inputString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            sbyte[] sbytes = new sbyte[bytes.Length];
            Buffer.BlockCopy(bytes, 0, sbytes, 0, bytes.Length);

            fixed (sbyte* ptr = sbytes)
            {
                return ptr;
            }
        }

    }
}
