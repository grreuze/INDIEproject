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
        
        if (targetedStar.chroma.isPure || formerChroma.isPure) { // if a pure color is involved, the prisms' colors might be getting overwritten
            // add one of the color of all the prisms going to target
            if (position < 0.5f) {
                foreach (Prism prism in attachedLink.prismToOrigin) {
                    empty += prism.chroma;
                }
            } else {
                foreach (Prism prism in attachedLink.prismToTarget) {
                    empty += prism.chroma;
                }
            }
            empty -= 1;
            empty.Floor();
            print("v fixed v");
        }
        empty.ReBalance();
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
        
        bool isFirstPrism = false;
        if (position < 0.5f) {
            targetedStar = (Star)attachedLink.parent;
            oppositeStar = (Star)attachedLink.target;
            attachedLink.prismToOrigin.Add(this);
            isFirstPrism = attachedLink.prismToOrigin[0] == this;
        } else {
            targetedStar = (Star)attachedLink.target;
            oppositeStar = (Star)attachedLink.parent;
            attachedLink.prismToTarget.Add(this);
            isFirstPrism = attachedLink.prismToTarget[0] == this;
        }
        transform.LookAt(targetedStar.transform);
        
        if (isFirstPrism) {
            Debug.Log(targetedStar.chroma + " + " + chroma + " + " + oppositeStar.chroma);
            targetedStar.chroma += oppositeStar.chroma;
        } else
            Debug.Log(targetedStar.chroma + " + " + chroma);
        targetedStar.chroma += chroma;

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
        if (position < 0.5f && attachedLink) attachedLink.prismToOrigin.Remove(this);
        else attachedLink.prismToTarget.Remove(this);

        targetedStar = null;
        attachedLink = null;
    }

    #endregion

}
