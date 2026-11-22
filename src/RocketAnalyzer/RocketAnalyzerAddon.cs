using UnityEngine;

namespace RocketAnalyzer
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class RocketAnalyzerAddon : MonoBehaviour
    {
        private ApplicationLauncherButton launcherButton;
        private EditorAnalysisWindow editorWindow;
        private FlightAnalysisWindow flightWindow;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Settings.Load();
            editorWindow = new EditorAnalysisWindow();
            flightWindow = new FlightAnalysisWindow();

            GameEvents.onGUIAppLauncherReady.Add(OnAppLauncherReady);
            GameEvents.onGUIAppLauncherDestroyed.Add(OnAppLauncherDestroyed);
            GameEvents.onEditorShipModified.Add(OnEditorShipModified);
            GameEvents.onVesselWasModified.Add(OnVesselModified);
            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.onStageActivated.Add(OnStageActivated);

            Debug.Log("[RocketAnalyzer] Charge (v1.0.0).");
        }

        private void OnDestroy()
        {
            GameEvents.onGUIAppLauncherReady.Remove(OnAppLauncherReady);
            GameEvents.onGUIAppLauncherDestroyed.Remove(OnAppLauncherDestroyed);
            GameEvents.onEditorShipModified.Remove(OnEditorShipModified);
            GameEvents.onVesselWasModified.Remove(OnVesselModified);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.onStageActivated.Remove(OnStageActivated);

            if (launcherButton != null && ApplicationLauncher.Instance != null)
                ApplicationLauncher.Instance.RemoveModApplication(launcherButton);

            Settings.Save();
        }

        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (Settings.ShowEditorWindow && editorWindow != null)
                    editorWindow.Draw();
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                if (Settings.ShowFlightWindow && flightWindow != null)
                    flightWindow.Draw();
            }
        }

        private void OnAppLauncherReady()
        {
            if (!ApplicationLauncher.Ready || launcherButton != null)
                return;

            Texture2D icon = GameDatabase.Instance.GetTexture("RocketAnalyzer/Textures/icon", false);
            launcherButton = ApplicationLauncher.Instance.AddModApplication(
                OnLauncherToggle, OnLauncherToggle, null, null, null, null,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
                icon);
        }

        private void OnAppLauncherDestroyed()
        {
            launcherButton = null;
        }

        private void OnLauncherToggle()
        {
            if (HighLogic.LoadedSceneIsEditor)
                Settings.ShowEditorWindow = !Settings.ShowEditorWindow;
            else if (HighLogic.LoadedSceneIsFlight)
                Settings.ShowFlightWindow = !Settings.ShowFlightWindow;
            Settings.Save();
        }

        private void OnEditorShipModified()
        {
            if (editorWindow != null) editorWindow.MarkDirty();
        }

        private void OnVesselModified(Vessel v)
        {
            if (flightWindow != null) flightWindow.MarkDirty();
        }

        private void OnVesselChange(Vessel v)
        {
            if (flightWindow != null) flightWindow.MarkDirty();
        }

        private void OnStageActivated(int stage)
        {
            if (flightWindow != null) flightWindow.MarkDirty();
        }
    }
}
