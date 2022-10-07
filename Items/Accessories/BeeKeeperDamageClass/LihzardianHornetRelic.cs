using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class LihzardianHornetRelic : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases movement speed and wing flight time by 35%\nmodifyTooltips\nWhile channeled, your hymenoptra damage, attack speed, and crit chance are increased by 12%\nYou take 10% more damage and take damage-over-time while channeling\nThis effect has a 45 second cooldown");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = BombusApisBee.LihzardianRelicHotkey.TooltipHotkeyString();
            foreach (TooltipLine line1 in list)
            {
                if (line1.Mod == "Terraria" && line1.Name == "Tooltip1")
                {
                    line1.Text = "Press " + hotkey + " to channel the power of the sun for a short time";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().LihzardRelic = true;
            player.Bombus().wingFlightTimeBoost += 0.35f;
            player.moveSpeed += 0.35f;
        }
    }
}
