using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Reflection;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.Core.TmodFile;

namespace BombusApisBee.Effects
{
    public class BeeShaders
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var file = (TmodFile)info.Invoke(ModContent.GetInstance<BombusApisBee>(), null);

            System.Collections.Generic.IEnumerable<FileEntry> shaders = file.Where(n => n.Name.StartsWith("Effects/") && n.Name.EndsWith(".xnb") && !n.Name.Contains("IGNORE"));

            foreach (FileEntry entry in shaders)
            {
                string name = entry.Name.Replace(".xnb", "").Replace("Effects/", "");
                string path = entry.Name.Replace(".xnb", "");
                LoadShader(name, path);
            }
        }

        public static void LoadShader(string name, string path)
        {
            var screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene[name] = new Filter(new ScreenShaderData(screenRef, screenRef.Value.CurrentTechnique.Passes[0].Name), EffectPriority.High);
            Filters.Scene[name].Load();
        }
    }
}
