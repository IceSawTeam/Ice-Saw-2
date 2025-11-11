using IceSaw2.LevelObject;
using IceSaw2.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace IceSaw2.Renderer
{
    public class TessellatedPatch
    {
        private TessellatedPatch() { }
        private static readonly TessellatedPatch _instance = new();
        public static TessellatedPatch Instance { get { return _instance; } }

        private const float LightmapPixelSize = 0.00625f; // Pixel size for a 160x160 lightmap;

        private Raylib_cs.Mesh _mesh;
        private Raylib_cs.Material _material;
        private readonly List<Raylib_cs.Texture2D> _paddedLightmaps = [];
        private readonly List<PatchEntry?> _patchEntries = [];
        private readonly List<Batch> _drawList = [];

        public bool WireOverlayEnabled = false;
        public bool LightmapEnabled = true;

        Matrix4x4[] identities = new Matrix4x4[8];

        Matrix4x4 PreviousView = new Matrix4x4();

        // DEBUG
        public int PatchCount()
        {
            return _patchEntries.Count;
        }

        public void Init(List<Raylib_cs.Image> lightmaps)
        {
            GenerateMesh();
            Raylib_cs.Raylib.UploadMesh(ref _mesh, false);
            GeneratePaddedLightmaps(lightmaps);
            Raylib_cs.Raylib.SetTraceLogLevel(Raylib_cs.TraceLogLevel.All);

            var shader = Raylib_cs.Raylib.LoadShaderFromMemory(LoadEmbeddedFile.LoadText("Shaders.TessellatedPatchBatch.vs", System.Text.Encoding.UTF8),
                                            LoadEmbeddedFile.LoadText("Shaders.TessellatedPatchBatch.fs", System.Text.Encoding.UTF8));
            Raylib_cs.Raylib.SetTraceLogLevel(Raylib_cs.TraceLogLevel.Info);
            _material = Raylib_cs.Raylib.LoadMaterialDefault();
            _material.Shader = shader;
            identities = [.. identities.Select(x => BaseObject.Default)];

            unsafe { _material.Shader.Locs[(int)Raylib_cs.ShaderLocationIndex.MatrixModel] = Raylib_cs.Raylib.GetShaderLocationAttrib(_material.Shader, "instanceTransform"); }

            int tex0Loc = Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "diffuseTexture");
            Raylib_cs.Raylib.SetShaderValue(_material.Shader, tex0Loc, 12, Raylib_cs.ShaderUniformDataType.Sampler2D);

            int tex1Loc = Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "lightmap");
            Raylib_cs.Raylib.SetShaderValue(_material.Shader, tex1Loc, 13, Raylib_cs.ShaderUniformDataType.Sampler2D);
        }

        public void Clear()
        {
            _patchEntries.Clear();
            foreach (var lightmap in _paddedLightmaps)
            {
                Raylib_cs.Raylib.UnloadTexture(lightmap);
            }
        }

        public int AddPatch(
                Vector3[] Controlpoints,
                Raylib_cs.Texture2D Texture,
                Vector2[] TextureUV,
                int LightmapID,
                Vector2[] LightmapUV,
                bool Highlighted)
        {
            bool foundEmptySpot = false;
            int foundID = 0;
            for (var i = 0; i < _patchEntries.Count; i++)
            {
                if (_patchEntries[i] == null)
                {
                    foundEmptySpot = true;
                    foundID = i;
                    break;
                }
            }

            PatchEntry entry = new()
            {
                Controlpoints = Controlpoints,
                Texture = Texture,
                TextureUV = TextureUV,
                LightmapID = LightmapID,
                LightmapUV = LightmapUV,
                Highlighted = Highlighted
            };
            entry.UpdateBoundingSphere();

            //Readjust points for padding tiles

            entry.LightmapUV[0] = ConvertLightmapUV(entry.LightmapUV[0]);
            entry.LightmapUV[1] = ConvertLightmapUV(entry.LightmapUV[1]);
            entry.LightmapUV[2] = ConvertLightmapUV(entry.LightmapUV[2]);
            entry.LightmapUV[3] = ConvertLightmapUV(entry.LightmapUV[3]);

            entry.LightmapUV[0] += new Vector2(LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[1] += new Vector2(LightmapPixelSize, -LightmapPixelSize);
            entry.LightmapUV[2] += new Vector2(-LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[3] += new Vector2(-LightmapPixelSize, -LightmapPixelSize);

            if (foundEmptySpot)
            {
               _patchEntries[foundID] = entry;
            }
            else
            {
                _patchEntries.Add(entry);
               foundID = _patchEntries.Count - 1;
            }
            return foundID;
        }

        Vector2 ConvertLightmapUV(Vector2 uv)
        {
            float oldTextureSize = 128f;
            float newTextureSize = 160f;
            float oldTileSize = 8f;
            float newTileSize = 10f;

            // Convert UV to pixel space
            float oldPixelX = uv.X * oldTextureSize;
            float oldPixelY = uv.Y * oldTextureSize;

            // Map pixels to new texture with padding
            float newPixelX = (oldPixelX / oldTileSize) * newTileSize;
            float newPixelY = (oldPixelY / oldTileSize) * newTileSize;

            // Convert back to normalized UVs
            float newUVX = newPixelX / newTextureSize;
            float newUVY = newPixelY / newTextureSize;

            return new Vector2(newUVX, newUVY);
        }

        public void DeletePatch(int patchID)
        {
            _patchEntries[patchID] = null;
        }

        public void UpdatePatchControlPoints(int patchID, Vector3[] Controlpoints)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.Controlpoints = Controlpoints;
            entry.UpdateBoundingSphere();
        }

        public void UpdatePatchTexture(int patchID, Raylib_cs.Texture2D texture, Vector2[] textureUV)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.Texture = texture;
            entry.TextureUV = textureUV;
        }

        public void UpdatePatchLightmap(int patchID, int lightmapID, Vector2[] lightmapUV)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.LightmapID = lightmapID;
            entry.LightmapUV = lightmapUV;
            entry.LightmapUV[0] = ConvertLightmapUV(entry.LightmapUV[0]);
            entry.LightmapUV[1] = ConvertLightmapUV(entry.LightmapUV[1]);
            entry.LightmapUV[2] = ConvertLightmapUV(entry.LightmapUV[2]);
            entry.LightmapUV[3] = ConvertLightmapUV(entry.LightmapUV[3]);

            entry.LightmapUV[0] += new Vector2(LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[1] += new Vector2(LightmapPixelSize, -LightmapPixelSize);
            entry.LightmapUV[2] += new Vector2(-LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[3] += new Vector2(-LightmapPixelSize, -LightmapPixelSize);
        }

        public void UpdatePatchHighlight(int patchID, bool Highlight)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.Highlighted = Highlight;
        }

        public void UpdateLightmaps(List<Raylib_cs.Image> lightmaps)
        {
            GeneratePaddedLightmaps(lightmaps);
        }

        public void Render()
        {
            unsafe { if (_material.Shader.Locs == null) return; }

            Matrix4x4 projection = Raylib_cs.Rlgl.GetMatrixModelview();
            if (PreviousView != projection)
            {
                PreviousView = projection;
                _drawList.Clear();
                List<Batch> drawList = new List<Batch>();

                foreach (var entry in _patchEntries)
                {
                    if (entry == null) continue;

                    // Frustum cull and fill draw list by batching them
                    if (FrustumCulling.IsSphereInFrustum(entry.Sphere.Position, entry.Sphere.Radius))
                    {
                        int ID = -1;
                        //Check drawList for Free space
                        for (int i = 0; i < drawList.Count; i++)
                        {
                            if (drawList[i].TextureID == entry.Texture.Id && drawList[i].LightmapID == _paddedLightmaps[entry.LightmapID].Id)
                            {
                                ID = i;
                                drawList[i].Patches.Add(entry);
                                drawList[i].PatchCount++;
                                break;
                            }
                        }

                        //If no list found make new list
                        if (ID == -1)
                        {
                            Batch batch = new Batch();
                            batch.Patches.Add(entry);
                            batch.PatchCount++;
                            batch.TextureID = entry.Texture.Id;
                            batch.LightmapID = _paddedLightmaps[entry.LightmapID].Id;
                            drawList.Add(batch);
                            ID = drawList.Count - 1;
                        }
                        //If list full add to main draw list
                        else if (drawList[ID].Patches.Count == 8)
                        {
                            _drawList.Add(drawList[ID]);
                            drawList.RemoveAt(ID);
                        }
                    }
                }

                _drawList.AddRange(drawList);
            }

            //Set if lightmap is enabled
            Raylib_cs.Raylib.SetShaderValue(
            _material.Shader,
            Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "lightmapsEnabled"),
            LightmapEnabled ? 1 : 0,
            Raylib_cs.ShaderUniformDataType.Int
            );

            uint PrevTextureID = 0;
            uint PrevLightmapID = 0;

            for (int a = 0; a < _drawList.Count; a++)
            {
                var batch = _drawList[a];

                Raylib_cs.Raylib.SetShaderValueV(
                    _material.Shader,
                    Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "controlPoints[0]"),
                    batch.GetMergedControlPoints(),
                    Raylib_cs.ShaderUniformDataType.Vec3,
                    batch.PatchCount * 16
                );
                Raylib_cs.Raylib.SetShaderValueV(
                    _material.Shader,
                    Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "diffuseTextureUVs[0]"),
                    batch.GetMergedDiffuseTextureUVs(),
                    Raylib_cs.ShaderUniformDataType.Vec2,
                    batch.PatchCount * 4
                );
                Raylib_cs.Raylib.SetShaderValueV(
                    _material.Shader,
                    Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "lightmapTextureUVs[0]"),
                    batch.GetMergedLightmapTextureUVs(),
                    Raylib_cs.ShaderUniformDataType.Vec2,
                    batch.PatchCount * 4
                );

                // Textures
                //If texture ID Different Set ID update
                if (PrevTextureID != batch.TextureID)
                {
                    PrevTextureID = batch.TextureID;
                    Raylib_cs.Rlgl.ActiveTextureSlot(12);
                    Raylib_cs.Rlgl.EnableTexture(batch.Patches[0].Texture.Id);
                }

                if (PrevLightmapID != batch.LightmapID)
                {
                    PrevLightmapID = batch.LightmapID;
                    Raylib_cs.Rlgl.ActiveTextureSlot(13);
                    Raylib_cs.Rlgl.EnableTexture(PrevLightmapID);
                }

                Raylib_cs.Raylib.SetShaderValueV(
                    _material.Shader,
                    Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "highlighted[0]"),
                    batch.GetMergedHighlighted(),
                    Raylib_cs.ShaderUniformDataType.Int,
                    batch.PatchCount
                );

                Raylib_cs.Raylib.DrawMeshInstanced(
                    _mesh,
                    _material,
                    identities,
                    batch.PatchCount
                );
            }
            Raylib_cs.Rlgl.ActiveTextureSlot(12);
            Raylib_cs.Rlgl.DisableTexture();
            Raylib_cs.Rlgl.ActiveTextureSlot(13);
            Raylib_cs.Rlgl.DisableTexture();
        }

        private void GenerateMesh()
        {
            _mesh = new(64, 98);
            _mesh.AllocVertices();
            _mesh.AllocIndices();
            _mesh.AllocTexCoords();
            _mesh.AllocTexCoords2();
            var verts = _mesh.VerticesAs<Vector3>();
            var texcoords = _mesh.TexCoordsAs<Vector2>();
            var texcoords2 = _mesh.TexCoords2As<Vector2>();
            var indices = _mesh.IndicesAs<ushort>();

            // Set vertices and texture UVs
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    verts[y * 8 + x] = new Vector3(x / 7.0f, y / 7.0f, 0);
                    texcoords[y * 8 + x] = new Vector2(x / 7.0f, y / 7.0f);
                    texcoords2[y * 8 + x] = new Vector2(x / 7.0f, y / 7.0f);
                }
            }

            // Set triangle indices
            // Quads are created from the bottom-left of the 4 tiled vertices.
            var indexCount = 0;
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    ushort vertexIndex = (ushort)(y * 8 + x);

                    indices[indexCount] = vertexIndex;
                    indices[indexCount + 1] = (ushort)(vertexIndex + 8);
                    indices[indexCount + 2] = (ushort)(vertexIndex + 8 + 1);
                    
                    indices[indexCount + 3] = vertexIndex;
                    indices[indexCount + 4] = (ushort)(vertexIndex + 8 + 1);
                    indices[indexCount + 5] = (ushort)(vertexIndex + 1);
                    indexCount += 6;
                }
            }
        }

        private void GeneratePaddedLightmaps(List<Raylib_cs.Image> lightmaps)
        {
            foreach (var lightmap in lightmaps)
            {
                var paddedLightmap = Raylib_cs.Raylib.GenImageColor(160, 160, Raylib_cs.Color.Blank);
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        var srcRect = new Raylib_cs.Rectangle(x * 8, y * 8, 8, 8);
                        var destRect = new Raylib_cs.Rectangle(x * 10, y * 10, 10, 10);
                        var paddedTile = PadTile(Raylib_cs.Raylib.ImageFromImage(lightmap, srcRect), 1);
                        Raylib_cs.Raylib.ImageDraw(ref paddedLightmap, paddedTile, new Raylib_cs.Rectangle(0, 0, 10, 10), destRect, Raylib_cs.Color.White);
                    }
                }
                var TempLightmap = Raylib_cs.Raylib.LoadTextureFromImage(paddedLightmap);

                Raylib_cs.Raylib.SetTextureFilter(TempLightmap, Raylib_cs.TextureFilter.Bilinear);

                _paddedLightmaps.Add(TempLightmap);
            }
        }

        private static Raylib_cs.Image PadTile(Raylib_cs.Image image, int padding)
        {
            var result = Raylib_cs.Raylib.GenImageColor(image.Width + padding * 2,
                                                        image.Height + padding * 2,
                                                        Raylib_cs.Color.Blank);

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

        private class PatchEntry
        {
            public Vector3[] Controlpoints = new Vector3[16];
            public Raylib_cs.Texture2D Texture;
            public Vector2[] TextureUV = new Vector2[4]; // List count is 4
            public int LightmapID;
            public Vector2[] LightmapUV = new Vector2[4]; // List count is 4
            public bool Highlighted;
            public BoundingSphere Sphere = new(new Vector3(0), 0);

            public void UpdateBoundingSphere()
            {
                Vector3 centerOfMass = new();
                foreach (var controlPoint in Controlpoints)
                {
                    centerOfMass += controlPoint * BaseObject.WorldScale;
                }
                centerOfMass /= Controlpoints.Length;

                float furthestDistance = 0;
                foreach (var controlPoint in Controlpoints)
                {
                    var dist = Vector3.DistanceSquared(controlPoint * BaseObject.WorldScale, centerOfMass);
                    furthestDistance = dist > furthestDistance ? dist : furthestDistance;
                }
                furthestDistance = MathF.Sqrt(furthestDistance);
                Sphere.Position = centerOfMass;
                Sphere.Radius = furthestDistance;
            }
        }

        private class BoundingSphere(Vector3 position, float radius)
        {
            public Vector3 Position = position;
            public float Radius = radius;
        }
    
        private class Batch
        {
            // This class only holds data for 8 or less patches.
            public int PatchCount = 0;
            public List<PatchEntry> Patches = [];
            public uint TextureID;
            public uint LightmapID;

            public Vector3[] GetMergedControlPoints()
            {
                Vector3[] output = new Vector3[PatchCount*16];
                for (int i = 0; i < PatchCount; i++)
                {
                    Patches[i].Controlpoints.CopyTo(output, i*16);
                }
                return output;
            }

            public Vector2[] GetMergedDiffuseTextureUVs()
            {
                Vector2[] output = new Vector2[PatchCount * 4];
                for (int i = 0; i < PatchCount; i++)
                {
                    Patches[i].TextureUV.CopyTo(output, i * 4);
                }
                return output;
            }

            public Vector2[] GetMergedLightmapTextureUVs()
            {
                Vector2[] output = new Vector2[PatchCount * 4];
                for (int i = 0; i < PatchCount; i++)
                {
                    Patches[i].LightmapUV.CopyTo(output, i * 4);
                }
                return output;
            }

            public List<int> GetMergedLightmapIDs()
            {
                List<int> output = [];
                foreach (var patch in Patches)
                {
                    output.Add(patch.LightmapID);
                }
                return output;
            }

            public int[] GetMergedHighlighted()
            {
                int[] output = new int[PatchCount];
                for (int i = 0; i < PatchCount; i++)
                {
                    output[i] = Patches[i].Highlighted ? 1 : 0;
                }
                return output;
            }
        }
    }
}