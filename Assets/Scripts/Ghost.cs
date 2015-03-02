using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {

    private float startTime;

    public float lifetime = 3f;

    private float alpha;

    public float initialAlpha = 0.8f;

    public float finalAlpha = 0.4f;

    private SpriteRenderer spriteRenderer;

    public Sprite sprite;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        alpha = 1;
	}
	
	// Update is called once per frame
	void Update () {
        float diff = Time.time - startTime;
        
        float width = initialAlpha - finalAlpha;

        alpha = initialAlpha - width * diff / lifetime;
        

        spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

        if (diff > lifetime)
        {
            Destroy(gameObject);
            
        }
       
	}
}
