using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Forest.Items.WoodenApiary;
using BombusApisBee.Content.Snow.Items.BorealApiary;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Underground.Items.FlamingApiary
{
    public class FlamingApiary : ApiaryItem
    {
        public override int BaseUseTime => 29;
        public override int AltUseTime => 41;

        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Flaming Apiary");
            Tooltip.SetDefault("" +
                "Hold <left> to rapidly fire bees\n" +
                "Hold <right> to fire the bees slower, but take control over the bees, lighting them on fire\n" +
                "Controlled bees move 25% faster, have 5% increased critical strike chance, and their critical strikes inflict On Fire!");
        }

        public override void AddDefaults()
        {
            Item.damage = 7;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;

            Item.value = Item.sellPrice(silver: 25);

            Item.rare = ItemRarityID.Blue;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<FlamingApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 2;
            altHoneyCost = 3;
        }

        public override void PreApiaryAI(Projectile projectile)
        {
            (projectile.ModProjectile as CommonBeeProjectile).speedMultiplier += 0.2f;
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].Beekeeper().RerollCrit(5))
                modifiers.SetCrit();
        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                // visuals
                if (!target.HasBuff(BuffID.OnFire))
                {
                    new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(target.Center, 0.1f, 0.1f, 0.3f);

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 pos = target.Center + Main.rand.NextVector2Circular(target.width, target.height);

                        Dust.NewDustPerfect(pos, DustType<PixelatedEmber>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 4f), 0, Color.DarkOrange with { A = 0 }, 0.25f).customData = Main.rand.NextBool() ? -1 : 1;
                        
                        pos = target.Center + Main.rand.NextVector2Circular(target.width, target.height);

                        Dust.NewDustPerfect(pos, DustType<PixelatedEmber>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 4f), 0, Color.OrangeRed with { A = 0 }, 0.25f).customData = Main.rand.NextBool() ? -1 : 1;

                        pos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2);

                        Dust.NewDustPerfect(pos, DustType<SmokeDust2>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(3f), 170, Color.OrangeRed with { A = 0 }, 0.7f);

                        pos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2);

                        Dust.NewDustPerfect(pos, DustType<SmokeDust2>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(1.5f), 170, Color.Yellow with { A = 0 }, 0.7f);

                        Dust.NewDustPerfect(pos, DustID.Torch, -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 4f), 20, default, 2f);
                        
                        Dust.NewDustPerfect(pos, DustID.Torch, Main.rand.NextVector2Circular(5f, 5f), 20, default, 2f);

                        Dust.NewDustPerfect(pos, DustID.Torch, Main.rand.NextVector2Circular(5f, 5f), 20, default, 3f).noGravity = true;
                    }
                }

                target.AddBuff(BuffID.OnFire, 240);
            }
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(150))
            {
                Color color = Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustType<PixelatedEmber>(), -Vector2.UnitY * Main.rand.NextFloat(1.5f), 0, color, 0.1f).customData = Main.rand.NextBool() ? -1 : 1;
            }

            if (Main.rand.NextBool(300))
            {
                Color color = Color.Lerp(new Color(252, 145, 28, 0), new Color(244, 42, 10, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustType<StarDustWhite>(), Main.rand.NextVector2Circular(3f, 3f), 0, color, 0.3f).customData = true;
            }

            if (Main.rand.NextBool(250))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Torch, Main.rand.NextVector2Circular(3f, 3f), 0, default, 2.5f).noGravity = true;
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Texture2D tex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer;

            Color color = Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, color * (holdTimer / (float)player.GetModPlayer<ApiaryPlayer>().maxVisualTimer) * 0.2f, 0f, tex.Size() / 2f, 0.25f, 0, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WoodenApiary>().
                AddIngredient<PollenItem>(5).
                AddIngredient(ItemID.StoneBlock, 30).
                AddRecipeGroup(BombusApisBeeModSystem.CopperBarGroupID, 9).
                AddIngredient(ItemID.Torch, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class FlamingApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override bool UseDefaultTextures => true;
        protected override void Shoot()
        {
            flashTimer = 20;
            swingRotation += Main.rand.NextFloat(-0.25f, 0.25f);
            shakeTimer = 13;

            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 2; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.15f).customData = Main.rand.NextBool() ? -1 : 1;
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 10f + Main.rand.NextVector2Circular(20f, 20f), DustID.Torch, Main.rand.NextVector2Circular(1f, 1f), 20, default, 2.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(1f, 4f), 200, new Color(20, 20, 20), 1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(1f, 3f), 200, new Color(50, 50, 50), 0.8f);
            }

            Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(3f) + Main.rand.NextVector2Circular(2f, 2f), ProjectileType<WeakBeeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
