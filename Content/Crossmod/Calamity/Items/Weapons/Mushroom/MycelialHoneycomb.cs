using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Mushroom
{
    public class MycelialHoneycomb : CalamityDamageItem
    {
        public float shootRotation;
        public int shootDirection;

        public bool justAltUsed; // to prevent custom recoil anim

        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires fungal bees\n" +
                "Hold <right> to erupt deadly spores from the nearest tile, inflicting Mycosis\n" +
                "Mycosis causes inflicted enemies to periodically create fungal bees");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 15;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<FungalBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = BombusApisBee.HoneycombWeapon;
            honeyCost = 4;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.altFunctionUse == 2)
            {
                Item.useTime = 120;
                Item.useAnimation = 120;
                justAltUsed = true;
            }
            else
            {
                Item.useTime = 32;
                Item.useAnimation = 32;
                justAltUsed = false;
            }

            shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
            shootDirection = Main.MouseWorld.X < Player.Center.X ? -1 : 1;

            return base.CanUseItem(Player);
        }

        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts<MycelialHoneycombProjectile>() <= 0;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (justAltUsed)
                return;

            if (Item.noUseGraphic) // the item draws wrong for the first frame it is drawn when you switch directions for some odd reason, this plus setting it to true in shoot makes it not draw for the first frame.
                Item.noUseGraphic = false;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            if (animProgress < 0.05f)
            {
                float lerper = animProgress / 0.05f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -5f, EaseFunction.EaseCircularOut.Ease(lerper));
            }
            else
            {
                float lerper = (animProgress - 0.05f) / 0.95f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-5f, 0f, EaseFunction.EaseBackInOut.Ease(lerper));
            }

            Vector2 itemSize = new Vector2(28f, 30f);
            Vector2 itemOrigin = new Vector2(-10f, 4f);

            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (justAltUsed)
                return;

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (!justAltUsed)
            {
                if (animProgress < 0.05f)
                {
                    float lerper = animProgress / 0.05f;
                    rotation += MathHelper.Lerp(0f, -.1f, EaseFunction.EaseCircularOut.Ease(lerper)) * player.direction;
                }
                else
                {
                    float lerper = (animProgress - 0.05f) / 0.95f;
                    rotation += MathHelper.Lerp(-.1f, 0, EaseFunction.EaseBackInOut.Ease(lerper)) * player.direction;
                }
            }

            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;

            if (justAltUsed)
            {
                if (animProgress > 0.85f)
                    stretch = Player.CompositeArmStretchAmount.None;
                else if (animProgress > 0.6f)
                    stretch = Player.CompositeArmStretchAmount.Quarter;
                else if (animProgress > 0.45f)
                    stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            }
            else
            {
                if (animProgress < 0.5f)
                    stretch = Player.CompositeArmStretchAmount.None;
                else if (animProgress < 0.75f)
                    stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            }

            player.SetCompositeArmFront(true, stretch, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.noUseGraphic = true;

            if (justAltUsed)
            {
                Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<MycelialHoneycombProjectile>(), damage, knockback, player.whoAmI);

                return false;
            }

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.3f), type, damage, knockback, player.whoAmI);
            }

            Vector2 barrelPos = position + new Vector2(16f, -4f * player.direction).RotatedBy(velocity.ToRotation());

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.25f, 1f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.25f, 1f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(90, 167, 209, 0), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
            }

            player.Bombus().AddShake(2);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class MycelialHoneycombProjectile : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public int MaxTimer;

        public Point16? spawnTile = new Point16?();
        public ref float Timer => ref Projectile.ai[0];
        public ref float FadeTimer => ref Projectile.ai[1];
        public Vector2 OwnerMouse => Owner.GetModPlayer<ControlsPlayer>().mouseWorld;
        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Mushroom/MycelialHoneycomb";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mycelial Honeycomb");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 startPos = OwnerMouse - new Vector2(8f, 0f);

            bool exit = false;

            for (int x = 1; x < 11; x++) // search 5 tiles each direction, starting at the cursor
            {
                if (exit)
                    break;

                for (int y = 0; y < 25; y++) // search 25 tiles down
                {
                    int direction = x % 2 == 0 ? -1 : 1;
                    int trueX = x / 2;

                    Vector2 worldPos = startPos + new Vector2(trueX * direction * 16f, y * 16f);
                    var tilePos = new Point16((int)worldPos.X / 16, (int)worldPos.Y / 16);
                    Tile tile = Framing.GetTileSafely(tilePos);
                    Tile aboveTile = Framing.GetTileSafely(new Point16(tilePos.X, tilePos.Y - 1));
                    if (tile.HasTile && !WorldGen.SolidOrSlopedTile(aboveTile) && (WorldGen.SolidOrSlopedTile(tile) || WorldGen.ActiveAndWalkableTile(tilePos.X, tilePos.Y)) && !aboveTile.HasTile)
                    {
                        spawnTile = tilePos;
                        exit = true;
                        break;
                    }
                }
            }

            MaxTimer = (int)Owner.ApplyHymenoptraSpeedTo(Owner.GetActiveItem().useAnimation);
            Projectile.velocity = Owner.DirectionTo(OwnerMouse);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.netUpdate = true;

            if (spawnTile is null)
            {
                Projectile.Kill();
                Owner.itemTime = 0;
                Owner.itemAnimation = 0;
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (FadeTimer > 0)
            {
                if (FadeTimer == 1)
                {
                    Projectile.Kill();
                    return;
                }

                FadeTimer--;
            }

            Timer++;

            if (Timer == MaxTimer)
            {
                FireProjectiles();
            }

            UpdateHeldProjectile();

            UpdateDustVisuals();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            // Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, handRotation);
            // Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D tellTex = Request<Texture2D>(Texture + "_TileTell").Value;

            float lerper = Timer / MaxTimer;
            if (FadeTimer > 0)
                lerper = FadeTimer / 30f;

            float rand = 2f * lerper;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(rand, rand), null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            if (spawnTile != null)
            {
                var pos = new Vector2(spawnTile.Value.X * 16, spawnTile.Value.Y * 16);

                int tellHeight = FadeTimer > 0 ? 128 : (int)MathHelper.Lerp(0f, 128f, EaseFunction.EaseQuinticIn.Ease(lerper));

                for (int i = 0; i < tellHeight; i++)
                {
                    Vector2 position = pos + new Vector2(8f, -0.5f - 1 * i);

                    Color color = Color.Lerp(new Color(90, 167, 209, 0), new Color(133, 255, 237, 0), i / (float)tellHeight);

                    Main.spriteBatch.Draw(tellTex, position - Main.screenPosition, null, color * (1f - i / (float)tellHeight) * EaseFunction.EaseCubicIn.Ease(lerper), 0f, tellTex.Size() / 2f, 1f, 0f, 0f);
                }
            }

            return false;
        }

        private void FireProjectiles()
        {
            Owner.Bombus().shakeTimer += 10;
            FadeTimer = 30;

            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                if (Main.myPlayer == Owner.whoAmI)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnTile.Value.ToWorldCoordinates(), new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(0f, -12f)),
                        ProjectileType<HomingMycelialSpore>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/MushroomShoot").PlayWith(spawnTile.Value.ToWorldCoordinates());
        }

        private void UpdateDustVisuals()
        {
            if (FadeTimer > 0)
                return;

            if (spawnTile != null)
            {
                var pos = new Vector2(spawnTile.Value.X * 16, spawnTile.Value.Y * 16);
                pos += new Vector2(12f, 0f);

                if (Main.rand.NextBool(5))
                {
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(8f, 0.5f), DustType<PixelatedGlowAlt>(),
                        new Vector2(0f, -MathHelper.Lerp(1f, 5f, EaseFunction.EaseQuinticIn.Ease(Timer / MaxTimer))), 0, new Color(32, 79, 79, 0), 0.35f);
                }

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        new Vector2(0f, -MathHelper.Lerp(1f, 5f, EaseFunction.EaseQuinticIn.Ease(Timer / MaxTimer))), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.35f, 0.6f)).noGravity = true;
                }

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.35f, 0.6f)).noGravity = true;
                }

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                       Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.35f, 0.6f)).noGravity = true;
                }

                if (Main.myPlayer == Owner.whoAmI && Main.rand.NextBool(30))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)),
                        ProjectileType<MycelialMushroom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        private void UpdateHeldProjectile()
        {
            Owner.ChangeDir(OwnerMouse.X < Owner.Center.X ? -1 : 1);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);

            Projectile.timeLeft = 2;

            Vector2 armPos = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Projectile.position = armPos - Projectile.Size * 0.5f + Projectile.velocity * MathHelper.Lerp(20f, 15f, Timer / MaxTimer);

            if (FadeTimer > 0)
            {
                float animProgress = 1f - FadeTimer / 30f;

                if (animProgress < 0.05f)
                {
                    float lerper = animProgress / 0.05f;
                    Projectile.position += Projectile.velocity * MathHelper.Lerp(0f, -8f, EaseFunction.EaseCircularOut.Ease(lerper));
                    Projectile.rotation += MathHelper.Lerp(0f, -.25f, EaseFunction.EaseCircularOut.Ease(lerper)) * Owner.direction;
                }
                else
                {
                    float lerper = (animProgress - 0.05f) / 0.95f;
                    Projectile.position += Projectile.velocity * MathHelper.Lerp(-8f, 0f, EaseFunction.EaseBackInOut.Ease(lerper));
                    Projectile.rotation += MathHelper.Lerp(-.25f, 0, EaseFunction.EaseBackInOut.Ease(lerper)) * Owner.direction;
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(spawnTile.Value.ToWorldCoordinates()), true);

                Vector2 oldVelocity = Projectile.velocity;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(spawnTile.Value.ToWorldCoordinates()), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class HomingMycelialSpore : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        private List<Vector2> cache;
        private Trail trail;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Spore");
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 1f)
                Projectile.Kill();

            if (Main.rand.NextBool(2))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(0.5f, 0.5f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.35f, 0.6f)).noGravity = true;
            }

            if (Main.rand.NextBool(2))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(0.5f, 0.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.35f, 0.6f)).noGravity = true;
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(32, 79, 79, 0), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(116, 108, 166, 0), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Vector2 targetCenter = Projectile.Center;
            bool foundTarget = false;
            float num = 1000f;
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
            if (foundTarget)
                Projectile.velocity = (Projectile.velocity * 20f + (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 8f) / 21f;
            else
                Projectile.velocity *= 0.98f;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Mycosis>(300);

            target.GetGlobalNPC<MycosisGlobalNPC>().owner = Projectile.owner;
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].Bombus().shakeTimer += 2;

            //new SoundStyle("CalamityMod/Sounds/Custom/PistolShrimpBubbleBurst").PlayWith(Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(32, 79, 79, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(116, 108, 166, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 150), new Color(70, 90, 166), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 150), new Color(90, 167, 209), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            Texture2D bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = new Color(70, 90, 166, 0);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 0.25f, 0f, 0f);
            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, bloom.Size() / 2f, 0.15f, 0f, 0f);

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(1), factor => 5f, factor =>
            {
                return Color.Lerp(new Color(32, 79, 79), new Color(70, 90, 166), factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
                effect.Parameters["repeats"].SetValue(2f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);

            });
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class MycelialMushroom : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public Vector2 StartCenter;
        public Vector2 EndCenter;
        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.hide = true;

            Projectile.frame = Main.rand.Next(4);
        }

        public override bool? CanDamage()
        {
            return Projectile.penetrate > 1;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            DisplayName.SetDefault("Mushroom");
        }

        public override void OnSpawn(IEntitySource source)
        {
            StartCenter = Projectile.Center;
            EndCenter = Projectile.Center + Vector2.One.RotatedBy(Projectile.velocity.ToRotation()) * 8f;
        }

        public override void AI()
        {
            float lerper = 1f - Projectile.timeLeft / 60f;

            Projectile.Center = Vector2.Lerp(StartCenter, EndCenter, EaseFunction.EaseBackOut.Ease(lerper));
            Projectile.rotation = StartCenter.DirectionTo(EndCenter).ToRotation() + MathHelper.PiOver2;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fadeOut = 1f;
            if (Projectile.timeLeft < 15f)
                fadeOut = Projectile.timeLeft / 15f;

            Texture2D tex = Request<Texture2D>(Texture).Value;

            Rectangle rect = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor * fadeOut, Projectile.rotation, rect.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class Mycosis : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mycosis");
            Description.SetDefault("Off the goop stinkin straight shrooms");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<MycosisGlobalNPC>().inflicted = true;

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(32, 79, 79, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class MycosisGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool InstancePerEntity => true;

        public bool inflicted;
        public int owner;

        public override void ResetEffects(NPC npc)
        {
            if (!inflicted)
                owner = -1;

            inflicted = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (inflicted)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 8;
                if (damage < 1)
                {
                    damage = 1;
                }
            }
        }

        public override void AI(NPC npc)
        {
            if (inflicted && Main.rand.NextBool(60) && owner >= 0)
            {
                Player player = Main.player[owner];

                if (player is null)
                {
                    inflicted = false;
                    return;

                }

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/MushroomHit").PlayWith(npc.Center);

                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f);

                Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileType<FungalBee>(), 15, 1f, player.whoAmI);

                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                       velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f), 0, new Color(90, 167, 209, 0), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
                }

                player.Bombus().AddShake(5);
            }
        }
    }
}
