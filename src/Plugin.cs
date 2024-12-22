using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace Zoom;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static ManualLogSource Log;

    internal static Harmony harmony = new Harmony("zoom");

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.DEBUG = true;

        harmony.PatchAll(typeof(Patch_WorldPlayer));
    }

    internal static class Patch_WorldPlayer
    {
		private static readonly float maxZoom = 10f;
        private static readonly float minZoom = 70f;
		private static readonly float zoomIncrement = 2f;
        private static float zoom = 20f;
		private static bool zooming = false;

		[HarmonyPatch(typeof(WorldPlayer), "Update")]
		[HarmonyPrefix]
		public static void Update(WorldPlayer __instance) {
			if (Input.GetKey(KeyCode.LeftAlt)) {
				if (Input.mouseScrollDelta.y > 0f) {
					float newZoom = zoom - zoomIncrement;
					if (newZoom >= maxZoom) {
                        zoom = newZoom;
                    } else if (zoom != maxZoom) {
                        zoom = maxZoom;
                    }
                } else if (Input.mouseScrollDelta.y < 0f) {
					float newZoom = zoom + zoomIncrement;
					if (newZoom <= minZoom) {
                        zoom = newZoom;
                    } else if (zoom != minZoom) {
                        zoom = minZoom;
                    }
                }
                if (zooming) {
                    __instance.CameraFOVSharply(zoom);
                }
            }
			if (Input.GetKeyDown(KeyCode.Mouse1)) {
				zooming = true;
                //__instance.CameraAnimationFOV(zoom);
                __instance.CameraFOVSharply(zoom);
            }
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.Mouse1) && zooming) {
                zooming = false;
                //__instance.CameraFOVLerpReset();
                __instance.CameraFOVSharplyReset();
            }
		}
	}
}
