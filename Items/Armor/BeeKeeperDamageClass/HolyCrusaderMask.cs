using BombusApisBee.Items.Other.Crafting;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using BombusApisBee.BeeDamageClass;
using System.Linq;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HolyCrusaderMask : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("5% increased damage reduction and hymenoptra damage\nIncreases maximum honey by 40\nIncreases your amount of Bees by 1");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 14;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HolyCrusaderGreaves>() && body.type == ModContent.ItemType<HolyCrusaderArmor>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Conjures a holy shield that attempts to protect the bearer";
            player.GetModPlayer<HolyCrusaderPlayer>().FullArmorSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
            player.endurance += 0.05f;
            player.Hymenoptra().BeeResourceMax2 += 40;
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 18).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient(ModContent.ItemType<Pollen>(), 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    class HolyCrusaderPlayer : ModPlayer
    {
        public bool FullArmorSet;

        public int[] hitProjHitTimer = new int[Main.maxProjectiles + 1];

        public override void ResetEffects()
        {
            FullArmorSet = false;

            for (int i = 0; i < hitProjHitTimer.Length; i++)
            {
                if (hitProjHitTimer[i] > 0)
                    hitProjHitTimer[i]--;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (FullArmorSet && Player.ownedProjectileCounts<HolyShieldProjectile>() <= 0)
                Projectile.NewProjectile(Player.GetSource_Misc("BombusApisBee: Spawn Holy Shield"), Player.Center, Vector2.Zero, ModContent.ProjectileType<HolyShieldProjectile>(), Player.ApplyHymenoptraDamageTo(100), 0f, Player.whoAmI);
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            hitProjHitTimer[proj.whoAmI] = 30;
        }
    }

    class HolyShieldProjectile : ModProjectile
    {
        int deathTimer;
        int hitTimer;
        int numHits;
        Player owner => Main.player[Projectile.owner];
        Vector2 collisionPoint(Player player) { return player.Center + -(Projectile.rotation + 0.15f).ToRotationVector2() * 65f; }

        public override bool? CanDamage() => deathTimer <= 0 && hitTimer <= 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Shield");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 40;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (deathTimer > 0)
            {
                if (deathTimer == 1)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.1f }, owner.Center);
                    BeeUtils.CircleDust(collisionPoint(owner), 30, ModContent.DustType<Dusts.GlowFastDecelerate>(), 2f, 0, new Color(255, 205, 100), 0.5f);
                }
                deathTimer--;
            }

            if (hitTimer > 0)
                hitTimer--;

            Projectile.spriteDirection = -1;

            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<HolyCrusaderPlayer>().FullArmorSet && !player.dead)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.AngleTo(player.Center);
            Projectile.ai[0]++;
            double deg = Projectile.ai[0] * 1.25f;
            double rad = deg * (Math.PI / 180);
            double dist = 64;
            Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

            if (Main.rand.NextBool(5) && deathTimer <= 0)
                Dust.NewDustPerfect(collisionPoint(owner) + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(255, 205, 100) * (1f - (deathTimer / 600f)), Main.rand.NextFloat(0.3f, 0.6f));

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (deathTimer <= 0 && hitTimer <= 0 && Vector2.Distance(collisionPoint(owner), proj.Center) < 50f && proj.active && proj.hostile && owner.GetModPlayer<HolyCrusaderPlayer>().hitProjHitTimer[proj.whoAmI] <= 0)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        Dust.NewDustPerfect(proj.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(255, 205, 100), 0.35f);

                        Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(proj.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(proj.Center) * 5f).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.25f, 1.75f), 0, new Color(255, 205, 100), 0.55f);

                        Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(proj.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(proj.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(255, 205, 100), 0.75f);
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath6, proj.position);

                    proj.Kill();
                    numHits++;

                    hitTimer = 60;
                }
            }

            if (numHits >= 3)
            {
                numHits = 0;
                deathTimer = 600;

                SoundEngine.PlaySound(SoundID.NPCDeath6 with { Pitch = -0.75f, PitchVariance = 0.15f }, Projectile.position);

                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Pitch = -0.25f, PitchVariance = 0.15f, Volume = 2f }, Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            float mult;
            if (deathTimer >= 585)
                mult = (deathTimer - 585) / 15f;
            else if (deathTimer >= 45)
                mult = 0f;
            else
                mult = 1f - (deathTimer / 45f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            float increase = MathHelper.Lerp(0f, 0.0025f, hitTimer / 60f);
            effect.Parameters["offset"].SetValue(Vector2.Lerp(new Vector2(0.002f), new Vector2(0.0035f), numHits / 3f) + new Vector2(increase));
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);
            Color color = Color.Lerp(new Color(225, 205, 100, 0), new Color(200, 165, 80, 0) * 0.75f, (float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 3f)) * mult;
            if (hitTimer > 0)
                color = Color.Lerp(new Color(255, 255, 255, 0) * mult, color, 1f - hitTimer / 60f);
                

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, owner.gfxOffY), null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(bloomTex, collisionPoint(owner) + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, (Color.Lerp(new Color(225, 205, 100, 0), new Color(200, 165, 80, 0) * 0.75f, (float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 3f)) * 0.25f) * mult, 0f, bloomTex.Size() / 2f, 1.85f, 0f, 0f);
            
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(collisionPoint(owner), targetHitbox.Center.ToVector2()) < 50f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.knockBackResist > 0)
                target.velocity += Main.player[Projectile.owner].DirectionTo(target.Center) * 10f;

            numHits++;
            hitTimer = 60;

            SoundEngine.PlaySound(SoundID.NPCDeath6, target.position);

            for (int x = 0; x < 10; x++)
            {
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(255, 205, 100), 0.35f);

                Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(target.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(target.Center) * 5f).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.25f, 1.75f), 0, new Color(255, 205, 100), 0.55f);

             
                Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(target.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(target.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(255, 205, 100), 0.75f);
            }

            if (numHits >= 3)
                owner.Bombus().shakeTimer += 6;
            else
                owner.Bombus().shakeTimer += 2;
        }
    }
}

