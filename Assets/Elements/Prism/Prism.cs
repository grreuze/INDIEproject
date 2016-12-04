using UnityEngine;
using System.Collections;

public class Prism : Element {

    #region Properties

    [Header("Prism Properties")]
    public float position;

    /// <summary>
    /// The Link the Prism is currently attached to, if any.
    /// </summary>
    Link attachedLink;
    Star targetedStar, oppositeStar;
    Chroma formerChroma;

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

    #region Holding Methods

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

    #endregion

    public void UpdateTargetColor() {
        if (formerChroma != oppositeStar.chroma) {
            targetedStar.chroma.Maximize();
            formerChroma.Maximize();
            oppositeStar.chroma.Maximize();
            StartCoroutine(_UpdateColor());
        }
    }

    IEnumerator _UpdateColor() {
        yield return new WaitForSeconds(0.1f);
        Chroma empty = targetedStar.chroma - formerChroma;
        empty.ReBalance(); // Lots of things wrong in chroma color calculation still...
        Debug.Log(targetedStar.chroma + " - " + formerChroma + " = " + empty + " + " + oppositeStar.chroma);

        targetedStar.chroma = empty + oppositeStar.chroma;
        targetedStar.ApplyChroma();
        formerChroma = oppositeStar.chroma;
        formerChroma.ReBalance();
    }

    #region Link Methods

    void AttachToLink() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            transform.position = hit.point;

        position = Vector3.Distance(transform.position, attachedLink.parent.transform.position) / attachedLink.length;

        targetedStar = position < 0.5f ? (Star)attachedLink.parent : (Star)attachedLink.target;
        oppositeStar = position < 0.5f ? (Star)attachedLink.target : (Star)attachedLink.parent;

        transform.LookAt(targetedStar.transform);

        oppositeStar.prisms.Add(this);

        Debug.Log(targetedStar.chroma + " + " + chroma + " + " + oppositeStar.chroma);

        targetedStar.chroma += chroma;
        targetedStar.chroma += oppositeStar.chroma; // we shouldn't be adding this for every prism on a same link
        targetedStar.ApplyChroma();

        formerChroma = oppositeStar.chroma;
    }

    void DetachLink() {
        if (targetedStar.isActive && attachedLink) {
            Chroma chromaToLose = chroma + oppositeStar.chroma;
            chromaToLose.ReBalance();
            targetedStar.chroma.ReBalance();
            Debug.Log(targetedStar.chroma + " - " + chromaToLose);
            targetedStar.chroma -= chromaToLose;
            targetedStar.ApplyChroma();
        }
        oppositeStar.prisms.Remove(this);
        targetedStar = null;
        attachedLink = null;
    }

    #endregion

}
