using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class Drawer : UIElement
{
    //"rows" in this class are tiles that appear together
    //
    //  X X X <- off-screen "row"
    //  - - - <- edge of screen
    //  X X X <- on-screen "row"
    //
    //     v- off-screen "row"
    //     X | X
    //     X | X
    //     X | X
    // edge -^ ^- on-screen "row"

    int insertion_preview_index = -1;

    bool IsFrontOnLeft { get; set; }
    bool IsFrontAtBottom { get; set; }

    public bool IsVertical;
    public int ElementsPerRow = 6;
    public int RowsVisible = 1;
    public int MaximumRowsVisible = 4;
    public DrawerHandle Handle;
    public Transform SpawnPosition;

    public bool HasHandle { get { return Handle != null; } }
    public RectTransform RectTransform { get { return transform as RectTransform; } }

    public List<Tile> Tiles
    {
        get { return GetComponentsInChildren<Tile>().ToList(); }
    }

    protected override void Start()
    {
        base.Start();

        Reset();


        IsFrontOnLeft = Mathf.Abs(RectTransform.rect.min.x) < Mathf.Abs(RectTransform.rect.max.x);
        IsFrontAtBottom = Mathf.Abs(RectTransform.rect.min.y) < Mathf.Abs(RectTransform.rect.max.y);
    }

    protected override void Update()
    {
        base.Update();

        RowsVisible = Mathf.Min(MaximumRowsVisible, RowsVisible);


        List<Tile> tiles = new List<Tile>(Tiles);

        if (this.IsModulusUpdate(10))
            foreach (Tile tile in tiles)
                tile.IsPositioned = false;

        foreach (Tile tile in tiles)
        {
            if (tile.IsPositioned || tile.IsBeingDragged)
                continue;

            int index = tiles.IndexOf(tile);
            if (insertion_preview_index >= 0 && 
                index >= insertion_preview_index)
                index++;

            int column = index % ElementsPerRow;
            int row = index / ElementsPerRow;

            Vector2 starting_position = GetStartingPosition();
            Vector2 direction = GetDirection();
            
            starting_position += direction / 2;

            float separation = Scene.Main.Style.TileSize + Scene.Main.Style.Padding;

            Vector3 tile_target_position = 
                starting_position + 
                new Vector2(direction.x * (IsVertical ? column : row), 
                            direction.y * (IsVertical ? row : column));

            tile.transform.position = Vector3.Lerp(tile.transform.position, tile_target_position, 4 * Time.deltaTime);
            if (tile.transform.position.Distance(tile_target_position) < 0.5f)
                tile.IsPositioned = true;
        }


        RectTransform parent_transform = transform.parent as RectTransform;
        float effective_rows_visible = 0;
        if (tiles.Count() > 0)
        {
            int effective_tile_count = tiles.Count() + (insertion_preview_index >= 0 ? 1 : 0);

            effective_rows_visible = Mathf.Min(RowsVisible, Mathf.Max(1, 
                (effective_tile_count - 1) / ElementsPerRow + 1));
        }

        //Does column axis grow in value from front to back?
        bool column_axis_grows = IsVertical ? IsFrontAtBottom : IsFrontOnLeft;
        
        Vector2 edge_position;
        if (column_axis_grows)
            edge_position = parent_transform.TransformPoint(parent_transform.rect.max);
        else
            edge_position = parent_transform.TransformPoint(parent_transform.rect.min);

        Vector2 target_position = transform.position;
        float tile_offset_unit = (column_axis_grows ? -1 : 1) * (Scene.Main.Style.TileSize + Scene.Main.Style.Padding);
        float offset = effective_rows_visible * tile_offset_unit;
        if (IsVertical)
            target_position.y =  offset + edge_position.y;
        else
            target_position.x = offset + edge_position.x;

        if(!HasHandle || !Handle.IsBeingDragged)
            transform.position = Vector3.Lerp(transform.position, target_position, 4 * Time.deltaTime);


        if (HasHandle)
        {
            if (Handle.IsBeingDragged)
            {
                Vector3 displacement = Input.mousePosition - Handle.transform.position;

                Vector2 drag_mask;
                if (IsVertical)
                    drag_mask = new Vector3(0, 1);
                else
                    drag_mask = new Vector3(1, 0);

                transform.position += Vector3.Scale(displacement, drag_mask);


                Vector2 visible_row_units = ((Vector2)transform.position - edge_position) / tile_offset_unit;
                RowsVisible = Mathf.Max(0, (int)((IsVertical ? visible_row_units.y : visible_row_units.x) + 0.5f));
            }
        }
    }

    protected Vector2 GetStartingPosition()
    {
        RectTransform rect_transform = transform as RectTransform;

        return transform.TransformPoint(
            new Vector2((IsFrontOnLeft ? rect_transform.rect.min : rect_transform.rect.max).x,
                        (IsFrontAtBottom ? rect_transform.rect.min : rect_transform.rect.max).y));
    }

    protected Vector2 GetDirection()
    {
        return new Vector2(IsFrontOnLeft ? 1 : -1,
                           IsFrontAtBottom ? 1 : -1) *
            (Scene.Main.Style.TileSize + Scene.Main.Style.Padding);
    }

    protected int PositionToInsertionIndex(Vector2 position)
    {
        Vector2 indices = (position - GetStartingPosition()) / GetDirection() - new Vector2(0.5f, 0.5f);

        return Mathf.Max(Mathf.Min((int)indices.x * (IsVertical ? 1 : ElementsPerRow) + 
                                   (int)indices.y * (IsVertical ? ElementsPerRow : 1), Tiles.Count), 0);
    }

    public virtual Tile Add(Tile tile)
    {
        tile.transform.SetParent(this.transform);

        StopPreviewing();

        return tile;
    }

    public virtual Tile Remove(Tile tile)
    {
        tile.transform.SetParent(null);

        StopPreviewing();

        return tile;
    }

    public void PreviewAt(Vector2 position)
    {
        insertion_preview_index = PositionToInsertionIndex(position);
    }

    public void StopPreviewing()
    {
        insertion_preview_index = -1;
    }

    public virtual Tile AddAt(Tile tile, Vector2 position)
    {
        Add(tile);
        tile.transform.SetSiblingIndex(PositionToInsertionIndex(position));

        return tile;
    }

    public virtual void Reset()
    {
        if (!UnityEditor.EditorApplication.isPlaying)
            return;

        foreach (Tile tile in new List<Tile>(Tiles))
            GameObject.Destroy(tile.gameObject);
    }
}
