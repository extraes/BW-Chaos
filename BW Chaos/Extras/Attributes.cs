using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWChaos.Extras
{
    // Attribute that EffectBase uses to get IEnumerators to run at the start of an effect and end at the end.
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AutoCoroutine : Attribute { }
}
