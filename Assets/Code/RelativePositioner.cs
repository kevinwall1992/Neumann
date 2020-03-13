using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class RelativePositioner : MonoBehaviour
{
    public RectTransform Reference;
    public bool ControlHorizontalPosition = true;
    public float HorizontalDistance;
    public bool ControlVerticalPosition = true;
    public float VerticalDistance;
    public bool MaintainCurrentDistance = false;

    bool ReferenceIsToTheLeft { get { return Reference.position.x < transform.position.x; } }
    bool ReferenceIsBelow { get { return Reference.position.y < transform.position.y; } }

    Vector2 ReferenceSidePosition
    {
        get
        {
            return Reference.TransformPoint(
                new Vector2(ReferenceIsToTheLeft ? Reference.rect.max.x : Reference.rect.min.x,
                            ReferenceIsBelow ? Reference.rect.max.y : Reference.rect.min.y));
        }
    }

    Vector2 ThisSidePosition
    {
        get
        {
            return transform.TransformPoint(
                new Vector2(ReferenceIsToTheLeft ? RectTransform.rect.min.x : RectTransform.rect.max.x,
                            ReferenceIsBelow ? RectTransform.rect.min.y : RectTransform.rect.max.y));
        }
    }

    public RectTransform RectTransform { get { return transform as RectTransform; } }

    void Start()
    {

    }

    void Update()
    {
        if (Reference == null)
            return;

        if (MaintainCurrentDistance)
        {
            HorizontalDistance = ThisSidePosition.x - ReferenceSidePosition.x;
            VerticalDistance = ThisSidePosition.y - ReferenceSidePosition.y;
        }

        float horizontal_positon = transform.position.x;
        if (ControlHorizontalPosition)
            horizontal_positon += ReferenceSidePosition.x + HorizontalDistance - ThisSidePosition.x;

        float vertical_positon = transform.position.y;
        if (ControlVerticalPosition)
            vertical_positon += ReferenceSidePosition.y + VerticalDistance - ThisSidePosition.y;

        transform.position = new Vector2(horizontal_positon, vertical_positon);
    }
}


