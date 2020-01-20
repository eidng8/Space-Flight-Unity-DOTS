using System.Collections.Generic;
using eidng8.SpaceFlight.Configurable;
using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components.Configurable
{
    public struct ConfigurableComponent : IComponentData
    {
        /// <summary>
        ///     Correspond to Unity's built-in
        ///     <see cref="Unity.Physics.PhysicsMass.InverseMass" />
        /// </summary>
        public float mass;

        public readonly bool hasDefense;
        public DefenseComponent defense;

        // public readonly bool hasOffense;
        // public OffenseComponent offense;

        public readonly bool hasPower;
        public PowerComponent power;

        public readonly bool hasEnergy;
        public EnergyComponent energy;

        public readonly bool hasPropulsion;
        public PropulsionComponent propulsion;

        public ConfigurableComponent(IConfigurable cfg) {
            this.mass = default;
            this.hasDefense = false;
            this.defense = default;
            // this.hasOffense = false;
            // this.offense = default;
            this.hasPower = false;
            this.power = default;
            this.hasEnergy = false;
            this.energy = default;
            this.hasPropulsion = false;
            this.propulsion = default;

            float v;
            Dictionary<string, float> dict = cfg.Aggregate();
            if (dict.TryGetValue("mass", out v)) {
                this.mass = v;
            }

            this.hasPropulsion = this.ParseDefense(dict);
            this.hasPropulsion = this.ParsePower(dict);
            this.hasPropulsion = this.ParseEnergy(dict);
            this.hasPropulsion = this.ParsePropulsion(dict);
        }

        private bool ParseDefense(IReadOnlyDictionary<string, float> dict) {
            float v;
            bool has = false;
            if (dict.TryGetValue("armor", out v)) {
                this.defense.armor = v;
                has = true;
            }

            if (dict.TryGetValue("shield", out v)) {
                this.defense.shield = v;
                has = true;
            }

            if (dict.TryGetValue("shieldRecharge", out v)) {
                this.defense.shieldRecharge = v;
                has = true;
            }

            return has;
        }

        private bool ParseEnergy(IReadOnlyDictionary<string, float> dict) {
            float v;
            bool has = false;
            if (dict.TryGetValue("capacitor", out v)) {
                this.energy.capacitor = v;
                has = true;
            }

            if (dict.TryGetValue("recharge", out v)) {
                this.energy.recharge = v;
                has = true;
            }

            return has;
        }

        private bool ParsePower(IReadOnlyDictionary<string, float> dict) {
            float v;
            bool has = false;
            if (dict.TryGetValue("power", out v)) {
                this.power.power = v;
                has = true;
            }

            return has;
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
