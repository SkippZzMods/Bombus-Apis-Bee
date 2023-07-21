using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyphyteMask : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 75");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 7;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneyphyteGreaves>() && body.type == ModContent.ItemType<HoneyphyteChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Strike enemies to build up honey energy\nConjures a honeycomb to aid you\nDouble tap " + (Main.ReversedUpDownArmorSetBonuses ? "UP " : "DOWN ") + "while at full energy to fire a concentrated honey laser from the honeycomb\nIncreases hymenoptra damage by 10% while your Bees are in Attacking mode\nIncreases damage reduction by 10% while your Bees are in Defense mode";
            var modPlayer = player.Hymenoptra();
            player.Bombus().HoneyLaser = true;

            if (modPlayer.HasBees)
            {
                if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
                {
                    player.IncreaseBeeDamage(0.1f);
                }
                else if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
                {
                    player.endurance += 0.1f;
                }
            }

            if (player.ownedProjectileCounts<HoneyphyteMaskLaserHoneycomb>() <= 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<HoneyphyteMaskLaserHoneycomb>(), 50, 1f, player.whoAmI);

                proj.originalDamage = 50;
            }
        }
        
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.10f);
            player.IncreaseBeeCrit(10);
            player.Hymenoptra().BeeResourceMax2 += 75;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    class HoneyphyteMaskLaserHoneycomb : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        public bool pulsed;
        public int pulseTimer;

        public Vector2 rotationalVelocity;

        public Vector2 LaserStart => Projectile.Center + Vector2.One.RotatedBy(rotationalVelocity.ToRotation() - MathHelper.PiOver4) * 20f - Projectile.velocity;

        public Vector2 LaserEnd => Projectile.Center + Vector2.One.RotatedBy(rotationalVelocity.ToRotation() - MathHelper.PiOver4) * 750f;

        public ref float LaserTimer => ref Projectile.ai[0];
        public ref float StartupTimer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public float Charge => Owner.Bombus().HoneyLaserCharge;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyphytecomb");
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(30);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            DoIdleMovement();

            UpdateProjectileLifetime();
            rotationalVelocity = Vector2.Lerp(rotationalVelocity, Projectile.DirectionTo(Main.MouseWorld), 0.025f);

            if (LaserTimer > 0)
                UpdateLaser();

            if (!pulsed && Charge >= BombusApisBeePlayer.HONEY_LASER_CHARGE_MAX)
            {
                pulsed = true;
                pulseTimer = 15;

                SoundID.MaxMana.PlayWith(Projectile.Center);

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(200, 100, 20), 1f);
                }
            }

            if (pulseTimer > 0)
                pulseTimer--;

            if (StartupTimer > 0)
            {
                StartupTimer--;

                float lerper = EaseBuilder.EaseCubicInOut.Ease(StartupTimer / 45f);

                Vector2 pos = Projectile.Center + Vector2.One.RotatedBy(rotationalVelocity.ToRotation() - MathHelper.PiOver4) * 17.5f - Projectile.velocity;

                for (int i = 0; i < 4; i++)
                {   
                    Dust.NewDustPerfect(pos + Vector2.One.RotatedBy(1.57f * i).RotatedBy(6.28f * lerper) * 50f * lerper, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, 0, new Color(255, 150, 20), 0.35f);
                }

                if (StartupTimer == 1)
                {
                    LaserTimer = 600;
                    Owner.Bombus().AddShake(20);

                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(200, 100, 20), 0.5f);
                        Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), 100, default, 1.25f);
                        Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(6f, 6f), 50, default, 1.25f).noGravity = true;
                    }

                    SoundID.Splash.PlayWith(Projectile.Center, 0, 0, 2);
                }
            }

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowFastDecelerate>(), target.DirectionTo(LaserStart).RotatedByRandom(0.5f) * Main.rand.NextFloat(2f), 0, new Color(255, 150, 20), 0.5f);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (LaserTimer > 0)
            {
                float useless = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), LaserStart, LaserEnd, 20 * TrailFade(), ref useless);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(Main.spriteBatch);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texBlur = ModContent.Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;

            Texture2D laserTex = ModContent.Request<Texture2D>(Texture + "_LaserStart").Value;
            Texture2D laserTexGlow = ModContent.Request<Texture2D>(Texture + "_LaserStart_Glow").Value;
            Texture2D laserTexBlur = ModContent.Request<Texture2D>(Texture + "_LaserStart_Blur").Value;

            float chargeProgress = Charge / BombusApisBeePlayer.HONEY_LASER_CHARGE_MAX;

            if (LaserTimer > 0) 
                chargeProgress = TrailFade() * 1.25f;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 20, 0) * chargeProgress, Projectile.rotation, bloomTex.Size() / 2f, 0.75f, 0f, 0f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 20, 0) * chargeProgress, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale * 1.15f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * chargeProgress, Projectile.rotation, texBlur.Size() / 2f, Projectile.scale, 0f, 0f);

            Vector2 laserPosition = LaserStart - Main.screenPosition + Main.rand.NextVector2Circular((LaserTimer > 0 ? 1.5f : 1f) * chargeProgress, (LaserTimer > 0 ? 1.5f : 1f) * chargeProgress);

            Main.spriteBatch.Draw(laserTexGlow, laserPosition, null, new Color(255, 150, 20, 0) * chargeProgress, rotationalVelocity.ToRotation(), laserTexGlow.Size() / 2f, Projectile.scale * 1.15f, 0f, 0f);

            Main.spriteBatch.Draw(laserTex, laserPosition, null, Color.White, rotationalVelocity.ToRotation(), laserTex.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(laserTexBlur, laserPosition, null, new Color(255, 255, 255, 0) * chargeProgress, rotationalVelocity.ToRotation(), laserTexBlur.Size() / 2f, Projectile.scale, 0f, 0f);

            if (LaserTimer > 0)
            {
                laserPosition = LaserEnd - Main.screenPosition;

                Main.spriteBatch.Draw(bloomTex, laserPosition, null, new Color(200, 100, 20, 0) * chargeProgress, Projectile.rotation, bloomTex.Size() / 2f, 1f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, laserPosition, null, new Color(255, 100, 50, 0) * chargeProgress, Projectile.rotation, bloomTex.Size() / 2f, 0.75f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, laserPosition, null, new Color(255, 200, 20, 0) * chargeProgress, Projectile.rotation, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, laserPosition, null, new Color(255, 255, 255, 0) * chargeProgress, Projectile.rotation, bloomTex.Size() / 2f, 0.35f, 0f, 0f);
            }

            if (pulseTimer > 0)
            {
                Main.spriteBatch.Draw(starTex, LaserStart - Main.screenPosition, null, new Color(200, 100, 20, 0) * (pulseTimer / 15f), pulseTimer * 0.05f, starTex.Size() / 2f, 0.25f, 0f, 0f);
                Main.spriteBatch.Draw(starTex, LaserStart - Main.screenPosition, null, new Color(255, 200, 20, 0) * (pulseTimer / 15f), pulseTimer * 0.05f, starTex.Size() / 2f, 0.2f, 0f, 0f);
            }

            return false;
        }

        public void ActivateLaser()
        {
            StartupTimer = 45;
        }

        internal void UpdateLaser()
        {
            Projectile.velocity -= Projectile.DirectionTo(LaserEnd) * 0.3f * TrailFade();
            LaserTimer--;

            Owner.Bombus().HoneyLaserCharge = (int)MathHelper.Lerp(BombusApisBeePlayer.HONEY_LASER_CHARGE_MAX, 0f, 1f - LaserTimer / 600f);

            Dust.NewDustPerfect(LaserStart, ModContent.DustType<GlowFastDecelerate>(), LaserStart.DirectionTo(LaserEnd).RotatedByRandom(1f) * Main.rand.NextFloat(5f), 0, new Color(255, 150, 20), 0.65f * TrailFade());

            if (Main.rand.NextBool())
            {
                Dust.NewDustPerfect(Vector2.Lerp(LaserStart, LaserEnd, Main.rand.NextFloat()), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(255, 150, 20), 0.75f * TrailFade());

                Dust.NewDustPerfect(Vector2.Lerp(LaserStart, LaserEnd, Main.rand.NextFloat()), DustID.Honey2, Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(200), default, 1.25f * TrailFade()).noGravity = true;
            }

            if (Owner.Bombus().shakeTimer < 2)
                Owner.Bombus().AddShake(2, false);

            if (LaserTimer % 5 == 0)
            {
                SoundID.Item13.PlayWith(Projectile.Center, 0.25f, 0.15f, 1.15f);
                SoundID.SplashWeak.PlayWith(Projectile.Center, 0.25f, 0.15f, 1.15f);
            }

            if (LaserTimer % 7 == 0)
            {
                SoundID.Item60.PlayWith(Projectile.Center, 0.25f, 0.15f, 0.75f);
            }

            if (LaserTimer == 1)
            {
                pulsed = false;
            }            
        }

        internal void DoIdleMovement()
        {
            Vector2 idlePos = Owner.Center + new Vector2(0f, -50);

            float dist = Vector2.Distance(Projectile.Center, idlePos);

            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
            {
                toIdlePos = Vector2.Zero;
            }
            else
            {
                float speed = 35f;
                if (dist < 1000f)
                    speed = MathHelper.Lerp(20f, 45f, dist / 1000f);

                if (dist < 100f)
                    speed = MathHelper.Lerp(0.15f, 20f, dist / 100f);

                toIdlePos.Normalize();
                toIdlePos *= speed; 
            }

            Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

            if (dist > 2000f)
            {
                Projectile.Center = idlePos;
                Projectile.velocity = Vector2.Zero + Main.rand.NextVector2Circular(5f, 5f);
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        internal void UpdateProjectileLifetime()
        {
            if (Owner.Bombus().HoneyLaser)
                Projectile.timeLeft = 2;
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 20; i++)
            {
                cache.Add(Vector2.Lerp(LaserStart, LaserEnd, i / 20f) + Projectile.velocity);
            }
            cache.Add(LaserEnd);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 21, new TriangularTip(12), factor => 15f * TrailFade(), factor =>
            {
                return new Color(200, 100, 0) * TrailFade();
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = LaserEnd;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 21, new TriangularTip(12), factor => 10f * TrailFade(), factor =>
            {
                return new Color(255, 255, 50) * TrailFade();
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = LaserEnd;

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 21, new TriangularTip(12), factor => 5f * TrailFade(), factor =>
            {
                return new Color(255, 255, 255) * TrailFade();
            });

            trail3.Positions = cache.ToArray();
            trail3.NextPosition = LaserEnd;
        }

        private void DrawTrail(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.03f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail3?.Render(effect);

            spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }

        private float TrailFade()
        {
            if (LaserTimer > 590)
                return 1f - (LaserTimer - 590) / 10f;

            if (LaserTimer < 100)
                return LaserTimer / 100f;

            return 1f;
        }
    }
}
