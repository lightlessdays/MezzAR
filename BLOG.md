[Find the Unity Project at the end of this article. I assume that the reader has some basic knowledge of Unity GameEngine and AR Development in general].

Today we will create an app like Measure. Measure was an app by Google that used a smartphone to measure real-life objects through the magic of augmented reality. It tracked the real-life objects in order to accurately place virtual items in a camera feed, and if the tracking was good enough, that app can turn that data into a pretty good estimate of distance. Our app would be accurate to within half an inch for short measurements, but off by several inches for long measurements. Plus, the app would work great if you wanted to measure something large, like a telephone pole, which would be pretty difficult with a tape measure.

## Stage One: Implementing Plane Detection

So, start by creating a new Unity 3D Project and then install a few new packages: ARFoundation, ARCore XR Plugin and XR Interaction Toolkit. You can check the packages and their versions below:

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038095771/w3OPzcVaC.png)

Next, switch your platform to Android and then click on “Add Open Scenes” button.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038115006/kQDzda299.png align="left")

Next, delete the MainCamera from hierarchy. Then, right click on the empty space in the hierarchy and add XR > AR Session Origin to the hierarchy.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038127376/bsuB01ve9.png align="left")

Also add AR Session.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038143293/zS3-tlDxk.png align="left")

Next, add XR > AR Default Plane to the hierarchy.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038188518/U9EbSknwW.png align="left")

This will be the prefab for plane detection. You can create a new material, if you do not want the default debug plane. Else, continue with the default debug plane. Since I do not like the default material, I created a new one, like the one shown below:

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038198561/E7f4jYfgg.png align="left")

Assign this material to your plane’s Mesh Renderer.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038205752/PYnXaZ9Fa.png align="left")

Next, make a prefab out of AR Default Plane.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038212316/ZhcjMSh-6.png align="left")

In AR Session Origin, add an AR Plane Manager script and assign the ARDefaultPlane prefab to the Plane Prefab field.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038222207/JT7v-i_g7.png align="left")

Now, let us run and test the app to see if it is working. You need to make some changes in the Player Settings though-

1. Disable Auto Graphics API.
2. Remove Vulkan Graphics from the Graphics APIs list.
3. Turn off Multithreaded rendering.
4. Set minimum API level to Android 8.0 ‘Oreo’.
5. Change scripting backend to IL2CPP.
6. Enable ARM64 Architecture.
7. In XR Plugin Management, choose ARCore as your Plugin Provider.
And we are done… now hit the Build and Run button to test your app.

## Stage Two: Implementing AR Interaction

Now, we need to implement a feature that will allow us to spawn objects when we tap on the plane. So, add XR Placement Interactable in hierarchy.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038312217/6uYFbzTCi.png align="left")

Create a prefab out of a sphere.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038329558/ofR7MXrlG.png align="left")

Assign that sphere to Placement Prefab field in AR Placement Interactable.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038348145/EA0ON2Ha1.png align="left")

To AR Session Origin, add AR Raycast Manager.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038366898/PDtl5h08F.png align="left")

In the AR Camera, add AR Gesture Interactor Script.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038381308/2DuWmB0ld.png align="left")

Now build the app again and test it.

## Stage Three: Implementing Line Renderer

Now we are coming to the part I faced the most difficulty in: the Line Renderer. The first step would be to add a Line Renderer component in the hierarchy.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038432477/p6qHbNtbN.png align="left")

Once it is added, change the size to zero.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038448233/MyXVgci4D.png align="left")

Size is the number of points a line renderer has. Change the line width to 0.01.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038461933/n2d_brfCV.png align="left")

We will be managing our Line Renderer using a script. We will create a function called DrawLine, which will be called every time the user places a new sphere. First, we create a reference of the Line Renderer.

```
[SerializeField] private LineRenderer lineRenderer;
```
Next, we will add a method called DrawLine. It will take ARObjectPlacementEventArgs as argument. First, it will increase the points in our line renderer, then set the position of the latest point added to the position of the new sphere added.

