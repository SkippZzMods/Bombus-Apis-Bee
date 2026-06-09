using BombusApisBee.Assets;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Systems.ParticleSystem;

namespace BombusApisBee.Content.Underground.Items.LeadApiary
{
    public class LeadApiary : ApiaryItem
    {
        internal sealed class LeadPoisoningNPC : GlobalNPC
        {
            public const int MAX_STACKS = 15;

            public override bool InstancePerEntity => true;

            private int stacks;
            private int timeTillDecay;

            public void AddStack(int stacksToAdd, int timeTillDecay = 300)
            {
                stacks += stacksToAdd;
                this.timeTillDecay = timeTillDecay;

                if (stacks > MAX_STACKS)
                    stacks = MAX_STACKS;
            }

            public override void ResetEffects(NPC npc)
            {
                if (timeTillDecay > 0)
                    timeTillDecay--;
                else if (stacks > 0)
                {
                    stacks--;
                    timeTillDecay = 20;
                }               
            }

            public override void DrawEffects(NPC npc, ref Color drawColor)
            {
                if (stacks > 0)
                    drawColor = Color.Lerp(drawColor, Color.DarkSlateBlue, stacks / (float)MAX_STACKS);
            }

            public override void UpdateLifeRegen(NPC npc, ref int damage)
            {
                if (stacks > 0)
                {
                    if (npc.lifeRegen > 0)
                        npc.lifeRegen = 0;

                    npc.lifeRegen -= stacks * 2;

                    damage = 1;
                }
            }

            public override void AI(NPC npc)
            {
                if (stacks > 0)
                {
                    // visual stuff

                    if (Main.rand.NextBool(115 - (stacks * 7)))
                    {
                        Dust d = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), DustID.Lead, Main.rand.NextVector2Circular(1f, 1f), 100, default, 1.2f);
                        d.noGravity = true;
                        d.fadeIn = 1f;
                    }

                    if (Main.rand.NextBool(130 - (stacks * 7)))
                    {
                        Dust d = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), DustID.Poisoned, Main.rand.NextVector2Circular(1.5f, 1.5f), 150, default, 1.2f);
                        d.noGravity = true;
                        d.fadeIn = 1f;
                    }
                }
            }
        }

        public override int BaseUseTime => 25;
        public override int AltUseTime => 36;

        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Lead Apiary");
            Tooltip.SetDefault("" +
                "Hold <left> to rapidly fire bees\n" +
                "Hold <right> to fire the bees slower, but take control over the bees\n" +
                "Controlled bees inflict stacks of Lead Poisoning\n" +
                "'Banned in Michigan'");
        }

        public override void AddDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.33f;

            Item.value = Item.sellPrice(silver: 19);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<LeadApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
            altHoneyCost = 3;
        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.33f)
            {
                target.GetGlobalNPC<LeadPoisoningNPC>().AddStack(1);
            }
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(300))
            {
                Color color = Main.rand.Next(new Color[] { Color.DarkSlateBlue with { A = 0 }, Color.SlateBlue with { A = 0 }, Color.LightSlateGray with { A = 0 } });

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustType<StarDustWhite>(), Main.rand.NextVector2Circular(3f, 3f), 0, color, 0.3f).customData = true;
            }
        }

        public override void PreDrawApiaryBees(Projectile projectile, ref Color lightColor, bool active)
        {
            bool giant = (projectile.ModProjectile as CommonBeeProjectile).Giant;

            Texture2D tex = Request<Texture2D>(giant ? AssetDirectory.GiantBeeOutline : AssetDirectory.BeeOutline).Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer;

            Color color = Color.Lerp(Color.DarkSlateBlue with { A = 0 }, Color.LightSlateGray with { A = 0 }, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            Rectangle frame = tex.Frame(1, 4, frameY: projectile.frame);

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center + new Vector2(0, -1).RotatedBy(projectile.rotation) - Main.screenPosition, frame, color * 0.5f * (holdTimer / (float)player.GetModPlayer<ApiaryPlayer>().maxVisualTimer), projectile.rotation, frame.Size() / 2f, projectile.scale, projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(12).
                AddIngredient(ItemID.LeadBar, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class LeadApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(Color.DarkSlateBlue with { A = 0 }, Color.LightSlateGray, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override bool UseDefaultTextures => true;
        protected override void Shoot()
        {
            flashTimer = 20;
            swingRotation += Main.rand.NextFloat(-0.25f, 0.25f);
            shakeTimer = 13;

            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 3; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), 50, default, 1.2f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.1f).customData = Main.rand.NextBool() ? -1 : 1;
            }

            for (int i = 0; i < 1 + Main.rand.Next(0, 2); i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity.RotatedByRandom(1f) * 2.5f + Main.rand.NextVector2CircularEdge(1.5f, 1.5f), ProjectileType<WeakBeeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                (p.ModProjectile as CommonBeeProjectile).speedMultiplier += 0.15f;
                (p.ModProjectile as CommonBeeProjectile)._hitCooldown = 13;
            }
        }
    }
}
