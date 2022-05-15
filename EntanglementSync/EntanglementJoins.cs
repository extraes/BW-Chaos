using ModThatIsNotMod;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class EntanglementJoins : EffectBase
    {
        public EntanglementJoins() : base("Popularity Contest", 120, EffectTypes.HIDDEN) { }

        readonly string[] names =
        {
            "adamdev",
            "the adamdev entity",
            "Camobiwan",
            "Camobiwon",
            "extraes",
            "Jack Frost",
            "kasploingus",
            "Lakatrazz",
            "ZCubed",
            "Mr. Potato",
        };
        readonly string[] templates =
        {
            "{0} has joined the server!",
            "{0} has joined the server!",
            "Joined {0}'s server!",
            "You left the server.",
        };
        const float minTime = 4;
        static readonly float maxExtraTime = 10;

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;

            while (Active)
            {
                string template = templates.Random();
                string name = names.Random();
                string formatted = string.Format(template, name);

                // no SendNetworkData because its funnier/better if everyone experiences different things
                Notifications.SendNotification(formatted, 4);

                yield return new WaitForSeconds(minTime + Random.value * maxExtraTime);
            }
        }
    }
}
