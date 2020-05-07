using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class OperationTileNode : MonoBehaviour
{
    Color color;

    [SerializeField]
    float ui_depth = 5;
    protected float UIDepth { get { return ui_depth; } }

    [SerializeField]
    Image image = null;

    [SerializeField]
    LineRenderer line = null;

    [SerializeField]
    protected ColorMaterialProperty line_color = new ColorMaterialProperty();

    [SerializeField]
    BezierLineController bezier_line_controller = null;
    protected BezierLineController BezierLineController { get { return bezier_line_controller; } }

    protected virtual bool IsLineValid { get { return true; } }

    public OperationTile OperationTile { get { return GetComponentInParent<OperationTile>(); } }

    public bool IsSelected
    {
        get { return Selected == this; }
        set
        {
            if (value)
                Selected = this;
            else if (Selected == this)
                Selected = null;
        }
    }

    public virtual bool IsOpen { get { return OperationTile.IsOpen; } }

    public float MouseDistance
    {
        get { return BezierLineController.GetDistanceInScreenSpace(Input.mousePosition); }
    }

    public virtual float LineAlpha { get; protected set; }

    protected virtual void Start()
    {
        color = image.color;
    }

    protected virtual void Update()
    {
        if (IsSelected)
            this.ClaimMouseLeftRelease();
        else
            this.YieldMouseLeftReleaseClaim();


        BezierLineController.StartPosition = Scene.Main.Camera.ScreenToWorldPoint(
            transform.position.ZChangedTo(ui_depth));

        if (this.IsPointedAt() &&
            (OperationTile.IsInOperationMenu || OperationTile.IsInProgramInterface))
        {
            image.color = color.Lerped(Color.yellow, 0.5f);
            image.transform.localScale = new Vector2(1, 1) * 1.25f;
        }
        else
        {
            image.color = color;
            image.transform.localScale = new Vector2(1, 1);
        }

        if(IsOpen)
        {
            image.color = image.color.AlphaChangedTo(1);

            if(IsLineValid)
                line.gameObject.SetActive(true);
        }
        else
        {
            image.color = image.color.AlphaChangedTo(0);
            line.gameObject.SetActive(false);
        }


        //Line color
        float target_line_alpha = OperationTile.IsHighlighted || IsSelected ? 1 : 0.1f;

        LineAlpha = Mathf.Lerp(LineAlpha, target_line_alpha, Time.deltaTime * 6);

        line_color.Value = line.startColor.AlphaChangedTo(LineAlpha);


        if (OperationTile.IsSelectable && 
            !InputUtility.DidDragOccur &&
            !IsSelected &&
            this.IsPointedAt() &&
            (OperationTile.IsInOperationMenu || OperationTile.IsInProgramInterface) &&
            this.UseMouseLeftRelease())
            IsSelected = true;
    }


    static OperationTileNode selected = null;
    public static OperationTileNode Selected
    {
        get { return selected; }
        internal set { selected = value; }
    }
}
