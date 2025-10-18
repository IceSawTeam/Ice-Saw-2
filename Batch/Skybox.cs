using IceSaw2.Manager.Tricky;
using SSXMultiTool.JsonFiles.Tricky;
using System.Numerics;

namespace IceSaw2.Batch
{
    public class Skybox
    {
        public static (Raylib_cs.Mesh, Raylib_cs.Image) FromFolder(string skyboxFolderPath)
        {
            // Load Materials
            var materialJson = MaterialJsonHandler.Load(Path.Combine(skyboxFolderPath, "Materials.json"));
            List<Raylib_cs.Image> materialTextures = []; // Index is the ID
            foreach (var material in materialJson.Materials)
            {
                materialTextures.Add(Raylib_cs.Raylib.LoadImage(Path.Combine(skyboxFolderPath, "Textures", material.TexturePath)));
            }

            // Load Models
            var modelJson = ModelJsonHandler.Load(Path.Combine(skyboxFolderPath, "Models.json"));
            List<Model> models = [];
            foreach (var model in modelJson.Models)
            {
                foreach (var modelObject in model.ModelObjects)
                {
                    foreach (var meshData in modelObject.MeshData)
                    {
                        var tempModel = new Model
                        {
                            Mesh = ObjImporter.ObjLoad(Path.Combine(skyboxFolderPath, "Meshes", meshData.MeshPath)),
                            Texture = materialTextures[meshData.MaterialID]
                        };
                        models.Add(tempModel);
                    }
                }
            }

            // Sort models by texture height
            models.Sort((a, b) => b.Texture.Height.CompareTo(a.Texture.Height));

            // Generate Padded Atlas
            const int padding = 4;
            int resolution = GetAtlasEstimateResolution(materialTextures, padding);
            var batchedTexture = Raylib_cs.Raylib.GenImageColor(resolution, resolution, Raylib_cs.Color.Black);
            int rowHighestHeight = 0;
            int cursorX = 0;
            int cursorY = 0;
            foreach (var model in models)
            {
                var paddedImage = PadImage(model.Texture, padding);
                if (cursorX + paddedImage.Width > resolution)
                {
                    cursorY += rowHighestHeight;
                    cursorX = 0;
                    rowHighestHeight = 0;
                }
                var paddedRect = new Raylib_cs.Rectangle(cursorX, cursorY, paddedImage.Width, paddedImage.Height);
                var uvRect = new Raylib_cs.Rectangle(cursorX + padding, cursorY + padding, paddedImage.Width - padding * 2, paddedImage.Height - padding * 2);
                Raylib_cs.Raylib.ImageDraw(ref batchedTexture, paddedImage,
                                        new Raylib_cs.Rectangle(0, 0, paddedImage.Width, paddedImage.Height),
                                        paddedRect,
                                        Raylib_cs.Color.White);
                cursorX += paddedImage.Width;
                rowHighestHeight = Math.Max(rowHighestHeight, paddedImage.Height);

                // Update mesh's UV coords to atlas
                var ModelVertices = model.Mesh.TexCoordsAs<Vector2>();
                for (int i = 0; i < ModelVertices.Length; i++)
                {
                    ModelVertices[i].Y *= -1;

                    // Size in normalized space
                    var width = uvRect.Width / resolution;
                    var height = uvRect.Height / resolution;

                    // Offset position in normalized space
                    var offsetX = uvRect.X / resolution;
                    var offsetY = uvRect.Y / resolution;

                    // Flip the UV map upside down.
                    offsetY += height;
                    height *= -1;

                    ModelVertices[i] = new Vector2
                    {
                        X = (ModelVertices[i].X * width) + offsetX,
                        Y = (ModelVertices[i].Y * height) + offsetY,
                    };
                }
            }

            // It's Merging time
            int vertexCount = 0;
            int triangleCount = 0;
            foreach (var model in models)
            {
                vertexCount += model.Mesh.VertexCount;
                triangleCount += model.Mesh.TriangleCount;
            }
            var batchedMesh = new Raylib_cs.Mesh(vertexCount, triangleCount);
            batchedMesh.AllocVertices();
            batchedMesh.AllocIndices();
            batchedMesh.AllocTexCoords();
            batchedMesh.AllocNormals();
            var batchedMeshVertices = batchedMesh.VerticesAs<Vector3>();
            var batchedMeshNormal = batchedMesh.NormalsAs<Vector3>();
            var batchedMeshTexCord = batchedMesh.TexCoordsAs<Vector2>();
            var batchedMeshIndices = batchedMesh.IndicesAs<ushort>();
            int vertexIndex = 0;
            int indicesIndex = 0;
            int normalsIndex = 0;
            int TexIndex = 0;
            int PrevIndex = 0;
            foreach (var model in models)
            {
                var ModelVertices = model.Mesh.VerticesAs<Vector3>();
                var ModelMeshNormal = model.Mesh.NormalsAs<Vector3>();
                var ModelMeshIndices = model.Mesh.IndicesAs<ushort>();
                var ModelMeshTex = model.Mesh.TexCoordsAs<Vector2>();
                for (var i = 0; i < ModelVertices.Length; i++)
                {
                    batchedMeshVertices[vertexIndex] = ModelVertices[i];
                    vertexIndex++;
                }
                for (var i = 0; i < ModelMeshIndices.Length; i++)
                {
                    batchedMeshIndices[indicesIndex] = (ushort)(ModelMeshIndices[i] + (ushort)PrevIndex);
                    indicesIndex++;
                }
                for (var i = 0; i < ModelMeshNormal.Length; i++)
                {
                    batchedMeshNormal[normalsIndex] = ModelMeshNormal[i];
                    normalsIndex++;
                }
                for (var i = 0; i < ModelMeshTex.Length; i++)
                {
                    batchedMeshTexCord[TexIndex] = ModelMeshTex[i];
                    TexIndex++;
                }
                PrevIndex += ModelVertices.Length;
            }
            return (batchedMesh, batchedTexture);
        }

