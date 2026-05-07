using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using CalamityMod.Items.Materials;

using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Desert
{
    class StormsentSceptre : CalamityDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Stormsent Sceptre");
            Tooltip.SetDefault("Casts a volley of Stormsent bees\n" +
                "Press <right> to cast a bolt of chain lightning, inflicting Stormsurged\n" +
                "Stormsent bees deal much more damage to Stormsurged enemies");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 8;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.knockBack = 1.5f;

            Item.rare = ItemRarityID.Blue;

            Item.shoot = ProjectileType<StormsentSceptreHoldout>();

            Item.value = Item.sellPrice(silver: 20);

            honeyCost = 3;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 37;
                Item.useAnimation = 37;
            }
            else
            {
                Item.useTime = 28;
                Item.useAnimation = 28;
            }

            return player.ownedProjectileCounts[ProjectileType<StormsentSceptreHoldout>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.UseBeeResource(honeyCost * 2);

                SoundStyle charge = new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/LightningCast") with { MaxInstances = 0 };

                charge.PlayWith(position, 1f - player.itemTime / 37);

                Projectile.NewProjectile(source, position, velocity, ProjectileType<StormsentSceptreHoldoutAlt>(), damage * 5, knockback, player.whoAmI);

                return false;
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormlionMandible>(2).
                AddIngredient<PollenItem>(10).
                AddIngredient(ItemID.SandstoneBrick, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class Stormsurged : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormsurged");
            Description.SetDefault("Stormy!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<StormsurgedGlobalNPC>().inflicted = true;

            if (Main.rand.NextBool(18))
            {
                Dust.NewDustPerfect(npc.Center, DustType<ElectricityDust>(),
                    Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(5f, 15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 3f)).customData = MathHelper.Pi;

            }

            if (Main.rand.NextBool(12))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), DustType<PixelatedGlow>(),
                                    Main.rand.NextVector2CircularEdge(.5f, .5f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(0.25f, 0.35f));
            }

            if (Main.rand.NextBool(30))
            {
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), DustType<ElectricSmoke>(), Main.rand.NextVector2CircularEdge(.25f, .25f),
                    0, new Color(70, 180, 220, 0), Main.rand.NextFloat(0.15f, 0.3f));

                dust.rotation = Main.rand.NextFloat(6.28f);
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class StormsurgedGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileType<StormsentBee>() && inflicted)
            {
                modifiers.ArmorPenetration += 10;
                modifiers.FinalDamage *= 2f;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class StormsentSceptreHoldout : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public bool updateVelocity = true;
        public ref float Timer => ref Projectile.ai[0];
        public ref float UseTime => ref Projectile.ai[1];

        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) +
            new Vector2(30f + MathHelper.Lerp(0f, -5f, EaseFunction.EaseQuarticInOut.Ease(Timer < UseTime ? Timer / UseTime : 1f)), -3f * Owner.direction).RotatedBy(Projectile.velocity.ToRotation()) + ArmOffset;
        public Vector2 ArmOffset;

        public Vector2 GemPosition => ArmPosition + new Vector2(22f, 0f).RotatedBy(Projectile.rotation);
        public bool Shot { get => Projectile.ai[2] != 0f; set => Projectile.ai[2] = value is true ? 1f : 0f; }
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Desert/StormsentSceptre";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormsent Sceptre");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        protected virtual void ExtraAI()
        {
            if (Shot)
            {
                float progress = 1f - Projectile.timeLeft / 25f;

                float recoilDist = -10f;
                float recoilRot = -0.35f;

                if (progress < 0.25f)
                {
                    float lerper = progress / 0.25f;

                    ArmOffset += Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0f, recoilDist, EaseFunction.EaseCircularOut.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(0f, recoilRot * Projectile.direction, EaseFunction.EaseCircularOut.Ease(lerper));
                }
                else
                {
                    float lerper = (progress - 0.25f) / 0.75f;

                    ArmOffset += Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(recoilDist, 0f, EaseFunction.EaseCircularIn.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(recoilRot * Projectile.direction, 0f, EaseFunction.EaseCircularIn.Ease(lerper));
                }
            }
        }

        public override void AI()
        {
            ArmOffset = Vector2.Zero;

            if (Timer == 0f)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
                UseTime = CombinedHooks.TotalUseTime(Owner.itemTime, Owner, Owner.HeldItem);
            }

            if (!Shot)
            {
                if (Timer >= UseTime)
                {
                    Shoot();
                    Shot = true;
                    Projectile.timeLeft = 25;
                }

                Timer++;
            }

            UpdateHeldProjectile(!Shot);

            ExtraAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;

            Texture2D gemBlur = Request<Texture2D>(Texture + "_GemBlur").Value;

            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;

            float rotation = Projectile.rotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0f) + MathHelper.PiOver4 * Projectile.spriteDirection;

            float fade = 0f;
            if (Timer < 8f)
                fade = Timer / 8f;
            else
                fade = 1f;

            float opacity = Timer / UseTime;

            if (Shot)
            {
                if (Projectile.timeLeft < 8f)
                    fade = EaseFunction.EaseCircularIn.Ease(Projectile.timeLeft / 8f);

                if (Projectile.timeLeft > 10)
                    opacity = (Projectile.timeLeft - 10) / 15f;
                else
                    opacity = 0f;
            }

            opacity *= fade;

            Main.spriteBatch.Draw(texGlow, position, null, new Color(120, 200, 200, 0) * opacity * 0.5f, rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(tex, position, null, lightColor * fade, rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(gemBlur, position, null, Color.White with { A = 0 } * opacity, rotation, gemBlur.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(bloomTex, GemPosition - Main.screenPosition, null, new Color(120, 200, 200, 0) * opacity * 0.15f, 0f, bloomTex.Size() / 2f, Projectile.scale * 1f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, GemPosition - Main.screenPosition, null, Color.White with { A = 0 } * opacity * 0.55f, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.2f, 0f, 0f);

            return false;
        }

        /// <summary>
        /// Called when the held projectile should shoot its projectile
        /// </summary>
        protected virtual void Shoot()
        {
            Vector2 pos = GemPosition + Projectile.velocity * 20f;

            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Projectile.velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(1f, 8f),
                    ProjectileType<StormsentBee>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 9; i++)
            {

                Dust.NewDustPerfect(pos, DustType<PixelImpactLineDust>(), Main.rand.NextVector2Circular(8f, 8f), 0, new Color(15, 70, 120, 0), 0.075f);

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(120, 200, 200, 0), 0.2f);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelatedGlow>(), Projectile.velocity * Main.rand.NextFloat(1f, 5f), 0, new Color(120, 200, 200, 0), 0.2f);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelImpactLineDust>(), Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(3f, 15f), 0, new Color(15, 70, 120, 0), 0.09f);
            }

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(pos, DustType<ElectricityDust>(), Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(5f, 10f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 3.5f));

                Dust.NewDustPerfect(pos, DustType<ElectricityDust>(), Projectile.velocity.RotatedByRandom(0.35f)
                    * Main.rand.NextFloat(5f, 20f) + Main.rand.NextVector2CircularEdge(1f, 1f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 3f)).customData = 1f;
            }

            Owner.Bombus().AddShake(5);
            Owner.Bombus().AddDirectionalShake(-Projectile.velocity * 8f);

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/LightningShortest1").PlayWith(Projectile.Center);

            updateVelocity = false;
        }

        /// <summary>
        /// Updates the basic variables needed for a held projectile
        /// </summary>
        protected void UpdateHeldProjectile(bool updateTimeleft = true)
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (updateTimeleft)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));

            Projectile.position = ArmPosition - Projectile.Size * 0.5f;

            if (Main.myPlayer == Projectile.owner && updateVelocity)
            {
                Vector2 oldVelocity = Projectile.velocity;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld), .15f);

                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }

    class StormsentBee : CommonBeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormsent Bee");
            Main.projFrames[Type] = 4;
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f), DustType<PixelatedGlow>(), Projectile.velocity * 0.25f, 0, new Color(120, 200, 200, 0), 0.1f);
        }

        public override void OnKill(int timeLeft)
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<PixelSmokeColor>(), Main.rand.NextVector2Circular(1f, 1f),
                    Main.rand.Next(125, 180), new Color(100, 200, 200, 0), Main.rand.NextFloat(0.05f, 0.1f));

            dust.rotation = Main.rand.NextFloat(6.28f);
            dust.customData = new Color(100, 200, 200, 0);
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                effect.Parameters["repeats"].SetValue(2);

                effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);
                effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                Color color = new Color(10, 80, 255, 0);

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null,
                    Color.White, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.35f, 0f, 0f);

                color = new Color(255, 255, 255, 0);

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null,
                    Color.White, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.35f, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            });

            Rectangle sourceRectangle = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() / 2f,
                Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null,
                Color.White with { A = 0 } * 0.35f, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.2f, 0f, 0f);

            return false;
        }
    }

    // basically the one earlier with different behavior
    // no im not doing it all in one class thats dumb
    [JITWhenModsEnabled("CalamityMod")]
    class StormsentSceptreHoldoutAlt : StormsentSceptreHoldout
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public float rot;

        protected override void ExtraAI()
        {
            if (Shot)
            {
                float progress = 1f - Projectile.timeLeft / 25f;

                float recoilDist = -14f;
                float recoilRot = -0.65f;

                if (progress < 0.25f)
                {
                    float lerper = progress / 0.25f;

                    ArmOffset += Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0f, recoilDist, EaseFunction.EaseCircularOut.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(0f, recoilRot * Projectile.direction, EaseFunction.EaseCircularOut.Ease(lerper));
                }
                else
                {
                    float lerper = (progress - 0.25f) / 0.75f;

                    ArmOffset += Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(recoilDist, 0f, EaseFunction.EaseCircularIn.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(recoilRot * Projectile.direction, 0f, EaseFunction.EaseCircularIn.Ease(lerper));
                }
            }
            else
            {
                if (Timer % 3 == 0)
                    Owner.Bombus().shakeTimer += 2;

                float lerper = EaseFunction.EaseQuarticOut.Ease(1f - Timer / UseTime);

                Vector2 pos = GemPosition + new Vector2(0f, -100f).RotatedBy(Projectile.rotation + MathHelper.TwoPi * lerper) * lerper;

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), Vector2.Zero, 0, new Color(100, 200, 200, 0), 0.1f);

                pos = GemPosition - new Vector2(0f, -100f).RotatedBy(Projectile.rotation + MathHelper.TwoPi * lerper) * lerper;

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), Vector2.Zero, 0, new Color(100, 200, 200, 0), 0.1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;

            Texture2D gemBlur = Request<Texture2D>(Texture + "_GemBlur").Value;

            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;


            float rotation = Projectile.rotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0f) + MathHelper.PiOver4 * Projectile.spriteDirection;

            float fade = 0f;
            if (Timer < 8f)
                fade = Timer / 8f;
            else
                fade = 1f;

            float opacity = Timer / UseTime;

            Vector2 offset = Main.rand.NextVector2CircularEdge(1f * opacity, 1f * opacity);

            position += offset;

            if (Shot)
            {
                if (Projectile.timeLeft < 8f)
                    fade = EaseFunction.EaseCircularIn.Ease(Projectile.timeLeft / 8f);

                if (Projectile.timeLeft > 10)
                    opacity = (Projectile.timeLeft - 10) / 15f;
                else
                    opacity = 0f;
            }

            opacity *= fade;

            Main.spriteBatch.Draw(texGlow, position, null, new Color(120, 200, 200, 0) * opacity * 0.5f, rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(tex, position, null, lightColor * fade, rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(gemBlur, position, null, Color.White with { A = 0 } * opacity, rotation, gemBlur.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
            {
                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                effect.Parameters["repeats"].SetValue(2);

                effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);
                effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                Color color = new Color(120, 200, 200, 0) * opacity * 0.1f;

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, GemPosition - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.8f * opacity, 0f, 0f);

                color = Color.White with { A = 0 } * opacity * 0.55f;

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, GemPosition - Main.screenPosition, null, color, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.2f * opacity, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            });


            return false;
        }

        protected override void Shoot()
        {
            Vector2 pos = GemPosition;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Projectile.velocity * 15f,
                ProjectileType<StormsentLightningBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(pos, DustType<PixelImpactLineDust>(), Main.rand.NextVector2Circular(15f, 15f), 0, new Color(15, 70, 120, 0), 0.075f);

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(4f, 4f), 0, new Color(120, 200, 200, 0), 0.2f);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelatedGlow>(), Projectile.velocity * Main.rand.NextFloat(2f, 8f), 0, new Color(120, 200, 200, 0), 0.2f);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelImpactLineDust>(), Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(5f, 20f), 0, new Color(15, 70, 120, 0), 0.09f);
            }

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(pos, DustType<ElectricityDust>(), Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(5f, 10f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 3.5f));

                Dust.NewDustPerfect(pos, DustType<ElectricityDust>(), Projectile.velocity.RotatedByRandom(0.35f)
                    * Main.rand.NextFloat(5f, 20f) + Main.rand.NextVector2CircularEdge(1f, 1f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 3f)).customData = 1f;

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<ElectricSmoke>(), Projectile.velocity * Main.rand.NextFloat(1f, 5f) + Main.rand.NextVector2Circular(2.5f, 2.5f),
                    Main.rand.Next(125, 180), new Color(100, 200, 200, 0), Main.rand.NextFloat(0.05f, 0.1f));

                dust.rotation = Main.rand.NextFloat(6.28f);
            }

            Owner.Bombus().AddShake(9);
            Owner.Bombus().AddDirectionalShake(-Projectile.velocity * 5f);

            updateVelocity = false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class StormsentLightningBolt : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<NPC> hitNPCs = new();
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        private Vector2 oldCenter;

        public int direction = -1;
        public float rot;
        public bool changeDir;
        public int changeDirTimer;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
        }

        public override void SafeSetDefaults()
        {
            Projectile.Size = new Vector2(24);

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (changeDirTimer > 0)
                changeDirTimer--;

            if (oldCenter == Vector2.Zero)
                oldCenter = Projectile.Center;

            if (Projectile.timeLeft < 60)
                Projectile.velocity *= 0.85f;

            if (Projectile.velocity.Length() < 1f)
                Projectile.timeLeft -= 5;

            if (Main.rand.NextBool(2) && changeDirTimer <= 0)
            {
                if (changeDir)
                {
                    rot = Main.rand.NextFloat(0.3f, 0.7f) * direction;

                    Projectile.velocity = Projectile.velocity.RotatedBy(rot);
                    changeDir = false;
                    direction *= -1;
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-rot);
                    changeDir = true;
                }
            }

            if (Projectile.timeLeft > 45)
            {
                if (Main.rand.NextBool(9))
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<ElectricityDust>(), Projectile.velocity * Main.rand.NextFloat(3f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 2f)).customData = 0.2f;

                if (Main.rand.NextBool(5))
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedGlow>(), Projectile.velocity * Main.rand.NextFloat(0.5f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(0.25f, 0.35f));

                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<ElectricSmoke>(), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.1f) + Main.rand.NextVector2Circular(1f, 1f),
                        0, new Color(70, 180, 220, 0), Main.rand.NextFloat(0.15f, 0.3f));

                    dust.rotation = Main.rand.NextFloat(6.28f);
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitNPCs.Add(target);

            target.AddBuff<Stormsurged>(240);

            Main.player[Projectile.owner].Bombus().AddShake(5);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Projectile.DirectionTo(oldCenter);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<ElectricityDust>(),
                    Main.rand.NextVector2Circular(15f, 15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 2f)).customData = 0.2f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<ElectricityDust>(),
                    velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 2f)).customData = 0.2f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedGlow>(),
                    velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(0.25f, 0.35f));

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<ElectricSmoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 2f),
                        0, new Color(100, 200, 200, 0), Main.rand.NextFloat(0.1f, 0.25f));

                dust.rotation = Main.rand.NextFloat(6.28f);
            }

            NPC closest = Main.npc.Where(n => !hitNPCs.Contains(n) && n != target && n.CanBeChasedBy() && n.Distance(Projectile.Center) < 500f).OrderBy(n => n.Distance(Projectile.Center)).FirstOrDefault();

            if (closest != default)
            {
                oldCenter = Projectile.Center;
                Projectile.timeLeft = 90;
                Projectile.velocity = Projectile.DirectionTo(closest.Center) * 15f;
                changeDirTimer = 10;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<ElectricityDust>(),
                    Main.rand.NextVector2Circular(15f, 15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(1.5f, 2f)).customData = 0.2f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedGlow>(),
                    Main.rand.NextVector2Circular(15f, 15f), 0, new Color(15, 70, 120, 0), Main.rand.NextFloat(0.25f, 0.35f));

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<ElectricSmoke>(), Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 2.5f),
                        0, new Color(100, 200, 200, 0), Main.rand.NextFloat(0.15f, 0.3f));

                dust.rotation = Main.rand.NextFloat(6.28f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 25; i++)
                {
                    cache.Add(Projectile.Center + Projectile.velocity);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 25)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 25, new TriangularTip(190), factor => (float)(4f * (factor < 0.25f ? factor / 0.25f : factor > 0.75f ? (factor - 0.75) / 0.25f : 1f)), factor =>
            {
                float lerper = Projectile.timeLeft / 90f;

                return new Color(15, 70, 120, 0) * lerper * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 25, new TriangularTip(190), factor => (float)(6f * (factor < 0.25f ? factor / 0.25f : factor > 0.75f ? (factor - 0.75) / 0.25f : 1f)), factor =>
            {
                float lerper = Projectile.timeLeft / 90f;

                return new Color(15, 70, 120, 0) * 0.65f * lerper * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());

                // !!! IMPORTANT WHEN PIXELIZING, MAKE SURE TO USE Main.GameViewMatrix.EffectMatrix IMPORTANT !!!

                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/LightningTrail").Value);

                trail2?.Render(effect);
            });
        }
    }
}
