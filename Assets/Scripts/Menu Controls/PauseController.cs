using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class PauseController : MonoBehaviour
    {
        public UIDocument document;

        private Button menu;
        private Button resume;
        private int _selectedIndex = 0; // Track selected button
        private Button[] _buttons; // Store buttons for navigation

        private void Awake()
        {
            resume = document.rootVisualElement.Q<Button>("Resume");
            menu = document.rootVisualElement.Q<Button>("Return");
            
            _buttons = new Button[] { resume, menu };
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
                OnResumeClicked();
            }
            else if (_selectedIndex == 1) // Exit button
            {
                OnReturnClicked();
            };
        }

        public void Pause()
        {
            resume.clicked += OnResumeClicked;
            menu.clicked += OnReturnClicked;
            
            GameManager.instance.input.actions.FindActionMap("UI").Enable();
            GameManager.instance.input.actions.FindActionMap("UI").Enable();
            GameManager.instance.input.actions["Navigate"].performed += OnNavigate;
            GameManager.instance.input.actions["Submit"].performed += OnSubmit;
        }

        private void OnResumeClicked()
        {
            GameManager.instance.input.actions["Navigate"].performed -= OnNavigate;
            GameManager.instance.input.actions["Submit"].performed -= OnSubmit;
            GameManager.instance.input.actions.FindActionMap("UI").Disable();
            GameManager.instance.ResumeGame();
        }

        private void OnReturnClicked()
        {
            GameManager.instance.input.actions["Navigate"].performed -= OnNavigate;
            GameManager.instance.input.actions["Submit"].performed -= OnSubmit;
            GameManager.instance.input.actions.FindActionMap("UI").Disable();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
