using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class BeeSniperGoggles : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("15% increased hymenoptra critical strike chance\nIncreases maximum honey by 35");
            SacrificeTotal = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BeeSniperArmor>() && legs.type == ModContent.ItemType<BeeSniperLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Striking enemies has a chance to mark them for 10 seconds\nMarked enemies take 25% more damage and have double the chance to be critically striked\n" +
                "While an enemy is marked, non-marked enemies take 15% less damage\nMarking enemies has a cooldown of 20 seconds";
            player.Bombus().BeeSniperSet = true;
        }

        public override void UpdateEquip(Player player)
        {
            BeeDamagePlayer.ModPlayer(player).BeeResourceMax2 += 35;
            player.IncreaseBeeCrit(15);
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 8).AddIngredient(ItemID.BeeWax, 15).AddIngredient(ItemID.TitaniumBar, 13).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.Silk, 8).AddIngredient(ItemID.BeeWax, 15).AddIngredient(ItemID.AdamantiteBar, 13).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    class MarkedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool marked;

        public int markedDuration;

        public override void ResetEffects(NPC npc)
        {
            if (marked && --markedDuration < 0)
                marked = false;
        }
    }

    class MarkedNPCDrawer
    {
        public static RenderTarget2D NPCTarget;
        public static void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                NPCTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
            Main.OnPreDraw += Main_OnPreDraw;
            On.Terraria.Main.DrawNPCs += DrawMarkedEffects;
        }

        public static void Unload()
        {
            Main.OnPreDraw -= Main_OnPreDraw;
            On.Terraria.Main.DrawNPCs -= DrawMarkedEffects;
        }

        private static void Main_OnPreDraw(GameTime obj)
        {
            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.gameMenu || Main.dedServ || spriteBatch is null || NPCTarget is null || gD is null)
                return;

            RenderTargetBinding[] bindings = gD.GetRenderTargets();
            gD.SetRenderTarget(NPCTarget);
            gD.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, default, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC NPC = Main.npc[i];

                if (NPC.active && NPC.GetGlobalNPC<MarkedGlobalNPC>().marked)
                {
                    if (NPC.ModNPC != null)
                    {
                        if (NPC.ModNPC != null && NPC.ModNPC is ModNPC ModNPC)
                        {
                            if (ModNPC.PreDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White)))
                                Main.instance.DrawNPC(i, false);

                            ModNPC.PostDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White));
                        }
                    }
                    else
                        Main.instance.DrawNPC(i, false);
                }
            }
            spriteBatch.End();
            gD.SetRenderTargets(bindings);
        }
        private static void DrawMarkedEffects(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            NPC drawTarget = Main.npc.Where(n => n.active && n.GetGlobalNPC<MarkedGlobalNPC>().marked).FirstOrDefault();
            if (drawTarget != default)
            {
                var gNPC = drawTarget.GetGlobalNPC<MarkedGlobalNPC>();
                GraphicsDevice gD = Main.graphics.GraphicsDevice;
                SpriteBatch spriteBatch = Main.spriteBatch;

                if (Main.dedServ || spriteBatch == null || NPCTarget == null || gD == null)
                    return;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

                Effect effect = Filters.Scene["MarkedEffect"].GetShader().Shader;
                effect.Parameters["uImageSize0"].SetValue(NPCTarget.Size());
                float alpha = MathHelper.Lerp(1f, 0.5f, (float)Math.Sin((gNPC.markedDuration / 600f) * 25f));
                if (gNPC.markedDuration > 570)
                    alpha = MathHelper.Lerp(1f, 0f, (gNPC.markedDuration - 570) / 30f);
                if (gNPC.markedDuration < 30)
                    alpha = MathHelper.Lerp(0.5f, 0f, 1f - (gNPC.markedDuration / 30f));
                effect.Parameters["alpha"].SetValue(alpha);
                effect.Parameters["colorOne"].SetValue(new Color(225, 220, 110).ToVector4());
                effect.Parameters["colorTwo"].SetValue(new Color(215, 160, 80).ToVector4());

                effect.Parameters["whiteness"].SetValue(0f);

                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(NPCTarget, new Rectangle((int)-Main.LocalPlayer.velocity.X, (int)-Main.LocalPlayer.velocity.Y, (int)NPCTarget.Size().X, (int)NPCTarget.Size().Y), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
