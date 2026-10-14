using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Inspectors.Tricky
{
    public class InstanceInspector
    {
        public static void Draw(IReadOnlyList<TrickyInstanceObject> instances)
        {
            InspectorManager.DrawStandardDetails<BaseObject>(instances);

            InspectorManager.Int("Model ID", instances, p => p.ModelID, (p, v) => p.ModelID = v);

            InspectorManager.Int("Prev Instance", instances, p => p.PrevInstance, (p, v) => p.PrevInstance = v);
            InspectorManager.Int("Next Instance", instances, p => p.NextInstance, (p, v) => p.NextInstance = v);

            InspectorManager.Int("UnknownInt26", instances, p => p.UnknownInt26, (p, v) => p.UnknownInt26 = v);
            InspectorManager.Int("UnknownInt27", instances, p => p.UnknownInt27, (p, v) => p.UnknownInt27 = v);
            InspectorManager.Int("UnknownInt28", instances, p => p.UnknownInt28, (p, v) => p.UnknownInt28 = v);
            InspectorManager.Int("UnknownInt30", instances, p => p.UnknownInt30, (p, v) => p.UnknownInt30 = v);
            InspectorManager.Int("UnknownInt31", instances, p => p.UnknownInt31, (p, v) => p.UnknownInt31 = v);
            InspectorManager.Int("UnknownInt32", instances, p => p.UnknownInt32, (p, v) => p.UnknownInt32 = v);

            InspectorManager.Int("LTGState", instances, p => p.LTGState, (p, v) => p.LTGState = v);

            InspectorManager.Int("Hash", instances, p => p.Hash, (p, v) => p.Hash = v);

            InspectorManager.Bool("IncludeSound", instances, p => p.IncludeSound, (p, v) => p.IncludeSound = v);
            //Sound Stuff

            InspectorManager.Float("U0", instances, p => p.U0, (p, v) => p.U0 = v);
            InspectorManager.Float("PlayerBounceAmmount", instances, p => p.PlayerBounceAmmount, (p, v) => p.PlayerBounceAmmount = v);
            InspectorManager.Int("U2", instances, p => p.U2, (p, v) => p.U2 = v);

            InspectorManager.Bool("InstanceVisable", instances, p => p.InstanceVisable, (p, v) => p.InstanceVisable = v);
            InspectorManager.Bool("PlayerCollision", instances, p => p.PlayerCollision, (p, v) => p.PlayerCollision = v);
            InspectorManager.Bool("PlayerBounce", instances, p => p.PlayerBounce, (p, v) => p.PlayerBounce = v);
            InspectorManager.Bool("Unknown241", instances, p => p.Unknown241, (p, v) => p.Unknown241 = v);
            InspectorManager.Bool("UVScroll", instances, p => p.UVScroll, (p, v) => p.UVScroll = v);

            //SurfaceType

            InspectorManager.Int("CollsionMode", instances, p => p.CollsionMode, (p, v) => p.CollsionMode = v);
            //Collision Paths

            InspectorManager.Int("EffectSlotIndex", instances, p => p.EffectSlotIndex, (p, v) => p.EffectSlotIndex = v);
            InspectorManager.Int("PhysicsIndex", instances, p => p.PhysicsIndex, (p, v) => p.PhysicsIndex = v);
            InspectorManager.Int("U8", instances, p => p.U8, (p, v) => p.U8 = v);

            InspectorManager.Vector3("LightVector1", instances, p => p.LightVector1, (p, v) => p.LightVector1 = v);
            InspectorManager.Vector3("LightVector2", instances, p => p.LightVector2, (p, v) => p.LightVector2 = v);
            InspectorManager.Vector3("LightVector3", instances, p => p.LightVector3, (p, v) => p.LightVector3 = v);
            InspectorManager.Vector3("AmbentLightVector", instances, p => p.AmbentLightVector, (p, v) => p.AmbentLightVector = v);
            InspectorManager.Vector3("LightColour1", instances, p => p.LightColour1, (p, v) => p.LightColour1 = v);
            InspectorManager.Vector3("LightColour2", instances, p => p.LightColour2, (p, v) => p.LightColour2 = v);
            InspectorManager.Vector3("LightColour3", instances, p => p.LightColour3, (p, v) => p.LightColour3 = v);
            InspectorManager.Vector4("AmbentLightColour", instances, p => p.AmbentLightColour, (p, v) => p.AmbentLightColour = v);

        }

    }
}
