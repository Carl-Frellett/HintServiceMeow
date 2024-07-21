using HarmonyLib;
using Hints;
using System;
using Mirror;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace HintServiceMeow
{
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    internal static class HintPatch
    {
        static bool Prefix(Hints.Hint hint, ref HintDisplay __instance)
        {
            try
            {
                Player player = Player.Get(__instance.gameObject);

                if (player == null)
                {
                    return false;
                }

                var playerDisplay = PlayerDisplay.Get(player);

                return playerDisplay.AllowPatchUpdate;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("ShowHint", typeof(string), typeof(float))]
    internal static class HintPatch2
    {
        static bool Prefix(string message, float duration, ref Player __instance)
        {
            try
            {
                ReferenceHub hub = __instance.ReferenceHub;

                if (hub?.characterClassManager?.connectionToClient == null)
                {
                    return true;
                }

                NetworkConnection connection = hub.characterClassManager.connectionToClient;

                TextHint textHint = new TextHint(message, new[] { new StringHintParameter(message) }, null, duration);
                connection.Send(new HintMessage(textHint));

                return false;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}
