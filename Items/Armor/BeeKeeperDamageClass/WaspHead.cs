using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using System.Linq;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class WaspHead : BeeKeeperItem
    {
        public override void Load()
        {
            BeePlayerBeeProjectile.ExtraAIEvent += AddStingers;

            BeePlayerBeeProjectile.ExtraPreAIEvent += AddGatheringStingers;
        }

        private void AddGatheringStingers(Projectile proj)
        {
            BeePlayerBeeProjectile modProj = proj.ModProjectile as BeePlayerBeeProjectile;

            if (modProj.Player.Bombus().WaspArmorSet)
            {
                if (modProj.waspAttackCooldown > 0)
                    modProj.waspAttackCooldown--;

                if (modProj.waspAttackCooldown <= 0)
                {
                    modProj.waspAttackCooldown = 75;

                    Projectile.NewProjectileDirect(proj.GetSource_FromAI(), proj.Center + proj.velocity, proj.DirectionTo(proj.Center - proj.velocity * 15f).RotatedByRandom(0.2f) * Main.rand.NextFloat(10, 20), ModContent.ProjectileType<Projectiles.HomingStinger>(), 15, 1f, proj.owner);

                    for (int d = 0; d < 15; d++)
                    {
                        Dust.NewDustPerfect(proj.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), proj.DirectionTo(proj.Center - proj.velocity * 15f).RotatedByRandom(0.3f) * Main.rand.NextFloat(1, 3), 0, new Color(79, 170, 29) * Main.rand.NextFloat(.2f, 1f), 0.4f);
                    }

                    BeeUtils.CircleDust(proj.Center, 10, ModContent.DustType<Dusts.GlowFastDecelerate>(), 1, 0, new Color(79, 170, 29) * Main.rand.NextFloat(.2f, 1f), 0.35f);
                }
            }
        }

        private void AddStingers(Projectile proj)
        {
            BeePlayerBeeProjectile modProj = proj.ModProjectile as BeePlayerBeeProjectile;

            if (modProj.Player.Bombus().WaspArmorSet)
            {
                if (modProj.Defense)
                {
                    if (modProj.waspAttackCooldown > 0)
                        modProj.waspAttackCooldown--;

                    NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(proj.Center) < 500f && Collision.CanHitLine(proj.Center, 1, 1, n.Center, 1, 1)).OrderBy(n => n.Distance(proj.Center)).FirstOrDefault();

                    if (target != default && modProj.waspAttackCooldown <= 0)
                    {
                        Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, proj.DirectionTo(target.Center) * 35f, ModContent.ProjectileType<Projectiles.StingerFriendly>(), 20, 4f, proj.owner);

                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustPerfect(proj.Center, ModContent.DustType<Dusts.StingerDust>(), proj.DirectionTo(target.Center).RotatedByRandom(.3f) * Main.rand.NextFloat(5f), Main.rand.Next(255), Scale: 1.25f).noGravity = true;
                        }

                        BeeUtils.CircleDust(proj.Center, 20, ModContent.DustType<Dusts.StingerDust>(), 1, Main.rand.Next(255), null, 1f);

                        modProj.waspAttackCooldown = Main.rand.Next(60, 150);
                    }
                }
                else if (modProj.Offense)
                {
                    if (modProj.waspAttackCooldown > 0)
                        modProj.waspAttackCooldown--;

                    if (modProj.Attacking && modProj.waspAttackCooldown <= 0 && modProj.attackTarget != null)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 velocity = proj.DirectionTo(modProj.attackTarget.Center).RotatedBy(MathHelper.ToRadians(i * 15)) * 25;

                            Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, velocity, ModContent.ProjectileType<Projectiles.StingerFriendly>(), 25, 1.5f, proj.owner);

                            for (int d = 0; d < 15; d++)
                            {
                                Dust.NewDustPerfect(proj.Center, ModContent.DustType<Dusts.StingerDust>(), velocity.RotatedByRandom(.05f) * Main.rand.NextFloat(.5f), Main.rand.Next(255), Scale: 1.25f).noGravity = true;
                            }
                        }

                        BeeUtils.CircleDust(proj.Center, 20, ModContent.DustType<Dusts.StingerDust>(), 1, Main.rand.Next(255), null, 1f);

                        modProj.waspAttackCooldown = 120;
                    }
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("40% increased wing flight time and increased jump speed\nIncreases maximum honey by 25");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WaspBreastplate>() && legs.type == ModContent.ItemType<WaspGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Your Loyal Bees shoot powerful stingers";
            player.Bombus().WaspArmorSet = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 25;
            player.jumpSpeedBoost += 2.25f;
            player.Bombus().wingFlightTimeBoost += 0.40f;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimtaneBar, 8).AddIngredient(ModContent.ItemType<Pollen>(), 9).AddIngredient(ItemID.TissueSample, 8).AddTile(TileID.Anvils).Register();
            CreateRecipe(1).AddIngredient(ItemID.DemoniteBar, 8).AddIngredient(ModContent.ItemType<Pollen>(), 9).AddIngredient(ItemID.ShadowScale, 8).AddTile(TileID.Anvils).Register();
        }
    }
}