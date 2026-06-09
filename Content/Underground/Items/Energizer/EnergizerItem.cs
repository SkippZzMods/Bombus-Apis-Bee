using BombusApisBee.Assets;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Underground.Items.Energizer
{
    public class EnergizerItem : BeekeeperAccessory
    {
        public EnergizerItem() : base("Energizer", "Increases beekeeper attack speed by 15%\nBees provide a small amount of light\nToggle this effect with accessory visibility") { }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                Color c = new Color(98, 255, 251) * 0.5f;

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.owner == player.whoAmI && p.ModProjectile is CommonBeeProjectile)
                        Lighting.AddLight(p.Center, c.R / 255f, c.G / 255f, c.B / 255f);
                }
            }
        }

        public override void SafeUpdateEquip(Player Player)
        {
            Player.GetAttackSpeed<BeekeeperDamageClass>() += 0.15f;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float fade = (float)Math.Sin(Main.timeForVisualEffects * 0.05f);
            if (fade < 0f)
                fade = 0f;

            Lighting.AddLight(Item.Center, new Vector3(98, 255, 251) * 0.001f * fade);

            //if (Main.rand.NextBool(30) && fade > 0.2f)
            //    Dust.NewDustPerfect(Item.Top, ModContent.DustType<ElectricityDust>(), Main.rand.NextVector2Circular(2f, 2f) - Vector2.UnitY * Main.rand.NextFloat(3f), 50, new Color(98, 255, 251, 0), 2f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fade = (float)Math.Sin(Main.timeForVisualEffects * 0.05f);
            if (fade < 0f)
                fade = 0f;

            spriteBatch.Draw(bloom, Item.Center + new Vector2(0, 2) - Main.screenPosition, null, new Color(98, 255, 251, 0) * 0.1f * fade, rotation, bloom.Size() / 2f, scale * .65f, SpriteEffects.None, 0f);

            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var outline = Request<Texture2D>(Texture + "_Outline").Value;

            float fade = (float)Math.Sin(Main.timeForVisualEffects * 0.05f);
            if (fade < 0f)
                fade = 0f;
            
            spriteBatch.Draw(outline, position, null, Color.White * 0.2f * fade, 0f, outline.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(bloom, position, null, new Color(98, 255, 251, 0) * 0.1f * fade, 0f, bloom.Size() / 2f, .4f, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var outline = Request<Texture2D>(Texture + "_Outline").Value;

            float fade = (float)Math.Sin(Main.timeForVisualEffects * 0.05f);
            if (fade < 0f)
                fade = 0f;

            spriteBatch.Draw(outline, Item.Center + new Vector2(0, 2) - Main.screenPosition, null, Color.White * 0.2f * fade, rotation, outline.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(bloom, Item.Center + new Vector2(0, 2) - Main.screenPosition, null, new Color(98, 255, 251, 0) * 0.1f * fade, rotation, bloom.Size() / 2f, scale * .4f, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(BombusApisBeeModSystem.SilverBarGroupID, 8).
                AddRecipeGroup(BombusApisBeeModSystem.CopperBarGroupID, 8).
                AddIngredient(ItemType<PollenItem>(), 5).
                AddTile(TileID.Anvils).
                Register();

        }
    }
}
