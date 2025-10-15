// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
// public class MotorcyclePhysics : MonoBehaviour
// {
//     WheelCollider frontWheel;
//     WheelCollider rearWheel;
//     GameObject meshFront;
//     GameObject meshRear;
//     public GameObject frontWheelObject;
//     public GameObject rearWheelObject;
//     public GameObject handle2;
//     Rigidbody ms_Rigidbody;

//     float rbVelocityMagnitude;
//     float horizontalInput;
//     float verticalInput;
//     float medRPM;

//     // 最大旋转角度
//     float maxHandleRotation = 20.0f;

//     // 最大速度
//     float maxSpeed = 20.0f; // 调整为你想要的最大速度

//     void Awake()
//     {
//         ms_Rigidbody = GetComponent<Rigidbody>();
//         ms_Rigidbody.mass = 400;
//         ms_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//         ms_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

//         // CenterOfMass
//         GameObject centerOfmassOBJ = new GameObject("centerOfmass");
//         centerOfmassOBJ.transform.parent = transform;
//         centerOfmassOBJ.transform.localPosition = new Vector3(0.0f, -0.3f, 0.0f);
//         ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfmassOBJ.transform.position);

//         // BoxCollider
//         BoxCollider collider = GetComponent<BoxCollider>();
//         collider.size = new Vector3(0.5f, 1.0f, 3.0f);

//         // Assign WheelColliders
//         if (frontWheelObject != null)
//             frontWheel = frontWheelObject.GetComponent<WheelCollider>();
//         if (rearWheelObject != null)
//             rearWheel = rearWheelObject.GetComponent<WheelCollider>();

//         if (frontWheel == null || rearWheel == null)
//         {
//             Debug.LogError("WheelCollider components are missing from the wheel objects.");
//         }

//         // Generate wheels if not assigned
//         if (frontWheel == null || rearWheel == null)
//         {
//             GenerateWheels();
//         }
//     }

//     void FixedUpdate()
//     {
//         horizontalInput = Input.GetAxis("Horizontal");
//         verticalInput = Input.GetAxis("Vertical");

//         // Apply motor torque based on input, reduced for slower acceleration
//         rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 0.5f; // Further reduced torque

//         // Apply steering angle based on input
//         float nextAngle = horizontalInput * 35.0f;
//         frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle, nextAngle, 0.125f);

//         // Update forward direction based on wheel rotation
//         Vector3 direction = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward).normalized;
//         ms_Rigidbody.AddForce(direction * verticalInput * ms_Rigidbody.mass * 2.5f); // Further reduced force

//         // Apply brakes when not moving and no input
//         if (ms_Rigidbody.velocity.magnitude < 1.0f && Mathf.Abs(verticalInput) < 0.1f)
//         {
//             rearWheel.brakeTorque = frontWheel.brakeTorque = ms_Rigidbody.mass * 2.0f;
//         }
//         else
//         {
//             rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
//         }

//         // Limit the maximum speed
//         if (ms_Rigidbody.velocity.magnitude > maxSpeed)
//         {
//             ms_Rigidbody.velocity = ms_Rigidbody.velocity.normalized * maxSpeed;
//         }

//         // Call stabilizer method for additional stability
//         Stabilizer();
//     }

//     void Update()
//     {
//         UpdateWheelPose(frontWheel, meshFront);
//         UpdateWheelPose(rearWheel, meshRear);
//         UpdateHandle2Rotation();
//     }

//     void UpdateWheelPose(WheelCollider collider, GameObject mesh)
//     {
//         if (collider != null && mesh != null)
//         {
//             Vector3 position;
//             Quaternion rotation;
//             collider.GetWorldPose(out position, out rotation);
//             mesh.transform.position = position;
//             mesh.transform.rotation = rotation;

//             // Rotate the wheel mesh based on RPM and direction, reduced rotation speed
//             float wheelRotation = collider.rpm * 180 / 60 * Time.deltaTime; // Reduced rotation speed
//             mesh.transform.Rotate(Vector3.forward, wheelRotation); // Z轴旋转
//         }
//     }

