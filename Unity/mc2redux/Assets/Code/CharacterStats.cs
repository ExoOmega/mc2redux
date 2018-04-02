using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public float health;
    public bool selected;
    public bool dead;
    public int team;
    public bool crouch;
    public bool run;
    public GameObject selectTag;
    PlayerControl plControl;

	// Use this for initialization
	void Start ()
    {
        plControl = GetComponent<PlayerControl>();
        selectTag = this.gameObject.transform.Find("SelectTag").gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        selectTag.SetActive(selected);

        if (run)
        {
            crouch = false;
        }
	}

    public void MoveToPosition(Vector3 position)
    {
        plControl.moveToPosition = true;
        plControl.destPosition = position;
    }
}
