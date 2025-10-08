using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Numerics;
using IceSaw2.Manager.Tricky;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyModelMaterialObject : MeshBaseObject
    {
        bool Skybox;

        public override ObjectType Type
        {
            get { return ObjectType.PrefabMesh; }
        }

        public string MeshPath = "";
        public int MaterialIndex;
        public void LoadPrefabMaterialObject(ModelJsonHandler.MeshHeader objectHeader, bool skybox)
        {
            Skybox = skybox;

            MeshPath = objectHeader.MeshPath;
            MaterialIndex = objectHeader.MaterialID;

            GenerateModel();
        }

        public void GenerateModel()
        {
            //Have it pull mesh and material from trickyMaterial and mesh instead


            mesh = TrickyDataManager.ReturnMesh(MeshPath, Skybox);

            var TexturePath = ""; 
            
            if(!Skybox)
            {
                TexturePath = TrickyDataManager.trickyMaterialObject[MaterialIndex].TexturePath;
            }
            else
            {
                TexturePath = TrickyDataManager.trickySkyboxMaterialObject[MaterialIndex].TexturePath;
            }

            Texture2D ReturnTexture = TrickyDataManager.ReturnTexture(TexturePath, Skybox);

            material = Raylib.LoadMaterialDefault();

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, ReturnTexture);
        }

        public RenderCache GenerateRenderCache()
        {
            RenderCache cache = new RenderCache();

            cache.baseObject = this;
            cache.WorldMatrix = worldMatrix4x4;

            return cache;
        }
    }
}
