using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Buffs
{
    public class NectarGlazed : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectarglazed");
            Description.SetDefault("Yum!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool())
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 velo = Vector2.UnitY * Main.rand.NextFloat(3f);
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), velo.X, velo.Y, 0, new Color(214, 158, 79, 50), 0.35f);
                }
                else
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.HoneyDust>(), 0f, 0f, Main.rand.Next(70, 125), default).velocity *= 0.25f;
            }
        }
    }
}