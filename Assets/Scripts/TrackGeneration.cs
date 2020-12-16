using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;

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
}

public class TrackGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Load in cone objects to duplicate
        GameObject blue = GameObject.Find("blueCone");
        GameObject yellow = GameObject.Find("yellowCone");
        GameObject orange = GameObject.Find("orangeCone");
        GameObject big = GameObject.Find("bigorangeCone");
        GameObject adsdv = GameObject.Find("ads-dv");

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
        trackButton.onClick.AddListener(() => { loadTrack(yellow, blue, big, orange, adsdv, clickProps); });

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void readJSONFolder(ClickArgs clickProps)
    {
        // Get file names of cones
        List<string> fileNames = new List<string>();
        string[] files = Directory.GetFiles("Assets/json/", "*.json");
        foreach (string file in files)
        {
            fileNames.Add(Path.GetFileName(file));
        }
        foreach (string name in fileNames)
        {
            clickProps.coneFiles.Add(name);
        }

    }

    public static Dropdown fillTrackDropdown(Dropdown dropOption, ClickArgs clickProps)
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

    public static void updateChoice(ClickArgs clickProps, Dropdown dropOption)
    {
        clickProps.choice = dropOption.captionText.text;
        readJSONfiles(clickProps, clickProps.choice);
    }

    public static void readJSONfiles(ClickArgs clickProps, string choice)
    {
        // Get cone file directories
        string coneFile = $"Assets/json/{choice}";

        // Use directories to get cone coordinates
        getConeCoords(clickProps, coneFile);

    }

    public static void getConeCoords(ClickArgs clickProps, string file)
    {
        using (StreamReader fileRead = File.OpenText(file))
        {
            JsonSerializer serializer = new JsonSerializer();
            clickProps.track = (Track)serializer.Deserialize(fileRead, typeof(Track));
        }

    }

    public static void createConeObjects(
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

    public static void loadTrack(GameObject yellow, GameObject blue, GameObject big, GameObject orange, 
        GameObject adsdv, ClickArgs clickProps)
    {
        // Show default objects to allow for duplication
        blue.SetActive(true);
        yellow.SetActive(true);
        big.SetActive(true);
        orange.SetActive(true);
        adsdv.SetActive(true);

        float adsRaise = 0.25f;

        // Position ads-dv
        adsdv.transform.position = new Vector3(clickProps.track.car.pos[0], adsRaise, clickProps.track.car.pos[1]);
        // adsdv.transform.Rotate(0, clickProps.track.car.heading, 0, Space.Self);

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

        // Hide default objects
        blue.SetActive(false);
        yellow.SetActive(false);
        orange.SetActive(false);
        big.SetActive(false);


    }

}