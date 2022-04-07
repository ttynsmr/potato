using UnityEngine;
using UnityEngine.UI;

public class CharBackground : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public Color Color
    {
        get
        {
            return image.color;
        }
        set
        {
            image.color = value;
        }
    }
}
