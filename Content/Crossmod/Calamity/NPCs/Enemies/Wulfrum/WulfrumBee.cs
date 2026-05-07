using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Sounds;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace BombusApisBee.Content.Crossmod.Calamity.NPCs.Enemies.Wulfrum
{
    [JITWhenModsEnabled("CalamityMod")]
    public class WulfrumBee : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public bool Supercharged => NPC.ai[0] == 1f;
        public Color SuperchargeColor => Supercharged ? new Color(55, 180, 220) : new Color(130, 200, 70);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Bee");
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 8;
            NPC.damage = 5;
            NPC.defense = 1;
            NPC.lifeMax = 10;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 0.5f;
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
                new FlavorTextBestiaryInfoElement("Machinations which appear to be mere copies of the Jungle's fierce yet small foe, these imitation bees can still pack a punch when powered up by their Wulfrum Hive.")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            NPC.rotation = NPC.velocity.X * 0.1f;

            NPC.spriteDirection = -Math.Sign(NPC.velocity.X);

            Player player = Main.player[NPC.target];

            Vector2 targetPos = NPC.Center + NPC.Size / 2f + NPC.velocity * 100f;

            if (player != null && !player.dead)
                targetPos = player.Center;

            float speed = Supercharged ? 7.5f : 5.5f;
            float adjust = Supercharged ? 0.3f : 0.2f;

            Vector2 vector = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float betweenX = targetPos.X - vector.X;
            float betweenY = targetPos.Y - vector.Y;
            float dist = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
            dist = speed / dist;
            betweenX *= dist;
            betweenY *= dist;

            if (NPC.velocity.X < betweenX)
            {
                NPC.velocity.X += adjust;
                if (NPC.velocity.X < 0f && betweenX > 0f)
                {
                    NPC.velocity.X += adjust * 2f;
                }
            }
            else if (NPC.velocity.X > betweenX)
            {
                NPC.velocity.X -= adjust;
                if (NPC.velocity.X > 0f && betweenX < 0f)
                {
                    NPC.velocity.X -= adjust * 2f;
                }
            }
            if (NPC.velocity.Y < betweenY)
            {
                NPC.velocity.Y += adjust;
                if (NPC.velocity.Y < 0f && betweenY > 0f)
                {
                    NPC.velocity.Y += adjust * 2f;
                }
            }
            else if (NPC.velocity.Y > betweenY)
            {
                NPC.velocity.Y -= adjust;
                if (NPC.velocity.Y > 0f && betweenY < 0f)
                {
                    NPC.velocity.Y -= adjust * 2f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 12;
                if (NPC.frame.Y >= 12 * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f);
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(NPC.Center, DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, SuperchargeColor, 0.35f);

                Dust.NewDustPerfect(NPC.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(2.5f, 2.5f), 0, SuperchargeColor with { A = 0 }, 0.05f);
            }

            if (NPC.life <= 0)
                KillEffect();
        }

        private void KillEffect()
        {
            Dust.NewDustPerfect(NPC.Center, DustType<WulfrumSmokeDust>(), Main.rand.NextVector2Circular(.5f, .5f), 150, SuperchargeColor with { A = 0 }, Main.rand.NextFloat(0.1f, 0.15f));
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<WulfrumMetalScrap>(), 15, 1, 1));
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (Supercharged)
            {
                modifiers.FinalDamage *= 0.5f;
                modifiers.Knockback *= 0.5f;
            }
        }

        #region drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Request<Texture2D>(Texture + (Supercharged ? "_Supercharged" : "")).Value;
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            spriteBatch.Draw(glowTex, NPC.Center - screenPos, null, SuperchargeColor with { A = 0 }, 0f, glowTex.Size() / 2f, NPC.scale * 0.25f, 0f, 0f);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            return false;
        }
        #endregion drawing;
    }
}
