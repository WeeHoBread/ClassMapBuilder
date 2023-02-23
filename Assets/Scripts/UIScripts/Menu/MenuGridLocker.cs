using UnityEngine;

public class MenuGridLocker : MonoBehaviour
{
    private void OnEnable()
    {
        GridBuilder.Instance.DisableBuild();
    }

    private void OnDisable()
    {
        //if (FindObjectOfType<GridBuilder>().enabled == false)
        //{
        //    FindObjectOfType<GridBuilder>().enabled = true;
        //}
        GridBuilder.Instance.EnableBuild();
    }

}
