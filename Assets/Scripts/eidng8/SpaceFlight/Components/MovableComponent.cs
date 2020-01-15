using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    public struct MovableComponent : IComponentData
    {
        public Entity entity;
        
        public float3 acceleration;

        public float3 lastAcceleration;

        public float3 angularAcceleration;

        public float3 lastAngularAcceleration;

        public float maxArmor;

        public float maxForward;

        public float maxPan;

        public float maxReverse;

        public float maxShield;

        public float maxTorque;
 
        public float speed;

        public bool stabilizing;

        public bool stopping;

        public float3 velocity;

        public float4 maxAcceleration;

        public float3 maxAngularAcceleration;

        public float3 appliedForces;

        public float3 appliedTorque;
    }
}
