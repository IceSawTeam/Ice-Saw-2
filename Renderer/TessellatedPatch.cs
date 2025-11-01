using System.Diagnostics;

namespace IceSaw2.Renderer
{
    public class TessellatedPatch
    {
        private TessellatedPatch() { }
        private static readonly TessellatedPatch _instance = new();
        public static TessellatedPatch Instance { get { return _instance; } }

        private const float LightmapPixelSize = 0.00625f; // Pixel size for a 160x160 lightmap;

        private Raylib_cs.Mesh _mesh = new(64, 98);
        private Raylib_cs.Material _material = new();
        private readonly List<RenderPatchEntry?> _patchEntries = [];
        private readonly List<Raylib_cs.Texture2D> _paddedLightmaps = [];
        private DrawList _drawList = new();

        public bool WireOverlayEnabled = false;
        public bool LightmapEnabled = true;

        public void Init(List<Raylib_cs.Image> lightmaps)
        {
            GenerateMesh();
            Raylib_cs.Raylib.UploadMesh(ref _mesh, false);
            GeneratePaddedLightmaps(lightmaps);
            var shader = Raylib.LoadShaderFromMemory(LoadEmbeddedFile.LoadText("Shaders.TessellatedPatchBatch.vs", System.Text.Encoding.UTF8),
                                            LoadEmbeddedFile.LoadText("Shaders.TessellatedPatchBatch.fs", System.Text.Encoding.UTF8));
            _material = Raylib_cs.Raylib.LoadMaterialDefault();
            _material.Shader = shader;
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
                List<Vector3> Controlpoints,
                Raylib_cs.Texture2D Texture,
                List<Vector2> TextureUV,
                int LightmapID,
                List<Vector2> LightmapUV,
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

            RenderPatchEntry entry = new()
            {
                Controlpoints = Controlpoints,
                Texture = Texture,
                TextureUV = TextureUV,
                LightmapID = LightmapID,
                LightmapUV = LightmapUV,
                Highlighted = Highlighted
            };
            entry.UpdateBoundingSphere();

            entry.LightmapUV[0] += new Vector2(LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[1] += new Vector2(-LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[2] += new Vector2(LightmapPixelSize, -LightmapPixelSize);
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

        public void DeletePatch(int patchID)
        {
            _patchEntries[patchID] = null;
        }

        public void UpdatePatchControlPoints(int patchID, List<Vector3> Controlpoints)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.Controlpoints = Controlpoints;
            entry.UpdateBoundingSphere();
        }

        public void UpdatePatchTexture(int patchID, Raylib_cs.Texture2D texture, List<Vector2> textureUV)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.Texture = texture;
            entry.TextureUV = textureUV;
        }

        public void UpdatePatchLightmap(int patchID, int lightmapID, List<Vector2> lightmapUV)
        {
            var entry = _patchEntries[patchID];
            Debug.Assert(entry != null);
            entry.LightmapID = lightmapID;
            entry.LightmapUV = lightmapUV;
            entry.LightmapUV[0] += new Vector2(LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[1] += new Vector2(-LightmapPixelSize, LightmapPixelSize);
            entry.LightmapUV[2] += new Vector2(LightmapPixelSize, -LightmapPixelSize);
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
            _drawList.Clear();
            var frustum = GetFrustum();

            // Frustum cull and fill draw list
            foreach (var entry in _patchEntries)
            {
                if (entry == null) continue;
                if (SphereIn(frustum, entry.Sphere))
                {
                    _drawList.InstanceMatrix.Add(Matrix4x4.Identity);
                    _drawList.Controlpoints.AddRange(entry.Controlpoints);
                    _drawList.Texture.Add(entry.Texture);
                    _drawList.TextureUV.AddRange(entry.TextureUV);
                    _drawList.LightmapID.Add(entry.LightmapID);
                    _drawList.LightmapUV.AddRange(entry.LightmapUV);
                    _drawList.Highlighted.Add(entry.Highlighted);
                    break;
                }
            }

            int drawListIndex = 0;
            while (true)
            {
                int batchSize = Math.Min(8, _drawList.InstanceMatrix.Count - drawListIndex);
                if (batchSize <= 0) break;
                for (int i = 0; i < batchSize; i++)
                {
                    unsafe
                    {
                        int* locs = _material.Shader.Locs;
                        locs[(int)Raylib_cs.ShaderLocationIndex.MatrixModel] = Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "instanceTransform");
                    }
                    Raylib_cs.Raylib.SetShaderValueV(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "controlPoints"),
                        _drawList.Controlpoints.GetRange(drawListIndex * 16, 16).ToArray(),
                        Raylib_cs.ShaderUniformDataType.Vec3,
                        16
                    );
                    Raylib_cs.Raylib.SetShaderValueV(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "diffuseTextureUVs"),
                        _drawList.TextureUV.GetRange(drawListIndex * 4, 4).ToArray(),
                        Raylib_cs.ShaderUniformDataType.Vec2,
                        4
                    );
                    Raylib_cs.Raylib.SetShaderValueV(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "diffuseTextures"),
                        _drawList.Texture.GetRange(drawListIndex * 8, 8).ToArray(),
                        Raylib_cs.ShaderUniformDataType.Sampler2D,
                        8
                    );

                    Raylib_cs.Texture2D[] lightmapTextures = new Raylib_cs.Texture2D[8];
                    for (int lmIndex = 0; lmIndex < 8; lmIndex++)
                    {
                        lightmapTextures[lmIndex] = _paddedLightmaps[_drawList.LightmapID[drawListIndex * 8 + lmIndex]];
                    }
                    Raylib_cs.Raylib.SetShaderValueV(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "lightmapTextures"),
                        lightmapTextures.ToArray(),
                        Raylib_cs.ShaderUniformDataType.Sampler2D,
                        8
                    );

