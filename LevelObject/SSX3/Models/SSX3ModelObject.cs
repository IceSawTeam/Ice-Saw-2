using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using Raylib_cs;
using SSXLibrary.FileHandlers.LevelFiles.SSX3PS2.SSB3Data;
using SSXLibrary.JsonFiles.SSX3;
using System.Numerics;
namespace IceSaw2.LevelObject.SSX3
{
    public class SSX3ModelObject : BaseObject
    {
        public override ObjectType Type
        {
            get { return ObjectType.Prefab; }
        }

        public int TrackID;
        public int RID;

        public int U3;
        public int U4;

        public float U6;

        public List<ObjectID> MaterialList;

        public List<SSX3ModelMeshObject> ssx3ModelMeshObjects = new List<SSX3ModelMeshObject>();


        public void LoadModel(MDRJsonHandler.MainModelHeader model)
        {
            Name = model.Name;

            TrackID = model.TrackID;
            RID = model.RID;

            U3 = model.U3;
            U4 = model.U4;

            U6 = model.U6;

            ssx3ModelMeshObjects = new List<SSX3ModelMeshObject>();

            for (int i = 0; i < model.ModelObjects.Count; i++)
            {
                var TempChild = new SSX3ModelMeshObject();

                TempChild.LoadPrefab(model.ModelObjects[i]);

                ssx3ModelMeshObjects.Add(TempChild);

            }
        }

        public override void Render()
        {
            for (int i = 0; i < ssx3ModelMeshObjects.Count; i++)
            {
                ssx3ModelMeshObjects[i].Render();
            }
        }

        public void AddToRenderCache(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < ssx3ModelMeshObjects.Count; i++)
            {
                ssx3ModelMeshObjects[i].AddToRenderCache(trickyInstanceObject);
            }
        }

        public void UpdateRenderCacheTransform(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < ssx3ModelMeshObjects.Count; i++)
            {
                ssx3ModelMeshObjects[i].UpdateRenderCacheTransform(trickyInstanceObject);
            }
        }

        public void RemoveFromRenderCache(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < ssx3ModelMeshObjects.Count; i++)
            {
                ssx3ModelMeshObjects[i].RemoveFromRenderCache(trickyInstanceObject);
            }
        }

        public List<ObjExporter.MassModelData> GenerateModel(SSX3InstanceObject trickyInstanceObject)
        {
            List<ObjExporter.MassModelData> massModelDatas = new List<ObjExporter.MassModelData>();

            int ModelID = 0;

            for (int i = 0; i < ssx3ModelMeshObjects.Count; i++)
            {
                for (int j = 0; j < ssx3ModelMeshObjects[i].meshes.Count; j++)
                {
                    ObjExporter.MassModelData data = new ObjExporter.MassModelData();

                    data.Name = trickyInstanceObject.Name + " " + ModelID;
                    data.Model = new Mesh(ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.TriangleCount * 3, ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.TriangleCount);
                    data.Model.AllocVertices();
                    data.Model.AllocTexCoords();
                    data.Model.AllocIndices();
                    data.Model.AllocNormals();

                    var Vert = data.Model.VerticesAs<Vector3>();
                    var oldVert = ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.VerticesAs<Vector3>();
                    for (int k = 0; k < Vert.Length; k++)
                    {
                        Vert[k] = Raymath.Vector3Transform(oldVert[k], (trickyInstanceObject.worldMatrix4x4 * ssx3ModelMeshObjects[i].localMatrix4X4));
                    }

                    var Tex = data.Model.TexCoordsAs<Vector2>();
                    var oldTex = ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.TexCoordsAs<Vector2>();
                    for (int k = 0; k < Tex.Length; k++)
                    {
                        Tex[k] = new Vector2(oldTex[k].X, -oldTex[k].Y);
                    }

                    var indices = data.Model.IndicesAs<ushort>();
                    var oldindices = ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.IndicesAs<ushort>();
                    for (int k = 0; k < indices.Length; k++)
                    {
                        indices[k] = oldindices[k];
                    }

                    var Normal = data.Model.NormalsAs<Vector3>();
                    var oldNormal = ssx3ModelMeshObjects[i].meshes[j].meshRef.Mesh.NormalsAs<Vector3>();
                    for (int k = 0; k < Normal.Length; k++)
                    {
                        Normal[k] = oldNormal[k];
                    }

                    data.TextureName = SSX3DataManager.levelDatas[ssx3ModelMeshObjects[i].meshes[j].MaterialIndex.TrackID].ssx3MaterialObjects[ssx3ModelMeshObjects[i].meshes[j].MaterialIndex.RID].TexturePath;

                    ModelID++;

                    massModelDatas.Add(data);
                }

            }

            return massModelDatas;
        }
    }
}