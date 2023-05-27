using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Items.Other.OnPickupItems
{
    public class HoneyPickup2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Droplet");
            Tooltip.SetDefault("you shouldn't see this....");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool ItemSpace(Player player)
        {
            return false;
        }

        public override bool OnPickup(Player player)
        {
            player.IncreaseBeeResource(8);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Items/Other/OnPickupItems/HoneyPickup2").Value;

            spriteBatch.Draw(tex, Item.position - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex, Item.position - Main.screenPosition + new Vector2(0f, 5), null, new Color(255, 200, 0, 0), 0f, glowTex.Size() / 2f, 0.4f, SpriteEffects.None, 0f);

            return false;
        }
    }
}