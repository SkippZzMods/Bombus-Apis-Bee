using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BombusApisBee.Buffs
{
    public class Frostbroken : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostbroken");
            Description.SetDefault("Brr!.. Part two!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)  
        {
            npc.GetGlobalNPC<FrostbrokenGlobalNPC>().inflicted = true;

            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool())
                {
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0f, 0f, 0, new Color(122, 180, 222), 0.4f);
                }
                else
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.FrostedDust>(), 0f, 0f, 0, default, 1.25f).velocity *= 0.5f;
            }
        }
    }

    class FrostbrokenGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (inflicted)
                damage = (int)Main.CalculateDamageNPCsTake((int)(damage * 1.05f), npc.defense - 25);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (inflicted)
                damage = (int)Main.CalculateDamageNPCsTake((int)(damage * 1.05f), npc.defense - 25);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!inflicted)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= 20;

            if (damage < 2)
                damage = 2;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (inflicted)
                drawColor = new Color(122, 180, 222);
        }
    }
}
