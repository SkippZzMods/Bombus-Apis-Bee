using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.BeeProjectile;
using Terraria;

/// essentially ModBuff that can be applied to projectiles, and is architectured specifically for Smokers
namespace BombusApisBee.Core.Common.Smoker
{
    /// <summary>
    /// A "buff" class that can be applied to CommonBeeProjectiles, Loyal Bee projectiles or Hostile NPCS
    /// </summary>
    /// 
    public abstract class SmokerBuff : ModType
    {
        /// <summary> The buff id of this smoker buff. </summary>
        public int Type { get; internal set; }

        protected override sealed void Register()
        {
            
        }

        public sealed override void Load()
        {
            Type = SmokerBuffLoader.Register(Name);
            SmokerBuffLoader.smokerBuffs.Add(this);
        }

        /// <summary>
        /// Should be used only when trying to modify a CommonBeeProjectile specifically, excluding Loyal Bee projectiles
        /// </summary>
        /// <param name="p">The projectile to check</param>
        /// <param name="bee">The CommonBeeProjectile (if not null)</param>
        /// <returns></returns>
        public static bool TryGetBee(Projectile p, out CommonBeeProjectile bee)
        {
            bee = p.ModProjectile as CommonBeeProjectile;

            return bee is not null;
        }

        /// <summary>
        /// A "Stronger" smoker buff would replace a weaker smoker buff if two were to be applied to the same projectile
        /// Two smoker buffs of the same priority simply do not override eachother
        /// </summary>
        
        public virtual int Priority => 1;

        public virtual void PreUpdateBuffedBees(Projectile p) { }

        public virtual void UpdateBuffedBees(Projectile p) { }
        public virtual void ModifyBuffedBeeHit(Projectile p, NPC target, ref NPC.HitModifiers modifiers) { }

        public virtual void OnBuffedBeeHit(Projectile p, NPC target, NPC.HitInfo hit, int damageDone) { }

        public virtual void PreDrawBuffedBees(Projectile p, ref Color lightColor) { }

        public virtual void PostDrawBuffedBees(Projectile p, Color lightColor) { }
        public virtual void PreUpdateDebuffedNPC(NPC n) { }
        public virtual void UpdateDebuffedNPC(NPC n) { }
        public virtual void ModifyDebuffedNPCHit(NPC npc, ref NPC.HitModifiers modifiers) { }
    }

    /// <summary>
    /// Handles loading all SmokerBuffs
    /// </summary>
    public class SmokerBuffLoader
    {
        public static readonly IList<SmokerBuff> smokerBuffs = new List<SmokerBuff>();
        public static readonly Dictionary<string, int> smokerBuffTypes = new();
        public static int buffCount;

        /// <summary>
        /// Gets the SmokerBuff instance with the given type. If no SmokerBuff with the given type exists, returns null.
        /// </summary>
        public static SmokerBuff GetBuff(int type) => smokerBuffs[type];

        internal static int Register(string key)
        {
            smokerBuffTypes.Add(key, buffCount);

            int reserveID = buffCount;
            buffCount++;
            return reserveID;
        }
    }

    /// <summary>
    /// Handles SmokerBuffs for projectiles
    /// </summary>
    public class SmokerGlobalProjectile : GlobalProjectile
    {
        // The amount of time between SmokerBuff applications
        public const int MAX_APPLICATION_COOLDOWN = 120;

        public override bool InstancePerEntity => true;

        public int _curBuffType = -1;
        public int _buffTimer;
        internal int applicationCooldown;
        public bool Active => _buffTimer > 0 && _curBuffType >= 0 && SmokerBuffLoader.GetBuff(_curBuffType) != null;
        public SmokerBuff CurrentActiveBuff => Active ? SmokerBuffLoader.GetBuff(_curBuffType) : null;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.ModProjectile is CommonBeeProjectile || entity.ModProjectile is LoyalBeeProjectile;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (applicationCooldown > 0)
                applicationCooldown--;

