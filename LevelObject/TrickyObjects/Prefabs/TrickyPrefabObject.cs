using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;


namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabObject : BaseObject
    {
        bool Skybox;

        public override ObjectType Type
        {
            get { return ObjectType.Prefab; }
        }

        public int Unknown3;
        public float AnimTime;

        public List<TrickyPrefabMeshObject> trickyPrefabSubObjects = new List<TrickyPrefabMeshObject>();

        public void LoadPrefab(PrefabJsonHandler.PrefabJson prefabJson, bool skybox = false)
        {
            Skybox = skybox;

            Name = prefabJson.PrefabName;
            Unknown3 = prefabJson.Unknown3;
            AnimTime = prefabJson.AnimTime;

            trickyPrefabSubObjects = new List<TrickyPrefabMeshObject>();

            for (int i = 0; i < prefabJson.PrefabObjects.Count; i++)
            {
                var TrickyPrefabMeshObject = new TrickyPrefabMeshObject();

                TrickyPrefabMeshObject.parent = this;

                TrickyPrefabMeshObject.LoadPrefabMeshObject(prefabJson.PrefabObjects[i], Skybox);

                trickyPrefabSubObjects.Add(TrickyPrefabMeshObject);
            }
        }

        public override void Render()
        {
            for (int i = 0; i < trickyPrefabSubObjects.Count; i++)
            {
                trickyPrefabSubObjects[i].Render();
            }
        }
    }
}
