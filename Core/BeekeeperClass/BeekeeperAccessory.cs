using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace BombusApisBee.Core.BeekeeperClass
{
    public abstract class BeekeeperAccessory : ModItem
    {
        public static int ACCESSORY_START_INDEX = 3;
        public static int ACCESSORY_END_INDEX = 9;
        public static int VANITY_ACCESSORY_START_INDEX = 13;
        public static int VANITY_ACCESSORY_END_INDEX = 19;
        public static int DEFAULT_ACCESSORY_SLOT_COUNT = 5;
        
        private readonly string name;
        private readonly string tooltip;

        protected BeekeeperAccessory(string name, string tooltip) : base()
        {
            this.name = name;
            this.tooltip = tooltip;
        }

        public bool IsEquipped(Player player)
        {
            AccessoryPlayer mp = player.GetModPlayer<AccessoryPlayer>();

            if (mp.equippedAccessories.Any(n => n.type == Item.type))
                return true;

            return false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(name);
            Tooltip.SetDefault(tooltip);
        }

        public virtual void OnEquip(Player player, Item item) { }
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            SafeSetDefaults();     

            Item.accessory = true;
        }

        public virtual void SafeUpdateEquip(Player Player) { }
        public sealed override void UpdateEquip(Player player)
        {
            player.GetModPlayer<AccessoryPlayer>().equippedTypes.Add(Item.type);

            // If the pointer to this item is not being tracked for updates yet, add it
            if (!IsEquipped(player))
            {
                player.GetModPlayer<AccessoryPlayer>().equippedAccessories.Add(Item);
                OnEquip(player, Item);
            }

            SafeUpdateEquip(player);
        }

        public virtual void OnEquippedHit(Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnEquippedHit(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnEquippedHit(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) { }

        public virtual void OnHurtWhileEquipped(Player player, Player.HurtInfo info) { }

        /// <summary>
        /// Can be equivocated to ModPlayer.Shoot, but can also be called from outside sources (Apiaries, for example, because they are a held projectile)
        /// </summary>
        public virtual void OnWeaponUse(Player player, int damage, float knockBack) { }
    }

    /// <summary>
    /// Provides common helpers for common methods used by accessories, beyond UpdateEquip
    /// </summary>
    public class AccessoryPlayer : ModPlayer
    {
        public List<Item> equippedAccessories = new();
        public List<int> equippedTypes = new();
        public override void ResetEffects()
        {
            equippedAccessories.RemoveAll(n => !equippedTypes.Contains(n.type));

            equippedTypes.Clear();
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            foreach (Item item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.OnHurtWhileEquipped(Player, info);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (Item item in equippedAccessories) 
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.OnEquippedHit(Player, target, hit, damageDone);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (Item Item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.OnEquippedHit(Player, Item, target, hit, damageDone);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (Item item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.OnEquippedHit(Player, proj, target, hit, damageDone);
            }
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            OnWeaponUse(damage, knockback);

            return true;
        }

        public void OnWeaponUse(int damage, float knockback)
        {
            foreach (Item item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.OnWeaponUse(Player, damage, knockback);
            }
        }
    }
}
