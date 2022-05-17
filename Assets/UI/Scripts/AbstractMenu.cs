using SimpleVoxelTanks.LevelControllers;

using UnityEngine;

public class AbstractMenu : MonoBehaviour
{
    [SerializeField]
    protected GameObject _canvas;

    [SerializeField]
    protected LevelLoader _levelLoader;

    public LevelLoader LevelLoader { get => _levelLoader; set => _levelLoader = value; }

    protected virtual void Awake ()
    {
        if (_levelLoader == null)
            _levelLoader = (LevelLoader) FindObjectOfType(typeof(LevelLoader));
        ShowMenu();
    }

    public void Exit () => Application.Quit();

    public void HideMenu ()
    {
        _canvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowMenu ()
    {
        _canvas.SetActive(true);
        Time.timeScale = 0f;
    }
}