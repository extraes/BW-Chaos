namespace BLChaos.Effects;

internal class ChangeSteamName : EffectBase
{
    // Shortened to 2m to avoid the effect running twice
    public ChangeSteamName() : base("Change Steam name for 2m", 120, EffectTypes.USE_STEAM | EffectTypes.AFFECT_STEAM_PROFILE | EffectTypes.DONT_SYNC) { }

    private static readonly string[] names = new string[] {
        "Chester Chucklenuts",
        "Your little PogChamp",
        "✨ 💓💞❤️ Deku X Bakugo ❤️💞💓 ✨",
        "Sydney|14|Bi|They/Them|BLM|ACAB",
        "lars|gay|transmasc|allosexual/poly|libra|ravenclaw",
        "xXx_DD0S_H4XX_xXx",
        "Oragani",
        "4K African",
        "Brayden|32|ladies man|4'3\"|short kings stay winning",
        "Brylan the wolf owo",
        "Brylan Bristopher|CEO|LLC Owner|$DOGE HODLer🚀|Multimillionaire|Bossman, ❌suit ❌tie",
        "xXAn0nym0usXx",
        "shoutouts to simpleflips",
        "Based, Redpilled | Dobe Johnson",
        "Hack Dudes 69",
        "Lawrence Albert Connor",
        "Big. Black. River balls.", // i dont have the pass :woeis:
        "The Holy Thighble",
        "#CryptoPunks TAP IN!!!",
        "astolfo enjoyer",
        "cs.money|darren.chungus.09",
        "edgelord mcspice",
        "'HATRED' GOTY 2015",
        "isometric sexercise",
        "i am so god damn racist i hate n",
        "heap overflow causer",
        "anime women gooner",
        "lubriderm's top customer",
        "BL Light Avatar R34 fan",
    };

    private string steamName = "helo :)";
    public override void OnEffectStart()
    {
        steamName = Steamworks.SteamFriends.GetPersonaName();
        string newName = $"{names.Random()}-BLC"; // yeah thisll definitely get cut off by how long most of the names are. oh well lol
        Steamworks.SteamFriends.
        Steamworks.SteamFriends.SetPersonaName(newName);
        Utilities.SpawnAd("So, how goes it, " + newName + "?");
    }

    public override void OnEffectEnd()
    {
        Steamworks.SteamFriends.SetPersonaName(steamName);
    }
}
