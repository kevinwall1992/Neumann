using UnityEngine;

[System.Serializable]
public class Cursor
{
    public Texture2D Texture;
    public Vector2 Hotspot;

    public Cursor(Texture2D texture, Vector2 hotspot)
    {
        Texture = texture;
        Hotspot = hotspot;
    }

    public Cursor(string texture_name, Vector2 hotspot)
    {
        Texture = Resources.Load<Texture2D>(texture_name);
        Hotspot = hotspot;
    }

    public void Use()
    {
        UnityEngine.Cursor.SetCursor(Texture, Hotspot, CursorMode.Auto);
        Current = this;
    }


    public static Cursor Default { get { return Scene.Main.Style.DefaultCursor; } }
    public static Cursor Current { get; private set; }
}
