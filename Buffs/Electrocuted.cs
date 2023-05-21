using BombusApisBee.Projectiles;
namespace BombusApisBee.Buffs
{
    public class Electrocuted : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrocuted");
            Description.SetDefault("Zap!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(480) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/LightningStrike").PlayWith(npc.position, 0, 0.1f, 1f);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + new Vector2(Main.rand.Next(-200, 200), -900), Vector2.UnitY * 10f, ModContent.ProjectileType<ElectricHoneycombLightning>(), Main.rand.Next(50, 100), 5f, Main.myPlayer);
                Main.player[npc.FindClosestPlayer()].Bombus().AddShake(10);
            }

            Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0f, 0f, 0, new Color(110, 220, 255), 0.4f);

            npc.GetGlobalNPC<ElectrocutedGlobalNPC>().inflicted = true;
        }
    }

    class ElectrocutedGlobalNPC : GlobalNPC
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

            npc.lifeRegen -= 30;

            if (damage < 1)
                damage = 1;
        }
    }
}
