using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Menu_Controls
{
    public class GameOverlay : MonoBehaviour
    {
        public UIDocument document;

        private Label myUnits;
        private Label others;

        private void Awake()
        {
            myUnits = document.rootVisualElement.Q<Label>("Units");
            others = document.rootVisualElement.Q<Label>("Others");
            
            document.rootVisualElement.style.display = DisplayStyle.None;
        }

        public void UpdateFriendlyUnits(string text)
        {
            Debug.Log(myUnits.text);
            myUnits.text = text;
        }

        public void UpdateOthers(string text)
        {
            others.text = text;
        }
    }
}
