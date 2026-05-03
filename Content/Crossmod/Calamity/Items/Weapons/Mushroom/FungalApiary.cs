using BombusApisBee.BeeHelperProj;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Forest.Items.WoodenApiary;
using BombusApisBee.Core.PixelationSystem;
using CalamityMod.Items.Materials;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Mushroom
{
    [JITWhenModsEnabled("CalamityMod")]
    public class FungalApiary : ApiaryItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Apiary");
            Tooltip.SetDefault("Hold <left> to fire fungal bees\nHold <right> to fire bees slower, but take control over the bees causing them to create homing spores on hit\nCritical hits spawn explosive spores");
        }

        public override void AddDefaults()
        {
            Item.damage = 8;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 28;
            Item.useAnimation = 28;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.15f;

            Item.value = Item.sellPrice(silver: 10);

            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<FungalApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 2;
            altHoneyCost = 3;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 40;
                Item.useAnimation = 40;

            }
            else
            {
                Item.useTime = 28;
                Item.useAnimation = 28;

            }

            return base.SafeCanUseItem(player);
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // if the hit is a crit, always spawn an explosive spore
            if (hit.Crit)
            {
                SoundID.Item42.PlayWith(target.Center, volume: 1.5f);

                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(4f, 4f),
                    ProjectileType<ExplosiveSpore>(), (int)(hit.Damage * 1.75f), 1f, projectile.owner);

                return;
            }

            // otherwise, spawn a homing spore 20% of the time
            if (Main.rand.NextBool(5))
            {
                SoundID.Item42.PlayWith(target.Center, volume: 1.5f);

                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(4f, 4f),
                    ProjectileType<FungalSpore>(), 5 + (int)(hit.Damage * 0.5f), 1f, projectile.owner);
            }
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(120))
            {
                Color color = Color.Lerp(new Color(65, 232, 236, 0), new Color(108, 72, 196, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.15f);
            }
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Texture2D tex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().holdTimer;

            Color color = Color.Lerp(new Color(65, 232, 236, 0), new Color(108, 72, 196, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, color * (holdTimer / 20f) * 0.13f, 0f, tex.Size() / 2f, 0.25f, 0, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WoodenApiary>().
                AddIngredient<PearlShard>(3).
                AddIngredient<PollenItem>(15).
                AddIngredient(ItemID.GlowingMushroom, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class FungalApiaryHoldout : ApiaryHoldout
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override Color GlowColor => Color.Lerp(new Color(65, 232, 236), new Color(108, 72, 196), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Mushroom/FungalApiary";

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        protected override void Shoot()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Owner.UseBeeResource(Owner.altFunctionUse == 2 ? (Owner.HeldItem.ModItem as ApiaryItem).altHoneyCost : (Owner.HeldItem.ModItem as ApiaryItem).honeyCost))
                {
                    shakeTimer = 15;

                    SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
                    BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

                    for (int j = 0; j < 4; j++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.25f);
                    }

                    for (int i = 0; i < 1 + Main.rand.Next(1, 4); i++)
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(7f, 8f), ProjectileType<FungalBee>(), Projectile.damage, Projectile.knockBack);
                    }
                }
                else
                    Projectile.Kill();
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class FungalBee : BaseBeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Bee");
            Main.projFrames[Type] = 4;
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(90))
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       Main.rand.NextVector2Circular(.5f, .5f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                       Main.rand.NextVector2Circular(2.5f, 2.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
                }
            }

            if (Main.rand.NextBool(20))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f), DustID.MushroomTorch, Projectile.velocity * 0.25f, 0, default, 1f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                    DustID.MushroomTorch, Main.rand.NextVector2Circular(3f, 3f), 0, default, 0.8f).noGravity = true;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class FungalSpore : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public int Time;
        public override bool? CanDamage() => Projectile.timeLeft < 400;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore");

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;

            Projectile.timeLeft = 430;
            Projectile.penetrate = 1;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Time++;

            NPC target = Main.npc.Where(n => n.CanBeChasedBy(Projectile, false) && Vector2.Distance(n.Center, Projectile.Center) < 500f).OrderBy(n => Vector2.Distance(n.Center, Projectile.Center)).FirstOrDefault();

            if (target != default && Projectile.timeLeft < 400)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                direction *= 10f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, 0.01f);
            }
            else
            {
                Projectile.velocity *= 0.985f;

                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundID.NPCDeath1.PlayWith(Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                    DustID.MushroomTorch, -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f), 0, default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f), 0, new Color(65, 232, 236, 0), 0.15f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fadeIn = Time / 15f;
            if (Time > 15f)
                fadeIn = 1f;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D blurTex = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D glowTex = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D outlineTex = Request<Texture2D>(Texture + "_Outline").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(outlineTex, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, Color.Lerp(new Color(65, 232, 236), new Color(255, 255, 255), i / (float)Projectile.oldPos.Length) * (1f - i / (float)Projectile.oldPos.Length),
                    Projectile.rotation, outlineTex.Size() / 2f, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(65, 232, 236, 0) * fadeIn, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * fadeIn, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(outlineTex, Projectile.Center - Main.screenPosition, null, new Color(65, 232, 236) * fadeIn, Projectile.rotation, outlineTex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(blurTex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * fadeIn * 0.5f, Projectile.rotation, blurTex.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class ExplosiveSpore : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public int Time;

        public float Progress => 1f - Projectile.timeLeft / 90f;
        public Player Owner => Main.player[Projectile.owner];
        public override bool? CanDamage() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosive Spore");

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;

            Projectile.timeLeft = 90;
            Projectile.penetrate = 1;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Time++;

            NPC target = Main.npc.Where(n => n.CanBeChasedBy(Projectile, false) && Vector2.Distance(n.Center, Projectile.Center) < 500f).OrderBy(n => Vector2.Distance(n.Center, Projectile.Center)).FirstOrDefault();

            if (target != default && Projectile.timeLeft < 75)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                direction *= 8f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, 0.015f);
            }
            else
            {
                Projectile.velocity *= 0.985f;

                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }

            Projectile.velocity *= MathHelper.Lerp(1f, 0.8f, Progress);

            Projectile.rotation += Projectile.velocity.Length() * 0.035f;

            if (Time > 45)
            {
                float lerper = (Time - 45) / 45f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), DustType<PixelatedGlow>(), Main.rand.NextVector2CircularEdge(5f * Progress, 5f * Progress), 0, new Color(225, 50, 10, 0), MathHelper.Lerp(0.1f, 0.25f, lerper));
            }
        }

        public override void OnKill(int timeLeft)
        {
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center);

            Owner.Bombus().AddShake(10, true);

            if (!Main.dedServ)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                    Vector2.Zero, ProjectileType<FungalExplosion>(), Projectile.damage, 0f, Projectile.owner, 55);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fadeIn = Time / 15f;
            if (Time > 15f)
                fadeIn = 1f;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D blurTex = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D glowTex = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D outlineTex = Request<Texture2D>(Texture + "_Outline").Value;
            Texture2D whiteTex = Request<Texture2D>(Texture + "_White").Value;

            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Vector2 pos = Projectile.Center;

            pos += Main.rand.NextVector2CircularEdge(MathHelper.Lerp(0f, 1.5f, Progress), MathHelper.Lerp(0f, 1.5f, Progress));

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(outlineTex, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, Color.Lerp(new Color(255, 255, 255), new Color(227, 69, 16), Progress),
                    Projectile.rotation, outlineTex.Size() / 2f, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.Draw(glowTex, pos - Main.screenPosition, null, Color.Lerp(new Color(255, 255, 255, 0), new Color(227, 69, 16, 0), Progress) * fadeIn, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White * fadeIn, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(outlineTex, pos - Main.screenPosition, null, Color.Lerp(new Color(255, 255, 255), new Color(227, 69, 16), Progress) * fadeIn, Projectile.rotation, outlineTex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(blurTex, pos - Main.screenPosition, null, Color.White with { A = 0 } * fadeIn * 0.5f, Projectile.rotation, blurTex.Size() / 2f, Projectile.scale, 0f, 0f);

            if (Time > 45)
            {
                float lerper = (Time - 45) / 45f;

                Main.spriteBatch.Draw(whiteTex, pos - Main.screenPosition, null, new Color(227, 69, 16, 0) * fadeIn * lerper, Projectile.rotation, whiteTex.Size() / 2f, Projectile.scale, 0f, 0f);

                Main.spriteBatch.Draw(glowTex, pos - Main.screenPosition, null, new Color(227, 69, 16, 0) * fadeIn * lerper, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(227, 69, 16, 0) * fadeIn * lerper, Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(227, 150, 150, 0) * fadeIn * lerper, Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale * 0.5f, 0f, 0f);
            }

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class FungalExplosion : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - Projectile.timeLeft / 30f;

        private float Radius => Projectile.ai[0] * EaseFunction.EaseQuinticOut.Ease(Progress);

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Explosion");
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

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, DustType<PixelatedGlow>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(200, 40, 20, 0) : new Color(255, 90, 20, 0), Main.rand.NextFloat(0.5f, 0.6f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return false;
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 41; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 41; k++)
            {
                cache[k] = Projectile.Center + Vector2.One.RotatedBy(k / 40f * 6.28f) * Radius;
            }

            while (cache.Count > 41)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 41, new TriangularTip(1), factor => 20 * (1f - Progress), factor =>
            {
                return Color.Lerp(new Color(255, 255, 255), new Color(250, 80, 20), EaseFunction.EaseCircularOut.Ease(Progress));
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 41, new TriangularTip(1), factor => 10 * (1f - Progress), factor =>
            {
                return Color.Lerp(new Color(255, 255, 255), new Color(255, 120, 20), EaseFunction.EaseCircularOut.Ease(Progress));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[40];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[40];
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
                effect.Parameters["repeats"].SetValue(5f);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

                trail2?.Render(effect);
            });
        }
    }
}
