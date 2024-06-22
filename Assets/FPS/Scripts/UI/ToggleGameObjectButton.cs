using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class ToggleGameObjectButton : MonoBehaviour
    {
        public GameObject ObjectToToggle;
        public Button EasyMode;
        public Button MediumMode;
        public Button HardMode;
        public Button DDMode;
        public bool ResetSelectionAfterClick;

        void Update()
        {
            if (ObjectToToggle.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel))
            {
                SetGameObjectActive(false);
            }
        }

        public void SetGameObjectActive(bool active)
        {
            ObjectToToggle.SetActive(active);
            EasyMode.gameObject.SetActive(!active);
            MediumMode.gameObject.SetActive(!active);
            HardMode.gameObject.SetActive(!active);
            DDMode.gameObject.SetActive(!active);

            if (ResetSelectionAfterClick)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}