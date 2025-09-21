using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    [SerializeField] private int m_agentID;
    [SerializeField] private string m_agentName;
    [SerializeField] private float m_money;

    [SerializeField] private Ingredient m_agentMain;
    [SerializeField] private NavMeshAgent m_navAgent;
    [SerializeField] private Workstation m_workstation;
    [SerializeField] private KitchenManager m_kitchenManager;
    private bool m_isOrderBeingTaken = false;


    // ---- Methods ----

    public void MoveTo(Vector3 _direction){ /// Déplacer l’agent vers une position
        m_navAgent.SetDestination(_direction);
    }

    public void Work(Counter _counter){ /// Lancer la routine de travail
        StartCoroutine(WorkRoutine(_counter));
    }

    private IEnumerator WorkRoutine(Counter _counter)
    {
        while (true)
        {

            // 1️ Si aucune commande, en prendre une
            if (!m_kitchenManager.HasCurrentOrder() && !m_kitchenManager.IsOrderBeingTaken())
            {
                m_kitchenManager.SetOrderBeingTaken(true);
                yield return StartCoroutine(TakeOrderRoutine(_counter));
                m_kitchenManager.SetOrderBeingTaken(false);
            }


            // 2️ Récupérer les ingrédients un par un
            if (m_kitchenManager.HasCurrentOrder())
            {
                Ingredient nextIngredient = m_kitchenManager.GetCurrentIngredient();
                while (nextIngredient != null)
                {
                    yield return StartCoroutine(FetchIngredientRoutine(nextIngredient));
                    nextIngredient = m_kitchenManager.GetCurrentIngredient();
                }

                // 3️ Déposer la commande si tous les ingrédients sont sur la workstation
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

            // 4️ Attendre la prochaine boucle
            yield return null;
        }
    }




    private IEnumerator TakeOrderRoutine(Counter _counter){ /// Prise de commande au comptoir

        // 1️ Se déplacer vers le comptoir
        m_navAgent.SetDestination(_counter.transform.position);

        // 2️ Attendre que l’agent soit proche
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        // 3️ Récupérer la commande
        m_kitchenManager.SetCurrentOrder(_counter.GiveOrder());
        if (m_kitchenManager.GetCurrentOrder() != null)
        {
            m_kitchenManager.GetCurrentOrder().SetStatus(OrderStatus.Preparing);
            Debug.Log(m_agentName + " took order " + m_kitchenManager.GetCurrentOrder().GetOrderId());
        }
    }

    private IEnumerator FetchIngredientRoutine(Ingredient _ingredient){ /// Récupération d’un ingrédient et dépose sur la workstation
        if (_ingredient == null)
            yield break;

        IngredientContainer container = _ingredient.GetContainer();
        if (container == null)
            yield break;

        // 1️ Se déplacer vers le bac
        MoveTo(container.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, container.transform.position) < 1.5f);

        // 2️ Récupérer l’ingrédient
        m_agentMain = container.ProvideIngredient();
        Debug.Log(m_agentName + " picked up: " + m_agentMain.GetName());

        // 3️ Se déplacer vers la workstation
        MoveTo(m_workstation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, m_workstation.transform.position) < 2f);

        // 4️ Déposer l’ingrédient
        m_workstation.AddIngredient(m_agentMain);
        Debug.Log(m_agentName + " placed: " + m_agentMain.GetName() + " on workstation");
        MoveTo(transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f))); // Légère variation de position pour éviter l’empilement


        m_agentMain = null;
    }

    private IEnumerator DeliverOrderRoutine(Counter _counter)
    {
        Order currentOrder = m_kitchenManager.GetCurrentOrder();
        if (currentOrder == null)
            yield break;

        // 1️ Déplacer l’agent vers le comptoir
        MoveTo(_counter.transform.position);

        // 2️ Attendre que l’agent arrive à proximité
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        // 3️ Livrer la commande
        _counter.ReceiveOrder(currentOrder, this, m_workstation);
        Debug.Log(m_agentName + " delivered order: " + currentOrder.GetOrderId());

        // 4️ Nettoyer la commande dans le KitchenManager
        m_workstation.ClearStation();
        m_kitchenManager.ClearCurrentOrder();
    }


    public void AddMoney(int _amount) => m_money += _amount;
    public bool IsOrderBeingTaken() => m_isOrderBeingTaken;
    public void SetOrderBeingTaken(bool value) => m_isOrderBeingTaken = value;
}
