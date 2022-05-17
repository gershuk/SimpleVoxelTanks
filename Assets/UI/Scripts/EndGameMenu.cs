#nullable enable

using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : AbstractMenu
{
    #region Buttons

    [SerializeField]
    protected Button _exitButton;

    [SerializeField]
    protected Button _mainMenuButton;

    #endregion Buttons

    protected override void Awake ()
    {
        base.Awake();
        _mainMenuButton.onClick.AddListener(GoToMainMenu);
        _exitButton.onClick.AddListener(Exit);
    }

    public void GoToMainMenu () => LevelLoader.GoToMainMenu();
}