namespace BWChaos.Effects
{
    internal class SpawnDogAd : EffectBase
    {
        public SpawnDogAd() : base("Spawn Dog Ad") { }

        public override void OnEffectStart()
        {
            // todo: stress test this
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
        }
    }
}
