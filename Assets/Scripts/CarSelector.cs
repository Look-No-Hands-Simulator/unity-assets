using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    [SerializeField]private Button nextCar;
    [SerializeField]private Button previousCar;    
    List<GameObject> cars = new List<GameObject>();

    private int carChoice;
    public int CarChoice
    {
        get
        {
            return carChoice;
        }
        set
        {
            carChoice = value;
        }
    }
    private int target = 0;
    private int numObjects;
    private int radius = 15;
    private Vector3 scaleChange = new Vector3(1.0f, 1.0f, 1.0f);
    private void Awake()
    {
        //set default car choice
        carChoice = 0;
        setCars();
        //cars[0].transform.localScale += scaleChange;
    }
    private void Update()
    {
        //constantly rotate selected car
        //cars[target].transform.RotateAround(cars[target].transform.position, Vector3.up, 40 * Time.deltaTime);
    }

    private void rotateCarsLeft()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].transform.RotateAround(transform.position, Vector3.up, 360 / numObjects);
        }
    }
    private void rotateCarsRight()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].transform.RotateAround(transform.position, Vector3.up, -360 / numObjects);
        }
    }
    private void setCars()
    {
        numObjects = transform.childCount;
        //sets cars in a circle based on amount of cars present
        for (int i = 0; i < transform.childCount; i++)
        {
            cars.Add(transform.GetChild(i).gameObject);            
            float theta = i * 2f * Mathf.PI / numObjects;
            float x = Mathf.Sin(theta +4.72f) * radius;
            float z = Mathf.Cos(theta +4.72f) * radius;            
            cars[i].transform.position = new Vector3(x, 1, z);
        }
        
    }
    public void changeCar(int select)
    {        

        //cars[carChoice].transform.localScale -= scaleChange;
        carChoice += select;
        //select car based on button input : right button +1, left button -1
        //allow for constant car selection in either left or right direction with modulus operator
        if (select > 0)
        {            
            carChoice = carChoice % numObjects;            
            rotateCarsRight();
        }
        else if(select < 0)
        {            
            carChoice = (carChoice + numObjects) % numObjects;            
            rotateCarsLeft();
        }
        //increase size of selected car object for further differentiation between currently selected objects
        //cars[carChoice].transform.localScale += scaleChange;
        target = carChoice;
        
    }
}
