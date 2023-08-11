using System;
using System.Collections.Generic;
using Classes;
using mKit;
using SonoGame;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;


namespace Tutorial
{
    //Everything relating to the tutorial
    //Hastly build with no real structure
    public class TutorialManager : MonoBehaviour
    {
        public TextMeshPro shownText;
        public GameObject frontArea;
        public GameObject compareAnchor;
        public GameObject compareArea;
        public GameObject sonoProbeStand;
        public GameObject sonoProbeInformation;
        public GameObject startButton;
        public SphereCollider rightHandCollider;
        public PlayerInput playerInput;

        //InputManagerParts
        private XRDirectInteractor leftController;

        private GameManager gm;


        private String[] tutorialTexts =
        {
            "Willkommen zu SonoVR!\nDrücke den Trigger links um loszulegen.\nFalls deine Position nicht ganz stimmt drücke links auf die Menü Taste.\nWo welche Taste ist, siehst du zu deiner Linken.",
            "Der Tisch vor dir ist höhenverstellbar. Greif den Griff und ziehe ihn nach belieben nach oben und unten.",
            "Bevor du starten kannst brauchst du noch das richtige Werkzeug um Volumen zu untersuchen.\nDafür ist dir eine SonoSonde™ bereitgestellt.\nMit ihr kannst du Schnittbilder erzeugen.\nGreif sie mit deiner rechten Hand auf um weiter zu machen.",
            "Die SonoSonde ist nun an deine rechte Hand gebunden\nVor dir ist nun ein untersuchbares Volumen. Halte die Sonde auf das Volumen um ein Schnittbild zu erzeugen.\nDas Schnittbild der Sonde siehst du auf der SonoSonde™ selbst und auf der Anzeige hinter diesem Text.",
            "Gut gemacht!\nGreif das Volumen mit deiner linken Hand um es hin und her zu bewegen. Greif es und drücke den linken Trigger um es als Antwort abzugeben.",
            "Zu deiner linken ist nun ein gesuchtes Objekt zugekommen.\nUntersuche die beiden Volumen und wähle die dazugehörige Antwort aus.",
            "Super! Ob die Antwort richtig war oder nicht, hörst du am Ton und siehst du an dem Farbrand.\nJetzt musst du das gesuchte Objekt untersuchen. Der Punkt auf der SonoSonde™ zeigt dir, ob du das Gesuchte(Gelb) oder die Antworten(Blau) untersuchen kannst.",
            "Jetzt solltest du alles wissen, um loszulegen zu können. Falls du dir unsicher mit der Steuerung bist, schau auf den Schildern nach.\n Greif mit der linken Hand die Box um zu starten."
        };

        public int CurrentTutorialTextId { get; private set; }
        private Transform rightController;


        private void Start()
        {
            shownText.text = tutorialTexts[CurrentTutorialTextId];
            rightController = GameObject.Find("Right Controller").transform;
            leftController = GameObject.Find("Left Controller").GetComponent<XRDirectInteractor>();

            ChangeLevelListToTutorialLevel(); //This start gets excecuted before GameManager.Start()

            //Hide SonoGame related objects
            ObjectHidden(frontArea.transform, true);
            ObjectHidden(compareAnchor.transform, true);
            ObjectHidden(compareArea.transform, true);
            sonoProbeStand.SetActive(false);
            startButton.SetActive(false);
            ObjectHidden(sonoProbeInformation.transform, true);
            GameHelper.SetVisibility(rightController.GetChild(0), false);
            rightHandCollider.enabled = false;
        }

