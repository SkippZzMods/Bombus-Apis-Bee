using BombusApisBee.Items.Accessories.BeeKeeperDamageClass;
using BombusApisBee.Items.Weapons.BeeKeeperDamageClass;
using Terraria.GameContent.ItemDropRules;

namespace BombusApisBee.Core
{
    public class BossBags : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.KingSlimeBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<GelatinousHoneycomb>()));

            if (item.type == ItemID.EyeOfCthulhuBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThePeeperPoker>(), 2));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RetinaReleaser>()));
            }

            if (item.type == ItemID.EaterOfWorldsBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaterOfHoneycombs>()));

            if (item.type == ItemID.BrainOfCthulhuBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrainyHoneycomb>()));

            if (item.type == ItemID.QueenBeeBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheQueensCharge>()));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Needleshot>()));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheSting>()));
            }

            if (item.type == ItemID.SkeletronBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CartilageCreator>()));

            if (item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<HoneyShot>()));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BeeEmblem>()));
            }

            if (item.type == ItemID.SkeletronPrimeBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MetalPlatedHoneycomb>()));

            if (item.type == ItemID.TwinsBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OcularRemote>()));

            if (item.type == ItemID.DestroyerBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProbeyComb>()));

            if (item.type == ItemID.PlanteraBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<HimensApiary>()));

            if (item.type == ItemID.GolemBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TomeOfTheSun>()));

            if (item.type == ItemID.MoonLordBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheQueensLarvae>()));
        }
    }
}