global using BombusApisBee.Core;
global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.ID;
global using Terraria.ModLoader;

using BombusApisBee.Effects;
using BombusApisBee.Items.Armor.BeeKeeperDamageClass;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace BombusApisBee
{
    public class BombusApisBee : Mod
    {
        public static readonly SoundStyle HoneycombWeapon = SoundID.NPCDeath1 with { Pitch = -0.2f, PitchVariance = 0.2f };
        public static string BeeWeapon = "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/";
        public static string Invisible = "BombusApisBee/ExtraTextures/Invisible";
        public static ModKeybind HoneyManipulatorHotkey;

        public Asset<Texture2D> stinger;
        public Asset<Texture2D> bee;
        public Asset<Texture2D> giantBee;
        public Asset<Texture2D> wasp;
        public Asset<Texture2D> beeArrow;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                BeeShaders.Load();
                MarkedNPCDrawer.Load();
                PlayerRenderTarget.Load();
            }

            DrawPrimitives.Load();
            stinger = TextureAssets.Projectile[ProjectileID.HornetStinger];
            bee = TextureAssets.Projectile[ProjectileID.Bee];
            giantBee = TextureAssets.Projectile[ProjectileID.GiantBee];
            wasp = TextureAssets.Projectile[ProjectileID.Wasp];
            beeArrow = TextureAssets.Projectile[ProjectileID.BeeArrow];
            TextureAssets.Projectile[ProjectileID.HornetStinger] = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StingerRetexture");
            TextureAssets.Projectile[ProjectileID.Bee] = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeRetexture");
            TextureAssets.Projectile[ProjectileID.GiantBee] = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GiantBeeRetexture");
            TextureAssets.Projectile[ProjectileID.Wasp] = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/WaspRetexture");
            TextureAssets.Projectile[ProjectileID.BeeArrow] = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeArrowRetexture");

            HoneyManipulatorHotkey = KeybindLoader.RegisterKeybind(this, "Honey Manipulation", "Y");
        }

        public override void Unload()
        {
            MarkedNPCDrawer.Unload();
            TextureAssets.Projectile[ProjectileID.HornetStinger] = stinger;
            TextureAssets.Projectile[ProjectileID.Bee] = bee;
            TextureAssets.Projectile[ProjectileID.GiantBee] = giantBee;
            TextureAssets.Projectile[ProjectileID.Wasp] = wasp;
            TextureAssets.Projectile[ProjectileID.BeeArrow] = beeArrow;
            HoneyManipulatorHotkey = null;
        }
    }
}