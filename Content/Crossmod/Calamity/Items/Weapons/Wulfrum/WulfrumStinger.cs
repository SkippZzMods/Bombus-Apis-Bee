using BombusApisBee.Content.Crossmod.Calamity.NPCs.Enemies.Wulfrum;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Sounds;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Wulfrum
{
    public class WulfrumStinger : CalamityDamageItem
    {
        internal float shootRotation;
        internal int shootDirection;
        internal int armDirection;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Rapidly throws wulfrum stingers\nStriking an enemy has a chance to drop an unstable Wulfrum power cell\nPower cells can be picked up, and are thrown with <right>\nPower cells can be hit mid air to supercharge them, causing them to detonate with an even more violent explosion");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 18;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<WulfrumStingerProjectile>();
            Item.shootSpeed = 15f;
            Item.UseSound = WulfrumKnife.Throw2Sound;
            honeyCost = 3;
            Item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player) => player.GetModPlayer<WulfrumStingerPlayer>().powerCells.Count > 0;

        public override bool CanUseItem(Player Player)
        {
            shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
            shootDirection = Main.MouseWorld.X < Player.Center.X ? -1 : 1;

            if (armDirection != 1 && armDirection != -1)
                armDirection = 1;

            armDirection *= -1;

            if (Player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item1;
                Item.useTime = 10;
                Item.useAnimation = 10;
            }
            else
            {
                Item.useTime = 25;
                Item.useAnimation = 25;
                Item.UseSound = WulfrumKnife.Throw2Sound;
            }

            return base.CanUseItem(Player);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (animProgress < 0.6f)
            {
                float lerper = animProgress / 0.6f;
                rotation += MathHelper.Lerp(0f, -1.57f * armDirection, EaseFunction.EaseCircularOut.Ease(lerper)) * player.direction;

                Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), DustType<Glow>(), rotation.ToRotationVector2() * 0.5f, 0, Color.Lerp(new Color(130, 200, 70), new Color(55, 180, 220), animProgress), 0.25f);
            }
            else
            {
                rotation += -1.57f * armDirection * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                var mp = player.GetModPlayer<WulfrumStingerPlayer>();
                mp.powerCells.Remove(mp.powerCells.OrderBy(p => p.lifeTime).FirstOrDefault());

                Projectile.NewProjectile(source, position, velocity, ProjectileType<WulfrumStingerPowerCellProjectile>(), (int)(damage * 0.85), knockback * 3, player.whoAmI);
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<PollenItem>(15).
                AddIngredient<WulfrumMetalScrap>(12).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumStingerPlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public int availableIndex => powerCells.All(p => p.index != 1) ? 1 : powerCells.All(p => p.index != 2) ? 2 : 3;

        public int drawTimer;
        public int timerDir = -1;

        public int fadeIn;

        public List<WulfrumStingerPowerCell> powerCells = new();

        public override void Load()
        {
            //On_Main.DrawInfernoRings += DrawPowerCells;
            On_Main.DrawPlayers_AfterProjectiles += DrawPowerCells;
        }

        private void DrawPowerCells(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead)
                {
                    var mp = player.GetModPlayer<WulfrumStingerPlayer>();
                    if (mp.fadeIn > 0)
                    {
                        foreach (WulfrumStingerPowerCell powerCell in mp.powerCells)
                        {
                            powerCell.Draw(Main.spriteBatch);
                        }
                    }
                }
            }

            orig(self);
        }

        public override void ResetEffects()
        {
            List<WulfrumStingerPowerCell> powerCellsToRemove = new();

            foreach (WulfrumStingerPowerCell powerCell in powerCells)
            {
                powerCell.Update();
                if (powerCell.lifeTime <= 0)
                    powerCellsToRemove.Add(powerCell);
            }

            foreach (WulfrumStingerPowerCell powerCell in powerCellsToRemove)
            {
                powerCells.Remove(powerCell);
            }

            if (powerCells.Count > 0)
                drawTimer++;
            else if (drawTimer > 0)
                drawTimer = 0;

            if (Player.HeldItem.type == ItemType<WulfrumStinger>())
            {
                if (fadeIn < 15)
                    fadeIn++;
            }
            else if (fadeIn > 0)
                fadeIn--;
        }

        /*private void DrawPowerCells(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead)
                {
                    var mp = player.GetModPlayer<WulfrumStingerPlayer>();
                    foreach (WulfrumStingerPowerCell powerCell in mp.powerCells)
                    {
                        powerCell.Draw(Main.spriteBatch);
                    }
                }
            }           

            orig(self);
        }*/
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumStingerPowerCell : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        internal string Texture = "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Wulfrum/WulfrumStingerPowerCell";

        public int lifeTime;
        public int index;
        public int ownerWhoAmI;

        public Player Owner => Main.player[ownerWhoAmI];

        internal Vector2 position;
        internal List<Vector2> cache;
        internal Trail trail;
        public WulfrumStingerPowerCell(int lifeTime, int index, int owner)
        {
            this.lifeTime = lifeTime;
            this.index = index;
            ownerWhoAmI = owner;

            position = Owner.Center;
        }

        public void Update()
        {
            lifeTime--;

            var mp = Owner.GetModPlayer<WulfrumStingerPlayer>();

            Vector2 positionToBe = Owner.Center + new Vector2(-20f * Owner.direction * index, -10f * (float)Math.Sin(index + mp.drawTimer * 0.05f));

            float speed = 0.05f;
            speed += 0.2f * Owner.velocity.Length() * 0.1f;

            position = Vector2.Lerp(position, positionToBe, speed);

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            DrawPrimitives(sb);

            var mp = Owner.GetModPlayer<WulfrumStingerPlayer>();

            float fade = mp.fadeIn / 15f;

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            sb.Draw(tex, position - Main.screenPosition, null, Color.White * fade, Owner.velocity.X * 0.02f, tex.Size() / 2f, 1f, 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = new Color(130, 200, 70, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, position - Main.screenPosition, null, Color.White, Owner.velocity.X * 0.02f, tex.Size() / 2f, 1.15f, 0f, 0f);

            color = new Color(55, 180, 220, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, position - Main.screenPosition, null, Color.White, Owner.velocity.X * 0.02f, tex.Size() / 2f, 1.15f, 0f, 0f);

            sb.End();
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            cache = new List<Vector2>();

            for (int i = 0; i < 10; i++)
            {
                cache.Add(Vector2.Lerp(position, Owner.Center + Owner.velocity * 2f, i / 10f));
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(0), factor => Utils.Clamp(15f * (float)Math.Sin(lifeTime * 0.025f), 5f, 15f), factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return Color.Lerp(new Color(55, 180, 220), new Color(130, 200, 70), factor.X) * 0.3f * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[9];
        }

        public void DrawPrimitives(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            var world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);

            sb.End();
        }
        #endregion Primitive Drawing
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumStingerPowerCellItemProjectile : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        internal List<Vector2> cache;
        internal Trail trail;
        public float TrailOpacity => Projectile.Distance(Owner.Center) > 150f ? 0 : 1f - Projectile.Distance(Owner.Center) / 150f;
        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Wulfrum/WulfrumStingerPowerCell";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Power Cell");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.Distance(Owner.Center) < 150f)
            {
                Vector2 idlePos = Owner.Center;

                float dist = Vector2.Distance(Projectile.Center, idlePos);

                Vector2 toIdlePos = idlePos - Projectile.Center;
                if (toIdlePos.Length() < 0.0001f)
                    toIdlePos = Vector2.Zero;
                else
                {
                    float speed = MathHelper.Lerp(5f, 15f, dist / 150f);

                    toIdlePos.Normalize();
                    toIdlePos *= speed;
                }

                Projectile.velocity = (Projectile.velocity * (20f - 1) + toIdlePos) / 20f;

                if (Projectile.Distance(Owner.Center) < 25f)
                {
                    var mp = Owner.GetModPlayer<WulfrumStingerPlayer>();

                    mp.powerCells.Add(new WulfrumStingerPowerCell(60000, mp.availableIndex, Projectile.owner));

                    SoundID.Grab.PlayWith(Owner.Center);

                    Projectile.Kill();
                }


                if (!Main.dedServ)
                {
                    ManageCaches();
                    ManageTrail();
                }
            }
            else
            {
                if (Projectile.velocity.Y < 16f)
                {
                    Projectile.velocity.Y += 0.15f;
                    Projectile.velocity.Y *= 1.015f;
                }

                Projectile.velocity.X *= 0.99f;
            }

            Projectile.rotation += Projectile.velocity.X * 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.95f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D tex = Request<Texture2D>(Texture).Value;

            DrawPrimitives(sb);

            float fade = Projectile.timeLeft / 600f;

            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, tex.Size() / 2f, 1f, 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = new Color(130, 200, 70, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            color = new Color(55, 180, 220, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            cache = new List<Vector2>();

            for (int i = 0; i < 10; i++)
            {
                cache.Add(Vector2.Lerp(Projectile.Center, Owner.Center + Owner.velocity * 2f, i / 10f));
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(0), factor => Utils.Clamp(15f * (float)Math.Sin(Projectile.timeLeft * 0.025f), 5f, 15f), factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return Color.Lerp(new Color(55, 180, 220), new Color(130, 200, 70), factor.X) * 0.3f * factor.X * TrailOpacity;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[9];
        }

        public void DrawPrimitives(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            var world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumStingerPowerCellProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        internal List<Vector2> cache;
        internal Trail trail;

        internal int explodingTimer;
        internal bool exploding;
        internal bool supercharged;

        internal int flashTimer;

        internal float Divisor => supercharged ? 24f : 12f;
        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Wulfrum/WulfrumStingerPowerCell";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Power Cell");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = true;
        }

        public override bool? CanDamage()
        {
            return Projectile.penetrate >= 2;
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (exploding)
            {
                explodingTimer++;
                if (explodingTimer > Divisor)
                {
                    Explode();
                    Projectile.Kill();
                }

                Projectile.velocity.Y += 0.15f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 12f)
                        Projectile.velocity.Y *= 1.05f;
                    else
                        Projectile.velocity.Y *= 1.025f;
                }

                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;

                Projectile.velocity.X *= 0.925f;

                Projectile.rotation += Projectile.velocity.Length() * 0.05f;

                if (supercharged)
                {
                    float lerper = EaseFunction.EaseQuinticOut.Ease(1f - explodingTimer / Divisor);

                    float length = 105f * lerper;

                    Vector2 offset = Vector2.One.RotatedBy(6.28f * lerper) * length;

                    Dust.NewDustPerfect(Projectile.Center + offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(55, 180, 220), 0.5f);

                    Dust.NewDustPerfect(Projectile.Center - offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(130, 200, 70), 0.5f);

                    offset = Vector2.Lerp(offset, Vector2.One.RotatedBy(6.28f * lerper * 2f) * length, 0.5f);

                    Dust.NewDustPerfect(Projectile.Center + offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(55, 180, 220), 0.5f);

                    Dust.NewDustPerfect(Projectile.Center - offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(130, 200, 70), 0.5f);

                }
            }
            else
            {
                if (Projectile.timeLeft < 580)
                {
                    Projectile.velocity.Y += 0.15f;
                    if (Projectile.velocity.Y > 0)
                    {
                        if (Projectile.velocity.Y < 12f)
                            Projectile.velocity.Y *= 1.05f;
                        else
                            Projectile.velocity.Y *= 1.025f;
                    }

                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;

                    Projectile.velocity.X *= 0.985f;
                }

                Projectile.rotation += Projectile.velocity.Length() * 0.05f;

                if (Main.projectile.Any(p => p.active && p.type == ProjectileType<WulfrumStingerProjectile>() && p.owner == Projectile.owner && p.Distance(Projectile.Center) < 35f))
                    StartExplode(true, Main.projectile.Where(p => p.active && p.type == ProjectileType<WulfrumStingerProjectile>() && p.owner == Projectile.owner && p.Distance(Projectile.Center) < 35f).OrderBy(p => p.Distance(Projectile.Center)).First());
            }

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public void StartExplode(bool supercharge = false, Projectile proj = null)
        {
            if (proj != null)
                proj.Kill();

            new SoundStyle("BombusApisBee/Sounds/Item/Ricochet").PlayWith(Projectile.Center);

            supercharged = supercharge;
            exploding = true;
            flashTimer = 12;
            Projectile.friendly = false;

            Projectile.velocity *= supercharge ? 0.5f : -0.35f;

            if (!supercharge)
            {
                Projectile.velocity.Y -= 2f;

                if (Projectile.velocity.Y > -1.5f)
                    Projectile.velocity.Y = -1.5f;

                if (Projectile.velocity.Y < -5f)
                    Projectile.velocity.Y = -5f;
            }
            else
            {
                RoverDrive.BreakSound.PlayWith(Projectile.Center);

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(1.5f), 0, new Color(130, 200, 70), 0.45f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(1.5f), 0, new Color(55, 180, 220), 0.45f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(1.5f), 0, new Color(130, 200, 70, 0), 0.1f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(1.5f), 0, new Color(55, 180, 220, 0), 0.1f);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.65f), 0, new Color(130, 200, 70), 0.45f);

                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.65f), 0, new Color(55, 180, 220), 0.45f);
            }
        }

        public void Explode()
        {
            PlasmaGrenade.ExplosionSound.PlayWith(Projectile.Center, volume: supercharged ? 0.8f : 0.4f);
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, volume: supercharged ? 0.8f : 0.4f);
            CommonCalamitySounds.WulfrumNPCDeathSound.PlayWith(Projectile.Center, 0, 0.2f, 0.75f);

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileType<WulfrumPowerCellExplosion>(), Projectile.damage * (supercharged ? 4 : 2), Projectile.knockBack, Projectile.owner, supercharged ? 100f : 50f);

            Owner.Bombus().AddDirectionalShake(Projectile.DirectionTo(Owner.Center) * (supercharged ? 25f : 9f));

            for (int i = 0; i < (supercharged ? 12 : 6); i++)
            {
                float rad = supercharged ? 30f : 15f;

                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(rad, rad), 0, new Color(130, 200, 70), 0.45f);

                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(rad, rad), 0, new Color(55, 180, 220), 0.45f);

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(rad, rad), 0, new Color(130, 200, 70, 0), 0.1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(rad, rad), 0, new Color(55, 180, 220, 0), 0.1f);
            }

            for (int i = 0; i < (supercharged ? 20 : 5); i++)
            {
                float rad = supercharged ? 120f : 70f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(rad, rad), DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 240), new Color(130, 200, 70, 0), Main.rand.NextFloat(0.15f, 0.45f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(rad, rad), DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 240), new Color(55, 180, 220, 0), Main.rand.NextFloat(0.15f, 0.45f)).noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!exploding)
                StartExplode();

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            StartExplode();
            Projectile.friendly = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texWhite = Request<Texture2D>(Texture + "_White").Value;
            Texture2D starTex = Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;

            DrawPrimitives(sb);

            Vector2 offset = Vector2.Zero;
            if (exploding && supercharged)
                offset = Main.rand.NextVector2Circular(5f * (explodingTimer / Divisor), 5f * (explodingTimer / Divisor));

            sb.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1f, 0f, 0f);

            if (explodingTimer > 0)
                sb.Draw(texWhite, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(55, 180, 220), Color.White, explodingTimer / Divisor) * (explodingTimer / Divisor), Projectile.rotation, texWhite.Size() / 2f, 1f, 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = new Color(130, 200, 70, 0);

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            color = new Color(55, 180, 220, 0);

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            sb.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            if (explodingTimer > 0)
                sb.Draw(starTex, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(55, 180, 220, 0), new Color(130, 200, 70, 0), 1f - explodingTimer / Divisor) * EaseFunction.EaseCircularInOut.Ease(1f - explodingTimer / Divisor), 0f, starTex.Size() / 2f, new Vector2(MathHelper.Lerp(0.35f, 1f, EaseFunction.EaseCircularInOut.Ease(1f - explodingTimer / Divisor)), MathHelper.Lerp(0.35f, 0.15f, 1f - explodingTimer / Divisor)), 0f, 0f);

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 10; i++)
                {
                    cache.Add(Projectile.Center + Projectile.velocity);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 10)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(0), factor => 12.5f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return Color.Lerp(new Color(55, 180, 220), new Color(130, 200, 70), factor.X) * 0.3f * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[9];
        }

        public void DrawPrimitives(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            var world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }

    [JITWhenModsEnabled("CalamityMod")]
    class WulfrumPowerCellExplosion : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - Projectile.timeLeft / 20f;

        private float Radius => Projectile.ai[0] * (float)Math.Sqrt(Math.Sqrt(Progress));

        public override void SafeSetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
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

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, DustType<GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.25f, 0, new Color(55, 180, 220), Main.rand.NextFloat(0.35f, 0.4f));

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, DustType<GlowFastDecelerate>(),
                   Vector2.One.RotatedBy(rot) * 0.5f, 0, new Color(130, 200, 70), Main.rand.NextFloat(0.35f, 0.4f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius * 1.25f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives(Main.spriteBatch);

            return false;
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 40; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 40; k++)
            {
                cache[k] = Projectile.Center + Vector2.One.RotatedBy(k / 19f * 6.28f) * Radius;
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 10, factor =>
            {
                return new Color(130, 200, 70) * EaseFunction.EaseCircularInOut.Ease(1f - Progress);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 5, factor =>
            {
                return new Color(55, 180, 220) * EaseFunction.EaseCircularInOut.Ease(1f - Progress) * 0.5f;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            var world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumStingerProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Stinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.25f), 0, new Color(130, 200, 70), 0.25f);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.rand.NextFloat(2, 4); i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(130, 200, 70, 0), 0.065f);
            }

            //Main.player[Projectile.owner].Bombus().AddShake(2);
            Main.player[Projectile.owner].Bombus().AddDirectionalShake(-Projectile.velocity * 0.25f);

            if (Main.rand.NextBool(2))
            {
                var mp = Owner.GetModPlayer<WulfrumStingerPlayer>();
                if (mp.powerCells.Count + Main.projectile.Count(p => p.active && p.type == ProjectileType<WulfrumStingerPowerCellItemProjectile>() && p.owner == Projectile.owner) < 3)
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Circular(7.5f, 2.5f) - Vector2.UnitY * 2.5f + Projectile.DirectionTo(Owner.Center + new Vector2(0f, -50f)) * 7.5f, ProjectileType<WulfrumStingerPowerCellItemProjectile>(), 0, 0f, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            CommonCalamitySounds.WulfrumNPCDeathSound.PlayWith(Projectile.Center, 0, 0.25f, 0.35f);

            for (int i = 0; i < Main.rand.NextFloat(3, 7); i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowFastDecelerate>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(130, 200, 70), 0.2f);
            }

            Dust.NewDustPerfect(Projectile.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(.5f, .5f), 150, new Color(130, 200, 70, 0), Main.rand.NextFloat(0.1f, 0.15f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = 1f;
            if (Projectile.timeLeft > 45)
                fade = (Projectile.timeLeft - 45) / 15f;

            Texture2D tex = Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * fade, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = new Color(130, 200, 70, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            color = new Color(55, 180, 220, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
