using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public void GotToCutscenePage()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToDialogueScene()
    {
        SceneManager.LoadScene(2);
    }
}
