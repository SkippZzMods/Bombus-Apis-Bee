namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class BeeSniperGoggles : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("15% increased hymenoptra critical strike chance\nIncreases maximum honey by 35\nIncreases your amount of Loyal Bees by 1");
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
                "While an enemy is marked, non-marked enemies take 15% less damage\nMarking enemies has a cooldown of 15 seconds\nKilling the Marked enemy resets the cooldown";
            player.Bombus().BeeSniperSet = true;
        }

        public override void UpdateEquip(Player player)
        {
            BeeDamagePlayer.ModPlayer(player).BeeResourceMax2 += 35;
            player.IncreaseBeeCrit(15);
            player.Hymenoptra().CurrentBees += 1;
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

        public int markedEffectTimer;

        public override void ResetEffects(NPC npc)
        {
            if (marked && --markedDuration < 0)
            {
                marked = false;
            }

            if (true)
            {
                markedEffectTimer++;
                if (markedEffectTimer > 5)
                    markedEffectTimer = 0;
            }
            else if (markedEffectTimer > 0)
                markedEffectTimer = 0;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (marked)
            {
                Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeSniperMarked").Value;
                Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeSniperMarked_Glow").Value;
                Texture2D texGlowAlt = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeSniperMarked_GlowAlt").Value;

                Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

                float scale = 2.5f;

                float width = npc.width / 20f;

                if (width > 2.5f)
                    scale = width;

                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.025f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.0025f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                effect.Parameters["repeats"].SetValue(1);
                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);
                Color color = new Color(255, 150, 0, 0);

                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);

                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(tex, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, tex.Size() / 2f, 0.25f * scale, 0f, 0f);

                spriteBatch.Draw(texGlowAlt, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, texGlowAlt.Size() / 2f, 0.25f * scale, 0f, 0f);

                spriteBatch.Draw(texGlow, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, texGlow.Size() / 2f, 0.25f * scale, 0f, 0f);

                float otherScale = MathHelper.Lerp(0.1f, 0.25f, markedEffectTimer / 5f);

                effect.Parameters["uColor"].SetValue((new Color(255, 150, 0, 0) * 0.65f).ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(tex, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, tex.Size() / 2f, otherScale * scale, 0f, 0f);

                spriteBatch.Draw(texGlowAlt, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, texGlowAlt.Size() / 2f, otherScale * scale, 0f, 0f);

                spriteBatch.Draw(texGlow, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, texGlow.Size() / 2f, otherScale * scale, 0f, 0f);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(glowTex, npc.Center - Main.screenPosition, null, new Color(255, 150, 0, 0) * 0.25f, 0f, glowTex.Size() / 2f, 0.45f * scale, 0f, 0f);

            }
        }
    }
}

/*class MarkedNPCDrawer
    {
        public static ScreenTarget target = new(DrawNPCTarget, () => active, 1);

        static bool active => Main.npc.Any(n => n.active && n.GetGlobalNPC<MarkedGlobalNPC>().marked);

        public static int timer;

        public static void Load()
        {
            On.Terraria.Main.DrawNPCs += DrawMarkedEffects;
        }

        public static void Unload()
        {
            On.Terraria.Main.DrawNPCs -= DrawMarkedEffects;
        }

        private static void DrawNPCTarget(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC NPC = Main.npc[i];

                if (NPC.active)
                {
                    if (NPC.ModNPC != null)
                    {
                        ModNPC ModNPC = NPC.ModNPC;

                        if (ModNPC.PreDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White)))
                            Main.instance.DrawNPC(i, false);

                        ModNPC.PostDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White));
                    }
                    else
                    {
                        Main.instance.DrawNPC(i, false);
                    }
                }
            }
        }

        private static void DrawMarkedEffects(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            if (!behindTiles && active)
            {
                GraphicsDevice gD = Main.graphics.GraphicsDevice;
                SpriteBatch spriteBatch = Main.spriteBatch;

                if (Main.dedServ || spriteBatch == null || target == null || gD == null)
                    return;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

                Effect effect = Filters.Scene["MarkedEffect"].GetShader().Shader;
                effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                float alpha = MathHelper.Lerp(1f, 0.5f, (float)Math.Sin((timer / 600f) * 25f));
                if (timer > 570)
                    alpha = MathHelper.Lerp(1f, 0f, (timer - 570) / 30f);
                if (timer < 30)
                    alpha = MathHelper.Lerp(0.5f, 0f, 1f - (timer / 30f));

                effect.Parameters["alpha"].SetValue(alpha);
                effect.Parameters["colorOne"].SetValue(Color.Lerp(new Color(225, 220, 110), new Color(215, 160, 80), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + 1)).ToVector4());
                effect.Parameters["colorTwo"].SetValue(Color.Lerp(new Color(215, 160, 80), new Color(225, 220, 110), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + 1)).ToVector4());

                effect.Parameters["whiteness"].SetValue(0.15f);

                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(target.RenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}*/
