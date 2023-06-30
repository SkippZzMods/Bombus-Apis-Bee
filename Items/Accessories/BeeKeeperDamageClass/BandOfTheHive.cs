using Terraria;
namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class BandOfTheHive : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive-Touched Band");
            Tooltip.SetDefault("Dealing a large amount of damage in a short amount of time causes your next hit to make a large honey explosion");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HiveBandPlayer>().equipped = true;
        }
    }

    class HiveBandPlayer : ModPlayer
    {
        public bool equipped;

        bool explode;
        int timer;
        int damage;

        public override void ResetEffects()
        {
            equipped = false;

            if (timer > 0)
                timer--;
            else
                damage = 0;

            if (damage >= 500)
                explode = true;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
        {
            if (!equipped || Player.HasBuff<BandOfTheHiveCooldown>())
                return;

            this.damage += damageDone;
            if (timer <= 0)
                timer = 120;

            Explode(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            if (!equipped || proj.type == ModContent.ProjectileType<BandHoneyGlob>() || Player.HasBuff<BandOfTheHiveCooldown>())
                return;

            this.damage += damageDone;
            if (timer <= 0)
                timer = 120;

            Explode(target);
        }

        void Explode(NPC target)
        {
            if (!explode)
                return;

            damage = Player.ApplyHymenoptraDamageTo(500);

            for (int i = 0; i < 50; i++)
            {
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Circular(15f, 15f), ModContent.ProjectileType<BandHoneyGlob>(), (int)(damage * 0.5f), 0f, Player.whoAmI);
            }

            for (int i = 0; i < 35; i++)
            {
                float angle = 6.2831855f * i / 35f;
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Utils.ToRotationVector2(angle) * 7.5f, ModContent.ProjectileType<BandHoneyGlob>(), (int)(damage * 0.5f), 0f, Player.whoAmI);
            }

            for (int i = 0; i < 200; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<BandHoneyDust>(), Main.rand.NextVector2Circular(10f, 10f), 0, default, 5f);
            }

            Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<BandHoneyExplosion>(), damage, 0, Player.whoAmI, 150, 55);

            Player.Bombus().AddShake(30);

            SoundEngine.PlaySound(new SoundStyle("BombusApisBee/Sounds/Item/BiggerSplash"), Player.Center);

            explode = false;
            damage = 0;
            timer = 0;

            Player.AddBuff<BandOfTheHiveCooldown>(1200);
        }
    }

    class BandHoneyGlob : ModProjectile
    {
        float originalScale;

        int originalTimeLeft;

        private Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Glob");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = Main.rand.Next(180, 240);
            originalTimeLeft = Projectile.timeLeft;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.scale = Main.rand.NextFloat(0.5f, 2f);
            originalScale = Projectile.scale;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(originalScale, originalScale * 0.05f, 1f - Projectile.timeLeft / (float)originalTimeLeft);

            Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !TileID.Sets.Platforms[tile.TileType])
            {
                Projectile.velocity *= 0f;

                float rad = 5 * Projectile.scale;

                if (Main.rand.NextBool(120))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(rad, rad), ModContent.DustType<BandHoneyDust>(), Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(0.1f, 0.5f), 0, default, 2 * Projectile.scale);

                    dust.noGravity = true;
                }
            }
            else
            {

                Projectile.rotation += (0.35f * (Projectile.velocity.X * 0.15f)) * Projectile.direction;
                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.035f;
                    else
                        Projectile.velocity.Y *= 1.01f;
                }
                if (Projectile.velocity.Y > 20f)
                    Projectile.velocity.Y = 20f;
            }

            if ((Main.npc.Any(n => n.active && Projectile.Distance(n.Center) < 25f * Projectile.scale) || Main.projectile.Any(p => p.active && p.type == Type && p != Projectile && p.velocity == Vector2.Zero && Projectile.Distance(p.Center) < 35f * p.scale)) && Projectile.timeLeft < 190)
                Projectile.velocity = Projectile.velocity * 0.15f;
        }

        public void DrawMetaball()
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(tex, (Projectile.Center - Main.screenPosition) / 2, null, Color.White, 0f, tex.Size() / 2, Projectile.scale / 2, SpriteEffects.None, 0f);
        }
    }

    class BandHoneyDust : ModDust
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.noGravity)
            {
                Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];
                if (tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType])
                {
                    if (dust.velocity.Y > 1f)
                        dust.velocity.Y *= 0.15f;

                    dust.velocity.Y += 0.005f;
                }
                else
                    dust.velocity.Y += 0.15f;

                dust.scale *= 0.985f;
            }
            else
            {
                dust.velocity.Y += 0.2f;

                Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];

                if (tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType])
                    dust.velocity *= -0.5f;

                dust.scale *= 0.96f;
            }

            dust.rotation = dust.velocity.ToRotation();

            if (dust.scale < 0.2f)
                dust.active = false;

            return false;
        }
    }

    class BandHoneyExplosion : ModProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 25f);

        private float Radius => Projectile.ai[0] * (float)Math.Sqrt(Math.Sqrt(Progress));

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Explosion");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            for (int k = 0; k < 6; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * (Radius * 0.75f), ModContent.DustType<Dusts.GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, new Color(255, 200, 10), Main.rand.NextFloat(0.35f, 0.4f));
            }
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            float radius = Radius;

            for (int i = 0; i < 33; i++) //TODO: Cache offsets, to improve performance
            {
                double rad = i / 32f * 6.28f;
                var offset = new Vector2((float)Math.Sin(rad), (float)Math.Cos(rad));
                offset *= radius;
                offset = offset.RotatedBy(Projectile.ai[1]);
                cache.Add(Projectile.Center + offset);
            }

            while (cache.Count > 33)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 33, new TriangularTip(1), factor => 40 * (1 - Progress), factor =>
            {
                return new Color(255, 100, 20);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 33, new TriangularTip(1), factor => 30 * (1 - Progress), factor =>
            {
                return new Color(255, 150, 10) * 0.25f;
            });

            float nextplace = 33f / 32f;
            var offset = new Vector2((float)Math.Sin(nextplace), (float)Math.Cos(nextplace));
            offset *= Radius;

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + offset;

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + offset;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public void DrawPrimitives()
        {
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(3f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/LiquidTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
}
