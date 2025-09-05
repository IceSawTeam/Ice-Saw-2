using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;


namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabObject : TrickyPrefabBase
    {
        public override ObjectType Type
        {
            get { return ObjectType.Prefab; }
        }

        public List<TrickyPrefabMeshObject> trickyPrefabSubObjects = new List<TrickyPrefabMeshObject>();

        public void LoadPrefab(PrefabJsonHandler.PrefabJson prefabJson, bool Skybox = false)
        {
            Name = prefabJson.PrefabName;
            Unknown3 = prefabJson.Unknown3;
            AnimTime = prefabJson.AnimTime;

            trickyPrefabSubObjects = new List<TrickyPrefabMeshObject>();

            for (int i = 0; i < prefabJson.PrefabObjects.Count; i++)
            {
                var TrickyPrefabMeshObject = new TrickyPrefabMeshObject();

                TrickyPrefabMeshObject.LoadPrefabMeshObject(prefabJson.PrefabObjects[i]);

                trickyPrefabSubObjects.Add(TrickyPrefabMeshObject);
            }
        }
    }
}
