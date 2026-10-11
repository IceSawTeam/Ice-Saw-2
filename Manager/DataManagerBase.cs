using IceSaw2.RayWarp;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Manager
{
    public static class DataManagerBase
    {
        public static List<TextureData> LoadImages(string LoadDirectory)
        {
            //Texture Data
            var TempList = new List<TextureData>();
            string[] TextureFiles = Directory.GetFiles(LoadDirectory, "*.png", SearchOption.AllDirectories);
            var result = TextureFiles.OrderBy(x => x).ToArray();
            for (int i = 0; i < TextureFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(TextureFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(TextureFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                TempList.Add(textureData);
            }

            return TempList;
        }


        public struct TextureData
        {
            public string Name;
            public Texture2D texture2D;
        }

        public struct MeshData
        {
            public string Name;
            public MeshRef mesh;
        }
    }
}
