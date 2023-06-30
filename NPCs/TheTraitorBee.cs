using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.Localization;

namespace BombusApisbee.NPCs
{
    [AutoloadHead]
    public class TheTraitorBee : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Traitor Bee");
            Main.npcFrameCount[NPC.type] = 23;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;

            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 150;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = 1
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Hate);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.width = 18;
            NPC.height = 48;
            NPC.aiStyle = 7;
            NPC.damage = 8;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = 208;

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Beenade, 1, 5, 10));
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */
        {
            return NPC.downedQueenBee;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return false;
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 8;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30; // The amount of ticks the Town NPC takes to cool down. Every 60 in-game ticks is a second.
            randExtraCooldown = 30; // How long it takes until the NPC attacks again, but with a chance.
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = 183; // The Projectile this NPC shoots. Search up Terraria Projectile IDs, I cannot link the websites in this code
            attackDelay = 1; // Delays the attacks, obviously.
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f; // The Speed of the Projectile
            randomOffset = 2f; // Random Offset
        }
        public override string GetChat()
        {
            // these lines of code are an example of how NPCs can refer to others, most NPCs often use this to hint a relationship between them and another.
            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0 && Main.rand.NextBool(8))
            {
                return "I feel quite safe around " + Main.npc[dryad].GivenName + ". She is one with nature, it is quite comforting.";
            }
            int witchdoctor = NPC.FindFirstNPC(NPCID.WitchDoctor);
            if (witchdoctor >= 0 && Main.rand.NextBool(8))
            {
                return "Can you tell " + Main.npc[witchdoctor].GivenName + " to please stay away from me, they seem very agressive.";
            }
            // Generic TownNPC dialogue
            switch (Main.rand.Next(8))
            {
                case 0:
                    return "Black n yellow, black n yellow, black n yellow, oh whats up?";
                case 1:
                    return "I'm quite good at playing jazz.";
                case 2:
                    return "Me and the townsfolk are celebrating tonight, bee there!";
                case 3:
                    return "My best friend still lives back at the hive, if you haven't killed her already.";
                case 4:
                    return "No, honey is not bee urine.";
                case 5:
                    return "No need to be scared, I dont sting!";
                case 6:
                    return "I think the Queen Bee had a husband, maybe watch out for that.";
                case 7:
                    return "My head is normal sized, I dont see what you are talking about";
                default:
                    return "I find the Bee outfit from the Queen Bee quite offensive.";
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Once a honeybee for the Hive, now an escaped refugee, ready to sell you all the exclusive bee items of the Hive... and also play jazz."),
            });
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Barry B Benson",
                "Beelliam",
                "Adam Flayman",
                "Buzzwell",
                "Jefferbee",
                "Jerobee",
                "Will O Wasp",
                "Henry Hornet",
                "Vincent Vespid",
                "Connor Cynipid"
            };
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Shop");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
        }

        public override void AddShops()
        {
            NPCShop shop = new NPCShop(Type, "Shop")
                .Add(ItemID.BottledHoney)
                .Add(ModContent.ItemType<TheTraitorsSaxophone>())
                .Add(ModContent.ItemType<NectarSlasher>())
                .Add(ItemID.BeeKeeper)
                .Add(ItemID.BeesKnees)
                .Add(ItemID.BeeGun)
                .Add(ModContent.ItemType<Wasparang>(), Condition.Hardmode)
                .Add(ModContent.ItemType<Nectarthrower>(), Condition.Hardmode)
                .Add(ModContent.ItemType<HoneyLocket>(), Condition.Hardmode)
                .Add(ModContent.ItemType<BombusApisBee.Items.Accessories.BeeKeeperDamageClass.HoneyBee>(), Condition.DownedMechBossAny)
                .Add(ModContent.ItemType<BeeInyGun>(), Condition.DownedGolem);
        }
    }
}