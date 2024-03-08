using BombusApisBee.Items.Other.Crafting;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneycombOfTheGalaxies : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Of The Cosmos"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'An almighty honeycomb, said to be born at the center of the Cosmos'\nHold left click to channel a honeycomb radiating with universal energy\nThe honeycomb charges up, weakening the fabric of the Cosmos\n" +
                "Upon releasing the mouse button, the honeycomb will be channeled back to the owner\nThis action breaks the fabric of the cosmos, releasing powerful energies and galactic bees");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 55;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 25, 65, 0);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GalaxyHoneycombProj>();
            Item.shootSpeed = 9;
            Item.UseSound = SoundID.Item120;
            Item.scale = 1;
            Item.crit = 4;
            honeyCost = 5;
            Item.channel = true;
            Item.noUseGraphic = true;

            resourceChance = 0.33f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FragmentNebula, 8).AddIngredient(ItemID.FragmentSolar, 8).AddIngredient(ItemID.FragmentStardust, 8).AddIngredient(ItemID.FragmentVortex, 8).AddIngredient(ModContent.ItemType<PhotonFragment>(), 8).AddTile(TileID.LunarCraftingStation).Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            position.Y -= 6f * scale;
            Texture2D itemTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/Items/Weapons/BeeKeeperDamageClass/HoneycombOfTheGalaxies");
            Rectangle itemFrame = (Main.itemAnimations[base.Item.type] == null) ? Utils.Frame(itemTexture, 1, 1, 0, 0) : Main.itemAnimations[Item.type].GetFrame(itemTexture);
            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;
            Vector2 displacement = Utils.RotatedBy(Vector2.UnitX, (double)(Main.GlobalTimeWrappedHourly * 3f), default(Vector2)) * 4.5f * (float)Math.Sin((double)Main.GlobalTimeWrappedHourly);
            spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw(itemTexture, position + displacement, new Rectangle?(itemFrame), Color.White, 0f, origin, scale, 0, 0f);
            Main.spriteBatch.Draw(itemTexture, position - displacement, new Rectangle?(itemFrame), Color.White, 0f, origin, scale, 0, 0f);
            spriteBatch.End();
            Main.spriteBatch.Begin(0, null, null, null, null, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw(itemTexture, position, new Rectangle?(itemFrame), Color.White, 0f, origin, scale, 0, 0f);
            return false;
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<GalaxyHoneycombProj>()] <= 0;
        }
    }
}