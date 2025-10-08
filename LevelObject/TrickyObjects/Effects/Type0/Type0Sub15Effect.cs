using SSXMultiTool.JsonFiles.Tricky;


public class Type0Sub15Effect : EffectBase
{
    public override int EffectType
    {
        get { return 0; }
    }

    public override int SubEffectType
    {
        get { return 15; }
    }

    public float Unknown1;
    public float Unknown2;
    public int Unknown3;
    public int Unknown4;
    public float Unknown5;

    public override void LoadEffect(SSFJsonHandler.Effect effect)
    {
        if (!effect.type0.HasValue) return;
        if (!effect.type0.Value.type0Sub15.HasValue) return;

        Unknown1 = effect.type0.Value.type0Sub15.Value.U0;
        Unknown2 = effect.type0.Value.type0Sub15.Value.U1;
        Unknown3 = effect.type0.Value.type0Sub15.Value.U2;
        Unknown4 = effect.type0.Value.type0Sub15.Value.U3;
        Unknown5 = effect.type0.Value.type0Sub15.Value.U4;
    }

    public override SSFJsonHandler.Effect SaveEffect()
    {
        var NewEffect = new SSFJsonHandler.Effect
        {
            MainType = EffectType
        };

        var NewType0Effect = new SSFJsonHandler.Type0
        {
            SubType = SubEffectType
        };

        var NewType0Sub0Effect = new SSFJsonHandler.Type0Sub15
        {
            U0 = Unknown1,
            U1 = Unknown2,
            U2 = Unknown3,
            U3 = Unknown4,
            U4 = Unknown5
        };

        NewType0Effect.type0Sub15 = NewType0Sub0Effect;

        NewEffect.type0 = NewType0Effect;

        return NewEffect;
    }
}
