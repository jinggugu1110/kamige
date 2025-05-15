using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{

    [SerializeField] public string sceneName;

    public void SceneChange()
    {
        SceneManager.LoadScene(sceneName);
    }

}