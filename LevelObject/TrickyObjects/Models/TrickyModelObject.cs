using IceSaw2.Manager.Tricky;
using Raylib_cs;
using SSXLibrary.JsonFiles.Tricky;
using System.Numerics;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyModelObject : BaseObject
    {
        bool Skybox;

        public override ObjectType Type
        {
            get { return ObjectType.Prefab; }
        }

        public int Unknown3;
        public float AnimTime;

        public List<TrickyModelMeshObject> trickyModelMeshObjects = new List<TrickyModelMeshObject>();

        public void LoadPrefab(ModelJsonHandler.ModelJson prefabJson, bool skybox = false)
        {
            Skybox = skybox;

            Name = prefabJson.ModelName;
            Unknown3 = prefabJson.Unknown3;
            AnimTime = prefabJson.AnimTime;

            trickyModelMeshObjects = new List<TrickyModelMeshObject>();

            for (int i = 0; i < prefabJson.ModelObjects.Count; i++)
            {
                var TrickyPrefabMeshObject = new TrickyModelMeshObject();

                TrickyPrefabMeshObject.parent = this;

                TrickyPrefabMeshObject.LoadModelMeshObject(prefabJson.ModelObjects[i], Skybox);

                trickyModelMeshObjects.Add(TrickyPrefabMeshObject);
            }
        }

        public override void Render()
        {
            for (int i = 0; i < trickyModelMeshObjects.Count; i++)
            {
                trickyModelMeshObjects[i].Render();
            }
        }

        public void AddToRenderCache(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < trickyModelMeshObjects.Count; i++)
            {
                trickyModelMeshObjects[i].AddToRenderCache(trickyInstanceObject);
            }
        }

        public void RemoveFromRenderCache(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < trickyModelMeshObjects.Count; i++)
            {
                trickyModelMeshObjects[i].RemoveFromRenderCache(trickyInstanceObject);
            }
        }

        public List<ObjExporter.MassModelData> GenerateModel(TrickyInstanceObject trickyInstanceObject)
        {
            List<ObjExporter.MassModelData> massModelDatas = new List<ObjExporter.MassModelData>();

            int ModelID = 0;

            for (int i = 0; i < trickyModelMeshObjects.Count; i++)
            {
                for (int j = 0; j < trickyModelMeshObjects[i].meshes.Count; j++)
                {
                    ObjExporter.MassModelData data = new ObjExporter.MassModelData();

                    data.Name = trickyInstanceObject.Name + " " + ModelID;
                    data.Model = new Mesh(trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.TriangleCount*3, trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.TriangleCount);
                    data.Model.AllocVertices();
                    data.Model.AllocTexCoords();
                    data.Model.AllocIndices();
                    data.Model.AllocNormals();

                    var Vert = data.Model.VerticesAs<Vector3>();
                    var oldVert = trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.VerticesAs<Vector3>();
                    for (int k = 0; k < Vert.Length; k++)
                    {
                        Vert[k] = Raymath.Vector3Transform(oldVert[k], (trickyInstanceObject.worldMatrix4x4 * trickyModelMeshObjects[i].localMatrix4X4));
                    }

                    var Tex = data.Model.TexCoordsAs<Vector2>();
                    var oldTex = trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.TexCoordsAs<Vector2>();
                    for (int k = 0; k < Tex.Length; k++)
                    {
                        Tex[k] = new Vector2(oldTex[k].X, -oldTex[k].Y);
                    }

                    var indices = data.Model.IndicesAs<ushort>();
                    var oldindices = trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.IndicesAs<ushort>();
                    for (int k = 0; k < indices.Length; k++)
                    {
                        indices[k] = oldindices[k];
                    }

                    var Normal = data.Model.NormalsAs<Vector3>();
                    var oldNormal = trickyModelMeshObjects[i].meshes[j].meshRef.Mesh.NormalsAs<Vector3>();
                    for (int k = 0; k < Normal.Length; k++)
                    {
                        Normal[k] = oldNormal[k];
                    }

                    data.TextureName = TrickyDataManager.trickyMaterialObject[trickyModelMeshObjects[i].meshes[j].MaterialIndex].TexturePath;

                    ModelID++;

                    massModelDatas.Add(data);
                }

            }

            return massModelDatas;
        }

            //public List<RenderCache> GenerateRenderCache()
            //{
            //    List<RenderCache> cache = new List<RenderCache>();

            //    for (int i = 0; i < Children.Count; i++)
            //    {
            //        cache.AddRange(((TrickyModelMeshObject)Children[i]).GenerateRenderCache());
            //    }

            //    return cache;
            //}
        }
}
