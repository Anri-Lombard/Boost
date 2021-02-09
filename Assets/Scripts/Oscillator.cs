using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    [Range(0, 1)] [SerializeField] float movementFactor;

    Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;
    }

    private void Update()
    { 
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2; // Tau is 2 PI
        float rawSineWave = Mathf.Sin(cycles * tau); // One wave is 2 PI (1 Tau)

        movementFactor = rawSineWave / 2f /* = 0.5 and -0.5 */ + 0.5f; // = 1 and 0
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
