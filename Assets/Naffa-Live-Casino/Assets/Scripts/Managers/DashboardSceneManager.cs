using UnityEngine;
using UnityEngine.SceneManagement;

public class DashboardSceneManager : MonoBehaviour
{
    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Supoprting Functions

    public void LoadDashboard()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads Tarneeb 400 Scene
    /// </summary>
    public void LoadTarneeb400()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadTarneeb41()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadTrix()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadHand()
    {
        SceneManager.LoadScene(5);
    }

    public void LoadPoker()
    {
        SceneManager.LoadScene(6);
    }

    public void LoadBlackjack()
    {
        SceneManager.LoadScene(7);
    }

    #endregion
}
