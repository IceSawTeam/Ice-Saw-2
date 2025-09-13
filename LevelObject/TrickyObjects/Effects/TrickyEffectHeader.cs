using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class TrickyEffectHeader : TrickyEffectHeaderBase
{
    public override ObjectType Type
    {
        get { return ObjectType.Effect; }
    }

    public void LoadEffectList(SSFJsonHandler.EffectHeaderStruct EffectHeader)
    {
        Name = EffectHeader.EffectName;
        for (int i = 0; i < EffectHeader.Effects.Count; i++)
        {
            LoadEffectData(EffectHeader.Effects[i]);
        }
    }


    public SSFJsonHandler.EffectHeaderStruct GenerateEffectHeader()
    {
        var NewHeader = new SSFJsonHandler.EffectHeaderStruct();

        NewHeader.EffectName = Name;
        NewHeader.Effects = new List<SSFJsonHandler.Effect>();

        for (int a = 0; a < effectBases.Count; a++)
        {
            NewHeader.Effects.Add(effectBases[a].SaveEffect());
        }

        return NewHeader;
    }
}
