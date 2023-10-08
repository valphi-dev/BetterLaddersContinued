using BepInEx;
using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BetterLaddersContinued
{
    [BepInPlugin("BetterLaddersContinued", "BetterLaddersContinued", "0.217.24")]
    [BepInProcess("valheim.exe")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class BetterLaddersContinued : BaseUnityPlugin
    {
        private readonly Harmony m_harmony = new Harmony(nameof(BetterLaddersContinued));

        private void Awake() => m_harmony.PatchAll();

        private void OnDestroy() => m_harmony.UnpatchSelf();

        [HarmonyPatch(typeof(AutoJumpLedge), "OnTriggerStay")]
        public static class Ladder_Patch
        {
            private static bool Prefix(AutoJumpLedge __instance, Collider collider)
            {
                Character character = collider.GetComponent<Character>();
                // Check if selected component is a player
                if (character == null || character != Player.m_localPlayer) return true;

                MoveCharacterUp(__instance, character);
                return false;
            }

            private static void MoveCharacterUp(AutoJumpLedge __instance, Character character)
            {
                Transform transform = character.transform;
                float objectYRotation = __instance.gameObject.transform.rotation.eulerAngles.y;
                float playerYRotation = transform.rotation.eulerAngles.y;

                // Check if rotation is aligned
                if (Mathf.Abs(Mathf.DeltaAngle(objectYRotation, playerYRotation)) <= 12.0f)
                {
                    float speedFactor = character.GetWalk() ? 0.06f : 0.08f;
                    transform.position =
                        transform.position + new Vector3(0, speedFactor, 0) + transform.forward * 0.08f;
                }
            }
        }
    }
}