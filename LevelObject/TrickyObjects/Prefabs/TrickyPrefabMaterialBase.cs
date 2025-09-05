using IceSaw2.LevelObject;
using SSXMultiTool.JsonFiles.Tricky; 
using System.Reflection;
using Raylib_cs;

public class TrickyPrefabMaterialBase : BaseObject
{
    public string MeshPath;
    public Mesh mesh;

    public int MaterialIndex;
    public Material material;
}
