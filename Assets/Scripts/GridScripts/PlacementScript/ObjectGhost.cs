using UnityEngine;

public class ObjectGhost : MonoBehaviour
{
    private Transform visual;
    private int objectGhostVariantNo;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material errorMaterial;
    
    //Start is called before the first frame update
        private void Start()
    {
        RefreshVisual();
        SetMaterial(baseMaterial);
        GridBuilder.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
        GridBuilder.Instance.OnVariantChanged += Instance_OnSelectedChanged;
        GridBuilder.Instance.OnBuildError += Instance_OnBuildError;
        GridBuilder.Instance.OnBuildable += Instance_OnBuildable;

    }

    #region Instance subscribed events
    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    private void Instance_OnBuildError(object sender, System.EventArgs e)
    {
        SetMaterial(errorMaterial);
    }

    private void Instance_OnBuildable(object sender, System.EventArgs e)
    {
        SetMaterial(baseMaterial);
    }
    #endregion

    private void LateUpdate()
    {
        Vector3 targetPosition = GridBuilder.Instance.GetMouseWorldSnappedPosition();
        if(MousePos3D.GetMousePositition3D().y == 999)
        {
            targetPosition.y = 5f;
        }
        else
        {
            targetPosition.y = 1f;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.GetChild(0).transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).transform.GetChild(0).rotation, GridBuilder.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);


    }

    #region Set visual change
    public void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = GridBuilder.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null)
        {
            //objectGhostVariantNo = GridBuilder.Instance.GetCurrentVariantIndex();
            Transform tempGhost = placedObjectTypeSO.ghost;
            SetActiveGhostVariant(objectGhostVariantNo, tempGhost);
            visual = Instantiate(placedObjectTypeSO.ghost, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetMaterial(Material nextMat)
    {
        Renderer[] children;

        children = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mat = new Material[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                mat[i] = nextMat;
            }
            rend.materials = mat;
        }

    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private void SetActiveGhostVariant(int variantNo, Transform ghostTransform)
    {
        //Cycle through children set all to in active
        for (int i = 0; i < ghostTransform.childCount; i++)
        {
            ghostTransform.GetChild(i).gameObject.SetActive(false);
        }

        //Set specific child to be active
        ghostTransform.GetChild(variantNo).gameObject.SetActive(true);
    }
    #endregion

}
