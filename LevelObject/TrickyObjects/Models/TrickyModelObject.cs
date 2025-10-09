using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
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

        public List<RenderCache> GenerateRenderCache()
        {
            List<RenderCache> cache = new List<RenderCache>();

            for (int i = 0; i < Children.Count; i++)
            {
                cache.AddRange(((TrickyModelMeshObject)Children[i]).GenerateRenderCache());
            }

            return cache;
        }
    }
}
