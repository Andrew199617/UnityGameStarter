using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Obstacle : MonoBehaviour {

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponent<Player>();
                if (player)
                {
                    player.ChangeHealthByAmount(100);
                }
            }
        }
    }
}
