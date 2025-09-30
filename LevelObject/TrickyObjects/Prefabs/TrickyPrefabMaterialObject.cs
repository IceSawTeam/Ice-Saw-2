using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Numerics;
using IceSaw2.Manager;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMaterialObject : MeshBaseObject
    {
        bool Skybox;

        public override ObjectType Type
        {
            get { return ObjectType.PrefabMesh; }
        }

        public string MeshPath;
        public int MaterialIndex;
        public void LoadPrefabMaterialObject(PrefabJsonHandler.MeshHeader objectHeader, bool skybox)
        {
            Skybox = skybox;

            MeshPath = objectHeader.MeshPath;
            MaterialIndex = objectHeader.MaterialID;

            GenerateModel();
        }

        public void GenerateModel()
        {
            //Have it pull mesh and material from trickyMaterial and mesh instead


            mesh = DataManager.ReturnMesh(MeshPath, Skybox);

            var TexturePath = ""; 
            
            if(!Skybox)
            {
                TexturePath = DataManager.trickyMaterialObject[MaterialIndex].TexturePath;
            }
            else
            {
                TexturePath = DataManager.trickySkyboxMaterialObject[MaterialIndex].TexturePath;
            }

            Texture2D ReturnTexture = DataManager.ReturnTexture(TexturePath, Skybox);

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
