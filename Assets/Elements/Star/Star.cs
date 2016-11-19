﻿using UnityEngine;

public class Star : Element {

    #region Properties
    
    [Header("Star Properties")]
    public Material mat;
    public bool anchored;
    public int id;
    
    [SerializeField]
    Material hoverMat = null;

    Renderer rend;
    Star[] clones;
    
    #endregion

    #region MonoBehaviour Methods

    void Start() {
        rend = GetComponent<Renderer>();
        rend.sharedMaterial = mat;
        Init();
        GetAllClones();
    }

    void OnMouseEnter() {
        if (!isHeld && !Input.GetMouseButton(0) && Mouse.holding == null) {
            hovered = true;
            Mouse.hover = this;
            rend.sharedMaterial = hoverMat;
        }
    }

    void OnMouseExit() {
        if (!isHeld) StopHold();
    }

    void Update() {
        Rescale(); // We should only rescale if zooming / dezooming / anchored
        CheckHeld();
        CheckLink();

        if (Mathf.Abs(transform.localPosition.x) > 1000)
            transform.localPosition /= 10000;
        if (Mathf.Abs(transform.localPosition.x) < 0.001)
            transform.localPosition *= 10000;
    }

    #endregion

    #region Holding Methods
        
    void CheckHeld() {
        if (isHeld) {
            if (Mouse.holding != this) {
                Mouse.holding = this;
                transform.parent = null;
                AnchorClones();
            }
            MoveClones();
            float screenDepth = Camera.main.WorldToScreenPoint(transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenDepth));
            UpdateLinks();
        } else if (Mouse.holding == this) StopHold();
    }

    void StopHold() {
        hovered = false;
        Mouse.hover = null;
        rend.sharedMaterial = mat;
        if (Mouse.holding == this) {
            Mouse.holding = null;
            ChangeInstance();
            ReleaseClones();
        }
    }

    void ChangeInstance() {
        int diff = wrapper.currentInstance.id - worldInstance.id;
        if (diff == 0)
            transform.parent = worldTransform;
        else {
            SetNewInstance(diff);
            SetNewCloneInstance(diff);
            UpdateLinks();
        }
    }

    public override void SetNewInstance(int diff) {
        base.SetNewInstance(diff);
        worldInstance.stars[id] = this;
    }

    #endregion

    #region CloneMethods

    void GetAllClones() {
        clones = new Star[wrapper.worldInstances.Count - 1];

        for (int i = 0, j = 0; i < clones.Length + 1; i++, j++) {
            if (i == worldInstance.id) {
                j--;
                continue;
            }
            Star clone = wrapper.worldInstances[i].stars[id];
            clones[j] = clone;
        }
    }

    void AnchorClones() {
        foreach (Star clone in clones) {
            clone.anchored = true;
        }
    }

    void MoveClones() {
        foreach (Star clone in clones) {
            clone.transform.localPosition = worldTransform.InverseTransformPoint(transform.position);
        }
    }

    void ReleaseClones() {
        foreach (Star clone in clones) {
            clone.anchored = false;
        }
    }

    void SetNewCloneInstance(int diff) {
        foreach (Star clone in clones) {
            clone.SetNewInstance(diff);
        }
    }

    #endregion

}