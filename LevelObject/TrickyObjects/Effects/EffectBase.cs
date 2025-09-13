using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class EffectBase
{
    public virtual int EffectType
    {
        get { return -1; }
    }
    public virtual int SubEffectType
    {
        get { return -1; }
    }

    public virtual void LoadEffect(SSFJsonHandler.Effect effect)
    {

    }

    public virtual SSFJsonHandler.Effect SaveEffect()
    {
        return new SSFJsonHandler.Effect();
    }
}