        public static (Raylib_cs.Mesh, Raylib_cs.Image) FromLoaded()
        {
            List<Raylib_cs.Image> materialTextures = []; // Index is the ID
            foreach (var material in TrickyDataManager.trickySkyboxMaterialObject)
            {
                materialTextures.Add(Raylib_cs.Raylib.LoadImageFromTexture(TrickyDataManager.ReturnTexture(material.TexturePath,true)));
            }

            // Load Models
            List<Model> models = [];
            foreach (var model in TrickyDataManager.trickySkyboxPrefabObjects)
            {
                foreach (var modelObject in model.trickyModelMeshObjects)
                {
                    foreach (var meshData in modelObject.trickyModelMaterialObjects)
                    {
                        var tempModel = new Model
                        {
                            Mesh = TrickyDataManager.ReturnMesh(meshData.MeshPath,true),
                            Texture = materialTextures[meshData.MaterialIndex]
                        };
                        models.Add(tempModel);
                    }
                }
            }

            // Sort models by texture height
            models.Sort((a, b) => b.Texture.Height.CompareTo(a.Texture.Height));

            // Generate Padded Atlas
            const int padding = 4;
            int resolution = GetAtlasEstimateResolution(materialTextures, padding);
            var batchedTexture = Raylib_cs.Raylib.GenImageColor(resolution, resolution, Raylib_cs.Color.Black);
            int rowHighestHeight = 0;
            int cursorX = 0;
            int cursorY = 0;
            foreach (var model in models)
            {
                var paddedImage = PadImage(model.Texture, padding);
                if (cursorX + paddedImage.Width > resolution)
                {
                    cursorY += rowHighestHeight;
                    cursorX = 0;
                    rowHighestHeight = 0;
                }
                var paddedRect = new Raylib_cs.Rectangle(cursorX, cursorY, paddedImage.Width, paddedImage.Height);
                var uvRect = new Raylib_cs.Rectangle(cursorX + padding, cursorY + padding, paddedImage.Width - padding * 2, paddedImage.Height - padding * 2);
                Raylib_cs.Raylib.ImageDraw(ref batchedTexture, paddedImage,
                                        new Raylib_cs.Rectangle(0, 0, paddedImage.Width, paddedImage.Height),
                                        paddedRect,
                                        Raylib_cs.Color.White);
                cursorX += paddedImage.Width;
                rowHighestHeight = Math.Max(rowHighestHeight, paddedImage.Height);

                // Update mesh's UV coords to atlas
                var ModelVertices = model.Mesh.TexCoordsAs<Vector2>();
                for (int i = 0; i < ModelVertices.Length; i++)
                {
                    ModelVertices[i].Y *= -1;

                    // Size in normalized space
                    var width = uvRect.Width / resolution;
                    var height = uvRect.Height / resolution;

                    // Offset position in normalized space
                    var offsetX = uvRect.X / resolution;
                    var offsetY = uvRect.Y / resolution;

                    // Flip the UV map upside down.
                    offsetY += height;
                    height *= -1;

                    ModelVertices[i] = new Vector2
                    {
                        X = (ModelVertices[i].X * width) + offsetX,
                        Y = (ModelVertices[i].Y * height) + offsetY,
                    };
                }
            }

            // It's Merging time
            int vertexCount = 0;
            int triangleCount = 0;
            foreach (var model in models)
            {
                vertexCount += model.Mesh.VertexCount;
                triangleCount += model.Mesh.TriangleCount;
            }
            var batchedMesh = new Raylib_cs.Mesh(vertexCount, triangleCount);
            batchedMesh.AllocVertices();
            batchedMesh.AllocIndices();
            batchedMesh.AllocTexCoords();
            batchedMesh.AllocNormals();
            var batchedMeshVertices = batchedMesh.VerticesAs<Vector3>();
            var batchedMeshNormal = batchedMesh.NormalsAs<Vector3>();
            var batchedMeshTexCord = batchedMesh.TexCoordsAs<Vector2>();
            var batchedMeshIndices = batchedMesh.IndicesAs<ushort>();
            int vertexIndex = 0;
            int indicesIndex = 0;
            int normalsIndex = 0;
            int TexIndex = 0;
            int PrevIndex = 0;
            foreach (var model in models)
            {
                var ModelVertices = model.Mesh.VerticesAs<Vector3>();
                var ModelMeshNormal = model.Mesh.NormalsAs<Vector3>();
                var ModelMeshIndices = model.Mesh.IndicesAs<ushort>();
                var ModelMeshTex = model.Mesh.TexCoordsAs<Vector2>();
                for (var i = 0; i < ModelVertices.Length; i++)
                {
                    batchedMeshVertices[vertexIndex] = ModelVertices[i];
                    vertexIndex++;
                }
                for (var i = 0; i < ModelMeshIndices.Length; i++)
                {
                    batchedMeshIndices[indicesIndex] = (ushort)(ModelMeshIndices[i] + (ushort)PrevIndex);
                    indicesIndex++;
                }
                for (var i = 0; i < ModelMeshNormal.Length; i++)
                {
                    batchedMeshNormal[normalsIndex] = ModelMeshNormal[i];
                    normalsIndex++;
                }
                for (var i = 0; i < ModelMeshTex.Length; i++)
                {
                    batchedMeshTexCord[TexIndex] = ModelMeshTex[i];
                    TexIndex++;
                }
                PrevIndex += ModelVertices.Length;
            }
            return (batchedMesh, batchedTexture);
        }

