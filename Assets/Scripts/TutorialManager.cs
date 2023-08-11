using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

//Everything relating to the tutorial
public class TutorialManager : MonoBehaviour
{
    public TextMeshPro shownText;

    public String[] tutorialTexts = new String[]
    {
        "Willkommen zu SonoVR!\nDrücke den Trigger links um loszulegen.\nFalls deine Position nicht ganz stimmt drücke links auf die Menü Taste.\nWelche Taste welche ist, siehst du zu deiner Linken.",
        "Bevor du starten kannst brauchst du noch das richtige Werkzeug um Volumen zu untersuchen.\nDafür ist dir eine SonoSonde™ bereitgestellt.\nMit ihr kannst du Schnittbilder erzeugen.\nGreif sie mit deiner rechten Hand auf um weiter zu machen.",
        "Vor dir ist nun ein untersuchbares Volumen. Halte die Sonde auf das Volumen um ein Schnittbild zu erzeugen.\nDas Schnittbild der Sonde siehst du auf der SonoSonde™ selbst und auf der Anzeige hinter diesem Text.",
        "Gut gemacht!\nGreif das Volumen mit deiner linken Hand um es hin und her zu bewegen. Greif es und drücke den linken Trigger um es als Antwort abzugeben.",
        "Zu deiner linken ist nun ein gesuchtes Objekt zugekommen.\nUntersuche die beiden Volumen und wähle die dazugehörige Antwort aus",
        "Super! Ob die Antwort richtig war oder nicht, hörst du am Ton und siehst du an den Objekten.\nJetzt musst du das gesuchte Objekt untersuchen. Der Punkt auf der SonoSonde™ zeigt dir, ob du das Gesuchte(Gelb) oder die Antworten(Blau) untersuchen kannst.",
    };

    private int currentTutorialTextId = 0;
    private Transform rightController;

    private void Start()
    {
        shownText.text = tutorialTexts[currentTutorialTextId];
        rightController = GameObject.Find("Right Controller").transform;
    }

    private void NextStep()
    {
        //Update Text
        currentTutorialTextId++;
        shownText.text = tutorialTexts[currentTutorialTextId];
        
    }


    //Input System
    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (context.started && currentTutorialTextId == 0) NextStep(); //Only work when initial click & on first text
    }

    public void OnRightGrab(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.name.Contains("Controller"))
        {
            //Switch to SonoProbe in right hand
            rightController.GetChild(0).gameObject.SetActive(true);
            rightController.GetChild(1).gameObject.SetActive(false);
            NextStep();
        }
    }
}