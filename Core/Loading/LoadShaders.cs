using System.Reflection;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.Core.TmodFile;

namespace BombusApisBee.Core.Loading
{
    public class LoadShaders
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var file = (TmodFile)info.Invoke(GetInstance<BombusApisBee>(), null);

            IEnumerable<FileEntry> shaders = file.Where(n => n.Name.StartsWith("Assets/Shaders/") && n.Name.EndsWith(".xnb") && !n.Name.Contains("IGNORE"));

            foreach (FileEntry entry in shaders)
            {
                string name = entry.Name.Replace(".xnb", "").Replace("Assets/Shaders/", "");
                string path = entry.Name.Replace(".xnb", "");
                LoadShader(name, path);
            }
        }

        public static void LoadShader(string name, string path)
        {
            var shaderAsset = GetInstance<BombusApisBee>().Assets.Request<Effect>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            string passName = shaderAsset.Value.CurrentTechnique.Passes[0].Name;

            Filters.Scene[name] = new Filter(new ScreenShaderData(shaderAsset, passName), EffectPriority.High);
            Filters.Scene[name].Load();
        }
    }
}
