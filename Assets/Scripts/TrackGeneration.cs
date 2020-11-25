using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;

// Class to hold values to pass through functions and events
public class ClickArgs
{
    public string trackDir = "";
    public GameObject blue = null;
    public GameObject yellow = null;
    public List<GameObject> coneTrack = new List<GameObject>();
    public string choice;
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
        trackDirs.Add("track1", "Assets/CSV/test1.csv");
        trackDirs.Add("yellowdiagonal", "Assets/CSV/yellowdiag.csv");
        dropOption = fillTrackDropdown(dropOption, trackDirs);

        clickProps.choice = dropOption.captionText.text;
        dropOption.onValueChanged.AddListener(delegate { updateChoice(clickProps, dropOption); } );
        // Change track dir to selection on button click
        trackButton.onClick.AddListener(() => { changeTrack(trackDirs, clickProps); } );

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void updateChoice(ClickArgs clickProps, Dropdown dropOption)
    {
        clickProps.choice = dropOption.captionText.text;
    }

    public static Dropdown fillTrackDropdown(Dropdown dropOption, Dictionary<string, string> trackDirs) 
    {
        dropOption.ClearOptions();
        List<string> dropOptionsList = new List<String>();
        // Fill the dropdown with track names
        foreach (var track in trackDirs)
        {
            dropOptionsList.Add(track.Key);
        }
        // Fill dropdown obj with tracks
        dropOption.AddOptions(dropOptionsList);
        return dropOption;
    }

    public static void changeTrack(Dictionary<string, string> trackDirs, ClickArgs clickProps)
    {
        string trackDir = "";
        foreach (var val in trackDirs)
        {
            if (val.Key == clickProps.choice)
            {
                trackDir = val.Value;
            }
        }
        clickProps.trackDir = trackDir;

        if (clickProps.trackDir != "")
        {
            Debug.Log(trackDir);
            // Read CSV. Each cone in list is {colour, x, y}
            var trackInfo = readCSV(clickProps.trackDir);
            loadTrack(trackInfo, clickProps.yellow, clickProps.blue, clickProps);
        }
    }

    public static List<List<string>> readCSV(string trackCSV)
    {
        // Read cone positions xy and colour from CSV
        List<List<string>> trackInfo = new List<List<string>>();
        using (var reader = new StreamReader(trackCSV))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                trackInfo.Add(new List<string> { values[0], values[1], values[2] });
            }
        }

        return trackInfo;
    }

    public static void loadTrack(List<List<string>> trackInfo, GameObject yellow, GameObject blue, ClickArgs clickProps)
    {
        // Show default objects to allow for duplication
        blue.SetActive(true);
        yellow.SetActive(true);

        // Make copy of coneTrack
        var copyConeTrack = clickProps.coneTrack.ToList();

        // Delete old track
        foreach (var cone in copyConeTrack)
        {
            Destroy(cone);
        }
        // Store edited cone objects
        clickProps.coneTrack = copyConeTrack;

        // Generate track objects
        foreach (var cone in trackInfo)
        {
            // Create a new cone in the correct colour
            GameObject newCone = new GameObject();
            switch (cone[0])
            {
                case "y":
                    newCone = GameObject.Instantiate(yellow);
                    break;
                case "b":
                    newCone = GameObject.Instantiate(blue);
                    break;
            }

            // Position cone to x and y
            newCone.transform.position = new Vector3(Int32.Parse(cone[1]), 0, Int32.Parse(cone[2]));
            // Append to storage
            clickProps.coneTrack.Add(newCone);
        }
        // Hide default objects
        blue.SetActive(false);
        yellow.SetActive(false);

    }

}
