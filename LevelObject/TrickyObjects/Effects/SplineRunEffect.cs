using IceSaw2.LevelObject.TrickyObjects;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class SplineRunEffect : EffectBase
{
    public override int EffectType
    {
        get { return 25; }
    }

    //public TrickySplineObject SplineObject;
    //public TrickyEffectHeader EffectHeader;

    int SplineIndex;
    int EffectIndex;

    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        SplineIndex = effect.Spline.Value.SplineIndex;
        EffectIndex = effect.Spline.Value.Effect;
    }

    public override SSFJsonHandler.Effect SaveEffect()
    {
        var NewEffect = new SSFJsonHandler.Effect();

        NewEffect.MainType = EffectType;

        var NewInstanceEffect = new SSFJsonHandler.SplineEffect();

        NewInstanceEffect.SplineIndex = SplineIndex;
        NewInstanceEffect.Effect = EffectIndex;
        //if (SplineObject != null)
        //{
        //    NewInstanceEffect.SplineIndex = TrickyLevelManager.Instance.dataManager.GetSplineID(SplineObject);
        //}
        //else
        //{
        //    NewInstanceEffect.SplineIndex = -1;
        //}

        //if (EffectHeader != null)
        //{
        //    NewInstanceEffect.Effect = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(EffectHeader);
        //}
        //else
        //{
        //    NewInstanceEffect.Effect = -1;
        //}


        NewEffect.Spline = NewInstanceEffect;

        return NewEffect;
    }
}
