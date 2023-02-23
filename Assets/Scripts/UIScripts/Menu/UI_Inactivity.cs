using UnityEngine;
using UnityEngine.UI;

public class UI_Inactivity : MonoBehaviour
{
    public float timeToDim;
    private float nextTimeToDim;
    private bool dimmed = false;

    private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        nextTimeToDim = Time.time + timeToDim;

        defaultColor = new Color(1f, 1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTimeToDim && !dimmed)
        {
            Dim();
        }
    }

    private void Dim()
    {
        Image[] children;
        children = GetComponentsInChildren<Image>();

        foreach (Image image in children)
        {
            Color tempColor = new Color(1f, 1f, 1f, 0.2f);
            image.color = Color.Lerp(image.color, tempColor, 0.005f);

            bool allDimmed = false;

            for(int i = 0; i < children.Length; i++)
            {
                if (children[i].color != tempColor)
                {
                    allDimmed = false;
                    break;
                }
                else
                {
                    allDimmed = true;
                }
            }

            if (allDimmed)
            {
                dimmed = true;
            }

        }
        
    }

    private void SetAllChildColor(Color color)
    {
        Image[] children;
        children = GetComponentsInChildren<Image>();

        foreach (Image image in children)
        {
            image.color = color;
        }
    }



    public void ResetTimer()
    {
        nextTimeToDim = Time.time + timeToDim;
        dimmed = false;
        SetAllChildColor(defaultColor);


    }

}
