using SimpleVoxelTanks.MapBuilders;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleVoxelTanks.LevelControllers
{
    public sealed class LevelLoader : MonoBehaviour
    {
        private const string _gameSceneName = "EmptyLevelScene";
        private const string _loseSceneName = "LoseScene";
        private const string _menuSceneName = "MainMenuScene";
        private const string _winSceneName = "WinScene";

        private void Awake ()
        {
            DontDestroyOnLoad(this);
        }

        public void GoToLoseScreen () => SceneManager.LoadScene(_loseSceneName, LoadSceneMode.Single);

        public void GoToMainMenu () => SceneManager.LoadScene(_menuSceneName, LoadSceneMode.Single);

        public void GoToWinScreen () => SceneManager.LoadScene(_winSceneName, LoadSceneMode.Single);

        public void LoadLevel<TLevelScript, TMapBuilder> () where TLevelScript : AbstractLevelScript, new()
                                                            where TMapBuilder : AbstractMapBuilder, new()
        {
            SceneManager.LoadSceneAsync(_gameSceneName, LoadSceneMode.Single).completed += (op) =>
            {
                var gameObject = new GameObject("SceneController");
                var levelScript = gameObject.AddComponent<TLevelScript>();
                levelScript.Init(gameObject.AddComponent<TMapBuilder>());
                levelScript.StartScript();
                levelScript.OnWin += GoToWinScreen;
                levelScript.OnLose += GoToLoseScreen;
            };
        }

        [ContextMenu("BattleCity")]
        public void TestInit () => LoadLevel<BattleCityScript, BoxBuilder>();
    }
}