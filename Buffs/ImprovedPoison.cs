namespace BombusApisBee.Buffs
{
    public class ImprovedPoison : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {

            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool() ? DustID.Poisoned : DustID.CorruptGibs, 0f, 0f, 100, default, 1.1f);

            npc.GetGlobalNPC<ImprovedPoisonGlobalNPC>().inflicted = true;
        }
    }

    class ImprovedPoisonGlobalNPC : GlobalNPC
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

            npc.lifeRegen -= 40;

            if (damage < 5)
                damage = 5;
        }
    }
}
