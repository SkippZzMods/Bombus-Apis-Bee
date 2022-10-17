namespace BombusApisBee.Buffs
{
    public class SkeletalCurse : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Curse");
            Description.SetDefault("You cannot feel your soul");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<SkeletalCurseGlobalNPC>().inflicted = true;

            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0f, 0f, 0, new Color(238, 164, 255), 0.4f).velocity *= 0.25f;
        }
    }

    class SkeletalCurseGlobalNPC : GlobalNPC
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
                damage = (int)(damage * 1.15f);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (inflicted)
                damage = (int)(damage * 1.15f);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (inflicted)
                drawColor = new Color(238, 164, 255, 0);
        }
    }
}
