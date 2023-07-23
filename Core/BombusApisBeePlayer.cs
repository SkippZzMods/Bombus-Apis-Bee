using BombusApisBee.Items.Other.OnPickupItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace BombusApisBee.Core
{
    public partial class BombusApisBeePlayer : ModPlayer
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

        public bool HoneyLaser;
        public int HoneyLaserCharge;
        public const int HONEY_LASER_CHARGE_MAX = 10000;

        public bool HoneyTeleport;
        public int HoneyTeleportCooldown;

        public bool WaspArmorSet;
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
        public bool LihzardRelic;
        public int LihzardRelicTimer;
        public bool HasRottenHoneycombShard;
        public bool HasHemocombShard;
        public bool HasHellcombShard;
        public bool JustActivatedArmorSetBonus;
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

            if (!HoneyLaser)
                HoneyLaserCharge--;
            HoneyLaserCharge = Utils.Clamp(HoneyLaserCharge, 0, HONEY_LASER_CHARGE_MAX);
            HoneyLaser = false;

            HoneyTeleport = false;
            if (HoneyTeleportCooldown > 0)
                HoneyTeleportCooldown--;

            WaspArmorSet = false;
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
            LihzardRelic = false;
            if (LihzardRelicTimer > 0)
                LihzardRelicTimer--;
            HasRottenHoneycombShard = false;
            HasHemocombShard = false;
            HasHellcombShard = false;
            JustActivatedArmorSetBonus = false;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (LihzardRelicTimer > 0)
                modifiers.SourceDamage *= 1.1f;
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
            if (BombusApisBee.HoneyManipulatorHotkey.JustPressed && modPlayer2.BeeResourceCurrent == modPlayer2.BeeResourceMax2 && modPlayer2.BeeResourceReserved < modPlayer2.BeeResourceMax2 && Main.myPlayer == Player.whoAmI && HoneyManipulator && !Player.HasBuff<HoneyManipulatorCooldown>())
            {
                Player.AddBuff(ModContent.BuffType<HoneyManipulatorCooldown>(), 5400, false);
                int healamount = (int)((modPlayer2.BeeResourceMax2 - modPlayer2.BeeResourceReserved) * 0.65f);
                Player.Heal(healamount);
                modPlayer2.BeeResourceCurrent -= healamount;
                modPlayer2.BeeResourceRegenTimer = -300;
            }

            if (LihzardRelic && BombusApisBee.LihzardianRelicHotkey.JustPressed && !Player.HasBuff<LihzardianHornetRelicCooldown>())
            {
                SoundID.Item74.PlayWith(Player.Center, -0.15f, 0.1f, 1.25f);
                Player.AddBuff(ModContent.BuffType<LihzardianHornetRelicCooldown>(), 2700, false);
                LihzardRelicTimer = 480;

                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.25f, 4.25f), 0, new Color(245, 245, 149), 0.55f);

                    Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5.25f, 5.25f), 0, new Color(222, 173, 40), 0.55f);
                }
            }
        }

        public override void UpdateEquips()
        {
            if (RoyalJelly)
                Player.Hymenoptra().BeeResourceMax2 += 25;

            if (LihzardRelicTimer > 0)
            {
                Player.IncreaseBeeCrit(12);
                Player.IncreaseBeeDamage(0.12f);
                Player.IncreaseBeeUseSpeed(0.12f);

                float lerper = MathHelper.Lerp(85f, 5f, 1f - LihzardRelicTimer / 480f);
                Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2CircularEdge(lerper, lerper), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(0.25f, 0.25f), 0, new Color(222, 173, 40), 0.45f);

                Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2CircularEdge(lerper, lerper), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(0.75f, 0.75f), 0, new Color(245, 245, 149), 0.35f);
            }
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

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                if (RetinaReleaser)
                {
                    Player.AddBuff(ModContent.BuffType<CthulhuEnraged>(), Main.rand.Next(new int[] { 240, 300, 360, 420 }));
                    for (int i = 0; i < Main.rand.Next(2, 5); i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<RetinaReleaser>())), Player.Center,
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5, 6), ModContent.ProjectileType<CthulhuBee>(), Player.ApplyHymenoptraDamageTo((int)(info.Damage * 0.5f)), 2.5f, Player.whoAmI);
                    }
                }
            }

            if (enchantedhoney)
            {
                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                {
                    Item item = Main.item[Item.NewItem(Player.GetSource_Accessory(new Item(ModContent.ItemType<EnchantedApiary>())), Player.getRect(), ModContent.ItemType<HoneyPickup>())];

                    item.noGrabDelay = 60;

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI, 1f);
                    }
                }
            }

            if (HimenApiary)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Item item = Main.item[Item.NewItem(Player.GetSource_Accessory(new Item(ModContent.ItemType<HimensApiary>())), Player.getRect(), ModContent.ItemType<HoneyPickup2>())];

                    item.noGrabDelay = 60;

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI, 1f);
                    }
                }
            }
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

        public override void UpdateBadLifeRegen()
        {
            if (LihzardRelicTimer > 0)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 16;
            }
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

        public void AddShake(int amount, bool clamped = true)
        {
            if (clamped)
            {
                if (shakeTimer < amount)
                    shakeTimer = amount;
            }
            else
                shakeTimer += amount;
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (improvedhoneyskull)
                Player.AddBuff(ModContent.BuffType<ImprovedHoney>(), 360, true);
            base.OnHitAnything(x, y, victim);
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
        {
            if (MarkedNPC != null)
            {
                int whoAmI = target.whoAmI;
                if (target.realLife >= 0)
                    whoAmI = target.realLife;

                int markedWhoAmI = MarkedNPC.whoAmI;
                if (MarkedNPC.realLife >= 0)
                    markedWhoAmI = MarkedNPC.realLife;

                if (MarkedTimer > 0 && Main.npc[whoAmI] == Main.npc[markedWhoAmI])
                {
                    modifiers.SourceDamage *= 1.25f;

                    if (Main.rand.NextFloat() < ((item.crit + Player.GetTotalCritChance<HymenoptraDamageClass>()) * 2 / 100))
                        modifiers.SetCrit();
                }
                else if (MarkedTimer > 0)
                    modifiers.SourceDamage *= 0.85f;
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            if (MarkedNPC != null)
            {
                int whoAmI = target.whoAmI;
                if (target.realLife >= 0)
                    whoAmI = target.realLife;

                int markedWhoAmI = MarkedNPC.whoAmI;
                if (MarkedNPC.realLife >= 0)
                    markedWhoAmI = MarkedNPC.realLife;

                if (MarkedTimer > 0 && Main.npc[whoAmI] == Main.npc[markedWhoAmI])
                {
                    modifiers.SourceDamage *= 1.25f;

                    if (Main.rand.NextFloat() < ((proj.CritChance * 2) / 100))
                        modifiers.SetCrit();
                }
                else if (MarkedTimer > 0)
                    modifiers.SourceDamage *= 0.85f;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            bool crit = hit.Crit;
            int damage = damageDone;

            if (target == MarkedNPC && target.life <= 0)
            {
                if (Player.HasBuff<BrokenScope>())
                    Player.ClearBuff(ModContent.BuffType<BrokenScope>());
            }

            if (proj.CountsAsClass<HymenoptraDamageClass>() && !NPCID.Sets.ProjectileNPC[target.type])
            {
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
                        Player.AddBuff(ModContent.BuffType<BrokenScope>(), 900);
                    }

                if (NectarVial && crit && NectarVialCooldown <= 0)
                {
                    Player.Heal((int)Utils.Clamp(1 + damage * 0.25f, 1, 8));
                    NectarVialCooldown = 90;
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
                    target.AddBuff<Frostbroken>(900);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType.CountsAsClass<HymenoptraDamageClass>() && !NPCID.Sets.ProjectileNPC[target.type] && target.CanBeChasedBy())
            {
                if (HoneyLaser && !Player.HasBuff<HoneyLaserCooldown>())
                    HoneyLaserCharge = Utils.Clamp(HoneyLaserCharge + damageDone, 0, HONEY_LASER_CHARGE_MAX);
            }
        }
    }
}

