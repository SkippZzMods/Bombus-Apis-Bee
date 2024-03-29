﻿using BombusApisBee.Items.Other.Consumables;
using BombusApisBee.Items.Other.Crafting;
using Terraria.Chat;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ID;

namespace BombusApisBee.Core
{
    public class BombusApisBeeGlobalNPCs : GlobalNPC
    {
        public int[] BeeHitCooldown = new int[256];

        public string NPCShopKey1 = "A faint revving can be heard from the Traitor Bee's shop";
        public string NPCShopKey2 = "An incessant buzzing is heard coming from the Traitor Bee's shop";
        public static List<int> PillarList = new List<int>(){
        NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarSpearman, 419, 417, 427, 428, 426, 425, 429, 421, 423, 420, 424, 412, 413, 407, 402, 403, 404, 405, 406, 411, 409, 410};
        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            for (int i = 0; i < 256; i++)
            {
                if (BeeHitCooldown[i] > 0)
                    BeeHitCooldown[i]--;
            }
        }
        public override bool PreKill(NPC npc)
        {
            if (npc.type == NPCID.Golem && !NPC.downedGolemBoss)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(NPCShopKey1), new Color?(Color.Orange).Value);
                    return true;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(NPCShopKey1, new object[0]), new Color?(Color.Orange).Value, -1);
                }
            }
            bool lastTwinStanding = false;
            if (npc.type == NPCID.Retinazer)
            {
                lastTwinStanding = !NPC.AnyNPCs(126);
            }
            else if (npc.type == NPCID.Spazmatism)
            {
                lastTwinStanding = !NPC.AnyNPCs(125);
            }
            bool mechBossFirst = !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);
            if (!NPC.downedMechBossAny && mechBossFirst)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(NPCShopKey2), new Color?(Color.Orange).Value);
                    return true;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(NPCShopKey2, new object[0]), new Color?(Color.Orange).Value, -1);
                }
            }
            return true;
        }

        private IItemDropRule onlyInNormalMode(int ItemID, int chanceDenominator = 2)
        {
            return new ItemDropWithConditionRule(ItemID, chanceDenominator, 1, 1, new Conditions.NotExpert(), 1);
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.ZombieMerman || npc.type == NPCID.EyeballFlyingFish)
            {
                npcLoot.Add(new ItemDropWithConditionRule(ItemType<HemocombShard>(), 8, 1, 1, new HemocombShardCondition()));
            }

            if (npc.type == NPCID.BloodZombie)
            {
                npcLoot.Add(new ItemDropWithConditionRule(ItemType<HemocombShard>(), 100, 1, 1, new HemocombShardCondition()));
            }            

            if (npc.type == NPCID.FlyingSnake || npc.type == NPCID.Lihzahrd || npc.type == NPCID.LihzahrdCrawler)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LihzardianHornetRelic>(), 35));

            Conditions.YoyosYelets cond = new();
            IItemDropRule rl = new LeadingConditionRule(cond);
            rl.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BandOfTheHive>(), 150));
            npcLoot.Add(rl);

            npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<NectarVial>(), 100, 1, 1, new NectarVialCondition()));
            npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<TriTipStinger>(), 50, 1, 1, new TriTipStingerCondition()));

            if (npc.type == NPCID.IceQueen)
            {
                Conditions.FrostMoonDropGatingChance condition = new();
                IItemDropRule rule = new LeadingConditionRule(condition);
                rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FrozenStinger>(), 5));
                npcLoot.Add(rule);
            }

            if (npc.type == NPCID.Pumpking)
            {
                Conditions.PumpkinMoonDropGatingChance condition = new();
                IItemDropRule rule = new LeadingConditionRule(condition);
                rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PumpkinetScepter>(), 5));
                npcLoot.Add(rule);
            }

            if (npc.type == NPCID.AngryNimbus)
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ElectricHoneycomb>(), 15, 10));

            if (npc.type == NPCID.EyeofCthulhu)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<ThePeeperPoker>()));

            if (npc.type == NPCID.MartianSaucerCore)
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<HoneyManipulator>(), 5, 4));

            if (npc.type == NPCID.QueenBee)
            {
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<Needleshot>()));
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<RoyalJelly>(), 2, 1));
            }

            if (npc.type == NPCID.SkeletronHead)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<Skelecomb>()));

            if (npc.type == NPCID.EyeofCthulhu)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<RetinaReleaser>()));

            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<BeeEmblem>()));
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<HoneyShot>()));
            }

            if (npc.type == NPCID.DarkCaster)
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ManaInfusedHoneycomb>(), 20, 15));

            if (npc.type == NPCID.KingSlime)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<GelatinousHoneycomb>()));

            if (npc.type == NPCID.SkeletronPrime)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<MetalPlatedHoneycomb>()));

            if (npc.type == NPCID.Spazmatism)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<OcularRemote>()));

            if (npc.type == NPCID.TheDestroyer)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<ProbeyComb>()));

            if (npc.type == NPCID.MoonLordCore)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<TheQueensLarvae>()));

            if (npc.type == NPCID.Golem)
                npcLoot.Add(onlyInNormalMode(ModContent.ItemType<TomeOfTheSun>()));

            if (npc.type == NPCID.MartianSaucerCore)
            {
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<LaserbeemBlaster>(), 5, 4));
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<BeeBubbleBlaster>(), 5, 4));
            }

            if (npc.lifeMax > 5 && npc.value > 0f && !NPCID.Sets.CountsAsCritter[npc.type] && !npc.boss)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Pollen>(), 5, 1, 3));

            if (PillarList.Contains(npc.type))
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PhotonFragment>(), 5, 4));
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
            {
                shop.InsertBefore(ItemID.PiggyBank, ModContent.ItemType<RottenHoneycombShard>());
            }
        }
    }
}
