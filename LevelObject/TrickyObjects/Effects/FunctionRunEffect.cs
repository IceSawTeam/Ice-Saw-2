using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class FunctionRunEffect : EffectBase
{
    public override int EffectType
    {
        get { return 21; }
    }

    public TrickyFunctionHeader FunctionObject;
    int FunctionIndex;

    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        FunctionIndex = effect.FunctionRunIndex.Value;
    }

    //public override SSFJsonHandler.Effect SaveEffect()
    //{
    //    var NewEffect = new SSFJsonHandler.Effect();

    //    NewEffect.MainType = EffectType;

    //    if(FunctionObject!=null)
    //    {
    //        NewEffect.FunctionRunIndex = TrickyLevelManager.Instance.dataManager.GetFunctionID(FunctionObject);
    //    }
    //    else
    //    {
    //        NewEffect.FunctionRunIndex = -1;
    //    }

    //    return NewEffect;
    //}
}
