using UnityEngine.InputSystem;

public static class InputActionExtensions
{
    public static void SetEnabled(this InputActionReference actionReference, bool enabled)
    {
        if (enabled)
            actionReference.EnableAction();
        else
            actionReference.DisableAction();
    }

    public static void EnableAction(this InputActionReference actionReference)
    {
        var action = actionReference.GetInputAction();
        if (action != null && !action.enabled)
            action.Enable();
    }

    public static void DisableAction(this InputActionReference actionReference)
    {
        var action = actionReference.GetInputAction();
        if (action != null && action.enabled)
            action.Disable();
    }

    public static InputAction GetInputAction(this InputActionReference actionReference)
    {
#pragma warning disable IDE0031 // Utilisation de la propagation null -- Ne pas utiliser pour les types UnityEngine.Object
        return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
    }
}