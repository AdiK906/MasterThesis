using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        [Tooltip("Damage of enemy_eyelazers which gives player")]
        public float Damage = 10f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        bool m_IsDead;

        public DataManager m_DataManager;

        public bool easyMode;
        public bool mediumMode;
        public bool hardMode;
        public bool adjustmentMode;

        void Start()
        {
            easyMode = PlayerPrefs.GetInt("EasyMode") == 1 ? true : false;
            mediumMode = PlayerPrefs.GetInt("MediumMode") == 1 ? true : false;
            hardMode = PlayerPrefs.GetInt("HardMode") == 1 ? true : false;
            adjustmentMode = PlayerPrefs.GetInt("AdjustmentMode") == 1 ? true : false;

            if (adjustmentMode)
            {
                m_DataManager = gameObject.AddComponent<DataManager>();
            }

            CurrentHealth = MaxHealth;
        }

        public void Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
                return;

            if (gameObject.tag == "Player")
            {
                if (adjustmentMode)
                {
                    damage = Damage;
                }
                if (easyMode)
                {
                    damage = 7;
                }
                if (mediumMode)
                {
                    damage = 15;
                }
                if (hardMode)
                {
                    damage = 30;
                }
            }

            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnDamage action
            float trueDamageAmount = healthBefore - CurrentHealth;
            if (trueDamageAmount > 0f)
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource);
            }

            HandleDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f;

            // call OnDamage action
            OnDamaged?.Invoke(MaxHealth, null);

            HandleDeath();
        }

        void HandleDeath()
        {
            if (adjustmentMode)
            {
                if (gameObject.tag == "Enemy_HoverBot")
                {
                    m_DataManager.Accuracy++;
                }

                if (gameObject.tag == "Player")
                {
                    m_DataManager.DamageTaken++;
                }
            }

            if (m_IsDead)
                return;

            // call OnDie action
            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;

                if (adjustmentMode)
                {
                    m_DataManager.KillFrequency++;
                }

                OnDie?.Invoke();
            }
        }
    }
}