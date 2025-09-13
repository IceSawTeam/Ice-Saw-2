using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class TeleportEffect : EffectBase
{
    public override int EffectType
    {
        get { return 24; }
    }
    
    //public TrickyInstanceObject InstanceObject;
    int TeleportInstanceIndex;

    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        TeleportInstanceIndex = effect.TeleportInstanceIndex.Value;
    }

    //public override SSFJsonHandler.Effect SaveEffect()
    //{
    //    var NewEffect = new SSFJsonHandler.Effect();

    //    NewEffect.MainType = EffectType;

    //    if (InstanceObject != null)
    //    {
    //        NewEffect.TeleportInstanceIndex = TrickyLevelManager.Instance.dataManager.GetInstanceID(InstanceObject);
    //    }
    //    else
    //    {
    //        NewEffect.TeleportInstanceIndex = -1;
    //    }

    //    return NewEffect;
    //}
}
