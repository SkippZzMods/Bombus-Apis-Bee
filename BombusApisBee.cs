global using BombusApisBee.BeeDamageClass;
global using BombusApisBee.Buffs;
global using BombusApisBee.Core;
global using BombusApisBee.Dusts;
global using BombusApisBee.Effects;
global using BombusApisBee.Items.Accessories.BeeKeeperDamageClass;
global using BombusApisBee.Items.Armor.BeeKeeperDamageClass;
global using BombusApisBee.Items.Weapons.BeeKeeperDamageClass;
global using BombusApisBee.PrimitiveDrawing;
global using BombusApisBee.Projectiles;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.Graphics.Effects;
global using Terraria.ID;
global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using BombusApisBee.Core.PixelationSystem;
using BombusApisBee.UI;
using ReLogic.Content;
using System.Reflection;
using Terraria.UI;

namespace BombusApisBee
{
    public class BombusApisBee : Mod
    {
        public static Color honeyIncreaseColor = new Color(255, 226, 43);

        public static readonly SoundStyle HoneycombWeapon = SoundID.NPCDeath1 with { Pitch = -0.2f, PitchVariance = 0.2f };
        public static string BeeWeapon = "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/";
        public static string Invisible = "BombusApisBee/ExtraTextures/Invisible";
        public static string Palettes = "BombusApisBee/ExtraTextures/Palettes/";

        public static ModKeybind HoneyManipulatorHotkey;
        public static ModKeybind LihzardianRelicHotkey;
        public static ModKeybind BeekeeperStateSwitchHotkey;

        public Asset<Texture2D> stinger;
        public Asset<Texture2D> bee;
        public Asset<Texture2D> giantBee;
        public Asset<Texture2D> wasp;
        public Asset<Texture2D> beeArrow;

        public UserInterface BeeDamageInterface;

        public BeePlayerUI BeePlayerUI;

        private List<IOrderedLoadable> loadCache;

        public static void SetLoadingText(string text)
        {
            FieldInfo Interface_loadMods = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)!;
            MethodInfo UIProgress_set_SubProgressText = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIProgress")!.GetProperty("SubProgressText", BindingFlags.Public | BindingFlags.Instance)!.GetSetMethod()!;

            UIProgress_set_SubProgressText.Invoke(Interface_loadMods.GetValue(null), new object[] { text });
        }

        public override void PostSetupContent()
        {
            //ModContent.GetInstance<PixellateSystem>().RegisterRenderTarget("Projectiles");
            //ModContent.GetInstance<PixellateSystem>().RegisterRenderTarget("Dusts", RenderType.Dust);

            ModContent.GetInstance<PixelationSystem>().RegisterScreenTarget("Projectiles");
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BeeShaders.Load();
                //MarkedNPCDrawer.Load();
                PlayerRenderTarget.Load();
                BombusApisBee_DoIL.Load();
                BombusApisBee_DoDetours.Load();

                BeeDamageInterface = new UserInterface();

                BeePlayerUI = new BeePlayerUI();
                BeePlayerUI.Activate(); 
                BeeDamageInterface.SetState(BeePlayerUI);
            }

            DrawPrimitives.Load();

            HoneyManipulatorHotkey = KeybindLoader.RegisterKeybind(this, "Honey Manipulation", "Y");

            LihzardianRelicHotkey = KeybindLoader.RegisterKeybind(this, "Lihzardian Hornet Relic", "L");

            BeekeeperStateSwitchHotkey = KeybindLoader.RegisterKeybind(this, "Change Beekeeper State", "N");


            loadCache = new List<IOrderedLoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    object instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as IOrderedLoadable);
                }

                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
                SetLoadingText("Loading " + loadCache[k].GetType().Name);
            }
        }

        public override void Unload()
        {
            //MarkedNPCDrawer.Unload();
            BombusApisBee_DoIL.Unload();
            BombusApisBee_DoDetours.Unload();
            HoneyManipulatorHotkey = null;

            LihzardianRelicHotkey = null;

            if (loadCache != null)
            {
                foreach (IOrderedLoadable loadable in loadCache)
                {
                    loadable.Unload();
                }

                loadCache = null;
            }
            else
            {
                Logger.Warn("load cache was null, IOrderedLoadable's may not have been unloaded...");
            }
        }
    }
}