using UnityEngine;

public class PrefabManager : MonoBehaviour {

    [Header("Main Prefabs")]
    [SerializeField]
    Star _star;
    [SerializeField]
    Prism _prism;
    [SerializeField]
    Link _link;

    public static Star star;
    public static Prism prism;
    public static Link link;

    [Header("Particle Systems")]
    [SerializeField]
    GameObject _starCreationParticles;
    [SerializeField]
    CircleSize _circleParticle;

    public static GameObject starCreationParticles;
    public static CircleSize circleParticle;

    void Awake() {
        star = _star;
        prism = _prism;
        link = _link;

        starCreationParticles = _starCreationParticles;
        circleParticle = _circleParticle;
    }
}
