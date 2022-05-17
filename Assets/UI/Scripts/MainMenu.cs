#nullable enable

using SimpleVoxelTanks.LevelControllers;
using SimpleVoxelTanks.MapBuilders;

using UnityEngine;
using UnityEngine.UI;

public class MainMenu : AbstractMenu
{
    #region Buttons

    [SerializeField]
    protected Button _exitButton;

    [SerializeField]
    protected Button _loadDemoLevelButton;

    #endregion Buttons

    protected override void Awake ()
    {
        base.Awake();
        _loadDemoLevelButton.onClick.AddListener(LoadDemoLevel);
        _exitButton.onClick.AddListener(Exit);
    }

    public void LoadDemoLevel ()
    {
        LevelLoader.LoadLevel<BattleCityScript, BoxBuilder>();
        HideMenu();
    }
}