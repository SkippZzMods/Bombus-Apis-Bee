using Terraria;
namespace BombusApisBee.Projectiles
{
    class HoneyBeeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool inHitDistance;
    }

    public class HoneyBeeProj : BeeProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public bool Defense => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense;
        public bool Offense => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense;
        public bool Gathering => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering;

        public int AttackDelay;
        public int shotsFired;
        public float rotIncrease;

        public float backGlowOpacity;

        public bool slammed;
        public bool slamming;
        public int slamTimer;

        public int blockDelay;
        public int shieldFade;
        public int shieldFlashTimer;

        public int honeyTimer;
        public int oldDir;
        public float oldRot;

        public NPC attackTarget;
        public Projectile projectileTarget;

        public Vector2 IdlePos;

        public Vector2 targetPosAdd;

        private enum AttackState : int
        {
            Searching,
            Firing,
            Slamming,
        }

        private AttackState attackState = AttackState.Searching;

        public Vector2 BottleVector => Projectile.Center + new Vector2(Projectile.spriteDirection == -1 ? -20 : 15, 15).RotatedBy(Projectile.rotation);

        public override string Texture => "BombusApisBee/Items/Accessories/BeeKeeperDamageClass/HoneyBee";

        public override bool? CanDamage() => Offense;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Bee");
            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void AI()
        {
            if (Owner.Hymenoptra().HoldingBeeWeaponTimer <= 0)
                Projectile.Kill();

            if (Owner.Bombus().HoneyBee && Owner.active)
                Projectile.timeLeft = 2;

            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            IdlePos = Owner.Center + new Vector2(115 * Owner.direction, MathHelper.Lerp(-20f, -25f, Utils.Clamp((float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 2f), 0, 1)));

            if (shieldFlashTimer > 0)
                shieldFlashTimer--;

            if (Defense)
            {
                if (blockDelay > 0)
                    blockDelay--;

                if (shieldFade < 15)
                    shieldFade++;

                if (blockDelay <= 0 && projectileTarget == null)
                    projectileTarget = Main.projectile.Where(p => p.active && p.hostile && p.Distance(Owner.Center) < 150f && Owner.GetModPlayer<HoneycombCrusaderPlayer>().hitProjHitTimer[p.whoAmI] <= 0).OrderBy(p => p.Distance(Owner.Center)).FirstOrDefault();

                if (blockDelay <= 0)
                {
                    if (projectileTarget != null)
                    {
                        BeeUtils.CircleDust(Projectile.Center, 25, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3f, 0, null, 4f);

                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(projectileTarget.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), projectileTarget.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(), 0, new Color(255, 200, 20), 0.4f);
                            Dust.NewDustPerfect(projectileTarget.Center, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Main.rand.NextVector2Circular(2f, 2f), 0, default, 2f).noGravity = true;
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 pos = Vector2.Lerp(Projectile.Center, projectileTarget.Center, i * 0.05f);
                            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Vector2.Zero, 0, default, 2f).noGravity = true;
                        }

                        new SoundStyle("BombusApisBee/Sounds/Item/BiggerSplash").PlayWith(Projectile.Center, 0, 0.1f, 0.5f);

                        Projectile.velocity -= Projectile.DirectionTo(projectileTarget.Center) * 3f;


                        shieldFlashTimer = 15;

                        blockDelay = 180;

                        projectileTarget.Kill();
                        projectileTarget = null;
                    }
                    else
                    {
                        NPC target = Main.npc.Where(n => (n.CanBeChasedBy() || (NPCID.Sets.ProjectileNPC[n.type] && n.active)) && n.Distance(Owner.Center) < 150f).OrderBy(n => n.Distance(Owner.Center)).OrderBy(n => !NPCID.Sets.ProjectileNPC[n.type]).FirstOrDefault();

                        if (target != null)
                        {
                            BeeUtils.CircleDust(Projectile.Center, 25, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3f, 0, null, 4f);

                            for (int i = 0; i < 5; i++)
                            {
                                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), target.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(), 0, new Color(255, 200, 20), 0.4f);
                                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Main.rand.NextVector2Circular(2f, 2f), 0, default, 2f).noGravity = true;
                            }

                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, i * 0.05f);
                                Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Vector2.Zero, 0, default, 2f).noGravity = true;
                            }

                            Projectile.velocity -= Projectile.DirectionTo(target.Center) * 3f;


                            shieldFlashTimer = 15;

                            blockDelay = 60;

                            target.SimpleStrikeNPC(Projectile.damage / 2, target.Center.X < Owner.Center.X ? -1 : 1, Main.rand.NextBool(4), 10f, Projectile.DamageType, true, Owner.luck);
                        }
                    }
                }

                IdleMovement();

                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (Projectile.Distance(IdlePos) > 30f)
                    Projectile.spriteDirection = Projectile.direction;
                else
                    Projectile.spriteDirection = -Owner.direction;

                honeyTimer = 0;
                AttackDelay = 0;
                attackTarget = null;
                attackState = AttackState.Searching;
                rotIncrease = 0;
                slamming = false;
                slammed = false;
            }
            else if (Offense)
            {
                if (shieldFade > 0)
                    shieldFade--;

                if (attackTarget == null)
                {
                    if (attackState == AttackState.Searching)
                    {
                        attackTarget = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Owner.Center) < 1000f && Collision.CanHitLine(Owner.Center, 1, 1, n.Center, 1, 1)).OrderBy(n => n.Distance(Owner.Center)).FirstOrDefault();

                        if (attackTarget != null)
                        {
                            targetPosAdd = new Vector2(Main.rand.NextBool() ? -75f : 75f, -65f);

                            attackState = AttackState.Firing;
                        }
                    }
                }

                if (slamTimer > 0)
                {
                    backGlowOpacity = slamTimer / 60f;

                    slamTimer--;

                    Projectile.rotation = oldRot;

                    rotIncrease = MathHelper.Lerp(0f, MathHelper.TwoPi, EaseBuilder.EaseCubicOut.Ease(1f - slamTimer / 60f)) + (oldDir == -1 ? MathHelper.Pi : 0f);

                    Projectile.velocity *= 0.94f;

                    if (slamTimer == 1)
                    {
                        backGlowOpacity = 0;
                        rotIncrease = (oldDir == -1 ? MathHelper.Pi : 0f);
                        slamTimer = 0;
                        attackState = AttackState.Searching;
                        AttackDelay = 0;
                        attackTarget = null;

                        slamming = false;
                        slammed = false;
                    }

                    return;
                }

                if (attackTarget != null)
                {
                    if (!attackTarget.CanBeChasedBy())
                    {
                        attackTarget = null;
                        attackState = AttackState.Searching;
                        return;
                    }

                    switch (attackState)
                    {
                        case AttackState.Firing: FiringBehavior(); break;

                        case AttackState.Slamming: SlammingBehavior(); break;
                    }
                }
                else
                {
                    IdleMovement();
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                    if (Projectile.Distance(IdlePos) > 30f)
                        Projectile.spriteDirection = Projectile.direction;
                    else
                        Projectile.spriteDirection = -Owner.direction;

                    backGlowOpacity = 0;
                    rotIncrease = 0;
                    slamTimer = 0;
                    AttackDelay = 0;
                    slamming = false;
                    slammed = false;
                }

                honeyTimer = 0;
            }
            else if (Gathering)
            {
                if (shieldFade > 0)
                    shieldFade--;

                IdleMovement();

                if (honeyTimer > 100)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(BottleVector, DustID.Honey2, Projectile.DirectionTo(Owner.Center).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), Main.rand.Next(50, 200), default, 2f).noGravity = true;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), BottleVector,
                        Projectile.DirectionTo(Owner.Center) * 5f, ModContent.ProjectileType<BeeResourceIncreaseProjectile>(), 0, 0f, Owner.whoAmI, 0, Main.rand.Next(20, 25));

                    honeyTimer = 0;
                }
                else
                    honeyTimer++;

                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (Projectile.Distance(IdlePos) > 30f)
                    Projectile.spriteDirection = Projectile.direction;
                else
                    Projectile.spriteDirection = -Owner.direction;

                AttackDelay = 0;
                attackState = AttackState.Searching;
                rotIncrease = 0;
                attackTarget = null;
                slamming = false;
                slammed = false;
            }
        }

        private void FiringBehavior()
        {
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Math.Abs(Projectile.DirectionTo(attackTarget.Center).ToRotation()), .1f);

            rotIncrease = (Projectile.Center.X > attackTarget.Center.X ? MathHelper.Pi : 0f);

            Projectile.spriteDirection = Projectile.Center.X > attackTarget.Center.X ? -1 : 1;

            Vector2 between = (attackTarget.Center + targetPosAdd) - Projectile.Center;

            float dist = Vector2.Distance(Projectile.Center, attackTarget.Center);

            float speed = 15f;
            float adjust = 0.35f;

            dist = speed / dist;
            between.X *= dist;
            between.Y *= dist;

            if (Projectile.velocity.X < between.X)
            {
                Projectile.velocity.X += adjust;
                if (Projectile.velocity.X < 0f && between.X > 0f)
                {
                    Projectile.velocity.X += adjust * 2f;
                }
            }
            else if (Projectile.velocity.X > between.X)
            {
                Projectile.velocity.X -= adjust;
                if (Projectile.velocity.X > 0f && between.X < 0f)
                {
                    Projectile.velocity.X -= adjust * 2f;
                }
            }
            if (Projectile.velocity.Y < between.Y)
            {
                Projectile.velocity.Y += adjust;
                if (Projectile.velocity.Y < 0f && between.Y > 0f)
                {
                    Projectile.velocity.Y += adjust * 2f;
                }
            }
            else if (Projectile.velocity.Y > between.Y)
            {
                Projectile.velocity.Y -= adjust;
                if (Projectile.velocity.Y > 0f && between.Y < 0f)
                {
                    Projectile.velocity.Y -= adjust * 2f;
                }
            }

            AttackDelay++;
            if (AttackDelay > 40)
            {
                if (AttackDelay >= 60)
                {
                    Vector2 pos = Projectile.Center + new Vector2(Projectile.spriteDirection == -1 ? -20 : 15, 15).RotatedBy(Projectile.rotation + rotIncrease);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, pos.DirectionTo(attackTarget.Center).RotatedByRandom(1f) * 5f, ModContent.ProjectileType<HoneyHomingMetaballs>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Owner.whoAmI);

                    for (float k = 0; k < 6.28f; k += 0.1f)
                    {
                        float x = (float)Math.Cos(k) * 50;
                        float y = (float)Math.Sin(k) * 25;

                        Dust.NewDustPerfect(pos + pos.DirectionTo(attackTarget.Center) * 5f, ModContent.DustType<Dusts.HoneyMetaballDust>(), new Vector2(x, y).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.04f, 0, default, 1f).noGravity = true;
                    }

                    BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);

                    Projectile.velocity -= Projectile.DirectionTo(attackTarget.Center) * Main.rand.NextFloat(5f, 7f);

                    shotsFired++;
                    AttackDelay = 40;
                }
            }

            if (shotsFired >= 6)
            {
                shotsFired = 0;
                AttackDelay = 0;
                attackState = AttackState.Slamming;
            }
        }

        private void SlammingBehavior()
        {
            AttackDelay++;

            backGlowOpacity = AttackDelay / 50f;

            int sign = Math.Sign(targetPosAdd.X);

            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Math.Abs(Projectile.DirectionTo(attackTarget.Center).ToRotation()), .1f);

            rotIncrease = (Projectile.Center.X > attackTarget.Center.X ? MathHelper.Pi : 0f);

            Projectile.spriteDirection = Projectile.Center.X > attackTarget.Center.X ? -1 : 1;

            Vector2 between = (attackTarget.Center + targetPosAdd + Vector2.Lerp(Vector2.Zero, new Vector2(35f * sign, -25f), AttackDelay / 50f)) - Projectile.Center;

            float dist = Vector2.Distance(Projectile.Center, attackTarget.Center);

            float speed = 15f;
            float adjust = 0.35f;

            if (dist < 10f)
                speed = 1f;

            if (AttackDelay >= 50f)
            {
                between = attackTarget.Center - Projectile.Center;
                slamming = true;
                speed = 25f;
                adjust = 0.5f;
            }

            dist = speed / dist;
            between.X *= dist;
            between.Y *= dist;

            if (Projectile.velocity.X < between.X)
            {
                Projectile.velocity.X += adjust;
                if (Projectile.velocity.X < 0f && between.X > 0f)
                {
                    Projectile.velocity.X += adjust * 2f;
                }
            }
            else if (Projectile.velocity.X > between.X)
            {
                Projectile.velocity.X -= adjust;
                if (Projectile.velocity.X > 0f && between.X < 0f)
                {
                    Projectile.velocity.X -= adjust * 2f;
                }
            }
            if (Projectile.velocity.Y < between.Y)
            {
                Projectile.velocity.Y += adjust;
                if (Projectile.velocity.Y < 0f && between.Y > 0f)
                {
                    Projectile.velocity.Y += adjust * 2f;
                }
            }
            else if (Projectile.velocity.Y > between.Y)
            {
                Projectile.velocity.Y -= adjust;
                if (Projectile.velocity.Y > 0f && between.Y < 0f)
                {
                    Projectile.velocity.Y -= adjust * 2f;
                }
            }
        }

        private void IdleMovement()
        {
            Vector2 toIdlePos = IdlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
            {
                toIdlePos = Vector2.Zero;
            }
            else
            {
                float speed = Vector2.Distance(IdlePos, Projectile.Center) * 0.2f;
                speed = Utils.Clamp(speed, 1f, 20f);
                if (Defense)
                    speed = Utils.Clamp(speed, 0.5f, 30f);

                toIdlePos.Normalize();
                toIdlePos *= speed;
            }

            Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

            if (Vector2.Distance(Projectile.Center, IdlePos) > 2000f)
            {
                Projectile.Center = IdlePos;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Offense && slamming)
            {
                slammed = true;
                slamTimer = 60;
                Projectile.velocity -= Projectile.velocity.RotatedByRandom(0.3f) * 2f;
                oldDir = Projectile.direction;
                oldRot = Projectile.rotation;

                for (int i = 0; i < 15; i++)
                {
                    Vector2 pos = target.Center + target.Center.DirectionTo(Projectile.Center) * (target.width * 0.5f);
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center).RotatedByRandom(0.45f) * Main.rand.NextFloat(1f, 6f), 0, new Color(250, 200, 0), 0.65f);
                }

                for (float k = 0; k < 6.28f; k += 0.1f)
                {
                    float x = (float)Math.Cos(k) * 50;
                    float y = (float)Math.Sin(k) * 25;

                    Dust.NewDustPerfect(target.Center + target.Center.DirectionTo(Projectile.Center) * (target.width * 0.5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), new Vector2(x, y).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.05f, 0, new Color(255, 150, 0), 0.65f);
                }

                SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center, .25f, .1f, 0.9f);
                Owner.Bombus().AddShake(10);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;

            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            lightColor = lightColor * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

            float mult = (1f - blockDelay / 180f) * (shieldFade / 15f) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

            if (honeyTimer > 0)
            {
                Main.spriteBatch.Draw(glowTex, BottleVector - new Vector2(Projectile.spriteDirection == -1 ? -20 : 15, 5) - Main.screenPosition, null, (new Color(255, 150, 20, 0) * (honeyTimer / 100f)) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f), 0f, glowTex.Size() / 2f, 0.6f, 0f, 0f);
            }

            if (Offense)
            {
                if (backGlowOpacity > 0)
                {
                    Rectangle glowFrame = texGlow.Frame(verticalFrames: 4, frameY: Projectile.frame);
                    Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, new Color(250, 170, 20, 0) * backGlowOpacity * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f), Projectile.rotation + rotIncrease, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

                    Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 20, 0) * backGlowOpacity * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f), 0f, glowTex.Size() / 2f, 1.25f, 0f, 0f);
                }

                if (slamming)
                {
                    Rectangle afterImageFrame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);

                    float fadeout = 1f;
                    if (slamTimer < 30 && slammed)
                    {
                        fadeout = slamTimer / 30f;
                    }

                    for (int k = 0; k < Projectile.oldPos.Length; k++)
                    {
                        Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2f;
                        Color color = new Color(250, 200, 0, 0) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                        Main.spriteBatch.Draw(tex, drawPos, afterImageFrame, color * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f) * fadeout, Projectile.rotation + rotIncrease, afterImageFrame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.35f, (k / (float)Projectile.oldPos.Length)), Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    }
                }
            }

            if (shieldFade > 0)
            {
                Rectangle glowFrame = texGlow.Frame(verticalFrames: 4, frameY: Projectile.frame);
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, new Color(250, 170, 20, 0) * mult, Projectile.rotation + rotIncrease, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 20, 0) * mult, 0f, glowTex.Size() / 2f, 0.8f, 0f, 0f);
            }

            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation + rotIncrease, frame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            if (shieldFlashTimer > 0)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * MathHelper.Lerp(0f, 1f, shieldFlashTimer / 15f), Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, shieldFlashTimer / 15f), Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            if (shieldFade > 0)
            {
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["HoneyShieldShader"].GetShader().Shader;
                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
                effect.Parameters["blowUpPower"].SetValue(3f);
                effect.Parameters["blowUpSize"].SetValue(1f);

                float noiseScale = MathHelper.Lerp(0.45f, 0.65f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.1f) + 1f);
                effect.Parameters["noiseScale"].SetValue(noiseScale);
                float opacity = 0.35f * mult;
                effect.Parameters["shieldOpacity"].SetValue(opacity);
                effect.Parameters["shieldEdgeColor"].SetValue((new Color(255, 200, 20) * mult).ToVector3());
                effect.Parameters["shieldEdgeBlendStrenght"].SetValue(5f);

                effect.Parameters["shieldColor"].SetValue((new Color(255, 100, 20) * mult).ToVector3());

                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["power"].SetValue(0.15f);
                effect.Parameters["offset"].SetValue(new Vector2(Main.screenPosition.X / Main.screenWidth * 0.5f, 0));
                effect.Parameters["speed"].SetValue(15f);

                Texture2D noise = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value;
                Vector2 pos = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(noise, pos, null, Color.White, 0f, noise.Size() / 2f, 0.16f, 0, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 20, 0) * mult * 0.5f, 0f, glowTex.Size() / 2f, 1f, 0f, 0f);
            }

            return false;
        }
    }
}
