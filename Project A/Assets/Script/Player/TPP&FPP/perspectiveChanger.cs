using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perspectiveChanger : MonoBehaviour
{
    public GameObject FirstPersonObj;
    public GameObject ThirdPersonObj;

    public perspective currentPerspective;

    public enum perspective
    {
        FirstPerson,
        ThirdPerson
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) SwtichPerspective(perspective.FirstPerson);
        if (Input.GetKeyDown(KeyCode.B)) SwtichPerspective(perspective.ThirdPerson);
    }

    void SwtichPerspective(perspective newPerspective)
    {
        FirstPersonObj.SetActive(false);
        ThirdPersonObj.SetActive(false);

        if (newPerspective == perspective.FirstPerson) FirstPersonObj.SetActive(true);
        if (newPerspective == perspective.ThirdPerson) ThirdPersonObj.SetActive(true);

        currentPerspective = newPerspective;
    }
}
