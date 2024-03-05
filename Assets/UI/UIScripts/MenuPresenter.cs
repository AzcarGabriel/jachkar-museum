using UnityEngine;
using UnityEngine.UIElements;

namespace UI.UIScripts
{
    public class MenuPresenter : MonoBehaviour
    {
        private VisualElement _mainMenu;
        private VisualElement _lobbyScreen;
        
        public VisualElement LoadScreen { get; private set; }
        
        private void Awake()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _mainMenu = root.Q("MainMenu");
            _lobbyScreen = root.Q("LobbyScreen");
            LoadScreen = root.Q("LoadingScreen");
            Debug.Log(LoadScreen);

            MainMenuBehaviour mainMenuBehaviour = new(_mainMenu)
            {
                OpenLobby = () =>
                {
                    _lobbyScreen.Display(true);
                    _mainMenu.Display(false);
                }
            };

            LobbyScreenBehaviour lobbyScreenBehaviour = new(_lobbyScreen)
            {
                OpenMainMenu = () =>
                {
                    _lobbyScreen.Display(false);
                    _mainMenu.Display(true);
                }
            };


        }
    }
}
