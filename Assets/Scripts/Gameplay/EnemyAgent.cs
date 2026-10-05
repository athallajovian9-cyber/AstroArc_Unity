using System;
using UnityEngine;

namespace AstroArc
{
    public class EnemyAgent : MonoBehaviour
    {
        public EnemyStats stats;
        public Action<EnemyAgent> OnDeath;
        public Action<EnemyAgent> OnBreach;

        private float currentHp;

        public void Initialize(EnemyStats initialStats)
        {
            stats = initialStats;
            currentHp = stats.hp;
        }

        private void Update()
        {
            // Move down toward the defense lane baseline
            transform.Translate(Vector3.down * stats.speed * Time.deltaTime);

            // Breach check (passed player baseline)
            if (transform.position.y < -6.0f)
            {
                OnBreach?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp <= 0)
            {
                OnDeath?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
