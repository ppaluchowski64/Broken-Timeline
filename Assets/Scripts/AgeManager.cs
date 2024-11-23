using UnityEngine;
using System.Collections;

public class AgeManager : MonoBehaviour
{
    public bool Prehistory = true;
    public bool Middle_Ages = false;
    public bool SCI_FI = false;

    public GameObject RedPlayerwins;
    public GameObject BluePlayerwins;
    public GameObject NextAgeSelectionPanel;
    public GameObject MedievalAgePanel;
    public GameObject ScifiAgePanel;
    public GameObject PrehistoryLevel;
    public GameObject Medievallevel;
    public GameObject ScifiAgelevel;
    public GameObject WinScreen;

    public int Player1_score = 0; // Red Player's score
    public int Player2_score = 0; // Blue Player's score

    public void ChangeAge(GameObject defeatedPlayer)
    {
        // Award a point to the other player
        if (defeatedPlayer.CompareTag("Red Player"))
        {
            Player2_score++;
            RedPlayerwins.SetActive(true); // Display "Blue Wins" UI
        }
        else if (defeatedPlayer.CompareTag("Blue Player"))
        {
            Player1_score++;
            BluePlayerwins.SetActive(true); // Display "Red Wins" UI
        }

        StartCoroutine(HandleDeathSequence());
    }

    IEnumerator HandleDeathSequence()
    {
        // Show "Player Wins" and score panels
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(2f);
        RedPlayerwins.SetActive(false);
        BluePlayerwins.SetActive(false);
        NextAgeSelectionPanel.SetActive(true);

        yield return new WaitForSeconds(2f);
        NextAgeSelectionPanel.SetActive(false);

        // Transition from Prehistory to Middle Ages
        if (Prehistory)
        {
            MedievalAgePanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            PrehistoryLevel.SetActive(false);
            Medievallevel.SetActive(true);
            Prehistory = false;
            Middle_Ages = true;
            MedievalAgePanel.SetActive(false);

            // Heal both players to full
            HealAllEnemies();
        }
        // Transition from Middle Ages to Sci-Fi age or show Win Screen
        else if (Middle_Ages)
        {
            if (Player1_score < 2 && Player2_score < 2)
            {
                ScifiAgePanel.SetActive(true);
                yield return new WaitForSeconds(2f);
                Medievallevel.SetActive(false);
                ScifiAgelevel.SetActive(true);
                Middle_Ages = false;
                SCI_FI = true;

                // Heal both players to full
                HealAllEnemies();
            }
            else
            {
                // Show the Win Screen if a player reaches the required score
                WinScreen.SetActive(true);
            }
        }
    }

    void HealAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.HealToFull();
        }
    }
}
