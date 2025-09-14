using IceSaw2.LevelObject;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;

public class TrickyEffectHeaderBase : BaseObject
{
    public List<EffectBase> effectBases = new List<EffectBase>();

    public void LoadEffectData(SSFJsonHandler.Effect TempEffect)
    {
        effectBases = new List<EffectBase>();

        if (TempEffect.MainType == 0)
        {
            if (TempEffect.type0.Value.SubType == 0)
            {
                var NewEffect = new Type0Sub0Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 2)
            {
                var NewEffect = new DebounceEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 5)
            {
                var NewEffect = new DeadNodeEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 6)
            {
                var NewEffect = new CounterEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 7)
            {
                var NewEffect = new BoostEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 10)
            {
                var NewEffect = new UVScrollingEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 11)
            {
                var NewEffect = new TextureFlipEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 12)
            {
                var NewEffect = new FenceFlexEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 13)
            {
                var NewEffect = new Type0Sub13Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 14)
            {
                var NewEffect = new Type0Sub14Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 15)
            {
                var NewEffect = new Type0Sub15Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 17)
            {
                var NewEffect = new CrowdBoxEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 18)
            {
                var NewEffect = new Type0Sub18Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 20)
            {
                var NewEffect = new Type0Sub20Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 23)
            {
                var NewEffect = new MovieScreenEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 24)
            {
                var NewEffect = new Type0Sub24Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 256)
            {
                var NewEffect = new Type0Sub256Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 257)
            {
                var NewEffect = new Type0Sub257Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type0.Value.SubType == 258)
            {
                var NewEffect = new Type0Sub258Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else
            {
                var NewEffect = new EffectBase();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
        }
        else if (TempEffect.MainType == 2)
        {
            if (TempEffect.type2.Value.SubType == 0)
            {
                var NewEffect = new Type2Sub0Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type2.Value.SubType == 1)
            {
                var NewEffect = new SplineAnimationEffect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else if (TempEffect.type2.Value.SubType == 2)
            {
                var NewEffect = new Type2Sub2Effect();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
            else
            {
                var NewEffect = new EffectBase();

                NewEffect.LoadEffect(TempEffect);

                effectBases.Add(NewEffect);
            }
        }
        else if (TempEffect.MainType == 3)
        {
            var NewEffect = new Type3Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 4)
        {
            var NewEffect = new WaitEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 5)
        {
            var NewEffect = new Type5Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 7)
        {
            var NewEffect = new InstanceRunEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 8)
        {
            var NewEffect = new SoundEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 9)
        {
            var NewEffect = new Type9Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 13)
        {
            var NewEffect = new Type13Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 14)
        {
            var NewEffect = new MultiplierEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 17)
        {
            var NewEffect = new Type17Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 18)
        {
            var NewEffect = new Type18Effect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 21)
        {
            var NewEffect = new FunctionRunEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 24)
        {
            var NewEffect = new TeleportEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else if (TempEffect.MainType == 25)
        {
            var NewEffect = new SplineRunEffect();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
        else
        {
            var NewEffect = new EffectBase();

            NewEffect.LoadEffect(TempEffect);

            effectBases.Add(NewEffect);
        }
    }
}
