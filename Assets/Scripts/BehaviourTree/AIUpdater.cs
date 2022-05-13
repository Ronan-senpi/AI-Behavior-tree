using System.Collections;
using System.Collections.Generic;
using BehaviourTree.Nodes;
using UnityEngine;

/// <summary>
/// A class to manage every AI in the scene
/// </summary>
public class AIUpdater : MonoBehaviour
{
    #region Singleton Instance

    private static AIUpdater instance;

    public static AIUpdater Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AIUpdater>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AIUpdater");
                    instance = go.AddComponent<AIUpdater>();
                }
            }

            return instance;
        }
    }

    #endregion


    public const float AI_UPDATE_FREQUENCY = 0.05f;
    private bool updatingAI = true;
    private Dictionary<string, Node> AINodes = new Dictionary<string, Node>();
    private NodeState previousState;

    private void Start()
    {
        StartCoroutine(UpdateAI());
    }


    private IEnumerator UpdateAI()
    {
        var wait = new WaitForSeconds(AI_UPDATE_FREQUENCY);
        Node currentNode;
        while (updatingAI)
        {
            Debug.Log(AINodes.Count);
            foreach (var currentAI in AINodes)
            {
                currentNode = currentAI.Value;
                if (currentNode != null)
                {
                    NodeState currentState = currentNode.Evaluate();
                    if (currentState == NodeState.NotExecuted)
                    {
                        currentNode.OnStart();
                        currentNode.OnUpdate(AI_UPDATE_FREQUENCY);
                    }
                    else if (currentState == NodeState.Running)
                    {
                        currentNode.OnUpdate(AI_UPDATE_FREQUENCY);
                    }
                    else if (previousState != currentState &&
                             (currentState == NodeState.Success || currentState == NodeState.Failed))
                    {
                        currentNode.OnEnd();
                    }

                    previousState = currentState;
                }
            }

            yield return wait;
        }
    }


    public void SetCurrentNode(string id, Node newNode)
    {
        Debug.Log("Update AI " + id +" with node " + newNode.GetName());
        AINodes[id] = newNode;
    }
}