using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public float minLoadTime;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject loadingWheel;
    [SerializeField] Image fadeImage;

    [SerializeField] float wheelTurnSpeed;
    [SerializeField] float fadeTime;

    private bool isLoading;
    public bool isDataRetrieved;


    // Start is called before the first frame update
    void Start()
    {
        fadeImage.gameObject.SetActive(false);
        InitiateLoading();
    }

    public void InitiateLoading()
    {
        StartCoroutine(LoadDataRoutine());
    }

    private IEnumerator LoadDataRoutine()
    {
        isLoading = true;
        isDataRetrieved = false;

        fadeImage.gameObject.SetActive(true);
        fadeImage.canvasRenderer.SetAlpha(0);
        while(!Fade(1))
        {
            yield return null;
        }

        loadingPanel.SetActive(true);
        StartCoroutine(SpinLoadingWheel());

        while (Fade(0))
        {
            yield return null;
        }

        float elaspedLoadTime = 0;
        elaspedLoadTime += Time.deltaTime;

        while (elaspedLoadTime < minLoadTime || !isDataRetrieved)
        {
            elaspedLoadTime += Time.deltaTime;
            yield return null;
        }

        while(!Fade(1))
        {
            yield return null;
        }

        loadingPanel.SetActive(false);

        while (!Fade(0))
        {
            yield return null;
        }

        isLoading = false;
    }

    private IEnumerator SpinLoadingWheel()
    {
        while (isLoading)
        {
            loadingWheel.transform.Rotate(0, 0, -wheelTurnSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private bool Fade(float target)
    {
        fadeImage.CrossFadeAlpha(target, fadeTime, true);

        if (Mathf.Abs(fadeImage.canvasRenderer.GetAlpha() - target) <= 0.05f)
        {
            fadeImage.canvasRenderer.SetAlpha(target);
            return true;
        }

        return false;
    }

}
