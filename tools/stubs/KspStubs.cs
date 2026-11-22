// Stubs minimaux de l'API KSP 1 / Unity, utilises UNIQUEMENT pour verifier la
// compilation du mod sans installation KSP (voir tools/verify-compile.sh).
// Ne pas embarquer dans le mod.
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Object
    {
        public static void DontDestroyOnLoad(Object target) { }
    }

    public class GameObject : Object { }

    public class MonoBehaviour : Object
    {
        public GameObject gameObject { get { return null; } }
    }

    public static class Debug
    {
        public static void Log(object message) { }
    }

    public static class Time
    {
        public static float realtimeSinceStartup { get { return 0f; } }
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float w, float h) { this.x = x; this.y = y; width = w; height = h; }
    }

    public struct Vector2
    {
        public float x, y;
    }

    public struct Vector3d
    {
        public double x, y, z;
    }

    public class Texture2D : Object { }

    public class GUISkin : Object { }

    public class GUILayoutOption { }

    public static class GUI
    {
        public delegate void WindowFunction(int id);
        public static GUISkin skin { get; set; }
        public static void DragWindow(Rect r) { }
    }

    public static class GUILayout
    {
        public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options) { return screenRect; }
        public static void BeginVertical(params GUILayoutOption[] o) { }
        public static void EndVertical() { }
        public static void BeginHorizontal(params GUILayoutOption[] o) { }
        public static void EndHorizontal() { }
        public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] o) { return scrollPosition; }
        public static void EndScrollView() { }
        public static void Label(string text, params GUILayoutOption[] o) { }
        public static bool Button(string text, params GUILayoutOption[] o) { return false; }
        public static void Space(float pixels) { }
        public static GUILayoutOption Width(double w) { return null; }
        public static GUILayoutOption MinWidth(double w) { return null; }
    }
}

public class KSPAddon : Attribute
{
    public enum Startup { Instantly, MainMenu, SpaceCentre, EditorAny, EditorVAB, EditorSPH, Flight, TrackingStation }
    public KSPAddon(Startup startup, bool once) { }
}

public class EventData
{
    public delegate void OnEvent();
    public void Add(OnEvent e) { }
    public void Remove(OnEvent e) { }
}

public class EventData<T>
{
    public delegate void OnEvent(T data);
    public void Add(OnEvent e) { }
    public void Remove(OnEvent e) { }
}

public static class GameEvents
{
    public static EventData onGUIAppLauncherReady = new EventData();
    public static EventData onGUIAppLauncherDestroyed = new EventData();
    public static EventData onEditorShipModified = new EventData();
    public static EventData<Vessel> onVesselWasModified = new EventData<Vessel>();
    public static EventData<Vessel> onVesselChange = new EventData<Vessel>();
    public static EventData<int> onStageActivated = new EventData<int>();
}

public class ApplicationLauncherButton { }

public class ApplicationLauncher
{
    public delegate void GUIEvent();

    [Flags]
    public enum AppScenes
    {
        MAINMENU = 1, SPACECENTER = 2, VAB = 4, SPH = 8,
        FLIGHT = 16, TRACKSTATION = 32, MAPVIEW = 64
    }

    public static bool Ready { get { return true; } }
    public static ApplicationLauncher Instance { get { return null; } }

    public ApplicationLauncherButton AddModApplication(
        GUIEvent onTrue, GUIEvent onFalse, GUIEvent onHover, GUIEvent onHoverOut,
        GUIEvent onEnable, GUIEvent onDisable, AppScenes visibleScenes, UnityEngine.Texture2D texture)
    {
        return null;
    }

    public void RemoveModApplication(ApplicationLauncherButton button) { }
}

public class GameDatabase
{
    public static GameDatabase Instance { get { return null; } }
    public UnityEngine.Texture2D GetTexture(string name, bool asNormalMap) { return null; }
}

public static class HighLogic
{
    public static UnityEngine.GUISkin Skin { get { return null; } }
    public static bool LoadedSceneIsEditor { get { return false; } }
    public static bool LoadedSceneIsFlight { get { return false; } }
}

public class EditorLogic
{
    public static EditorLogic fetch { get { return null; } }
    public ShipConstruct ship;
}

public class ShipConstruct
{
    public List<Part> parts = new List<Part>();
}

public class Part
{
    public int stage;
    public float mass;
    public PartResourceList Resources = new PartResourceList();
    public List<T> FindModulesImplementing<T>() { return new List<T>(); }
}

public class PartResourceList : List<PartResource> { }

public class PartResource
{
    public double amount;
    public string resourceName;
    public PartResourceDefinition info;
}

public class PartResourceDefinition
{
    public double density;
}

public class Propellant
{
    public string name;
}

public class FloatCurve
{
    public float Evaluate(float t) { return 0f; }
}

public class ModuleEngines
{
    public float maxThrust;
    public float thrustPercentage = 100f;
    public FloatCurve atmosphereCurve;
    public List<Propellant> propellants = new List<Propellant>();
    public bool EngineIgnited;
    public bool isOperational;
    public float finalThrust;
}

public class Orbit
{
    public double ApA, PeA, eccentricity, inclination, timeToAp;
    public CelestialBody referenceBody;
}

public class CelestialBody
{
    public string bodyName;
    public double GeeASL;
    public double Radius;
    public double gravParameter;
}

public class Planetarium
{
    public static Planetarium fetch { get { return null; } }
    public CelestialBody Home;
}

public class Vessel
{
    public enum Situations { PRELAUNCH, LANDED, SPLASHED, FLYING, SUB_ORBITAL, ORBITING, ESCAPING, DOCKED }

    public List<Part> parts = new List<Part>();
    public UnityEngine.Vector3d CoM;
    public Orbit orbit;
    public int currentStage;
    public Situations situation;
    public double missionTime;
    public string vesselName;
    public CelestialBody mainBody;
    public bool Landed { get { return false; } }
    public bool Splashed { get { return false; } }
    public double GetTotalMass() { return 0; }
}

public static class FlightGlobals
{
    public static Vessel ActiveVessel { get { return null; } }
    public static double getGeeForceAtPosition(UnityEngine.Vector3d pos) { return 9.80665; }
}

public class ConfigNode
{
    public ConfigNode() { }
    public ConfigNode(string name) { }
    public static ConfigNode Load(string path) { return null; }
    public void Save(string path) { }
    public string GetValue(string name) { return null; }
    public bool HasValue(string name) { return false; }
    public void SetValue(string name, string value, bool createIfNotFound) { }
}

public static class KSPUtil
{
    public static string ApplicationRootPath { get { return ""; } }
}
