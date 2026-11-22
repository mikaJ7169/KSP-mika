using System;
using UnityEngine;

namespace RocketAnalyzer
{
    public class EditorAnalysisWindow
    {
        private const int WindowId = 747001;
        private const double RefreshInterval = 0.5;

        private Rect pos;
        private Vector2 scroll;
        private AnalysisResult result;
        private string status = "";
        private double nextRefresh;

        public EditorAnalysisWindow()
        {
            pos = Settings.EditorWindowPos;
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
            pos = GUILayout.Window(WindowId, pos, DrawWindow, "Rocket Analyzer - Editeur", GUILayout.MinWidth(700));
            Settings.EditorWindowPos = pos;
        }

        private void Refresh()
        {
            nextRefresh = Time.realtimeSinceStartup + RefreshInterval;
            try
            {
                if (EditorLogic.fetch == null || EditorLogic.fetch.ship == null || EditorLogic.fetch.ship.parts == null)
                {
                    result = null;
                    status = "Aucun vaisseau en cours d'edition.";
                    return;
                }
                double g = StageAnalyzer.G0;
                try
                {
                    if (Planetarium.fetch != null && Planetarium.fetch.Home != null)
                        g = Planetarium.fetch.Home.GeeASL * StageAnalyzer.G0;
                }
                catch { }
                result = StageAnalyzer.AnalyzeShip(EditorLogic.fetch.ship.parts, g);
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
            else if (result != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Masse totale : " + GuiUtil.FormatMassKg(result.TotalMass), GUILayout.Width(230));
                GUILayout.Label("Masse a vide : " + GuiUtil.FormatMassKg(result.DryMass), GUILayout.Width(230));
                GUILayout.Label("Dv total (vide) : " + GuiUtil.FormatDeltaV(result.TotalDeltaV));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Ergol total : " + GuiUtil.FormatMassKg(result.PropellantMass), GUILayout.Width(230));
                GUILayout.Label("Dv total (niveau mer) : " + GuiUtil.FormatDeltaV(result.TotalDeltaVAsl), GUILayout.Width(280));
                if (result.Stages.Count > 0)
                {
                    StageInfo first = result.Stages[0];
                    GUILayout.Label("TWR lancement : " + first.Twr.ToString("F2") + "  (" + GuiUtil.FormatThrustN(first.Thrust) + ")");
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(4);

                scroll = GUILayout.BeginScrollView(scroll);
                DrawHeader();
                foreach (StageInfo s in result.Stages)
                    DrawStageRow(s, -1);
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rafraichir", GUILayout.Width(110)))
                MarkDirty();
            if (GUILayout.Button("Fermer", GUILayout.Width(110)))
                Settings.ShowEditorWindow = false;
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
            Cell("Isp ASL", 70);
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
            Cell(s.IspAsl.ToString("F0") + " s", 70);
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
