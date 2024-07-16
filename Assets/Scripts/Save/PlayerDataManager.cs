using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private string FileName;
    [SerializeField] private Inventory inventory; // Assign this in the Inspector
    private PlayerSaveData Data;
    private List<IPlayerData> DataWantigObjects;
    private IFileHandler FileHandler;

    private static PlayerDataManager instance;
    private static readonly object instanceLock = new object();

    public static PlayerDataManager Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerDataManager>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject("PlayerDataManager");
                        instance = singletonObject.AddComponent<PlayerDataManager>();
                    }

                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    private void Awake()
    {
              if (instance == null)
    {
        
        instance = this as PlayerDataManager;
        DontDestroyOnLoad(gameObject);
    }
    
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= LoadDataOnSceneLoad;
        SceneManager.sceneUnloaded -= SaveDataFromScene;
    }

    private void Start()
    {
        if (Instance == this)
        {
            this.FileHandler = new FileHandler(Application.persistentDataPath, FileName);
            this.DataWantigObjects = FindAllDataWantigObjects();
            if (this.DataWantigObjects != null)
            {
                LoadGame();
            }
            SceneManager.sceneLoaded += LoadDataOnSceneLoad;
            SceneManager.sceneUnloaded += SaveDataFromScene;
        }
    }

    public void NewGame()
    {
        this.Data = new PlayerSaveData();
    }

    public void LoadDataOnSceneLoad(Scene NewScene, LoadSceneMode mode)
    {
        this.DataWantigObjects = FindAllDataWantigObjects();

        if (this.DataWantigObjects == null)
        {
            Debug.LogError("DataWantigObjects is null.");
            return;
        }

        if (this.Data == null)
        {
            Debug.LogError("Data is null. Cannot load data.");
            return;
        }

        foreach (IPlayerData DataWantingObj in DataWantigObjects)
        {
            if (DataWantingObj == null)
            {
                Debug.LogError("DataWantingObj is null.");
                continue;
            }

            // Ensure inventory is assigned before loading data
            if (DataWantingObj is UI_Inventory uiInventory)
            {
                uiInventory.SetInventory(inventory);
            }
            if(DataWantingObj is UI_EndlessRun uiEndlessrun)
            {
                uiEndlessrun.SetInventory(inventory);
            }
            if(DataWantingObj is ShopManager shopManager){
                    print("found shopmanager setting inventory");
                    shopManager.SetInventory(inventory);
                }
            DataWantingObj.LoadData(Data);
            DataWantingObj.LoadInventoryData(Data); // Load inventory data
            Debug.Log("Pushing data to: " + DataWantingObj);
        }
    }

    public void SaveDataFromScene(Scene CurrentScene)
    {
        ReadSceneData();
    }

    public void LoadGame()
    {
        this.Data = FileHandler.Load();

        if (this.Data == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        if (this.DataWantigObjects != null)
        {
            foreach (IPlayerData DataWantingObj in DataWantigObjects)
            {
                if (DataWantingObj == null)
                {
                    Debug.LogError("DataWantingObj is null.");
                    continue;
                }

                // Ensure inventory is assigned before loading data
                if (DataWantingObj is UI_Inventory uiInventory)
                {
                    uiInventory.SetInventory(inventory);
                }
                if(DataWantingObj is ShopManager shopManager){
                    print("found shopmanager setting inventory");
                    shopManager.SetInventory(inventory);
                }

                DataWantingObj.LoadData(Data);
                DataWantingObj.LoadInventoryData(Data);
                Debug.Log("Pushing data to: " + DataWantingObj);
            }
        }
    }

    public void SaveGame()
    {
        if (Data == null)
        {
            Debug.LogError("Data is null. Cannot save the game.");
            return;
        }

        if (FileHandler == null)
        {
            Debug.LogError("FileHandler is null. Cannot save the game.");
            return;
        }

        if (DataWantigObjects == null)
        {
            Debug.LogError("DataWantigObjects is null. Cannot save the game.");
            return;
        }

        ReadSceneData();
        FileHandler.Save(Data);
    }

    public void ReadSceneData()
    {
        if (DataWantigObjects == null)
        {
            Debug.LogError("DataWantigObjects is null. Cannot read scene data.");
            return;
        }

        foreach (IPlayerData dataPersistenceObj in DataWantigObjects)
        {
            dataPersistenceObj.SaveData(Data);
            dataPersistenceObj.SaveInventoryData(Data);
        }
    }

   private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause()
    {
        SaveGame();
    }

    private List<IPlayerData> FindAllDataWantigObjects()
    {
        IEnumerable<IPlayerData> DataWantigObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerData>();
        return new List<IPlayerData>(DataWantigObjects);
    }
}
