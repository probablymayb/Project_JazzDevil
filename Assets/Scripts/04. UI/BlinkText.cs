using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 1f;
    private Text myText;

    void Start()
    {
        myText = GetComponent<Text>();
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        Color color = myText.color;
        color.a = alpha;
        myText.color = color;
    }
}
