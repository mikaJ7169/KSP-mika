using System;
using UnityEngine;

namespace RocketAnalyzer
{
    public class FlightAnalysisWindow
    {
        private const int WindowId = 747002;
        private const double RefreshInterval = 0.5;

        private Rect pos;
        private Vector2 scroll;
        private Vessel vessel;
        private AnalysisResult result;
        private double currentThrust, currentMass, currentTwr, currentG;
        private string status = "";
        private double nextRefresh;

        public FlightAnalysisWindow()
        {
            pos = Settings.FlightWindowPos;
            MarkDirty();
        }

        public void MarkDirty()
        {
            nextRefresh = 0;
        }

        public void Draw()
        {
            if (Time.realtimeSinceStartup >= nextRefresh)
                Refresh();
            GUI.skin = HighLogic.Skin;
            pos = GUILayout.Window(WindowId, pos, DrawWindow, "Rocket Analyzer - Vol", GUILayout.MinWidth(640));
            Settings.FlightWindowPos = pos;
        }

        private void Refresh()
        {
            nextRefresh = Time.realtimeSinceStartup + RefreshInterval;
            try
            {
                vessel = FlightGlobals.ActiveVessel;
                if (vessel == null)
                {
                    result = null;
                    status = "Aucun vaisseau actif.";
                    return;
                }
                double g = StageAnalyzer.G0;
                try { g = FlightGlobals.getGeeForceAtPosition(vessel.CoM); }
                catch { }

                result = StageAnalyzer.AnalyzeShip(vessel.parts, g);

                double thrust = 0;
                foreach (Part p in vessel.parts)
                    foreach (ModuleEngines e in p.FindModulesImplementing<ModuleEngines>())
                        if (e.EngineIgnited && e.isOperational)
                            thrust += e.finalThrust;

                currentThrust = thrust;
                currentMass = vessel.GetTotalMass();
                currentG = g;
                currentTwr = currentMass > 0 ? currentThrust / (currentMass * g) : 0;
                status = "";
            }
            catch (Exception e)
            {
                result = null;
                status = "Erreur d'analyse : " + e.Message;
            }
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            if (!string.IsNullOrEmpty(status))
            {
                GUILayout.Label(status);
            }
            else if (vessel != null && result != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Vaisseau : " + vessel.vesselName, GUILayout.Width(320));
                GUILayout.Label("Situation : " + vessel.situation);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Masse : " + GuiUtil.FormatMassKg(currentMass), GUILayout.Width(210));
                GUILayout.Label("Poussee : " + GuiUtil.FormatThrustN(currentThrust), GUILayout.Width(210));
                GUILayout.Label("TWR : " + currentTwr.ToString("F2") + "  (g = " + currentG.ToString("F2") + " m/s2)");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Dv restant (vide) : " + GuiUtil.FormatDeltaV(result.TotalDeltaV), GUILayout.Width(270));
                GUILayout.Label("Dv restant (ASL) : " + GuiUtil.FormatDeltaV(result.TotalDeltaVAsl), GUILayout.Width(250));
                GUILayout.Label("Temps mission : " + GuiUtil.FormatSeconds(vessel.missionTime));
                GUILayout.EndHorizontal();

                if (vessel.orbit != null && !vessel.Landed && !vessel.Splashed)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Apoapsis : " + GuiUtil.FormatDistance(vessel.orbit.ApA), GUILayout.Width(210));
                    GUILayout.Label("Periapsis : " + GuiUtil.FormatDistance(vessel.orbit.PeA), GUILayout.Width(210));
                    GUILayout.Label("Excentricite : " + vessel.orbit.eccentricity.ToString("F3"));
                    GUILayout.EndHorizontal();
                    if (vessel.orbit.eccentricity < 1 && vessel.orbit.timeToAp > 0)
                        GUILayout.Label("Temps avant apoapsis : " + GuiUtil.FormatSeconds(vessel.orbit.timeToAp)
                            + "   |   Corps : " + (vessel.orbit.referenceBody != null ? vessel.orbit.referenceBody.bodyName : "?"));
                    else
                        GUILayout.Label("Corps : " + (vessel.orbit.referenceBody != null ? vessel.orbit.referenceBody.bodyName : "?"));
                }

                GUILayout.Space(4);

                scroll = GUILayout.BeginScrollView(scroll);
                DrawHeader();
                foreach (StageInfo s in result.Stages)
                    DrawStageRow(s, vessel.currentStage);
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rafraichir", GUILayout.Width(110)))
                MarkDirty();
            if (GUILayout.Button("Fermer", GUILayout.Width(110)))
                Settings.ShowFlightWindow = false;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 24));
        }

        private static void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            Cell("Etage", 50);
            Cell("Moteurs", 60);
            Cell("Masse depart", 100);
            Cell("Ergol", 90);
            Cell("Poussee", 90);
            Cell("Isp vide", 70);
            Cell("TWR", 55);
            Cell("Dv", 90);
            Cell("Duree", 70);
            GUILayout.EndHorizontal();
        }

        private void DrawStageRow(StageInfo s, int currentStage)
        {
            GUILayout.BeginHorizontal();
            Cell((s.Stage == currentStage ? "> " : "") + s.Stage, 50);
            Cell(s.EngineCount.ToString(), 60);
            Cell(GuiUtil.FormatMassKg(s.MassStart), 100);
            Cell(GuiUtil.FormatMassKg(s.PropellantMass), 90);
            Cell(GuiUtil.FormatThrustN(s.Thrust), 90);
            Cell(s.IspVac.ToString("F0") + " s", 70);
            Cell(s.Twr.ToString("F2"), 55);
            Cell(GuiUtil.FormatDeltaV(s.DeltaV), 90);
            Cell(GuiUtil.FormatSeconds(s.BurnTime), 70);
            GUILayout.EndHorizontal();
        }

        private static void Cell(string text, double width)
        {
            GUILayout.Label(text, GUILayout.Width(width));
        }
    }
}
