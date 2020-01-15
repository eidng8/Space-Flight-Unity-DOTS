using eidng8.SpaceFlight.Components;
using Unity.Entities;
using Unity.Transforms;

namespace eidng8.SpaceFlight.Entities
{
    public class MovableEntity : UnityEngine.MonoBehaviour
    {
        private void Start() {
            EntityManager em = World.Active.EntityManager;

            ComponentType[] ct = new ComponentType[] {
                typeof(MovableComponent),
                typeof(Translation),
            };
            em.CreateEntity(ct);
        }
    }
}
