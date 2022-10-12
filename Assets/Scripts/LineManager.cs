using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LineManager : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TextMeshPro m_Text;
    [SerializeField] private bool isContinous = true;
    [SerializeField] private TextMeshProUGUI m_DiscreteContinousText;
    private List<GameObject> spheres;
    private int pointCount = 0;
    LineRenderer line;
    [SerializeField] GameObject UIPanel;
    private bool isCredit = false;

    public void DrawLine(ARObjectPlacementEventArgs args)
    {


        pointCount++;
        if (pointCount < 2)
        {
            line = Instantiate(lineRenderer);
            line.positionCount = 1;
        }
        else
        {
            line.positionCount = pointCount;
            
            if (!isContinous)
            {
                pointCount = 0;
            }
        }


        line.SetPosition(line.positionCount - 1, args.placementObject.transform.position);

        //adding textmeshpro above the lines
        if (line.positionCount > 1) //there must be at least two points two show textmeshpro
        {
            Vector3 pointA = line.GetPosition(line.positionCount - 1);
            Vector3 pointB = line.GetPosition(line.positionCount - 2);
            float dist = Vector3.Distance(pointA, pointB);
            TextMeshPro distText = Instantiate(m_Text);
            distText.text = (dist * 100f).ToString("F2") + " cm";
            Vector3 directionVector = (pointB - pointA);
            Vector3 normal = args.placementObject.transform.up;
            Vector3 upd = Vector3.Cross(directionVector, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, upd);
            distText.transform.rotation = rotation;
            distText.transform.position = (pointA + directionVector * 0.5f) + upd * 0.008f;
        }


    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinousButton()
    {
        isContinous = !isContinous;
        if (isContinous)
            m_DiscreteContinousText.text = "Discrete";
        else
            m_DiscreteContinousText.text = "Continous";
    }

    public void CreditButton()
    {
        isCredit = !isCredit;
        if (isCredit)
        {
            UIPanel.SetActive(true);
        }
        else
        {
            UIPanel.SetActive(false);
        }
    }

}
