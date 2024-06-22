using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class Adjustment : MonoBehaviour
    {
        // parametry wejœciowe
        public int capacity = 0;
        public double killingDelay = 0;
        public int damageTaken = 0;

        //parametry wyjœciowe
        public float maxHealth = 0f;
        public float damage = 0f;
        public float delayBetweenShots = 0f;
        public float bulletSpreadAngle = 0f;

        // Start is called before the first frame update
        public bool MyBool { get; set; }
        void Start()
        {
            //var m_ProjectileBase = GetComponent<ProjectileStandard>();
            //var m_ProjectileBase = GetComponent<Weaponcon>();
            //Debug.Log(m_ProjectileBase);
            ParamaterSet();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ParamaterSet()
        {
            MyBool = true;
        }
    }
}