```
public void DrawLine(ARObjectPlacementEventArgs args) {
//the drawline method is called whenever the user places a sphere on the plane. since the user has
        //placed a sphere, we will add a point to our line renderer.
        lineRenderer.positionCount++;
//we will set the position of our new point to the position of our placement object.
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, args.placementObject.transform.position);
}
```

Assign this script to an empty GameObject.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038569795/pDma6vC7X.png align="left")

Next, in AR Placement Interactable, we will find an interactable event called Object Placed. This gives the argument of type ARObjectPlacementEventArgs. Now we know why we added it in the function.

Object Placed method is called everytime a new object is placed onto the plane.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038588355/pc8HhXMpl.png align="left")

Click on the “+” sign and assign LineManager GameObject to the None field. From functions, select DrawLine.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038604980/B_BR_eZMs.png align="left")

Once this is done, build your app and test it.

## Stage Four: Implementing TextMeshPro

Here, we will calculate the distance between the two points in line renderer. To do this, we will first check if there are more than one point in Line Renderer. If yes, we will get the distance between the last and the second last point:

```
//adding textmeshpro above the lines
        if (lineRenderer.positionCount > 1) //there must be at least two points two show textmeshpro
        {
            Vector3 pointA = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            Vector3 pointB = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
            float dist = Vector3.Distance(pointA, pointB);
        }
```

Next, we will use TextMeshPro to show the distance to the user. We will create a serialized reference to TextMeshPro.

```
[SerializeField] private TextMeshPro m_Text;
```

Next, we can update the TextMeshPro in our if statement.

```
m_Text.text = "" + dist;
```

Now, here is a problem with our code: we have created only one TextMeshPro, but we need multiple TextMeshPro rendering on different lines. So, everytime a new line is created, a new TextMeshPro should be created. So, we can make changes to our code, so that our DrawLine method now looks like this:

```
public void DrawLine(ARObjectPlacementEventArgs args) {

        lineRenderer.positionCount++;

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, args.placementObject.transform.position);
        if (lineRenderer.positionCount > 1) /
        {
            Vector3 pointA = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            Vector3 pointB = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
            float dist = Vector3.Distance(pointA, pointB);
            TextMeshPro distText = Instantiate(m_Text);
m_Text.text = "" + dist;
        }
    }
```

Now, we need to align our TextMeshPro to our line parallelly with a slight offset in the up direction.

```
Vector3 directionVector = pointB - pointA;
            Vector3 normal = args.placementObject.transform.up;
            Vector3 upd = Vector3.Cross(directionVector, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, upd);
            distText.transform.rotation = rotation;
            distText.transform.position = (pointA + directionVector * 0.5f) + (upd * 0.2f);
```

Now, we can create a prefab out of TextMeshPro object.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038734667/7fWJFdWY2.png align="left")

Now, assign it to our LineManager.

![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1669038758130/XrkulK21U.png align="left")

Build and test your app now.

## Stage Five: Adding Buttons

Next, I decided to add two features in my app: Clear All Button and Discrete Continuous Toggle Button. I won’t be going deep into the functionality part, because it is mostly obvious. The code for clear all button was simple. All I had to do was implement a function that reloaded the scene.

```
public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
```

This would reload the scene, and essentially clear off all the spheres and line renderers. To make the discrete/continuous button, I had to make a few changes to my script. Here is the entire script:

```
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
```

Next, I also created a TouchManager GameObject that managed touches on the UI button, so that the touches on button do not spawn spheres. To the TouchManager GameObject, I attached a TouchManager script.

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    /// <summary>
    /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
    /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
    /// </summary>
    private bool IsPointerOverUIObject()
    {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    /// <summary>
    /// Cast a ray to test if screenPosition is over any UI object in canvas. This is a replacement
    /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
    /// </summary>
    private bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventDataCurrentPosition, results);
        return results. Count > 0;
    }
}
```

I added the buttons and some UI components… and my app was made… Github repository for this project: [https://github.com/lightlessdays/MezzAR](https://github.com/lightlessdays/MezzAR).


