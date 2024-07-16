using UnityEngine;

public class ClocksRotation : MonoBehaviour
{
    [SerializeField] private Transform minClockHandler; // Reference to the minute hand
    [SerializeField] private Transform hourClockHandler; // Reference to the hour hand

    [SerializeField] private float minuteHandSpeed = 6f; // Degrees per second for the minute hand
    [SerializeField] private float hourHandSpeed = 0.1f; // Degrees per second for the hour hand

    void Update()
    {
        RotateHands();
    }

    void RotateHands()
    {
        // Rotate the minute hand counterclockwise
        minClockHandler.Rotate(0, 0, -(minuteHandSpeed * Time.deltaTime));

        // Rotate the hour hand counterclockwise
        hourClockHandler.Rotate(0, 0, -(hourHandSpeed * Time.deltaTime));
    }
}