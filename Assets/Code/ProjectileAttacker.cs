using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectileAttacker : RangedAttacker
{
    public Projectile Projectile;
    public float ProjectileVelocity;

    public bool UseHighFiringAngle { get; set; }

    public override IEnumerable<Operation> Abilities
    {
        get
        {
            List<Operation> abilities = base.Abilities.ToList();

            abilities.Add(new HighFiringAngleOperation());
            abilities.Add(new LowFiringAngleOperation());

            return abilities;
        }
    }

    protected override void Start()
    {
        base.Start();

        Unit.Memory.Memorize("Firing Angle", () => UseHighFiringAngle ? "High" : "Low");
    }

    public override void Fire()
    {
        Projectile projectile = GameObject.Instantiate(Projectile);
        projectile.transform.parent = Scene.Main.World.Asteroid.transform;
        projectile.Source = this;

        projectile.Physical.Position = BarrelTip;
        projectile.Physical.Velocity = FiringDirection * ProjectileVelocity;
    }

    protected override Pile GetCartridgeRequirements()
    {
        Projectile.Physical.Velocity = new Vector3(ProjectileVelocity, 0, 0);

        return new Pile()
            .PutIn(Projectile.Materials)
            .PutIn(Resource.Energy, Measures.JoulesToEnergyUnits(Projectile.Physical.KineticEnergy));
    }
}

public class HighFiringAngleOperation : Operation
{
    public override void Execute(Unit unit)
    {
        unit.GetComponent<ProjectileAttacker>().UseHighFiringAngle = true;

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new HighFiringAngleOperation();
    }

    public override Style.Operation Style
    { get { return Scene.Main.Style.HighFiringAngleOperation; } }
}

public class LowFiringAngleOperation : Operation
{
    public override void Execute(Unit unit)
    {
        unit.GetComponent<ProjectileAttacker>().UseHighFiringAngle = false;

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new LowFiringAngleOperation();
    }

    public override Style.Operation Style
    { get { return Scene.Main.Style.LowFiringAngleOperation; } }
}
