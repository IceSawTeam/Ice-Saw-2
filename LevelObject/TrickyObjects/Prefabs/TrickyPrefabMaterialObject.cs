using System.Collections;
using System.Collections.Generic;
using SSXMultiTool.JsonFiles.Tricky;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMaterialObject : TrickyPrefabMaterialBase
    {
        public override ObjectType Type
        {
            get { return ObjectType.PrefabMesh; }
        }

        public void LoadPrefabMeshObject(PrefabJsonHandler.MeshHeader objectHeader)
        {
            MeshPath = objectHeader.MeshPath;
            MaterialIndex = objectHeader.MaterialID;
        }
    }
}