//     void UpdateHandle2Rotation()
//     {
//         if (handle2 != null)
//         {
//             // Calculate the desired rotation based on horizontal input and max rotation angle
//             float targetZRotation = horizontalInput * maxHandleRotation;

//             // Get the current local rotation of handle2
//             Vector3 currentRotation = handle2.transform.localEulerAngles;

//             // Adjust the current Z rotation to be in the range -180 to 180 for proper clamping
//             float currentZRotation = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;

//             // Calculate the new rotation incrementally
//             float newZRotation = currentZRotation + targetZRotation * Time.deltaTime * 10.0f;

//             // Clamp the rotation to be within the maxHandleRotation limits
//             newZRotation = Mathf.Clamp(newZRotation, -maxHandleRotation, maxHandleRotation);

//             // Apply the rotation while preserving the original x and y rotations
//             handle2.transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newZRotation);
//         }
//     }

//     void Stabilizer()
//     {
//         Vector3 axisFromRotate = Vector3.Cross(transform.up, Vector3.up);
//         Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
//         torqueForce.x = torqueForce.x * 0.4f;
//         torqueForce -= ms_Rigidbody.angularVelocity;
//         ms_Rigidbody.AddTorque(torqueForce * ms_Rigidbody.mass * 0.02f, ForceMode.Impulse);

//         float rpmSign = Mathf.Sign(medRPM) * 0.02f;
//         if (ms_Rigidbody.velocity.magnitude > 1.0f && frontWheel.isGrounded && rearWheel.isGrounded)
//         {
//             ms_Rigidbody.angularVelocity += new Vector3(0, horizontalInput * rpmSign, 0);
//         }
//     }

//     void GenerateWheels()
//     {
//         // Generate front wheel
//         GameObject frontWheelModel = GameObject.Find("frontWheel"); // Assumes your front wheel is named "frontWheel"
//         if (frontWheelModel != null)
//         {
//             frontWheelModel.transform.parent = transform;
//             frontWheelModel.transform.localPosition = new Vector3(0, -0.5f, 1.0f);
//             frontWheel = frontWheelModel.GetComponent<WheelCollider>();
//         }
//         else
//         {
//             Debug.LogError("Cannot find front wheel object, please ensure there is an object named 'frontWheel' in the scene.");
//         }

//         // Generate rear wheel
//         GameObject rearWheelModel = GameObject.Find("rearWheel");
//         if (rearWheelModel != null)
//         {
//             rearWheelModel.transform.parent = transform;
//             rearWheelModel.transform.localPosition = new Vector3(0, -0.5f, -1.0f);
//             rearWheel = rearWheelModel.GetComponent<WheelCollider>();
//         }
//         else
//         {
//             Debug.LogError("Cannot find rear wheel object, please ensure there is an object named 'rearWheel' in the scene.");
//         }

//         // Settings
//         if (frontWheel != null && rearWheel != null)
//         {
//             frontWheel.mass = rearWheel.mass = 40;
//             frontWheel.radius = rearWheel.radius = 0.5f;
//             frontWheel.suspensionDistance = rearWheel.suspensionDistance = 0.3f;

//             JointSpring spring = frontWheel.suspensionSpring;
//             spring.spring = 18500;
//             spring.damper = 1250;
//             spring.targetPosition = 0.75f;
//             frontWheel.suspensionSpring = rearWheel.suspensionSpring = spring;

//             WheelFrictionCurve forwardFriction = frontWheel.forwardFriction;
//             forwardFriction.asymptoteSlip = 0.8f;
//             forwardFriction.asymptoteValue = 0.5f;
//             forwardFriction.extremumSlip = 0.4f;
//             forwardFriction.extremumValue = 1.0f;
//             frontWheel.forwardFriction = rearWheel.forwardFriction = forwardFriction;

