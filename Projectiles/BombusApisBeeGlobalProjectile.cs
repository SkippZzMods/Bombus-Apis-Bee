using BombusApisBee.BeeDamageClass;
using Terraria.DataStructures;

namespace BombusApisBee.Projectiles
{
    public class BombusApisBeeGlobalProjectile : GlobalProjectile
    {
        public bool ForceBee;
        public bool HeldProj;
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo { Entity: Item i })
            {
                if (i.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = i.DamageType;
            }

            else if (source is EntitySource_Parent { Entity: Item item })
            {
                if (item.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = item.DamageType;
            }

            else if (source is EntitySource_Parent { Entity: Projectile proj })
            {
                if (proj.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = proj.DamageType;
            }

            else if (source is EntitySource_Death { Entity: Projectile proj2 })
            {
                if (proj2.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = proj2.DamageType;
            }

            else if (source is EntitySource_OnHit { EntityStriking: Projectile proj3 })
            {
                if (proj3.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = proj3.DamageType;
            }
            else if (source is EntitySource_OnHit { EntityStriking: Player player })
            {
                if (player.HeldItem.DamageType == BeeUtils.BeeDamageClass())
                    projectile.DamageType = player.HeldItem.DamageType;
            }
        }
        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Wasp)
            {
                projectile.usesIDStaticNPCImmunity = true;
                projectile.idStaticNPCHitCooldown = 10;
            }
            if (projectile.GetGlobalProjectile<BombusApisBeeGlobalProjectile>().ForceBee)
            {
                projectile.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
                projectile.minion = false;
                projectile.friendly = true;
                projectile.hostile = false;
            }
        }
        public override void AI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Wasp)
                projectile.extraUpdates = 1;
            if (projectile.CountsAsClass<HymenoptraDamageClass>() && projectile.CritChance == 0)
                projectile.CritChance = (int)Main.player[projectile.owner].GetTotalCritChance<HymenoptraDamageClass>();
        }
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if ((projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Wasp) && player.GetModPlayer<BombusApisBeePlayer>().IgnoreWater)
                projectile.ignoreWater = true;
            return true;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.CountsAsClass<HymenoptraDamageClass>())
                if (projectile.CritChance <= 0)
                {
                    int critChance = (int)Main.player[projectile.owner].GetTotalCritChance<HymenoptraDamageClass>();
                    if (Main.rand.Next(100) < critChance)
                        crit = true;
                }
        }
    }

}
