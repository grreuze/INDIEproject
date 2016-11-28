using UnityEngine;

public class Prism : Element {

    #region Properties

    [Header("Prism Properties")]
    public float position;

    /// <summary>
    /// The Link the Prism is currently attached to, if any.
    /// </summary>
    Link attachedLink;
    Star targetedStar;

    #endregion

    #region MonoBehaviour Methods

    void Awake() {
        existence = Existence.unique;
    }

    void LateUpdate() {
        if (attachedLink) {
            transform.position = Vector3.Lerp(attachedLink.parent.transform.position, attachedLink.target.transform.position, position);
            if (targetedStar)
                transform.LookAt(targetedStar.transform);
        }
    }

    #endregion

    public override void StartHold() {
        if (targetedStar) DetachLink();
        base.StartHold();
    }

    public override void StopHold() {
        attachedLink = Mouse.hover ? Mouse.hover.GetComponent<Link>() : null;
        if (attachedLink)
            AttachToLink();
        base.StopHold();
    }

    void AttachToLink() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            transform.position = hit.point;

        position = Vector3.Distance(transform.position, attachedLink.parent.transform.position) / attachedLink.length;

        targetedStar = position < 0.5f ? (Star)attachedLink.parent : (Star)attachedLink.target;
        
        Star oppositeStar = position < 0.5f ? (Star)attachedLink.target : (Star)attachedLink.parent;

        transform.LookAt(targetedStar.transform);
        
        targetedStar.chroma += chroma;
        targetedStar.chroma += oppositeStar.chroma;
        targetedStar.ApplyChroma();
    }

    void DetachLink() {
        if (targetedStar.isActive && attachedLink) {
            targetedStar.chroma -= chroma;
            targetedStar.ApplyChroma();
        }
        targetedStar = null;
        attachedLink = null;
    }

}
