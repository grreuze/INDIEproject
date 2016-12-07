using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoldToQuit : MonoBehaviour {

    [SerializeField, Tooltip("The button the player must hold to quit the game.")]
    string button = "Cancel";
    [SerializeField, Tooltip("The duration the player must hold the button, in seconds.")]
    float duration = 2;

    Color clear = new Color(1, 1, 1, 0);

    Image fade;

	void Start() {
        fade = GetComponent<Image>();
        StartCoroutine(_WaitForInput());
	}
	
    IEnumerator _WaitForInput() {
        float elapsed = 0;
        while (elapsed < duration) {
            if (Input.GetButton(button)) {
                elapsed += Time.deltaTime;
                fade.color = Color.Lerp(clear, Color.white, elapsed / duration);
            }
            else {
                elapsed = 0;
                if (fade.color.a > 0)
                    fade.color = clear;
            }
            yield return null;
        }
        fade.color = Color.white;
        Application.Quit();
    }
}
