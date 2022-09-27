using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BombusApisBee
{
    public class BombusConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Label("Disable Honey UI")]
        [Tooltip("Prevents drawing the small Honey UI")]
        public bool DisableHoneyUI { get; set; }
        [Label("Honey UI Y-Offset")]
        [Range(-100f, 100f)]
        [Increment(2f)]
        [DrawTicks]
        [DefaultValue(25f)]
        public float YOffset;
        [Label("Honey UI X-Offset")]
        [Range(-300f, 300f)]
        [Increment(5f)]
        [DrawTicks]
        [DefaultValue(0f)]
        public float XOffset;
    }
}
