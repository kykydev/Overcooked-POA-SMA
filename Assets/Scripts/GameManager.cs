using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Requis pour List<>

public class SimpleGameManager : MonoBehaviour
{
    [Header("Configuration de la Partie")]
    [Tooltip("Nombre d'agents à faire apparaître pour cette partie.")]
    [Range(1, 10)]
    public int numberOfAgents = 3;

    [Tooltip("Durée de la partie en secondes.")]
    public float gameDurationSeconds = 180f; // 3 minutes par défaut

    [Header("Références (À glisser dans l'inspecteur)")]
    [Tooltip("Le Prefab de votre agent.")]
    public GameObject agentPrefab;

    [Tooltip("La référence à votre KitchenManager DANS la scène.")]
    public KitchenManager kitchenManager;

    void Awake()
    {
        // --- 1. Remettre le jeu à vitesse normale ---
        Time.timeScale = 1f;

        // --- 2. Vérification des erreurs ---
        if (kitchenManager == null)
        {
            Debug.LogError("ERREUR : Vous devez glisser le KitchenManager de la scène dans le script SimpleGameManager !");
            this.enabled = false; // Désactive ce script pour éviter plus d'erreurs
            return;
        }
        if (agentPrefab == null)
        {
            Debug.LogError("ERREUR : Vous devez glisser le Prefab de l'Agent dans le script SimpleGameManager !");
            this.enabled = false;
            return;
        }

        // --- 3. Configuration (se produit AVANT le Start() du KitchenManager) ---

        // On configure le temps de jeu
        kitchenManager.m_gameTimer = gameDurationSeconds;

        // On crée une nouvelle liste vide pour les agents
        kitchenManager.m_agents = new List<Agent>();

        // --- 4. Création (Instanciation) des Agents ---
        for (int i = 0; i < numberOfAgents; i++)
        {
            // Vous pouvez changer Vector3.zero par un point de spawn si vous en avez un
            GameObject agentObj = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity);
            agentObj.name = $"Agent_{i + 1}";

            // Ajoute l'agent fraîchement créé à la liste du KitchenManager
            kitchenManager.m_agents.Add(agentObj.GetComponent<Agent>());
        }

        Debug.Log($"--- Partie de débogage lancée avec {numberOfAgents} agent(s) pour {gameDurationSeconds}s ---");
    }

    // ----- MODIFICATION ICI -----

    void Start()
    {
        // On ne lance PAS les agents tout de suite.
        // On lance une coroutine qui va attendre que KitchenManager.Start() soit terminé.
        StartCoroutine(DelayedAgentStart());
    }

    /// <summary>
    /// Attend une frame pour s'assurer que KitchenManager.Start()
    /// a bien initialisé les assiettes, PUIS démarre les agents.
    /// </summary>
    private IEnumerator DelayedAgentStart()
    {
        // Attend la prochaine frame. 
        // Cela garantit que TOUTES les méthodes Start() (y compris KitchenManager.Start())
        // se sont exécutées.
        yield return null;

        // Maintenant, nous sommes sûrs que les assiettes existent.
        if (kitchenManager != null && kitchenManager.enabled)
        {
            // On lance la routine de démarrage des agents
            StartCoroutine(kitchenManager.StartAgentsNextFrame());
        }
    }
}