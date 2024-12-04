#nullable enable

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public static class Utils {
    public static bool TryGetGameObject(
        this Object obj,
        [NotNullWhen(true)] out GameObject? gameObject) {
        switch (obj) {
            case GameObject gObj:
                gameObject = gObj;
                return true;
            case Component component:
                gameObject = component.gameObject;
                return true;
            default:
                gameObject = null;
                return false;
        }
    }
}
