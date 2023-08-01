using System;

namespace Classes
{
    public class LevelType
    {
        public readonly ObjectType
            compareObject; //Type of object the player gets to compare to the answerOptions to idenfity the right answer

        public readonly ObjectType answerOptions; // What type of answerOption it is
        public readonly ProbeType toProbe; //What is probable with the Sonoprobe
        public readonly string description;
        public LevelType(ObjectType iCompareObject, ObjectType iAnswerOption, string iDescription)
        {
            compareObject = iCompareObject;
            answerOptions = iAnswerOption;
            description = iDescription;
            toProbe = new Func<ProbeType>(() =>
            {
                //Priority over Probe: HiddenVolume -> Volume -> Slice
                switch (compareObject)
                {
                    case ObjectType.HiddenVolume:
                    case ObjectType.HiddenVolumeAfterglow:
                        return ProbeType.CompareObject;
                    case ObjectType.Volume:
                        return answerOptions == ObjectType.Slice ? ProbeType.CompareObject : ProbeType.AnswerOptions;
                    case ObjectType.Slice: //Included for readability
                        return ProbeType.AnswerOptions;
                    default:
                        return ProbeType.AnswerOptions;
                }
            })();
        }

        public static readonly LevelType[] LevelTypes = new LevelType[]
        {
            new LevelType(ObjectType.Volume,ObjectType.Slice, "Untersuche das Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!"),
            new LevelType(ObjectType.Slice,ObjectType.Volume, "Untersuche die Volumen und finde heraus, welches der Volumen das Schnittbild darstellt!"),
            new LevelType(ObjectType.HiddenVolumeAfterglow,ObjectType.Volume, "Untersuche das unsichtbare Volumen und finde heraus, welches der sichtbaren Volumen identisch ist!"),
            new LevelType(ObjectType.Volume,ObjectType.HiddenVolumeAfterglow, "Untersuche die unsichtbaren Volumen und finde heraus, welches identisch zum sichtbaren Volumen ist!"),
            new LevelType(ObjectType.HiddenVolumeAfterglow,ObjectType.Slice, "Untersuche das unsichtbare Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!"),
            new LevelType(ObjectType.Slice,ObjectType.HiddenVolumeAfterglow, "Untersuche die unsichtbaren Volumen und finde heraus, welches identisch zum sichtbaren Schnittbild ist!"),
            new LevelType(ObjectType.HiddenVolume,ObjectType.Volume, "Untersuche das unsichtbare Volumen und finde heraus, welches der sichtbaren Volumen identisch ist!"),
            new LevelType(ObjectType.Volume,ObjectType.HiddenVolume, "Untersuche die unsichtbaren Volumen und finde heraus, welches identisch zum sichtbaren Volumen ist!"),
            new LevelType(ObjectType.HiddenVolume,ObjectType.Slice, "Untersuche das unsichtbare Volumen und finde heraus, welches der Schnittbilder das Volumen darstellt!"),
            new LevelType(ObjectType.Slice,ObjectType.HiddenVolume, "Untersuche die unsichtbaren Volumen und finde heraus, welches identisch zum sichtbaren Schnittbild ist!")
        };
    }
}