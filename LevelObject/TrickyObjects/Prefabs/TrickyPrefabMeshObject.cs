using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using SSXMultiTool.JsonFiles.Tricky;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMeshObject : TrickyPrefabMeshBase
    {

        public List<TrickyPrefabMaterialObject> trickyPrefabMaterialObjects = new List<TrickyPrefabMaterialObject>();
        public void LoadPrefabMeshObject(PrefabJsonHandler.ObjectHeader objectHeader)
        {
            ParentID = objectHeader.ParentID;
            Flags = objectHeader.Flags;

            IncludeAnimation = objectHeader.IncludeAnimation;
            IncludeMatrix = objectHeader.IncludeMatrix;
            Animation = new ObjectAnimation();

            if (IncludeAnimation)
            {
                Animation.U1 = objectHeader.Animation.Value.U1;
                Animation.U2 = objectHeader.Animation.Value.U2;
                Animation.U3 = objectHeader.Animation.Value.U3;
                Animation.U4 = objectHeader.Animation.Value.U4;
                Animation.U5 = objectHeader.Animation.Value.U5;
                Animation.U6 = objectHeader.Animation.Value.U6;
                Animation.AnimationAction = objectHeader.Animation.Value.AnimationAction;
                Animation.AnimationEntries = new List<AnimationEntry>();
                for (int a = 0; a < objectHeader.Animation.Value.AnimationEntries.Count; a++)
                {
                    var TempEntry = new AnimationEntry();
                    TempEntry.AnimationMaths = new List<AnimationMath>();

                    for (int b = 0; b < objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths.Count; b++)
                    {
                        var TempMaths = new AnimationMath();
                        TempMaths.Value1 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value1;
                        TempMaths.Value2 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value2;
                        TempMaths.Value3 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value3;
                        TempMaths.Value4 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value4;
                        TempMaths.Value5 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value5;
                        TempMaths.Value6 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value6;
                        TempEntry.AnimationMaths.Add(TempMaths);
                    }
                    Animation.AnimationEntries.Add(TempEntry);
                }
            }


            if (IncludeMatrix)
            {
                Position = JsonUtil.ArrayToVector3(objectHeader.Position);
                Scale = JsonUtil.ArrayToVector3(objectHeader.Scale);
                Rotation = JsonUtil.ArrayToQuaternion(objectHeader.Rotation);
            }

            trickyPrefabMaterialObjects = new List<TrickyPrefabMaterialObject>();

            //Load MeshHeaders
            for (int i = 0; i < objectHeader.MeshData.Count; i++)
            {
                var TrickyPrefabMaterialObject = new TrickyPrefabMaterialObject();

                TrickyPrefabMaterialObject.LoadPrefabMaterialObject(objectHeader.MeshData[i]);

                trickyPrefabMaterialObjects.Add(TrickyPrefabMaterialObject);

                //GameObject ChildMesh = new GameObject(i.ToString());

                //ChildMesh.transform.parent = transform;
                //ChildMesh.transform.localPosition = Vector3.zero;
                //ChildMesh.transform.localScale = Vector3.one;
                //ChildMesh.transform.localRotation = new Quaternion(0, 0, 0, 0);

                //ChildMesh.AddComponent<PrefabMeshObject>().LoadPrefabMeshObject(objectHeader.MeshData[i]);
            }
        }

        public override void Render()
        {
            for (int i = 0; i < trickyPrefabMaterialObjects.Count; i++)
            {
                trickyPrefabMaterialObjects[i].Render();
            }
        }
    }
}
