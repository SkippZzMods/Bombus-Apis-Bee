using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BombusApisBee
{
    public class BombusConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Honey Player UI Y-Offset")]
        [Range(-100f, 100f)]
        [Increment(2f)]
        [DrawTicks]
        [DefaultValue(30f)]
        public float yOffset;

        [Label("Draw Legacy Honey Resource UI (Outdated)")]
        [DefaultValue(false)]
        public bool DrawLegacyUI;

        [Label("Drag Honey Resource UI")]
        [DefaultValue(false)]
        public bool DragUI;

        [Label("Used for the dragging properties of the Resource UI. Should not be changed manually, reset this and the Y Offset to 0 to use default posiiton.")]
        [DefaultValue(false)]
        public int ResourceOffX;

        [Label("Used for the dragging properties of the Resource UI. Should not be changed manually, reset this and the X Offset 0 to use default posiiton.")]
        [DefaultValue(false)]
        public int ResourceOffY;
    }
}
