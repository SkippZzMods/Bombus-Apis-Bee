using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    internal class WoodenApiary : ApiaryItem
    {
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Apiary");
            Tooltip.SetDefault("Rapidly ejects bees\nHold <right> to take control of the bees, causing them to deal 50% increased damage");
        }

        public override void AddDefaults()
        {
            Item.damage = 5;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 14;
            Item.useAnimation = 14;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.15f;

            Item.value = Item.sellPrice(silver: 1);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileID.Bee;

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1 + Main.rand.Next(1, 3); i++)
            {
                BeeUtils.SpawnBee(player, source, position + Main.rand.NextVector2Circular(15f, 15f), velocity.RotatedByRandom(0.05f), damage, knockback);
            }

            return false;
        }
    }
}
