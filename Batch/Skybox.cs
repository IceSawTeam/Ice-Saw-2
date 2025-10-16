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

            int resolution = GetAtlasEstimateResolution(materialTextures);
            var batchedTexture = Raylib_cs.Raylib.GenImageColor(resolution, resolution, Raylib_cs.Color.Black);
            int rowHighestHeight = 0;
            int cursorX = 0;
            int cursorY = 0;

            foreach (var model in models)
            {
                var image = model.Texture;
                if (cursorX + image.Width > resolution)
                {
                    cursorY += rowHighestHeight;
                    cursorX = 0;
                    rowHighestHeight = 0;
                }
                var destRectangle = new Raylib_cs.Rectangle(cursorX, cursorY, image.Width, image.Height);

                Raylib_cs.Raylib.ImageDraw(ref batchedTexture, image,
                                        new Raylib_cs.Rectangle(0, 0, image.Width, image.Height),
                                        destRectangle,
                                        Raylib_cs.Color.White);
                cursorX += image.Width;
                rowHighestHeight = Math.Max(rowHighestHeight, image.Height);

                var ModelVertices = model.Mesh.TexCoordsAs<Vector2>();

                for (int i = 0; i < ModelVertices.Length; i++)
                {
                    // Size in normalized space
                    var width = destRectangle.Width / resolution;
                    var height = destRectangle.Height / resolution;

                    // Offset position in normalized space
                    var offsetX = destRectangle.X / resolution;
                    var offsetY = destRectangle.Y / resolution;

                    ModelVertices[i] = new Vector2((ModelVertices[i].X * width) + offsetX, (ModelVertices[i].Y * height) + offsetY);
                }
            }

            // Merging time
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

            // Vertices
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

        private static int GetAtlasEstimateResolution(List<Raylib_cs.Image> images)
        {
            int totalArea = 0;
            foreach (var image in images)
            {
                totalArea += image.Width * image.Height;
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



