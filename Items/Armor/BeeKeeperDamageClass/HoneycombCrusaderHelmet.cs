using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Items.Other.OnPickupItems;
using Terraria;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneycombCrusaderHelmet : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("5% increased hymenoptra critical strike chance\nMaximum honey increased by 75");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneycombCrusaderGreaves>() && body.type == ModContent.ItemType<HoneycombCrusaderPlatemail>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Conjures a honey shield that attempts to protect the bearer\nThe shield breaks into honeycomb chunks upon being struck";
            player.GetModPlayer<HoneycombCrusaderPlayer>().FullArmorSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(5);
            player.Hymenoptra().BeeResourceMax2 += 75;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 18).
                AddIngredient(ModContent.ItemType<Pollen>(), 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    class HoneycombCrusaderPlayer : ModPlayer
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
            if (FullArmorSet && Player.ownedProjectileCounts<HoneycombCrusaderShieldProjectile>() <= 0)
                Projectile.NewProjectile(Player.GetSource_Misc("BombusApisBee: Spawn Holy Shield"), Player.Center, Vector2.Zero, ModContent.ProjectileType<HoneycombCrusaderShieldProjectile>(), Player.ApplyHymenoptraDamageTo(100), 0f, Player.whoAmI);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            hitProjHitTimer[proj.whoAmI] = 30;
        }
    }

    class HoneycombCrusaderShieldProjectile : ModProjectile
    {
        int deathTimer;
        int hitTimer;
        int numHits;
        Player owner => Main.player[Projectile.owner];
        Vector2 collisionPoint(Player player) { return player.Center + -(Projectile.rotation + 0.15f).ToRotationVector2() * 65f; }

        public override bool? CanDamage() => deathTimer <= 0 && hitTimer <= 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Shield");
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
                    BeeUtils.CircleDust(collisionPoint(owner), 30, ModContent.DustType<Dusts.GlowFastDecelerate>(), 2f, 0, new Color(150, 100, 30), 0.5f);
                }
                deathTimer--;
            }

            if (hitTimer > 0)
                hitTimer--;

            Projectile.spriteDirection = -1;

            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<HoneycombCrusaderPlayer>().FullArmorSet && !player.dead)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.AngleTo(player.Center);
            Projectile.ai[0]++;
            double deg = Projectile.ai[0] * 1.25f;
            double rad = deg * (Math.PI / 180);
            double dist = 64;
            Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

            if (Main.rand.NextBool(5) && deathTimer <= 0)
                Dust.NewDustPerfect(collisionPoint(owner) + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(155, 105, 20) * (1f - (deathTimer / 600f)), Main.rand.NextFloat(0.3f, 0.6f));

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (deathTimer <= 0 && hitTimer <= 0 && Vector2.Distance(collisionPoint(owner), proj.Center) < 50f && proj.active && proj.hostile && proj.damage > 0 && owner.GetModPlayer<HoneycombCrusaderPlayer>().hitProjHitTimer[proj.whoAmI] <= 0)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        Dust.NewDustPerfect(proj.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(165, 105, 0), 0.35f);

                        Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(proj.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(proj.Center) * 5f).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.25f, 1.75f), 0, new Color(150, 50, 20), 0.55f);

                        Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(proj.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(proj.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(200, 150, 20), 0.75f);

                        Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(proj.Center) * 65f, DustID.Honey2, (owner.Center.DirectionTo(proj.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, default, 1.65f).noGravity = true;
                    }

                    DropChunks();

                    SoundEngine.PlaySound(BombusApisBee.HoneycombWeapon, proj.position);

                    proj.Kill();
                    numHits++;

                    hitTimer = 60;
                }
            }

            if (numHits >= 3)
            {
                numHits = 0;
                deathTimer = 600;

                SoundEngine.PlaySound(BombusApisBee.HoneycombWeapon with { Pitch = -0.75f, PitchVariance = 0.15f }, Projectile.position);

                SoundEngine.PlaySound(SoundID.SplashWeak with { Pitch = -0.25f, PitchVariance = 0.15f, Volume = 2f }, Projectile.position);
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

            Main.spriteBatch.Draw(bloomTex, collisionPoint(owner) + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, new Color(200, 50, 20, 0) * 0.35f * mult, 0f, bloomTex.Size() / 2f, 1.25f, 0f, 0f);

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
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);
            Color color = new Color(200, 50, 20, 0) * mult;
            if (hitTimer > 0)
                color = Color.Lerp(new Color(255, 255, 0, 0) * mult, color, 1f - hitTimer / 60f);

            effect.Parameters["uColor"].SetValue(color.ToVector4());

            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, owner.gfxOffY), null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale * 1.25f, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            color = new Color(160, 100, 20, 0) * mult;
            if (hitTimer > 0)
                color = Color.Lerp(new Color(255, 255, 0, 0) * mult, color, 1f - hitTimer / 60f);

            effect.Parameters["uColor"].SetValue(color.ToVector4());
           
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, owner.gfxOffY), null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale * 1.15f, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
           
            color = new Color(230, 165, 20, 0) * mult;
            if (hitTimer > 0)
                color = Color.Lerp(new Color(255, 255, 0, 0) * mult, color, 1f - hitTimer / 60f);
            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, owner.gfxOffY), null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(bloomTex, collisionPoint(owner) + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, new Color(230, 165, 20, 0) * 0.35f * mult, 0f, bloomTex.Size() / 2f, 1.35f, 0f, 0f);

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist > 0)
                target.velocity += Main.player[Projectile.owner].DirectionTo(target.Center) * 10f;

            numHits++;
            hitTimer = 60;

            SoundEngine.PlaySound(BombusApisBee.HoneycombWeapon, target.position);

            for (int x = 0; x < 10; x++)
            {
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(165, 105, 0), 0.35f);

                Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(target.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(target.Center) * 5f).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.25f, 1.75f), 0, new Color(150, 50, 20), 0.55f);

                Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(target.Center) * 65f, ModContent.DustType<Dusts.GlowFastDecelerate>(), (owner.Center.DirectionTo(target.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, new Color(200, 150, 20), 0.75f);

                Dust.NewDustPerfect(owner.Center + owner.Center.DirectionTo(target.Center) * 65f, DustID.Honey2, (owner.Center.DirectionTo(target.Center) * 5f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1f), 0, default, 1.65f).noGravity = true;
            }
            
            if (numHits >= 3)
                owner.Bombus().AddShake(6);
            else
                owner.Bombus().AddShake(2);

            DropChunks();
        }

        internal void DropChunks()
        {
            for (int i = 0; i < Main.rand.Next(3, 7); i++)
            {
                Item item = Main.item[Item.NewItem(Projectile.GetSource_FromAI(), Projectile.getRect(), ModContent.ItemType<HoneycombChunkPickup>())];

                item.noGrabDelay = 60;

                (item.ModItem as HoneycombChunkPickup).TextureString = "BombusApisBee/Items/Other/OnPickupItems/HoneycombChunkPickup_" + Main.rand.Next(1, 5);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI, 1f);
                }
            }
        }
    }
}

