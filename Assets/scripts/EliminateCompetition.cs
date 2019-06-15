using UnityEngine;

public class EliminateCompetition : MonoBehaviour
{
    public int deleted;
    public bool alreadyGone = false;
    public int maxDeletedNum=0;
    public float currentValue;
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        deleted = 0;
    }
    void OnTriggerEnter(Collider anotherObj)
    {
        if (!anotherObj.gameObject.GetComponent<EliminateCompetition>().alreadyGone)
        {
            alreadyGone = true;
            anotherObj.gameObject.GetComponent<EliminateCompetition>().updateDeleted(deleted);
            Destroy(this.gameObject);
        }
    }
    public void updateDeleted(int amount)
    {
        if (amount <= deleted)
        {
            deleted++;
        } else
        {
            deleted = amount + 1;
        }
        GetComponentInParent<IO_Importer2>().maxEliminated = 
            deleted > GetComponentInParent<IO_Importer2>().maxEliminated ? 
            deleted : GetComponentInParent<IO_Importer2>().maxEliminated;

    }
    
    private void Update()
    {
        maxDeletedNum = GetComponentInParent<IO_Importer2>().maxEliminated;
        if (transform.parent.childCount > 1000)
        {
            if(deleted < GetComponentInParent<IO_Importer2>().maxEliminated*0.5)
            {
                this.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                this.GetComponent<MeshRenderer>().enabled = true;
                SetColor();
            }
        }  
        SetColor(); //Do zakomentowania!
    }
    
    private void SetColor()
    {
        if(maxDeletedNum!=0)
        {
            currentValue = ((1f / maxDeletedNum) * deleted*1.0f)*1.0f;
            if (currentValue > 1)
                currentValue = 1;
            meshRenderer.material.color = new Color(0f, 1 - currentValue, currentValue);
        }
    }
}
