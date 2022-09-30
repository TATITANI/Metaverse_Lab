using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerStop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckContact()
    {
        bool FingerContactPlasticTable, FingerContactMetalTable, FingerContactWoodTable;

        FingerContactPlasticTable = Physics.Raycast(this.transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 1.0f, LayerMask.GetMask("PlasticTable")) || Physics.Raycast(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z), 0.4f, LayerMask.GetMask("PlasticTable"));
        FingerContactMetalTable = Physics.Raycast(this.transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 1.0f, LayerMask.GetMask("MetalTable")) || Physics.Raycast(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z), 0.4f, LayerMask.GetMask("MetalTable"));
        FingerContactWoodTable = Physics.Raycast(this.transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 1.0f, LayerMask.GetMask("WoodTable")) || Physics.Raycast(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z), 0.4f, LayerMask.GetMask("WoodTable"));
        
        if (FingerContactPlasticTable || FingerContactMetalTable || FingerContactWoodTable)
        {
            return true;

        }
        else
        {
            return false;
        }

    }
}
