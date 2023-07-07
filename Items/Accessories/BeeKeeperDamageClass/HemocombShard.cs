using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HemocombShard : BeeKeeperItem
    {
        public override void Load()
        {
            BombusApisBeeGlobalProjectile.StrongBeeKillEvent += SpawnBloodsplosion;
        }

        private void SpawnBloodsplosion(Projectile proj, int timeLeft)
        {
            if (Main.player[proj.owner].Bombus().HasHemocombShard && Main.rand.NextBool(10))
            {
                for (int i = 0; i < 5; i++)
                {
                    int item = Item.NewItem(proj.GetSource_Death(), proj.getRect(), Main.rand.Next(new int[] { ItemType<HemocombShardChunk1>(), ItemType<HemocombShardChunk2>(), ItemType<HemocombShardChunk3>() }));
                    Main.item[item].velocity += Main.rand.NextVector2CircularEdge(2f, 2f);

                    if (Main.netMode == NetmodeID.MultiplayerClient && item >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                    }
                }

                new SoundStyle("BombusApisBee/Sounds/Item/GoreHeavy").PlayWith(proj.Center, 0, 0, 1.25f);

                Main.player[proj.owner].Bombus().AddShake(15);

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(proj.Center, DustID.Blood, Main.rand.NextVector2Circular(7f, 7f), Main.rand.Next(150), default, 2.25f).fadeIn = 1f;

                    Dust dust = Dust.NewDustPerfect(proj.Center, DustID.Blood, Main.rand.NextVector2Circular(15f, 15f), Main.rand.Next(150), default, 2.25f);
                    dust.fadeIn = 1f;
                    dust.noGravity = true;

                    Dust.NewDustPerfect(proj.Center, DustID.BloodWater, Main.rand.NextVector2Circular(7f, 7f), Main.rand.Next(150), default, 1.65f).fadeIn = 1f;

                    Dust.NewDustPerfect(proj.Center, DustType<GoreBloodDark>(), Main.rand.NextVector2Circular(15f, 15f) - Vector2.UnitY * 2f, 0, default, 1.5f).fadeIn = 1f;

                    Dust.NewDustPerfect(proj.Center, DustType<GoreBloodLight>(), Main.rand.NextVector2Circular(15f, 15f) - Vector2.UnitY * 2f, 0, default, 1.5f).fadeIn = 1f;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 30%\nStrengthened bees have a chance to explode into healing meat chunks");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.3f;
            player.Bombus().HasHemocombShard = true;
        }
    }

    internal abstract class BaseChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meat Chunk");
            Tooltip.SetDefault("You shouldn't see this...");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
        }

        public override bool ItemSpace(Player Player)
        {
            return true;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal(Main.rand.Next(1, 3));
            player.AddBuff(BuffID.Regeneration, 300);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;


            spriteBatch.Draw(bloomTex, Item.Center - Main.screenPosition, null, new Color(100, 20, 20, 0) * 0.5f, rotation, bloomTex.Size() / 2f, scale * 0.5f, 0f, 0f);

            spriteBatch.Draw(texGlow, Item.Center - Main.screenPosition, null, new Color(100, 20, 20, 0) * 0.5f, rotation, texGlow.Size() / 2f, scale, 0f, 0f);

            spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, lightColor, rotation, tex.Size() / 2f, scale, 0f, 0f);

            spriteBatch.Draw(bloomTex, Item.Center - Main.screenPosition, null, new Color(200, 50, 50, 0) * 0.35f, rotation, bloomTex.Size() / 2f, scale * 0.35f, 0f, 0f);

            return false;
        }
    }

    internal class HemocombShardChunk1 : BaseChunk { }
    internal class HemocombShardChunk2 : BaseChunk { }
    internal class HemocombShardChunk3 : BaseChunk { }
}
