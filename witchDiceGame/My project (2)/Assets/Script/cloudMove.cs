using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudMove : MonoBehaviour
{
    float initX;
    float spd;
    // Start is called before the first frame update
    void Start()
    {
        cloudSpriteChange();
        initX = gameObject.transform.position.x;
        spd = Random.Range(0.3f, 0.8f);
        gameObject.transform.position = new Vector3(Random.Range(-800f,-200f), Random.Range(-600f, -400f), 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position += new Vector3(spd, 0.0f, 0.0f);
        if(gameObject.transform.position.x - initX > 600)
        {
            gameObject.transform.position = new Vector3(-800f, Random.Range(-600f, -400f), gameObject.transform.position.z);
            cloudSpriteChange();
            spd = Random.Range(0.3f, 0.8f);
        }
    }

    private void cloudSpriteChange()
    {
        int spriteTemp = Random.Range(0, 3);
        Debug.Log("cloud sprite is...! " + spriteTemp);
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_cloud_" + spriteTemp.ToString());
    }

    public void cloudStop()
    {
        spd = 0;
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");
    }
    public void cloudActive()
    {
        cloudSpriteChange();
        spd = Random.Range(0.3f, 0.8f);
        gameObject.transform.position = new Vector3(Random.Range(-800f, -200f), Random.Range(-600f, -400f), 0);
    }
    
}
