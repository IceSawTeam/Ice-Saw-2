using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class TrickyFunctionHeader : TrickyEffectHeaderBase
{
    public override ObjectType Type
    {
        get { return ObjectType.Function; }
    }

    public void LoadFunction(SSFJsonHandler.Function EffectHeader)
    {
        Name = EffectHeader.FunctionName;
        for (int i = 0; i < EffectHeader.Effects.Count; i++)
        {
            LoadEffectData(EffectHeader.Effects[i]);
        }
    }

    public SSFJsonHandler.Function GenerateFunction()
    {
        var NewHeader = new SSFJsonHandler.Function();

        NewHeader.FunctionName = Name;
        NewHeader.Effects = new List<SSFJsonHandler.Effect>();

        for (int a = 0; a < effectBases.Count; a++)
        {
            NewHeader.Effects.Add(effectBases[a].SaveEffect());
        }

        return NewHeader;
    }
}
