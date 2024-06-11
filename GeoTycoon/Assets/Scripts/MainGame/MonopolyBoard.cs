using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonopolyBoard : MonoBehaviour
{
    public List<MonopolyNode> route = new List<MonopolyNode>();

    private void OnValidate()
    {
        route.Clear();
        foreach(Transform node in transform.GetComponentInChildren<Transform>())
        {
            route.Add(node.GetComponent<MonopolyNode>());
        }
    }
    private void OnDrawGizmos()
    {
        if (route.Count > 1) 
        {
         for(int i = 0; i < route.Count; i++)
            {
                Vector3 current = route[i].transform.position;
                Vector3 next = (i + 1 < route.Count) ? route[i+1].transform.position:current;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(current, next);

            }
        }
    }

    public void MovePlayertonken(int steps, Player_Mono player)
    {
        StartCoroutine(MovePlayerInSteps(steps, player));
    }

    IEnumerator MovePlayerInSteps(int steps, Player_Mono player)
    {
        int stepsLeft = steps;
        GameObject tonkenTomove = player.MyTonken;
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode);
        bool moveOverGo = false;
        while (stepsLeft > 0)
        {
            indexOnBoard++;
            // is this over go?
            if (indexOnBoard > route.Count - 1)
            {
                indexOnBoard = 0;
                moveOverGo = true;
            }
            //Get start and end positions
            Vector3 startPos = tonkenTomove.transform.position;
            Vector3 endPos = route[indexOnBoard].transform.position;
            //perform the move
            while (MoveToNextNode(tonkenTomove, endPos, 20))
            {
                yield return null;
            }
            stepsLeft--;
        }
        //Get go Money
        if(moveOverGo)
        {
            //Collect money on the player
        }
        //set new node on the current  player

        player.SetMyCurrentNode(route[indexOnBoard]);
    }

    bool MoveToNextNode(GameObject tonkenTomove, Vector3 endPos, float speed)
    {
        return endPos != (tonkenTomove.transform.position = Vector3.MoveTowards(tonkenTomove.transform.position,endPos,speed * Time.deltaTime));
    }
}
