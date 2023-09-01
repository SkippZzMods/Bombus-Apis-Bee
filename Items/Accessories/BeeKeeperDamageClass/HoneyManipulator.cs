using Humanizer;
using Terraria.DataStructures;
using Terraria.Localization;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneyManipulator : BeeKeeperItem
    {
        public static int MaxHoney = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxHoney);

        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("\nTP\n+20 max Honey\nThis effect has a cooldown of 90 seconds\nYou cannot regenerate honey for 5 seconds after activating the ability");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(12, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = BombusApisBee.HoneyManipulatorHotkey.TooltipHotkeyString();
            foreach (TooltipLine line1 in list)
            {
                if (line1.Mod == "Terraria" && line1.Name == "Tooltip1")
                {
                    
                    line1.Text = Language.GetTextValue("Mods.BombusApisBee.Items.HoneyManipulator.Hotkey").FormatWith(hotkey);
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().HoneyManipulator = true;
            player.Hymenoptra().BeeResourceMax2 += 20;
        }
    }
}