            if (Active)
            {
                _buffTimer--;

                if (_buffTimer <= 0)
                    _curBuffType = -1;
                else
                    CurrentActiveBuff.PreUpdateBuffedBees(projectile);
            }

            return true;
        }

        public override void AI(Projectile projectile)
        {
            if (Active)
                CurrentActiveBuff.UpdateBuffedBees(projectile);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Active)
                CurrentActiveBuff.ModifyBuffedBeeHit(projectile, target, ref modifiers);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
                CurrentActiveBuff.OnBuffedBeeHit(projectile, target, hit, damageDone);
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (Active)
                CurrentActiveBuff.PreDrawBuffedBees(projectile, ref lightColor);

            return true;
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (Active)
                CurrentActiveBuff.PostDrawBuffedBees(projectile, lightColor);
        }

        public static void ApplySmokerBuff(Projectile projectile, int buffID, int buffTime)
        {
            if (projectile.TryGetGlobalProjectile<SmokerGlobalProjectile>(out var gp))
            {
                if (gp.applicationCooldown > 0)
                    return;

                var buff = SmokerBuffLoader.GetBuff(buffID);

                if (gp.Active)
                {
                    if (gp.CurrentActiveBuff.Priority < buff.Priority)
                    {
                        gp._curBuffType = buffID;
                        gp.applicationCooldown = MAX_APPLICATION_COOLDOWN;
                        gp._buffTimer = buffTime;
                    }                       
                }
                else
                {
                    gp._curBuffType = buffID;
                    gp.applicationCooldown = MAX_APPLICATION_COOLDOWN;
                    gp._buffTimer = buffTime;
                }
            }
        }
    }

    /// <summary>
    /// Handles SmokerBuffs for NPCS
    /// </summary>
    public class SmokerGlobalNPC : GlobalNPC
    { 
        // The amount of time between SmokerBuff applications
        public const int MAX_APPLICATION_COOLDOWN = 120;

        public override bool InstancePerEntity => true;

        public int _curBuffType;
        public int _buffTimer;
        internal int applicationCooldown;
        public bool Active => _buffTimer > 0 && _curBuffType >= 0 && SmokerBuffLoader.GetBuff(_curBuffType) != null;
        public SmokerBuff CurrentActiveBuff => Active ? SmokerBuffLoader.GetBuff(_curBuffType) : null;

        public override bool PreAI(NPC npc)
        {
            if (Active)
            {
                CurrentActiveBuff.PreUpdateDebuffedNPC(npc);
            }

            return true;
        }

        public override void AI(NPC npc)
        {
            if (applicationCooldown > 0)
                applicationCooldown--;

            if (Active)
            {
                _buffTimer--;

                if (_buffTimer <= 0)
                    _curBuffType = -1;
                else
                    CurrentActiveBuff.UpdateDebuffedNPC(npc);
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (Active)
                CurrentActiveBuff.ModifyDebuffedNPCHit(npc, ref modifiers);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Active)
                CurrentActiveBuff.ModifyDebuffedNPCHit(npc, ref modifiers);
        }

        public static void ApplySmokerBuff(NPC npc, int buffID, int buffTime)
        {
            if (npc.TryGetGlobalNPC<SmokerGlobalNPC>(out var gnpc))
            {
                if (gnpc.applicationCooldown > 0)
                    return;

                var buff = SmokerBuffLoader.GetBuff(buffID);

                if (gnpc.Active)
                {
                    if (gnpc.CurrentActiveBuff.Priority < buff.Priority)
                    {
                        gnpc._curBuffType = buffID;
                        gnpc.applicationCooldown = MAX_APPLICATION_COOLDOWN;
                        gnpc._buffTimer = buffTime;
                    }
                }
                else
                {
                    gnpc._curBuffType = buffID;
                    gnpc.applicationCooldown = MAX_APPLICATION_COOLDOWN;
                    gnpc._buffTimer = buffTime;
                }
            }
        }
    }
}
