using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BombusApisBee
{
    public class BombusConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Honey UI Y-Offset")]
        [Range(-100f, 100f)]
        [Increment(2f)]
        [DrawTicks]
        [DefaultValue(30f)]
        public float yOffset;

    }
}
