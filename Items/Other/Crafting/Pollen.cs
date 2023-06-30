using Terraria.DataStructures;

namespace BombusApisBee.Items.Other.Crafting
{
    public class Pollen : BeeKeeperItem
    {
        public bool useAltTex = false;

        public override string Texture => "BombusApisBee/Items/Other/Crafting/PollenOne";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pollen");
            Tooltip.SetDefault("'Careful if you're allergic'");
            Item.ResearchUnlockCount = 999;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
            Item.value = 10;
            Item.width = Item.height = 16;
        }

        public override void OnSpawn(IEntitySource source)
        {
            useAltTex = Main.rand.NextBool();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Items/Other/Crafting/PollenOne").Value;

            spriteBatch.Draw(tex, position, tex.Frame(), drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex, position, null, new Color(255, 150, 0, 0), 0f, glowTex.Size() / 2f, .4f, SpriteEffects.None, 0f);

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(30))
            {
                Dust.NewDustPerfect(Item.position + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.UnitY * -Main.rand.NextFloat(3f, 4f), 0, new Color(255, 150, 0) * .35f, 0.4f);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Items/Other/Crafting/PollenOne").Value;
            if (useAltTex)
                tex = ModContent.Request<Texture2D>("BombusApisBee/Items/Other/Crafting/PollenTwo").Value;

            spriteBatch.Draw(tex, Item.position - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex, Item.position - Main.screenPosition, null, new Color(255, 150, 0, 0) * MathHelper.Lerp(.5f, 1f, Utils.Clamp((float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 4f), 0, 1)), 0f, glowTex.Size() / 2f, 0.4f, SpriteEffects.None, 0f);

            return false;
        }
    }
}