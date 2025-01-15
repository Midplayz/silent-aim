using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Header("Score")]
    [field: SerializeField] private TextMeshProUGUI scoreText;
    [field: SerializeField] private int scoreAmount = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int adjustingValue)
    {
        scoreAmount += adjustingValue;
        scoreText.text = "Score: " + scoreAmount;
    }
}
