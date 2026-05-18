using Elin.Plugin.Main.Models.Impl;
using HarmonyLib;

namespace Elin.Plugin.Main.Patches
{
    [HarmonyPatch(typeof(DramaCustomSequence))]
    internal static class DramaCustomSequencePatch
    {
        #region function

        [HarmonyPatch(nameof(DramaCustomSequence.Build), new[] { typeof(Chara) })]
        [HarmonyPrefix]
        public static bool BuildPrefix(DramaCustomSequence __instance, Chara c)
        {
            return DramaCustomSequenceImpl.BuildPrefix(__instance, c);
        }


        [HarmonyPatch(nameof(DramaCustomSequence.Build), new[] { typeof(Chara) })]
        [HarmonyPostfix]
        public static void BuildPostfix(DramaCustomSequence __instance, Chara c)
        {
            DramaCustomSequenceImpl.BuildPostfix(__instance, c);
        }


        [HarmonyPatch(nameof(DramaCustomSequence.Choice2), new[] { typeof(string), typeof(string) })]
        [HarmonyPrefix]
        public static bool Choice2Prefix(DramaCustomSequence __instance, string lang, ref string idJump)
        {
            return DramaCustomSequenceImpl.Choice2Prefix(__instance, lang, ref idJump);
        }

        [HarmonyPatch(nameof(DramaCustomSequence.Choice), new[] { typeof(string), typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        public static bool ChoicePrefix(DramaCustomSequence __instance, string lang, string idJump, bool cancel)
        {
            return DramaCustomSequenceImpl.ChoicePrefix(__instance, lang, idJump, cancel);
        }

        [HarmonyPatch(nameof(DramaCustomSequence.Step), new[] { typeof(string) })]
        [HarmonyPostfix]
        public static void StepPostfix(DramaCustomSequence __instance, string step)
        {
            DramaCustomSequenceImpl.StepPostfix(__instance, step);
        }

        #endregion
    }
}
