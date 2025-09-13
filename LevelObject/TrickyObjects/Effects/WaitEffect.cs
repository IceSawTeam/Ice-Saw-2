using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class WaitEffect : EffectBase
{
    public override int EffectType
    {
        get { return 4; }
    }

    public float WaitTime;
    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        WaitTime = effect.WaitTime.Value;
    }

    public override SSFJsonHandler.Effect SaveEffect()
    {
        var NewEffect = new SSFJsonHandler.Effect();

        NewEffect.MainType = EffectType;
        NewEffect.WaitTime = WaitTime;

        return NewEffect;
    }

}
