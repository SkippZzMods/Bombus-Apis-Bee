﻿using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace BombusApisBee.BeeDamageClass
{
    public class BeeDamagePlayer : ModPlayer
    {
        public enum BeeState : int
        {
            Defense = 1,
            Offense = 2,
            Gathering = 3,
        }

        public const int DefaultBeeResourceMax = 100;

        public int BeeResourceCurrent = 100;
        public int BeeResourceMax = 100;
        public int BeeResourceMax2;

        public int BeeResourceReserved;
        public int BeeResourceReservedIncrease;

        public int BeeResourceRegenTimer;
        public int BeeResourceIncrease = 4;

        public float ResourceChanceAdd = 0f;

        public const int DefaultBees = 3;
        public int CurrentBees = DefaultBees;

        public int CurrentBeeState = 1;

        public bool HoneyShield = true;
        public bool JustShielded;
        public int HoneyShieldCD;
        public int MaxHoneyShieldCD = 1200;
        public int HoneyImmuneTimer;

        public int GatheringIncrease = 2;

        public int HoldingBeeWeaponTimer;
        public int HeldBeeWeaponTimer;

        public int StateSwitchCooldown;

        public float BeeStrengthenChance;

        public bool HasBees => Player.ownedProjectileCounts<BeePlayerBeeProjectile>() > 0;

        public static BeeDamagePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<BeeDamagePlayer>();
        }

        public override void Initialize()
        {
            BeeResourceMax = DefaultBeeResourceMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            BeeResourceCurrent = (int)(BeeResourceMax2 * 0.75f);
            ResetVariables();
        }

        private void ResetVariables()
        {
            JustShielded = false;
            HoneyShield = false;
            BeeResourceIncrease = 4;
            GatheringIncrease = 10;
            ResourceChanceAdd = 0f;
            BeeResourceMax2 = BeeResourceMax;
            CurrentBees = DefaultBees;

            if (HeldBeeWeaponTimer > 0)
                HeldBeeWeaponTimer--;

            BeeResourceReserved = 0;
            BeeStrengthenChance = 0;
        }

        public override void UpdateEquips()
        {
            if (Player.strongBees)
                BeeStrengthenChance += 0.5f;
        }

        public override void PostUpdateMiscEffects()
        {
            if (StateSwitchCooldown > 0)
                StateSwitchCooldown--;

            if (++BeeResourceRegenTimer >= 60)
            {
                Player.IncreaseBeeResource(BeeResourceIncrease, false);
                BeeResourceRegenTimer = 0;
            }

            BeeResourceCurrent = Utils.Clamp(BeeResourceCurrent, 0, BeeResourceMax2);

            BeeResourceReserved = Utils.Clamp(BeeResourceReserved, 0, BeeResourceMax2);

            if (HasBees)
            {
                if (CurrentBeeState == (int)BeeState.Defense)
                {
                    UpdateDefense();
                }
                else if (CurrentBeeState == (int)BeeState.Offense)
                {
                    UpdateOffense();
                }
                else if (CurrentBeeState == (int)BeeState.Gathering)
                {
                    UpdateGathering();
                }

                if (HoneyShieldCD > 0)
                    HoneyShieldCD--;
                else
                    HoneyShield = true;

                if (HoneyImmuneTimer > 0)
                    HoneyImmuneTimer--;

                if (HoneyShieldCD == 1)
                {
                    new SoundStyle("BombusApisBee/Sounds/Item/Ricochet").PlayWith(Player.Center, 0, 0.1f, 1f);

                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.UnitY * -5f, ModContent.ProjectileType<BeePlayerShieldProjectile>(), 0, 0f, Player.whoAmI);
                }
            }

            if (Player.HeldItem.CountsAsClass<HymenoptraDamageClass>())
                HeldBeeWeaponTimer = 900;
            else if (HoldingBeeWeaponTimer > 0 && HeldBeeWeaponTimer <= 0)
                HoldingBeeWeaponTimer--;

            if (HeldBeeWeaponTimer > 0)
            {
                if (Player.ownedProjectileCounts<BeePlayerBeeProjectile>() < CurrentBees)
                    Projectile.NewProjectile(Player.GetSource_ReleaseEntity("BombusApisBee: Spawn Player Bee"), Player.Center + Main.rand.NextVector2Circular(50f, 50f),
                        Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<BeePlayerBeeProjectile>(), 10, 0f, Player.whoAmI);

                if (HoldingBeeWeaponTimer < 15)
                    HoldingBeeWeaponTimer++;
            }

            UpdateHoneyShader();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BombusApisBee.BeekeeperStateSwitchHotkey.JustPressed && StateSwitchCooldown <= 0)
            {
                if (CurrentBeeState < 3)
                    CurrentBeeState++;
                else
                    CurrentBeeState = 1;

                StateSwitchCooldown = 20;
            }
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (CurrentBeeState == (int)BeeState.Defense)
            {
                if (HoneyShield)
                {
                    MaxHoneyShieldCD = Utils.Clamp(1200 - (100 * CurrentBees), 300, 1200);
                    HoneyShieldCD = MaxHoneyShieldCD;
                    JustShielded = true;
                    HoneyShield = false;
                    HoneyImmuneTimer = 15;

                    SoundEngine.PlaySound(SoundID.Splash with { Volume = 2f });

                    for (int i = 0; i < 25; i++)
                    {
                        Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Honey2, 0f, 0f, 25, default, 1.25f).velocity *= 0.25f;

                        Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Honey2, 0f, 0f, 90, default, 1.25f).velocity *= 0.25f;
                    }

                    BeeUtils.CircleDust(Player.Center, 50, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3.5f, 0, null, 2.5f);
                }

                if (HoneyImmuneTimer > 0)
                    return true;
            }

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }

        private void UpdateDefense()
        {
            Player.statDefense += CurrentBees * 2;

            if (HoneyShieldCD == 1)
                BeeUtils.CircleDust(Player.Center, 50, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3.75f, 0, null, 2f);
        }

        private void UpdateOffense()
        {
            Player.IncreaseBeeCrit(CurrentBees);
            Player.IncreaseBeeDamage(CurrentBees * 0.02f);
        }

        private void UpdateGathering()
        {
            BeeResourceIncrease += 4;
            Player.IncreaseBeeDamage(-0.15f);
        }

        private void UpdateHoneyShader()
        {
            if (Main.dedServ)
                return;

            if (CurrentBeeState == (int)BeeState.Defense)
            {
                if (!Filters.Scene["CircleDistort"].Active)
                    Filters.Scene.Activate("CircleDistort", Player.Center);
                else
                {
                    float power = MathHelper.Lerp(0f, 0.00035f, (1f - HoneyShieldCD / (float)MaxHoneyShieldCD)) * HoldingBeeWeaponTimer / 15f;
                    Filters.Scene["CircleDistort"].GetShader().
                        UseProgress((float)Main.timeForVisualEffects * 0.001f).
                        UseColor(1f, power, power).
                        UseImage(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value, 1).
                        UseImage(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value, 2).
                        UseTargetPosition(Player.Center + Player.velocity + new Vector2(0f, Player.gfxOffY));
                }
            }
            else
            {
                Filters.Scene["CircleDistort"].GetShader().
                        UseColor(1f, 0f, 0f);

                Filters.Scene.Deactivate("CircleDistort");
            }
        }
    }

    class BeePlayerShieldProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 20;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor *= Projectile.timeLeft / 60f;

            return base.PreDraw(ref lightColor);
        }
    }

    public class BeePlayerBeeProjectile : ModProjectile
    {
        public Player Player => Main.player[Projectile.owner];

        public bool Defense => Player.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense;
        public bool Offense => Player.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense;
        public bool Gathering => Player.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering;

        public int RandTimer;
        public Vector2 RandOffset;
        public Vector2 IdlePos;

        public int AttackDelay;
        public int maxAttackDelay;
        public bool Attacking;
        public NPC attackTarget;

        public int FrameOffset;

        int enemyWhoAmI;
        bool stuck = false;
        Vector2 offset = Vector2.Zero;

        public int HoneyTimer;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        float mult;

        float lerper;

        public delegate void ExtraAI(Projectile proj);
        public static event ExtraAI ExtraAIEvent;

        public delegate void ExtraPreAI(Projectile proj);
        public static event ExtraPreAI ExtraPreAIEvent;

        public int waspAttackCooldown = 90;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee");

            Main.projFrames[Type] = 8;

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void Unload()
        {
            ExtraPreAIEvent = null;
            ExtraAIEvent = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.DamageType = BeeUtils.BeeDamageClass();
        }

        public override bool PreAI()
        {
            NPC target = Main.npc[enemyWhoAmI];
            if (Gathering && stuck)
            {
                Projectile.position = target.position + offset;
                if (!target.active)
                {
                    stuck = false;
                }

                target.GetGlobalNPC<BeeDamagePlayerNPC>().BeeAmount++;

                if (Main.rand.NextBool(15))
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f) + Projectile.velocity, DustID.Honey2, Vector2.Zero, Main.rand.Next(100), default, 0.75f);

                if (++HoneyTimer % 150 == 0)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Projectile.DirectionTo(Player.Center) * 10f, ModContent.ProjectileType<Projectiles.BeeResourceIncreaseProjectile>(), 0, 0f, Player.whoAmI, 0, Player.Hymenoptra().GatheringIncrease).tileCollide = false;

                if (Player.Hymenoptra().HoldingBeeWeaponTimer <= 0)
                    Projectile.Kill();

                if (Player.Hymenoptra().HeldBeeWeaponTimer > 0 || Player.Hymenoptra().HoldingBeeWeaponTimer > 0)
                    Projectile.timeLeft = 2;

                Projectile.frame = FrameOffset + 1;

                ExtraPreAIEvent.Invoke(Projectile);

                return false;
            }

            return true;
        }

        public override void AI()
        {
            if (RandTimer > 0)
                RandTimer--;

            if (RandTimer <= 0)
            {
                RandOffset = Main.rand.NextVector2Circular(50f, 50f);
                RandTimer = 15;
            }

            IdlePos = Player.Center + RandOffset;

            if (Player.Hymenoptra().HoldingBeeWeaponTimer <= 0 || Player.dead || Player.ownedProjectileCounts<BeePlayerBeeProjectile>() > Player.Hymenoptra().CurrentBees)
                Projectile.Kill();

            if (Player.Hymenoptra().HeldBeeWeaponTimer > 0 || Player.Hymenoptra().HoldingBeeWeaponTimer > 0)
                Projectile.timeLeft = 2;

            if (Defense)
            {
                Projectile.friendly = true;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 12;

                FrameOffset = 0;

                ResetVariables();

                Projectile.rotation = Projectile.velocity.X * 0.085f;
                Projectile.spriteDirection = Projectile.direction;

                IdleMovement();

                if (Player.HeldItem.damage > 0)
                    Projectile.damage = Player.ApplyHymenoptraDamageTo(5 + (int)(Player.HeldItem.damage * 0.5f));
                else
                    Projectile.damage = Player.ApplyHymenoptraDamageTo(10);

                if (Player.Hymenoptra().JustShielded)
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(255, 205, 0), 0.45f);
                    }

                if (Player.Hymenoptra().HoneyShieldCD == 1)
                    BeeUtils.CircleDust(Projectile.Center, 10, ModContent.DustType<Dusts.GlowFastDecelerate>(), 1f, 0, new Color(255, 205, 0), 0.5f);

                mult = 1f;
                if (Projectile.Distance(Player.Center) > 150 && Projectile.Distance(Player.Center) < 450)
                    mult = MathHelper.Lerp(0f, 1f, 1f - (Projectile.Distance(Player.Center) - 150) / 300f);
                else if (Projectile.Distance(Player.Center) > 450)
                    mult = 0f;

                lerper = 1f - Player.Hymenoptra().HoneyShieldCD / (float)Player.Hymenoptra().MaxHoneyShieldCD;

                if (!Main.dedServ)
                {
                    ManageCaches();
                    ManageTrail();
                }
            }
            else if (Offense)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.usesIDStaticNPCImmunity = false;

                FrameOffset = 0;

                ResetVariables();

                Projectile.rotation = Projectile.velocity.X * 0.085f;
                Projectile.spriteDirection = Projectile.direction;

                if (AttackDelay <= 0 && attackTarget == null)
                    attackTarget = FindTarget();

                if (AttackDelay > 0 && Projectile.Distance(Player.Center) < 50f)
                    AttackDelay--;

                if (AttackDelay == 1)
                    if (Player.HeldItem.damage > 0)
                        Projectile.damage = Player.ApplyHymenoptraDamageTo((int)(Player.HeldItem.damage * 0.65f));
                    else
                        Projectile.damage = Player.ApplyHymenoptraDamageTo(10);

                if (attackTarget != null && AttackDelay <= 0)
                {
                    Projectile.friendly = true;
                    Attacking = true;

                    Vector2 between = attackTarget.Center - Projectile.Center;

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
                }
                else
                {
                    Projectile.friendly = false;
                    Attacking = false;
                    IdleMovement();
                }
            }
            else if (Gathering)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.usesIDStaticNPCImmunity = false;
                Projectile.friendly = true;

                if (AttackDelay <= 0 && attackTarget == null)
                    attackTarget = FindTarget();

                if (AttackDelay > 0 && Projectile.Distance(Player.Center) < 50f)
                    AttackDelay--;

                if (AttackDelay == 1)
                    if (Player.HeldItem.damage > 0)
                    {
                        Projectile.damage = Player.ApplyHymenoptraDamageTo(Player.HeldItem.damage / 3);
                        if (Projectile.damage < 6)
                            Projectile.damage = 6;
                    }
                    else
                        Projectile.damage = Player.ApplyHymenoptraDamageTo(10);

                if (attackTarget != null && AttackDelay <= 0)
                {
                    if (Main.rand.NextBool(5))
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, Main.rand.NextBool() ? new Color(255, 125, 0) : new Color(255, 200, 0), 0.25f);

                    FrameOffset = 4;
                    Attacking = true;

                    Vector2 between = attackTarget.Center - Projectile.Center;

                    float dist = Vector2.Distance(Projectile.Center, attackTarget.Center);

                    float speed = 15f;
                    float adjust = 0.5f;

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

                    Projectile.rotation = Projectile.DirectionTo(attackTarget.Center).ToRotation();
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.X * 0.085f;
                    Projectile.spriteDirection = Projectile.direction;

                    FrameOffset = 0;
                    Projectile.friendly = false;
                    Attacking = false;
                    IdleMovement();
                }
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4 + FrameOffset)
                {
                    Projectile.frame = FrameOffset;
                }
            }

            if (attackTarget != null && !attackTarget.active)
                attackTarget = null;

            ExtraAIEvent?.Invoke(Projectile);
        }
        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 15; i++)
            {
                cache.Add(Vector2.Lerp(Projectile.Center + Projectile.velocity, Player.Center, i / 15f));
            }

            cache.Add(Player.Center);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 16, new TriangularTip(12), factor => 3f, factor =>
            {
                return (Color.Lerp(Color.Transparent, new Color(255, 50, 20, 0) * mult, lerper) * (Player.Hymenoptra().HoldingBeeWeaponTimer / 15f));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Player.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 16, new TriangularTip(12), factor => 6f, factor =>
            {
                return (Color.Lerp(Color.Transparent, new Color(255, 200, 0, 0) * 0.25f * mult, lerper) * (Player.Hymenoptra().HoldingBeeWeaponTimer / 15f));
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Player.Center;
        }

        private void DrawTrail(SpriteBatch spriteBatch)
        {
            if (Defense)
            {
                spriteBatch.End();
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
                effect.Parameters["repeats"].SetValue(1);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
                trail?.Render(effect);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
                trail2?.Render(effect);
                trail?.Render(effect);

                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        private void ResetVariables()
        {
            enemyWhoAmI = 0;
            stuck = false;
            offset = Vector2.Zero;
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
                float speed = Vector2.Distance(IdlePos, Projectile.Center) * 0.25f;
                speed = Utils.Clamp(speed, 1f, 25f);
                if (Defense)
                    speed = Utils.Clamp(speed, 0.5f, 35f);

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

        private NPC FindTarget() { return Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Player.Center) < 1000f && Collision.CanHitLine(Player.Center, 1, 1, n.Center, 1, 1)).OrderBy(n => n.Distance(Player.Center)).FirstOrDefault(); }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Attacking)
            {
                if (Offense)
                {
                    if (Player.HeldItem.damage > 0)
                    {
                        AttackDelay = Player.HeldItem.damage;
                        maxAttackDelay = AttackDelay;
                    }
                    else
                    {
                        AttackDelay = 30;
                        maxAttackDelay = AttackDelay;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), (-Projectile.velocity * Main.rand.NextFloat(0.2f)).RotatedByRandom(0.55f), 0, Main.rand.NextBool() ? new Color(255, 125, 0) : new Color(255, 200, 0), 0.45f);
                    }

                    attackTarget = null;
                }

                if (Gathering)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, (-Projectile.velocity * Main.rand.NextFloat(0.2f)).RotatedByRandom(0.55f), 0, default, 1.45f).noGravity = true;
                    }

                    AttackDelay = 120;
                }
            }


            if (Gathering)
            {
                if (!stuck && target.life > 0)
                {
                    stuck = true;
                    Projectile.friendly = false;
                    Projectile.tileCollide = false;
                    enemyWhoAmI = target.whoAmI;
                    offset = Projectile.position - target.position;
                    offset -= Projectile.velocity;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(Main.spriteBatch);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Rectangle frame = tex.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

            lightColor = lightColor * (Player.Hymenoptra().HoldingBeeWeaponTimer / 15f);

            if (Defense)
            {
                Color glowColor = Color.Lerp(Color.Transparent, new Color(255, 125, 0, 0), 1f - Player.Hymenoptra().HoneyShieldCD / (float)Player.Hymenoptra().MaxHoneyShieldCD) * (Player.Hymenoptra().HoldingBeeWeaponTimer / 15f);

                Rectangle glowFrame = texGlow.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

                Main.spriteBatch.Draw(glowTex, Player.Center - Main.screenPosition, null, glowColor * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0f, 0f);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, glowTex.Size() / 2f, 0.35f, 0f, 0f);

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
            }

            if (Offense)
            {
                Color glowColor = Color.Lerp(Color.Transparent, new Color(255, 125, 0, 0), 1f - AttackDelay / (float)maxAttackDelay) * (Player.Hymenoptra().HoldingBeeWeaponTimer / 15f);

                Rectangle glowFrame = texGlow.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, glowTex.Size() / 2f, 0.25f, 0f, 0f);

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0);
                }
            }

            if (Gathering)
            {
                if (stuck)
                {
                    Color glowColor = Color.Lerp(Color.Transparent, new Color(255, 125, 0, 0) * 0.5f, (float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 3f));

                    Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, glowTex.Size() / 2f, 0.35f, 0f, 0f);
                }

                if (Attacking)
                {
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, new Color(255, 125, 0, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                            Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0);
                    }
                }
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
            return false;
        }
    }

    class BeeDamagePlayerNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int BeeAmount;

        public override void PostAI(NPC npc)
        {
            BeeAmount = 0;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (BeeAmount <= 0)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= BeeAmount * 10;

            if (damage < 10)
                damage = 10;
        }
    }
}
