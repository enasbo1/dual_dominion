using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [Range(0.01f, 3f)]
    public float neutralTimeFlow = 1f;
    [Range(0.01f, 3f)]
    public float pauseTimeFlow = 0.1f;
    public List<GameObject> objectsToDisable = new List<GameObject>();
    
    [SerializeField] private GameObject canvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button leavePartyButton;
    [SerializeField] private Button leaveGameButton;
    
    private void Awake() {
        resumeButton.onClick.AddListener(() => {
            Time.timeScale = neutralTimeFlow;
            canvas.SetActive(false);
            objectsToDisable.ForEach(x => x.SetActive(true));
            Cursor.lockState = CursorLockMode.Locked;
        });

        leavePartyButton.onClick.AddListener(() => {
            SceneManagerScript.ChangeToScene(SceneName.Lobby);
        });
        
        leaveGameButton.onClick.AddListener(Application.Quit);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Time.timeScale = pauseTimeFlow;
            canvas.SetActive(true);
            objectsToDisable.ForEach(x => x.SetActive(false));
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
