using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileInfoUI : MonoBehaviour
{
    [SerializeField]
    MotionData motionData;

    [SerializeField]
    TrackedObjectList trackedObjectList;

    [SerializeField]
    GameObject filePanel;

    [SerializeField]
    Dropdown sports;

    [SerializeField]
    InputField generation;

    [SerializeField]
    Dropdown gender;

    [SerializeField]
    InputField age;

    [SerializeField]
    InputField subjectName;

    [SerializeField]
    InputField date;

    [SerializeField]
    InputField actNumber;

    [SerializeField]
    GameObject trackerPanel;

    [SerializeField]
    Dropdown leftWrist;

    [SerializeField]
    Dropdown rightWrist;

    [SerializeField]
    Dropdown waist;

    [SerializeField]
    Dropdown leftFoot;

    [SerializeField]
    Dropdown rightFoot;

    [SerializeField]
    Button fileButton;

    [SerializeField]
    Button trackerButton;

    [SerializeField]
    Button fileOkButton;

    [SerializeField]
    Button trackerOkButton;

    [SerializeField]
    Button fileCloseButton;

    [SerializeField]
    Button trackerCloseButton;

    List<string> sportsList;
    List<string> genderList;

    string sportsStr;
    string genderStr;

    private void Awake()
    {
        motionData = new MotionData();
        trackedObjectList = new TrackedObjectList();
        sportsList = new List<string>();
        genderList = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        filePanel.SetActive(false);

        sportsList.Add("None");
        sportsList.Add("Bowling");
        sportsList.Add("Golf");
        sportsList.Add("Walking");

        sports.ClearOptions();
        sports.AddOptions(sportsList);
        sports.value = 0;

        genderList.Add("None");
        genderList.Add("Male");
        genderList.Add("Female");
        genderList.Add("Other");

        gender.ClearOptions();
        gender.AddOptions(genderList);
        gender.value = 0;

        fileButton.onClick.AddListener(delegate { filePanel.SetActive(true); });
        fileOkButton.onClick.AddListener(delegate { ClickFileOkButton(); });
        fileCloseButton.onClick.AddListener(delegate { filePanel.SetActive(false); });

        sports.onValueChanged.AddListener(delegate { SetSportsStr(); });
        gender.onValueChanged.AddListener(delegate { SetGenderStr(); });

        leftWrist.ClearOptions();
        leftWrist.AddOptions(trackedObjectList.GetTrackerList());
        leftWrist.value = 0;

        rightWrist.ClearOptions();
        rightWrist.AddOptions(trackedObjectList.GetTrackerList());
        rightWrist.value = 0;

        waist.ClearOptions();
        waist.AddOptions(trackedObjectList.GetTrackerList());
        waist.value = 0;

        leftFoot.ClearOptions();
        leftFoot.AddOptions(trackedObjectList.GetTrackerList());
        leftFoot.value = 0;

        rightFoot.ClearOptions();
        rightFoot.AddOptions(trackedObjectList.GetTrackerList());
        rightFoot.value = 0;

        trackerButton.onClick.AddListener(delegate { trackerPanel.SetActive(true); });
        trackerOkButton.onClick.AddListener(delegate { ClickTrackerOkButton(); });
        trackerCloseButton.onClick.AddListener(delegate { trackerPanel.SetActive(false); });

        leftWrist.onValueChanged.AddListener(delegate { SetLeftWristTracker(); });
        rightWrist.onValueChanged.AddListener(delegate { SetRightWristTracker(); });
        waist.onValueChanged.AddListener(delegate { SetWaistTracker(); });
        leftFoot.onValueChanged.AddListener(delegate { SetLeftFootTracker(); });
        rightFoot.onValueChanged.AddListener(delegate { SetRightFootTracker(); });
    }

    void SetSportsStr()
    {
        if (sports.value == 0)
        {
            sportsStr = null;
        }
        else if (sports.value == 1)
        {
            sportsStr = "Bowling";
        }
        else if (sports.value == 2)
        {
            sportsStr = "Golf";
        }
        else if(sports.value == 3)
        {
            sportsStr = "Walking";
        }
    }

    void SetGenderStr()
    {
        if (gender.value == 0)
        {
            genderStr = null;
        }
        else if (gender.value == 1)
        {
            genderStr = "Male";
        }
        else if (gender.value == 2)
        {
            genderStr = "Female";
        }
        else if(gender.value == 3)
        {
            genderStr = "Other";
        }
    }

    void SetLeftWristTracker()
    {

    }

    void SetRightWristTracker()
    {

    }

    void SetWaistTracker()
    {

    }

    void SetLeftFootTracker()
    {

    }

    void SetRightFootTracker()
    {

    }

    void ClickFileOkButton()
    {
        filePanel.SetActive(false);
        motionData.MakeFileName(sportsStr, generation.text, genderStr, age.text, subjectName.text, date.text, actNumber.text);
    }

    void ClickTrackerOkButton()
    {
        trackerPanel.SetActive(false);
    }
}
