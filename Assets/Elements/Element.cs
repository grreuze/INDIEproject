using UnityEngine;

public abstract class Element : MonoBehaviour {

    [SerializeField, Header("Element Properties")]
    float scaleFactor = 0.2f;
    [SerializeField]
    MinMax scale;

    Vector3 GetParentScale() {
        Vector3 sf = Vector3.one;
        Transform parent = transform.parent ?? null;

        while (parent) {
            sf.x *= 1 / parent.localScale.x;
            sf.y *= 1 / parent.localScale.y;
            sf.z *= 1 / parent.localScale.z;

            parent = parent.parent ?? null;
        }
        return sf;
    }

    /// <summary>
    /// Rescales an element depending on its parent's scale.
    /// </summary>
    public void Rescale() {
        Vector3 parentScale = GetParentScale();
        Vector3 localScale = Vector3.one * Vector3.Distance(Vector3.zero, transform.position) * scaleFactor;
        localScale.x *= parentScale.x;
        localScale.y *= parentScale.y;
        localScale.z *= parentScale.z;

        localScale.x = Mathf.Clamp(localScale.x, scale.min * parentScale.x, scale.max * parentScale.x);
        localScale.y = Mathf.Clamp(localScale.y, scale.min * parentScale.y, scale.max * parentScale.y);
        localScale.z = Mathf.Clamp(localScale.z, scale.min * parentScale.z, scale.max * parentScale.z);

        transform.localScale = localScale;
    }
}
