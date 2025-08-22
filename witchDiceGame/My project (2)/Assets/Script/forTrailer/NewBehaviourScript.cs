using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    public float xSpd = 1.3f;
    [SerializeField]
    public float jumpAmount = 20f;
    [SerializeField]
    public float jumpSpd = 0.01f;
    [SerializeField]
    public float jumpDelay = 0.1f;
    [SerializeField]
    public float spdVal = 2.0f;

    Vector3 defaultPoint = new Vector3(-746f, -522f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void OnMouseUp()
    {
        StartCoroutine(trailerAilMove());
    }
    private IEnumerator trailerAilMove() {
        this.transform.position = defaultPoint;
        yield return new WaitForSeconds(1.0f);
        gameObject.GetComponent<SpriteRenderer>().flipX = true;

        spdVal *= 2.0f;
        for (int helo = 0; helo < 4;  helo++) {
            for (float i = 0.0f; i * spdVal < 1.0f; i += jumpSpd)
            {
                this.transform.position += new Vector3(xSpd, 0f, 0f);
                this.transform.position =
                    new Vector3(this.transform.position.x, defaultPoint.y + jumpAmount * Mathf.Sin(spdVal * i * Mathf.PI), this.transform.position.z);
                yield return new WaitForSeconds(jumpSpd);
            }
            yield return new WaitForSeconds(jumpDelay);
        }
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
        yield return new WaitForSeconds(0.5f);

        spdVal /= 2.0f;
        for (float i = 0.0f; i * spdVal < 1.0f; i += jumpSpd)
        {
            this.transform.position += new Vector3(xSpd, 0f, 0f);
            this.transform.position =
                new Vector3(this.transform.position.x, defaultPoint.y + jumpAmount * Mathf.Sin(spdVal * i * Mathf.PI), this.transform.position.z);
            yield return new WaitForSeconds(jumpSpd);
        }
        yield return new WaitForSeconds(jumpDelay);
        for (float i = 0.0f; i * spdVal < 1.0f; i += jumpSpd)
        {
            this.transform.position += new Vector3(xSpd, 0f, 0f);
            this.transform.position =
                new Vector3(this.transform.position.x, defaultPoint.y + jumpAmount * Mathf.Sin(spdVal * i * Mathf.PI), this.transform.position.z);
            yield return new WaitForSeconds(jumpSpd);
        }
    }


}
