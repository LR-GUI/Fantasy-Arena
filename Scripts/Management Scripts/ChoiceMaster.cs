using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceMaster : MonoBehaviour
{

    public List<Char> warriors;
    public List<Char> rogues;
    public List<Char> mages;
    public List<Char> allChars;

    public List<GameObject> warriorIcons = new List<GameObject>();
    public List<GameObject> rogueIcons = new List<GameObject>();
    public List<GameObject> mageIcons = new List<GameObject>();

    public List<GameObject> team1Icons = new List<GameObject>();
    public List<GameObject> team2Icons = new List<GameObject>();

    public List<Char> team1 = new List<Char>();
    public List<Char> team2 = new List<Char>();

    public GameObject card;
    public GameObject passive;

    public Sprite warriorPassive;
    public Sprite roguePassive;
    public Sprite whiteMagePassive;
    public Sprite darkMagePassive;
    public Sprite minionPassive;
    public Sprite placePassive;

    public GameObject ready;
    public GameObject clean;

    public List<GameObject> team1Panel;
    public List<GameObject> team2Panel;

    public GameObject team1highlight;
    public GameObject team2highlight;

    public int turn;
    
    public bool aiReady=false;

    public GameMaster gm;
    


    public Dictionary<Char,Sprite> chars = new Dictionary<Char, Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        ChoiceSetup();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(clean.GetComponent<Button>().interactable && (team1.Count==0&&team2.Count==0)) {
            clean.GetComponent<Button>().interactable=false;
        }
        if(!clean.GetComponent<Button>().interactable && (team1.Count>0||team2.Count>0)) {
            clean.GetComponent<Button>().interactable=true;
        }

        if(ready.GetComponent<Button>().interactable && ((team1.Count<3||team2.Count<3)||!aiReady)) {
            ready.GetComponent<Button>().interactable=false;
        }
        if(!ready.GetComponent<Button>().interactable && ((team1.Count==3&&team2.Count==3)||aiReady)) {
            ready.GetComponent<Button>().interactable=true;
        }
    }

    public void ChoiceSetup() {

        if(LockVariables.sirCat) {
            gm.locks[0].SetActive(false);
            gm.locked[0].SetActive(true);
        }
        if(LockVariables.uki) {
            gm.locks[1].SetActive(false);
            gm.locked[1].SetActive(true);
        }
        if(LockVariables.beelzebub) {
            gm.locks[2].SetActive(false);
            gm.locked[2].SetActive(true);
        }

        if(GameObject.Find("AIbehavior")==null) {
            team1highlight.GetComponent<Image>().color = new Color(1,1,1,1);
            team2highlight.GetComponent<Image>().color = new Color(1,1,1,0.4f);
        }
        

        int i = 0;
        foreach(GameObject icon in warriorIcons) {
            icon.GetComponent<Image>().sprite = warriors[i].iconImage;
            i+=1;
            icon.GetComponent<ChooseIcon>().thisIcon = icon.GetComponent<Image>().sprite;
            foreach(Char character in this.allChars) {
                if(icon.GetComponent<ChooseIcon>().thisIcon == character.iconImage) {
                    icon.GetComponent<ChooseIcon>().thisChar = character;
                }
            }
            
        }
        i = 0;
        foreach(GameObject icon in rogueIcons) {
            icon.GetComponent<Image>().sprite = rogues[i].iconImage;
            i+=1;
            icon.GetComponent<ChooseIcon>().thisIcon = icon.GetComponent<Image>().sprite;
            foreach(Char character in this.allChars) {
                if(icon.GetComponent<ChooseIcon>().thisIcon == character.iconImage) {
                    icon.GetComponent<ChooseIcon>().thisChar = character;
                }
            }
            
        }
        i = 0;
        foreach(GameObject icon in mageIcons) {
            icon.GetComponent<Image>().sprite = mages[i].iconImage;
            i+=1;
            icon.GetComponent<ChooseIcon>().thisIcon = icon.GetComponent<Image>().sprite;
            foreach(Char character in this.allChars) {
                if(icon.GetComponent<ChooseIcon>().thisIcon == character.iconImage) {
                    icon.GetComponent<ChooseIcon>().thisChar = character;
                }
            }
            
        }

        this.turn = 1;
    }
    

    public void Clean() {
        team1.Clear();
        team2.Clear();
        this.turn=1;
        team1highlight.GetComponent<Image>().color = new Color(1,1,1,1);
        team2highlight.GetComponent<Image>().color = new Color(1,1,1,0.4f);
        foreach(GameObject icon in team1Icons) {
            icon.SetActive(false);
        }
        foreach(GameObject icon in team2Icons) {
            icon.SetActive(false);
        }
        foreach(GameObject icon in warriorIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
        foreach(GameObject icon in rogueIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
        foreach(GameObject icon in mageIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
    }
    public void Ready() {
        foreach(Char character in team1) {
            if(character.warrior) {
                Char instance = Instantiate(character,new Vector3(-7.4f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.rogue) {
                Char instance = Instantiate(character,new Vector3(-5.75f,2.4f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.whiteMage || character.darkMage) {
                Char instance = Instantiate(character,new Vector3(-5.75f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
        }
        foreach(Char character in team2) {
            if(character.warrior) {
                Char instance = Instantiate(character,new Vector3(7.45f,4.05f,0f),Quaternion.identity);
                instance.team = 2;
            }
            if(character.rogue) {
                Char instance = Instantiate(character,new Vector3(5.8f,2.4f,0f),Quaternion.identity);
                instance.team = 2;
            }
            if(character.whiteMage || character.darkMage) {
                Char instance = Instantiate(character,new Vector3(5.8f,4.05f,0f),Quaternion.identity);
                instance.team = 2;
            }
        }

        foreach(Char character in team1) {
            if(character.warrior) {
                team1Panel[0].GetComponent<Image>().sprite = character.iconImage;
            }
            if(character.rogue) {
                team1Panel[1].GetComponent<Image>().sprite = character.iconImage;
            }
            if(character.whiteMage||character.darkMage) {
                team1Panel[2].GetComponent<Image>().sprite = character.iconImage;
            }
        }
        foreach(Char character in team2) {
            if(character.warrior) {
                team2Panel[0].GetComponent<Image>().sprite = character.iconImage;
            }
            if(character.rogue) {
                team2Panel[1].GetComponent<Image>().sprite = character.iconImage;
            }
            if(character.whiteMage||character.darkMage) {
                team2Panel[2].GetComponent<Image>().sprite = character.iconImage;
            }
        }
        gm.doneChoosing = true;
        this.gameObject.SetActive(false);
    }

    public void AIClean() {
        aiReady=false;
        team1.Clear();
        foreach(GameObject icon in team1Icons) {
            icon.SetActive(false);
        }
        foreach(GameObject icon in warriorIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
        foreach(GameObject icon in rogueIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
        foreach(GameObject icon in mageIcons) {
            icon.GetComponent<ChooseIcon>().isChosen = false;
            if(icon.activeSelf) {icon.GetComponent<ChooseIcon>().button.interactable = true;}
        }
    }

    public void GraveyardReady() {
        Tile firstTile = GameObject.Find("Tile26").GetComponent<Tile>();
        Tile secondTile = GameObject.Find("Tile15").GetComponent<Tile>();
        Tile thirdTile = GameObject.Find("Tile16").GetComponent<Tile>();
        List<Char> players = new List<Char>(team1);

        Char first = players[Random.Range(0,3)];
        players.Remove(first);
        first = Instantiate(first,firstTile.transform.position,Quaternion.identity);
        first.turnOrder = 1;
        first.team = 1;
        PlaceChar(first);

        Char second = players[Random.Range(0,2)];
        players.Remove(second);
        second = Instantiate(second,secondTile.transform.position,Quaternion.identity);
        second.turnOrder = 3;
        second.team = 1;
        PlaceChar(second);

        Char third = players[0];
        third = Instantiate(third,thirdTile.transform.position,Quaternion.identity);
        third.turnOrder = 5;
        third.team = 1;
        PlaceChar(third);

        Char mausoleum = Instantiate(allChars[10],GameObject.Find("Tile61").GetComponent<Tile>().transform.position,Quaternion.identity);
        mausoleum.team=2;
        PlaceChar(mausoleum);
        FindObjectOfType<AI>().mausoleum = mausoleum;
        FindObjectOfType<AI>().GenerateZombie();
        FindObjectOfType<AI>().GenerateZombie();
        FindObjectOfType<AI>().GenerateZombie();

        gm.placingDone = true;
        gm.doneChoosing = true;
        this.gameObject.SetActive(false);
    }

    public void HorrorReady() {
        foreach(Char character in team1) {
            if(character.warrior) {
                Char instance = Instantiate(character,new Vector3(-7.4f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.rogue) {
                Char instance = Instantiate(character,new Vector3(-5.75f,2.4f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.whiteMage || character.darkMage) {
                Char instance = Instantiate(character,new Vector3(-5.75f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
            
        }
        Char horror = Instantiate(allChars[9],new Vector3(7.45f,4.05f,-7f),Quaternion.identity);
        horror.team=2;
        Char lackey1 = Instantiate(allChars[10],new Vector3(5.8f,4.05f,-8f),Quaternion.identity);
        lackey1.team=2;
        Char lackey2 = Instantiate(allChars[10],new Vector3(5.8f,2.4f,-8f),Quaternion.identity);
        lackey2.team=2;

        FindObjectOfType<HorrorAI>().lackeys.Add(lackey2);
        FindObjectOfType<HorrorAI>().lackeys.Add(lackey1);
        FindObjectOfType<HorrorAI>().horror = horror;



        //gm.placingDone = true;
        gm.doneChoosing = true;
        this.gameObject.SetActive(false);
    }

    public void SiblingsReady() {
        foreach(Char character in team1) {
            if(character.warrior) {
                Char instance = Instantiate(character,new Vector3(-7.4f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.rogue) {
                Char instance = Instantiate(character,new Vector3(-5.75f,2.4f,0f),Quaternion.identity);
                instance.team = 1;
            }
            if(character.whiteMage || character.darkMage) {
                Char instance = Instantiate(character,new Vector3(-5.75f,4.05f,0f),Quaternion.identity);
                instance.team = 1;
            }
            
        }
        Char yin = Instantiate(allChars[9],new Vector3(5.8f,4.05f,-8f),Quaternion.identity);
        yin.team=2;
        Char yang = Instantiate(allChars[10],new Vector3(7.45f,4.05f,-7f),Quaternion.identity);
        yang.team=2;

        FindObjectOfType<SiblingsAI>().yin = yin;
        FindObjectOfType<SiblingsAI>().yang = yang;



        //gm.placingDone = true;
        gm.doneChoosing = true;
        this.gameObject.SetActive(false);
    }

    public void PlaceChar(Char character) {
        character.UpdatePosition();
        character.SetTile();
        character.tile.SetOccupation();
        character.isSet = true;
    }
    
}
