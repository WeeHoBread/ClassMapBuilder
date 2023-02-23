using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchImage : MonoBehaviour
{
    public Sprite straight;
    public Sprite diagonal;

    private bool toggled;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void ToggleSprite()
    {
        if (toggled)
        {
            toggled = false;
        }
        else
        {
            toggled = true;
        }

        if (image.sprite == straight)
        {
            image.sprite = diagonal;
        }
        else
        {
            image.sprite = straight;
        }

    }

}
