using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene: MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadSlider;

    private int sceneIndex = -1;
    private bool loadingStartet = false;

    public void PlayDirector(int _sceneIndex)
    {
        if (!loadingStartet)
        {
            loadingStartet = true;
            sceneIndex = _sceneIndex;
            director.Play();
        }
    }

    public void LoadAScene()
    {
        if (sceneIndex > -1)
        {
            Debug.Log("load scene from index: " + sceneIndex);
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void LoadSpecificScene(int sceneIndex)
    {
        Debug.Log("load scene from index: " + sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneAsync(int sceneIndex)
    {
        StartCoroutine(LoadAsynchron(sceneIndex));
    }

    IEnumerator LoadAsynchron(int index)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(index);
        loadingScreen.SetActive(true);

        while (!load.isDone)
        {
            float progress = Mathf.Clamp01(load.progress / 0.9f);
            loadSlider.value = progress;
            //Debug.Log(progress);
            yield return null;
        }
    }


}