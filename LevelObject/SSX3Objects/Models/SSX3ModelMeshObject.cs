//using SSXMultiTool.JsonFiles.SSX3;
//using SSXMultiTool.Utilities;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using static SSXMultiTool.JsonFiles.SSX3.MDRJsonHandler;

//[ExecuteInEditMode]
//[SelectionBase]
//public class SSX3ModelMeshObject : MonoBehaviour
//{
//    public int ParentID;

//    public UnknownS2 unknownS2;
//    public UnknownS3 unknownS3;



//    public void LoadPrefab(MDRJsonHandler.ModelObject model)
//    {
//        ParentID = model.ParentID;

//        transform.localPosition = JsonUtil.ArrayToVector3(model.Position);
//        transform.localRotation = JsonUtil.ArrayToQuaternion(model.Rotation);
//        transform.localScale = JsonUtil.ArrayToVector3(model.Scale);

//        if(transform.localScale == Vector3.zero)
//        {
//            transform.localScale = Vector3.one;
//        }

//        unknownS2 = model.unknownS2;
//        unknownS3 = model.unknownS3;

//        for (int i = 0; i < model.unknownS2.ModelHeaderOffset.Count; i++)
//        {
//            GameObject ChildMesh = new GameObject(i.ToString());

//            ChildMesh.transform.parent = transform;
//            ChildMesh.transform.localPosition = Vector3.zero;
//            ChildMesh.transform.localScale = Vector3.one;
//            ChildMesh.transform.localRotation = new Quaternion(0, 0, 0, 0);

//            ChildMesh.AddComponent<SSX3ModelMaterialObject>().LoadMaterialObject(model.unknownS2.ModelHeaderOffset[i]);

//        }
//    }
//}