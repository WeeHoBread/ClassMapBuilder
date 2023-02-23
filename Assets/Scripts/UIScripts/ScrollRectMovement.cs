using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectMovement : MonoBehaviour
{
    [HideInInspector]
    public ScrollRect scroll;

    private void Start()
    {
        scroll = GetComponent<ScrollRect>();
    }

    public void ScrollLeft(float scrollSpeed)
    {
        scroll.horizontalNormalizedPosition = Mathf.Clamp(scroll.horizontalNormalizedPosition -= scrollSpeed * Time.deltaTime, 0f, 1f);
    }

    public void ScrollRight(float scrollSpeed)
    { 
        scroll.horizontalNormalizedPosition = Mathf.Clamp(scroll.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime, 0f, 1f);
    }

    public void ScrollToPosition(int childIndex)
    {
        if(childIndex == 0 || childIndex == 1)
        {
            //If target content is the first two, scroll all the way to the left
            scroll.horizontalNormalizedPosition = 0;
        }
        else if (childIndex == scroll.content.transform.childCount -1 || childIndex == scroll.content.transform.childCount - 2)
        {
            //If target content is the last 2, scroll all the way to the right
            scroll.horizontalNormalizedPosition = 1;
        }
        else
        {
            //Target is somewhere in between
            float amountOfContent = scroll.content.transform.childCount;
            float newPos = 1.0f - (float)childIndex / amountOfContent;
            scroll.horizontalNormalizedPosition = newPos;
        }       
    }

}
