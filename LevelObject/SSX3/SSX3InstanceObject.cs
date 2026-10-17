using IceSaw2.LevelObject;
using IceSaw2.LevelObject.SSX3;
using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using SSXLibrary.JsonFiles.SSX3;
using SSXMultiTool.Utilities;
using System.Collections.Generic;
using System.Numerics;

public class SSX3InstanceObject : BaseObject
{
    public int TrackID;
    public int RID;

    public int U0;
    public int U1;
    public int U2;
    public int U3;

    public Vector4 V0;
    public Vector3 V1;
    public Vector3 V2;

    public int U4;

    public int ModelTrackID;
    public int ModelRID;

    public int U6;
    public int U7;

    public int U8;
    public int U9;
    public int U10;
    public int U11;
    public int U12;
    public SSX3ModelObject PrefabObject;

    public void LoadBin3(InstanceJsonHandler.Instance bin3)
    {
        Name = bin3.Name;

        Position = JsonUtil.ArrayToVector3(bin3.Position);
        Rotation = JsonUtil.ArrayToQuaternion(bin3.Rotation);
        Scale = JsonUtil.ArrayToVector3(bin3.Scale);

        U0 = bin3.U0;
        U1 = bin3.U1;
        U2 = bin3.U2;
        U3 = bin3.U3;

        V0 = JsonUtil.ArrayToVector4(bin3.V0);
        V1 = JsonUtil.ArrayToVector3(bin3.V1);
        V2 = JsonUtil.ArrayToVector3(bin3.V2);

        TrackID = bin3.TrackID;
        RID = bin3.RID;
        U4 = bin3.U4;

        ModelTrackID = bin3.ModelTrackID;
        ModelRID = bin3.ModelRID;

        U6 = bin3.U6;
        U7 = bin3.U7;

        U8 = bin3.U8;
        U9 = bin3.U9;
        U10 = bin3.U10;
        U11 = bin3.U11;
        U12 = bin3.U12;

        LoadPrefabs();
    }

    public void LoadPrefabs()
    {
        if (ModelRID != -1)
        {
            PrefabObject = SSX3DataManager.ReturnModel(ModelRID, ModelTrackID);
            AddToModelRender();
        }
    }

    #region Rendering

    public void AddToModelRender()
    {
        if (PrefabObject != null)
        {
            PrefabObject.AddToRenderCache(this);
        }
    }

    public void RemoveFromModelRender()
    {
        if (PrefabObject != null)
        {
            PrefabObject.RemoveFromRenderCache(this);
        }
    }

    // Called after Position/Rotation/Scale change on an already-placed instance (e.g. by the
    // viewport gizmo) so the batched render cache's snapshot of this instance's transform
    // doesn't go stale - see TrickyModelMeshObject.UpdateRenderCacheTransform.
    public void RefreshRenderCache()
    {
        PrefabObject?.UpdateRenderCacheTransform(this);
    }

    public List<ObjExporter.MassModelData> GenerateModel()
    {
        if (PrefabObject != null)
        {
            return PrefabObject.GenerateModel(this);
        }
        else
        {
            return new List<ObjExporter.MassModelData>();
        }
    }

    //List<RenderCache> renderCaches = new List<RenderCache>();

    //public void GenerateRenderCache()
    //{
    //    renderCaches = new List<RenderCache>();
    //    if (TrickyPrefab != null)
    //    {
    //        TrickyPrefab.parent = this;
    //        renderCaches = TrickyPrefab.GenerateRenderCache();
    //        TrickyPrefab.parent = null;
    //    }
    //}

    //public override void Render()
    //{
    //    for (int i = 0; i < renderCaches.Count; i++)
    //    {
    //        renderCaches[i].baseObject.Render(renderCaches[i].WorldMatrix);
    //    }
    //}
    #endregion
}