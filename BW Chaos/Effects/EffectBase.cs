namespace BW_Chaos.Effects
{
    // todo: maybe add a "conflicting effects" list variable in case of something such as 2 effects modifying gravity
    internal class EffectBase
    {
        public string Name;
        public int Duration;

        public EffectBase(string eName, int eDuration)
        {
            Name = eName;
            Duration = eDuration;
        }

        public EffectBase(string eName)
        {
            Name = eName;
            Duration = 0;
        }

        public virtual void OnEffectStart() { }
        public virtual void OnEffectUpdate() { }
        public virtual void OnEffectEnd() { }
    }
}
