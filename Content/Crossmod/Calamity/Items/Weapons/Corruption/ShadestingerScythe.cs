using BombusApisBee.Content.Dusts;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Projectiles;
using BombusApisBee.Core.PixelationSystem;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption
{
    [JITWhenModsEnabled("CalamityMod")]
    public class ShadestingerScytheCooldown : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darkheal Cooldown");
            Description.SetDefault("Their souls take time to regenerate proper healing for the taking...");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }

    public class ShadestingerScythe : CalamityDamageItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private int combo = 0;

        int cooldown;
        internal float shootRotation;
        internal int shootDirection;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts<ShadestingerScytheProjectile>() <= 0;

        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger Scythe");
            Tooltip.SetDefault(
                "Rapidly strikes enemies with a flurry of swift stabs and throws, infusing Dark Energy within them\n" +
                "Strike enemies infused with sufficient Dark Energy to rip Shadestung bees from them\n" +
                "Press <right> to rip healing energy from all enemies, ridding them off all their dark energy\n" +
                "Healing has a cooldown of 30 seconds");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<ShadestingerScytheProjectile>();
            Item.shootSpeed = 10f;

            Item.value = Item.sellPrice(gold: 1);

            honeyCost = 2;
            altHoneyCost = 10;
        }

        public override bool AltFunctionUse(Player player) => !player.HasBuff<ShadestingerScytheCooldown>(); 

        public override void UpdateInventory(Player player)
        {
            if (cooldown > 0)
                cooldown--;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ProjectileType<ShadestingerScytheAltProjectile>();
                player.AddBuff<ShadestingerScytheCooldown>(1800); // 30 secs
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, combo);
            combo++;
            if (combo > 2)
            {
                combo = 0;
            }

            return false;
        }

        /*public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RottenMatter>(13).
                AddIngredient<Pollen>(30).
                AddIngredient(ItemID.RottenChunk, 5).
                AddTile(TileID.DemonAltar).
                Register();
        }*/
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadestingerScytheProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Corruption/ShadestingerScythe";

        private Vector2 scaleVec;
        private Vector2 originalVelocity;

        private bool initialized;

        private bool swung;

        private bool flipBlade = true;
        private bool drawAfterImages;
        private bool spawnedProjectile;

        private float maxTimeLeft;
        private float originalDirection;

        private int oldTimeLeft;
        private int pauseTimer;

        private int hits;

        private List<float> oldRotation = new();
        private List<Vector2> oldScale = new();
        private List<Vector2> oldPositions = new();
        private List<bool> oldFlipBlade = new();

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        private Vector2 tipPosition;

        private bool flashed;
        private int flashTimer;

        private float oldRot;

        public float Combo => Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger Scythe");
        }
        public override void SafeSetDefaults()
        {
            if (Combo == 2)
            {
                Projectile.timeLeft = 300;
                Projectile.friendly = true;
                Projectile.hostile = false;
                Projectile.tileCollide = false;
                Projectile.Size = new Vector2(24);
                Projectile.penetrate = -1;

                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 15;

                return;
            }

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(60);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.Bombus().HeldProj = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Combo == 2)
            {
                Owner.Bombus().AddShake(3);

                SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShurikenThrow").PlayWith(Projectile.Center);
                Projectile.ownerHitCheck = false;
                originalVelocity = Projectile.velocity;
                Projectile.timeLeft = 300;
                Projectile.velocity *= 2f;
                Projectile.soundDelay = 0;
                return;
            }

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/HeavyWhoosh").PlayWith(Projectile.Center, volume: 0.5f);

            Projectile.timeLeft = (int)Owner.ApplyHymenoptraSpeedTo(Owner.HeldItem.useAnimation);
            maxTimeLeft = Projectile.timeLeft;
            Projectile.rotation = Owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
            Projectile.netUpdate = true;
            Projectile.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            originalDirection = Projectile.direction;
            Owner.itemTime = Projectile.timeLeft / 2;
            Owner.itemAnimation = Owner.itemTime;
        }

        public override bool ShouldUpdatePosition()
        {
            if (Combo == 2)
                return pauseTimer <= 0;

            return true;
        }

        public override bool PreAI()
        {
            if (pauseTimer > 0)
            {
                Projectile.timeLeft = oldTimeLeft;

                if (Combo == 2)
                    Projectile.rotation = oldRot;

                pauseTimer--;
            }

            return true;
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (Combo != 2)
                UpdateProj();
            else
            {
                if (Projectile.timeLeft % 3 == 0 && pauseTimer <= 0)
                {
                    oldRotation.Add(Projectile.rotation);

                    if (oldRotation.Count > 15)
                        oldRotation.RemoveAt(0);

                    if (Combo == 2)
                    {
                        oldPositions.Add(Projectile.Center);

                        if (oldPositions.Count > 15)
                            oldPositions.RemoveAt(0);
                    }
                    else
                    {
                        oldPositions.Add(Projectile.Center - Owner.Center);

                        if (oldPositions.Count > 15)
                            oldPositions.RemoveAt(0);
                    }

                    oldScale.Add(scaleVec);

                    if (oldScale.Count > 15)
                        oldScale.RemoveAt(0);
                }
            }

            switch (Combo)
            {
                case 0:
                    UpSlash(); break;
                case 1:
                    DownSlash(); break;
                case 2:
                    ThrowSlash(); break;
            }
        }

        private void UpdateProj()
        {
            if (Owner.HeldItem.ModItem is not ShadestingerScythe)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;

            Owner.ChangeDir(Projectile.direction);

            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft && pauseTimer <= 0)
            {
                ManageCaches();
                ManageTrail();
            }

            if (!drawAfterImages)
                return;

            if (Projectile.timeLeft % 3 == 0 && pauseTimer <= 0)
            {
                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 15)
                    oldRotation.RemoveAt(0);

                if (Combo == 2)
                {
                    oldPositions.Add(Projectile.Center);

                    if (oldPositions.Count > 15)
                        oldPositions.RemoveAt(0);
                }
                else
                {
                    oldPositions.Add(Projectile.Center - Owner.Center);

                    if (oldPositions.Count > 15)
                        oldPositions.RemoveAt(0);
                }

                oldScale.Add(scaleVec);
            }
        }

        private void UpSlash()
        {
            drawAfterImages = true;

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            scaleVec = new Vector2(0.9f, 1.1f);
            
            float armRotation = Projectile.velocity.ToRotation() + (originalDirection == -1 ? MathHelper.Pi : 0f);

            if (progress < 0.6f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.4f) / (maxTimeLeft * 0.6f);

                Projectile.rotation += MathHelper.Lerp(-3f, -1f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;
                
                armRotation += MathHelper.Lerp(-3f, -1f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                scaleVec = Vector2.Lerp(new Vector2(0.9f, 1.1f), new Vector2(0.9f, 1.5f), EaseBuilder.EaseQuinticIn.Ease(lerper));

                scaleVec *= MathHelper.Lerp(0.7f, 1.5f, EaseBuilder.EaseQuinticIn.Ease(lerper));
            }
            else
            {
                if (!flashed)
                {
                    flashed = true;
                    flashTimer = (int)(maxTimeLeft * 0.4f);

                    Owner.Bombus().AddShake(2);

                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);

                    new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShurikenThrow").PlayWith(Projectile.Center);
                }

                float lerper = 1f - Projectile.timeLeft / (maxTimeLeft * 0.4f);

                Projectile.rotation += MathHelper.Lerp(-1f, 2.25f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;
                
                armRotation += MathHelper.Lerp(-1f, 0.25f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                scaleVec = Vector2.Lerp(new Vector2(0.9f, 1.5f), new Vector2(0.9f, 1.1f), EaseBuilder.EaseQuinticOut.Ease(lerper));

                scaleVec *= MathHelper.Lerp(1.5f, 0.7f, EaseBuilder.EaseQuinticOut.Ease(lerper));

                if (pauseTimer <= 0)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                                            Owner.Center.DirectionTo(Projectile.Center) * 3f, 0, new Color(119, 89, 227, 0), MathHelper.Lerp(1f, 0.25f, lerper)).customData = true;
                }        
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation);

            Projectile.Center = Owner.MountedCenter + new Vector2(22f * originalDirection, -25f).RotatedBy(Projectile.rotation);

            tipPosition = Projectile.Center + new Vector2(55f * originalDirection, 25f).RotatedBy(Projectile.rotation);
        }

        private void DownSlash()
        {
            drawAfterImages = true;

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            scaleVec = new Vector2(0.9f, 1.1f);

            float armRotation = Projectile.velocity.ToRotation() + (originalDirection == -1 ? MathHelper.Pi : 0f);

            if (progress < 0.6f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.4f) / (maxTimeLeft * 0.6f);

                Projectile.rotation += MathHelper.Lerp(2.25f, 0f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;
                
                armRotation += MathHelper.Lerp(0.25f, -0.5f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                scaleVec = Vector2.Lerp(new Vector2(0.9f, 1.1f), new Vector2(0.9f, 1.5f), EaseBuilder.EaseQuinticIn.Ease(lerper));

                scaleVec *= MathHelper.Lerp(0.7f, 1.5f, EaseBuilder.EaseQuinticIn.Ease(lerper));
            }
            else
            {
                if (!flashed)
                {
                    flashed = true;
                    flashTimer = (int)(maxTimeLeft * 0.4f);

                    Owner.Bombus().AddShake(2);

                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);

                    new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShurikenThrow").PlayWith(Projectile.Center);
                }

                float lerper = 1f - Projectile.timeLeft / (maxTimeLeft * 0.4f);

                Projectile.rotation += MathHelper.Lerp(0f, -3f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;
                
                armRotation += MathHelper.Lerp(-0.5f, -3f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                scaleVec = Vector2.Lerp(new Vector2(0.9f, 1.5f), new Vector2(0.9f, 1.1f), EaseBuilder.EaseQuinticOut.Ease(lerper));

                scaleVec *= MathHelper.Lerp(1.5f, 0.7f, EaseBuilder.EaseQuinticOut.Ease(lerper));

                if (pauseTimer <= 0)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                                            Owner.Center.DirectionTo(Projectile.Center) * 3f, 0, new Color(119, 89, 227, 0), MathHelper.Lerp(1f, 0.25f, lerper)).customData = true;
                }
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation);

            Projectile.Center = Owner.MountedCenter + new Vector2(22f * originalDirection, -25f).RotatedBy(Projectile.rotation);

            tipPosition = Projectile.Center + new Vector2(55f * originalDirection, 25f).RotatedBy(Projectile.rotation);
        }

        private void ThrowSlash()
        {
            drawAfterImages = true;

            if (Projectile.timeLeft < 250)
            {
                float intertia = MathHelper.Lerp(50f, 8f, 1f - (Projectile.timeLeft - 50f) / 250f);

                float speed = MathHelper.Lerp(5f, 25f, 1f - (Projectile.timeLeft - 50f) / 250f);

                if (Projectile.Distance(Owner.Center) < 250f)
                {
                    intertia = 5f;
                    speed = 25f;
                }

                Projectile.velocity = (Projectile.velocity * intertia + Utils.SafeNormalize(Owner.Center - Projectile.Center, Vector2.UnitX) * speed) / (intertia + 1f);

                if (Projectile.Distance(Owner.Center) < 30f)
                    Projectile.Kill();
            }
            else
            {
                float armRotation = originalVelocity.ToRotation() + (originalDirection == -1 ? MathHelper.Pi : 0f);

                armRotation += MathHelper.Lerp(-4f, 0f, EaseBuilder.EaseQuinticOut.Ease(1f - (Projectile.timeLeft - 250f) / 50f));

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);

                scaleVec = Vector2.Lerp(new Vector2(0.8f, 1.25f), new Vector2(1f, 1f), EaseBuilder.EaseQuinticIn.Ease(1f - (Projectile.timeLeft - 250f) / 50f));

                Projectile.velocity *= 0.97f;
            }

            if (Projectile.timeLeft % 2 == 0)
            {
                Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlowAlt>(),
                    Vector2.One.RotatedBy(Projectile.rotation) * 1.5f, 0, color, 0.4f);
            }

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            Projectile.rotation += 0.15f + Projectile.velocity.Length() * 0.025f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X < Owner.Center.X ? -1 : 1);

            modifiers.SourceDamage *= 1.75f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DarkEnergyGlobalNPC gnpc = target.GetGlobalNPC<DarkEnergyGlobalNPC>();

            if (gnpc.darkEnergy >= DarkEnergyGlobalNPC.MAXENERGY)
                gnpc.Explode(target, Owner);
            else
                gnpc.AddEnergy(4);

            if (Combo == 2)
            {
                oldRot = Projectile.rotation;
                flashTimer = 35;

                if (hits <= 3)
                    pauseTimer = 8;

                hits++;
            }
            else
            {
                if (hits <= 0)
                    pauseTimer = 12;

                hits++;
            }          

            oldTimeLeft = Projectile.timeLeft;

            if (Combo == 2)
            {
                Owner.Bombus().AddShake(3);

                if (BeeUtils.IsFleshy(target))
                {
                    for (int i = 0; i < Main.rand.Next(3, 7); i++)
                    {
                        Vector2 velocity = -Projectile.rotation.ToRotationVector2();

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(3.5f), Main.rand.Next(150, 220), new Color(101, 13, 13), Main.rand.NextFloat(0.5f, 0.8f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(4.5f), Main.rand.Next(150, 220), new Color(215, 29, 29), Main.rand.NextFloat(0.6f, 1f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(5.5f), Main.rand.Next(150, 220), new Color(150, 15, 15), Main.rand.NextFloat(0.7f, 1.2f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(8.5f), 50, default, 2f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(5.5f), 50, default, 2f).fadeIn = 1f;
                    }
                }
                else
                {

                }

                new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center, volume: 0.45f);

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShurikenBounce").PlayWith(Projectile.Center, pitch: 0.7f);
                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShadowDeath").PlayWith(Projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = Vector2.One.RotatedBy(i * MathHelper.TwoPi / 8f) * 1.5f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlowAlt>(),
                        velocity, 0, new Color(152, 137, 255, 0), 0.4f);

                    velocity = -Projectile.velocity.SafeNormalize(Vector2.One);

                    velocity = velocity.RotatedByRandom(1f) * Main.rand.NextFloat(7f);

                    Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlowAlt>(),
                        velocity, 0, color, 0.8f);
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Vector2.One.RotatedBy(1f + (i * MathHelper.TwoPi / 4f)) * 2f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                        velocity, 0, new Color(119, 89, 227, 0), 0.8f).customData = true;
                }
            }
            else
            {
                Owner.Bombus().AddShake(4);

                if (BeeUtils.IsFleshy(target))
                {
                    for (int i = 0; i < Main.rand.Next(5, 10); i++)
                    {
                        Vector2 velocity = target.Center.DirectionTo(Owner.Center);

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.55f) * Main.rand.NextFloat(3.5f), Main.rand.Next(150, 220), new Color(101, 13, 13), Main.rand.NextFloat(0.5f, 0.8f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(4.5f), Main.rand.Next(150, 220), new Color(215, 29, 29), Main.rand.NextFloat(0.6f, 1f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            velocity.RotatedByRandom(0.85f) * Main.rand.NextFloat(5.5f), Main.rand.Next(150, 220), new Color(150, 15, 15), Main.rand.NextFloat(0.7f, 1.2f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(8.5f), 50, default, 2f).fadeIn = 1f;

                        Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(5.5f), 50, default, 2f).fadeIn = 1f;
                    }
                }
                else
                {

                }

                new SoundStyle("BombusApisBee/Sounds/Item/GoreLight").PlayWith(Projectile.Center, volume: 0.45f);
                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/ShadowDeath").PlayWith(Projectile.Center);

                for (int i = 0; i < 9; i++)
                {
                    Vector2 velocity = target.Center.DirectionTo(Owner.Center).RotatedByRandom(0.5f) * Main.rand.NextFloat(5f, 35f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(),
                        velocity, 0, new Color(119, 89, 227, 0), 0.25f);
                }
            }
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;

            if (Combo == 2)
            {
                return null;
            }

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (90f * Projectile.scale), 10, ref collisionPoint))
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Combo != 2)
                DrawPrimitives();

            Texture2D texture = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_Flipped" : "")).Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_FlippedGlow" : "_Glow")).Value;
            Texture2D textureBlur = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_FlippedBlur" : "_Blur")).Value;
            Texture2D textureWhite = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_FlippedWhite" : "_White")).Value;

            Texture2D bladeTexture = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_FlippedBlade" : "_Blade")).Value;
            Texture2D bladeTextureBlur = ModContent.Request<Texture2D>(Texture + (Combo == 1 ? "_FlippedBlade" : "_Blade") + "Blur").Value;
            
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            if (Combo == 2)
            {
                if (drawAfterImages)
                {
                    for (int i = 15; i > 0; i--)
                    {
                        float fade = 0f;

                        if (Projectile.timeLeft > 250f)
                            fade = 1f - (Projectile.timeLeft - 250f) / 50f;
                        else if (Projectile.timeLeft > 200f)
                            fade = (Projectile.timeLeft - 200f) / 50f;

                        fade *= 1 - (15f - i) / 15f;

                        if (i > 0 && i < oldPositions.Count)
                        {
                            Main.spriteBatch.Draw(texture, oldPositions[i] - Main.screenPosition, null, lightColor * fade * 0.25f,
                                                oldRotation[i], texture.Size() / 2f, oldScale[i], 0f, 0f);
                        }
                    }
                }

                ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
                {
                    Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

                    Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                    float opacity = 1f;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                    effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                    effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                    effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                    effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                    effect.Parameters["repeats"].SetValue(2);
                    effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                    effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                    effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

                    Color color = new Color(119, 89, 277, 0) * opacity * 0.5f;

                    effect.Parameters["uColor"].SetValue(color.ToVector4());

                    effect.CurrentTechnique.Passes[0].Apply();

                    Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, 1.5f, 0f, 0f);

                    color = new Color(152, 137, 255, 0) * opacity * 0.5f;

                    effect.Parameters["uColor"].SetValue(color.ToVector4());
                    effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);

                    effect.CurrentTechnique.Passes[0].Apply();

                    Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, 1.5f, 0f, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.EffectMatrix);

                    Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(119, 89, 277, 0) * opacity, 0f, bloomTex.Size() / 2f, 1f / 2f, 0f, 0f);
                });

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(119, 89, 277, 0) * 0.5f,
                    0f, glowTex.Size() / 2f, 2f, 0f, 0f);

                Main.spriteBatch.Draw(textureBlur, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0),
                    Projectile.rotation, textureBlur.Size() / 2f, scaleVec, 0f, 0f);

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor,
                    Projectile.rotation, texture.Size() / 2f, scaleVec, 0f, 0f);

                if (flashTimer > 0)
                {
                    float flash = flashTimer / 35f;

                    Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, null, new Color(119, 89, 277, 0) * flash,
                    Projectile.rotation, textureGlow.Size() / 2f, scaleVec, 0f, 0f);

                    Main.spriteBatch.Draw(textureWhite, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255) * flash,
                    Projectile.rotation, textureWhite.Size() / 2f, scaleVec, 0f, 0f);

                    Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition,
                    null, new Color(119, 89, 227, 0) * flash * 0.25f, 0f, glowTex.Size() / 2f, 1.5f, 0f, 0f);

                    Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition,
                        null, new Color(119, 89, 227, 0) * flash , 0f, starTex.Size() / 2f, 1.5f, 0f, 0f);

                    Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition,
                        null, new Color(255, 255, 255, 0) * flash, 0f, starTex.Size() / 2f, 0.5f, 0f, 0f);
                }

                return false;
            }

            SpriteEffects flip = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            float fadeOut = 1f;

            if (Projectile.timeLeft > maxTimeLeft - 20)
                fadeOut = 1f - (Projectile.timeLeft - (maxTimeLeft - 20)) / 20f;

            if (Projectile.timeLeft < 20)
                fadeOut = Projectile.timeLeft / 20f;

            if (drawAfterImages)
            {
                for (int i = 15; i > 0; i--)
                {
                    float fade = 0f;

                    float progress = 1f - Projectile.timeLeft / maxTimeLeft;

                    if (progress < 0.5f)
                    {
                        float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.5f) / (maxTimeLeft * 0.5f);

                        fade = lerper;
                    }
                    else
                    {
                        float lerper = Projectile.timeLeft / (maxTimeLeft * 0.5f);

                        fade = lerper;
                    }

                    fade *= 1 - (15f - i) / 15f;

                    if (i > 0 && i < oldRotation.Count)
                    {
                        Main.spriteBatch.Draw(glowTex, Owner.Center + oldPositions[i] * 2f - Main.screenPosition,
                            null, new Color(119, 89, 227, 0) * 0.15f * fade * fadeOut, 0f, glowTex.Size() / 2f, 1.5f, 0f, 0f);

                        Main.spriteBatch.Draw(bladeTexture, Owner.Center + oldPositions[i] - Main.screenPosition,
                            null, lightColor * 0.25f * fade * fadeOut, oldRotation[i], bladeTexture.Size() / 2f, oldScale[i], flip, 0f);

                        Main.spriteBatch.Draw(bladeTextureBlur, Owner.Center + oldPositions[i] - Main.screenPosition,
                            null, new Color(255, 255, 255, 0) * 0.5f * fade * fadeOut, oldRotation[i], bladeTextureBlur.Size() / 2f, oldScale[i], flip, 0f);
                    }
                }
            }

            float addRot = 0f;

            if (Combo == 1)
                addRot = MathHelper.PiOver2 * originalDirection;

            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
            {
                Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                float opacity = 1f;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                effect.Parameters["repeats"].SetValue(2);
                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

                Color color = new Color(119, 89, 277, 0) * opacity * 0.35f * fadeOut;

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, 1f, 0f, 0f);

                color = new Color(152, 137, 255, 0) * opacity * 0.35f * fadeOut;

                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, 1f, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.EffectMatrix);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition,
                    null, new Color(119, 89, 277, 0) * opacity * fadeOut, 0f, bloomTex.Size() / 2f, 0.5f, 0f, 0f);
            });

            Main.spriteBatch.Draw(textureBlur, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, new Color(255, 255, 255, 0) * fadeOut,
               Projectile.rotation + (Owner.direction == -1 ? MathHelper.TwoPi : 0f) + addRot, textureBlur.Size() / 2f, scaleVec, flip, 0f);

            Main.spriteBatch.Draw(texture, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, lightColor * fadeOut, 
                Projectile.rotation + (Owner.direction == -1 ? MathHelper.TwoPi : 0f) + addRot, texture.Size() / 2f, scaleVec, flip, 0f);

            if (flashTimer > 0)
            {
                float flash = flashTimer / (float)(maxTimeLeft * 0.4f);

                Main.spriteBatch.Draw(textureGlow, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, new Color(119, 89, 227, 0) * flash * fadeOut,
                    Projectile.rotation + (Owner.direction == -1 ? MathHelper.TwoPi : 0f) + addRot, textureGlow.Size() / 2f, scaleVec, flip, 0f);

                Main.spriteBatch.Draw(textureWhite, Projectile.Center + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition, null, new Color(255, 255, 255) * flash * fadeOut,
                    Projectile.rotation + (Owner.direction == -1 ? MathHelper.TwoPi : 0f) + addRot, textureWhite.Size() / 2f, scaleVec, flip, 0f);
                
                //Main.spriteBatch.Draw(glowTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition,
                    //null, new Color(119, 89, 227, 0) * flash * fadeOut * 0.25f, 0f, glowTex.Size() / 2f, 1.5f, flip, 0f);

                Main.spriteBatch.Draw(starTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition,
                    null, new Color(119, 89, 227, 0) * flash * fadeOut, 0f, starTex.Size() / 2f, 1.5f, flip, 0f);

                Main.spriteBatch.Draw(starTex, Projectile.Center + new Vector2(0f, Owner.gfxOffY) + new Vector2(5f * originalDirection, -35f).RotatedBy(Projectile.rotation) - Main.screenPosition,
                    null, new Color(255, 255, 255, 0) * flash * fadeOut, 0f, starTex.Size() / 2f, 0.5f, flip, 0f);
            }

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            var adjustedPos = tipPosition - Owner.Center;

            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(adjustedPos);
                }
            }

            cache.Add(adjustedPos);

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(0), factor => 30f * factor, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(91, 71, 127) * 0.8f) * factor.X * TrailFade();
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(0), factor => 25f * factor, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(119, 89, 227) * 0.6f) * factor.X * TrailFade();
            });

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(0), factor => 40f * factor, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(210, 228, 255) * 0.4f) * factor.X * TrailFade();
            });

            var realCache = new Vector2[20];

            for (int k = 0; k < 20; k++)
            {
                realCache[k] = Owner.Center + cache[k];
            }

            trail.Positions = realCache;
            trail2.Positions = realCache;
            trail3.Positions = realCache;

            trail.NextPosition = Owner.Center + (tipPosition - Owner.Center) + Projectile.velocity;
            trail2.NextPosition = Owner.Center + (tipPosition - Owner.Center) + Projectile.velocity;
            trail3.NextPosition = Owner.Center + (tipPosition - Owner.Center) + Projectile.velocity;
        }

        public float TrailFade()
        {
            float ratio = Projectile.timeLeft / maxTimeLeft;


            if (ratio > 0.5f)
                return 0f;
            else if (ratio > 0.2f)
                return (1f - (Projectile.timeLeft) / (maxTimeLeft * 0.5f));
            else
                return ratio;
        }

        public void DrawPrimitives()
        {
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());

                // !!! IMPORTANT WHEN PIXELIZING, MAKE SURE TO USE Main.GameViewMatrix.EffectMatrix IMPORTANT !!!

                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
                trail3?.Render(effect);
            });
        }

        #endregion Primitive Drawing
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadestingerScytheAltProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public override string Texture => BombusApisBee.Invisible;

        public int maxTimeLeft;
        public int originalDirection;

        public float Combo => Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger Scythe");
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(24);
            Projectile.penetrate = -1;
            Projectile.Bombus().HeldProj = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = 30;
            maxTimeLeft = Projectile.timeLeft;
            Projectile.rotation = Owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
            Projectile.netUpdate = true;
            Projectile.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            originalDirection = Projectile.direction;
            Owner.itemTime = Projectile.timeLeft / 2;
            Owner.itemAnimation = Owner.itemTime;
        }

        public override void AI()
        {
            if (Owner.HeldItem.ModItem is not ShadestingerScythe)
                Projectile.Kill();

            Owner.heldProj = Projectile.whoAmI;

            Owner.ChangeDir(Projectile.direction);

            Projectile.Center = Owner.Center;

            float armRotation = Projectile.velocity.ToRotation() + (originalDirection == -1 ? MathHelper.Pi : 0f);

            armRotation += MathHelper.Lerp(0f, -3f, EaseBuilder.EaseQuarticIn.Ease(1f - Projectile.timeLeft / (float)maxTimeLeft)) * originalDirection;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);

            if (Projectile.timeLeft < 15)
            {
                float rot = Projectile.velocity.ToRotation();

                rot += MathHelper.Lerp(0f, -3f, EaseBuilder.EaseQuarticIn.Ease(1f - Projectile.timeLeft / (float)maxTimeLeft)) * originalDirection;

                Color color = new Color(10, 255, 50, 0);

                Dust.NewDustPerfect(Owner.Center, DustType<StarDustWhite>(),
                                   rot.ToRotationVector2() * 3f, 0, color, 0.3f).customData = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active)
                {
                    DarkEnergyGlobalNPC gnpc = npc.GetGlobalNPC<DarkEnergyGlobalNPC>();

                    if (gnpc.darkEnergy > 0)
                    {
                        gnpc.Heal(Owner);

                        for (int j = 0; j < 5; j++)
                        {
                            Vector2 velocity = npc.DirectionTo(Owner.Center).RotatedByRandom(0.9f) * Main.rand.NextFloat(8f);

                            Color color = new Color(100, 255, 50, 0);

                            Dust.NewDustPerfect(npc.Center, DustType<PixelatedGlowAlt>(),
                                velocity, 0, color, 1f);

                            if (Main.rand.NextBool(2))
                            {
                                color = new Color(10, 255, 50, 0);

                                Dust.NewDustPerfect(npc.Center, DustType<StarDustWhite>(),
                                                   velocity, 0, color, 1.2f).customData = true;
                            }
                        }
                    }                       
                }             
            }
        }
    }
}
