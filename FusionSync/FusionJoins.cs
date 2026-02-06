using Jevil;
using BoneLib;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using LabFusion.Utilities;

namespace BLChaos.Effects;

internal class FusionJoins : EffectBase
{
    public FusionJoins() : base("Popularity Contest", 120, EffectTypes.HIDDEN) { }

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
        "Mr. Potato",
        "BamBaeYoh", // idk why i namedropped bam, just the first person that came to mind when i thought of a bl content creator
    };
    readonly string[] templates =
    {
        "{0} has joined the server!",
        "{0} has joined the server!",
        "{0} has left the server.",
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
            FusionNotification notif = new()
            {
                popupLength = FusionNotifier.DefaultDuration,
                type = NotificationType.SUCCESS,
                title = formatted,//todo make it look like a join message
            };
            FusionNotifier.Send(notif);

            yield return new WaitForSeconds(minTime + Random.value * maxExtraTime);
        }
    }
}
