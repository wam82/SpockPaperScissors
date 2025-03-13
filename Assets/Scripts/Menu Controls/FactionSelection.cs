using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class FactionSelection : MonoBehaviour
    {
        private UIDocument uiDocument;
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
            uiDocument = GetComponent<UIDocument>();
        
            rock = uiDocument.rootVisualElement.Q<Button>("Rock");
            paper = uiDocument.rootVisualElement.Q<Button>("Paper");
            scissors = uiDocument.rootVisualElement.Q<Button>("Scissors");
            lizard = uiDocument.rootVisualElement.Q<Button>("Lizard");
            spock = uiDocument.rootVisualElement.Q<Button>("Spock");
        
            _buttons = new Button[] { rock, paper, scissors, lizard, spock };

            rock.clicked += OnRockClicked;
            paper.clicked += OnPaperClicked;
            scissors.clicked += OnScissorsClicked;
            lizard.clicked += OnLizardClicked;
            spock.clicked += OnSpockClicked;
        
            playerInput.actions.FindActionMap("UI").Disable();
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

        private void Unsubscribe()
        {
            playerInput.actions.FindActionMap("UI").Disable();
            playerInput.actions["Navigate"].performed -= OnNavigate;
            playerInput.actions["Submit"].performed -= OnSubmit;
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
            Unsubscribe();
            ApplicationModel.factionTag = "Rocks";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    
        private void OnPaperClicked()
        {
            Unsubscribe();
            ApplicationModel.factionTag = "Papers";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    
        private void OnScissorsClicked()
        {
            Unsubscribe();
            ApplicationModel.factionTag = "Scissors";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    
        private void OnLizardClicked()
        {
            Unsubscribe();
            ApplicationModel.factionTag = "Lizards";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    
        private void OnSpockClicked()
        {
            Unsubscribe();
            ApplicationModel.factionTag = "Spocks";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}
