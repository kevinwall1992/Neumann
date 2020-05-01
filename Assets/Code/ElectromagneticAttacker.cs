using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ElectromagneticAttacker : RangedAttacker
{
    [SerializeField]
    LineRenderer beam_line = null;

    [SerializeField]
    BoolAnimatorProperty is_beam_on = null;

    public float Wattage;
    public float FiringDuration;

    public bool IsFiring { get; private set; }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsAttacking || (System.DateTime.Now - LastFiring).TotalSeconds > FiringDuration)
            IsFiring = false;

        if (!IsFiring)
        {
            is_beam_on.Value = false;
            return;
        }

        is_beam_on.Value = true;


        RaycastHit[] raycast_hits = UnityEngine.Physics.RaycastAll(BarrelTip, FiringDirection);

        RaycastHit terrain_hit = raycast_hits.ToList().Find(raycast_hit => raycast_hit.collider is TerrainCollider);
        float stop_distance = terrain_hit.collider != null ? terrain_hit.distance : 10000;

        foreach (RaycastHit raycast_hit in raycast_hits)
        {
            if (raycast_hit.collider.gameObject == gameObject)
                continue;

            if (raycast_hit.distance > stop_distance)
                continue;

            if (raycast_hit.collider.gameObject.HasComponent<Unit>())
                raycast_hit.collider.gameObject.GetComponent<Unit>()
                    .Status<Excited>()
                    .Excite(Wattage * Time.deltaTime);
        }

        beam_line.SetPosition(0, BarrelTip);
        beam_line.SetPosition(1, BarrelTip + FiringDirection * stop_distance);
    }

    public override void Fire()
    {
        IsFiring = true;
    }

    protected override Pile GetCartridgeRequirements()
    {
        return new Pile().PutIn(Resource.Energy, Measures.JoulesToEnergyUnits(Wattage));
    }
}
