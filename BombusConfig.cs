using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BombusApisBee
{
    public class BombusConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(-100f, 100f)]
        [Increment(2f)]
        [DrawTicks]
        [DefaultValue(30f)]
        public float yOffset;

        [DefaultValue(false)]
        public bool DrawLegacyUI;

        [DefaultValue(false)]
        public bool DragUI;

        [DefaultValue(false)]
        public int ResourceOffX;

        [DefaultValue(false)]
        public int ResourceOffY;
    }
}
