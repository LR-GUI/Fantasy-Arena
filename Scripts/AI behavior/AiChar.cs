using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChar : Char
{

    //public List<Char> effectedChars new List<Char>;
    //public GameObject effectPrefab;
    //private GameObject effectEffect;

    void Update() { // for triggers
        if(!this.isSet) {this.isSet = true;}
    }

    public override void AlternativeAbilities() { // override to sinalize alternative use of abilities
        hasAlternativeMoveSkill = true;           // when you do, you turn on the calls to AlternativeMove()
    }

    public override void AlternativeMove(Char character) { // attacks are wired to make chars hittable and
                                                            // skills are wired to make chars movable
                                                            // if yours is different, turn this on to modify.
                                                            // this is automatically called for alternative skills (in Char.ChooseMove())
                                                            // and when selecting .targeted chars
                                                            // the character param is the chosen target (is being clicked on)

        //if(this.isAttacking) {} to rewire attacks
        //if(this.isSkilling) {} to rewire skills 
    
    }

    public override void MoveActive() {} // override for when chars have alternative move activations


    public override void TurnEffects() { // called whenever a turn begins, to update turn-based effects

    }

    public override void Conditions() { // called whenever the board changes, to update board-based effects
                                        // or to take care of multiple steps abilities (because everytime the board
                                        // changes, you can trigger the next change using this)

    }

    public override void Attack() { // this is for when A is pressed; makes target tiles hittable then makes actual hittable chars hittable

    }
    
    public override void Skill() { // this is for when S is pressed; makes only tiles movable (or targeted, if rewired), the tile script takes care of changing chars

    }
    
}
