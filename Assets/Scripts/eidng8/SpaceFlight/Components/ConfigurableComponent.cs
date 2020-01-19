using System.Collections.Generic;
using eidng8.SpaceFlight.Configurable;
using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    public struct ConfigurableComponent : IComponentData
    {
        public float mass;

        public readonly bool hasDefense;
        public DefenseComponent defense;

        public readonly bool hasOffense;
        public OffenseComponent offense;

        public readonly bool hasPower;
        public PowerComponent power;

        public readonly bool hasPropulsion;
        public PropulsionComponent propulsion;

        public ConfigurableComponent(IConfigurable cfg) {
            this.mass = default;
            this.hasDefense = false;
            this.defense = default;
            this.hasOffense = false;
            this.offense = default;
            this.hasPower = false;
            this.power = default;
            this.hasPropulsion = false;
            this.propulsion = default;

            float v;
            Dictionary<string, float> dict = cfg.Aggregate();
            if (dict.TryGetValue("mass", out v)) {
                this.mass = v;
            }

            this.hasPropulsion = this.ParsePropulsion(dict);
        }

        private bool ParsePropulsion(IReadOnlyDictionary<string, float> dict) {
            float v;
            bool has = false;
            if (dict.TryGetValue("maxForward", out v)) {
                this.propulsion.maxForces.z = v;
                has = true;
            }

            if (dict.TryGetValue("maxReverse", out v)) {
                this.propulsion.maxForces.w = v;
                has = true;
            }

            if (dict.TryGetValue("maxPan", out v)) {
                this.propulsion.maxForces.x = this.propulsion.maxForces.y = v;
                has = true;
            }

            if (dict.TryGetValue("maxTorque", out v)) {
                this.propulsion.maxTorques = new float3(v, v, v);
                has = true;
            }

            return has;
        }
    }
}
