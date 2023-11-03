using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererConnector : MonoBehaviour
{
    [SerializeField]
    Transform rootTrackersTr;

    [SerializeField]
    Transform[] childTrackersTr;

    [SerializeField]
    LineRenderer[] lineRenderers;

    void Update()
    {
        int lineIndex = 0;
        int childIndex = 0;

        foreach (Transform tr in childTrackersTr)
        {
            lineRenderers[lineIndex].SetPosition(0, rootTrackersTr.position);
            lineRenderers[lineIndex].SetPosition(1, childTrackersTr[childIndex].position);

            lineIndex++;
            childIndex++;
        }
    }
}