                    Raylib_cs.Raylib.SetShaderValueV(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "highlighted"),
                        _drawList.Highlighted.GetRange(drawListIndex * 8, 8).ToArray(),
                        Raylib_cs.ShaderUniformDataType.Sampler2D,
                        8
                    );

                    Raylib_cs.Raylib.SetShaderValue(
                        _material.Shader,
                        Raylib_cs.Raylib.GetShaderLocation(_material.Shader, "lightmapsEnabled"),
                        LightmapEnabled,
                        Raylib_cs.ShaderUniformDataType.Int
                    );

                    Raylib_cs.Raylib.DrawMeshInstanced(
                        _mesh,
                        _material,
                        _drawList.InstanceMatrix.GetRange(drawListIndex, batchSize).ToArray(),
                        batchSize
                    );
                }
                drawListIndex += 1;
            }
        }

        private void GenerateMesh()
        {
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
                    verts[y * 8 + x] = new Vector3(x, y, 0);
                    texcoords[y * 8 + x] = new Vector2(x, y);
                    texcoords2[y * 8 + x] = new Vector2(x, y);
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
                }
            }
        }

        private void GeneratePaddedLightmaps(List<Raylib_cs.Image> lightmaps)
        {
            foreach (var lightmap in lightmaps)
            {
                var paddedLightmap = Raylib_cs.Raylib.GenImageColor(160, 160, Raylib_cs.Color.Black);

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        var srcRect = new Raylib_cs.Rectangle(x * 8, y * 8, 8, 8);
                        var destRect = new Raylib_cs.Rectangle(x * 10, y * 10, 10, 10);
                        var paddedTile = PadTile(Raylib_cs.Raylib.ImageFromImage(lightmap, srcRect), 1);
                        Raylib_cs.Raylib.ImageDraw(ref paddedLightmap, paddedTile, srcRect, destRect, Raylib_cs.Color.Black);
                    }
                }
                _paddedLightmaps.Add(Raylib_cs.Raylib.LoadTextureFromImage(paddedLightmap));
            }
        }

        private static Raylib_cs.Image PadTile(Raylib_cs.Image image, int padding)
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

        private static bool SphereIn(List<Vector4> frustum, BoundingSphere sphere)
        {
            foreach (var plane in frustum)
            {
                if (DistanceToPlane(plane, sphere.Position) < -sphere.Radius)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<Vector4> GetFrustum()
        {
            Matrix4x4 projection = Raylib_cs.Rlgl.GetMatrixProjection();
            Matrix4x4 modelview = Raylib_cs.Rlgl.GetMatrixModelview();
            Matrix4x4 planes = modelview * projection;

            List<Vector4> output = [];
            var right = new Vector4(planes.M41 - planes.M11, planes.M42 - planes.M12, planes.M43 - planes.M13, planes.M44 - planes.M14);
            output.Add(Vector4.Normalize(right));
            var left = new Vector4(planes.M41 + planes.M11, planes.M42 + planes.M12, planes.M43 + planes.M13, planes.M44 + planes.M14);
            output.Add(Vector4.Normalize(left));
            var top = new Vector4(planes.M41 - planes.M21, planes.M42 - planes.M22, planes.M43 - planes.M23, planes.M44 - planes.M24);
            output.Add(Vector4.Normalize(top));
            var bottom = new Vector4(planes.M41 + planes.M21, planes.M42 + planes.M22, planes.M43 + planes.M23, planes.M44 + planes.M24);
            output.Add(Vector4.Normalize(bottom));
            var back = new Vector4(planes.M41 - planes.M31, planes.M42 - planes.M32, planes.M43 - planes.M33, planes.M44 - planes.M34);
            output.Add(Vector4.Normalize(back));
            var front = new Vector4(planes.M41 + planes.M31, planes.M42 + planes.M32, planes.M43 + planes.M33, planes.M44 + planes.M34);
            output.Add(Vector4.Normalize(front));
            return output;
        }

        public static float DistanceToPlane(Vector4 plane, Vector3 position)
        {
            return plane.X * position.X + plane.Y * position.Y + plane.Z * position.Z + plane.W;
        }

        private class RenderPatchEntry
        {
            public List<Vector3> Controlpoints = [];
            public Raylib_cs.Texture2D Texture;
            public List<Vector2> TextureUV = []; // List count is 4
            public int LightmapID;
            public List<Vector2> LightmapUV = []; // List count is 4
            public bool Highlighted;
            public BoundingSphere Sphere = new(new Vector3(0), 0);

            public void UpdateBoundingSphere()
            {
                Vector3 centerOfMass = new();
                foreach (var controlPoint in Controlpoints)
                {
                    centerOfMass += controlPoint;
                }
                centerOfMass /= Controlpoints.Count;

                float furthestDistance = 0;
                foreach (var controlPoint in Controlpoints)
                {
                    var dist = Vector3.DistanceSquared(controlPoint, centerOfMass);
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

        private class DrawList
        {
            public List<Matrix4x4> InstanceMatrix = []; // 1 per patch. Always identity.
            public List<Vector3> Controlpoints = []; // 16 per patch
            public List<Raylib_cs.Texture2D> Texture = []; // 1 per patch
            public List<Vector2> TextureUV = []; // 4 per patch
            public List<int> LightmapID = []; // 1 per patch
            public List<Vector2> LightmapUV = []; // 4 per patch
            public List<bool> Highlighted = []; // 1 per patch

            public void Clear()
            {
                InstanceMatrix.Clear();
                Controlpoints.Clear();
                Texture.Clear();
                TextureUV.Clear();
                LightmapUV.Clear();
                Highlighted.Clear();
            }
        }
    }
}