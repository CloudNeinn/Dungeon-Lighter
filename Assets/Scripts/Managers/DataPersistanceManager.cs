using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string saveFileName = "savegame.json";
    [SerializeField] private string settingsFileName = "settings.json";
    [SerializeField] private string notesFileName = "notes.json";

    private GameData gameData;
    private NoteData noteData;

    private List<IDataPersistance> dataPersistenceObjects;

    private FileDataHandler<GameData> gameDataHandler;
    private FileDataHandler<NoteData> notesHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Start()
    {
        InitializeHandlers();
        //LoadGame();
        LoadNotes();
    }

    private void InitializeHandlers()
    {
        string dataPath = Application.persistentDataPath;
        gameDataHandler = new FileDataHandler<GameData>(dataPath, saveFileName);
        notesHandler = new FileDataHandler<NoteData>(dataPath, notesFileName);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistance Manager instance");
        }
        Instance = this;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = gameDataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        foreach (IDataPersistance dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        gameDataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
        .OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistenceObjects);
    }

    public void LoadNotes()
    {
        this.noteData = notesHandler.Load();

        if (this.noteData == null)
        {
            Debug.Log("Note file is not present. Creating note file and populating with default values");
            PopulateNoteFile();
        }
    }

    public void PopulateNoteFile()
    {
        this.noteData = new NoteData();
        notesHandler.Save(noteData);
    }
}
