using UnityEngine;

public class PlayersPartController : MonoBehaviour {
    public GameObject player;
    private new Rigidbody rigidbody;
    private float radius;
    private float playerRadius;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius;
        playerRadius = GetComponent<SphereCollider>().radius;
    }

    void Update() {
        rigidbody.velocity = (player.transform.position - transform.position) * GetVelocityMultiplier();
    }

    private float GetVelocityMultiplier() {
        var distance = (player.transform.position - transform.position).magnitude;
        if (distance <= radius + playerRadius) return 0;
        return distance * Mathf.Log(distance);
    }
}
