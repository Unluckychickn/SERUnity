using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Drawing;

public class Destroyer : MonoBehaviour
{   
    [SerializeField] private ObjectPooler GroundPooler;
    [SerializeField] private ObjectPooler CoinPooler;
    [SerializeField] private ObjectPooler VisualPooler;
    [SerializeField] private List<string> ValidTags = new List<string>{"SmallGround", "MediumGround", "LargeGround"};

   public void OnTriggerEnter2D(Collider2D collider)
    {  
        if(collider.transform.parent.tag == "StartingGround"){
                Destroy(collider.gameObject.transform.parent.gameObject);
                return;
        }
        if(collider.transform.parent != null)
        {   
            Transform parentTransform = collider.transform.parent;
            string parentTag = parentTransform.tag;

        
        if(ValidTags.Contains(parentTag) && collider.transform.GetComponentInParent<GroundMovement>()!=null){        

        
        foreach (Transform child in collider.transform.parent)
        {
            if (child.CompareTag("Coin"))
            {   child.SetParent(null);
                print($"Set Parent of {child.tag} to {child.transform.parent}");
                CoinPooler.ReturnToPool(child.gameObject.tag, child.gameObject);
                print($"Returned {child.gameObject.tag} to CoinPooler ");
            }
            else if (child.CompareTag("SmallVisual"))
            {    child.SetParent(null);
            print($"Set Parent of {child.tag} to {child.transform.parent}");
                VisualPooler.ReturnToPool(child.gameObject.tag, child.gameObject);
                 print($"Returned {child.gameObject.tag} to SmallVisualPooler ");
            }
            else if (child.CompareTag("MediumVisual"))
            {    child.SetParent(null);
            print($"Set Parent of {child.tag} to {child.transform.parent}");
                VisualPooler.ReturnToPool(child.gameObject.tag, child.gameObject);
                 print($"Returned {child.gameObject.tag} to MediumVisualPooler ");
            }
        }    
        GroundPooler.ReturnToPool(parentTag, collider.transform.parent.gameObject);
        print($"Returned {parentTag} {collider.transform.gameObject} to GroundPooler ");
            
        }
        }else{
            print($"Didnt fit any Tag!: {collider.transform.tag} with name {collider.transform.name}");
        }
    }
}
    


           

    
