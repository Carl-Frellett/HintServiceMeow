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
                // 获取与当前 HintDisplay 关联的玩家
                Player player = Player.Get(__instance.gameObject);

                if (player == null)
                {
                    return false;
                }

                // 根据玩家的一些条件来控制是否显示提示
                var playerDisplay = PlayerDisplay.Get(player);

                return playerDisplay.AllowPatchUpdate; // 返回允许或禁止显示提示的信息。
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
                // 获取 ReferenceHub 对象（包含有关该玩家的信息）
                ReferenceHub hub = __instance.ReferenceHub;

                if (hub?.characterClassManager?.connectionToClient == null)
                {
                    return true;
                }

                NetworkConnection connection = hub.characterClassManager.connectionToClient;

                // 构建 HintMessage 和 TextHint，并发送给客户端。
                TextHint textHint = new TextHint(message, new[] { new StringHintParameter(message) }, null, duration);
                connection.Send(new HintMessage(textHint));

                return false; // 返回 false 来阻止 ShowHint 方法执行默认行为。
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}
