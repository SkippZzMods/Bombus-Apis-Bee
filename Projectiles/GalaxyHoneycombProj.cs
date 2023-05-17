using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class GalaxyHoneycombProj : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/HoneycombOfTheGalaxies";
        internal Player Owner
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }
        public ref float Time => ref Projectile.ai[0];
        public bool Boolean = false;
        public bool HasScaled = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Honeycomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
            Projectile.Bombus().HeldProj = true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                float completionRatio = (float)k / (float)Projectile.oldPos.Length;
                Color drawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, drawColor, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        private ref float StarTimer => ref Projectile.ai[1];

        public static readonly SoundStyle StarSound = SoundID.DD2_PhantomPhoenixShot with
        {
            Volume = 3.75f
        };

        public static readonly SoundStyle BigStarSound = SoundID.DD2_PhantomPhoenixShot with
        {
            Volume = 4f
        };
        public override void AI()
        {
            Owner.Hymenoptra().BeeResourceRegenTimer = -120;
            StarTimer += 1f;
            Time += 1f;
            if (StarTimer >= 10f)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Player player = Main.player[Projectile.owner];
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (vel.X == 0f && vel.Y == 0f)
                        {
                            vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        vel.Normalize();
                        vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<GalacticStar>(), Projectile.damage * 1 / 2, Projectile.knockBack, Projectile.owner);
                    }
                    if (HasScaled)
                    {
                        Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (vel.X == 0f && vel.Y == 0f)
                        {
                            vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        vel.Normalize();
                        vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<GalacticBee>(), Projectile.damage * 1 / 2, Projectile.knockBack, Projectile.owner);
                    }
                }

                Owner.UseBeeResource(1);
                StarTimer = 0f;
            }
            Projectile.rotation += 0.25f / Projectile.MaxUpdates;
            if (Owner.channel && Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent > Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceReserved)
            {
                HomeTowardsMouse();
            }
            else
            {
                ReturnToOwner();
                float idealAngle = Projectile.AngleTo(Owner.Center) - 0.7853982f;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, idealAngle, 0.1f);
                Projectile.rotation = Utils.AngleTowards(Projectile.rotation, idealAngle, 0.25f);
                if (Time > 180f && !Boolean)
                {
                    Owner.GetModPlayer<BombusApisBeePlayer>().shakeTimer = 35;
                    SoundEngine.PlaySound(StarSound, Projectile.position);
                    SpawnDeathProjectiles();
                    Boolean = true;
                }
                Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceRegenTimer = -180;
            }
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Projectile.timeLeft = 2;
            //enbiggening section
            if (Time > 180f)
            {
                if (!HasScaled)
                {
                    Projectile.damage *= 2;
                    Owner.Bombus().shakeTimer = 20;
                    SoundEngine.PlaySound(BigStarSound, Projectile.position);
                    const int Repeats = 55;
                    for (int i = 0; i < Repeats; ++i)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)Repeats;
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center, 27, null, 50, default(Color), 2.1f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 5f;
                        dust3.noGravity = true;
                    }
                    const int Repeats2 = 65;
                    for (int i = 0; i < Repeats2; ++i)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)Repeats2;
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center, 58, null, 50, default(Color), 2.3f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 6f;
                        dust3.noGravity = true;
                    }
                    const int Repeats3 = 65;
                    for (int i = 0; i < Repeats3; ++i)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)Repeats3;
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center, 59, null, 50, default(Color), 2.5f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 7f;
                        dust3.noGravity = true;
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                        int starGore = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, value15, 16, 1.1f);
                        Main.gore[starGore].velocity *= 0.72f;
                        Main.gore[starGore].velocity += Projectile.velocity * 0.35f;
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<GalacticSeeker>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        }
                    }
                    Projectile.scale = 1.35f;
                    Projectile.localNPCHitCooldown = 12;
                    Projectile.width = 65;
                    Projectile.height = 65;
                    HasScaled = true;
                }
            }
        }
        internal void SpawnDeathProjectiles()
        {
            for (int i = 0; i < (Time > 240f ? 8 : 4); i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = Projectile.SafeDirectionTo(Owner.Center, null) * 22f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, velocity.RotatedByRandom(35), ModContent.ProjectileType<HealingProjectile>(), 0, 0f, Projectile.owner, 0, 6);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = Projectile.SafeDirectionTo(Owner.Center, null) * 22f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, velocity.RotatedByRandom(8), ModContent.ProjectileType<GalacticBee>(), Projectile.damage * 1 / 2, 0f, Projectile.owner);
                }
            }
            for (int i = 0; i < 55; i++)
            {
                int dustType = Main.rand.Next(3);
                dustType = ((dustType == 0) ? 27 : ((dustType == 1) ? 59 : 58));
                Vector2 velocity = Projectile.SafeDirectionTo(Owner.Center, null) * 22f;
                Dust dust1 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, velocity.X, velocity.Y, 10, default, 1.7f);
                dust1.velocity *= Main.rand.NextFloat(2.6f, 3.5f);
                dust1.noGravity = true;
                dust1.fadeIn = 1f;
            }
        }
        internal void HomeTowardsMouse()
        {
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }
            if (Projectile.WithinRange(Main.MouseWorld, Projectile.velocity.Length() * 0.7f))
            {
                Projectile.Center = Main.MouseWorld;
            }
            else
            {
                Projectile.velocity = (Projectile.velocity * 3f + Projectile.DirectionTo(Main.MouseWorld) * 19f) / 4f;
            }
            Projectile.netSpam = 0;
            Projectile.netUpdate = true;
        }
        internal void ReturnToOwner()
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, this.Owner.Center, 0.02f);
            Projectile.velocity = Projectile.SafeDirectionTo(this.Owner.Center, null) * 22f;
            if (Projectile.Hitbox.Intersects(this.Owner.Hitbox))
            {
                const int Repeats = 120;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)Repeats;
                    Dust dust3 = Dust.NewDustPerfect(Owner.Center, DustID.Shadowflame, null, 50, default(Color), 1.1f);
                    dust3.velocity = Utils.ToRotationVector2(angle2) * 6f;
                    dust3.noGravity = true;
                }
                Projectile.Kill();
            }
        }
    }
}