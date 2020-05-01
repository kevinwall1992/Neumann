using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    BezierLineController bezier_line_controller = null;
    protected BezierLineController BezierLineController { get { return bezier_line_controller; } }

    protected bool Hide { get; set; } = true;

    public OperationTile OperationTile { get { return GetComponentInParent<OperationTile>(); } }

    public bool IsSelected
    {
        get { return Selected == this; }
        set
        {
            if (value)
            {
                Selected = this;
                this.ClaimMouseLeftRelease();
            }
            else
            {
                if (Selected == this)
                    Selected = null;

                this.YieldMouseLeftReleaseClaim();
            }
        }
    }

    protected virtual void Start()
    {
        color = image.color;
    }

    protected virtual void Update()
    {
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

        if(Hide)
        {
            image.color = image.color.AlphaChangedTo(0);
            line.gameObject.SetActive(false);
        }

        if (!InputUtility.DidDragOccur &&
            !IsSelected &&
            this.IsPointedAt() &&
            (OperationTile.IsInOperationMenu || OperationTile.IsInProgramInterface) &&
            this.UseMouseLeftRelease())
            IsSelected = true;
    }

    protected void HideLine()
    {
        line.gameObject.SetActive(false);
    }
    protected void ShowLine()
    {
        line.gameObject.SetActive(true);
    }


    static OperationTileNode selected = null;
    public static OperationTileNode Selected
    {
        get { return selected; }
        internal set { selected = value; }
    }
}
