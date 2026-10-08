//using Unity.VisualScripting;
//using UnityEngine;
//using static SSXMultiTool.JsonFiles.SSX3.MDRJsonHandler;

//public class SSX3ModelMaterialObject : MonoBehaviour
//{
//    public string ModelPath;

//    public int MaterialID;
//    public int U1;
//    public int U4;

//    //Main Header Object
//    public int U00;
//    public int U01;
//    public int U02;
//    public int U03;

//    public float[] U04;

//    [HideInInspector]
//    public Mesh mesh;

//    [HideInInspector]
//    public Material material;

//    [HideInInspector]
//    public MeshFilter meshFilter;
//    [HideInInspector]
//    public MeshRenderer meshRenderer;

//    [ContextMenu("Add Missing Components")]
//    public void AddMissingComponents()
//    {
//        if (meshFilter != null)
//        {
//            DestroyImmediate(meshFilter);
//        }
//        if (meshRenderer != null)
//        {
//            DestroyImmediate(meshRenderer);
//        }

//        meshFilter = transform.AddComponent<MeshFilter>();
//        meshRenderer = transform.AddComponent<MeshRenderer>();

//        meshFilter.hideFlags = HideFlags.HideInInspector;
//        meshRenderer.hideFlags = HideFlags.HideInInspector;
//    }

//    public void LoadMaterialObject(ModelDataHeaderStruct modelDataHeaderStruct)
//    {
//        AddMissingComponents();

//        ModelPath = modelDataHeaderStruct.ModelPath;

//        MaterialID = modelDataHeaderStruct.MaterialID;

//        U1 = modelDataHeaderStruct.U1;
//        U4 = modelDataHeaderStruct.U4;

//        U00 = modelDataHeaderStruct.U00;
//        U01 = modelDataHeaderStruct.U01;
//        U02 = modelDataHeaderStruct.U02;
//        U03 = modelDataHeaderStruct.U03;

//        U04 = modelDataHeaderStruct.U04;

//        LoadMesh();
//    }

//    public void LoadMesh()
//    {
//        if (ModelPath != "")
//        {
//            var ssx3LevelManager = SSX3LevelManager.GetLevelManager(this.gameObject);

//            mesh = ssx3LevelManager.GetMesh(ModelPath);
//        }

//        try
//        {
//            material = GenerateMaterial(this.transform.parent.parent.GetComponent<SSX3ModelObject>().MaterialList[MaterialID].RID, this.gameObject);
//        }
//        catch
//        {

//        }

//        //material = new Material(Shader.Find("Custom/DoubleSided"));

//        //material.color = Color.gray; // Set the material color to gray
//        //material.SetFloat("_Smoothness", 0.5f); // Adjust smoothness

//        meshFilter.sharedMesh = mesh;
//        meshRenderer.sharedMaterial = material;
//    }

//    public static Material GenerateMaterial(int MaterialID, GameObject gameObject)
//    {
//        var NewMaterial = new Material(Shader.Find("ModelShader"));

//        var SSX3Material = SSX3LevelManager.GetLevelManager(gameObject).GetMaterial(MaterialID);
//        var Texture = Bin0Object.GetTexture(SSX3Material.TextureName);

//        NewMaterial.SetTexture("_MainTexture", Texture);
//        NewMaterial.SetFloat("_NoLightMode", 1);

//        return NewMaterial;
//    }
//}
