//Credits to Starlight River

namespace BombusApisBee.Dusts
{
    public class ExplosionDust : ModDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 80f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Orange, gray, (dust.alpha - 80) / 80f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;
            if (dust.alpha > 100)
            {
                dust.scale += 0.01f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }
            dust.position += dust.velocity;
            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class ExplosionDustTwo : ModDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 80f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Orange, gray, (dust.alpha - 80) / 80f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }
            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class ExplosionDustThree : ModDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 40)
                ret = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 40f);
            else if (dust.alpha < 80)
                ret = Color.Lerp(Color.Orange, gray, (dust.alpha - 40) / 40f);
            else
                ret = gray;

            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 60)
            {
                dust.scale += 0.01f;
                dust.alpha += 6;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class ExplosionDustFour : ModDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 80f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Orange, gray, (dust.alpha - 80) / 80f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (Math.Abs(dust.velocity.X) > 7)
                dust.velocity.X *= 0.85f;
            else
                dust.velocity.X *= 0.92f;

            if (dust.velocity.Y > -2)
                dust.velocity.Y -= 0.1f;

            if (dust.velocity.Y > 3.5f)
                dust.velocity.Y = 3.5f;

            else
                dust.velocity.Y *= 0.92f;

            if (dust.alpha > 100)
            {
                dust.scale += 0.01f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }
            dust.position += dust.velocity;
            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}