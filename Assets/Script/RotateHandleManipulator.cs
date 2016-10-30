using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class RotateHandleManipulator : MonoBehaviour {

    [Tooltip("How much to scale each axis of hand movement (camera relative) when manipulating the object")]
    public Vector3 handPositionScale = new Vector3(2.0f, 2.0f, 4.0f);  // Default tuning values, expected to be modified per application

    private Vector3 initialHandPosition;
    private Vector3 initialObjectPosition;

    private Interpolator targetInterpolator;

    private bool Manipulating { get; set; }

    private RotateHandlePosition rotateHandlePositionScript;
    private Vector3 axis;
    private Vector3 proj;
    private GameObject target;
    private Vector3 handleInitialPosition;
    private Quaternion targetInitialRotation;
    private Vector3 targetInitialPosition;

    // Use this for initialization
    void Start()
    {
        rotateHandlePositionScript = GetComponent<RotateHandlePosition>();
        Vector3 p = rotateHandlePositionScript.RelativePosition;
        p.x = p.x == 0 ? 1 : 0;
        p.y = p.y == 0 ? 1 : 0;
        p.z = p.z == 0 ? 1 : 0;
        axis = p;
        proj = new Vector3(1, 1, 1) - axis;
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

                handleInitialPosition = transform.position;
                targetInitialRotation = target.transform.rotation;
                targetInitialPosition = target.transform.position;
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

            Vector3 from = handleInitialPosition - targetInitialPosition;
            from.Scale(proj);
            Vector3 to = worldObjectPosition - targetInitialPosition;
            to.Scale(proj);
            Vector3 diff = to - from;
            if (diff.sqrMagnitude > 0 && from.sqrMagnitude > 0) 
            {
                float length = diff.magnitude;
                float refLength = from.magnitude;
                float angle = length / refLength * 90;
                Vector3 normal = Vector3.Cross(from, to);
                if (Vector3.Dot(normal, axis) < 0)
                {
                    angle = -angle;
                }
                Quaternion q = Quaternion.AngleAxis(angle, axis);
                Quaternion newRotation = targetInitialRotation * q;

                // If the object has an interpolator we should use it, otherwise just move the transform directly
                if (targetInterpolator != null)
                {
                    targetInterpolator.SetTargetRotation(newRotation);
                }
                else
                {
                    target.transform.localRotation = newRotation;
                }
            }
        }
    }
}
