
using BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Desert;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Desert;
using BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Mushroom;
using CalamityMod.Items.TreasureBags;
using Terraria.GameContent.ItemDropRules;

namespace BombusApisBee.Content.Crossmod.Calamity.Core
{
    [JITWhenModsEnabled("CalamityMod")]
    public class BombusCalamityBossBags : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemType<DesertScourgeBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ItemType<ScourgedHoneycomb>(), 2));

                itemLoot.Add(ItemDropRule.Common(ItemType<ChippedTailspike>(), 4));
            }

            if (item.type == ItemType<CrabulonBag>())
                itemLoot.Add(ItemDropRule.Common(ItemType<MycelialHoneycomb>(), 2));

            if (item.type == ItemType<HiveMindBag>())
                itemLoot.Add(ItemDropRule.OneFromOptions(1, [ ItemType<ShadestingerScepter>(), ItemType<ShadestingerScythe>()]));
        }
    }
}
