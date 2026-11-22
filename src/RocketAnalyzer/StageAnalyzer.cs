using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketAnalyzer
{
    public class StageInfo
    {
        public int Stage;
        public int PartCount;
        public int EngineCount;
        public double DryMass;
        public double PropellantMass;
        public double MassStart;
        public double MassEnd;
        public double Thrust;
        public double IspVac;
        public double IspAsl;
        public double DeltaV;
        public double DeltaVAsl;
        public double BurnTime;
        public double Twr;
    }

    public class AnalysisResult
    {
        public List<StageInfo> Stages = new List<StageInfo>();
        public double TotalMass;
        public double DryMass;
        public double PropellantMass;
        public double TotalDeltaV;
        public double TotalDeltaVAsl;
        public double TotalThrust;
        public double TotalBurnTime;
    }

    public static class StageAnalyzer
    {
        public const double G0 = 9.80665;

        public static AnalysisResult AnalyzeShip(IEnumerable<Part> parts, double surfaceGravity)
        {
            var result = new AnalysisResult();
            List<Part> partList = parts.ToList();
            if (partList.Count == 0)
                return result;

            var propellantNames = new HashSet<string>();
            foreach (Part p in partList)
                foreach (ModuleEngines e in p.FindModulesImplementing<ModuleEngines>())
                    if (e.propellants != null)
                        foreach (Propellant pr in e.propellants)
                            propellantNames.Add(pr.name);

            double totalMass = 0, dryMass = 0, propMass = 0;
            foreach (Part p in partList)
            {
                double rm = ResourceMass(p);
                double pm = PropellantMass(p, propellantNames);
                totalMass += p.mass + rm;
                dryMass += p.mass + (rm - pm);
                propMass += pm;
            }

            int maxStage = partList.Max(p => Math.Max(0, p.stage));
            double g = surfaceGravity > 0 ? surfaceGravity : G0;
            double m = totalMass;

            for (int s = maxStage; s >= 0; s--)
            {
                List<Part> stageParts = partList.Where(p => Math.Max(0, p.stage) == s).ToList();
                if (stageParts.Count == 0)
                    continue;

                List<ModuleEngines> engines = stageParts
                    .SelectMany(p => p.FindModulesImplementing<ModuleEngines>())
                    .ToList();

                double prop = stageParts.Sum(p => PropellantMass(p, propellantNames));
                double stageDry = stageParts.Sum(p => p.mass + ResourceMass(p)) - prop;

                double thrust = 0, ispVacW = 0, ispAslW = 0;
                foreach (ModuleEngines e in engines)
                {
                    double t = e.maxThrust * (e.thrustPercentage / 100.0);
                    thrust += t;
                    double iv = e.atmosphereCurve != null ? e.atmosphereCurve.Evaluate(0f) : 0;
                    double ia = e.atmosphereCurve != null ? e.atmosphereCurve.Evaluate(1f) : 0;
                    ispVacW += iv * t;
                    ispAslW += ia * t;
                }
                double ispVac = thrust > 0 ? ispVacW / thrust : 0;
                double ispAsl = thrust > 0 ? ispAslW / thrust : 0;

                double m0 = m;
                double m1 = m0 - prop;
                if (m1 < 0.001) m1 = 0.001;

                double dvVac = (ispVac > 0 && prop > 0 && m0 > m1) ? ispVac * G0 * Math.Log(m0 / m1) : 0;
                double dvAsl = (ispAsl > 0 && prop > 0 && m0 > m1) ? ispAsl * G0 * Math.Log(m0 / m1) : 0;
                double burn = (thrust > 0 && ispVac > 0 && prop > 0) ? prop * ispVac * G0 / thrust : 0;

                result.Stages.Add(new StageInfo
                {
                    Stage = s,
                    PartCount = stageParts.Count,
                    EngineCount = engines.Count,
                    DryMass = stageDry,
                    PropellantMass = prop,
                    MassStart = m0,
                    MassEnd = m1,
                    Thrust = thrust,
                    IspVac = ispVac,
                    IspAsl = ispAsl,
                    DeltaV = dvVac,
                    DeltaVAsl = dvAsl,
                    BurnTime = burn,
                    Twr = thrust / (m0 * g)
                });

                m = m1;
            }

            result.TotalMass = totalMass;
            result.DryMass = dryMass;
            result.PropellantMass = propMass;
            result.TotalDeltaV = result.Stages.Sum(s => s.DeltaV);
            result.TotalDeltaVAsl = result.Stages.Sum(s => s.DeltaVAsl);
            result.TotalThrust = result.Stages.Sum(s => s.Thrust);
            result.TotalBurnTime = result.Stages.Sum(s => s.BurnTime);
            return result;
        }

        public static double ResourceMass(Part p)
        {
            double m = 0;
            if (p.Resources != null)
                foreach (PartResource r in p.Resources)
                    if (r.info != null)
                        m += r.amount * r.info.density;
            return m;
        }

        public static double PropellantMass(Part p, HashSet<string> propellantNames)
        {
            double m = 0;
            if (p.Resources != null)
                foreach (PartResource r in p.Resources)
                    if (r.info != null && propellantNames.Contains(r.resourceName))
                        m += r.amount * r.info.density;
            return m;
        }
    }
}
