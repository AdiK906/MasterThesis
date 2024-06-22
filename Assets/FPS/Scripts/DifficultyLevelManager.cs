using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.Gameplay
{
    public class DifficultyLevelManager : MonoBehaviour
    {
        // TENSOR
        public NNModel modelAsset;
        private IWorker worker;

        //Input parameters
        public float accuracy = 0;
        public float killingDelay = 0;
        public float damageTaken = 0;
        public float bulletShooted = 0;
        public float accuracyDifference = 0;

        private float previousAccuracy = 0f;
        private float previousKillingDelay = 0f;
        private float previousBulletShooted = 0f;
        private float previousDamageTaken = 0f;

        //Output parameters
        public float maxHealth = 0f;
        public float damage = 0f;
        public float delayBetweenShots = 0f;

        //Text values
        public float damageValue = 0f;
        public float delayBetweenShootsValue = 0f;
        public float maxHealthValue = 0f;

        //Game objects
        public static List<DataManager> dataManagersList = new List<DataManager>();
        public GameObject[] enemyGameObjects;
        public GameObject playerGameObject;
        public GameObject[] enemyWeaponGameObjects;
        public GameObject playerWeaponGameObject;

        public bool adjustmentMode;

        public Text displayText;

        void Start()
        {
            adjustmentMode = PlayerPrefs.GetInt("AdjustmentMode") == 1 ? true : false;
            if (adjustmentMode)
            {
                displayText.text = "";

                //zaci¹ganie modelu treningowego
                var model = ModelLoader.Load(modelAsset);

                worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);

                //inicjalizacja komponentów
                enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy_HoverBot");
                playerGameObject = GameObject.FindGameObjectWithTag("Player");
                playerWeaponGameObject = GameObject.FindGameObjectWithTag("PlayerWeapon");

                foreach (GameObject enemy in enemyGameObjects)
                {
                    Health health = enemy.GetComponent<Health>();
                    if (health is not null)
                    {
                        dataManagersList.Add(health.m_DataManager);
                    }
                }

                Health playerHealth = playerGameObject.GetComponent<Health>();
                if (playerHealth is not null)
                {
                    dataManagersList.Add(playerHealth.m_DataManager);
                }

                WeaponController playerWeapon = playerWeaponGameObject.GetComponent<WeaponController>();
                if (playerWeapon is not null)
                {
                    dataManagersList.Add(playerWeapon.m_DataManager);
                }

                InvokeRepeating("ProcessEnemyParameters", 10.0f, 10.0f);
            }
        }

        void ProcessEnemyParameters()
        {
            var inputData = CalculateInputData();

            //Przekszta³æ dane wejœciowe na Tensor
            Tensor inputTensor = new Tensor(1, 3, inputData);

            //Wykonaj przewidywanie
            worker.Execute(inputTensor);
            Tensor outputTensor = worker.PeekOutput();

            var outputData = outputTensor.ToReadOnlyArray();

            SetEnemyParameters(outputData);

            //Zwolnij zasoby
            inputTensor.Dispose();
            outputTensor.Dispose();
        }

        public float[] CalculateInputData()
        {
            float currentAccuracy = dataManagersList.Sum(x => x.Accuracy);
            float currentKillingDelay = dataManagersList.Sum(x => x.KillFrequency);
            float currentBulletShooted = dataManagersList.Sum(x => x.BulletShooted);
            float currentDamageTaken = dataManagersList.Sum(x => x.DamageTaken);

            accuracy = Mathf.Max(0f, currentAccuracy - previousAccuracy);
            killingDelay = Mathf.Max(0f, currentKillingDelay - previousKillingDelay);
            bulletShooted = Mathf.Max(0f, currentBulletShooted - previousBulletShooted);
            damageTaken = Mathf.Max(0f, currentDamageTaken - previousDamageTaken);

            previousAccuracy = currentAccuracy;
            previousKillingDelay = currentKillingDelay;
            previousBulletShooted = currentBulletShooted;
            previousDamageTaken = currentDamageTaken;

            return new float[] { accuracy, killingDelay, damageTaken };
        }

        public void SetEnemyParameters(float[] outputData)
        {
            enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy_HoverBot");
            enemyWeaponGameObjects = GameObject.FindGameObjectsWithTag("Enemy_EyeLazers");
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerWeaponGameObject = GameObject.FindGameObjectWithTag("PlayerWeapon");

            var playerHealth = playerGameObject.GetComponent<Health>();
            if (playerHealth.Damage + outputData[0] >= 7
                && playerHealth.Damage + outputData[0] <= 35)
            {
                playerHealth.Damage += outputData[0];
                damageValue = playerHealth.Damage;
            }

            foreach (var enemyGameObject in enemyGameObjects)
            {
                var enemyHealth = enemyGameObject?.GetComponent<Health>();
                if (enemyHealth is not null
                    && enemyHealth.MaxHealth + outputData[2] >= 70
                    && enemyHealth.MaxHealth + outputData[2] <= 130)
                {
                    enemyHealth.MaxHealth += outputData[2];
                    enemyHealth.CurrentHealth += outputData[2];
                    maxHealthValue = enemyHealth.MaxHealth;
                }

            }
            foreach (var enemyWeaponGameObject in enemyWeaponGameObjects)
            {
                var enemyWeaponController = enemyWeaponGameObject?.GetComponent<WeaponController>();
                if (enemyWeaponController is not null
                    && enemyWeaponController.DelayBetweenShots + outputData[1] >= 0.5
                    && enemyWeaponController.DelayBetweenShots + outputData[1] <= 1.1)
                {
                    enemyWeaponController.DelayBetweenShots += outputData[1];
                    delayBetweenShootsValue = enemyWeaponController.DelayBetweenShots;
                }
            }

            displayText.text = $"Obra¿enia: {damageValue}, " +
                $"Prêdkoœæ strza³ów: {delayBetweenShootsValue}, " +
                $"Maksymalne ¿ycie {maxHealthValue}";
        }

        void OnDestroy()
        {
            if (adjustmentMode)
            {
                worker.Dispose();
            }
        }
    }
}
