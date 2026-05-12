using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Forest.Items.WoodenApiary;
using BombusApisBee.Content.Snow.Items.BorealApiary;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Underground.Items.IronApiary
{
    public class IronApiary : ApiaryItem
    {
        public override int BaseUseTime => 35;
        public override int AltUseTime => 45;

        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Iron Apiary");
            Tooltip.SetDefault("" +
                "Hold <left> to rapidly fire bees\n" +
                "Hold <right> to fire the bees slower, but take control over the bees\n" +
                "Controlled bees get coated in a heavy metal, making them move 25% slower but deal double damage");
        }

        public override void AddDefaults()
        {
            Item.damage = 9;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;

            Item.value = Item.sellPrice(silver: 19);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<IronApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 2;
            altHoneyCost = 3;
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 5;
            modifiers.FinalDamage *= 2;
        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                // visuals
                
            }
        }

        public override void PreApiaryAI(Projectile projectile)
        {
            (projectile.ModProjectile as CommonBeeProjectile).speedMultiplier -= 0.25f;
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(300))
            {
                Color color = Color.Lerp(new Color(252, 145, 28, 0), new Color(244, 42, 10, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustType<StarDustWhite>(), Main.rand.NextVector2Circular(3f, 3f), 0, color, 0.3f).customData = true;
            }
        }

        public override void PreDrawApiaryBees(Projectile projectile, ref Color lightColor, bool active)
        {

        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            bool giant = (projectile.ModProjectile as CommonBeeProjectile).Giant;

            Texture2D tex = Request<Texture2D>("BombusApisBee/Content/Underground/Items/IronApiary/MetallicBee" + (giant ? "_Giant" : "")).Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().holdTimer;

            Color color = Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
            {
                Rectangle frame = tex.Frame(1, 4, frameY: projectile.frame);

                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, frame, Color.White * (holdTimer / 20f) * 0.5f, 0f, frame.Size() / 2f, MathHelper.Lerp(3f, 1f, holdTimer / 20f), 0, 0f);
            }              
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(12).
                AddRecipeGroup(RecipeGroupID.IronBar, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class IronApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override bool UseDefaultTextures => true;
        public override string Texture => "BombusApisBee/Content/Underground/Items/IronApiary/IronApiary";

        protected override void Shoot()
        {
            shakeTimer = 12;

            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 2; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.15f).customData = Main.rand.NextBool() ? -1 : 1;
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 10f + Main.rand.NextVector2Circular(20f, 20f), DustID.Torch, Main.rand.NextVector2Circular(1f, 1f), 20, default, 2.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(1f, 4f), 200, new Color(20, 20, 20), 1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(1f, 3f), 200, new Color(50, 50, 50), 0.8f);
            }

            for (int i = 0; i < 1 + Main.rand.Next(0, 3); i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(4f, 5f), ProjectileType<WeakBeeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void PostDraw(Color lightColor)
        {
           
        }
    }
}
