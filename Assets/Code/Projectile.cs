using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Physical))]
public class Projectile : MonoBehaviour
{
    float spin_angle = 0;
    float angular_velocity = Mathf.PI * 2;

    Vector3 last_position;

    [SerializeField]
    MeshRenderer mesh_renderer = null;

    public Pile Materials;
    public float BaseDamage;
    public float Penetration;

    public ProjectileAttacker Source { get; set; }

    public Physical Physical { get { return GetComponent<Physical>(); } }

    void Start()
    {
        last_position = transform.position;
    }

    void Update()
    {
        spin_angle += angular_velocity * Time.deltaTime; 
        mesh_renderer.transform.localRotation = 
            Quaternion.AngleAxis(MathUtility.RadiansToDegrees(spin_angle), 
                                 new Vector3(0, 0, 1));

        Vector3 displacement = transform.position - last_position;
        RaycastHit[] raycast_hits = UnityEngine.Physics.RaycastAll(last_position, displacement.normalized, displacement.magnitude);

        foreach (RaycastHit raycast_hit in raycast_hits)
        {
            if (raycast_hit.collider.gameObject == Source.gameObject)
                continue;

            if(raycast_hit.collider.gameObject.HasComponent<Mortal>())
            {
                Mortal mortal = raycast_hit.collider.gameObject.GetComponent<Mortal>();
                Physical physical = mortal.GetComponent<Physical>();

                float angle = (mortal.transform.position - raycast_hit.point).AngleBetween(displacement);
                float intersection_length = Mathf.Min(mortal.transform.localScale.y / Mathf.Cos(angle), 
                                                      raycast_hit.point.Distance(transform.position));

                float energy_imparted = Physical.KineticEnergy * (1 - Mathf.Pow(Penetration, intersection_length));
                Physical.KineticEnergy -= energy_imparted;

                float damage_energy = 0;
                if (physical == null)
                    damage_energy = energy_imparted;
                else if (Physical.Velocity.magnitude > mortal.ImpactTolerance)
                    damage_energy = energy_imparted * mortal.ImpactFragility;

                float kinetic_energy = energy_imparted - damage_energy;

                if (kinetic_energy > 0)
                    physical.Velocity += Physical.Velocity.normalized *
                                         Measures.MetersToWorldUnits(Mathf.Sqrt(2 * kinetic_energy / physical.Mass));

                mortal.Health.Value -= BaseDamage * KineticEnergyToDamage(damage_energy);
            }
        }

        if (Scene.Main.World.Asteroid.GetSurfaceHeight(Physical.Position) > transform.position.y)
            GameObject.Destroy(gameObject);
    }

    public static float KineticEnergyToDamage(float joules)
    {
        return 100 * joules / 20000;
    }
}
