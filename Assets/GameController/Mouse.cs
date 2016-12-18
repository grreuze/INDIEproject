using UnityEngine;

public static class Mouse {

    /// <summary>
    /// The element that the player is currently hovering, if any.
    /// </summary>
    public static MonoBehaviour hover;

    /// <summary>
    /// The element that the player is currently holding, if any.
    /// </summary>
    public static Element holding;

    /// <summary>
    /// The element that the player is currently linking, if any.
    /// </summary>
    public static Element linking;

    /// <summary>
    /// The link tied to the mouse, if any.
    /// </summary>
    public static Link link;

    public static bool breakLinkMode;

    public static bool isHoldingPrism {
        get { return holding && holding.GetComponent<Prism>(); }
    }

    /// <summary>
    /// Breaks the link tied to the mouse.
    /// </summary>
    public static void BreakLink() {
        linking = null;
        if (link) MonoBehaviour.Destroy(link.gameObject); // We should probably do object pooling for the links
    }

}
