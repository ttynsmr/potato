using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharForeground : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public char Character
    {
        get
        {
            return text.text.FirstOrDefault();
        }
        set
        {
            text.text = value.ToString();
        }
    }

    public Color Color
    {
        get
        {
            return text.color;
        }
        set
        {
            text.color = value;
        }
    }
}
