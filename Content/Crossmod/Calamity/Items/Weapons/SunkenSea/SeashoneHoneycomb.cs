using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
using System.IO;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.SunkenSea
{
    public class SeashoneHoneycomb : CalamityDamageItem
    {
        internal float shootRotation;
        internal int shootDirection;
        internal int armDirection;
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<SeashoneHoneycombHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("<left> to throw the honeycomb\n" +
                "Hold <right> to manifest a powerful crystal which imbeds in enemies" +
                "\nStrike enemies with embedded crystals to drive them deeper, eventually shattering the crystal into violent projectiles" +
                "\nThere can only be one crystal embedded in an enemy at once");
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void SafeSetDefaults()
        {
            Item.damage = 25;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<SeashoneHoneycombHoldout>();
            Item.shootSpeed = 15f;
            honeyCost = 2;
            altHoneyCost = 8;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override bool CanUseItem(Player Player)
        {
            if (Player.altFunctionUse == 2)
            {
                Item.UseSound = new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FancySwoosh");
                Item.damage = 25;
                Item.useTime = 50;
                Item.useAnimation = 50;
                Item.shoot = ProjectileType<SeashoneHoneycombHoldout>();
                Item.shootSpeed = 15;
            }
            else
            {
                Item.damage = 18;
                Item.useTime = 22;
                Item.useAnimation = 22;
                Item.shoot = ProjectileType<SeashoneHoneycomb_Thrown>();
                Item.shootSpeed = 25f;
                Item.UseSound = SoundID.Item1;

                shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
                shootDirection = Main.MouseWorld.X < Player.Center.X ? -1 : 1;

                if (armDirection != 1 && armDirection != -1)
                    armDirection = 1;

                armDirection *= -1;
            }

            return base.CanUseItem(Player);
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (Main.myPlayer == player.whoAmI)
                    player.direction = shootDirection;

                float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;
                float rotation = shootRotation * player.gravDir + 1.5707964f;

                if (animProgress < 0.6f)
                {
                    float lerper = animProgress / 0.6f;
                    rotation += MathHelper.Lerp(0f, -1.57f * armDirection, EaseFunction.EaseCircularOut.Ease(lerper)) * player.direction;

                    //Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), DustType<Glow>(), rotation.ToRotationVector2() * 0.5f, 0, Color.Lerp(new Color(130, 200, 70), new Color(55, 180, 220), animProgress), 0.25f);
                }
                else
                {
                    rotation += -1.57f * armDirection * player.direction;
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                for (float k = 0; k < 6.28f; k += 0.2f)
                {
                    float x = (float)Math.Cos(k) * 30;
                    float y = (float)Math.Sin(k) * 10;

                    Dust.NewDustPerfect(position + velocity, DustType<StarDustWhite>(),
                        new Vector2(x, y).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 0.08f, 0, new Color(150, 255, 255, 0), 0.3f).customData = true;

                    Dust.NewDustPerfect(position + velocity * 2f, DustType<StarDustWhite>(),
                        new Vector2(x, y).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 0.05f, 0, new Color(150, 255, 255, 0), 0.3f).customData = true;
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(30)
            .AddIngredient<SeaPrism>(7)
            .AddIngredient<PearlShard>(4)
            .AddIngredient<Navystone>(20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class SeashoneHoneycombHoldout : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public bool flashed = false;
        public int flashTimer = 0;

        public bool shooting;
        public int shootTimer;

        public int fadeInTimer;

        public Vector2 oldCenter;
        public bool CanHold => Owner.HeldItem.ModItem is SeashoneHoneycomb && Main.mouseRight && !Owner.CCed && !Owner.noItems;
        public Player Owner => Main.player[Projectile.owner];
        public Vector2? OwnerMouse => Main.myPlayer == Owner.whoAmI ? Main.MouseWorld : null;
        public ref float CurrentCharge => ref Projectile.ai[0];
        public ref float MaxCharge => ref Projectile.ai[1];
        public float ChargeProgress => CurrentCharge / MaxCharge;
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/SunkenSea/SeashoneHoneycomb";
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.width = 28;
            Projectile.height = 36;

            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Owner.DirectionTo(OwnerMouse.Value);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.netUpdate = true;

            MaxCharge = Owner.itemTime * 2;
        }

        public override void AI()
        {
            if (CurrentCharge < MaxCharge)
                CurrentCharge++;

            if (flashTimer > 0)
                flashTimer--;

            if (fadeInTimer < 25)
                fadeInTimer++;

            if (CurrentCharge == MaxCharge && !flashed)
            {
                Vector2 crystalPos = oldCenter + new Vector2(0f, Owner.gfxOffY) + new Vector2(MathHelper.Lerp(85f, 50f, EaseFunction.EaseBackOut.Ease(ChargeProgress)), 0f).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

                flashTimer = 20;
                flashed = true;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(crystalPos, DustType<StarDustWhite>(),
                            Main.rand.NextVector2Circular(5f, 2f), 0, new Color(150, 255, 255, 0), 0.45f);

                    Dust.NewDustPerfect(crystalPos, DustType<PixelatedGlow>(),
                            Main.rand.NextVector2Circular(5f, 2f), 0, new Color(150, 255, 255, 0), 0.35f);
                }

                SoundID.MaxMana.PlayWith(Projectile.Center);
            }

            if (shooting)
            {
                shootTimer++;

                float progress = shootTimer / 35f;
                Projectile.Center = Owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 15f;
                Projectile.timeLeft = 2;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Owner.ChangeDir(Projectile.velocity.X.DirectionalSign());

                if (progress < 0.05f)
                {
                    float lerper = progress / 0.05f;
                    Projectile.Center += Projectile.velocity * MathHelper.Lerp(0f, -8f, EaseFunction.EaseCircularOut.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(0f, -.15f, EaseFunction.EaseCircularOut.Ease(lerper)) * Owner.direction;
                }
                else
                {
                    float lerper = (progress - 0.05f) / 0.95f;
                    Projectile.Center += Projectile.velocity * MathHelper.Lerp(-8f, 0f, EaseFunction.EaseBackInOut.Ease(lerper));

                    Projectile.rotation += MathHelper.Lerp(-.15f, 0, EaseFunction.EaseBackInOut.Ease(lerper)) * Owner.direction;
                }

                Owner.heldProj = Projectile.whoAmI;

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);

                if (shootTimer > 35)
                    Projectile.Kill();

                return;
            }

            if (CanHold || ChargeProgress <= 0.25f)
            {
                Vector2 crystalPos = oldCenter + new Vector2(0f, Owner.gfxOffY) + new Vector2(MathHelper.Lerp(85f, 50f, EaseFunction.EaseBackOut.Ease(ChargeProgress)), 0f).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(OwnerMouse.Value), 0.1f);
                oldCenter = Projectile.Center;
                Projectile.Center = Owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 15f;

                if (ChargeProgress < 0.9f)
                {
                    if (Main.rand.NextFloat() < 0.65f)
                    {
                        Vector2 dustPos = crystalPos + Main.rand.NextVector2CircularEdge(50f * (1f - ChargeProgress), 50f * (1f - ChargeProgress));

                        Dust.NewDustPerfect(dustPos, DustType<PixelatedGlow>(),
                            dustPos.DirectionTo(crystalPos), 0, new Color(150, 255, 255, 0), 0.15f);
                    }
                }
                else
                {
                    if (Main.rand.NextFloat() < 0.1f)
                    {
                        Dust.NewDustPerfect(crystalPos, DustType<PixelatedGlow>(),
                            Main.rand.NextVector2Circular(5f, 2f), 0, new Color(150, 255, 255, 0), 0.35f);
                    }
                }
            }
            else if (ChargeProgress > 0.25f)
            {
                Vector2 crystalPos = oldCenter + new Vector2(0f, Owner.gfxOffY) + new Vector2(MathHelper.Lerp(85f, 50f, EaseFunction.EaseBackOut.Ease(ChargeProgress)), 0f).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(OwnerMouse.Value), 0.1f);
                oldCenter = Projectile.Center;
                Projectile.Center = Owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 15f;

                if (Main.myPlayer == Owner.whoAmI)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), crystalPos, Projectile.velocity * MathHelper.Lerp(2f, 10f, ChargeProgress),
                        ProjectileType<SeashoneHoneycomb_CrystalBig>(), (int)(Projectile.damage * MathHelper.Lerp(0.7f, 2.5f, ChargeProgress)), Projectile.knockBack, Projectile.owner);

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(crystalPos, DustType<StarDustWhite>(),
                                Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(MathHelper.Lerp(2f, 10f, ChargeProgress)), 0, new Color(150, 255, 255, 0), 0.45f);

                        Dust.NewDustPerfect(crystalPos, DustType<PixelatedGlow>(),
                                Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(MathHelper.Lerp(2f, 10f, ChargeProgress)), 0, new Color(150, 255, 255, 0), 0.35f);
                    }
                }

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FancySwoosh").PlayWith(Projectile.Center, volume: 0.5f);
                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/HeavyWhoosh").PlayWith(Projectile.Center, 0.65f, 0, 0.75f);
                SoundID.Item43.PlayWith(Projectile.Center);

                Owner.Bombus().AddShake(6);

                shooting = true;
            }

            Projectile.timeLeft = 2;
            Owner.ChangeDir(OwnerMouse.Value.X < Owner.Center.X ? -1 : 1);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texBlur = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texWhite = Request<Texture2D>(Texture + "_White").Value;
            Texture2D crystalTex = Request<Texture2D>(Texture + "_CrystalBig").Value;
            Texture2D crystalTexBlur = Request<Texture2D>(Texture + "_CrystalBig_Blur").Value;
            Texture2D crystalTexGlow = Request<Texture2D>(Texture + "_CrystalBig_Glow").Value;
            Texture2D crystalTexWhite = Request<Texture2D>(Texture + "_CrystalBig_White").Value;
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fadeIn;
            Vector2 offset;

            if (fadeInTimer < 25)
            {
                fadeIn = fadeInTimer / 25f;
                offset = new Vector2(MathHelper.Lerp(30f, 0f, EaseFunction.EaseCircularOut.Ease(fadeIn)), Owner.gfxOffY);
            }
            else
            {
                fadeIn = 1f;
                offset = new Vector2(0f, Owner.gfxOffY);
            }

            Main.spriteBatch.Draw(bloomTex, Projectile.Center + offset - Main.screenPosition, null, new Color(150, 255, 255, 0) * 0.35f, Projectile.rotation - MathHelper.PiOver4, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, lightColor * EaseFunction.EaseCircularIn.Ease(fadeIn), Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(texBlur, Projectile.Center + offset - Main.screenPosition, null, lightColor with { A = 0 } * EaseFunction.EaseCircularIn.Ease(fadeIn) * 0.5f, Projectile.rotation, texBlur.Size() / 2f, Projectile.scale, 0f, 0f);

            if (shooting)
                return false;

            Vector2 crystalPos = oldCenter + new Vector2(0f, Owner.gfxOffY) + new Vector2(MathHelper.Lerp(85f, 50f, EaseFunction.EaseBackOut.Ease(ChargeProgress)), 0f).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Main.spriteBatch.Draw(bloomTex, crystalPos - Main.screenPosition, null, new Color(150, 255, 255, 0) * 0.35f, Projectile.rotation - MathHelper.PiOver4, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(crystalTex, crystalPos + Main.rand.NextVector2Circular(1.5f * ChargeProgress, 1.5f * ChargeProgress) - Main.screenPosition,
                null, Color.White * ChargeProgress, Projectile.rotation - MathHelper.PiOver4, crystalTex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(crystalTexBlur, crystalPos + Main.rand.NextVector2Circular(1.5f * ChargeProgress, 1.5f * ChargeProgress) - Main.screenPosition,
                null, Color.White with { A = 0 } * ChargeProgress, Projectile.rotation - MathHelper.PiOver4, crystalTexBlur.Size() / 2f, Projectile.scale, 0f, 0f);

            float lerper = flashTimer / 20f;

            if (flashTimer > 0)
            {
                Main.spriteBatch.Draw(crystalTexWhite, crystalPos - Main.screenPosition,
                    null, Color.White with { A = 0 } * lerper, Projectile.rotation - MathHelper.PiOver4, crystalTexWhite.Size() / 2f, MathHelper.Lerp(1f, 1.15f, 1f - lerper), 0f, 0f);
            }

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class SeashoneHoneycomb_Thrown : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/SunkenSea/SeashoneHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashone Honeycomb");
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;

            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.35f * (Projectile.velocity.X * 0.15f) * Projectile.direction;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 0)
            {
                if (Projectile.velocity.Y < 13f)
                    Projectile.velocity.Y *= 1.085f;
                else
                    Projectile.velocity.Y *= 1.04f;
            }
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.15f);

            if (Main.rand.NextBool(6))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texBlur = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texWhite = Request<Texture2D>(Texture + "_White").Value;
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, lightColor * MathHelper.Lerp(0.5f, 0.25f, i / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, i / (float)Projectile.oldPos.Length), 0, 0);

                Main.spriteBatch.Draw(texBlur, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, lightColor with { A = 0 } * MathHelper.Lerp(0.5f, 0.25f, i / (float)Projectile.oldPos.Length),
                    Projectile.rotation, texBlur.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, i / (float)Projectile.oldPos.Length), 0, 0);
            }

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(150, 255, 255, 0) * 0.3f, Projectile.rotation, bloomTex.Size() / 2f, 2f, 0, 0f);
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(150, 255, 255, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Bombus().AddShake(4);

            Projectile proj = Main.projectile.Where(n => n.active && n.owner == Projectile.owner &&
            n.type == ProjectileType<SeashoneHoneycomb_CrystalBig>() && (n.ModProjectile as SeashoneHoneycomb_CrystalBig).enemyID == target.whoAmI).FirstOrDefault();

            if (proj != default)
            {
                SeashoneHoneycomb_CrystalBig crystal = proj.ModProjectile as SeashoneHoneycomb_CrystalBig;

                if (crystal.timesHit < 3)
                {
                    new SoundStyle("BombusApisBee/Sounds/Item/GoreLight").PlayWith(Projectile.Center, volume: 0.5f);

                    proj.ai[0]++;


                    NPC.HitInfo crystalHit = hit;

                    crystalHit.Damage = 30;
                    crystalHit.Crit = true;

                    target.StrikeNPC(crystalHit);

                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(proj.Center + Main.rand.NextVector2Circular(5f, 5f),
                            DustType<PixelatedGlow>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.15f), 0, new Color(150, 255, 255, 0), 0.15f);

                        Dust.NewDustPerfect(proj.Center + Main.rand.NextVector2Circular(5f, 5f),
                            DustType<StarDustWhite>(), -proj.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.15f), 0, new Color(150, 255, 255, 0), 0.25f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelImpactLineDustGlow>(),
                            (-proj.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelImpactLineDustGlow>(),
                            (-proj.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelImpactLineDustGlow>(),
                            (-proj.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(255, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelImpactLineDustGlow>(),
                            (-proj.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(255, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelatedGlow>(),
                            (-proj.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<PixelatedGlow>(),
                            (-proj.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<StarDustWhite>(),
                            (-proj.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);

                        Dust.NewDustPerfect(proj.Center, DustType<StarDustWhite>(),
                            (-proj.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 0.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.1f);
                    }
                }
                else
                {
                    crystal.Shatter();
                }
            }

            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, volume: 0.25f);
            SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center);

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.15f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(150, 255, 255, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(70, 190, 200, 0), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(150, 255, 255, 0), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), Main.rand.NextVector2Circular(6f, 6f), 0, new Color(150, 255, 255, 0), 0.5f).customData = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            /*SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.85f, PitchVariance = 0.15f }, Projectile.position);
            for (int i = 1; i < 4; i++)
            {
                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("StoneHoneycombGore_" + i).Type).timeLeft = 90;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Bone : DustID.Stone);
            }

            for (int i = 0; i < Main.rand.Next(3); i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    Main.rand.NextVector2Circular(3.5f, 3.5f), Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage / 2), 0f, Projectile.owner);

                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("DriedHoneycombGore_" + Main.rand.Next(1, 4)).Type).timeLeft = 90;
            }*/
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class SeashoneHoneycomb_CrystalBig : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        bool stuck;
        Vector2 offset = Vector2.Zero;
        public int enemyID;

        public int timesHit => (int)Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
            //ProjectileID.Sets.TrailingMode[Type] = 0;
            //ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;

            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.hide = true;

            Projectile.extraUpdates = 1;
        }
        public override bool PreAI()
        {
            if (stuck)
            {
                if (Main.rand.NextBool(30))
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                        DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(70, 190, 200, 0), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;

                }

                NPC target = Main.npc[enemyID];
                Projectile.position = target.position + offset + Projectile.velocity * 0.2f * timesHit;

                if (!target.active)
                    Projectile.Kill();

                return false;
            }

            return base.PreAI();
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 290)
            {
                Projectile.velocity *= 1.12f;
            }
            else
                Projectile.velocity *= 0.95f;

            if (Projectile.velocity.Length() < 0.2f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.15f);

            if (Main.rand.NextBool(4))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.35f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.projectile.Any(n => n.active && n.type == Type && n.owner == Owner.whoAmI && n.ModProjectile
            != null && (n.ModProjectile as SeashoneHoneycomb_CrystalBig).enemyID == target.whoAmI && (n.ModProjectile as SeashoneHoneycomb_CrystalBig).stuck))
            {
                modifiers.FinalDamage *= 1.5f;
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, volume: 1f);
            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center, volume: 0.65f);

            Owner.Bombus().AddShake(10);

            if (!stuck && target.life > 0)
            {
                Projectile.timeLeft = 600;

                stuck = true;
                Projectile.friendly = false;
                Projectile.tileCollide = false;
                enemyID = target.whoAmI;
                offset = Projectile.position - target.position;
                offset -= Projectile.velocity * 1f;
                Projectile.netUpdate = true;
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.15f), 0, new Color(150, 255, 255, 0), 0.15f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<StarDustWhite>(), -Projectile.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.15f), 0, new Color(150, 255, 255, 0), 0.25f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(),
                    (-Projectile.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(),
                    (-Projectile.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(),
                    (-Projectile.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(255, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(),
                    (-Projectile.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(255, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(),
                    (-Projectile.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(),
                    (-Projectile.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                    (-Projectile.velocity.RotatedBy(0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);

                Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                    (-Projectile.velocity.RotatedBy(-0.4f) * Main.rand.NextFloat(0.25f, 1.5f)).RotatedByRandom(0.2f), 0, new Color(155, 255, 255, 0), 0.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texBlur = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texWhite = Request<Texture2D>(Texture + "_White").Value;
            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float lerper = 1f;
            if (Projectile.timeLeft > 250)
            {
                lerper = (Projectile.timeLeft - 250) / 50f;
            }
            else
                lerper = 0f;

            float fadeOut = 1f;

            if (Projectile.timeLeft < 20)
            {
                fadeOut = Projectile.timeLeft / 20f;
            }

            float shakeIntensity = 0.6f * timesHit;

            if (stuck)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(150, 255, 255, 0) * fadeOut, Projectile.rotation, bloomTex.Size() / 2f, 0.5f, 0, 0f);
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * fadeOut, Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0f);

                Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * fadeOut,
                                Projectile.rotation - MathHelper.PiOver4, texBlur.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2CircularEdge(1f, 1f) * shakeIntensity, null, Color.White * fadeOut, Projectile.rotation - MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            if (!stuck)
            {
                Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * MathHelper.Lerp(0.5f, 1f, lerper),
                                Projectile.rotation - MathHelper.PiOver4, texBlur.Size() / 2f, Projectile.scale, 0f, 0f);

                Main.spriteBatch.Draw(texWhite, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * lerper,
                                Projectile.rotation - MathHelper.PiOver4, texWhite.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stuck);
            writer.WriteVector2(offset);
            writer.Write(enemyID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stuck = reader.ReadBoolean();
            offset = reader.ReadVector2();
            enemyID = reader.ReadInt32();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public void Shatter()
        {
            Projectile.Kill();

            Owner.Bombus().AddShake(15);

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FrostHit").PlayWith(Projectile.Center, volume: 0.65f);
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, volume: 1f);
            SoundID.DD2_WitherBeastHurt.PlayWith(Projectile.Center, volume: 0.65f);

            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.One).RotatedByRandom(0.3f) * Main.rand.NextFloat(15f, 20f),
                                       ProjectileType<SeashoneHoneycomb_CrystalSmall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.One);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                     velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 10f), Main.rand.Next(120, 200), new Color(28, 125, 139), Main.rand.NextFloat(0.75f, 1f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(1f, 10f), Main.rand.Next(120, 200), new Color(67, 187, 204), Main.rand.NextFloat(0.75f, 1f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 8f), Main.rand.Next(120, 200), new Color(155, 255, 255), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(1f, 8f), Main.rand.Next(120, 200), new Color(200, 255, 255), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 8f), Main.rand.Next(120, 200), new Color(255, 255, 255), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(),
                        velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 14f), 0, new Color(155, 255, 255, 0), 0.3f);

                Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                   velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 14f), 0, new Color(155, 255, 255, 0), 0.6f).customData = true;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class SeashoneHoneycomb_CrystalSmall : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public override bool? CanDamage() => Projectile.timeLeft < 465;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Crystal");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 8;
            Projectile.tileCollide = false;
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
            float num = 1500f;
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
            if (foundTarget && Projectile.timeLeft < 465)
            {
                Projectile.velocity = (Projectile.velocity * 20f + (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 10f) / 21f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft % 5 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(),
                    Vector2.Zero, 0, new Color(155, 255, 255, 0), 0.1f);

                Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                   Vector2.Zero, 0, new Color(155, 255, 255, 0), 0.1f);
            }

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(155, 255, 255, 0), 0.4f).customData = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Bombus().AddShake(3);

            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, volume: 0.8f);
            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center, volume: 0.45f);

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f), 0, new Color(150, 255, 255, 0), 0.15f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(150, 255, 255, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustType<StarDustWhite>(), Main.rand.NextVector2Circular(6f, 6f), 0, new Color(150, 255, 255, 0), 0.5f).customData = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation - MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(150, 255, 255, 0) * 0.45f, Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0, 0f);

            return false;
        }
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
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 4.5f, factor =>
            Color.Lerp(new Color(28, 125, 139), new Color(67, 187, 204), EaseFunction.EaseQuarticOut.Ease(1f - factor.X)));

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 2f, factor =>
            Color.White);

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
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

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
            });
        }
    }
}
