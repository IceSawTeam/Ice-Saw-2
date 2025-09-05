using SSXMultiTool.JsonFiles.Tricky;
using System;
using System.Collections.Generic;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabBase : BaseObject
    {
        public int Unknown3;
        public float AnimTime;



        //public void ForceReloadMeshMat()
        //{
        //    var TempHeader = GetComponentsInChildren<TrickyPrefabSubObject>();

        //    for (int i = 0; i < TempHeader.Length; i++)
        //    {
        //        TempHeader[i].ForceRegenMeshMat();
        //    }
        //}

        //public string[] GetTextureNames()
        //{
        //    List<string> TextureNames = new List<string>();
        //    var TempList = GetComponentsInChildren<TrickyPrefabSubObject>();

        //    for (int i = 0; i < TempList.Length; i++)
        //    {
        //        var TempModel = TempList[i].GetComponentsInChildren<PrefabMeshObject>();
        //        for (int a = 0; a < TempModel.Length; a++)
        //        {
        //            TextureNames.Add(TempModel[a].TrickyMaterialObject.TexturePath);
        //        }
        //    }
        //    return TextureNames.ToArray();
        //}
    }
}
