using System;
using UnityEngine;

namespace AstroArc
{
    [Serializable]
    public class DifficultySettings
    {
        public int spawnStart = 48;
        public int spawnMin = 11;
        public float spawnDecay = 3.6f;
        public float speedMult = 1.0f;
        public float hpMult = 0.96f;
        public int toiletsBase = 11;
        public float toiletsGrowth = 1.45f;
        public int maxToilets = 45;
        public int sigmaGain = 9;
        public int breachDamage = 1;
    }

    public enum EnemyType
    {
        AstroTrooper,
        AstroAssailant,
        AstroObliterator,
        AstroJuggernaut,
        AstroDestructor,
        GManBoss
    }

    [Serializable]
    public class EnemyStats
    {
        public EnemyType type;
        public int hp;
        public int maxHp;
        public float speed;
        public int sigmaReward;
        public bool isBoss;
    }
}
