using UnityEngine;

public class BoardAnimManager : MonoBehaviour
{
    [Header("Hover vars")]
    [SerializeField] private float magnitude;
    [SerializeField] private float frequency;

    [Header("Animation positions")]
    [SerializeField] private float startYPos;
    [SerializeField] private float defaultYPos;
    [SerializeField] private float endYPos;
    [SerializeField] private float duration;

    [Header("External components")]
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private HealthManager healthManager;

    private bool canPlayIdle = false;
    private bool appear = false;
    private bool disappear = false;

    private float targetPos;
    private float startTime;

    private void Start()
    {
        gameManager.gameStarted.AddListener(PlayAppearAnim);
        gameManager.gameEnded.AddListener(PlayDisappearAnim);
    }

    private void Update()
    {
        Vector3 position = new Vector3();
        //I had to animate the board idle animation this way because there is a bug in this Unity version where you can't properly edit animation curves so the animation was stuttering
        if (canPlayIdle)
        {
            position.y = magnitude * Mathf.Sin(frequency * Time.time);
            transform.position = position;
        }

        if(appear || disappear)
        {
            float t = (Time.time - startTime) / duration;
            position.y = Mathf.SmoothStep(transform.position.y, targetPos, t);
            transform.position = position;

            if(Mathf.Abs((transform.position.y - targetPos)) < 0.01f)
            {
                targetPos = 0;

                if (appear)
                {
                    appear = false;
                    canPlayIdle = true;
                }
                else
                    disappear = false;
            }
        }
    }

    //make board fly away off the screen
    private void PlayDisappearAnim()
    {
        canPlayIdle = false;
        startTime = Time.time;
        disappear = true;
        targetPos = endYPos;
    }

    //make board fly up onto the screen from the bottom
    private void PlayAppearAnim()
    {
        appear = true;
        startTime = Time.time;
        targetPos = defaultYPos;
        transform.position = new Vector3(0, startYPos, 0);
    }
}
