using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class Setup : MonoBehaviour
    {
        private UIDocument _document;

        private Label confirm;
        private Label cancel;
        private Label units; 
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            
            confirm = _document.rootVisualElement.Q<Label>("Confirm");
            cancel = _document.rootVisualElement.Q<Label>("Revert");
            units = _document.rootVisualElement.Q<Label>("Units");
        }

        public void UpdateUnits(int nb)
        {
            units.text = "Number of units left to place: " + nb.ToString();
        }

        public void EnableConfirmation()
        {
            confirm.visible = true;
            cancel.visible = true;
        }

        public void DisableConfirmation()
        {
            confirm.visible = false;
            cancel.visible = false;
        }
    }
}
