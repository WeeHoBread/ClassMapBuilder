using UnityEngine;

public class EmptyGridCollider : MonoBehaviour
{
    public GameObject manager;
    public GameManager gm;

    // Update is called once per frame
    void Update()
    {
        if (gm == null)
        {
            manager = GameObject.FindGameObjectWithTag("Manager");
            gm = manager.GetComponent<GameManager>();
        }

        if (gm.isPlaying == false)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
