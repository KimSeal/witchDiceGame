using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class needDiceInfoHover : MonoBehaviour
{
    [SerializeField]
    public GameObject hoverInfo;
    // Start is called before the first frame update
    void Start()
    {
        hoverInfo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        hoverInfo.SetActive(true);
    }
    private void OnMouseExit()
    {
        hoverInfo.SetActive(false);

    }
}
