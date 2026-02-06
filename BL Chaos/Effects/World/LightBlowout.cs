namespace BLChaos.Effects;

internal class LightBlowout : EffectBase
{
    public LightBlowout() : base("LED Headlight simulator", 90) { }

    List<Light> lights = new(18); // 18 realtime lights. Lol Mode.

    public override void OnEffectStart()
    {
        // Unfortunately, not a lot of lights in BW are realtime.
        foreach (Light light in GameObject.FindObjectsOfType<Light>())
        {
            if (light.type == LightType.Area) continue;
            else if (light.bakingOutput.lightmapBakeType != LightmapBakeType.Realtime)

            lights.Add(light);

            light.intensity *= 100;
        }
    }

    public override void OnEffectEnd()
    {
        foreach (Light light in lights)
        {
            light.intensity /= 1000;
        }
    }
}
