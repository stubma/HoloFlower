using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class ResizeHandleManipulator : MonoBehaviour {

    [Tooltip("How much to scale each axis of hand movement (camera relative) when manipulating the object")]
    public Vector3 handPositionScale = new Vector3(2.0f, 2.0f, 4.0f);  // Default tuning values, expected to be modified per application

    private Vector3 initialHandPosition;
    private Vector3 initialObjectPosition;

    private Interpolator targetInterpolator;

    private bool Manipulating { get; set; }

    private GameObject target;
    private ResizeHandlePosition resizeHandlePositionScript;
    private Vector3 targetInitialPosition;
    private Vector3 targetInitialScale;
    private Vector3 initialPosition;
    private Vector3 initialAnchorPosition;

    void Start()
    {
        resizeHandlePositionScript = GetComponent<ResizeHandlePosition>();
    }

    private void OnEnable()
    {
        if (GestureManager.Instance != null)
        {
            GestureManager.Instance.ManipulationStarted += BeginManipulation;
            GestureManager.Instance.ManipulationCompleted += EndManipulation;
            GestureManager.Instance.ManipulationCanceled += EndManipulation;
        }
        else
        {
            Debug.LogError(string.Format("GestureManipulator enabled on {0} could not find GestureManager instance, manipulation will not function", name));
        }
    }

    private void OnDisable()
    {
        if (GestureManager.Instance)
        {
            GestureManager.Instance.ManipulationStarted -= BeginManipulation;
            GestureManager.Instance.ManipulationCompleted -= EndManipulation;
            GestureManager.Instance.ManipulationCanceled -= EndManipulation;
        }

        Manipulating = false;
    }

    private void BeginManipulation()
    {
        if (GestureManager.Instance != null && GestureManager.Instance.ManipulationInProgress)
        {
            if (GestureManager.Instance.FocusedObject == gameObject && TargetManager.Instance.GetTarget() != null)
            {
                Manipulating = true;

                target = TargetManager.Instance.GetTarget();
                targetInterpolator = target.GetComponent<Interpolator>(); 

                // In order to ensure that any manipulated objects move with the user, we do all our math relative to the camera,
                // so when we save the initial hand position and object position we first transform them into the camera's coordinate space
                initialHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
                initialObjectPosition = Camera.main.transform.InverseTransformPoint(transform.position);

                targetInitialPosition = target.transform.position;
                targetInitialScale = target.transform.localScale;
                initialPosition = transform.position;

                initialAnchorPosition = target.GetComponent<Renderer>().bounds.center;
                initialAnchorPosition.y = target.GetComponent<Renderer>().bounds.min.y;
                //resizeHandlePositionScript.GetResizeAnchorPosition();
            }
        }
    }

    private void EndManipulation()
    {
        Manipulating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Manipulating)
        {
            // First step is to figure out the delta between the initial hand position and the current hand position
            Vector3 localHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
            Vector3 initialHandToCurrentHand = localHandPosition - initialHandPosition;

            // When performing a manipulation gesture, the hand generally only translates a relatively small amount.
            // If we move the object only as much as the hand itself moves, users can only make small adjustments before
            // the hand is lost and the gesture completes.  To improve the usability of the gesture we scale each
            // axis of hand movement by some amount (camera relative).  This value can be changed in the editor or
            // at runtime based on the needs of individual movement scenarios.
            Vector3 scaledLocalHandPositionDelta = Vector3.Scale(initialHandToCurrentHand, handPositionScale);

            // Once we've figured out how much the object should move relative to the camera we apply that to the initial
            // camera relative position.  This ensures that the object remains in the appropriate location relative to the camera
            // and the hand as the camera moves.  The allows users to use both gaze and gesture to move objects.  Once they
            // begin manipulating an object they can rotate their head or walk around and the object will move with them
            // as long as they maintain the gesture, while still allowing adjustment via hand movement.
            Vector3 localObjectPosition = initialObjectPosition + scaledLocalHandPositionDelta;
            Vector3 worldObjectPosition = Camera.main.transform.TransformPoint(localObjectPosition);

            float scale = (worldObjectPosition - initialAnchorPosition).magnitude / (initialPosition - initialAnchorPosition).magnitude;
            Vector3 newScale = targetInitialScale * scale;
            Vector3 newPosition = (targetInitialPosition - initialAnchorPosition) * scale + initialAnchorPosition;
            
            // If the object has an interpolator we should use it, otherwise just move the transform directly
            if (targetInterpolator != null)
            {
                targetInterpolator.SetTargetPosition(newPosition);
                targetInterpolator.SetTargetLocalScale(newScale);
            }
            else
            {
                target.transform.position = newPosition;
                target.transform.localScale = newScale;
            }
        }
    }
}
