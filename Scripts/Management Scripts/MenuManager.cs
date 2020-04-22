using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject credits;
    public GameObject instructions;
    public GameObject challenge;
    public List<Sprite> challengesList;
    public GameObject challengeDescription;
    public GameObject congratulations;

    void Start() {
        if(LockVariables.all==1 && LockVariables.cheat==false) {
            congratulations.SetActive(true);
            LockVariables.all=2;
        }
    }

    public void CongratulationsOK() {
        congratulations.SetActive(false);
    }

    public void StartGame() {
        SceneManager.LoadScene("SampleScene");
    }

    public void ReturnMenu() {
        SceneManager.LoadScene("main menu");
    }

    public void Challenge() {
        challenge.SetActive(true);
    }
    public void GraveyardChallenge() {
        challengeDescription.SetActive(true);
        challengeDescription.GetComponent<Image>().sprite = challengesList[0];
    }
    public void HorrorChallenge() {
        challengeDescription.SetActive(true);
        challengeDescription.GetComponent<Image>().sprite = challengesList[1];
    }
    public void SiblingsChallenge() {
        challengeDescription.SetActive(true);
        challengeDescription.GetComponent<Image>().sprite = challengesList[2];
    }
    public void GraveyardGo() {
        SceneManager.LoadScene("graveyard");
    }
    public void HorrorGo() {
        SceneManager.LoadScene("horror");
    }
    public void SiblingsGo() {
        SceneManager.LoadScene("siblings");
    }

    public void Credits() {
        credits.SetActive(true);
    }
    public void Instructions() {
        instructions.SetActive(true);
    }

    public void CancelButton() {
        credits.SetActive(false);
        challenge.SetActive(false);
        instructions.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }
    
}