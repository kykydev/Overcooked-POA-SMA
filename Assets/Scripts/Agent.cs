using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using UnityEngineInternal;

public class Agent : MonoBehaviour
{
    /// --- Attributes ---
    private int m_agentID;
    private string m_agentName;
    private Ingredient m_agentMain;

    private Workstation m_workstation;

    [SerializeField] private NavMeshAgent m_navAgent;
    [SerializeField] private KitchenManager m_kitchenManager;

    /// ---- Getters ----
    public int GetAgentID() => m_agentID;
    public string GetAgentName() => m_agentName;
    public Ingredient GetAgentMain() => m_agentMain;

    /// ---- Setters ----
    public void SetAgentID(int _id) => m_agentID = _id;
    public void SetAgentName(string _name) => m_agentName = _name;
    public void SetAgentMain(Ingredient _main) => m_agentMain = _main;
    public void SetWorkstation(Workstation _workstation) => m_workstation = _workstation;


    /// ---- Methods ----

    // Déplacer l’agent vers une position
    public void MoveTo(Vector3 _direction)
    {
        m_navAgent.SetDestination(_direction);
    }

    // Lancer la routine de travail
    public void Work(Counter _counter)
    {
        StartCoroutine(WorkRoutine(_counter));
    }

    private IEnumerator WorkRoutine(Counter _counter)
    {
        while (true)
        {

            //Si aucune commande, en prendre une
            if (!m_kitchenManager.HasCurrentOrder() && !m_kitchenManager.IsOrderBeingTaken())
            {
                m_kitchenManager.SetOrderBeingTaken(true);
                yield return StartCoroutine(TakeOrderRoutine(_counter));
                m_kitchenManager.SetOrderBeingTaken(false);
            }


            //Récupérer les ingrédients un par un
            if (m_kitchenManager.HasCurrentOrder())
            {
                Ingredient nextIngredient = m_kitchenManager.GetCurrentIngredient();
                while (nextIngredient != null)
                {
                    yield return StartCoroutine(FetchIngredientRoutine(nextIngredient));
                    nextIngredient = m_kitchenManager.GetCurrentIngredient();
                }

                //Déposer la commande si tous les ingrédients sont sur la workstation
                if (m_workstation.ValidateOrder(m_kitchenManager.GetCurrentOrder()))
                {
                    if (!m_kitchenManager.IsOrderBeingDelivered())
                    {
                        m_kitchenManager.SetOrderBeingDelivered(true);
                        
                        yield return StartCoroutine(DeliverOrderRoutine(_counter));
                       
                        m_kitchenManager.SetOrderBeingTaken(true);
                        yield return StartCoroutine(TakeOrderRoutine(_counter));
                        m_kitchenManager.SetOrderBeingTaken(false);

                        m_kitchenManager.SetOrderBeingDelivered(false);
                    }
                }
            }

            //Attendre la prochaine boucle
            yield return null;
        }
    }



    // Prise de commande au comptoir
    private IEnumerator TakeOrderRoutine(Counter _counter)
    {

        //Se déplacer vers le comptoir
        m_navAgent.SetDestination(_counter.transform.position);

        //Attendre que l’agent soit proche
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 2f);

        //Récupérer la commande
        m_kitchenManager.SetCurrentOrder(_counter.GiveOrder());
        if (m_kitchenManager.GetCurrentOrder() != null)
        {
            m_kitchenManager.GetCurrentOrder().SetStatus(OrderStatus.Preparing);
            Debug.Log(m_agentID + " took order " + m_kitchenManager.GetCurrentOrder().GetOrderId());
        }
    }

    // Récupération d’un ingrédient et dépose sur la workstation
    private IEnumerator FetchIngredientRoutine(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        IngredientContainer container = _ingredient.GetContainer();
        if (container == null)
            yield break;

        //Se déplacer vers le bac
        MoveTo(container.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, container.transform.position) < 2f);

        //Récupérer l’ingrédient
        m_agentMain = container.GetIngredient();
        Debug.Log(m_agentID + " picked up: " + m_agentMain.GetName());

        //Se déplacer vers la workstation
        MoveTo(m_workstation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, m_workstation.transform.position) < 2f);

        //Déposer l’ingrédient
        m_workstation.AddIngredient(m_agentMain);
        Debug.Log(m_agentID + " placed: " + m_agentMain.GetName() + " on workstation");
        MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f))); // Légère variation de position pour éviter l’empilement


        m_agentMain = null;
    }

    private IEnumerator DeliverOrderRoutine(Counter _counter)
    {
        Order currentOrder = m_kitchenManager.GetCurrentOrder();
        if (currentOrder == null)
            yield break;

        //Déplacer l’agent vers le comptoir
        MoveTo(_counter.transform.position);

        //Attendre que l’agent arrive à proximité
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 2f);

        //Livrer la commande
        _counter.ReceiveOrder(currentOrder, m_workstation);
        Debug.Log(m_agentID + " delivered order: " + currentOrder.GetOrderId());

        //Nettoyer la commande dans le KitchenManager
        m_workstation.ClearStation();
        m_kitchenManager.ClearCurrentOrder();
    }
}
