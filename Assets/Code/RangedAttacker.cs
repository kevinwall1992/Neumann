using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Hoarder))]
public abstract class RangedAttacker : Attacker
{
    float firing_angle;
    float target_firing_angle;
    Stock.Request ammunition_request;

    [SerializeField]
    Transform barrel_tip_transform = null;

    [SerializeField]
    Animator animator = null;

    public float AimingSpeed;
    public float ShallowestFiringAngleInDegrees;
    public float SteepestFiringAngleInDegrees;
    public float Cooldown;
    public float MagazineSize;

    public float ShallowestFiringAngle
    {
        get { return MathUtility.DegreesToRadians(ShallowestFiringAngleInDegrees); }
    }

    public float SteepestFiringAngle
    {
        get { return MathUtility.DegreesToRadians(SteepestFiringAngleInDegrees); }
    }

    public float FiringAngle
    {
        get { return firing_angle; }
        set { target_firing_angle = value; }
    }

    public Vector3 FiringDirection
    {
        get
        {
            return AimBehavior.GetFiringDirection(
                Unit.Physical.Direction, 
                Scene.Main.World.Asteroid.GetSurfaceNormal(Unit.Physical.Position), 
                FiringAngle);
        }
    }

    public Vector3 BarrelTip { get { return barrel_tip_transform.position; } }

    public System.DateTime LastFiring { get; private set; }

    public Hoarder Hoarder { get { return GetComponent<Hoarder>(); } }

    protected virtual void Start()
    {
        LastFiring = System.DateTime.Now;
        
        Pile cartridge_requirements = GetCartridgeRequirements();
        Hoarder.Capacity.PutIn(cartridge_requirements * MagazineSize);
        ammunition_request = Unit.Team.Stock.MakeRequest(cartridge_requirements / Cooldown);
    }

    protected override void Update()
    {
        base.Update();

        Hoarder.Contents.PutIn(ammunition_request.Disbursement);

        firing_angle = MathUtility.ZenoLerpAngle(firing_angle,  
                                                target_firing_angle,
                                                Time.deltaTime * AimingSpeed, 
                                                MathUtility.DegreesToRadians(5));

        if (animator != null)
            animator.SetFloat("moment", (firing_angle - ShallowestFiringAngle) / 
                                        (SteepestFiringAngle - ShallowestFiringAngle));

        if (IsAttacking && (System.DateTime.Now - LastFiring).Seconds >= Cooldown)
        {
            if (UseCartridge())
            {
                Fire();
                LastFiring = System.DateTime.Now;
            }
        }

        if (Unit.Task is AttackTask)
            this.Start<RangedAttackBehavior>().Target = (Unit.Task as AttackTask).Target;
        else
            this.Stop<RangedAttackBehavior>();
    }

    protected abstract Pile GetCartridgeRequirements();

    bool UseCartridge()
    {
        Pile cartridge_requirements = GetCartridgeRequirements();

        Pile cartridge = Hoarder.Contents.TakeOut(cartridge_requirements);
        if (cartridge.Volume < cartridge_requirements.Volume)
        {
            Hoarder.Contents.PutIn(cartridge);
            return false;
        }

        return true;
    }

    public abstract void Fire();
}

public class AimBehavior : Behavior
{
    bool is_aimed = false;

    public Target Target { get; set; }

    public RangedAttacker RangedAttacker { get { return GetComponent<RangedAttacker>(); } }

    public bool IsAimed { get { return is_aimed; } }

    protected override void Update()
    {
        base.Update();

        is_aimed = false;

        Vector3 position = RangedAttacker.BarrelTip;
        Vector3 displacement = Target.Position - position;
        Vector3 direction = displacement.normalized;
        Vector3 flat_up = new Vector3(0, 1, 0);
        Vector3 flat_direction = direction.InPlane(flat_up).normalized;
        Vector3 surface_normal = Scene.Main.World.Asteroid.GetSurfaceNormal(position);
        
        float flat_angle;
        if(!(RangedAttacker is ProjectileAttacker))
            flat_angle = Mathf.Atan(flat_up.Dot(direction) / flat_direction.Dot(direction));
        else
        {
            ProjectileAttacker projectile_attacker = RangedAttacker as ProjectileAttacker;

            float squared_velocity = Mathf.Pow(projectile_attacker.ProjectileVelocity, 2);
            float horizontal_distance = displacement.Dot(flat_direction);
            float vertical_distance = Target.Position.y - position.y;
            float gravity = Scene.Main.World.Asteroid.SurfaceGravity;

            float root = squared_velocity * squared_velocity -
                         gravity * (gravity * Mathf.Pow(horizontal_distance, 2) +
                                    2 * vertical_distance * squared_velocity);
            if (root < 0)
            {
                RangedAttacker.FiringAngle = Mathf.PI / 4;
                return;
            }
            root = Mathf.Sqrt(root);
            if (!projectile_attacker.UseHighFiringAngle)
                root *= -1;

            flat_angle = Mathf.Atan((squared_velocity + root) / (gravity * horizontal_distance));
        }

        Vector3 target_firing_direction = GetFiringDirection(flat_direction, flat_up, flat_angle);
        Vector3 target_forward = target_firing_direction.InPlane(surface_normal).normalized;
        float target_angle = target_firing_direction.AngleBetween(target_forward);
        if (target_firing_direction.y < target_forward.y)
            target_angle *= -1;

        target_angle = Mathf.Max(target_angle, RangedAttacker.ShallowestFiringAngle);
        target_angle = Mathf.Min(target_angle, RangedAttacker.SteepestFiringAngle);

        //Turn
        if (RangedAttacker.HasComponent<Motile>())
            RangedAttacker.GetComponent<Motile>().Turn(target_forward);
        
        //Point
        RangedAttacker.FiringAngle = target_angle;

        if (target_firing_direction.Dot(RangedAttacker.FiringDirection) > 0.9999f)
            is_aimed = true;
    }

    public static Vector3 GetFiringDirection(Vector3 forward, Vector3 up, float firing_angle)
    {
        Vector3 forward_component = forward * Mathf.Cos(firing_angle);
        Vector3 up_component = up * Mathf.Sin(firing_angle);

        return forward_component + up_component;
    }
}

public class RangedAttackBehavior : Behavior
{
    public Target Target { get; set; }

    public RangedAttacker RangedAttacker { get { return GetComponent<RangedAttacker>(); } }

    protected override void Update()
    {
        base.Update();

        this.Start<AimBehavior>().Target = Target;

        if (this.GetBehavior<AimBehavior>().IsAimed)
            RangedAttacker.Attack();
        else
            RangedAttacker.StopAttacking();
    }

    protected override void OnDestroy()
    {
        this.Stop<AimBehavior>();
        RangedAttacker.StopAttacking();

        base.OnDestroy();
    }
}
