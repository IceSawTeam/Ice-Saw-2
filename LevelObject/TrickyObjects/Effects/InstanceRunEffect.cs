using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using static SSXMultiTool.JsonFiles.Tricky.SSFJsonHandler;

public class InstanceRunEffect : EffectBase
{
    public override int EffectType
    {
        get { return 7; }
    }

    //public TrickyInstanceObject InstanceObject;
    //public TrickyEffectHeader EffectHeaderObject;

    int InstanceIndex;
    int EffectIndex;

    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        InstanceIndex = effect.Instance.Value.InstanceIndex;
        EffectIndex = effect.Instance.Value.EffectIndex;
    }

    public override SSFJsonHandler.Effect SaveEffect()
    {
        var NewEffect = new SSFJsonHandler.Effect();

        NewEffect.MainType = EffectType;

        var NewInstanceEffect = new SSFJsonHandler.InstanceEffect();

        NewInstanceEffect.InstanceIndex = InstanceIndex;
        NewInstanceEffect.EffectIndex = EffectIndex;

        //if (InstanceObject != null)
        //{
        //    NewInstanceEffect.InstanceIndex = TrickyLevelManager.Instance.dataManager.GetInstanceID(InstanceObject);
        //}
        //else
        //{
        //    NewInstanceEffect.InstanceIndex = -1;
        //}

        //if (EffectHeaderObject != null)
        //{
        //    NewInstanceEffect.EffectIndex = TrickyLevelManager.Instance.dataManager.GetEffectID(EffectHeaderObject);
        //}
        //else
        //{
        //    NewInstanceEffect.EffectIndex = -1;
        //}

        NewEffect.Instance = NewInstanceEffect;

        return NewEffect;
    }
}
