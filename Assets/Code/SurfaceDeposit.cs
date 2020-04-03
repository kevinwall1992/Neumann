using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class SurfaceDeposit : Deposit
{
    public float MaxVolume = 0;

    public float FillRatio
    {
        get
        {
            if (MaxVolume == 0)
                return 1;

            return Mathf.Max(0, Volume / MaxVolume);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Volume > MaxVolume)
            MaxVolume = Volume;

        float height = MaxVolume > 0 ? Mathf.Pow(MaxVolume, 0.333f) * unit_height : 0;
        transform.localScale = new Vector3(height, height, height);

        extent_mesh_renderer.transform.localScale = 
            extent_mesh_renderer.transform.localScale.YChangedTo(FillRatio);
    }

    public void AddTo(Pile pile)
    {
        Composition = (Composition.Normalized() * Volume + pile).Normalized();
        Volume += pile.Volume;
    }


    static float unit_height = 1;

    new public static SurfaceDeposit Create()
    {
        SurfaceDeposit surface_deposit = GameObject.Instantiate(Scene.Main.Prefabs.SurfaceDeposit);
        surface_deposit.Volume = 0;
        surface_deposit.MaxVolume = 0;

        return surface_deposit;
    }
}
