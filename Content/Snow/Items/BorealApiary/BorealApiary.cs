using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Apiary;

namespace BombusApisBee.Content.Snow.Items.BorealApiary
{
    public class BorealApiary : ApiaryItem
    {
        public override int BaseUseTime => 21;
        public override int AltUseTime => 30;

        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Boreal Apiary");
            Tooltip.SetDefault("" +
                "Hold <left> to rapidly fire snowy bees\n" +
                "Hold <right> to fire the bees slower, but take control over the bees\n" +
                "Controlled bees have 10% increased critical strike chance, and their crits inflict Frostburn");
        }

        public override void AddDefaults()
        {
            Item.damage = 6;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            critAdd = 4;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.15f;

            Item.value = Item.sellPrice(silver: 1);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<BorealApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
            altHoneyCost = 2;
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].Beekeeper().RerollCrit(10))
                modifiers.SetCrit();
        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                target.AddBuff(BuffID.Frostburn, 180);

                Main.player[projectile.owner].Bombus().AddShake(3);

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FrostHit").PlayWith(target.Center, 0.1f, 0.1f, 0.3f);

                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = target.Center + Main.rand.NextVector2Circular(target.width, target.height);

                    Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), -Vector2.UnitY * Main.rand.NextFloat(2f), 0, new Color(128, 180, 226, 0), 0.1f);

                    pos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2);

                    Dust.NewDustPerfect(pos, DustType<SmokeDust2>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(3f), 170, new Color(233, 233, 242), 0.7f);

                    pos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2);

                    Dust.NewDustPerfect(pos, DustType<SmokeDust2>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(1.5f), 170, new Color(178, 181, 212), 0.7f);
                }

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.DistanceSQ(target.Center) > 20 * 20)
                        continue;

                    if (proj.owner == projectile.owner && proj.type == projectile.type)
                        proj.velocity += Main.rand.NextVector2CircularEdge(7f, 7f) * Main.rand.NextFloat(0.7f, 1.1f);
                }
            }
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(150))
            {
                Color color = Color.Lerp(new Color(215, 216, 234, 0), new Color(190, 223, 232, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.15f);
            }
        }

        public override void PreDrawApiaryBees(Projectile projectile, ref Color lightColor, bool active)
        {
            
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Texture2D tex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().holdTimer;

            Color color = Color.Lerp(new Color(215, 216, 234, 0), new Color(190, 223, 232, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, color * (holdTimer / 20f) * 0.2f, 0f, tex.Size() / 2f, 0.25f, 0, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(5).
                AddIngredient(ItemID.SnowBlock, 15).
                AddIngredient(ItemID.BorealWood, 30).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class BorealApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(new Color(215, 216, 234), new Color(190, 223, 232), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override string Texture => "BombusApisBee/Content/Snow/Items/BorealApiary/BorealApiary";

        protected override void Shoot()
        {
            shakeTimer = 12;

            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 2; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.1f);
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(1f, 4f), 200, GlowColor, 1f);
            }

            for (int i = 0; i < 1 + Main.rand.Next(1, 4); i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(4f, 5f), ProjectileType<SnowBee>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
