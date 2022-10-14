using System;

namespace BLChaos;

// Attribute that EffectBase uses to get IEnumerators to run at the start of an effect and end at the end.
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class AutoCoroutine : Attribute { }

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class EffectPreference : Attribute
{
    public string desc = "";
    public EffectPreference() { desc = ""; }
    public EffectPreference(string description) { desc = description; }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class RangePreference : Attribute
{
    public float low;
    public float high;
    public float inc;
    public RangePreference(int lowerBound, int upperBound, int increment)
    {
        low = lowerBound;
        high = upperBound;
        inc = increment;
    }

    public RangePreference(float lowerBound, float upperBound, float increment)
    {
        low = lowerBound;
        high = upperBound;
        inc = increment;
    }
}

#if DEBUG
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DontRegisterEffect : Attribute { }
#endif
