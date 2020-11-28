using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Text;

// Class to hold values to pass through functions and events
public class ClickArgs
{
    public string trackDir = "";
    public GameObject blue = null;
    public GameObject yellow = null;
    public List<GameObject> coneTrack = new List<GameObject>();
    public string choice;
    public List<string> blueConeFiles = new List<string>();
    public List<string> yellowConeFiles = new List<string>();
    public List<List<string>> blueConesCoords = new List<List<string>>();
    public List<List<string>> yellowConesCoords = new List<List<string>>();
    public List<GameObject> yellowConeObjs = new List<GameObject>();
    public List<GameObject> blueConeObjs = new List<GameObject>();
}

public class TrackGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Load in cone objects to duplicate
        GameObject blue = GameObject.Find("blue");
        GameObject yellow = GameObject.Find("yellow");

        // UI objects
        Dropdown dropOption = GameObject.Find("trackDropdown").GetComponent<Dropdown>();
        Button trackButton = GameObject.Find("trackButton").GetComponent<Button>();
        // Object to hold return values from eventhandlers
        ClickArgs clickProps = new ClickArgs();

        clickProps.blue = blue;
        clickProps.yellow = yellow;

        // Tracks
        Dictionary<string, string> trackDirs = new Dictionary<string, string>();

        // Get track names
        readCSVFolder(clickProps);
        // Fill dropdown with track names
        dropOption = fillTrackDropdown(dropOption, clickProps);

        // Default dropdown track
        clickProps.choice = dropOption.captionText.text;
        updateChoice(clickProps, dropOption);

        // Allow for selection of track using dropdown
        dropOption.onValueChanged.AddListener(delegate { updateChoice(clickProps, dropOption); });
        // Change track dir to selection on button click
        trackButton.onClick.AddListener(() => { loadTrack(yellow, blue, clickProps); });

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void readCSVFolder(ClickArgs clickProps)
    {
        // Get file names of cones
        List<string> fileNames = new List<string>();
        string[] files = Directory.GetFiles("Assets/CSV/", "*.csv");
        foreach (string file in files)
        {
            fileNames.Add(Path.GetFileName(file));
        }
        // Split yellow cone file names into new structure
        foreach (string name in fileNames)
        {
            if (name.EndsWith("tight.json.csv"))
            {
                // Yellow file
                clickProps.yellowConeFiles.Add(name);
            }
            else if (name.EndsWith(".json.csv"))
            {
                // Blue file
                clickProps.blueConeFiles.Add(name);
            }
        }

    }

    public static Dropdown fillTrackDropdown(Dropdown dropOption, ClickArgs clickProps)
    {
        dropOption.ClearOptions();
        List<string> dropOptionsList = new List<String>();
        // Fill the dropdown with track names
        foreach (var track in clickProps.blueConeFiles)
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
        readCSVfiles(clickProps, clickProps.choice);
    }

    public static void readCSVfiles(ClickArgs clickProps, string choice)
    {
        // Get cone file directories
        string blueConeFile = $"Assets/CSV/{choice}";
        string yellowConeFile = $"Assets/CSV/{choice.Replace(".json.csv", "_tight.json.csv")}";

        // Use directories to get cone coordinates
        getConeCoords(clickProps, blueConeFile, clickProps.blueConesCoords);
        getConeCoords(clickProps, yellowConeFile, clickProps.yellowConesCoords);
    }


    public static void getConeCoords(ClickArgs clickProps, string file, List<List<string>> coordList)
    {
        using (StreamReader reader = new StreamReader(file))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                // XY
                coordList.Add(new List<string> { values[0], values[1] });
            }
        }

    }

    public static void createConeObjects(List<GameObject> copyTrack,
        GameObject coneColour, ClickArgs clickProps, char col,
        List<List<string>> coneCoords)
    {
        // Delete old track
        foreach (var cone in copyTrack)
        {
            Destroy(cone);
        }
        // Store edited cone objects; the reference to clickProps is necessary since C# can't pass obj
        // properties as references
        switch (col)
        {
            case 'y':
                clickProps.yellowConeObjs = copyTrack;
                break;
            case 'b':
                clickProps.blueConeObjs = copyTrack;
                break;
        }

        // Generate track objects
        foreach (var cone in coneCoords)
        {
            // Create a new cone in the correct colour
            GameObject newCone = new GameObject();
            newCone = GameObject.Instantiate(coneColour);

            // Position cone to x and y
            newCone.transform.position = new Vector3(float.Parse(cone[0]), 0, float.Parse(cone[1]));
            // Append to storage
            switch (col)
            {
                case 'y':
                    clickProps.yellowConeObjs.Add(newCone);
                    break;
                case 'b':
                    clickProps.blueConeObjs.Add(newCone);
                    break;
            }
        }

    }

    public static void loadTrack(GameObject yellow, GameObject blue, ClickArgs clickProps)
    {
        // Show default objects to allow for duplication
        blue.SetActive(true);
        yellow.SetActive(true);

        // Make copies of cone tracks to allow for enumeration
        var copyYellowTrack = clickProps.yellowConeObjs.ToList();
        var copyBlueTrack = clickProps.blueConeObjs.ToList();

        // Create cone objects
        createConeObjects(copyYellowTrack, yellow, clickProps, 'y',
            clickProps.yellowConesCoords);
        createConeObjects(copyBlueTrack, blue, clickProps, 'b',
            clickProps.blueConesCoords);

        // Hide default objects
        blue.SetActive(false);
        yellow.SetActive(false);

    }

}