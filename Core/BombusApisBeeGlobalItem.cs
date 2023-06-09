namespace BombusApisBee.Core
{
    public class BombusApisBeeGlobalItem : GlobalItem
    {
        public float secondtimer;
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            BombusApisBeeGlobalItem GlobalItem = (BombusApisBeeGlobalItem)base.Clone(item, itemClone);
            GlobalItem.secondtimer = secondtimer;
            return GlobalItem;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem != null && item.ModItem is BeeKeeperItem)
            {
                secondtimer++;
                int index;
                index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
                if (index != -1)
                {
                    float Beefade = (float)((Math.Sin((double)(6.2831855f / secondtimer) * Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "BeeKeeperItemTag", "- Bee Keeper Item -")
                    {
                        OverrideColor = Color.Lerp(new Color(255, 155, 0), new Color(255, 255, 0), Beefade)
                    }); ;
                }

            }
            if (item.ModItem != null && item.ModItem is BeeDamageItem)
            {
                secondtimer++;
                int index;
                index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
                if (index != -1)
                {

                    float Beefade = (float)((Math.Sin((double)(6.2831855f / secondtimer) * Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "BeeKeeperItemTag", "- Bee Keeper Item -")
                    {
                        OverrideColor = Color.Lerp(new Color(255, 155, 0), new Color(255, 255, 0), Beefade)
                    }); ;
                }
            }
        }
    }
}