        private void ChangeLevelListToTutorialLevel()
        {
            var materialConfig = VolumeManager.Instance.materialConfig;
            gm = FindObjectOfType<GameManager>();
            gm.LevelList = new List<Level>()
            {
                new Level(LevelType.LevelTypes[1],
                    new List<ShapeConfig>() // Shapes in every Volume
                    ,
                    new List<List<ShapeConfig>> // Unique Shapes
                    {
                        new() {LevelHelper.GenerateBasicCube(materialConfig.map[2].color)}
                    }
                ),
                new Level(LevelType.LevelTypes[1],
                    new()
                    {
                        LevelHelper.GenerateRandomizedShape(ShapeType.ELIPSOID, materialConfig.map[3].color,
                            usesSlices: true, rotation: Quaternion.identity)
                    },
                    new List<List<ShapeConfig>> // Unique Shapes
                    {
                        // Volume 1
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.CUBOID, materialConfig.map[5].color,
                                usesSlices: true, size: new Vector3(100, 100, 100),
                                center: new Vector3(100, 150, 100), rotation: Quaternion.identity)
                        },
                        // Volume 2
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.CUBOID, materialConfig.map[5].color,
                                usesSlices: true, size: new Vector3(100, 100, 100),
                                center: new Vector3(100, 150, 100), rotation: Quaternion.identity, edgeWidth: 5)
                        }
                    }
                ),
                new Level(LevelType.LevelTypes[0],
                    new List<ShapeConfig> // Shapes in every Volume
                        {LevelHelper.GenerateBasicCube(materialConfig.map[4].color)},
                    new List<List<ShapeConfig>> // Unique Shapes
                    {
                        // Volume 1
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_X, materialConfig.map[3].color,
                                usesSlices: true, rotation: Quaternion.identity)
                        },
                        // Volume 2
                        new()
                        {
                            LevelHelper.GenerateRandomizedShape(ShapeType.TUBE_X, materialConfig.map[3].color,
                                size: new Vector3(100, 100, 100), usesSlices: true, edgeWidth: 10,
                                rotation: Quaternion.identity)
                        }
                    }
                ),

                new Level(LevelType.LevelTypes[0], new List<ShapeConfig>(), new List<List<ShapeConfig>>())
            };
        }

        private void NextStep()
        {
            //Update Text
            CurrentTutorialTextId++;
            shownText.text = tutorialTexts[CurrentTutorialTextId];
            switch (CurrentTutorialTextId)
            {
                case 2:
                    sonoProbeStand.SetActive(true);
                    rightHandCollider.enabled = true;
                    break;
                case 3:

                    //Switch to SonoProbe in right hand
                    GameHelper.SetVisibility(rightController.GetChild(0), true);
                    rightController.GetChild(1).gameObject.SetActive(false);
                    rightHandCollider.enabled = false;

                    //Change Scene to now show Sono Elements
                    sonoProbeStand.SetActive(false);
                    ObjectHidden(frontArea.transform, false);
                    ObjectHidden(sonoProbeInformation.transform, false);
                    //Deactivate normal input
                    playerInput.enabled = false;
                    break;
                case 4:
                    playerInput.enabled = true;
                    break;
                case 5:
                    ObjectHidden(compareAnchor.transform, false);
                    ObjectHidden(compareArea.transform, false);
                    break;
                case 7:
                    ObjectHidden(frontArea.transform, true);
                    ObjectHidden(compareAnchor.transform, true);
                    ObjectHidden(compareArea.transform, true);
                    startButton.SetActive(true);
                    break;
            }
        }

        private void ObjectHidden(Transform obj, bool toHide)
        {
            obj.position += toHide ? new Vector3(0, -100, 0) : new Vector3(0, 100, 0);
        }

        public void TriggerWithVolume()
        {
            NextStep();
        }


        //Input System
        public void OnLeftTrigger(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (CurrentTutorialTextId == 0) NextStep(); //Only work when initial click & on first text
            if (leftController.hasSelection && CurrentTutorialTextId >= 4)
            {
                int answerId = leftController.interactablesSelected[0]
                    .transform.GetComponent<InteractableInformation>().answerId;
                if (answerId == 0) return;
                NextStep(); //If answered, next tutorialText should be shown
            }
        }

        public void OnLeftGrab(SelectEnterEventArgs args)
        {
            if (CurrentTutorialTextId == 1 && args.interactableObject.transform.name == "Table")
            {
                NextStep();
            }
            else if (CurrentTutorialTextId == 7 && args.interactableObject.transform.name == "StartButton")
            {
                SceneManager.LoadScene("Scenes/MainScene");
            }
        }

        public void OnRightGrab(SelectEnterEventArgs args)
        {
            Debug.Log("Right grabbed");
            if (args.interactableObject.transform.name == "GrabbableSonoProbe")
            {
                NextStep();
            }
        }
    }
}