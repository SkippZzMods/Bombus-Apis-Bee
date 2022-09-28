using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BombusApisBee
{
    public static class BeeUtils
    {
        public static void DrawDustImage(Vector2 position, int dustType, float size, Texture2D tex, float dustSize = 1f, int Alpha = 0, Color? color = null, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(0 - rot, rot);
                Color[] data = new Color[tex.Width * tex.Height];
                tex.GetData(data);
                for (int i = 0; i < tex.Width; i += 2)
                {
                    for (int j = 0; j < tex.Height; j += 2)
                    {
                        Color alpha = data[j * tex.Width + i];
                        if (alpha == new Color(255, 255, 255))
                        {
                            double dustX = (i - (tex.Width / 2));
                            double dustY = (j - (tex.Height / 2));
                            dustX *= size;
                            dustY *= size;
                            Dust.NewDustPerfect(position, dustType, new Vector2((float)dustX, (float)dustY).RotatedBy(rotation), Alpha, (Color)color, dustSize).noGravity = noGravity;
                        }
                    }
                }
            }
        }
        public static void AddBuff<T>(this NPC npc, int time, bool quiet = true) where T : ModBuff
        {
            npc.AddBuff(ModContent.BuffType<T>(), time, quiet);
        }
        public static void AddBuff<T>(this Player player, int time, bool quiet = true, bool foodHack = false) where T : ModBuff
        {
            player.AddBuff(ModContent.BuffType<T>(), time, quiet, foodHack);
        }
        public static void CircleDust(Vector2 position, int repeats, int dustType, float speed = 3f, int alpha = 0, Color? dustColor = null, float scale = 1f, bool noGravity = true)
        {
            for (int i = 0; i < repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)repeats;
                Dust.NewDustPerfect(position, dustType, Utils.ToRotationVector2(angle2) * speed, 0, dustColor is null ? default : (Color)dustColor, scale).noGravity = noGravity;
            }
        }
        public static void CircleDust(this Entity entity, int repeats, int dustType, float speed = 3f, int alpha = 0, Color? dustColor = null, float scale = 1f, bool noGravity = true)
        {
            for (int i = 0; i < repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)repeats;
                Dust.NewDustPerfect(entity.Center, dustType, Utils.ToRotationVector2(angle2) * speed, 0, dustColor is null ? default : (Color)dustColor, scale).noGravity = noGravity;
            }
        }
        public static string TooltipHotkeyString(this ModKeybind mhk)
        {
            if (Main.dedServ || mhk == null)
            {
                return "";
            }
            List<string> keys = mhk.GetAssignedKeys(0);
            if (keys.Count == 0)
            {
                return "[NONE]";
            }
            StringBuilder sb = new StringBuilder(16);
            sb.Append(keys[0]);
            for (int i = 1; i < keys.Count; i++)
            {
                sb.Append(" / ").Append(keys[i]);
            }
            return sb.ToString();
        }

        public static int ownedProjectileCounts<T>(this Player player) where T : ModProjectile
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<T>()];
        }
        public static HymenoptraDamageClass BeeDamageClass()
        {
            return ModContent.GetInstance<HymenoptraDamageClass>();
        }
        public static BombusApisBeeGlobalItem Bombus(this Item item)
        {
            return item.GetGlobalItem<BombusApisBeeGlobalItem>();
        }
        public static BombusApisBeePlayer Bombus(this Player player)
        {
            return player.GetModPlayer<BombusApisBeePlayer>();
        }
        public static BeeDamagePlayer Hymenoptra(this Player player)
        {
            return player.GetModPlayer<BeeDamagePlayer>();
        }
        public static BombusApisBeeGlobalProjectile Bombus(this Projectile proj)
        {
            return proj.GetGlobalProjectile<BombusApisBeeGlobalProjectile>();
        }
        public static void IncreaseBeeDamage(this Player player, float damage)
        {
            player.GetDamage<HymenoptraDamageClass>() += damage;
        }
        public static void IncreaseBeeCrit(this Player player, float crit)
        {
            player.GetCritChance<HymenoptraDamageClass>() += crit;
        }
        public static void IncreaseBeeKnockback(this Player player, float knockback)
        {
            player.GetKnockback<HymenoptraDamageClass>() += knockback;
        }
        public static void IncreaseBeeUseSpeed(this Player player, float speed)
        {
            player.GetAttackSpeed<HymenoptraDamageClass>() += speed;
        }
        public enum EasingType
        {
            Linear,
            SineIn,
            SineOut,
            SineInOut,
            SineBump,
            PolyIn,
            PolyOut,
            PolyInOut,
            ExpIn,
            ExpOut,
            ExpInOut,
            CircIn,
            CircOut,
            CircInOut
        }

        public struct CurveSegment
        {
            public CurveSegment(EasingType MODE, float ORGX, float ORGY, float DISP, int DEG = 1)
            {
                mode = MODE;
                originX = ORGX;
                originY = ORGY;
                displacement = DISP;
                degree = DEG;
            }
            public EasingType mode;
            public float originX;
            public float originY;
            public float displacement;
            public int degree;
        }
        public static void SendPacket(this Player player, ModPacket packet, bool server)
        {
            if (!server)
            {
                packet.Send(-1, -1);
                return;
            }
            packet.Send(-1, player.whoAmI);
        }
        public static float PiecewiseAnimation(float progress, CurveSegment[] segments)
        {
            if (segments.Length == 0)
            {
                return 0f;
            }
            if (segments[0].originX != 0f)
            {
                segments[0].originX = 0f;
            }
            progress = MathHelper.Clamp(progress, 0f, 1f);
            float ratio = 0f;
            for (int i = 0; i <= segments.Length - 1; i++)
            {
                CurveSegment segment = segments[i];
                float startPoint = segment.originX;
                float endPoint = 1f;
                if (progress >= segment.originX)
                {
                    if (i < segments.Length - 1)
                    {
                        if (segments[i + 1].originX <= progress)
                        {
                            goto IL_454;
                        }
                        endPoint = segments[i + 1].originX;
                    }
                    float segmentLenght = endPoint - startPoint;
                    float segmentProgress = (progress - segment.originX) / segmentLenght;
                    ratio = segment.originY;
                    switch (segment.mode)
                    {
                        case EasingType.Linear:
                            return ratio + segmentProgress * segment.displacement;
                        case EasingType.SineIn:
                            return ratio + (1f - (float)Math.Cos((double)(segmentProgress * 3.1415927f / 2f))) * segment.displacement;
                        case EasingType.SineOut:
                            return ratio + (float)Math.Sin((double)(segmentProgress * 3.1415927f / 2f)) * segment.displacement;
                        case EasingType.SineInOut:
                            return ratio + -((float)Math.Cos((double)(segmentProgress * 3.1415927f)) - 1f) / 2f * segment.displacement;
                        case EasingType.SineBump:
                            return ratio + (float)Math.Sin((double)(segmentProgress * 3.1415927f)) * segment.displacement;
                        case EasingType.PolyIn:
                            return ratio + (float)Math.Pow((double)segmentProgress, (double)segment.degree) * segment.displacement;
                        case EasingType.PolyOut:
                            return ratio + (1f - (float)Math.Pow((double)(1f - segmentProgress), (double)segment.degree)) * segment.displacement;
                        case EasingType.PolyInOut:
                            return ratio + ((segmentProgress < 0.5f) ? ((float)Math.Pow(2.0, (double)(segment.degree - 1)) * (float)Math.Pow((double)segmentProgress, (double)segment.degree)) : (1f - (float)Math.Pow((double)(-2f * segmentProgress + 2f), (double)segment.degree) / 2f)) * segment.displacement;
                        case EasingType.ExpIn:
                            return ratio + ((segmentProgress == 0f) ? 0f : ((float)Math.Pow(2.0, (double)(10f * segmentProgress - 10f)))) * segment.displacement;
                        case EasingType.ExpOut:
                            return ratio + ((segmentProgress == 1f) ? 1f : (1f - (float)Math.Pow(2.0, (double)(-10f * segmentProgress)))) * segment.displacement;
                        case EasingType.ExpInOut:
                            return ratio + ((segmentProgress == 0f) ? 0f : ((segmentProgress == 1f) ? 1f : ((segmentProgress < 0.5f) ? ((float)Math.Pow(2.0, (double)(20f * segmentProgress - 10f)) / 2f) : ((2f - (float)Math.Pow(2.0, (double)(-20f * segmentProgress - 10f))) / 2f)))) * segment.displacement;
                        case EasingType.CircIn:
                            return ratio + (1f - (float)Math.Sqrt(1.0 - Math.Pow((double)segmentProgress, 2.0))) * segment.displacement;
                        case EasingType.CircOut:
                            return ratio + (float)Math.Sqrt(1.0 - Math.Pow((double)(segmentProgress - 1f), 2.0)) * segment.displacement;
                        case EasingType.CircInOut:
                            return ratio + (((double)segmentProgress < 0.5) ? ((1f - (float)Math.Sqrt(1.0 - Math.Pow((double)(2f * segmentProgress), 2.0))) / 2f) : (((float)Math.Sqrt(1.0 - Math.Pow((double)(-2f * segmentProgress - 2f), 2.0)) + 1f) / 2f)) * segment.displacement;
                        default:
                            return ratio;
                    }
                }
            IL_454:;
            }
            return ratio;
        }
        public static Item GetActiveItem(this Player player)
        {
            if (!Main.mouseItem.IsAir)
            {
                return Main.mouseItem;
            }
            return player.HeldItem;
        }
        public static float GetBeeItemResourceChance(this Player player)
        {
            if (player.GetActiveItem().ModItem is BeeDamageItem damageItem && damageItem.ResourceChance > 0f)
            {
                float Chance = damageItem.ResourceChance;
                return Chance;
            }
            return 0f;
        }
        public static float TrueResourceChance(this Player player)
        {
            return Utils.Clamp((player.GetBeeItemResourceChance() + player.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd), 0f, 0.5f);
        }
        public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
        {
            if (fallback == null)
            {
                fallback = new Vector2?(Vector2.Zero);
            }
            return Utils.SafeNormalize(destination - entity.Center, fallback.Value);
        }
        public static void UseBeeResource(this Player player, int amount)
        {
            if (Main.rand.NextFloat() > player.TrueResourceChance())
            {
                player.Hymenoptra().BeeResourceRegenTimer = -120;
                player.Hymenoptra().BeeResourceCurrent -= amount;
            }
        }
        public static float AsRadians(this float amount)
        {
            return MathHelper.ToRadians(amount);
        }

        public static float AsRadians(this int amount)
        {
            return (float)MathHelper.ToRadians(amount);
        }

        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);
        /// <summary>
        /// fancy slr utility
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="front"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }

        public static void CleanHoldStyle(Player player, float desiredRotation, Vector2 desiredPosition, Vector2 spriteSize, Vector2? rotationOriginFromCenter = null, bool noSandstorm = false, bool flipAngle = false, bool stepDisplace = true)
        {
            if (noSandstorm)
                player.sandStorm = false;

            if (rotationOriginFromCenter == null)
                rotationOriginFromCenter = new Vector2?(Vector2.Zero);

            Vector2 origin = rotationOriginFromCenter.Value;
            origin.X *= player.direction;
            origin.Y *= player.gravDir;
            player.itemRotation = desiredRotation;

            if (flipAngle)
                player.itemRotation *= player.direction;
            else if (player.direction < 0)
                player.itemRotation += 3.1415927f;

            Vector2 consistentAnchor = player.itemRotation.ToRotationVector2() * (spriteSize.X / -2f - 10f) * player.direction - origin.RotatedBy(player.itemRotation, default(Vector2));
            Vector2 offsetAgain = spriteSize * -0.5f;
            Vector2 finalPosition = desiredPosition + offsetAgain + consistentAnchor;
            if (stepDisplace)
            {
                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                if ((frame > 6 && frame < 10) || (frame > 13 && frame < 17))
                    finalPosition -= Vector2.UnitY * 2f;
            }
            player.itemLocation = finalPosition;
        }

        public static int ApplyHymenoptraDamageTo(this Player player, int damage) => (int)player.GetTotalDamage<HymenoptraDamageClass>().ApplyTo(damage);

        public static float ApplyHymenoptraSpeedTo(this Player player, int input) => input * (1f - (player.GetTotalAttackSpeed<HymenoptraDamageClass>() - 1f));

        public static SlotId PlayWith(this SoundStyle sound, Vector2? pos = null, float pitch = 0f, float pitchVariance = 0f, float volume = 1f)
        {
            return SoundEngine.PlaySound(sound with { Pitch = pitch, PitchVariance = pitchVariance, Volume = volume }, pos);
        }

        public static Color MulticolorLerp(float increment, params Color[] colors)
        {
            increment %= 0.999f;
            int currentColorIndex = (int)(increment * (float)colors.Length);
            Color color = colors[currentColorIndex];
            Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
            return Color.Lerp(color, nextColor, increment * (float)colors.Length % 1f);
        }
    }
}
