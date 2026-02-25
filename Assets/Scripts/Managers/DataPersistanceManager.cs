using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string _saveFileName = "savegame.json";
    [SerializeField] private string _settingsFileName = "settings.json";
    [SerializeField] private string _notesFileName = "NoteData.json";

    private GameData _gameData;
    private NoteData _noteData;

    private List<IDataPersistance> _dataPersistenceObjects;

    private FileDataHandler<GameData> _gameDataHandler;
    private FileDataHandler<NoteData> _notesHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    { 
        InitializeHandlers();
        //LoadGame();
        LoadNotes();
    }


    private void InitializeHandlers()
    {
        string dataPath = Application.persistentDataPath;
        _gameDataHandler = new FileDataHandler<GameData>(dataPath, _saveFileName);
        _notesHandler = new FileDataHandler<NoteData>("Assets/Resources/", _notesFileName);
    }
    public void NewGame()
    {
        this._gameData = new GameData();
    }

    public void LoadGame()
    {
        this._gameData = _gameDataHandler.Load();

        if (this._gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        foreach (IDataPersistance dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref _gameData);
        }

        _gameDataHandler.Save(_gameData);
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistance> _dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
        .OfType<IDataPersistance>();

        return new List<IDataPersistance>(_dataPersistenceObjects);
    }

    public void LoadNotes()
    {
        this._noteData = _notesHandler.Load();
        
        if (this._noteData != null) NoteManager.Instance.notes = _noteData.notes;
    }

    public void PopulateNoteFile()
    {
        this._noteData = new NoteData();
        _notesHandler.Save(_noteData);
    }
}
