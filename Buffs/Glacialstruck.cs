namespace BombusApisBee.Buffs
{
    public class Glacialstruck : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacialstruck");
            Description.SetDefault("Brr!");
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
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0f, 0f, 0, new Color(153, 212, 242, 150), 0.35f);
                }
                else
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.FrostedDust>(), 0f, 0f, 0, default).velocity *= 0.5f;
            }

            npc.GetGlobalNPC<GlacialstruckGlobalNPC>().inflicted = true;
        }
    }

    class GlacialstruckGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!inflicted)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= 6;

            if (damage < 2)
                damage = 2;
        }
    }
}
