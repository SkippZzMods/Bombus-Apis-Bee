using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.BeekeeperClass;
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Core
{
    [JITWhenModsEnabled("CalamityMod")]
    public partial class BombusApisCalamityPlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public bool WulfrumHCShard;
        public bool WulfrumHCShardDraw;

        public bool MyocombShard;

        public bool MolluscanShard;

        public bool ShadesentShard;

        public override void ResetEffects()
        {
            WulfrumHCShard = false;
            WulfrumHCShardDraw = false;
            MolluscanShard = false;
            MyocombShard = false;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Player.Calamity().victideSet && item.CountsAsClass<BeekeeperDamageClass>() && Main.rand.NextBool(10) && !item.channel && Player.whoAmI == Main.myPlayer)
            {
                int seashellDamage = CalamityUtils.DamageSoftCap(damage * 2, 46);
                Projectile.NewProjectile(source, position, Player.DirectionTo(Main.MouseWorld) * 12f, ModContent.ProjectileType<Seashell>(), seashellDamage, 1f, Player.whoAmI, 0f, 0f);
            }

            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile projectile = proj;

            if (!projectile.npcProj)
            {
                if (MolluscanShard && BeeUtils.IsStrongBee(proj.whoAmI))
                {
                    modifiers.ArmorPenetration += 15;

                    if (Main.rand.NextBool(4))
                    {
                        Main.player[projectile.owner].Bombus().AddShake(3);

                        SoundID.DD2_WitherBeastDeath.PlayWith(projectile.Center, volume: 0.5f);

                        for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                                ModContent.DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(2.5f, 2.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.15f);

                            Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                                ModContent.DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(150, 255, 255, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;

                            Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                                ModContent.DustType<StarDustWhite>(), Main.rand.NextVector2Circular(6f, 6f), 0, new Color(150, 255, 255, 0), 0.5f).customData = true;
                        }
                    }
                }
            }
        }
    }
}
