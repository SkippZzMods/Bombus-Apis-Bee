using BombusApisBee.Dusts;
using BombusApisBee.Items.Weapons.BeeKeeperDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace BombusApisBee.Projectiles
{
    public class RetinaHoneycomb : BeeProjectile
    {
        public int flashTimer;
        public bool deathAnimation;
        public bool dash;
        public float rotTimer;
        public float shots;
        public ref float chargeDelay => ref Projectile.ai[1];
        public ref float attackDelay => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];

        public override bool? CanDamage() => Projectile.penetrate > 1 && dash;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retinacomb");
            Main.projFrames[Type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 40;
            Projectile.timeLeft = 7200;
            DrawOffsetX = -15;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter % 7 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            if (Main.myPlayer == owner.whoAmI && owner.HeldItem.ModItem is OcularRemote && Main.mouseRight && owner.Bombus().OcularCooldown <= 0)
            {
                owner.Bombus().OcularCooldown = 1800;
                Projectile.Kill();
            }

            if (rotTimer > 0)
                rotTimer--;

            if (flashTimer > 0)
                flashTimer--;

            if (Projectile.timeLeft < 130 || owner.Hymenoptra().BeeResourceCurrent <= 0)
                deathAnimation = true;

            if (deathAnimation)
            {
                Projectile.velocity *= 0.95f;

                if (++attackDelay < 120)
                {
                    Projectile.rotation += MathHelper.Lerp(0.1f, 0.5f, attackDelay / 120f) * Projectile.direction;

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f;
                        float lerper = MathHelper.Lerp(100, 0f, attackDelay / 120f);
                        Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(lerper, lerper), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(161, 31, 85), 0.4f);
                    }

                    if (attackDelay % (int)MathHelper.Lerp(10, 3, attackDelay / 120f) == 0)
                    {
                        Vector2 velo = Projectile.rotation.ToRotationVector2();
                        Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f;
                        for (int i = 0; i < 35; ++i)
                        {
                            float angle2 = 6.28f * (float)i / (float)35;
                            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 1.25f, 0, new Color(161, 31, 85), 0.35f);
                        }

                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, velo * 8f,
                                ModContent.ProjectileType<RedLaserHoming>(), Projectile.damage / 2, 2.5f, Projectile.owner);

                        SoundID.Item33.PlayWith(Projectile.position, -0.15f, 0.1f);
                    }
                }
                else
                    Projectile.Kill();
                return;
            }
            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(owner.Center) < 750f).OrderBy(n => Vector2.Distance(n.Center, owner.Center)).FirstOrDefault();

            if (target != default)
            {
                if (shots < 8)
                {
                    Projectile.rotation = Projectile.DirectionTo(target.Center).ToRotation() - (MathHelper.ToRadians(rotTimer) * target.direction);

                    Vector2 targetPos = new Vector2(target.Center.X - 75 * target.direction, target.Center.Y - 100);

                    Vector2 toIdlePos = targetPos - Projectile.Center;
                    if (toIdlePos.Length() < 0.0001f)
                        toIdlePos = Vector2.Zero;

                    else
                    {
                        float speed = Vector2.Distance(targetPos, Projectile.Center) * 0.15f;
                        speed = Utils.Clamp(speed, 2.5f, 25f);
                        toIdlePos.Normalize();
                        toIdlePos *= speed;
                    }

                    Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

                    if (++attackDelay > 15 && Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1))
                    {
                        Vector2 velo = Projectile.DirectionTo(target.Center);
                        Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f;
                        for (int i = 0; i < 35; ++i)
                        {
                            float angle2 = 6.28f * (float)i / (float)35;
                            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 1.25f, 0, new Color(161, 31, 85), 0.35f);
                        }
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, velo * 8f,
                                ModContent.ProjectileType<RedLaser>(), Projectile.damage, 2.5f, Projectile.owner);

                        SoundID.Item33.PlayWith(Projectile.position, -0.15f, 0.1f);

                        Projectile.velocity += velo * -3.45f;
                        flashTimer = 15;
                        attackDelay = 0;
                        shots++;
                    }
                }
                else if (!dash)
                {
                    Vector2 targetPos = new Vector2(target.Center.X, target.Center.Y - 100);

                    Vector2 toIdlePos = targetPos - Projectile.Center;
                    if (toIdlePos.Length() < 0.0001f)
                        toIdlePos = Vector2.Zero;

                    else
                    {
                        float speed = Vector2.Distance(targetPos, Projectile.Center) * 0.15f;
                        speed = Utils.Clamp(speed, 2.5f, 25f);
                        toIdlePos.Normalize();
                        toIdlePos *= speed;
                    }

                    Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

                    if (chargeDelay < 90)
                        chargeDelay++;
                    else
                    {
                        dash = true;
                        chargeDelay = 0;
                        attackDelay = -25f;
                        return;
                    }   

                    rotTimer = MathHelper.Lerp(0, -35f, chargeDelay / 25f);
                    if (chargeDelay <= 45f)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f;
                            float lerper = MathHelper.Lerp(55, 0f, chargeDelay / 45f);
                            Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(lerper, lerper), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(161, 31, 85), 0.4f);
                        }
                    }

                    if (chargeDelay >= 25)
                    {
                        if (chargeDelay <= 45)
                            rotTimer = -35f;
                        else
                        {
                            rotTimer = MathHelper.Lerp(-35, 35f, (chargeDelay - 45f) / 45f);

                            if (chargeDelay % 5 == 0)
                            {
                                Vector2 velo = Projectile.rotation.ToRotationVector2();
                                Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f;
                                for (int i = 0; i < 35; ++i)
                                {
                                    float angle2 = 6.28f * (float)i / (float)35;
                                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 1.25f, 0, new Color(161, 31, 85), 0.35f);
                                }
                                if (Main.myPlayer == Projectile.owner)
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, velo * 8f,
                                        ModContent.ProjectileType<RedLaser>(), Projectile.damage, 2.5f, Projectile.owner);

                                SoundID.Item33.PlayWith(Projectile.position, -0.15f, 0.1f);

                                Projectile.velocity += velo * -3f;

                                flashTimer = 15;
                            }
                        }
                    }    
                    Projectile.rotation = Projectile.DirectionTo(target.Center).ToRotation() - (MathHelper.ToRadians(rotTimer) * target.direction);
                }
                else
                {
                    if (attackDelay == -25f)
                    {
                        Projectile.velocity = Projectile.DirectionTo(target.Center) * 18f;
                    }

                    if (++attackDelay < 0)
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    else
                    {
                        shots = 0;
                        dash = false;
                    }
                }
            }
            else
            {
                attackDelay = 0;
                shots = 0;

                Vector2 idlePos = new Vector2(owner.Center.X - 75 * owner.direction, owner.Center.Y - 100);

                Vector2 toIdlePos = idlePos - Projectile.Center;
                if (toIdlePos.Length() < 0.0001f)
                    toIdlePos = Vector2.Zero;

                else
                {
                    float speed = Vector2.Distance(idlePos, Projectile.Center) * 0.15f;
                    speed = Utils.Clamp(speed, 1f, 20f);
                    toIdlePos.Normalize();
                    toIdlePos *= speed;
                }

                if (Projectile.Distance(idlePos) > 25f)
                    Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.penetrate == 2)
            {
                deathAnimation = true;
                Projectile.velocity *= -1;
                attackDelay = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            owner.Bombus().AddShake(12);
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

            for (int i = 0; i < 55; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.25f, 4.25f), 0,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.7f, 0.95f));
            }

            for (int i = 0; i < 45; ++i)
            {
                float angle2 = 6.28f * (float)i / (float)45;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 4.25f, 0, new Color(161, 31, 85), 0.85f);
            }

            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<MetalBee>(), Projectile.damage / 2, 0f, Projectile.owner);
                }

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - 0.75f) * 18f, Main.rand.NextVector2Circular(5.5f, 5.5f), Mod.Find<ModGore>(Name + "_Gore1").Type).timeLeft = 90;

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(5.5f, 5.5f), Main.rand.NextVector2Circular(5.5f, 5.5f), Mod.Find<ModGore>(Name + "_Gore2").Type).timeLeft = 90;

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(5.5f, 5.5f), Main.rand.NextVector2Circular(5.5f, 5.5f), Mod.Find<ModGore>(Name + "_Gore2").Type).timeLeft = 90;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame), Color.Lerp(new Color(213, 95, 89, 0), Color.Transparent, 1f - flashTimer / 15f), Projectile.rotation, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4 - 0.05f) * 17f - Main.screenPosition, null, new Color(161, 31, 85, 0) * MathHelper.Lerp(0.5f, 1f, 1f - flashTimer / 15f), Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4 - 0.05f) * 17f - Main.screenPosition, null, new Color(213, 95, 89, 0) * MathHelper.Lerp(0.5f, 1f, 1f - flashTimer / 15f), Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
            return false;
        }
    }
    public class SpazHoneycomb : BeeProjectile
    {
        public float flashTimer;
        public float rotTimer;
        public int dashes;
        public ref float DashDelay => ref Projectile.ai[0];
        public ref float ChargeDelay => ref Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override bool? CanDamage() => DashDelay > 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spazacomb");
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 100;
            Projectile.timeLeft = 7200;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;

            DrawOffsetX = -12;
        }

        public override void AI()
        {
            if (Main.myPlayer == owner.whoAmI && owner.HeldItem.ModItem is OcularRemote && Main.mouseRight && owner.Bombus().OcularCooldown <= 0)
                Projectile.Kill();

            if (++Projectile.frameCounter % 7 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];


            if (owner.Hymenoptra().BeeResourceCurrent <= 0)
                Projectile.Kill();

            if (flashTimer > 0)
                flashTimer--;

            if (DashDelay > 0)
                DashDelay--;

            if (rotTimer > 0)
                rotTimer--;

            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(owner.Center) < 750f).OrderBy(n => Vector2.Distance(n.Center, owner.Center)).FirstOrDefault();

            if (target != default)
            {
                if (dashes < 3)
                {
                    Vector2 targetPos = target.Center + new Vector2(250 * target.direction, 0);
                    if (DashDelay <= 0 && Projectile.Distance(target.Center) < 450f)
                    {
                        for (int i = 0; i < 25; i++)
                        {
                            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(98, 205, 123), 0.65f);

                            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(231, 215, 157), 0.5f);
                        }

                        for (int i = 0; i < Main.rand.Next(1, 4); i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<SpazBee>(), Projectile.damage * 2/3, 2f, Projectile.owner);
                        }
                        Projectile.velocity = Projectile.DirectionTo(target.Center) * 15f;
                        flashTimer = 25f;
                        dashes++;
                        DashDelay = 60;
                    }
                    
                    else if (DashDelay < 35)
                    {
                        Vector2 toIdlePos = targetPos - Projectile.Center;
                        if (toIdlePos.Length() < 0.0001f)
                            toIdlePos = Vector2.Zero;

                        else
                        {
                            float speed = Vector2.Distance(targetPos, Projectile.Center) * 0.2f;
                            speed = Utils.Clamp(speed, 3f, 16.5f);
                            toIdlePos.Normalize();
                            toIdlePos *= speed;
                        }

                        Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;
                    }

                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                else
                {
                    if (ChargeDelay < 120)
                        ChargeDelay++;

                    if (ChargeDelay > 20)
                        Projectile.velocity *= 0.955f;

                    Projectile.rotation = Projectile.DirectionTo(target.Center).ToRotation() - MathHelper.ToRadians(rotTimer) * (Projectile.Center.X < target.Center.X ? 1 : -1);

                    if (ChargeDelay < 100)
                    {
                        rotTimer = MathHelper.Lerp(0f, 20f, ChargeDelay / 100f);

                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(),
                            Projectile.rotation.ToRotationVector2().RotatedByRandom(0.55f) * Main.rand.NextFloat(2.5f, 3.5f), 0, new Color(98, 205, 123), Main.rand.NextFloat(0.25f, 0.3f));

                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(),
                            Projectile.rotation.ToRotationVector2().RotatedByRandom(0.6f) * Main.rand.NextFloat(2f, 3f), 0, new Color(231, 215, 157), Main.rand.NextFloat(0.3f, 0.35f));
                    }
                    else
                    {
                        if (Main.myPlayer == Projectile.owner && ChargeDelay == 100)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.rotation.ToRotationVector2().RotatedByRandom(0.55f) * Main.rand.NextFloat(4.5f, 6.5f), 0, new Color(98, 205, 123), Main.rand.NextFloat(0.45f, 0.6f));
                                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.rotation.ToRotationVector2().RotatedByRandom(0.6f) * Main.rand.NextFloat(4f, 6f), 0, new Color(231, 215, 157), Main.rand.NextFloat(0.6f, 0.7f));
                            }
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2() * 15f, ModContent.ProjectileType<CursedFlameball>(), Projectile.damage, 3f, Projectile.owner);

                            SoundID.Item61.PlayWith(Projectile.position, -0.2f, 0.1f);
                            Projectile.velocity += Projectile.rotation.ToRotationVector2() * -5f;

                            flashTimer = 25f;
                        }                           

                        if (ChargeDelay >= 120)
                        {
                            dashes = 0;
                            ChargeDelay = 0;
                            DashDelay = 0;
                        }
                    }
                }
            }
            else
            {
                dashes = 0;
                ChargeDelay = 0;
                DashDelay = 0;
                Vector2 idlePos = new Vector2(owner.Center.X + 75 * owner.direction, owner.Center.Y - 100);

                Vector2 toIdlePos = idlePos - Projectile.Center;
                if (toIdlePos.Length() < 0.0001f)
                    toIdlePos = Vector2.Zero;

                else
                {
                    float speed = Vector2.Distance(idlePos, Projectile.Center) * 0.2f;
                    speed = Utils.Clamp(speed, 1f, 20f);
                    toIdlePos.Normalize();
                    toIdlePos *= speed;
                }

                if (Projectile.Distance(idlePos) > 25f)
                    Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
            SoundID.Item74.PlayWith(Projectile.Center, 0.25f, 0.15f, 0.85f);
            Main.player[Projectile.owner].Bombus().AddShake(15);

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), Projectile.damage, 0f, Projectile.owner, 95);

            for (int i = 0; i < 35; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(7f, 7f), 0, new Color(98, 205, 123), 0.8f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(10.5f, 10.5f), 0, new Color(231, 215, 157), 0.7f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.55f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(98, 205, 123, 0), Color.Transparent, 1f - flashTimer / 25f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4 - 0.05f) * 8f - Main.screenPosition, null, new Color(60, 128, 88, 0) * MathHelper.Lerp(0.5f, 1f, 1f - flashTimer / 25f), Projectile.rotation, bloomTex.Size() / 2f, 0.35f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4 - 0.05f) * 8f - Main.screenPosition, null, new Color(98, 205, 123, 0) * MathHelper.Lerp(0.5f, 1f, 1f - flashTimer / 25f), Projectile.rotation, bloomTex.Size() / 2f, 0.35f, 0, 0);
            return false;
        }
    }
}
