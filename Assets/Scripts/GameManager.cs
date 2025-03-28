using System.Collections.Generic;
using AI_Foundation;
using Camera_Controllers;
using Menu_Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerInput input;
    
    public enum GameState
    {
        Setup, Play, Pause, End
    }
    
    public GameState gameState = GameState.Setup;
    
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera setupCamera;
    
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> factions = new List<GameObject>();
    public GameObject playerFaction;
    
    public GameOverlay gameOverlay;
    public PauseController pauseController;

    public void PauseGame()
    {
        gameState = GameState.Pause;
        pauseController.document.rootVisualElement.style.display = DisplayStyle.Flex;
        pauseController.Pause();
    }
    public void ResumeGame()
    {
        if (gameState == GameState.Pause)
        {
            pauseController.document.rootVisualElement.style.display = DisplayStyle.None;
            gameState = GameState.Play;
        }
    }
    
    private void InstantiateSceneObjects()
    {
        switch (playerFaction.tag)
        {
            case "Lizards":
                factions.Add(GameObject.FindGameObjectWithTag("Papers"));
                factions.Add(GameObject.FindGameObjectWithTag("Rocks"));
                factions.Add(GameObject.FindGameObjectWithTag("Scissors"));
                factions.Add(GameObject.FindGameObjectWithTag("Spocks"));
                break;
            case "Papers":
                factions.Add(GameObject.FindGameObjectWithTag("Lizards"));
                factions.Add(GameObject.FindGameObjectWithTag("Rocks"));
                factions.Add(GameObject.FindGameObjectWithTag("Scissors"));
                factions.Add(GameObject.FindGameObjectWithTag("Spocks"));
                break;
            case "Rocks":
                factions.Add(GameObject.FindGameObjectWithTag("Lizards"));
                factions.Add(GameObject.FindGameObjectWithTag("Papers"));
                factions.Add(GameObject.FindGameObjectWithTag("Scissors"));
                factions.Add(GameObject.FindGameObjectWithTag("Spocks"));
                break;
            case "Scissors":
                factions.Add(GameObject.FindGameObjectWithTag("Lizards"));
                factions.Add(GameObject.FindGameObjectWithTag("Rocks"));
                factions.Add(GameObject.FindGameObjectWithTag("Papers"));
                factions.Add(GameObject.FindGameObjectWithTag("Spocks"));
                break;
            case "Spocks":
                factions.Add(GameObject.FindGameObjectWithTag("Lizards"));
                factions.Add(GameObject.FindGameObjectWithTag("Rocks"));
                factions.Add(GameObject.FindGameObjectWithTag("Scissors"));
                factions.Add(GameObject.FindGameObjectWithTag("Papers"));
                break;
            default:
                Debug.LogError("No player faction found");
                break;
        }
        
        foreach (Transform child in playerFaction.transform)
        {
            units.Add(child.gameObject);
        }
    }
    private int GetUnitsCount(GameObject faction)
    {
        int count = 0;
        foreach (Transform dummy in faction.transform)
        {
            count++;
        }
        return count;
    }
    
    public void RemovePlayerUnit(GameObject unit)
    {
        if (unit.CompareTag(playerFaction.GetComponent<GroupAI>().factionTag))
        {
            units.Remove(unit);
        }
    }
    public void CompleteSetup()
    {
        gameState = GameState.Play;
        mainCamera.gameObject.SetActive(true);
        units = setupCamera.GetComponent<UnitPlacement>().GetUnits();
        foreach (GameObject unit in units)
        {
            unit.GetComponent<IndividualAI>().enabled = true;
        }
        mainCamera.GetComponent<CameraController>().SetUp();
        EnableEnemies();
        setupCamera.gameObject.SetActive(false);
        input.actions.FindActionMap("Game").Enable();
        input.actions.FindActionMap("SetUp").Disable();
    }

    private void SetUp()
    {
        DisableEnemies();
        mainCamera.gameObject.SetActive(false);
        
        Vector3 position = playerFaction.transform.position;
        position.y = 40;
        setupCamera.transform.position = position;
        
        setupCamera.gameObject.SetActive(true);
        setupCamera.GetComponent<UnitPlacement>().SetUp();
        
        input.actions.FindActionMap("Game").Disable();
        input.actions.FindActionMap("SetUp").Enable();
        pauseController.document.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void EnableEnemies()
    {
        foreach (GameObject faction in factions)
        {
            foreach (Transform child in faction.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void DisableEnemies()
    {
        foreach (GameObject faction in factions)
        {
            foreach (Transform child in faction.transform)
            {
                child.GetComponentInChildren<Camera>().gameObject.SetActive(false);
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            
        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
            playerFaction = GameObject.FindGameObjectWithTag(ApplicationModel.factionTag);
            InstantiateSceneObjects();
            SetUp();
        }
    }

    private void Update()
    {
        if (gameState == GameState.Pause)
        {
            return;
        }
        
        if (gameOverlay.gameObject.activeInHierarchy == true)
        {
            gameOverlay.UpdateFriendlyUnits("Number of " + playerFaction.name.ToLower() + " remaining: " + units.Count);
            string others = "";
            foreach (GameObject faction in factions)
            {
                others += "Number of " + faction.name.ToLower() + " remaining: " + GetUnitsCount(faction) + "\n";
            }
            gameOverlay.UpdateOthers(others);
        }
        
        foreach (GameObject faction in factions)
        {
            if (GetUnitsCount(faction) <= 0)
            {
                ApplicationModel.winningFactionTag = faction.name;
                UnityEngine.SceneManagement.SceneManager.LoadScene("EndCredit");
                Debug.Log(faction.name + " has lost");
                break;
            }
        }
    }
}
