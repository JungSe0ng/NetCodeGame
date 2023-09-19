using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SK
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] private int worldSceneIndex = 1;

        private void Awake()
        {
            // THERE CAN ONLY BE ONE INSTANCE OF THIS SCRIPT AT ONE TIME, IF ANOTHER EXISTS, DESTROY IT
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        public IEnumerator LoadNewGame()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}
