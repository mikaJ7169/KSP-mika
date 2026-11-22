using System;
using System.IO;
using UnityEngine;

namespace RocketAnalyzer
{
    public static class Settings
    {
        public static bool ShowEditorWindow = true;
        public static bool ShowFlightWindow = true;
        public static Rect EditorWindowPos = new Rect(60, 60, 700, 380);
        public static Rect FlightWindowPos = new Rect(60, 60, 640, 400);

        private static string ConfigPath
        {
            get
            {
                string dir = Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "RocketAnalyzer", "PluginData");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return Path.Combine(dir, "settings.cfg");
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return;
                ConfigNode node = ConfigNode.Load(ConfigPath);
                if (node == null)
                    return;
                ShowEditorWindow = GetBool(node, "showEditorWindow", ShowEditorWindow);
                ShowFlightWindow = GetBool(node, "showFlightWindow", ShowFlightWindow);
                EditorWindowPos = GetRect(node, "editorWindowPos", EditorWindowPos);
                FlightWindowPos = GetRect(node, "flightWindowPos", FlightWindowPos);
            }
            catch (Exception e)
            {
                Debug.Log("[RocketAnalyzer] Impossible de charger les reglages : " + e.Message);
            }
        }

        public static void Save()
        {
            try
            {
                ConfigNode node = new ConfigNode("ROCKET_ANALYZER");
                node.SetValue("showEditorWindow", ShowEditorWindow.ToString(), true);
                node.SetValue("showFlightWindow", ShowFlightWindow.ToString(), true);
                SetRect(node, "editorWindowPos", EditorWindowPos);
                SetRect(node, "flightWindowPos", FlightWindowPos);
                node.Save(ConfigPath);
            }
            catch (Exception e)
            {
                Debug.Log("[RocketAnalyzer] Impossible de sauvegarder les reglages : " + e.Message);
            }
        }

        private static bool GetBool(ConfigNode node, string name, bool fallback)
        {
            string v = node.GetValue(name);
            bool b;
            if (v != null && bool.TryParse(v, out b))
                return b;
            return fallback;
        }

        private static Rect GetRect(ConfigNode node, string name, Rect fallback)
        {
            try
            {
                string sx = node.GetValue(name + "_x");
                string sy = node.GetValue(name + "_y");
                string sw = node.GetValue(name + "_w");
                string sh = node.GetValue(name + "_h");
                if (sx == null || sy == null || sw == null || sh == null)
                    return fallback;
                return new Rect(float.Parse(sx), float.Parse(sy), float.Parse(sw), float.Parse(sh));
            }
            catch
            {
                return fallback;
            }
        }

        private static void SetRect(ConfigNode node, string name, Rect r)
        {
            node.SetValue(name + "_x", r.x.ToString(), true);
            node.SetValue(name + "_y", r.y.ToString(), true);
            node.SetValue(name + "_w", r.width.ToString(), true);
            node.SetValue(name + "_h", r.height.ToString(), true);
        }
    }
}
