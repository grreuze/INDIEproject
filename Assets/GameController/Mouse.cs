using UnityEngine;

public static class Mouse {

    /// <summary>
    /// The star that the player is currently hovering, if any.
    /// </summary>
    public static Star hover;

    /// <summary>
    /// The star that the player is currently holding, if any.
    /// </summary>
    public static Star holding;

    /// <summary>
    /// The star that the player is currently linking, if any.
    /// </summary>
    public static Star linking;

    /// <summary>
    /// The link tied to the mouse, if any.
    /// </summary>
    public static Link link;

    public static bool breakLinkMode;

    /// <summary>
    /// Breaks the link tied to the mouse.
    /// </summary>
    public static void BreakLink() {
        linking = null;
        if (link) MonoBehaviour.Destroy(link.gameObject); // We should probably do object pooling for the links
    }

}
