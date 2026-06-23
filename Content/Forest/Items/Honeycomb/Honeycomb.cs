using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.Honeycomb;
using BombusApisBee.Core.Systems.ParticleSystem;

namespace BombusApisBee.Content.Forest.Items.Honeycomb
{
    public class Honeycomb : BaseHoneycombWeapon
    {
        public Honeycomb() : base("Honeycomb", "Throws a fragile honeycomb\nIncreases combo by 1 on hit\nWhen at max combo, direct hits create a burst of bees") { }

        public override void AddDefaults()
        {
            Item.damage = 6;
            Item.knockBack = 1f;

            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.value = 100;
            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<HoneycombProjectile>();
            Item.shootSpeed = 12f;
            honeyCost = 1;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemType<PollenItem>(), 5).
                AddIngredient(ItemID.Wood, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class HoneycombProjectile : BaseHoneycombProjectile
    {
        public bool flashed;
        public int flashTimer;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (!flashed)
            {
                if (ComboProjectile)
                {
                    flashTimer = 60;
                    Projectile.velocity *= 1.15f;

                    new SoundStyle("BombusApisBee/Sounds/Item/ProjectileLaunch1").PlayWith(Projectile.Center, -0.1f, 0, 1.2f);
                    Owner.Bombus().AddShake(3);

                    for (int i = 0; i < 3; i++)
                    {
                        ParticleHandler.SpawnParticle(new StarParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.3f), Color.Orange, 1f, 50));

                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Honey2, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.3f), 50, default, Main.rand.NextFloat(0.5f, 1.1f));

                    d.noGravity = true;
                    d.fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Honey2, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.2f), 50, default, Main.rand.NextFloat(0.4f, 1f));
                }

                flashed = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;
            
            if (++Timer > 10)
            {
                Projectile.velocity.Y += 0.2f;

                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.07f;
                    else
                        Projectile.velocity.Y *= 1.04f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }      
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = Request<Texture2D>(Texture).Value;
            var outline = Request<Texture2D>(Texture + "_Outline").Value;
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var star = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            SpriteBatch sb = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float lerp = 1f -  i / (float)Projectile.oldPos.Length;

                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f;
                
                sb.Draw(texture, pos - Main.screenPosition, null, lightColor * lerp * 0.5f, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            sb.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);

            if (flashTimer > 0)
            {
                float lerp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / 60f);
                
                sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(251, 184, 17, 0) * lerp * 0.3f, Projectile.rotation * 2f, bloom.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(outline, Projectile.Center - Main.screenPosition, null, new Color(251, 184, 17, 0) * lerp, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(251, 184, 17, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.6f, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 235, 25, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.4f, 0f, 0f);
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ComboProjectile)
            {
                new SoundStyle("BombusApisBee/Sounds/Item/GoreLight").PlayWith(Projectile.Center, -0.1f, 0, 0.7f);
                BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, -0.3f, 0.1f, 1.25f);

                Owner.Bombus().AddShake(5);

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 150, new Color(200, 150, 20), Main.rand.NextFloat(0.5f, 0.9f));

                    Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 150, new Color(251, 184, 17), Main.rand.NextFloat(0.5f, 0.8f));

                    if (Main.rand.NextBool()) 
                        ParticleHandler.SpawnParticle(new LargeBeeParticle(Projectile.Center, Main.rand.NextVector2Circular(1.5f, 1.5f), 0f, 1f, 60));
                }

                for (int i = 0; i < 3; i++)
                {
                    ParticleHandler.SpawnParticle(new StarParticle(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 1f, 50));

                    if (Main.myPlayer == Owner.whoAmI)
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f),
                            ProjectileType<WeakBeeProjectile>(), (int)(Projectile.damage * 1.5f), 2f, Owner.whoAmI);
                }
            }
            else
            {
                ParentWeapon?.AddCombo();

                if (ParentWeapon.HasMaxCombo)
                {
                    SoundID.MaxMana.PlayWith(Owner.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        ParticleHandler.SpawnParticle(new StarParticle(Owner.Center, Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 1f, 50)
                        {
                            Layer = Core.Systems.PixelationSystem.RenderLayer.OverPlayers
                        });
                    }
                }
            }            
        }

        public override void OnKill(int timeLeft)
        {
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1f, 1f), 150, new Color(200, 150, 20), Main.rand.NextFloat(0.5f, 0.9f));

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1f, 1f), 150, new Color(251, 184, 17), Main.rand.NextFloat(0.5f, 0.8f));
            }

            for (int i = 0; i < 4; i++)
            {
                if (Main.rand.NextBool(3))
                    ParticleHandler.SpawnParticle(new BeeParticle(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), 0f, 1f, 60));

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Honey2, Main.rand.NextVector2Circular(4f, 4f), 50, default, Main.rand.NextFloat(0.5f, 1.1f));

                d.noGravity = true;
                d.fadeIn = 1f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Honey2, -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.2f), 50, default, Main.rand.NextFloat(0.4f, 1f));

                ParticleHandler.SpawnParticle(new HoneycombParticle(Projectile.Center, Main.rand.NextVector2Circular(7f, 7f), Main.rand.NextFloat(0.9f, 1.1f), Main.rand.Next(30, 70)));
            }
        }
    }

    class HoneycombParticle : Particle
    {
        internal int _variant;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public HoneycombParticle(Vector2 position, Vector2 velocity, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;

            _variant = 1 + Main.rand.Next(3);
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Rotation += Velocity.Length() * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = Request<Texture2D>("BombusApisBee/Content/Forest/Items/Honeycomb/HoneycombParticle_0" + _variant).Value;

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * fadeIn, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}