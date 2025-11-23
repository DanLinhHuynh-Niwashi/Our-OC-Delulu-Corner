using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Model Data List")]
    public List<ModelData> allModels;

    [Header("Current Loaded Model")]
    public ModelData currentModelData;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ModelData GetCurrentModel()
    {
        return currentModelData;
    }
}
