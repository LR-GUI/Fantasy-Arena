using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseIcon : MonoBehaviour
{
    private ChoiceMaster cm;
    public bool isChosen;
    public Button button;
    public Sprite thisIcon;
    public Char thisChar;
    
    
    void Start()
    {
        cm = FindObjectOfType<ChoiceMaster>();
        button = this.GetComponent<Button>();
        isChosen = false;
        if(button!=null) {
            button.interactable = true;
        }
    }

    void Update() {
        
    }

    public void Card() {
        foreach(Char character in cm.allChars) {
            if(character.iconImage == this.GetComponent<Image>().sprite) {
                cm.card.GetComponent<Image>().sprite = character.cardImage;
                
                if(character.warrior) {
                    cm.passive.GetComponent<Image>().sprite = cm.warriorPassive;
                }
                if(character.rogue) {
                    cm.passive.GetComponent<Image>().sprite = cm.roguePassive;
                }
                if(character.whiteMage) {
                    cm.passive.GetComponent<Image>().sprite = cm.whiteMagePassive;
                }
                if(character.darkMage) {
                    cm.passive.GetComponent<Image>().sprite = cm.darkMagePassive;
                }
                if(character.minion) {
                    cm.passive.GetComponent<Image>().sprite = cm.minionPassive;
                }
                if(character.place) {
                    cm.passive.GetComponent<Image>().sprite = cm.placePassive;
                }
                
                cm.card.SetActive(true);
                cm.passive.SetActive(true);

            }
        }
    }

    public void ChooseChar() {
        if(cm.turn == 1) {
            bool canAdd = true;
            
            foreach(Char character in cm.team1) {
                if( (character.warrior&&thisChar.warrior) || (character.rogue&&thisChar.rogue) || (character.whiteMage&&(thisChar.whiteMage||thisChar.darkMage)) || (character.darkMage&&(thisChar.whiteMage||thisChar.darkMage))) {
                    canAdd = false;
                }
            }
            if(canAdd) {
                cm.team1highlight.GetComponent<Image>().color = new Color(1,1,1,0.4f);
                cm.team2highlight.GetComponent<Image>().color = new Color(1,1,1,1);
                cm.team1.Add(thisChar);
                foreach(GameObject icon in cm.team1Icons) {
                    if(!icon.activeSelf) {
                        icon.SetActive(true);
                        icon.GetComponent<Image>().sprite = thisIcon;
                        this.isChosen = true;
                        break;
                    }
                }
            }
        }
        if(cm.turn == 2) {
            bool canAdd = true;
            
            foreach(Char character in cm.team2) {
                if( (character.warrior&&thisChar.warrior) || (character.rogue&&thisChar.rogue) || (character.whiteMage&&(thisChar.whiteMage||thisChar.darkMage)) || (character.darkMage&&(thisChar.whiteMage||thisChar.darkMage))) {
                    canAdd = false;
                }
            }
            if(canAdd) {
                cm.team2.Add(thisChar);
                if(cm.team2.Count<3) {
                    cm.team1highlight.GetComponent<Image>().color = new Color(1,1,1,1);
                }
                cm.team2highlight.GetComponent<Image>().color = new Color(1,1,1,0.4f);
                foreach(GameObject icon in cm.team2Icons) {
                    if(!icon.activeSelf) {
                        icon.GetComponent<Image>().sprite = thisIcon;
                        icon.SetActive(true);
                        this.isChosen = true;
                        break;
                    }
                }
            }
        }

        if(isChosen && button.interactable) {
                if(cm.turn==1) {cm.turn=2; button.interactable = false;}
                if(cm.turn==2 && button.interactable) {cm.turn=1; button.interactable = false;}
        }

    }

    public void AIChooseChar() {
        bool canAdd = true;
            
        foreach(Char character in cm.team1) {
            if( (character.warrior&&thisChar.warrior) || (character.rogue&&thisChar.rogue) || (character.whiteMage&&(thisChar.whiteMage||thisChar.darkMage)) || (character.darkMage&&(thisChar.whiteMage||thisChar.darkMage))) {
                canAdd = false;
            }
        }
        if(canAdd) {
            cm.team1.Add(thisChar);
            foreach(GameObject icon in cm.team1Icons) {
                if(!icon.activeSelf) {
                    icon.SetActive(true);
                    icon.GetComponent<Image>().sprite = thisIcon;
                    this.isChosen = true;
                    break;
                }
            }
        }
        if(isChosen && button.interactable) {
            button.interactable = false;
        }
        if(cm.team1.Count==3) {
            cm.aiReady=true;
        }
    }
    
}
