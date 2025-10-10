using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using UnityEngineInternal;


// cet agent prendre seulement l'ingrédient passer en paramètre unity
// si l'ingrédient n'est pas spécifié, il agit comme un agent classic
public class AgentIngredient : Agent {

    private string m_ingredient="Bread";

    private IEnumerator WorkRoutine(Counter _counter){
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

                // vérifier si l'agent à le bon ingrédient
                Debug.Log("mon ingredient "+m_ingredient);

                if(m_ingredient!=null){
                    while(nextIngredient.GetName()!=m_ingredient){
                        m_kitchenManager.EnqueueIngredient(nextIngredient);
                        nextIngredient = m_kitchenManager.GetCurrentIngredient();
                    }
                }


                while (nextIngredient != null)
                {
                    yield return StartCoroutine(FetchIngredientRoutine(nextIngredient));
                    nextIngredient = m_kitchenManager.GetCurrentIngredient();
                }

                // Si l'agent porte un plat, il livre la commande
                if (m_agentMain is Dish)
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
}