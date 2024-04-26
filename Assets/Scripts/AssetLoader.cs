using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AssetLoader : MonoBehaviour
{
    public GameObject[] hideObjects;
    public GameObject playerParts;
    public Animator playerAnimator;
    public Slider progressSlider;
    [Range(0f, 100f)] public float progress = 0f;

    public float sleepAfterFinish = 1f;

    private int partsCount;
    private float previousProgress = 0f;
    private float targetProgress = 1f;
    private int sceneToLoad;

    private void Start()
    {
        partsCount = playerParts.transform.childCount;
        initPlayerPrefs();
    }

    private void Update()
    {
        targetProgress = Mathf.SmoothStep(previousProgress, progress, 0.1f);
        progress = Mathf.Clamp(progress, 0f, 100f);
        targetProgress = Mathf.Clamp(targetProgress, 0f, 100f);
        previousProgress = targetProgress;

        progressSlider.value = targetProgress / 100f;
        for (int partIndex = 0;
            partIndex < (int) (targetProgress / (99 / partsCount));
            partIndex++) // reveal parts after every 8 units of progress  ex: 33 / (99 / 11)
        {
            playerParts.transform.GetChild(partIndex).gameObject.SetActive(true);
            if (partIndex > 5)
            {
                playerAnimator.enabled = true; // enable after revealing head
            }
        }
    }

    public void LoadScene(int sceneIndex)
    {
        foreach (var hideObject in hideObjects)
        {
            hideObject.SetActive(false);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < playerParts.transform.childCount; i++)
        {
            playerParts.transform.GetChild(i).gameObject.SetActive(false);
        }

        sceneToLoad = sceneIndex; // ----------------------------- disable for real Loading
        Invoke("fakeLoadShowOff", 0.5f); // ---------------------- disable for real Loading
        // StartCoroutine(LoadSceneAsync(sceneIndex));  // ------- enable for real Loading
    }

    private void fakeLoadShowOff()  // --------------------------- disable for real Loading
    {
        if (progress > 90f)
        {
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }

        progress += Random.Range(3f, 6f);
        Invoke("fakeLoadShowOff", Random.Range(0.3f, 1.5f));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float asyncProgress = Mathf.Clamp(operation.progress / 0.9f * 100f, 0f, 100f);
            Debug.Log("asyncProg: " + asyncProgress);
            // progress = asyncProgress;    //-------------------- enable for real Loading
            yield return null;
        }
    }

    public void initPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
        }
        PlayerPrefs.Save();
    }
}