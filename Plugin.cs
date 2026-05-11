using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MirrorShadeBugFix
{
    [BepInPlugin("com.mirrorshadebugfix", "Mirror & Shade Bug Fix", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("MirrorShadeBugFix loaded — will re-register mirrors/shades after save load.");
            new Harmony("com.mirrorshadebugfix").PatchAll();
        }
    }
}
