using UnityEngine;
using UnityEngine.UI;

public class MetaInfLoader : MonoBehaviour
{
    public Text metaText;
    public TextAsset asset; // this should be dynamically loaded at runtime

	// Use this for initialization
	void Start ()
	{
        metaText.text = asset.text;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
