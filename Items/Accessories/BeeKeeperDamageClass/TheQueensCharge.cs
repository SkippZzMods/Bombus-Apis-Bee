namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Shield)]
    public class TheQueensCharge : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Double tap any direction to dash\nCreates bees as you dash");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(gold: 4);
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<QueensChargeDashPlayer>().DashAccessoryEquipped = true;
        }
    }

    public class QueensChargeDashPlayer : ModPlayer
    {
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;
        public const int DashCooldown = 65;
        public const int DashDuration = 40;
        public const float DashVelocity = 12.5f;
        public int DashDir = -1;
        public bool DashAccessoryEquipped;
        public int DashDelay = 0;
        public int DashTimer = 0;
        public override void ResetEffects()
        {
            DashAccessoryEquipped = false;
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
                DashDir = DashDown;
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
                DashDir = DashUp;
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                DashDir = -1;
        }
        public override void PreUpdateMovement()
        {
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashUp when Player.velocity.Y > -DashVelocity:
                    case DashDown when Player.velocity.Y < DashVelocity:
                        {
                            float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * DashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Honey2, 0f, 0f, Scale: Main.rand.NextFloat(1.2f, 1.5f)).noGravity = true;
                    Dust.NewDustPerfect(Player.Center, DustID.Bee, (Player.velocity.RotatedByRandom(0.4f)) * Main.rand.NextFloat(0.3f, 0.4f), 0, default, Main.rand.NextFloat(0.6f, 1.2f)).noGravity = true;
                }
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            {
                if (DashTimer % 10 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectileDirect(Player.GetSource_Accessory(new Item(ModContent.ItemType<TheQueensCharge>())), Player.Center, Vector2.One.RotatedByRandom(6.28f) * 3.5f, Player.beeType(),
                            Player.beeDamage(Player.ApplyHymenoptraDamageTo(10)), Player.beeKB(2f), Player.whoAmI).DamageType = BeeUtils.BeeDamageClass();
                    }
                    const int Repeats = 35;
                    for (int i = 0; i < Repeats; ++i)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)Repeats;
                        Dust dust3 = Dust.NewDustPerfect(Player.Center, DustID.Honey2, null, 0, default(Color), 1.1f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                        dust3.noGravity = true;
                    }
                }
                DashTimer--;
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == 0
                && !Player.setSolar
                && !Player.mount.Active;
        }
    }
}