using System.Collections;
using UnityEngine;

public class FovChangeTest1 : MonoBehaviour
{

    public playermovement pm;
    public Camera cm;
    public float initialfov;
    public float newfov;
    public float currentfov { get; private set; }
    public float desiredfov;
    public float fovmultiplier;
    public KeyCode camerakey = KeyCode.R;
    public bool movespeedtime;
    public bool movespeedscale;
    public float movespeed;
    private float movescale;

    // Start is called before the first frame update
    void Start()
    {
        cm.fieldOfView = initialfov;
        currentfov = cm.fieldOfView;
        initialfov = currentfov;
        desiredfov = currentfov;


    }

    // Update is called once per frame
    void Update()
    {

        if (movespeedtime)
        {
            movespeed = pm.speed;
        }
        else
        {
            movespeed = 1;
        }

        if (!movespeedscale)
        {
            movescale = 0;
        }
        else
        {
            movescale = pm.speed;
        }

        CheckInputForFovChange();



    }

    void CheckInputForFovChange()
    {
        if (Input.GetKeyDown(camerakey) || Input.GetKeyDown(pm.sprintkey))
        {
            desiredfov = newfov;
            StopAllCoroutines(); // Stop any ongoing Coroutine
            StartCoroutine(SmoothLerpCamera());

        }
        else if (Input.GetKeyUp(camerakey) || Input.GetKeyUp(pm.sprintkey))
        {
            desiredfov = initialfov;
            StopAllCoroutines(); // Stop any ongoing Coroutine
            StartCoroutine(SmoothLerpCamera2());
        }
    }


        private IEnumerator SmoothLerpCamera()
        {
        float time = 0f;
        float startValue = currentfov;
        float difference = Mathf.Abs(desiredfov + movescale - startValue);

        while (time < difference)
        {

            currentfov = Mathf.Lerp(startValue, desiredfov + movescale, time / difference);
            cm.fieldOfView = currentfov; // Applying the FOV change to the camera
            time += Time.deltaTime * fovmultiplier * movespeed;
            yield return null;
        }

        currentfov = desiredfov + movescale;
        cm.fieldOfView = currentfov; // Final adjustment to ensure it ends at the desired value
        }

    private IEnumerator SmoothLerpCamera2()
    {
        float time = 0f;
        float startValue = currentfov;
        float difference = Mathf.Abs(desiredfov - startValue);

        while (time < difference)
        {

            currentfov = Mathf.Lerp(startValue, desiredfov, time / difference);
            cm.fieldOfView = currentfov; // Applying the FOV change to the camera
            time += Time.deltaTime * fovmultiplier * movespeed;
            yield return null;
        }

        currentfov = desiredfov;
        cm.fieldOfView = currentfov; // Final adjustment to ensure it ends at the desired value
    }

}