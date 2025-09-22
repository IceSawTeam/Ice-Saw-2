using IceSaw2.LevelObject;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

public class TrickyEffectSlotObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.EffectSlot; }
    }

    //public TrickyEffectHeader PersistantEffectSlot;
    //public TrickyEffectHeader CollisionEffectSlot;
    //public TrickyEffectHeader Slot3;
    //public TrickyEffectHeader Slot4;
    //public TrickyEffectHeader EffectTriggerSlot;
    //public TrickyEffectHeader Slot6;
    //public TrickyEffectHeader Slot7;

    int PersistantEffectSlotIndex;
    int CollisionEffectSlotIndex;
    int Slot3Index;
    int Slot4Index;
    int EffectTriggerSlotIndex;
    int Slot6Index;
    int Slot7Index;

    public void LoadEffectSlot(SSFJsonHandler.EffectSlotJson effectSlot)
    {
        Name = effectSlot.EffectSlotName;

        PersistantEffectSlotIndex = effectSlot.PersistantEffectSlot;
        CollisionEffectSlotIndex = effectSlot.CollisionEffectSlot;
        Slot3Index = effectSlot.Slot3;
        Slot4Index = effectSlot.Slot4;
        EffectTriggerSlotIndex = effectSlot.EffectTriggerSlot;
        Slot6Index = effectSlot.Slot6;
        Slot7Index = effectSlot.Slot7;
    }

    public SSFJsonHandler.EffectSlotJson SaveEffectSlot()
    {
        var TempEffectslot = new SSFJsonHandler.EffectSlotJson();

        TempEffectslot.EffectSlotName = Name;

        //if (PersistantEffectSlot != null)
        //{
        //    TempEffectslot.PersistantEffectSlot = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(PersistantEffectSlot);
        //}
        //else
        //{
        //    TempEffectslot.PersistantEffectSlot = -1;
        //}

        //if (CollisionEffectSlot != null)
        //{
        //    TempEffectslot.CollisionEffectSlot = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(CollisionEffectSlot);
        //}
        //else
        //{
        //    TempEffectslot.CollisionEffectSlot = -1;
        //}

        //if (Slot3 != null)
        //{
        //    TempEffectslot.Slot3 = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(Slot3);
        //}
        //else
        //{
        //    TempEffectslot.Slot3 = -1;
        //}

        //if (Slot4 != null)
        //{
        //    TempEffectslot.Slot4 = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(Slot4);
        //}
        //else
        //{
        //    TempEffectslot.Slot4 = -1;
        //}

        //if (EffectTriggerSlot != null)
        //{
        //    TempEffectslot.EffectTriggerSlot = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(EffectTriggerSlot);
        //}
        //else
        //{
        //    TempEffectslot.EffectTriggerSlot = -1;
        //}

        //if (Slot6 != null)
        //{
        //    TempEffectslot.Slot6 = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(Slot6);
        //}
        //else
        //{
        //    TempEffectslot.Slot6 = -1;
        //}

        //if (Slot7 != null)
        //{
        //    TempEffectslot.Slot7 = TrickyLevelManager.Instance.dataManager.GetEffectHeaderID(Slot7);
        //}
        //else
        //{
        //    TempEffectslot.Slot7 = -1;
        //}

        return TempEffectslot;
    }
}
