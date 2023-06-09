namespace BombusApisBee.Projectiles
{
    public class PumpkinetScepterHoldout : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "PumpkinetScepter";
        private Player Owner => Main.player[Projectile.owner];
        public override void SafeSetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            var modPlayer = Owner.Hymenoptra();
            modPlayer.BeeResourceRegenTimer = -120;
            if (!Owner.channel || !(Owner.Hymenoptra().BeeResourceCurrent > Owner.Hymenoptra().BeeResourceReserved))
            {
                Owner.reuseDelay = 60;
                Projectile.Kill();
                return;
            }

            if (++Projectile.ai[0] % 10 == 0)
                if (!Owner.UseBeeResource(1))
                {
                    Owner.reuseDelay = 60;
                    Projectile.Kill();
                    return;
                }

            if (Main.myPlayer == Projectile.owner && Owner.ownedProjectileCounts[ModContent.ProjectileType<PumpkinetHornet>()] <= 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Main.rand.NextVector2CircularEdge(5f, 5f), ModContent.ProjectileType<PumpkinetHornet>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);

            UpdatePlayerVisuals();
        }
        public override bool? CanDamage() => false;
        private void UpdatePlayerVisuals()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
            Projectile.velocity = Utils.ToRotationVector2(Projectile.rotation);
            Projectile.Center += Utils.ToRotationVector2(Projectile.rotation) * 30f;
            Projectile.direction = (Projectile.spriteDirection = Utils.ToDirectionInt(Math.Cos((double)Projectile.rotation) > 0.0));
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = WrapAngle90Degrees(Projectile.rotation);
            Projectile.rotation += 0.7853982f;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += 1.5707964f;
        }

        private float WrapAngle90Degrees(float theta)
        {
            if (theta > MathF.PI)
                theta -= MathF.PI;

            if (theta > MathF.PI / 2f)
                theta -= MathF.PI;

            if (theta < -MathF.PI / 2f)
                theta += MathF.PI;

            return theta;
        }
    }

    class PumpkinetHornet : BeeProjectile
    {
        public Player player => Main.player[Projectile.owner];

        public NPC target;

        public ref float DashDelay => ref Projectile.ai[0];

        public ref float StingerDelay => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkinet");
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.height = 40;
            Projectile.width = 50;
            Projectile.timeLeft = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (player.channel && !player.CCed && !player.noItems && player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceReserved)
                Projectile.timeLeft = 2;

            if (DashDelay > 0)
                DashDelay--;

            if (StingerDelay > 0)
                StingerDelay--;

            if (++Projectile.frameCounter % 7 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            if (Projectile.localAI[0] == 0f)
            {
                const int Repeats = 55;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)Repeats;
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 2f, 0, new Color(225, 93, 26), 0.35f);
                }
                Projectile.localAI[0] = 1f;
            }


            Projectile.rotation = Projectile.velocity.X * 0.02f;

            if (Main.myPlayer == Projectile.owner)
            {
                target = Main.npc.Where(n => n.CanBeChasedBy(Projectile, false) && Vector2.Distance(n.Center, Main.MouseWorld) < 1000f).
                    OrderBy(n => Vector2.Distance(n.Center, Main.MouseWorld)).FirstOrDefault();
            }

            if (target != default)
                Attacking(target);
            else
                IdleMovement();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : 0;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + (Projectile.Size / 2f) + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, drawPos, sourceRect, color, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, spriteEffects, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, sourceRect, lightColor, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(209, 93, 26), 0.55f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(226, 130, 33), 0.45f);
            }
        }
        private void IdleMovement()
        {
            Vector2 idlePos = player.Center + new Vector2(-80 * player.direction, -50);
            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed;
            float inertia;
            if (distance > 450f)
            {
                speed = 18f;
                inertia = 40f;
            }
            else
            {
                speed = 12f;
                inertia = 55f;
            }

            if (distance > 25f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.25f;
                Projectile.velocity.Y = -0.1f;
            }

            if (Main.myPlayer == player.whoAmI && distance > 2000f)
            {
                Projectile.position = idlePos;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private void Attacking(NPC target)
        {
            Vector2 targetCenter = target.Center;

            Vector2 toCenter = targetCenter - Projectile.Center;
            if (toCenter.Length() < 0.0001f)
                toCenter = Vector2.Zero;

            if (DashDelay <= 0)
            {
                DashDelay = 120;
                Projectile.velocity = Projectile.DirectionTo(targetCenter) * 15f;
                for (int i = 0; i < 8; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)8;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Utils.ToRotationVector2(angle2) * 8f, ModContent.ProjectileType<PumpkinStinger>(), Projectile.damage * 2 / 3, 2f, player.whoAmI);
                }
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                const int Repeats = 55;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)Repeats;
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 3f, 0, new Color(225, 93, 26), 0.35f);
                }
            }
            else if (DashDelay <= 105)
            {
                float distance = toCenter.Length();
                float speed;
                float inertia;
                if (distance > 300)
                {
                    speed = 18f;
                    inertia = 35f;
                }
                else
                {
                    speed = 12f;
                    inertia = 55f;
                }
                if (distance > 25f)
                {
                    toCenter.Normalize();
                    toCenter *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + toCenter) / inertia;
                }


                if (StingerDelay <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(targetCenter) * 15f, ModContent.ProjectileType<PumpkinStinger>(), Projectile.damage, 2f, player.whoAmI);
                    StingerDelay = 15f;
                }
            }
        }
    }
    public class PumpkinStinger : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        private int hitDelay;
        public override bool? CanDamage() => Projectile.timeLeft < 465 && hitDelay <= 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Stinger");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 2000f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
                {
                    float num2 = Projectile.Distance(npc.Center);
                    if (num > num2)
                    {
                        num = num2;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget && Projectile.timeLeft < 465 && --hitDelay <= 0)
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 19f) / 21f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(225, 93, 26));
                dust.scale = 0.3f;
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            hitDelay = 15;
            Projectile.damage = Projectile.damage * 2 / 3;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return true;
        }
        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(240), factor => 6.5f, factor =>
            {
                return new Color(209, 93, 26) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(240), factor => 5f, factor =>
            {
                return new Color(226, 130, 33) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail2?.Render(effect);
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
