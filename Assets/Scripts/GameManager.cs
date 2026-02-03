using UnityEngine;

public class GameManager : MonoBehaviour
{   

    private int score;

    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500);
    }

    public void AddScore(int scoreAmount)
    {
        score += scoreAmount;
    }
}
