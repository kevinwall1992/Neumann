using UnityEngine;
using System.Collections;

public static class InputUtility
{
    public enum MouseButton { Left, Right, Middle }

    public static bool WasMouseLeftPressed { get { return Input.GetMouseButtonDown((int)MouseButton.Left); } }
    public static bool IsMouseLeftPressed { get { return Input.GetMouseButton((int)MouseButton.Left); } }
    public static bool WasMouseLeftReleased { get { return Input.GetMouseButtonUp((int)MouseButton.Left); } }

    public static bool WasMouseRightPressed { get { return Input.GetMouseButtonDown((int)MouseButton.Right); } }
    public static bool IsMouseRightPressed { get { return Input.GetMouseButton((int)MouseButton.Right); } }
    public static bool WasMouseRightReleased { get { return Input.GetMouseButtonUp((int)MouseButton.Right); } }

    public static bool IsDragOccurring { get { return Scene.Main.InputModule.IsDragOccurring; } }
    public static bool DidDragOccur { get { return Scene.Main.InputModule.DidDragOccur; } }

    public static bool DidMouseMove { get { return Scene.Main.InputModule.DidMouseMove; } }


    static MonoBehaviour keyboard_input_claimant = null;
    public static bool ClaimKeyboardInput(this MonoBehaviour claimant)
    {
        if (!claimant.CanUseKeyboardInput())
            return false;

        keyboard_input_claimant = claimant;
        return true;
    }

    public static void YieldKeyboardInputClaim(this MonoBehaviour claimant)
    {
        if (keyboard_input_claimant == claimant)
            keyboard_input_claimant = null;
    }

    public static bool CanUseKeyboardInput(this MonoBehaviour enquirer)
    {
        return keyboard_input_claimant == null ||
               keyboard_input_claimant == enquirer;
    }


    static MonoBehaviour mouse_left_release_claimant = null;
    static int mouse_left_release_claim_yield_frame = -1;
    static MonoBehaviour MouseLeftReleaseClaimant
    {
        get
        {
            if (mouse_left_release_claim_yield_frame >= 0 &&
               mouse_left_release_claim_yield_frame < Time.frameCount)
                mouse_left_release_claimant = null;

            return mouse_left_release_claimant;
        }

        set
        {
            if (MouseLeftReleaseClaimant == null ||
                MouseLeftReleaseClaimant == value)
            {
                mouse_left_release_claimant = value;
                mouse_left_release_claim_yield_frame = -1;
            }
        }
    }

    public static bool ClaimMouseLeftRelease(this MonoBehaviour claimant)
    {
        if (MouseLeftReleaseClaimant != null)
            return false;

        MouseLeftReleaseClaimant = claimant;

        return true;
    }

    public static void YieldMouseLeftReleaseClaim(this MonoBehaviour claimant)
    {
        if (MouseLeftReleaseClaimant == claimant)
            mouse_left_release_claim_yield_frame = Time.frameCount;
    }

    public static bool UseMouseLeftRelease(this MonoBehaviour enquirer)
    {
        if (!WasMouseLeftReleased)
            return false;

        if (MouseLeftReleaseClaimant != null)
        {
            if (MouseLeftReleaseClaimant == enquirer)
            {
                YieldMouseLeftReleaseClaim(enquirer);
                return true;
            }

            return false;
        }

        return true;
    }

    public static Vector2 GetMouseMotion()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public static bool IsPointedAt(this GameObject game_object)
    {
        if (Scene.Main.InputModule.IsHovered(game_object))
            return true;

        if (game_object.transform is RectTransform)
            return (game_object.transform as RectTransform).ContainsMouse();

        Collider collider = game_object.GetComponent<SphereCollider>();
        if (collider != null)
        {
            RaycastHit hit_info;
            return collider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition),
                                    out hit_info,
                                    10000);
        }

        return false;
    }

    public static bool IsPointedAt(this MonoBehaviour mono_behaviour)
    {
        return mono_behaviour.gameObject.IsPointedAt();
    }

    public static bool IsPointedAt(this Transform transform)
    {
        return transform.gameObject.IsPointedAt();
    }

    public static T GetElementTouched<T>() where T : class
    {
        if (Scene.Main.InputModule.ElementTouched == null)
            return null;

        return Scene.Main.InputModule.ElementTouched.GetComponentInParent<T>();
    }

    public static bool IsTouched(this GameObject game_object)
    {
        if (Scene.Main.InputModule.ElementTouched == null)
            return false;

        return Scene.Main.InputModule.ElementTouched.DescendsFrom(game_object);
    }

    public static bool IsTouched(this MonoBehaviour mono_behaviour)
    {
        return mono_behaviour.gameObject.IsTouched();
    }

    public static bool IsTouched(this Transform transform)
    {
        return transform.gameObject.IsTouched();
    }

    public static bool Contains(this RectTransform rect_transform, Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect_transform, position);
    }

    public static bool ContainsMouse(this RectTransform rect_transform)
    {
        return rect_transform.Contains(Input.mousePosition);
    }


    //This should only be called from OnGUI()
    public static bool ConsumeIsKeyUp(KeyCode key_code)
    {
        if (Event.current.isKey && Event.current.keyCode == key_code && Input.GetKeyUp(key_code))
        {
            Event.current.Use();

            return true;
        }

        return false;
    }
}
