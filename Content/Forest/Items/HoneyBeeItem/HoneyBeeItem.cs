using BombusApisBee.Core.BeekeeperClass;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.HoneyBeeItem
{
    [AutoloadEquip(EquipType.Balloon)]
    public class HoneyBeeItem : BeeKeeperItem
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
            var modPlayer = BeekeeperPlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 25;
            player.Bombus().HoneyBee = true;

            if (player.Beekeeper().HoldingBeeWeaponTimer > 0)
            {
                if (player.ownedProjectileCounts[ProjectileType<HoneyBeeProj>()] < 1)
                {
                    Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.One, ProjectileType<HoneyBeeProj>(), 235, 4f, player.whoAmI).originalDamage = 235;
                }
                player.AddBuff(BuffType<HoneyBeeBuff>(), 15);
            }
        }
    }
}