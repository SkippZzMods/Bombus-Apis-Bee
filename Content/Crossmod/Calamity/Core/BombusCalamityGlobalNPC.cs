using BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Desert;
using BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Wulfrum;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Desert;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Mushroom;
using BombusApisBee.Content.Crossmod.Calamity.NPCs.Enemies.Wulfrum;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.NormalNPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace BombusApisBee.Content.Crossmod.Calamity.Core
{
    [JITWhenModsEnabled("CalamityMod")]
    public class BombusCalamityGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public static List<int> WulfrumEnemies = [ NPCType<WulfrumAmplifier>(), NPCType<WulfrumDrone>(), NPCType<WulfrumGyrator>(), NPCType<WulfrumHovercraft>(), NPCType<WulfrumRover>(), NPCType<WulfrumHive>(),
        ];
        private IItemDropRule onlyInNormalMode(int ItemID, int chanceDenominator = 2)
        {
            return new ItemDropWithConditionRule(ItemID, chanceDenominator, 1, 1, new Conditions.NotExpert(), 1);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!CrossMod.Calamity.Enabled)
                return;

            if (npc.type == NPCType<DesertScourgeHead>())
            {
                npcLoot.Add(onlyInNormalMode(ItemType<ScourgedHoneycomb>()));
                npcLoot.Add(onlyInNormalMode(ItemType<ChippedTailspike>()));
            }

            if (WulfrumEnemies.Contains(npc.type))
                npcLoot.Add(ItemDropRule.Common(ItemType<GlisteningGear>(), 15));

            if (npc.type == NPCType<Crabulon>())
            {
                npcLoot.Add(onlyInNormalMode(ItemType<MycelialHoneycomb>()));
            }

            if (npc.type == NPCType<HiveMind>())
            {
                npcLoot.Add(onlyInNormalMode(ItemType<ShadestingerScepter>()));
                npcLoot.Add(onlyInNormalMode(ItemType<ShadestingerScythe>()));
            }         
        }
    }
}
