using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Buffs
{
    public class BladeOfAculeusCleave : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cleaved");
            Description.SetDefault("You're defense is worthless");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CleaveGlobalNPC>().inflicted = true;
        }
    }

    class CleaveGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
            {
                modifiers.Defense.Base *= 0.75f;
                modifiers.FinalDamage *= 1.1f;
            }
                
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
            {
                modifiers.Defense.Base *= 0.75f;
                modifiers.FinalDamage *= 1.1f;
            }          
        }
    }
}
