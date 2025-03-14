using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class MainMenu : MonoBehaviour
    {
        private UIDocument _document;
        public PlayerInput Input;

        private Button play;
        private Button exit;
        private int _selectedIndex = 0; // Track selected button
        private Button[] _buttons; // Store buttons for navigation

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        
            play = _document.rootVisualElement.Q<Button>("Play");
            exit = _document.rootVisualElement.Q<Button>("Exit");
        
            // Store buttons in an array for navigation
            _buttons = new Button[] { play, exit };
        
        
            play.clicked += OnPlayButtonClicked;
            exit.clicked += OnExitButtonClicked;
        
            Input.actions.FindActionMap("UI").Enable();
            Input.actions["Navigate"].performed += OnNavigate;
            Input.actions["Submit"].performed += OnSubmit;
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
            if (_selectedIndex == 0) // Play button
            {
                OnPlayButtonClicked();
            }
            else if (_selectedIndex == 1) // Exit button
            {
                OnExitButtonClicked();
            };
        }
        private void OnPlayButtonClicked()
        {
            Input.actions["Navigate"].performed -= OnNavigate;
            Input.actions["Submit"].performed -= OnSubmit;
            Input.actions.FindActionMap("UI").Disable();
            UnityEngine.SceneManagement.SceneManager.LoadScene("FactionSelection");
        }
    
        private void OnExitButtonClicked()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}
