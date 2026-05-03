using BombusApisBee.BeeHelperProj;
using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Forest.Items.Pollen;
using CalamityMod.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Corruption
{
    public class ShadesentHoneycombShard : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases chance to strengthen friendly bees by 35%\nStrengthened bees can travel through walls, empowering their critical strike chance");
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
            player.Hymenoptra().BeeStrengthenChance += 0.35f;
            player.GetModPlayer<BombusApisCalamityPlayer>().ShadesentShard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(20)
            .AddIngredient<PearlShard>(2)
            .AddIngredient(ItemID.DemoniteBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadesentHoneycombShardGlobalProjectile : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if (!entity.friendly)
                return false;

            if (entity.type == ProjectileID.Bee || entity.type == ProjectileID.GiantBee)
                return true;

            if (entity.ModProjectile != null && entity.ModProjectile is BaseBeeProjectile)
                return true;

            return false;
        }

        public override bool InstancePerEntity => true;

        public int _inTileTimer;
        public int _origCrit;

        public override void SetDefaults(Projectile entity)
        {
            if (Enabled(entity))
                _origCrit = entity.CritChance;
        }

        public override void AI(Projectile projectile)
        {
            if (Enabled(projectile))
            {
                if (projectile.tileCollide)
                    projectile.tileCollide = false;

                if (_inTileTimer > 0)
                {
                    Main.NewText("testing" + _inTileTimer);

                    projectile.CritChance = _origCrit + 100;
                    _inTileTimer--;
                    if (_inTileTimer == 0)
                        projectile.CritChance = _origCrit;
                }
                else
                {
                    if (Collision.SolidTiles(projectile.Center, projectile.width, projectile.height))
                        _inTileTimer = 90;
                }
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (Enabled(projectile))
            {
                _inTileTimer = 90;
            }

            return false;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        internal static bool Enabled(Projectile p)
        {
            if (p.TryGetOwner(out Player player))
            {
                return player.GetModPlayer<BombusApisCalamityPlayer>().ShadesentShard;
            }

            return false;
        }
    }
}
