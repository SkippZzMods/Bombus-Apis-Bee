using BombusApisBee.Core.BeekeeperClass;
using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Core.Common.Honeycomb
{
    public abstract class BaseHoneycombWeapon : BeekeeperWeapon
    {
        private readonly string name;
        private readonly string tooltip;

        private int fadeInBar;
        private int comboDecayTimer;

        protected BaseHoneycombWeapon(string name, string tooltip) : base()
        {
            this.name = name;
            this.tooltip = tooltip;
        }

        public virtual int MaxCombo => 5;

        public int CurrentCombo;

        public bool HasMaxCombo => CurrentCombo >= MaxCombo;

        // see below
        public virtual void AddStaticDefaults()
        {

        }

        public sealed override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault(name);
            Tooltip.SetDefault(tooltip);
        }

        // im not doing SafeSafeSetDefaults
        public virtual void AddDefaults()
        {

        }

        public sealed override void SafeSetDefaults()
        {
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;

            AddDefaults();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, HasMaxCombo ? 1 : 0);

            if (HasMaxCombo)
                CurrentCombo = 0;

            (p.ModProjectile as BaseHoneycombProjectile).ParentWeapon = this;

            return false;
        }

        public void AddCombo(int amount = 1)
        {
            CurrentCombo += amount;
            comboDecayTimer = 180;
        }

        public override void UpdateInventory(Player player)
        {
            if (!Main.playerInventory && player.HeldItem == Item)
            {
                if (fadeInBar < 15 && CurrentCombo > 0)
                    fadeInBar++;
                else if (CurrentCombo <= 0 && fadeInBar > 0)
                {
                    fadeInBar -= 2;
                    if (fadeInBar < 0)
                        fadeInBar = 0;
                }
            }
            else if (fadeInBar > 0)
                fadeInBar--;          
            
            if (comboDecayTimer > 0)
            {
                comboDecayTimer--;
                if (comboDecayTimer == 0 && CurrentCombo > 0)
                {
                    comboDecayTimer = 5;
                    CurrentCombo--;                
                }
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var tex = Request<Texture2D>(Texture).Value;

            Texture2D barFrontTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarFront").Value;

            Texture2D barBackTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarBack").Value;
            Texture2D barBackTexGlow = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarBack_Glow").Value;

            spriteBatch.Draw(tex, position, frame, drawColor, 0f, origin, scale, 0f, 0f);

            if (fadeInBar > 0)
            {
                float lerpInBar = fadeInBar / 15f;

                Vector2 drawPosition = position + new Vector2(0f, 8f);
                float factor = Math.Min(CurrentCombo / (float)MaxCombo, 1);

                var source = new Rectangle(0, 0, (int)(factor * barFrontTex.Width), barFrontTex.Height);
                var target = new Rectangle((int)(drawPosition.X),
                    (int)(drawPosition.Y), (int)(factor * barFrontTex.Width), (int)(barFrontTex.Height));

                //spriteBatch.Draw(glowTex, position + new Vector2(0f, 6f) - Main.screenPosition, null, new Color(152, 137, 255, 0) * 0.25f, 0, glowTex.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barBackTexGlow, drawPosition + new Vector2(0f, 6f), null, new Color(139, 70, 0, 0) * lerpInBar, 0, barBackTexGlow.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barBackTex, drawPosition + new Vector2(0f, 6f), null, Color.White * lerpInBar, 0, barBackTex.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barFrontTex, target, source, new Color(183, 97, 8) * lerpInBar, 0, new Vector2(barFrontTex.Width / 2, 0), 0, 0);

                spriteBatch.Draw(barFrontTex, target, source, new Color(234, 122, 0, 0) * lerpInBar, 0, new Vector2(barFrontTex.Width / 2, 0), 0, 0);
            }

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

            if (MaxCombo > 0)
            {
                int index = tooltips.Count;

                if (index != -1)
                    tooltips.Insert(index, new TooltipLine(Mod, "Combo Tooltip", $"[c/FFBC00:Has a maximum combo of {MaxCombo}]"));
            }
        }
    }
}
