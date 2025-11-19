using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ExperimentManager : MonoBehaviour
{
    [Header("Configuration de l'Expérience")]
    public int maxAgents = 8;
    public int runsPerConfig = 1000;
    public float gameDuration = 180f;
    public float simulationSpeed = 50f;

    [Header("Contrôle")]
    [Tooltip("COCHEZ CECI pour redémarrer l'expérience à zéro (efface la sauvegarde).")]
    public bool resetExperiment = false; // <--- NOUVEAU CHAMP

    [Header("Prérequis")]
    public GameObject agentPrefab;

    private KitchenManager m_kitchenManager;
    private int m_currentAgentCount;
    private int m_currentGameRun;
    private string m_csvPath;

    private const string KEY_AGENT_COUNT = "Experiment_AgentCount";
    private const string KEY_GAME_RUN = "Experiment_GameRun";

    void Awake()
    {
        Time.timeScale = simulationSpeed;
        m_kitchenManager = FindObjectOfType<KitchenManager>();

        if (m_kitchenManager == null)
        {
            Debug.LogError("ExperimentManager n'a pas trouvé de KitchenManager !");
            return;
        }

        m_csvPath = Path.Combine(Application.persistentDataPath, "experiment_results.csv");

        // --- GESTION DU RESET ---
        if (resetExperiment)
        {
            Debug.LogWarning("RESET ACTIVÉ : Remise à zéro des compteurs et suppression de l'ancien CSV.");
            PlayerPrefs.DeleteKey(KEY_AGENT_COUNT);
            PlayerPrefs.DeleteKey(KEY_GAME_RUN);

            // Optionnel : Supprimer l'ancien fichier CSV pour repartir sur du propre
            if (File.Exists(m_csvPath))
            {
                File.Delete(m_csvPath);
            }

            // Important : on remet resetExperiment à false pour ne pas reset au prochain rechargement de scène !
            // Note : cela ne se décoche pas visuellement dans l'éditeur une fois le jeu lancé, 
            // mais la logique le prendra en compte.
            resetExperiment = false;
        }

        // --- RÉCUPÉRATION OU DÉFAUT ---
        m_currentAgentCount = PlayerPrefs.GetInt(KEY_AGENT_COUNT, 1);
        m_currentGameRun = PlayerPrefs.GetInt(KEY_GAME_RUN, 0);

        // --- CRÉATION FICHIER / EN-TÊTE ---
        // Correctif : On écrit l'en-tête si le fichier n'existe pas, PEU IMPORTE le run actuel.
        // Cela permet de recréer un fichier propre même si on reprend au run 27.
        if (!File.Exists(m_csvPath))
        {
            File.WriteAllText(m_csvPath, "num_agents,run_id,score,seed\n");
        }

        // --- FIN ---
        if (m_currentAgentCount > maxAgents)
        {
            Debug.Log($"--- EXPÉRIMENTATION TERMINÉE ---");
            Debug.Log($"CSV : {m_csvPath}");
            Time.timeScale = 1f;
            PlayerPrefs.DeleteKey(KEY_AGENT_COUNT);
            PlayerPrefs.DeleteKey(KEY_GAME_RUN);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            return;
        }

        m_kitchenManager.enabled = false;
        StartCoroutine(ConfigureAndStartRun());
    }

    IEnumerator ConfigureAndStartRun()
    {
        int seed = (m_currentAgentCount * 10000) + m_currentGameRun;
        Random.InitState(seed);

        Debug.Log($"Run {m_currentGameRun}/{runsPerConfig} - Agents: {m_currentAgentCount}");

        m_kitchenManager.m_gameTimer = gameDuration;
        m_kitchenManager.m_agents = new List<Agent>();

        for (int i = 0; i < m_currentAgentCount; i++)
        {
            GameObject agentObj = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity);
            agentObj.name = $"Agent_{i + 1}";
            m_kitchenManager.m_agents.Add(agentObj.GetComponent<Agent>());
        }

        m_kitchenManager.enabled = true;
        yield return null;
        StartCoroutine(m_kitchenManager.StartAgentsNextFrame());
    }

    void Update()
    {
        if (m_kitchenManager == null || !m_kitchenManager.enabled) return;

        if (m_kitchenManager.GetGameTimer() <= 0)
        {
            EndRunAndReload();
        }
    }

    void EndRunAndReload()
    {
        int score = m_kitchenManager.GetTotalMoney();
        int seed = (m_currentAgentCount * 10000) + m_currentGameRun;

        // Utilisation de AppendAllText qui crée le fichier s'il n'existe pas
        File.AppendAllText(m_csvPath, $"{m_currentAgentCount},{m_currentGameRun},{score},{seed}\n");

        m_currentGameRun++;

        if (m_currentGameRun >= runsPerConfig)
        {
            m_currentGameRun = 0;
            m_currentAgentCount++;
        }

        PlayerPrefs.SetInt(KEY_AGENT_COUNT, m_currentAgentCount);
        PlayerPrefs.SetInt(KEY_GAME_RUN, m_currentGameRun);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}