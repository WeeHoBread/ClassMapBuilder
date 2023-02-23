using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos3D : MonoBehaviour
{
    public static MousePos3D Instance { get; private set; }

    private Camera mainCamera;
    LayerMask layerMask;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        layerMask = LayerMask.GetMask("Ground");
    }

    public static Vector3 GetMousePositition3D() => Instance.GetMousePosition_Instance();

    private Vector3 GetMousePosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
        {
            return raycastHit.point;
        }
        return new Vector3 (999,999,999);
    }

}