//             WheelFrictionCurve sidewaysFriction = frontWheel.sidewaysFriction;
//             sidewaysFriction.asymptoteSlip = 0.5f;
//             sidewaysFriction.asymptoteValue = 0.75f;
//             sidewaysFriction.extremumSlip = 0.25f;
//             sidewaysFriction.extremumValue = 1.0f;
//             frontWheel.sidewaysFriction = rearWheel.sidewaysFriction = sidewaysFriction;
//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class MotorcyclePhysics : MonoBehaviour
{
    WheelCollider frontWheel;
    WheelCollider rearWheel;
    GameObject meshFront;
    GameObject meshRear;
    public GameObject frontWheelObject;
    public GameObject rearWheelObject;
    public GameObject handle2;
    Rigidbody ms_Rigidbody;

    float rbVelocityMagnitude;
    float horizontalInput;
    float verticalInput;
    float medRPM;

    // 最大旋转角度
    float maxHandleRotation = 20.0f;

    // 最大速度
    float maxSpeed = 20.0f; // 调整为你想要的最大速度

    void Awake()
    {
        ms_Rigidbody = GetComponent<Rigidbody>();
        ms_Rigidbody.mass = 400;
        ms_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        ms_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // CenterOfMass
        GameObject centerOfmassOBJ = new GameObject("centerOfmass");
        centerOfmassOBJ.transform.parent = transform;
        centerOfmassOBJ.transform.localPosition = new Vector3(0.0f, -0.3f, 0.0f);
        ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfmassOBJ.transform.position);

        // BoxCollider
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(0.5f, 1.0f, 3.0f);

        // Assign WheelColliders
        if (frontWheelObject != null)
            frontWheel = frontWheelObject.GetComponent<WheelCollider>();
        if (rearWheelObject != null)
            rearWheel = rearWheelObject.GetComponent<WheelCollider>();

        if (frontWheel == null || rearWheel == null)
        {
            Debug.LogError("WheelCollider components are missing from the wheel objects.");
        }

        // Generate wheels if not assigned
        if (frontWheel == null || rearWheel == null)
        {
            GenerateWheels();
        }
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Apply motor torque based on input, reduced for slower acceleration
        rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 0.5f; // Further reduced torque

        // Apply steering angle based on input
        float nextAngle = horizontalInput * 35.0f;
        frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle, nextAngle, 0.125f);

        // Update forward direction based on wheel rotation
        Vector3 direction = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward).normalized;
        ms_Rigidbody.AddForce(direction * verticalInput * ms_Rigidbody.mass * 2.5f); // Further reduced force

        // Apply brakes when not moving and no input
        if (ms_Rigidbody.velocity.magnitude < 1.0f && Mathf.Abs(verticalInput) < 0.1f)
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = ms_Rigidbody.mass * 2.0f;
        }
        else
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
        }

        // Limit the maximum speed
        if (ms_Rigidbody.velocity.magnitude > maxSpeed)
        {
            ms_Rigidbody.velocity = ms_Rigidbody.velocity.normalized * maxSpeed;
        }

        // Call stabilizer method for additional stability
        Stabilizer();

        // Limit the RPM of the wheel colliders
        LimitWheelRPM(frontWheel, 60.0f);
        LimitWheelRPM(rearWheel, 60.0f);
    }

    void Update()
    {
        UpdateWheelPose(frontWheel, meshFront);
        UpdateWheelPose(rearWheel, meshRear);
        UpdateHandle2Rotation();
    }

    void UpdateWheelPose(WheelCollider collider, GameObject mesh)
    {
        if (collider != null && mesh != null)
        {
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            mesh.transform.position = position;
            mesh.transform.rotation = rotation;

            // Rotate the wheel mesh based on RPM and direction, reduced rotation speed
            float wheelRotation = collider.rpm * 180 / 60 * Time.deltaTime; // Reduced rotation speed
            mesh.transform.Rotate(Vector3.forward, wheelRotation); // Z轴旋转
        }
    }

    void UpdateHandle2Rotation()
    {
        if (handle2 != null)
        {
            // Calculate the desired rotation based on horizontal input and max rotation angle
            float targetZRotation = horizontalInput * maxHandleRotation;

            // Get the current local rotation of handle2
            Vector3 currentRotation = handle2.transform.localEulerAngles;

            // Adjust the current Z rotation to be in the range -180 to 180 for proper clamping
            float currentZRotation = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;

            // Calculate the new rotation incrementally
            float newZRotation = currentZRotation + targetZRotation * Time.deltaTime * 10.0f;

            // Clamp the rotation to be within the maxHandleRotation limits
            newZRotation = Mathf.Clamp(newZRotation, -maxHandleRotation, maxHandleRotation);

            // Apply the rotation while preserving the original x and y rotations
            handle2.transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newZRotation);
        }
    }

    void Stabilizer()
    {
        Vector3 axisFromRotate = Vector3.Cross(transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * 0.4f;
        torqueForce -= ms_Rigidbody.angularVelocity;
        ms_Rigidbody.AddTorque(torqueForce * ms_Rigidbody.mass * 0.02f, ForceMode.Impulse);

        float rpmSign = Mathf.Sign(medRPM) * 0.02f;
        if (ms_Rigidbody.velocity.magnitude > 1.0f && frontWheel.isGrounded && rearWheel.isGrounded)
        {
            ms_Rigidbody.angularVelocity += new Vector3(0, horizontalInput * rpmSign, 0);
        }
    }

    void LimitWheelRPM(WheelCollider wheel, float maxRPM)
    {
        if (Mathf.Abs(wheel.rpm) > maxRPM)
        {
            // Apply braking force proportional to the excess RPM
            float brakeTorque = ms_Rigidbody.mass * (Mathf.Abs(wheel.rpm) - maxRPM) * 0.1f;
            wheel.brakeTorque = Mathf.Max(wheel.brakeTorque, brakeTorque);
        }
        else
        {
            wheel.brakeTorque = 0;
        }
    }

    void GenerateWheels()
    {
        // Generate front wheel
        GameObject frontWheelModel = GameObject.Find("frontWheel"); // Assumes your front wheel is named "frontWheel"
        if (frontWheelModel != null)
        {
            frontWheelModel.transform.parent = transform;
            frontWheelModel.transform.localPosition = new Vector3(0, -0.5f, 1.0f);
            frontWheel = frontWheelModel.GetComponent<WheelCollider>();
        }
        else
        {
            Debug.LogError("Cannot find front wheel object, please ensure there is an object named 'frontWheel' in the scene.");
        }

        // Generate rear wheel
        GameObject rearWheelModel = GameObject.Find("rearWheel");
        if (rearWheelModel != null)
        {
            rearWheelModel.transform.parent = transform;
            rearWheelModel.transform.localPosition = new Vector3(0, -0.5f, -1.0f);
            rearWheel = rearWheelModel.GetComponent<WheelCollider>();
        }
        else
        {
            Debug.LogError("Cannot find rear wheel object, please ensure there is an object named 'rearWheel' in the scene.");
        }

        // Settings
        if (frontWheel != null && rearWheel != null)
        {
            frontWheel.mass = rearWheel.mass = 40;
            frontWheel.radius = rearWheel.radius = 0.5f;
            frontWheel.suspensionDistance = rearWheel.suspensionDistance = 0.3f;

            JointSpring spring = frontWheel.suspensionSpring;
            spring.spring = 18500;
            spring.damper = 1250;
            spring.targetPosition = 0.75f;
            frontWheel.suspensionSpring = rearWheel.suspensionSpring = spring;

            WheelFrictionCurve forwardFriction = frontWheel.forwardFriction;
            forwardFriction.asymptoteSlip = 0.8f;
            forwardFriction.asymptoteValue = 0.5f;
            forwardFriction.extremumSlip = 0.4f;
            forwardFriction.extremumValue = 1.0f;
            frontWheel.forwardFriction = rearWheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = frontWheel.sidewaysFriction;
            sidewaysFriction.asymptoteSlip = 0.5f;
            sidewaysFriction.asymptoteValue = 0.75f;
            sidewaysFriction.extremumSlip = 0.25f;
            sidewaysFriction.extremumValue = 1.0f;
            frontWheel.sidewaysFriction = rearWheel.sidewaysFriction = sidewaysFriction;
        }
    }
}