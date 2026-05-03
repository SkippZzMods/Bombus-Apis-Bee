using BombusApisBee.Content.Crossmod;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Wulfrum;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Sounds;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;

namespace BombusApisBee.Content.Crossmod.Calamity.NPCs.Enemies.Wulfrum
{
    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumHive : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public const int MAX_CHARGE = 120;

        public int SuperchargeTimer;

        public Vector2 cannonRotationVector;

        public bool Supercharged => SuperchargeTimer > 0;
        public float CannonRotation => cannonRotationVector.ToRotation();
        public Vector2 CannonPosition => NPC.Center + CannonRotation.ToRotationVector2() * 12f;
        public Vector2 ThrusterPosition => NPC.oldPosition + NPC.Size / 2f + new Vector2(0f, 16f).RotatedBy(NPC.rotation);

        public Color SuperchargeColor => Supercharged ? new Color(55, 180, 220) : new Color(130, 200, 70);

        public bool CanFire
        {
            get => NPC.ai[0] == 1f;
            set
            {
                if (value)
                    NPC.ai[0] = 1f;
                else
                    NPC.ai[0] = 0f;
            }
        }

        public ref float CurrentCharge => ref NPC.ai[1];
        public ref float CooldownTimer => ref NPC.ai[2];
        public ref float AttackTimer => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Hive");
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 24;
            NPC.damage = 5;
            NPC.defense = 5;
            NPC.lifeMax = 30;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 1f;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound with { Volume = 0.5f };
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = false;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Hovering around with their trusty thruster, they charge up an attack so powerful that immobilizes the unit temporarily.")
            });
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false; // we dont do contact damage up in these parts

        public override void AI()
        {
            if (SuperchargeTimer > 0)
                SuperchargeTimer--;

            CheckSupercharge();

            NPC.TargetClosest(false);

            Player target = Main.player[NPC.target];

            if (target != null && !target.dead)
            {
                TargetFoundAI(target);
            }
            else
            {
                NPC.velocity.Y -= 1f;
            }

            DrawDusts();
        }

        private void CheckSupercharge()
        {
            NPC pylon = Main.npc.Where(n => n.active && n.type == NPCType<WulfrumAmplifier>()).OrderBy(n => n.Distance(NPC.Center)).FirstOrDefault(); // find closest pylon
            if (pylon != default && NPC.Distance(pylon.Center) < (pylon.ModNPC as WulfrumAmplifier).ChargeRadius && SuperchargeTimer <= 0)
            {
                SuperchargeTimer = 720;
                NPC.netUpdate = true;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, DustID.Electric, Main.rand.NextVector2Circular(5f, 5f));

                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(7.5f, 7.5f), 0, new Color(55, 180, 220), 0.5f);
                }
            }
        }

        private void TargetFoundAI(Player target)
        {
            if (CooldownTimer > 0)
            {
                CoolingDownAI();
                return;
            }

            if (!NPC.noGravity)
                NPC.noGravity = true;

            if (target != null && !target.dead)
            {
                cannonRotationVector = Vector2.Lerp(cannonRotationVector, NPC.DirectionTo(target.Center), 0.05f);

                float distance = NPC.Distance(target.Center);
                if (distance < 500f) // its got to get decently close to start charging its attack
                {
                    if (CurrentCharge < MAX_CHARGE)
                    {
                        CurrentCharge += Supercharged ? 3 : 1;
                    }
                    else if (AttackTimer <= 0 && !CanFire)
                    {
                        if (CurrentCharge > MAX_CHARGE)
                            CurrentCharge = MAX_CHARGE;

                        CanFire = true;
                        AttackTimer = 20;
                    }
                }

                Vector2 targetPosition = target.Center + target.DirectionTo(NPC.Center) * 150f;
                float posDist = NPC.Distance(targetPosition);
                float speed;

                if (posDist > 150f)
                    speed = Supercharged ? 15f : 10f;
                else
                {
                    float lerper = 1f - posDist / 150f;
                    speed = MathHelper.Lerp(Supercharged ? 15f : 10f, 0f, lerper);
                }

                Vector2 toTargetPos = targetPosition - NPC.Center;

                if (toTargetPos.Length() < 0.0001f)
                {
                    toTargetPos = Vector2.Zero;
                }
                else
                {
                    toTargetPos.Normalize();
                    toTargetPos *= speed;
                }

                NPC.velocity = (NPC.velocity * (10f - 1) + toTargetPos) / 10f;

                Vector2 startPos = NPC.Bottom;
                for (int x = -1; x < 2; x++) // search 1 tiles each direction
                {
                    for (int y = 0; y < 5; y++) // search 5 tiles down in order to hover
                    {
                        Vector2 worldPos = startPos + new Vector2(x * 16f, y * 16f);
                        var tilePos = new Point16((int)worldPos.X / 16, (int)worldPos.Y / 16);
                        Tile tile = Framing.GetTileSafely(tilePos);
                        Tile aboveTile = Framing.GetTileSafely(new Point16(tilePos.X, tilePos.Y - 1));
                        if (tile.HasTile && !WorldGen.SolidOrSlopedTile(aboveTile) && WorldGen.SolidOrSlopedTile(tile))
                        {
                            NPC.velocity.Y -= 0.05f;

                            break;
                        }
                    }
                }

                if (AttackTimer > 0)
                {
                    AttackTimer--;

                    float lerper = EaseFunction.EaseCubicOut.Ease(AttackTimer / 20f);

                    float length = 30f * lerper;

                    Vector2 offset = Vector2.One.RotatedBy(6.28f * lerper).RotatedBy(NPC.rotation) * length;

                    Dust.NewDustPerfect(NPC.Center + offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, SuperchargeColor, 0.35f);

                    Dust.NewDustPerfect(NPC.Center - offset,
                        DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, SuperchargeColor, 0.35f);

                    NPC.velocity *= 0.95f;
                }
                else if (CanFire)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, NPCType<WulfrumBee>());
                            npc.velocity = cannonRotationVector.RotatedBy(i * 45f) * 5f;
                            npc.ai[0] = Supercharged ? 1f : 0f;
                        }
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(CannonPosition, DustType<GlowFastDecelerate>(), cannonRotationVector.RotatedByRandom(0.45f) * Main.rand.NextFloat(1f, 4f), 0, SuperchargeColor, 0.5f);

                        Dust.NewDustPerfect(NPC.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, SuperchargeColor, 0.5f);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(CannonPosition, DustType<ImpactLineDust>(), cannonRotationVector.RotatedByRandom(0.65f) * Main.rand.NextFloat(5f, 15f), 0, SuperchargeColor with { A = 0 }, 0.05f);

                        Dust.NewDustPerfect(NPC.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2Circular(15f, 15f), 0, SuperchargeColor with { A = 0 }, 0.05f);
                    }

                    new SoundStyle("CalamityMod/Sounds/Item/WulfrumProsthesisShoot").PlayWith(NPC.Center, 0, 0.2f, 1f);

                    CurrentCharge = 0;
                    CooldownTimer = 360;
                    AttackTimer = 0;
                    NPC.velocity -= cannonRotationVector * 5f;
                    CanFire = false;
                }
            }

            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.2f, 0.1f);
        }

        private void CoolingDownAI()
        {
            if (NPC.noGravity)
                NPC.noGravity = false;

            NPC.GravityMultiplier *= 0.5f;

            NPC.rotation += NPC.velocity.Length() * 0.01f;

            if (Main.rand.NextBool(20))
                Dust.NewDustPerfect(NPC.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(1f, 1f) - Vector2.UnitY * 2f, 150, SuperchargeColor with { A = 0 }, Main.rand.NextFloat(0.1f, 0.12f)).noGravity = true;

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(NPC.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(1f, 1f), 150, Color.White with { A = 0 }, Main.rand.NextFloat(0.1f, 0.12f));

            if (Main.rand.NextBool(20))
                Dust.NewDustPerfect(NPC.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(10f, 10f), 0, SuperchargeColor with { A = 0 }, 0.075f);

            CooldownTimer -= Supercharged ? 2 : 1;
        }

        private void DrawDusts()
        {
            if (NPC.velocity.Length() > 1f && CooldownTimer <= 0)
                Dust.NewDustPerfect(ThrusterPosition, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f) - NPC.velocity, 0, SuperchargeColor, 0.25f);
        }

        #region netsync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(cannonRotationVector);
            writer.Write7BitEncodedInt(SuperchargeTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            cannonRotationVector = reader.ReadVector2();
            SuperchargeTimer = reader.Read7BitEncodedInt();
        }
        #endregion netsync

        public override bool? CanFallThroughPlatforms()
        {
            return CooldownTimer <= 0;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(NPCType<WulfrumAmplifier>()) ? 3f : 1f;
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
                return 0f;

            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.01f : 0.075f) * pylonMult;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(NPC.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(.5f, .5f) - Vector2.UnitY * 2f, 150, SuperchargeColor with { A = 0 }, Main.rand.NextFloat(0.05f, 0.07f));
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, SuperchargeColor, 0.5f);

                Dust.NewDustPerfect(NPC.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(5f, 5f), 0, SuperchargeColor with { A = 0 }, 0.075f);
            }

            if (NPC.life <= 0)
                KillEffect();
        }

        private void KillEffect()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("WulfrumHiveGore").Type, 1f);

            Gore.NewGore(NPC.GetSource_Death(), ThrusterPosition, NPC.velocity, Mod.Find<ModGore>("WulfrumHiveThrusterGore" + (Supercharged ? "_Empowered" : "")).Type, 1f);

            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("WulfrumHiveEyeGore" + (Supercharged ? "_Empowered" : "")).Type, 1f);

            Mod calamity = GetInstance<CalamityMod.CalamityMod>();

            for (int k = 0; k < Main.rand.Next(2, 5); k++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(10f, 10f), NPC.velocity, calamity.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString()).Type, 1f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemType<WulfrumMetalScrap>(), 1, 1, 3);
            npcLoot.Add(ItemType<WulfrumHoneycomb>(), 10);
            npcLoot.Add(ItemType<WulfrumBattery>(), new Fraction(7, 100), 1, 1);
            npcLoot.AddIf((info) => info.npc.ModNPC<WulfrumHive>().Supercharged, ItemType<EnergyCore>());
        }

        #region drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texBlur = Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texEye = Request<Texture2D>(Texture + "_Eye").Value;
            Texture2D texEyeBlur = Request<Texture2D>(Texture + "_Eye_Blur").Value;
            Texture2D texCannon = Request<Texture2D>(Texture + "_Cannon").Value;
            Texture2D texCannonGlow = Request<Texture2D>(Texture + "_Cannon_Glow").Value;
            Texture2D texThruster = Request<Texture2D>(Texture + "_Thruster").Value;
            Texture2D texThrusterBlur = Request<Texture2D>(Texture + "_Thruster_Blur").Value;
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            if (NPC.IsABestiaryIconDummy)
            {
                Rectangle bestiaryFrame = texThruster.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(texThruster, NPC.Center + new Vector2(0, 18f) - screenPos, bestiaryFrame, drawColor, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                bestiaryFrame = texThrusterBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(texThrusterBlur, NPC.Center + new Vector2(0, 18f) - screenPos, bestiaryFrame, Color.White with { A = 0 }, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                spriteBatch.Draw(texCannon, NPC.Center + new Vector2(12f, 0) - screenPos, null, drawColor, 0f, texCannon.Size() / 2f, NPC.scale, 0f, 0f);

                bestiaryFrame = tex.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(tex, NPC.Center - screenPos, bestiaryFrame, drawColor, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                bestiaryFrame = texBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(texBlur, NPC.Center - screenPos, bestiaryFrame, Color.White with { A = 0 }, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                bestiaryFrame = texEye.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(texEye, NPC.Center - screenPos, bestiaryFrame, Color.White, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                bestiaryFrame = texEyeBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: 0);
                spriteBatch.Draw(texEyeBlur, NPC.Center - screenPos, bestiaryFrame, Color.White with { A = 0 }, NPC.rotation, bestiaryFrame.Size() / 2f, NPC.scale, 0f, 0f);

                return false;
            }

            int frame = Supercharged ? 1 : 0;

            Rectangle texFrame = texThruster.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(texThruster, ThrusterPosition - screenPos, texFrame, drawColor, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            texFrame = texThrusterBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(texThrusterBlur, ThrusterPosition - screenPos, texFrame, Color.White with { A = 0 }, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            Effect effect = Terraria.Graphics.Effects.Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/WulfrumThrusterNoise").Value);
            effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/WulfrumThrusterNoise").Value);

            Color color = SuperchargeColor with { A = 0 };

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/WulfrumThrusterNoise").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(glowTex, ThrusterPosition - screenPos, null, Color.White, NPC.rotation, glowTex.Size() / 2f, 0.25f, 0f, 0f);

            Main.spriteBatch.Draw(glowTex, ThrusterPosition - screenPos, null, Color.White, NPC.rotation, glowTex.Size() / 2f, 0.35f, 0f, 0f);

            color = Color.White with { A = 0 } * 0.5f;
            effect.Parameters["uColor"].SetValue(color.ToVector4());

            Main.spriteBatch.Draw(glowTex, ThrusterPosition - screenPos, null, Color.White, NPC.rotation, glowTex.Size() / 2f, 0.35f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texCannon, CannonPosition - screenPos, null, drawColor, CannonRotation, texCannon.Size() / 2f, NPC.scale, 0f, 0f);

            if (AttackTimer > 0)
            {
                float lerper = EaseFunction.EaseCubicOut.Ease(1f - AttackTimer / 20f);
                spriteBatch.Draw(texCannonGlow, CannonPosition - screenPos, null, Color.Lerp(Color.Transparent, SuperchargeColor with { A = 0 }, lerper), CannonRotation, texCannonGlow.Size() / 2f, NPC.scale, 0f, 0f);
            }

            if (CurrentCharge > 0)
            {
                float lerper = CurrentCharge / MAX_CHARGE;

                spriteBatch.Draw(texGlow, NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 0.5f - screenPos, null, Color.Lerp(Color.Transparent, SuperchargeColor with { A = 0 }, lerper), NPC.rotation, texGlow.Size() / 2f, NPC.scale, 0f, 0f);

                spriteBatch.Draw(glowTex, NPC.Center - screenPos, null, Color.Lerp(Color.Transparent, SuperchargeColor with { A = 0 }, lerper), NPC.rotation, glowTex.Size() / 2f, NPC.scale * 0.5f, 0f, 0f);
            }

            texFrame = tex.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(tex, NPC.Center - screenPos, texFrame, drawColor, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            texFrame = texBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(texBlur, NPC.Center - screenPos, texFrame, Color.White with { A = 0 }, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            texFrame = texEye.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(texEye, NPC.Center + CannonRotation.ToRotationVector2() * 1f + NPC.velocity * 0.5f - screenPos, texFrame, Color.White, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            texFrame = texEyeBlur.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: frame);
            spriteBatch.Draw(texEyeBlur, NPC.Center + CannonRotation.ToRotationVector2() * 1f + NPC.velocity * 0.5f - screenPos, texFrame, Color.White with { A = 0 }, NPC.rotation, texFrame.Size() / 2f, NPC.scale, 0f, 0f);

            return false;
        }
        #endregion drawing;
    }

    public class WulfrumSmokeDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible; // lol

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
            dust.customData = 1 + Main.rand.Next(3);
            dust.rotation = Main.rand.NextFloat(6.28f);
            dust.fadeIn = 1f;
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y -= 0.1f;
            if (dust.noGravity)
                dust.position.Y -= 0.5f;

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity *= 0.99f;
            else
            {
                dust.velocity *= 0.975f;
                dust.velocity.X *= 0.99f;
            }

            dust.rotation += dust.velocity.Length() * 0.01f;

            if (dust.noGravity)
                dust.alpha += 2;
            else
                dust.alpha += 5;

            dust.alpha = (int)(dust.alpha * 1.005f);

            if (!dust.noGravity)
                dust.scale *= 1.02f;
            else
                dust.scale *= 0.99f;

            if (dust.fadeIn > 0f)
                dust.fadeIn -= 0.05f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            float fadeIn = 1f - dust.fadeIn;

            Texture2D tex = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/SmokeAlpha_" + dust.customData).Value;

            Color color = dust.color;
            if (lerper < 0.25f)
                color = Color.Lerp(dust.color, new Color(200, 200, 200, 0), 1f - lerper / 0.25f);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * lerper * fadeIn, dust.rotation, tex.Size() / 2f, dust.scale, 0f, 0f);

            return false;
        }
    }
}
