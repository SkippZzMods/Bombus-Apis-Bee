using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Systems.ParticleSystem;

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
                "Controlled bees get coated in a heavy metal, making them move 25% slower but deal 50% more damage");
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
            modifiers.FinalDamage *= 1.5f;
        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.2f)
            {
                Player Player = Main.player[projectile.owner];

                // visuals
                SoundID.Item37.PlayWith(target.Center, 0.1f, 0.1f, 1f);
                Player.Bombus().AddShake(3);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(target.Center, velocity, Color.LightSteelBlue with { A = 0 }, velocity.ToRotation(), new Vector2(0.4f, 0.8f), 30, extraUpdateAction: ExtraUpdate));

                    velocity = Main.rand.NextVector2Circular(7f, 7f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(target.Center, velocity, [Color.Red with { A = 0 }, Color.Orange with { A = 0 }, Color.Yellow with { A = 0 }], velocity.ToRotation(), new Vector2(0.2f, 0.6f), 60, extraUpdateAction: ExtraUpdate));

                    ParticleHandler.SpawnParticle(new GlowLineParticle(target.Center, velocity, Color.White with { A = 0 }, velocity.ToRotation(), new Vector2(0.1f, 0.6f), 50, extraUpdateAction: ExtraUpdate));
                } 

                static void ExtraUpdate(Particle p)
                {
                    p.Velocity *= 0.94f;
                }
            }
        }

        public override void PreApiaryAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer == 29 && player.GetModPlayer<ApiaryPlayer>().apiaryActive && projectile.numUpdates == 0 && player.HeldItem.ModItem is IronApiary)
            {
                Dust.NewDustPerfect(projectile.Center, DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, Color.OrangeRed with { A = 0 }, Main.rand.NextFloat(0.15f, 0.3f)).customData = Main.rand.NextBool() ? -1 : 1;

                Dust.NewDustPerfect(projectile.Center, DustType<StarDustWhite>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 40, Color.DarkRed with { A = 0 }, Main.rand.NextFloat(0.3f, 0.5f)).customData = true;

                Dust.NewDustPerfect(projectile.Center, DustType<StarDustWhite>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 40, Color.LightSteelBlue with { A = 0 }, Main.rand.NextFloat(0.3f, 0.5f)).customData = true;
            }

            (projectile.ModProjectile as CommonBeeProjectile).speedMultiplier -= 0.25f;
        }

        public override void UpdateApiaryPlayer(Player Player, bool altUse)
        {
            if (altUse)
                Player.GetModPlayer<ApiaryPlayer>().maxVisualTimer = 30;

            if (altUse && Player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer == 29 && Player.GetModPlayer<ApiaryPlayer>().apiaryActive && Player.HeldItem.ModItem is IronApiary)
            {
                SoundID.Item37.PlayWith(Player.Center, 0.1f, 0.1f, 2f);
                Player.Bombus().AddShake(8);
            }             
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(300))
            {
                Color color = Main.rand.Next(new Color[] { Color.LightSteelBlue with { A = 0 }, Color.LightCyan with { A = 0 }, Color.LightSlateGray with { A = 0 } });

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustType<StarDustWhite>(), Main.rand.NextVector2Circular(3f, 3f), 0, color, 0.3f).customData = true;
            }
        }

        public override void PreDrawApiaryBees(Projectile projectile, ref Color lightColor, bool active)
        {

        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Player player = Main.player[projectile.owner];

            bool giant = (projectile.ModProjectile as CommonBeeProjectile).Giant;

            Texture2D tex = Request<Texture2D>("BombusApisBee/Content/Underground/Items/IronApiary/MetallicBee" + (giant ? "_Giant" : "")).Value;
           
            int holdTimer = player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer;

            float maxTimer = (float)player.GetModPlayer<ApiaryPlayer>().maxVisualTimer;

            float progress = EaseBuilder.EaseQuarticIn.Ease(holdTimer / maxTimer);

            Color color = Color.Lerp(new Color(244, 42, 10, 0), new Color(252, 145, 28, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
            {
                Rectangle frame = tex.Frame(1, 4, frameY: projectile.frame);

                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, frame, lightColor * progress, projectile.rotation, frame.Size() / 2f, projectile.scale * MathHelper.Lerp(3f, 1f, progress), projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }              
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(12).
                AddIngredient(ItemID.IronBar, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class IronApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(Color.LightSteelBlue, Color.LightCyan with { A = 0 }, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
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

            if (Owner.altFunctionUse == 2)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(6f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity, [Color.Red with { A = 0 }, Color.Orange with { A = 0 }, Color.Yellow with { A = 0 }], velocity.ToRotation(), new Vector2(0.2f, 0.6f), 60, extraUpdateAction: ExtraUpdate));

                    ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity, Color.White with { A = 0 }, velocity.ToRotation(), new Vector2(0.1f, 0.6f), 50, extraUpdateAction: ExtraUpdate));

                    static void ExtraUpdate(Particle p)
                    {
                        p.Velocity *= 0.95f;
                    }
                }
            }

            for (int i = 0; i < 1 + Main.rand.Next(0, 2); i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity.RotatedByRandom(1f) * 2.5f + Main.rand.NextVector2CircularEdge(1.5f, 1.5f), ProjectileType<WeakBeeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                (p.ModProjectile as CommonBeeProjectile).speedMultiplier += 0.1f;
                (p.ModProjectile as CommonBeeProjectile)._hitCooldown = 12;
            }
        }
    }
}
