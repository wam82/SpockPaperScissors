using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FactionSelection : MonoBehaviour
{
    private UIDocument _uiDocument;
    public PlayerInput playerInput;

    private Button rock;
    private Button paper;
    private Button scissors;
    private Button lizard;
    private Button spock;
    
    private int _selectedIndex = 0; // Track selected button
    private Button[] _buttons; // Store buttons for navigation

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        
        rock = _uiDocument.rootVisualElement.Q<Button>("Rock");
        paper = _uiDocument.rootVisualElement.Q<Button>("Paper");
        scissors = _uiDocument.rootVisualElement.Q<Button>("Scissors");
        lizard = _uiDocument.rootVisualElement.Q<Button>("Lizard");
        spock = _uiDocument.rootVisualElement.Q<Button>("Spock");
        
        _buttons = new Button[] { rock, paper, scissors, lizard, spock };

        rock.clicked += OnRockClicked;
        paper.clicked += OnPaperClicked;
        scissors.clicked += OnScissorsClicked;
        lizard.clicked += OnLizardClicked;
        spock.clicked += OnSpockClicked;
        
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions["Navigate"].performed += OnNavigate;
        playerInput.actions["Submit"].performed += OnSubmit;
    }
    
    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigation = context.ReadValue<Vector2>();

        if (navigation.y > 0) _selectedIndex = Mathf.Max(0, _selectedIndex - 1);
        if (navigation.y < 0) _selectedIndex = Mathf.Min(_buttons.Length - 1, _selectedIndex + 1);

        // Visually indicate selection (if needed)
        HighlightButton(_selectedIndex);
    }
    
    private void HighlightButton(int index)
    {
        foreach (var btn in _buttons)
        {
            btn.RemoveFromClassList("selected");
        }
        _buttons[index].AddToClassList("selected");
    }
    
    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (_selectedIndex == 0)
        {
            OnRockClicked();
        }
        else if (_selectedIndex == 1)
        {
            OnPaperClicked();
        }
        else if (_selectedIndex == 2) 
        {
            OnScissorsClicked();
        }
        else if (_selectedIndex == 3) 
        {
            OnLizardClicked();
        }
        else if (_selectedIndex == 4) 
        {
            OnSpockClicked();
        }
    }

    private void OnRockClicked()
    {
        ApplicationModel.factionTag = "Rocks";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    private void OnPaperClicked()
    {
        ApplicationModel.factionTag = "Papers";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    private void OnScissorsClicked()
    {
        ApplicationModel.factionTag = "Scissors";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    private void OnLizardClicked()
    {
        ApplicationModel.factionTag = "Lizards";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    private void OnSpockClicked()
    {
        ApplicationModel.factionTag = "Spocks";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
