using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public int turn;
    public Char activeChar;

    public bool startGame;
    public bool passive;

    public int hitCount1;
    public int hitCount2;
    public int winHit = 5;

    public bool placingDone;
    public int chooseTurnOrder;

    public float scale = 1.65f;

    public Char movingChar;

    public GameObject teamPanels;
    public GameObject controlPanel;
    public GameObject[] lifePoints1;
    public GameObject[] lifePoints2;

    private Animator camAnimator;

    public bool canPassTurn;
    public Char lastHitted;

    public GameObject team1Card;
    public GameObject team2Card;

    public List<GameObject> team1Turn;
    public List<GameObject> team2Turn;

    public GameObject team1Wins;
    public GameObject team2Wins;
    public bool gameOver;

    public List<GameObject> locks;
    public List<GameObject> locked;

    public bool doneChoosing = false;

    public GameObject unlockMessages;



    void Start()
    {
        this.startGame = false;
        this.canPassTurn = false;
        this.turn = 0;
        this.hitCount1 = 0;
        this.hitCount2 = 0;
        this.chooseTurnOrder = 1;
        this.placingDone = false;
        this.passive = true;
        this.movingChar = null;
        this.lastHitted = null;
        this.gameOver = false;

        camAnimator = Camera.main.GetComponent<Animator>();
    }

    void Update() {

        if(this.canPassTurn) {
            if (Input.GetKeyDown(KeyCode.Space) && this.startGame) {
                PassTurn();
            }
        }

        if(this.activeChar==null && !this.startGame && this.placingDone) {
            this.startGame = true;
            teamPanels.SetActive(true);
            this.canPassTurn = true;
            PassTurn();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && GameObject.Find("ChoiceMaster")==null && !this.gameOver) {
            controlPanel.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.A) && !this.passive && !this.activeChar.isAttacking && !this.activeChar.attacked && !this.activeChar.usedAction) {
            UpdateBoard();
            SetActiveChar();
            this.activeChar.moveActing = false;
            this.activeChar.isAttacking = true;
            this.activeChar.isSkilling = false;
            this.activeChar.Attack();
        }

        if(Input.GetKeyDown(KeyCode.S) && !this.passive && !this.activeChar.isSkilling && !this.activeChar.usedSkill && !this.activeChar.usedAction) {
            UpdateBoard();
            SetActiveChar();
            this.activeChar.moveActing = false;
            this.activeChar.isAttacking = false;
            this.activeChar.isSkilling = true;
            this.activeChar.Skill();
        }

        if(Input.GetKeyDown(KeyCode.X) && (this.activeChar.isSkilling || this.activeChar.isAttacking)) {
            this.activeChar.isSkilling = false;
            this.activeChar.isAttacking = false;
            UpdateBoard();
            SetActiveChar();
            if(this.activeChar.moveActivations > 0) {
                this.activeChar.MoveActive();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.X) && this.passive) {
            EndPassive();
        }

        if(Input.GetKeyDown(KeyCode.X) && this.activeChar.movable && this.movingChar == this.activeChar) {
            UpdateConditions();
        }
        
        if(!this.doneChoosing) {
            if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                if(Input.GetKeyDown(KeyCode.U)) { 
                    foreach(GameObject loc in locks) {
                        loc.SetActive(false);
                    }
                    foreach(GameObject ked in locked) {
                        ked.SetActive(true);
                    }
                    LockVariables.sirCat=true;
                    LockVariables.uki=true;
                    LockVariables.beelzebub=true;
                    LockVariables.cheat=true;
                }
            }
        }
        
    }

    public void CancelControl() {
        controlPanel.SetActive(false);
    }
    public void ReturnMenu() {
        SceneManager.LoadScene("main menu");
    }

    public void PlaceCharacter(Tile tile) {
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character.placeSelected) {
                character.turnOrder = this.chooseTurnOrder;
                float distanceX = tile.positionX - character.positionX;
                float distanceY = tile.positionY - character.positionY;
                character.transform.position += new Vector3(distanceX*scale,distanceY*scale,-character.turnOrder);
                character.UpdatePosition();
                character.SetTile();
                tile.SetOccupation();
                character.isSet = true;
            }
        }
        this.chooseTurnOrder += 1;

        foreach(Char character in FindObjectsOfType<Char>()) {
                character.ResetChar();
            }
            foreach(Tile tiles in FindObjectsOfType<Tile>()) {
                tiles.ResetTile();
            }
        if(this.chooseTurnOrder > 6) {
            this.placingDone = true;
        }
    }





    public float Distance(Char A, Char B) {
        return Mathf.Abs(A.positionX - B.positionX)+Mathf.Abs(A.positionY-B.positionY);
    }
    public float Distance(Char A, Tile B) {
        return Mathf.Abs(A.positionX - B.positionX)+Mathf.Abs(A.positionY-B.positionY);
    }
    public float Distance(Tile A, Tile B) {
        return Mathf.Abs(A.positionX - B.positionX)+Mathf.Abs(A.positionY-B.positionY);
    }
    public bool Meelee(Tile A, Tile B) {
        if(this.Distance(A,B) <= 2 && this.Distance(A,B) != 0 && Mathf.Abs(B.positionX-A.positionX) <= 1 && Mathf.Abs(B.positionY-A.positionY) <= 1) {
            return true;
        } else {return false;}
    }

    public Tile GetAdjacent(Tile A, string s) {
        if(s == "up") {return this.GetTile(A.positionX,A.positionY+1);}
        if(s == "down") {return this.GetTile(A.positionX,A.positionY-1);}
        if(s == "left") {return this.GetTile(A.positionX-1,A.positionY);}
        if(s == "right") {return this.GetTile(A.positionX+1,A.positionY);}
        return null;
    }

    public Tile GetDiagonal(Tile A, string s) {
        if(s == "upperRight") {return this.GetTile(A.positionX+1,A.positionY+1);}
        if(s == "upperLeft") {return this.GetTile(A.positionX-1,A.positionY+1);}
        if(s == "lowerRight") {return this.GetTile(A.positionX+1,A.positionY-1);}
        if(s == "lowerLeft") {return this.GetTile(A.positionX-1,A.positionY-1);}
        return null;
    }

    public void PassTurn() {
        if(this.activeChar != null) {
            this.activeChar.moveActing = false;
            this.activeChar.attacked = false;
            this.activeChar.usedSkill = false;
            this.activeChar.usedAction = false;
        }
        if(this.activeChar?.moveActivations < 2) {
            this.activeChar.moveActivations = 2;
        }
        this.turn += 1;
        if(this.turn > 6) {
            this.turn = 1;
        }
        UpdateConditions();
        UpdateBoard();
        SetActiveChar();
        BeginTurn();
        this.activeChar.Passive();

        if(GameObject.Find("AIbehavior")==null) {
            TurnPanels();
        }
        
    }

    public void TurnPanels() {
        if(this.turn%2==1) {
            foreach(Char character in FindObjectsOfType<Char>()) {

                int charTurn = character.turnOrder-1;
                int thisTurn = this.turn-1;

                if(charTurn == thisTurn % 6) {
                    this.team1Turn[1].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+1) % 6) {
                    this.team1Turn[2].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+2) % 6) {
                    this.team1Turn[3].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+3) % 6) {
                    this.team1Turn[4].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+4) % 6) {
                    this.team1Turn[5].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+5) % 6) {
                    this.team1Turn[6].GetComponent<Image>().sprite = character.iconImage;
                }
            }
            this.team2Turn[0].SetActive(false);
            this.team1Turn[0].SetActive(true);
        }
        if(this.turn%2==0) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                
                int charTurn = character.turnOrder-1;
                int thisTurn = this.turn-1;

                if(charTurn == thisTurn % 6) {
                    this.team2Turn[1].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+1) % 6) {
                    this.team2Turn[2].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+2) % 6) {
                    this.team2Turn[3].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+3) % 6) {
                    this.team2Turn[4].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+4) % 6) {
                    this.team2Turn[5].GetComponent<Image>().sprite = character.iconImage;
                }
                if(charTurn == (thisTurn+5) % 6) {
                    this.team2Turn[6].GetComponent<Image>().sprite = character.iconImage;
                }
            }
            this.team1Turn[0].SetActive(false);
            this.team2Turn[0].SetActive(true);
        }
    }

    public void ShowCard(Char character) {
        if(character.team == 1) {
            this.team1Card.GetComponent<Image>().sprite = character.cardImage;
            this.team1Card.SetActive(true);
        }
        if(character.team == 2) {
            this.team2Card.GetComponent<Image>().sprite = character.cardImage;
            this.team2Card.SetActive(true);
        }
    }
    

    public void EndPassive() {
        this.passive = false;
        UpdateBoard();
        SetActiveChar();
        this.activeChar.MoveActive();
    }

    public void EndAttack() {
        this.activeChar.isAttacking = false;
        this.activeChar.attacked = true;
        this.activeChar.usedAction = true;
        UpdateBoard();
        SetActiveChar();
        if(this.activeChar.moveActivations > 0) {
            this.activeChar.MoveActive();
        }
    }

    public void EndSkill() {
        this.activeChar.isSkilling = false;
        this.activeChar.usedSkill = true;
        this.activeChar.usedAction = true;
        if(this.activeChar.moveActivations > 0) {
            this.activeChar.MoveActive();
        }
    }

    public void SetActiveChar() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character.turnOrder == this.turn) {
                this.activeChar = character;
                character.tile.Active();
            }
        }
    }

    public void UpdateBoard() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            character.SetTile();
            character.ResetChar();
        }
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            tile.SetOccupation();
            tile.ResetTile();
        }
    }

    public void Hit(int team) {
        
        if(team == 1) {
            this.hitCount1 += 1;
            camAnimator.SetTrigger("Shake");
            this.lifePoints1[this.hitCount1-1].SetActive(false);
        }
        if(team == 2) {
            this.hitCount2 +=1;
            camAnimator.SetTrigger("Shake");
            this.lifePoints2[5-this.hitCount2].SetActive(false);
        }
        ResetHit();

        if(this.hitCount1 == winHit) {
            this.gameOver = true;
            Team2Over();
        }
        if(this.hitCount2 == winHit) {
            this.gameOver = true;
            Team1Over();
        }
    }
    public void ResetHit() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character.hittable) {character.hittable = false;}
        }
    }
    IEnumerator FadeIn(Image img) {
        for (float i = 0; i <= 1 ; i += Time.deltaTime/3) {
            img.color = new Color(1,1,1,i);
            img.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1,1,1,i);
            if(img.transform.GetChild(1).gameObject.GetComponent<Image>()!=null) {
                img.transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(1,1,1,i);
            }
            yield return null;
        }
    }
    public void Team1Over() {
        team1Wins.SetActive(true);

        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "graveyard") {
            if(!LockVariables.sirCat) {unlockMessages.SetActive(true);}    
            LockVariables.sirCat = true;
        }
        if(currentScene.name == "horror") {
            if(!LockVariables.uki) {unlockMessages.SetActive(true);} 
            LockVariables.uki = true;
        }
        if(currentScene.name == "siblings") {
            if(!LockVariables.beelzebub) {unlockMessages.SetActive(true);} 
            LockVariables.beelzebub = true;
        }

        if(LockVariables.sirCat==true && LockVariables.uki==true && LockVariables.beelzebub==true && LockVariables.all==0) {
            LockVariables.all = 1;
        }

        StartCoroutine(FadeIn(team1Wins.GetComponent<Image>()));
        
    }
    public void Team2Over() {
        team2Wins.SetActive(true);
        StartCoroutine(FadeIn(team2Wins.GetComponent<Image>()));
        
    }

    public void DestinateMove(Char character, Tile tile) {
        float distanceX = tile.positionX - character.positionX;
        float distanceY = tile.positionY - character.positionY;
        character.transform.position += new Vector3(distanceX*scale,distanceY*scale,0);
        character.UpdatePosition();
        UpdateBoard();
        this.activeChar.tile.Active();
        this.movingChar = null;
        UpdateConditions();
        if(character.moveActing == true) {
            this.activeChar.moveActivations -= 1;
            if(this.activeChar.moveActivations > 0) {
                this.activeChar.MoveActive();
            } else {character.moveActing = false;}
        }
    }

    public void UpdateConditions() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            character.Conditions();
        }
    }

    public void BeginTurn() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            character.TurnEffects();
        }
    }

    public Tile GetTile(float X, float Y) {
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (tile.positionX == X && tile.positionY == Y) {
                return tile;
            }
        }
        return null;
    }
}
