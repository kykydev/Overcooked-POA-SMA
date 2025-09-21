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
    [SerializeField] private Order m_currentOrder;
    [SerializeField] private Workstation m_workstation;

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

            // 1️ Attendre que la commande soit prise
            yield return StartCoroutine(TakeOrderRoutine(_counter));

            if (m_currentOrder == null)
                yield break;

            // 2️ Récupérer les ingrédients un par un
            foreach (Ingredient ingredient in m_currentOrder.GetRecipe())
            {
                yield return StartCoroutine(FetchIngredientRoutine(ingredient));
            }

            // 3️ Marquer la commande comme complète et livrer
            m_currentOrder.SetStatus(OrderStatus.Completed);
            yield return StartCoroutine(DeliverOrderRoutine(_counter));
        }
    }

    private IEnumerator TakeOrderRoutine(Counter _counter){ /// Prise de commande au comptoir
        // 1️ Se déplacer vers le comptoir
        m_navAgent.SetDestination(_counter.transform.position);

        // 2️ Attendre que l’agent soit proche
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        // 3️ Récupérer la commande
        m_currentOrder = _counter.GiveOrder();
        if (m_currentOrder != null)
        {
            m_currentOrder.SetStatus(OrderStatus.Preparing);
            Debug.Log("Agent " + m_agentName + " took order " + m_currentOrder.GetOrderId());
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
        yield return new WaitUntil(() => Vector3.Distance(transform.position, m_workstation.transform.position) < 1.5f);

        // 4️ Déposer l’ingrédient
        m_workstation.AddIngredient(m_agentMain);
        Debug.Log(m_agentName + " placed: " + m_agentMain.GetName() + " on workstation");

        m_agentMain = null;
    }

    private IEnumerator DeliverOrderRoutine(Counter _counter){ /// Livraison de la commande au comptoir
        if (m_currentOrder == null) yield break;

        // 1️ Déplacer l’agent vers le comptoir
        MoveTo(_counter.transform.position);

        // 2️ Attendre que l’agent arrive à proximité
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        // 3️ Livrer la commande
        _counter.ReceiveOrder(m_currentOrder, this, m_workstation);
        Debug.Log(m_agentName + " delivered order: " + m_currentOrder.GetOrderId());

        m_currentOrder = null;
    }

    public void AddMoney(int _amount) => m_money += _amount;
}
