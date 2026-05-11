using Game.ObjectInfoDataScripts;
using Game.ObjectInfoDataScripts.CustomFacilitiesAndModules;
using HarmonyLib;

namespace MirrorShadeBugFix
{
    // After a save is loaded, SpaceMirrorOrShadeFacility instances are deserialized from JSON.
    // The load loop calls StackAndSetup(0, increaseEnabled: false) which never triggers the
    // Enabled setter, so AllocateMirrorsAcrossTargets() is never called. As a result, the
    // transient ObjectInfo.SpaceMirrorsAndShadesTargetingThisObject HashSet stays empty and
    // UpdateTemperature() computes mirrorsStrength = 0 and shadesStrength = 0 for all bodies.
    //
    // Fix: postfix-patch Facility.OnAfterLoadSave(). When the facility is a
    // SpaceMirrorOrShadeFacility with at least one enabled unit, call
    // AllocateMirrorsAcrossTargets(allocateExcess: false) to re-register it in the
    // SpaceMirrorsAndShadesTargetingThisObject sets of all its target bodies, using the
    // saved allocation without redistribution.
    [HarmonyPatch(typeof(Facility), nameof(Facility.OnAfterLoadSave))]
    public static class Patch_Facility_OnAfterLoadSave
    {
        static void Postfix(Facility __instance)
        {
            SpaceMirrorOrShadeFacility mirror = __instance as SpaceMirrorOrShadeFacility;
            if (mirror == null)
                return;

            if (mirror.Enabled <= 0)
                return;

            Plugin.Log.LogDebug(
                $"[MirrorShadeBugFix] Re-registering {(mirror.IsMirror() ? "mirror" : "shade")} " +
                $"'{mirror.facilityDescriptor?.ID}' on '{mirror.ObjectInfoData?.ObjectInfo?.ObjectName}' " +
                $"(enabled={mirror.Enabled}, targets={mirror.Targets.Count}).");

            mirror.AllocateMirrorsAcrossTargets(allocateExcess: false);
        }
    }
}
