using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterestPointHUD : MonoBehaviour
{
    //Holder Objects
    Canvas hudCanvas;

    [SerializeField]
    GameObject hudGameObject;

    [SerializeField]
    GameObject nameObject;

    Text districtName;

    public float widthBuffer;
    public float textScaleMultiplier;
    public float minAngle;
    public float unitsToPixels;

    //InterestPoint Components
    public PolygonCollider2D pointCollider;

    InterestPoint interestPoint;

    public GameObject interestPointBkg;
    MeshRenderer bkgMeshRenderer;

    public GameObject interestPointHighlighter;


    //Icons
    public Image infoImage;
    
    public Image[] iconSlots;


    [Header("Interest Point Icons")]

    public Sprite highPopSprite;
    public Sprite lowPopSprite;

    public Sprite enfranchisedSprite;
    public Sprite disenfranchisedSprite;

    public Sprite upperClassSprite;
    public Sprite workingClassSprite;

    public Sprite ethnicMajSprite;
    public Sprite ethnicMinSprite;


    public Sprite noInfoSprite;
    public Sprite lowInfoSprite;
    public Sprite medInfoSprite;
    public Sprite highInfoSprite;



    public void Initialize(InterestPoint parent)
    {
        pointCollider = parent.gameObject.GetComponent<PolygonCollider2D>();
        interestPoint = parent;

        //Get all the different HUD components
        hudCanvas = GetComponentInChildren<Canvas>();
        if(hudCanvas != null)
        {

            hudGameObject = hudCanvas.gameObject;
            
            foreach(Transform t in hudCanvas.transform)
            {
                if(t.name == "InterestPointName")
                {
                    nameObject = t.gameObject;
                    districtName = nameObject.GetComponent<Text>();
                }


            }
            
        }

        //Rotate the district name to fit inside
        if(pointCollider != null && nameObject != null)
        {

            RectTransform hudRect = hudGameObject.GetComponent<RectTransform>();

            float height = 300f;
            //float width = 1000f;

            float width = (pointCollider.bounds.size.x/1.3f) * unitsToPixels;

            hudRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            hudRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            hudRect.ForceUpdateRectTransforms();

            //Check whether all the corners for a given HUD are within the interestPoint's Polygon collider

            bool triedRoot = false;
            bool triedColCenter = false;

            if (hudRect != null)
            {
                //TODO: PUT THIS IN A LOOP
                bool inCollider = false;
                int tries = 0;


                while (!inCollider)
                {
                    Vector3[] hudCorners = new Vector3[4];
                    List<Vector2> nearestPoints = new List<Vector2>();
                    hudRect.GetWorldCorners(hudCorners);

                    foreach (Vector3 x in hudCorners)
                    {
                        Vector2 corner2D = new Vector2(x.x, x.y);
                        if (!pointCollider.OverlapPoint(corner2D))
                        {
                            Vector2 closestPointRaw = pointCollider.bounds.ClosestPoint(corner2D);
                            Vector2 closestPoint = corner2D - (Vector2)interestPoint.transform.InverseTransformPoint(closestPointRaw) ;
                            nearestPoints.Add(closestPoint);
                        }
                    }

                    if(nearestPoints.Count == 4)
                    {
                        if(triedRoot == false)
                        {
                            transform.position = interestPoint.transform.position;
                            triedRoot = true;
                        } else if (triedColCenter == false)
                        {
                            transform.position = pointCollider.bounds.center;
                            triedColCenter = true;
                        } else
                        {
                            transform.position = interestPoint.voronoiPoint;
                        }

                    }
                    else if (nearestPoints.Count >= 1)
                    {
                        Vector2 correction = new Vector2(nearestPoints.Average(n => n.x), nearestPoints.Average(n => n.y));
                        Vector3 correctionLocal = new Vector3(correction.x, correction.y, transform.position.z);
                        Debug.Log(interestPoint.name + " requires a HUD correction of " + correctionLocal);
                        transform.localPosition += correctionLocal;
                    }
                    else
                    {
                        inCollider = true;
                    }

                    if (tries >= 10)
                    {
                        Debug.Log(interestPoint.name + " gave up...");
                        transform.position = pointCollider.bounds.center;
                        inCollider = true;
                    }
                    tries++;
                }
            }

        }
    }

    
    public void PopulateInfoIcons()
    {
        foreach(Image i in iconSlots)
        {
            i.gameObject.SetActive(false);
        }

        int iconIndex = 0;

        if(interestPoint.populationLevel >= 3)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = highPopSprite;
            iconIndex++;
        } else
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = lowPopSprite;
            iconIndex++;
        }

        if(interestPoint.enfranchisementLevel >= 3)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = enfranchisedSprite;
            iconIndex++;
        } else
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = disenfranchisedSprite;
            iconIndex++;
        }

        if (interestPoint.hasUpperClassPop)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = upperClassSprite;
            iconIndex++;
        }

        if (interestPoint.hasWorkingClassPop)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = workingClassSprite;
            iconIndex++;
        }

        if (interestPoint.hasMajorityPop)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = ethnicMajSprite;
            iconIndex++;
        }

        if (interestPoint.hasMinorityPop)
        {
            iconSlots[iconIndex].gameObject.SetActive(true);
            iconSlots[iconIndex].sprite = ethnicMinSprite;
            iconIndex++;
        }
    }

    public void UpdatePlayerInfoLevels()
    {
        if(StrategyLayerManager.instance.currentFaction != null)
        {
            Faction f = StrategyLayerManager.instance.currentFaction;
            int infoLevel = f.pointInfoDictionary[interestPoint.interestPointName];

            if (infoLevel >= 70)
            {
                infoImage.sprite = highInfoSprite;
            }
            else if (infoLevel >= 50)
            {
                infoImage.sprite = medInfoSprite;
            }
            else if (infoLevel >= 20)
            {
                infoImage.sprite = lowInfoSprite;
            }
            else
            {
                infoImage.sprite = noInfoSprite;
            }
        } else
        {
            infoImage.sprite = noInfoSprite;
        }
    }
    
}
