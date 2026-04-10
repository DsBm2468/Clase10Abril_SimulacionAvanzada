using UnityEngine;

public class FreeFallSimulation : MonoBehaviour // CAIDA LIBRE
{
    [Header("Parámetros de físicas")]

    public float initialHeight = 10f; // Altura inicial
    public float initialVelocity = 0f; // Velocidad inicial
    public float gravity = -9.81f; // El valor es negativo debido a que
    public float groundHeight = 0f; // Altura de suelo
    [Range(0f, 1f)] public float restitution = 0.8f;// eLASTICIDAD DE OBJETO SIEMPER DEBE SER ENTRE 0 Y 1
    [Min(0f)] public float dragCoefficient = 0.1f; // coeficiene de resistencia del aire
    [Min(0.0001f)] public float massFactor = 1f; // factor de masa, que tanto la masa influye en el 
    public float stopThreshold = 0.05f; // Pare de rebotar

    private float velocity;
    private float position;
    private float mass;
    private bool isActive = true;

    private void Start() => ResetSimulation(); // Uma forma de hacer un void corto

    private void OnEnable()
    {
        if (SimulationManager.Instance != null) SimulationManager.Instance.OnSimulationStep += Step;
        if (SimulationManager.Instance != null) SimulationManager.Instance.OnSimulationReset += ResetSimulation;
    }

    private void Step(float dt) // dt es diferencial de tiempo
    {

        if (!isActive) return;

        float dragAcceleration = ( -dragCoefficient * velocity ) / mass; // es negativo por ser una fuerza inversa
        float totalAcceleration = gravity + dragAcceleration;

        velocity += totalAcceleration * dt;
        position += velocity * dt;

        if (position <= groundHeight)
        {
            position = groundHeight;
            velocity = -velocity * restitution;
            if (Mathf.Abs(velocity) <= stopThreshold)
            {
                velocity = 0f;
                isActive = false;
            }
        }

        UpdateVisuals();
    }

    private void OnDisable()
    {
        if (SimulationManager.Instance != null) SimulationManager.Instance.OnSimulationStep -= Step;
        if (SimulationManager.Instance != null) SimulationManager.Instance.OnSimulationReset -= ResetSimulation;
    }

    private void UpdateVisuals()
    {
        transform.position = new Vector3(
            transform.position.x,
            position,
            transform.position.z
        );
    }

    private void ResetSimulation()
    {
        computeMass();

        velocity = initialVelocity;
        position = initialHeight;
        isActive = true;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.z
        );
    }

    private void computeMass()
    {
        Vector3 scale = transform.localScale;
        float volume = scale.x * scale.y * scale.z;
        massFactor = volume * massFactor;
    }
}