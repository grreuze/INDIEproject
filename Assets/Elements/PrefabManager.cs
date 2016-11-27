using UnityEngine;

public class PrefabManager : MonoBehaviour {

    [SerializeField]
    Star _star;
    [SerializeField]
    Prism _prism;
    [SerializeField]
    Link _link;

    public static Star star;
    public static Prism prism;
    public static Link link;

    void Awake() {
        star = _star;
        prism = _prism;
        link = _link;
    }
}
