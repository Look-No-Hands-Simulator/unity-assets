using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;

// Data
public class Track
{
    public List<List<float>> blue { get; set; }
    public List<List<float>> yellow { get; set; }
    public List<List<float>> orange { get; set; }
    public List<List<float>> big { get; set; }
    public struct Car
    {
        public List<float> pos;
        public float heading;

    };
    public Car car = new Car();
 

}

// Class to hold values to pass through functions and events
// Actual game objects
public class ClickArgs
{
    public string trackDir = "";
    public string choice;
    public List<string> coneFiles = new List<string>();
    public List<GameObject> yellowConeObjs = new List<GameObject>();
    public List<GameObject> blueConeObjs = new List<GameObject>();
    public List<GameObject> bigConeObjs = new List<GameObject>();
    public List<GameObject> orangeConeObjs = new List<GameObject>();
    public Track track = new Track();
    public GameObject car;
}

public class TrackGeneration : MonoBehaviour
{
    
    // Get instance of camera switcher viewer with its cameras
    public GameObject cameraSwitcherGameObject;
    //private CameraSwitcherViewer cameraSwitcherHolder;
    
    // Start is called before the first frame update
    void Start()
    {


        // Load in cone objects to duplicate
        GameObject blue = GameObject.Find("blueCone");
        GameObject yellow = GameObject.Find("yellowCone");
        GameObject orange = GameObject.Find("orangeCone");
        GameObject big = GameObject.Find("bigorangeCone");


        // UI objects
        Dropdown dropOption = GameObject.Find("trackDropdown").GetComponent<Dropdown>();
        Button trackButton = GameObject.Find("trackButton").GetComponent<Button>();
        // Object to hold return values from eventhandlers
        ClickArgs clickProps = new ClickArgs();

        // Tracks
        Dictionary<string, string> trackDirs = new Dictionary<string, string>();

        // Get track names
        readJSONFolder(clickProps);
        // Fill dropdown with track names
        dropOption = fillTrackDropdown(dropOption, clickProps);

        // Default dropdown track
        clickProps.choice = dropOption.captionText.text;
        updateChoice(clickProps, dropOption);

        // Allow for selection of track using dropdown
        dropOption.onValueChanged.AddListener(delegate { updateChoice(clickProps, dropOption); });

        // Change track dir to selection on button click
        trackButton.onClick.AddListener(() => { loadTrack(yellow, blue, big, orange, clickProps); });

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void readJSONFolder(ClickArgs clickProps)
    {
        // Get file names of cones
        List<string> fileNames = new List<string>();
        string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/json/", "*.json");
        //Debug.Log(files);
        foreach (string file in files)
        {
            fileNames.Add(Path.GetFileName(file));
        }
        foreach (string name in fileNames)
        {
            clickProps.coneFiles.Add(name);
        }

    }

    public Dropdown fillTrackDropdown(Dropdown dropOption, ClickArgs clickProps)
    {
        dropOption.ClearOptions();
        List<string> dropOptionsList = new List<String>();
        // Fill the dropdown with track names
        foreach (var track in clickProps.coneFiles)
        {
            dropOptionsList.Add(track);
        }
        // Fill dropdown obj with tracks
        dropOption.AddOptions(dropOptionsList);
        return dropOption;
    }

    public void updateChoice(ClickArgs clickProps, Dropdown dropOption)
    {
        clickProps.choice = dropOption.captionText.text;
        readJSONfiles(clickProps, clickProps.choice);
    }

    public void readJSONfiles(ClickArgs clickProps, string choice)
    {
        // Get cone file directories
        string coneFile = Application.streamingAssetsPath + $"/json/{choice}";

        // Use directories to get cone coordinates
        getConeCoords(clickProps, coneFile);

    }

    public void getConeCoords(ClickArgs clickProps, string file)
    {
        using (StreamReader fileRead = File.OpenText(file))
        {
            JsonSerializer serializer = new JsonSerializer();
            clickProps.track = (Track)serializer.Deserialize(fileRead, typeof(Track));
        }

    }

    public void createConeObjects(
        GameObject coneColour, ClickArgs clickProps, string col,
        List<List<float>> coneCoords)
    {
        // Generate track objects
        foreach (var cone in coneCoords)
        {
            // Create a new cone in the correct colour
            GameObject newCone = new GameObject();
            newCone = GameObject.Instantiate(coneColour);

            // Position cone to x and y
            newCone.transform.Translate(cone[0], 0, cone[1], Space.World);

            // Append to storage
            switch (col)
            {
                case "y":
                    clickProps.yellowConeObjs.Add(newCone);
                    break;
                case "b":
                    clickProps.blueConeObjs.Add(newCone);
                    break;
                case "bo":
                    clickProps.bigConeObjs.Add(newCone);
                    break;
                case "o":
                    clickProps.orangeConeObjs.Add(newCone);
                    break;
            }
        }

    }
    public void GetCarObject(ClickArgs clickProps)
    {
        GameObject carObject = GameObject.Find("carSelect");
        CarSelector carSelect = carObject.GetComponent<CarSelector>();        
        int carChoice = carSelect.CarChoice;
        for (int i = 0; i < carObject.transform.childCount; i++)
        {
            //Destroy(carObject.transform.GetChild(i).gameObject);
            carObject.transform.GetChild(i).gameObject.SetActive(false);
            
        }
        GameObject car = carObject.transform.GetChild(carChoice).gameObject;
        clickProps.car = car;
        //car.transform.rotation = new Quaternion(car.transform.rotation.x, 0.0f, car.transform.rotation.z, car.transform.rotation.w);
        car.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        car.SetActive(true);
        //disable parent object to allow for correct x, y, z positioning
        car.transform.parent = null;
        car.transform.SetParent(GameObject.Find("player").transform);
        //newCone.transform.Translate(cone[0], 0, cone[1], Space.World);
        // Position car
        // float adsRaise = 0.29f;
        // TODO: The line below causes a bug (car flips). I am not sure how the car is transformed otherwise however, it must be from elsewhere as it works.
        car.transform.position = new Vector3(clickProps.track.car.pos[0], car.transform.position.y, clickProps.track.car.pos[1]);
        //car.transform.rotation = new Quaternion(car.transform.rotation.x, clickProps.track.car.heading, car.transform.rotation.z + 0);
        car.transform.Rotate(0.0f, clickProps.track.car.heading - car.transform.rotation.y, 0.0f);
        
        Debug.Log("JSON CAR X: " + clickProps.track.car.pos[0] + " Z: " + clickProps.track.car.pos[1]);
        Debug.Log("UNITY CAR X: " + car.transform.position.x + " Y:" + car.transform.position.y + " Z: " + car.transform.position.z);
        Debug.Log("JSON car heading: " + clickProps.track.car.heading);
        Debug.Log("UNITY actual car heading: " + car.transform.rotation.y);
        
        

        //float heading = car.transform.rotation.eulerAngles.y - 360;
        
        //Debug.Log("UNITY heading euler y: " + car.transform.rotation.eulerAngles.y);
        //Debug.Log("UNITY heading euler y - 360: " + heading);

        

        //disable carselect script
        carSelect.enabled = false;
    }

    public void loadTrack(GameObject yellow, GameObject blue, GameObject big, GameObject orange, ClickArgs clickProps)
    {
        
        // Show default objects to allow for duplication
        blue.SetActive(true);
        yellow.SetActive(true);
        big.SetActive(true);
        orange.SetActive(true);

        //set select car and position in track
        GetCarObject(clickProps);
        
        
        // Make copies of cone tracks to allow for enumeration
        var copyYellowTrack = clickProps.yellowConeObjs.ToList();
        var copyBlueTrack = clickProps.blueConeObjs.ToList();
        var copyOrangeTrack = clickProps.orangeConeObjs.ToList();
        var copyBigTrack = clickProps.bigConeObjs.ToList();

        // Delete old track
        foreach (var cone in copyYellowTrack)
        {
            Destroy(cone);
        }
        foreach (var cone in copyBlueTrack)
        {
            Destroy(cone);
        }
        foreach (var cone in copyBigTrack)
        {
            Destroy(cone);
        }
        foreach (var cone in copyOrangeTrack)
        {
            Destroy(cone);
        }
        clickProps.yellowConeObjs = copyYellowTrack;
        clickProps.blueConeObjs = copyBlueTrack;
        clickProps.bigConeObjs = copyBigTrack;
        clickProps.orangeConeObjs = copyOrangeTrack;

        // Create cone objects
        createConeObjects(yellow, clickProps, "y", clickProps.track.yellow);
        createConeObjects(blue, clickProps, "b", clickProps.track.blue);
        createConeObjects(big, clickProps, "bo", clickProps.track.big);
        createConeObjects(orange, clickProps, "o", clickProps.track.orange);

        configureCarObject(clickProps.car, clickProps.track.car);

        // Hide default objects
        blue.SetActive(false);
        yellow.SetActive(false);
        orange.SetActive(false);
        big.SetActive(false);

        cameraSwitcherGameObject.GetComponent<CameraSwitcherViewer>().SetHighCamActive();


    }

    public void configureCarObject(GameObject car, Track.Car carInfo) {
        car.transform.position = new Vector3(carInfo.pos[0],car.transform.position.y,carInfo.pos[1]);
        car.transform.eulerAngles = new Vector3(0,carInfo.heading,0);
        Debug.Log("The pos [0] is: " + carInfo.pos[0] + " and the pos [1] is" + carInfo.pos[1]);
        Debug.Log("The rotation is: " + carInfo.heading);

    }

}
