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

        /// <summary>
		/// Gets the instance of the accessory thats equipped in a player's normal slots or that is being simulated by other accessories.
		/// </summary>
		/// <param name="player">The player to get the equipped instance from.</param>
		/// <returns>The SmartAccessory instance if one is found, null if the item is not equipped or simulated.</returns>
		public BeekeeperAccessory GetEquippedInstance(Player player)
        {
            return GetEquippedInstance(player, Item.type);
        }

        /// <summary>
        /// Gets the instance of an equipped accessory based on it's type in a given player's normal slots, or being simulated by other accessories.
        /// </summary>
        /// <param name="player">The player to get the equipped instance from.</param>
        /// <param name="type">The type of accessory to look for, this should be the ID of an item extending SmartAccessory</param>
        /// <returns>The SmartAccessory instance if one is found, null if the item is not equipped or simulated.</returns>
        public static BeekeeperAccessory GetEquippedInstance(Player player, int type)
        {
            AccessoryPlayer mp = player.GetModPlayer<AccessoryPlayer>();

            return mp.equippedAccessories.FirstOrDefault(n => n.type == type)?.ModItem as BeekeeperAccessory;
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

        /// <summary>
        /// Mirrors ModPlayer.ResetEffects()
        /// </summary>
        public virtual void ResetEffects(Player player) { }

        public virtual void OnEquippedHit(Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnEquippedHit(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnEquippedHit(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) { }

        public virtual void ModifyEquippedHit(Player player, NPC target, ref NPC.HitModifiers modifiers) { }

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
            foreach (Item item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.ResetEffects(Player);
            }

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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (Item item in equippedAccessories)
            {
                var accessory = item.ModItem as BeekeeperAccessory;

                accessory.ModifyEquippedHit(Player, target, ref modifiers);
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
