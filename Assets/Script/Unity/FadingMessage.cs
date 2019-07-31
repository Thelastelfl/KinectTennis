using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingMessage : MonoBehaviour {

    float DURATION = 2.5f;
    public GameObject gameObject;
    private Text info;

    void Start() {
        info = gameObject.GetComponent<Text>();
    }

    public void Fading(float time,string text)
    {
        if (time > DURATION)
        {
            gameObject.SetActive(false);
        }
        info.text = text;
        Color newColor = info.color;
        float proportion = (time / DURATION);
        newColor.a = Mathf.Lerp(1, 0, proportion);
        info.color = newColor;
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.SetActive(true);
        }
    }
}
