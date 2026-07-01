using IceSaw2.Manager.Tricky;
using SSXLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Utilities
{
    public class OBJLevelExtract
    {
        public static void ExtractOBJ(string Folder)
        {
            var Patch = TrickyDataManager.trickyPatchObjects;
            var Instance = TrickyDataManager.trickyInstanceObjects;
            var Textures = TrickyDataManager.worldTextureData;

            //Generate MMD List
            List<ObjExporter.MassModelData> MMD = new List<ObjExporter.MassModelData>();

            for (int i = 0; i < Patch.Count; i++)
            {
                MMD.Add(Patch[i].GenerateModel());
            }

            for (int i = 0; i < Instance.Count; i++)
            {
                MMD.AddRange(Instance[i].GenerateModel());
            }

            //Save Objects 
            ObjExporter.SaveModelList(Folder, MMD, Textures);
        }

    }
}