        private static Raylib_cs.Image PadImage(Raylib_cs.Image image, int padding)
        {
            var result = Raylib_cs.Raylib.GenImageColor(image.Width + padding * 2,
                                                        image.Height + padding * 2,
                                                        Raylib_cs.Color.Black);

            // Paste image's center
            var src = new Raylib_cs.Rectangle(0, 0, image.Width, image.Height);
            var dst = new Raylib_cs.Rectangle(padding, padding, image.Width, image.Height);
            Raylib_cs.Raylib.ImageDraw(ref result, image, src, dst, Raylib_cs.Color.White);

            // Draw edges
            var srcTop = new Raylib_cs.Rectangle(0, 0, image.Width, 1);
            var dstTop = new Raylib_cs.Rectangle(padding, 0, image.Width, padding);
            Raylib_cs.Raylib.ImageDraw(ref result, image, srcTop, dstTop, Raylib_cs.Color.White);
            
            var srcBottom = new Raylib_cs.Rectangle(0, image.Height - 1, image.Width, 1);
            var dstBottom = new Raylib_cs.Rectangle(padding, padding + image.Height, image.Width, padding);
            Raylib_cs.Raylib.ImageDraw(ref result, image, srcBottom, dstBottom, Raylib_cs.Color.White);
            
            var srcLeft = new Raylib_cs.Rectangle(0, 0, 1, image.Height);
            var dstLeft = new Raylib_cs.Rectangle(0, padding, padding, image.Height);
            Raylib_cs.Raylib.ImageDraw(ref result, image, srcLeft, dstLeft, Raylib_cs.Color.White);
            
            var srcRight = new Raylib_cs.Rectangle(image.Width - 1, 0, 1, image.Height);
            var dstRight = new Raylib_cs.Rectangle(padding + image.Width, padding, padding, image.Height);
            Raylib_cs.Raylib.ImageDraw(ref result, image, srcRight, dstRight, Raylib_cs.Color.White);

            // Draw corners
            var topLeft = Raylib_cs.Raylib.GetImageColor(image, 0, 0);
            Raylib_cs.Raylib.ImageDrawRectangle(ref result, 0, 0, padding, padding, topLeft);

            var topRight = Raylib_cs.Raylib.GetImageColor(image, image.Width - 1, 0);
            Raylib_cs.Raylib.ImageDrawRectangle(ref result, padding + image.Width, 0, padding, padding, topRight);

            var bottomLeft = Raylib_cs.Raylib.GetImageColor(image, 0, image.Height - 1);
            Raylib_cs.Raylib.ImageDrawRectangle(ref result, 0, padding + image.Height, padding, padding, bottomLeft);

            var bottomRight = Raylib_cs.Raylib.GetImageColor(image, image.Width - 1, image.Height - 1);
            Raylib_cs.Raylib.ImageDrawRectangle(ref result, padding + image.Width, padding + image.Height, padding, padding, bottomRight);
            return result;
        }

        private static int GetAtlasEstimateResolution(List<Raylib_cs.Image> images, int padding)
        {
            int fullPadding = padding * 2;
            int totalArea = 0;
            foreach (var image in images)
            {
                totalArea += (image.Width + fullPadding) * (image.Height + fullPadding);
            }
            int estimatedArea = (int)(totalArea / 0.60);
            var res = (int)Math.Round(Math.Sqrt(estimatedArea));
            return RoundToNearestPowerOfTwo(res);
        }

        private static int RoundToNearestPowerOfTwo(int value)
        {
            int nextPowerOfTwo = 1;
            while (nextPowerOfTwo < value)
            {
                nextPowerOfTwo *= 2;
            }
            return nextPowerOfTwo;
        }

        private struct Model
        {
            public Raylib_cs.Mesh Mesh;
            public Raylib_cs.Image Texture;
        }
    }
}



