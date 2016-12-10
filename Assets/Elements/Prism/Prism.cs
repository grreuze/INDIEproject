using UnityEngine;
using System.Collections;

public class Prism : Element {

    #region Properties

    [Header("Prism Properties")]
    public float position;

    /// <summary>
    /// The Link the Prism is currently attached to, if any.
    /// </summary>
    public Link attachedLink;
    Star targetedStar, oppositeStar;
    Chroma formerChroma;

    #endregion

    #region MonoBehaviour Methods

    void Awake() {
        existence = Existence.unique;
    }

    void LateUpdate() {
        if (attachedLink) {
            transform.position = Vector3.Lerp(attachedLink.origin.transform.position, attachedLink.target.transform.position, position);
            if (targetedStar)
                transform.LookAt(targetedStar.transform);
        }
    }

    #endregion

    #region Holding Methods

    public override void StartHold() {
        if (targetedStar) DetachLink();
        base.StartHold();
        StartCoroutine("CheckShake");
    }

    public override void StopHold() {
        attachedLink = Mouse.hover ? Mouse.hover.GetComponent<Link>() : null;
        if (attachedLink)
            AttachToLink();
        base.StopHold();
        StopCoroutine("CheckShake");
    }
    
    IEnumerator CheckShake() {
        bool isShaking = false;
        int shakeRequired = 6; // Number of mouse x direction change required
        int delay = 2; // Delay allowed to shake, in seconds
        float shakeTime = Time.time + delay;
        bool previousMove = false;
        bool currentMove = false;
        float minAmplitude = 0.3f;

        while (isShaking == false && Time.time < shakeTime) {

            float mouseX = Input.GetAxis("Mouse X");
            currentMove = Mathf.Sign(mouseX) > 0;

            if (Mathf.Abs(mouseX) > minAmplitude) {
                if (previousMove != currentMove) {
                    // Feedback secouer
                    previousMove = currentMove;
                    shakeRequired--;
                }
                if (shakeRequired == 0)
                    isShaking = true;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (isShaking) {
            SetNextChroma();
            VertexPing();     
        }
        ResetCheckShake();
    }

    void SetNextChroma() {
        if (chroma == Chroma.red)
            chroma = Chroma.green;
        else if (chroma == Chroma.green)
            chroma = Chroma.blue;
        else if (chroma == Chroma.blue)
            chroma = Chroma.red;
        ApplyChroma();
    }

    void ResetCheckShake() {
        StopCoroutine("CheckShake");
        StartCoroutine("CheckShake");
    }

    #endregion

    #region Color Methods

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

    #endregion

    #region Link Methods

    void AttachToLink() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            transform.position = hit.point;

        position = Vector3.Distance(transform.position, attachedLink.origin.transform.position) / attachedLink.length;
        
        bool isFirstPrism = false;
        if (position < 0.5f) {
            targetedStar = (Star)attachedLink.origin;
            oppositeStar = (Star)attachedLink.target;
            attachedLink.prismToOrigin.Add(this);
            isFirstPrism = attachedLink.prismToOrigin[0] == this;
        } else {
            targetedStar = (Star)attachedLink.target;
            oppositeStar = (Star)attachedLink.origin;
            attachedLink.prismToTarget.Add(this);
            isFirstPrism = attachedLink.prismToTarget[0] == this;
        }
        transform.LookAt(targetedStar.transform);

        targetedStar.chroma.Maximize();
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
            Chroma chromaToLose = chroma;

            if (position < 0.5f && attachedLink.prismToOrigin.Count == 1
             || position > 0.5f && attachedLink.prismToTarget.Count == 1) {
                chromaToLose += oppositeStar.chroma;
                chromaToLose.ReBalance();
                targetedStar.chroma.ReBalance();
                Debug.Log(targetedStar.chroma + " - " + chromaToLose);
                targetedStar.chroma -= chromaToLose;
            }
            else {
                chromaToLose = (0 - chroma).rebalanced;
                targetedStar.chroma.ReBalance();
                Debug.Log(targetedStar.chroma + " + " + chromaToLose);
                targetedStar.chroma += chromaToLose;
            }
            targetedStar.ApplyChroma();
        }
        if (attachedLink) {
            if (position < 0.5f) attachedLink.prismToOrigin.Remove(this);
            else attachedLink.prismToTarget.Remove(this);
        }

        targetedStar = null;
        attachedLink = null;
    }

    #endregion

}
