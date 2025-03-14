using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class EndMenu : MonoBehaviour
    {
        private UIDocument _document;
        [FormerlySerializedAs("Input")] public PlayerInput playerInput;

        private Label statement;
        private Button menu;
        
        private int _selectedIndex = 0; // Track selected button
        private Button[] _buttons; // Store buttons for navigation
        
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        
            menu = _document.rootVisualElement.Q<Button>("Return");
            statement = _document.rootVisualElement.Q<Label>("Statement");
            statement.text = ApplicationModel.winningFactionTag + " has lost!";
        
            // Store buttons in an array for navigation
            _buttons = new Button[] { menu };


            menu.clicked += OnReturnClicked;
        
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
                OnReturnClicked();
            }
        }
        
        private void OnReturnClicked()
        {
            playerInput.actions["Navigate"].performed -= OnNavigate;
            playerInput.actions["Submit"].performed -= OnSubmit;
            playerInput.actions.FindActionMap("UI").Disable();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
