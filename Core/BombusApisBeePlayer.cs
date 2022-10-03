﻿using BombusApisBee.BeeDamageClass;
using BombusApisBee.Buffs;
using BombusApisBee.Dusts;
using BombusApisBee.Items.Accessories.BeeKeeperDamageClass;
using BombusApisBee.Items.Armor.BeeKeeperDamageClass;
using BombusApisBee.Items.Weapons.BeeKeeperDamageClass;
using BombusApisBee.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace BombusApisBee.Core
{
    public class BombusApisBeePlayer : ModPlayer
    {
        public bool RetinaReleaser;
        public bool BeeShoot;
        public bool BeeArrowShoot;
        public bool leafbee;
        public bool squire;
        public bool hivemind;
        public bool queenroar;
        public int shakeTimer = 0;
        public bool improvedhoney;
        public const int PollenMax = 1;
        public int Pollen;
        public bool BeeKeeper;
        public int squiredamage;
        public bool improvedhoneyskull;
        public bool IgnoreWater;
        public bool enchantedhoney;
        public bool HimenApiary;
        public bool HoneyBee;
        public bool HoneyHoarderSet;
        public bool HoneyCrit;
        public bool WaspArmorSet;
        public bool ChloroSet;
        public bool SkeletalSet;
        public int SkeletalHornetWhoAmI;
        public float wingFlightTimeBoost;
        public bool BeeSniperSet;
        public NPC MarkedNPC;
        public int MarkedTimer;
        public bool HoneyLocket;
        public bool NectarVial;
        public int NectarVialCooldown;
        public bool HoneyedHeart;
        public bool HoneyManipulator;
        public bool LivingFlower;
        public int LivingFlowerRot;
        public bool HeartOfNectar;
        public int GlacialstruckCooldown;
        public int OcularCooldown;
        public bool FrozenStinger;
        public bool RoyalJelly;
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return new[] { new Item(ModContent.ItemType<Honeycomb>(), 1) };
        }
        public override void ResetEffects()
        {
            RetinaReleaser = false;
            BeeShoot = false;
            BeeArrowShoot = false;
            leafbee = false;
            squire = false;
            hivemind = false;
            queenroar = false;
            improvedhoney = false;
            squiredamage = 0;
            improvedhoneyskull = false;
            IgnoreWater = false;
            enchantedhoney = false;
            HimenApiary = false;
            HoneyBee = false;
            HoneyHoarderSet = false;
            HoneyCrit = false;
            WaspArmorSet = false;
            ChloroSet = false;
            SkeletalSet = false;
            wingFlightTimeBoost = 1f;
            BeeSniperSet = false;
            HoneyLocket = false;
            if (MarkedNPC != null && !MarkedNPC.active)
                MarkedNPC = null;
            if (--MarkedTimer == 1)
                MarkedNPC = null;
            NectarVial = false;
            HoneyedHeart = false;
            HoneyManipulator = false;
            LivingFlower = false;
            HeartOfNectar = false;
            if (NectarVialCooldown > 0)
                NectarVialCooldown--;
            if (GlacialstruckCooldown > 0)
                GlacialstruckCooldown--;
            if (OcularCooldown > 0)
                OcularCooldown--;
            FrozenStinger = false;
        }

        public override void LoadData(TagCompound tag)
        {
            RoyalJelly = tag.GetBool("RoyalJelly");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["RoyalJelly"] = RoyalJelly;
        }

        public static BombusApisBeePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<BombusApisBeePlayer>();
        }
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inHoney && attempt.heightLevel >= 1 && attempt.waterTilesCount >= 10 && attempt.fishingLevel >= 35f && NPC.downedBoss1)
            {
                if (Main.rand.NextFloat() < (attempt.fishingLevel * 0.00075f))
                    itemDrop = ModContent.ItemType<BeeFinTuna>();
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var modPlayer2 = BeeDamagePlayer.ModPlayer(Player);
            if (BombusApisBee.HoneyManipulatorHotkey.JustPressed && modPlayer2.BeeResourceCurrent == modPlayer2.BeeResourceMax2 && Main.myPlayer == Player.whoAmI && HoneyManipulator && !Player.HasBuff<HoneyManipulatorCooldown>())
            {
                Player.AddBuff(ModContent.BuffType<HoneyManipulatorCooldown>(), 5400, false);
                int healamount = (int)(modPlayer2.BeeResourceMax2 * 0.65f);
                Player.Heal(healamount);
                modPlayer2.BeeResourceCurrent -= healamount;
                modPlayer2.BeeResourceRegenTimer = -300;
            }
        }

        public override void UpdateEquips()
        {
            if (RoyalJelly)
                Player.Hymenoptra().BeeResourceMax2 += 15;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Player.wingTimeMax > 0)
                Player.wingTimeMax = (int)(Player.wingTimeMax * wingFlightTimeBoost);

            if (LivingFlower)
                LivingFlowerRot++;
            else
                LivingFlowerRot = 0;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (HoneyHoarderSet && Main.rand.NextBool(15))
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Honey2, 0f, 0f, 10, default, 1.1f);
                Dust.NewDust(Player.position, Player.width, Player.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 10, default, 0.9f);
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                if (RetinaReleaser)
                {
                    Player.AddBuff(ModContent.BuffType<CthulhuEnraged>(), Main.rand.Next(new int[] { 240, 300, 360, 420 }));
                    for (int i = 0; i < Main.rand.Next(2, 5); i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<RetinaReleaser>())), Player.Center,
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5, 6), ModContent.ProjectileType<CthulhuBee>(), Player.ApplyHymenoptraDamageTo((int)(damage * 0.5f)), 2.5f, Player.whoAmI);
                    }
                }
            }
            base.PostHurt(pvp, quiet, damage, hitDirection, crit, cooldownCounter);
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 Speed = velocity;
            Vector2 ShootSpeed = Vector2.Clamp(Speed, Vector2.One * -6, Vector2.One * 6);
            if (item.CountsAsClass<HymenoptraDamageClass>() && Player.whoAmI == Main.myPlayer)
            {
                if (BeeShoot)
                {
                    Vector2 perturbedSpeed = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(5));
                    if (Main.rand.NextBool(2))
                    {
                        Projectile.NewProjectile(source, Player.Center, perturbedSpeed, ModContent.ProjectileType<SkeletalBee>(), (int)(damage * 0.25f), knockback, Player.whoAmI);
                    }
                }

                if (BeeArrowShoot)
                {
                    int numberProjectiles = 1 + Main.rand.Next(2);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = Vector2.Normalize(position - Main.MouseWorld) * 16f;
                        Projectile.NewProjectileDirect(source, position, perturbedSpeed.RotatedByRandom(MathHelper.ToRadians(6f)), ProjectileID.BeeArrow, damage * 1 / 2, knockback, Player.whoAmI).
                            DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
                    }
                    SoundEngine.PlaySound(SoundID.Item97, position);
                }

                if (leafbee)
                {
                    Vector2 perturbedSpeed = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(5));
                    if (Main.rand.NextBool(2))
                    {
                        Projectile.NewProjectile(source, Player.Center, perturbedSpeed, ModContent.ProjectileType<ChlorophyteBee>(), damage * 1 / 2, knockback, Player.whoAmI);
                    }
                }
            }
            return true;
        }
        public override void UpdateLifeRegen()
        {
            var BeeDamagePlayer = Player.GetModPlayer<BeeDamagePlayer>();
            if (BeeDamagePlayer.BeeResourceCurrent >= BeeDamagePlayer.BeeResourceMax2 * 0.5f)
            {
                Player.lifeRegen += 2;
            }
            if (improvedhoney)
            {
                Player.lifeRegen += 10;
            }
            if (HeartOfNectar)
            {
                Player.lifeRegen += 4;
                Player.lifeRegenTime += 4;
            }
            base.UpdateLifeRegen();
        }


        public override void ModifyScreenPosition()
        {
            if (shakeTimer > 0)
            {
                shakeTimer--;
                Vector2 shake = new Vector2(Main.rand.NextFloat(shakeTimer), Main.rand.NextFloat(shakeTimer));
                Main.screenPosition += shake;
            }
        }
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (improvedhoneyskull)
                Player.AddBuff(ModContent.BuffType<ImprovedHoney>(), 360, true);
            base.OnHitAnything(x, y, victim);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (MarkedTimer > 0 && target == MarkedNPC)
            {
                damage = (int)(damage * 1.25f);

                if (!crit)
                    crit = Main.rand.NextFloat() < ((item.crit + Player.GetTotalCritChance<HymenoptraDamageClass>()) * 2 / 100);
            }
            else if (MarkedTimer > 0 && MarkedNPC != null)
                damage = (int)(damage * 0.85f);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (MarkedTimer > 0 && target == MarkedNPC)
            {
                damage = (int)(damage * 1.25f);

                if (!crit)
                    crit = Main.rand.NextFloat() < ((proj.CritChance * 2) / 100);
            }
            else if (MarkedTimer > 0 && MarkedNPC != null)
                damage = (int)(damage * 0.85f);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.CountsAsClass<HymenoptraDamageClass>() && Player.whoAmI == Main.myPlayer && !NPCID.Sets.ProjectileNPC[target.type])
            {
                if (HoneyCrit && crit)
                {
                    if (proj.type == ModContent.ProjectileType<HoneyHoming>())
                    {
                        return;
                    }
                    Vector2 vel = Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(proj.GetSource_OnHit(target), target.position, vel, ModContent.ProjectileType<HoneyHoming>(), 15 + proj.damage * 1 / 4, proj.knockBack, Player.whoAmI);
                }

                if (ChloroSet && proj.type != ModContent.ProjectileType<ChloroHoney>())
                {
                    var globalNPC = target.GetGlobalNPC<ChloroInfestedGlobalNPC>();
                    if (globalNPC.ChloroStacks >= 10)
                    {
                        if (crit && Player.ownedProjectileCounts[ModContent.ProjectileType<ChloroHoney>()] <= 10)
                        {
                            globalNPC.ChloroStacks = 0;
                            globalNPC.DebuffTime = 0;
                            for (int i = 0; i < Main.rand.Next(2, 4); i++)
                            {
                                Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(4f, 5f), ModContent.ProjectileType<ChloroHoney>(), 10 + (int)(proj.damage * 0.55f), proj.knockBack * 0.5f, Player.whoAmI);
                            }
                        }
                    }
                    else if (Main.rand.NextFloat() < 0.5f)
                    {
                        globalNPC.ChloroStacks++;
                        globalNPC.DebuffTime = 600;
                    }
                }

                if (SkeletalSet && (crit || target.life <= 0))
                {
                    if (Main.projectile[SkeletalHornetWhoAmI].ModProjectile is SkeletalHornetProjectile hornet)
                    {
                        if (hornet.EnrageTimer <= 0)
                        {
                            hornet.EnrageTimer = 600;
                            hornet.EnrageTransitionTimer = 60;
                        }
                        else
                            hornet.EnrageTimer += 30;
                    }
                }

                if (BeeSniperSet)
                    if (Main.rand.NextFloat() < 0.2f && !Player.HasBuff<BrokenScope>())
                    {
                        target.GetGlobalNPC<MarkedGlobalNPC>().marked = true;
                        target.GetGlobalNPC<MarkedGlobalNPC>().markedDuration = 600;
                        MarkedNPC = target;
                        MarkedTimer = 600;
                        Player.AddBuff(ModContent.BuffType<BrokenScope>(), 1200);
                    }

                if (NectarVial && crit && NectarVialCooldown <= 0)
                {
                    Player.Heal((int)Utils.Clamp(1 + damage * 0.25f, 1, 8));
                    NectarVialCooldown = 90;
                }

                if (HoneyedHeart && !target.SpawnedFromStatue)
                {
                    if (crit && Main.rand.NextFloat() < 0.75f && Player.ownedProjectileCounts[ModContent.ProjectileType<BeeResourceIncreaseProjectile>()] < 9)
                    {
                        Vector2 shootVelocity = target.Center.DirectionTo(Player.Center) * 10f;
                        Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, shootVelocity, ModContent.ProjectileType<BeeResourceIncreaseProjectile>(), 0, 1f, Player.whoAmI, 0, 1);
                    }
                }

                if (target.HasBuff<Glacialstruck>() && GlacialstruckCooldown <= 0 && crit)
                {
                    GlacialstruckCooldown = 240;
                    for (int i = 0; i < 3; i++)
                    {
                        int beeDamage = Utils.Clamp(proj.damage, 10, 20);
                        Projectile.NewProjectile(proj.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<FrostedBee>(), beeDamage, 0f, Player.whoAmI);
                    }

                    Projectile.NewProjectile(proj.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<BasicExplosionDebuff>(), Player.ApplyHymenoptraDamageTo(17), 1f, Player.whoAmI, 25f, BuffID.Frostburn);

                    SoundID.Item120.PlayWith(target.Center, pitch: 0.15f);

                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<GlowFastDecelerate>(), 
                            Main.rand.NextVector2Circular(5f, 5f), newColor: new Color(153, 212, 242, 150)).scale = 0.4f;

                        Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.DustType<GlowFastDecelerate>(),
                            Main.rand.NextVector2CircularEdge(5f, 5f), newColor: new Color(153, 212, 242, 150)).scale = 0.5f;

                        Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<FrostedDust>(),
                            Main.rand.NextVector2Circular(5f, 5f), Scale: 1.2f).noGravity = true;

                        Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<FrostedDust>(),
                            Main.rand.NextVector2Circular(5f, 5f), Scale: 1.2f);

                        for (int k = 0; k < 5; k++)
                        {
                            Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Gas>(),
                                Main.rand.NextVector2Circular(3f, 3f), newColor: new Color(255, 255, 255) * 0.85f).scale = Main.rand.NextFloat(3.5f, 5.5f);
                        }
                    }
                }

                if (target.HasBuff<NectarGlazed>() && Main.rand.NextFloat() < 0.1f)
                {
                    for (int i = 0; i < Main.rand.Next(1, 4); i++)
                    {
                        Projectile.NewProjectile(proj.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<NectarHealingBolt>(), 0, 0f, Player.whoAmI);
                    }
                    target.DelBuff(target.FindBuffIndex(ModContent.BuffType<NectarGlazed>()));
                    BeeUtils.DrawDustImage(target.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0.25f, ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HoneyDustImage").Value, 1f, 0, new Color(255, 255, 150), rot: 0);

                    for (int i = 0; i < 45; ++i)
                    {
                        float angle2 = 6.28f * (float)i / (float)45;
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 4.25f, 0, new Color(255, 255, 150), 1.15f);
                    }
                }

                if (FrozenStinger && crit)
                    target.AddBuff<Frostbroken>(240);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (WaspArmorSet && proj.type == ProjectileID.Stinger)
                damage = damage / 2;
        }
    }
}

