﻿using Terraria.DataStructures;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Balloon)]
    public class HoneyBee : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Summons a large Honeybee which will mimic your loyal bees\nIncreases maximum honey by 25");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(32);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 25);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 25;
            player.Bombus().HoneyBee = true;

            if (player.Hymenoptra().HoldingBeeWeaponTimer > 0)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<HoneyBeeProj>()] < 1)
                {
                    Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.One, ModContent.ProjectileType<HoneyBeeProj>(), 235, 4f, player.whoAmI).originalDamage = 235;
                }
                player.AddBuff(ModContent.BuffType<HoneyBeeBuff>(), 15);
            }
        }
    }
}