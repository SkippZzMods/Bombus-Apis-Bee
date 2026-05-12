namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Desert
{
    public class ChippedTailspike : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Causes spikes to grow on your back over time\nGetting hit from behind breaks the spikes, reflecting damage back at the attacker\nOnce spikes have fully grown, getting hit reflects 500% of the damage back to the attacker, and grants a buff to beekeeper critical strike chance for a short time");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 40);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ChippedTailspikePlayer>().equipped = true;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ChippedTailspikePlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public const int MAX_GROW_TIMER = 300;

        public bool equipped;

        public int growTimer;
        public int drawTimer;
        public bool FullyGrown => spikes.Count >= 5 && spikes.All(s => s.lifeTime > s.growTime);

        public List<ChippedTailspikeSpike> spikes = new();

        public override void Load()
        {
            On_Main.DrawPlayers_AfterProjectiles += DrawSpikes;
        }

        private void DrawSpikes(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead)
                {
                    var mp = player.GetModPlayer<ChippedTailspikePlayer>();
                    if (mp.equipped)
                    {
                        SpriteBatch sb = Main.spriteBatch;
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                        foreach (ChippedTailspikeSpike spike in mp.spikes)
                        {
                            spike.Draw(sb);
                        }

                        sb.End();
                    }
                }
            }

            orig(self);
        }

        public override void ResetEffects()
        {
            List<ChippedTailspikeSpike> spikesToKill = new();

            foreach (ChippedTailspikeSpike spike in spikes)
            {
                spike.Update();
                if (!spike.active)
                    spikesToKill.Add(spike);
            }

            foreach (ChippedTailspikeSpike spike in spikesToKill)
            {
                spikes.Remove(spike);
            }

            equipped = false;
        }

        public override void UpdateEquips()
        {
            if (growTimer < MAX_GROW_TIMER && equipped)
                growTimer++;

            if (!equipped)
            {
                growTimer = 0;

                List<ChippedTailspikeSpike> spikesToKill = [.. spikes];

                foreach (var spike in spikesToKill)
                {
                    spike.Kill();
                }
            }
            else
            {
                drawTimer++;

                if (growTimer % (MAX_GROW_TIMER / 5) == 0 && spikes.Count < 5)
                {
                    float rot = 0.45f * (spikes.Count - 2);

                    float offset = -20f + -4f * (spikes.Count - 2);
                    if (spikes.Count - 2 > 0)
                        offset = -20f + 4f * (spikes.Count - 2);

                    spikes.Add(new ChippedTailspikeSpike(60, spikes.Count, Player.whoAmI, rot, offset));
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (equipped)
            {
                if (Player.direction == 1 ? npc.Center.X < Player.Center.X : npc.Center.X > Player.Center.X)
                {
                    if (FullyGrown)
                        Player.AddBuff<ChippedTailspikeBuff>(600);

                    float lerper = growTimer / (float)MAX_GROW_TIMER;

                    npc.SimpleStrikeNPC((int)(hurtInfo.SourceDamage * MathHelper.Lerp(1f, 5f, lerper)), -Player.direction, false, 5f);

                    growTimer = 0;

                    foreach (ChippedTailspikeSpike spike in spikes)
                    {
                        spike.HitEffect();
                        spike.dying = true;
                    }

                    Player.Bombus().AddShake((int)MathHelper.Lerp(3f, 12f, lerper));

                    for (int i = 0; i < spikes.Count; i++)
                    {
                        Dust.NewDustPerfect(Player.Center, DustType<ImpactLineDust>(), new Vector2(-10f * Player.direction, 0f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1.5f),
                            50, new Color(205, 50, 45, 0), 0.15f);

                        Dust.NewDustPerfect(Player.Center, DustType<ImpactLineDust>(), new Vector2(-10f * Player.direction, 0f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1.5f),
                            50, new Color(170, 125, 85, 0), 0.15f);
                    }

                    for (int i = 0; i < spikes.Count * 3; i++)
                    {
                        Dust.NewDustPerfect(Player.Center, DustType<GoreBloodDark>(), new Vector2(-5f * Player.direction, 0f).RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 1.5f), Main.rand.Next(100),
                            default, 2f);
                    }

                    new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Player.Center);
                }
            }
        }
    }

    public class ChippedTailspikeSpike
    {
        public bool active;
        public bool flashed;
        public int flashTimer;
        public int growTime;
        public int index;
        public int ownerWhoAmI;
        public float rotOffset;
        public float distOffset;

        internal bool dying;
        internal int killTime;
        internal int lifeTime;
        internal float rotation;

        internal Player Owner => Main.player[ownerWhoAmI];

        public ChippedTailspikeSpike(int growTime, int index, int ownerWhoAmI, float rotOffset, float distOffset)
        {
            active = true;

            this.growTime = growTime;
            this.index = index;
            this.ownerWhoAmI = ownerWhoAmI;
            this.rotOffset = rotOffset;
            this.distOffset = distOffset;
        }

        public void Update()
        {
            var mp = Owner.GetModPlayer<ChippedTailspikePlayer>();

            if (dying)
            {
                killTime++;
                if (killTime > 14)
                    Kill();
            }

            if (flashTimer > 0)
                flashTimer--;

            lifeTime++;

            if (mp.FullyGrown && !flashed)
            {
                flashTimer = 20;
                flashed = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D tex = Request<Texture2D>("BombusApisBee/Content/Crossmod/Calamity/Items/Accessories/Desert/ChippedTailspike_Spike").Value;
            Texture2D texBlur = Request<Texture2D>("BombusApisBee/Content/Crossmod/Calamity/Items/Accessories/Desert/ChippedTailspike_Spike_Blur").Value;
            Texture2D texGlow = Request<Texture2D>("BombusApisBee/Content/Crossmod/Calamity/Items/Accessories/Desert/ChippedTailspike_Spike_Glow").Value;

            var mp = Owner.GetModPlayer<ChippedTailspikePlayer>();

            float lerper;
            if (lifeTime < growTime)
                lerper = EaseFunction.EaseBackOut.Ease(lifeTime / (float)growTime);
            else
                lerper = 1f;

            float fadeOut = 1f;
            if (killTime > 0)
                fadeOut = EaseFunction.EaseQuarticOut.Ease(1f - killTime / 14f);

            float scaleOut = 1f;
            if (killTime > 0)
                scaleOut = 1f + 0.2f * EaseFunction.EaseQuarticOut.Ease(1f - killTime / 14f);

            float sin = (float)Math.Sin(mp.drawTimer * 0.0175f);
            if (sin < 0f)
                sin *= -1f;

            Vector2 offset = new Vector2(distOffset * lerper * Owner.direction + 4.5f * EaseFunction.EaseCircularInOut.Ease(sin) * -Owner.direction, 0f).RotatedBy(rotOffset * Owner.direction);

            if (killTime > 0)
                offset = Vector2.Lerp(offset, new Vector2(distOffset * 2.85f * Owner.direction, 0f).RotatedBy(rotOffset * Owner.direction), EaseFunction.EaseBackOut.Ease(killTime / 14f));

            SpriteEffects flip = Owner.direction == 1 ? SpriteEffects.FlipHorizontally : 0f;

            Vector2 pos = Owner.Center + offset + new Vector2(0f, Owner.gfxOffY) - Main.screenPosition;

            float rot = Owner.Center.DirectionTo(Owner.Center + offset).ToRotation() + MathHelper.PiOver2;

            sb.Draw(texBlur, pos, null, Color.Lerp(new Color(255, 170, 70, 0), new Color(115, 50, 45, 0), flashTimer / 20f) * (mp.growTimer / (float)ChippedTailspikePlayer.MAX_GROW_TIMER) * fadeOut,
                rot, texBlur.Size() / 2f, 1f * scaleOut * lerper, flip, 0f);

            sb.Draw(tex, pos, null, Color.White * fadeOut, rot, tex.Size() / 2f, 1f * scaleOut * lerper, flip, 0f);

            sb.Draw(texBlur, pos, null, Color.White with { A = 0 } * (1f - lerper) * fadeOut, rot, texBlur.Size() / 2f, 1.5f * scaleOut * lerper, flip, 0f);

            if (flashTimer > 0)
                sb.Draw(texGlow, pos, null, new Color(115, 50, 45, 0) * (flashTimer / 20f) * fadeOut, rot, texBlur.Size() / 2f, 1f * scaleOut, flip, 0f);
        }

        public void Kill()
        {
            active = false;
        }

        public void HitEffect()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedBy(rotOffset * Owner.direction + (Owner.direction == 1 ? MathHelper.Pi : 0f));

                Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(10f, 10f), DustType<GlowFastDecelerate>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(10f), 0, Main.rand.NextBool() ? new Color(185, 160, 125) : new Color(155, 50, 45), 0.35f);
            }
        }
    }
}
