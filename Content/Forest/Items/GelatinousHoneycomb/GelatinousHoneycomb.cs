using BombusApisBee.Content.Forest.Items.Honeycomb;
using BombusApisBee.Content.Forest.Items.PetrifiedHoneycomb;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Honeycomb;
using BombusApisBee.Core.Systems.ParticleSystem;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.GelatinousHoneycomb
{
    public class GelatinousHoneycomb : BaseHoneycombWeapon
    {
        public override int ThrowDustType => DustID.t_Slime;
        public override int MaxCombo => 7;
        public GelatinousHoneycomb() : base("Gelatinous Honeycomb", "Throws a slimy honeycomb that causes slimeballs to fall from the sky\nIncreases combo by 1 on direct hits\nCombo hits conjure a volatile gemstone") { }

        public override void AddDefaults()
        {
            Item.damage = 11;
            Item.knockBack = 3f;

            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 27;
            Item.useAnimation = 27;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 2);

            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<GelatinousHoneycombProjectile>();
            Item.shootSpeed = 13;
            honeyCost = 1;

            Item.noUseGraphic = true;
        }

        public override void ItemEffects(Player player, float rotation, float lerper)
        {
            Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), ThrowDustType, rotation.ToRotationVector2() * 0.5f, 150, new Color(44, 113, 255, 0), 1.2f * (1f - lerper)).noGravity = true;
        }
    }

    public class GelatinousHoneycombProjectile : BaseHoneycombProjectile
    {
        public bool flashed;
        public int flashTimer;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(2.5f, 2.5f), 150, new Color(69, 148, 255, 0), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(3.5f, 3.5f), 150, new Color(44, 113, 255, 0), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;
            }

            if (flashTimer > 0)
                flashTimer--;

            if (!flashed)
            {
                if (ComboProjectile)
                {
                    flashTimer = 60;
                    Projectile.velocity *= 1.2f;

                    new SoundStyle("BombusApisBee/Sounds/Item/ProjectileLaunch1").PlayWith(Projectile.Center, -0.1f, 0, 1.2f);
                    Owner.Bombus().AddShake(3);
                }

                flashed = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;

            if (++Timer > 20)
            {
                Projectile.velocity.Y += 0.15f;

                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.06f;
                    else
                        Projectile.velocity.Y *= 1.03f;
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

            var ribbon = Request<Texture2D>(Texture + "_Ribbon").Value;

            SpriteBatch sb = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float lerp = 1f - i / (float)Projectile.oldPos.Length;

                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f;

                if (flashTimer > 0)
                {
                    float interp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / 60f);

                    sb.Draw(outline, pos - Main.screenPosition, null, new Color(224, 33, 22, 0) * lerp * interp, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);
                }

                sb.Draw(texture, pos - Main.screenPosition, null, lightColor * lerp * 0.5f, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            sb.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);

            if (flashTimer > 0)
            {
                float lerp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / 60f);

                sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(224, 33, 22, 0) * lerp * 0.3f, Projectile.rotation * 2f, bloom.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(outline, Projectile.Center - Main.screenPosition, null, new Color(224, 33, 22, 0) * lerp, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(224, 33, 22, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.6f, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.4f, 0f, 0f);
            }

            var curve = GetCurve();
           
            float fadeIn = Timer / 30f;
            if (fadeIn > 1)
                fadeIn = 1;

            int points = 6;

            Vector2[] positions = curve.GetPoints(points).ToArray();

            for (int i = 0; i < points; i++)
            {
                float lerp = i / (float)points;

                Vector2 pos = positions[i];

                int variant = (int)MathHelper.Lerp(1, 5, lerp);

                Rectangle frame = ribbon.Frame(1, 5, 0, variant);

                float rotation = 0f;

                if (i < points - 1)
                {
                    rotation = positions[i + 1].DirectionTo(pos).ToRotation();
                }
                else
                {
                    rotation = pos.DirectionTo(positions[i - 1]).ToRotation();
                }

                sb.Draw(ribbon, pos - Main.screenPosition, frame, lightColor * fadeIn, rotation + MathHelper.PiOver2, frame.Size() / 2f, 1f, 0f, 0f);
            }

            Rectangle rect = ribbon.Frame(1, 5, 0, 0);
            
            float stampRotation = positions[1].DirectionTo(positions[0]).ToRotation();

            sb.Draw(ribbon, positions[0] + new Vector2(12f, -4f).RotatedBy(stampRotation) - Main.screenPosition, rect, lightColor, stampRotation, rect.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }

        private BezierCurve GetCurve()
        {
            float fadeIn = Timer / 30f;
            if (fadeIn > 1)
                fadeIn = 1;

            Vector2[] points = [
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.02f) * 1.25f * fadeIn,
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.01f) * 4 * fadeIn,
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.005f) * 6 * fadeIn,
            ];

            var curve = new BezierCurve(points);

            return curve;
        }
        

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ComboProjectile)
            {
                Owner.Bombus().AddShake(5);
            }
            else
            {
                ParentWeapon?.AddCombo();

                if (ParentWeapon.CurrentCombo == ParentWeapon.MaxCombo)
                {
                    SoundID.MaxMana.PlayWith(Owner.Center);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 125, new Color(13, 61, 158), Main.rand.NextFloat(0.5f, 0.9f));

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(2f, 2f), 125, new Color(44, 113, 255), Main.rand.NextFloat(0.5f, 0.8f));

                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1.25f, 1.25f), 125, new Color(118, 185, 255), Main.rand.NextFloat(0.5f, 0.8f));
            }

            for (int i = 0; i < 5; i++)
            {
                ParticleHandler.SpawnParticle(new GelatinousHoneycombParticle(Projectile.Center, Main.rand.NextVector2Circular(6f, 6f), Main.rand.NextFloat(0.9f, 1.1f), Main.rand.Next(30, 70)));

                Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(5f, 5f), 200, new Color(69, 148, 255), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(3.5f, 3.5f), 200, new Color(44, 113, 255), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;
            }
        }

        static void SmokeBehavior(Particle p)
        {
            p.Velocity.Y -= 0.015f;
            p.Velocity.X *= 0.97f;
        }
    }

    class GelatinousHoneycombParticle : Particle
    {
        internal int _variant;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public GelatinousHoneycombParticle(Vector2 position, Vector2 velocity, float scale, int maxTime)
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
            var texture = Request<Texture2D>("BombusApisBee/Content/Forest/Items/GelatinousHoneycomb/GelatinousHoneycombParticle_0" + _variant).Value;

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