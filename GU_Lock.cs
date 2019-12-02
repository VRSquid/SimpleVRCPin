using System;
using System.Linq;
using UnityEngine;
using VRCSDK2;

public class GU_Lock : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject pinMenu;
    public GameObject[] buttons = new GameObject[10];

    private GameObject[] pinButtons;
    private GameObject[] dummyButtons = new GameObject[10];
    private bool firstButton;
    private VRC_Trigger.TriggerEvent ResetEvent;

    public void setNewCode(string newCode) {
        int[] codeArray;
        try {
            codeArray = newCode.Select(t => int.Parse(t.ToString())).ToArray();
        } catch {
            throw new Exception("invalid character in code.");
        }
        pinButtons = new GameObject[codeArray.Length];

        firstButton = true;
        GameObject nextButton = null;
        GameObject tempButton = null;

        for (int i = 0; i < buttons.Length; i++) {
            var dummy = createNewButton(i,false);
            dummyButtons[i] = dummy;
        }

        for (int i = 0; i < codeArray.Length; i++) {
            if (i == 0) {
                dummyButtons[codeArray[i]].SetActive(false);
                tempButton = createNewButton(codeArray[i],true);
                nextButton = createNewButton(codeArray[i + 1],true);
            } else if (i + 1 < codeArray.Length) {
                tempButton = nextButton;
                nextButton = createNewButton(codeArray[i + 1],true);
            } else {
                tempButton = nextButton;
                nextButton = null;
            }

            var trigEvent = new VRC_Trigger.TriggerEvent {
                BroadcastType = VRC_EventHandler.VrcBroadcastType.Local,
                TriggerType = VRC_Trigger.TriggerType.OnInteract
            };

            var trigRef = tempButton.AddComponent<VRC_Trigger>();
            trigRef.Triggers.Add(trigEvent);

            var vrcEvent = new VRC_EventHandler.VrcEvent {
                EventType = VRC_EventHandler.VrcEventType.SetGameObjectActive,
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.True,
                ParameterObjects = new GameObject[1],
            };

            tempButton.SetActive(firstButton);
            firstButton = false;
            vrcEvent.ParameterObjects[0] = nextButton != null ? nextButton : mainMenu;

            if (i + 1 < codeArray.Length && codeArray[i] != codeArray[i + 1]) {
                Array.Resize(ref vrcEvent.ParameterObjects,2);
                vrcEvent.ParameterObjects[1] = dummyButtons[codeArray[i]];
            }

            var vrcEvent2 = new VRC_EventHandler.VrcEvent {
                EventType = VRC_EventHandler.VrcEventType.SetGameObjectActive,
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.False,
                ParameterObjects = new GameObject[1]
            };

            vrcEvent2.ParameterObjects[0] = nextButton != null ? tempButton : pinMenu;

            if (i + 1 < codeArray.Length && codeArray[i] != codeArray[i + 1]) {
                Array.Resize(ref vrcEvent2.ParameterObjects,2);
                vrcEvent2.ParameterObjects[1] = dummyButtons[codeArray[i + 1]];
            }

            trigEvent.Events.Add(vrcEvent);
            trigEvent.Events.Add(vrcEvent2);

            pinButtons[i] = tempButton;
        }

        var distinctCodeArray = codeArray.Distinct().ToArray();
        foreach (var button in dummyButtons) {
            var trig = button.AddComponent<VRC_Trigger>();
            var trigEvent = new VRC_Trigger.TriggerEvent {
                BroadcastType = VRC_EventHandler.VrcBroadcastType.Local,
                TriggerType = VRC_Trigger.TriggerType.OnInteract
            };
            trig.Triggers.Add(trigEvent);

            var vrcEvent = new VRC_EventHandler.VrcEvent {
                EventType = VRC_EventHandler.VrcEventType.SetGameObjectActive,
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.True,
                ParameterObjects = new GameObject[distinctCodeArray.Length]
            };
            vrcEvent.ParameterObjects[0] = pinButtons[0];
            for (int i = 1; i < vrcEvent.ParameterObjects.Length; i++) {
                vrcEvent.ParameterObjects[i] = dummyButtons[distinctCodeArray[i]];
            }

            var vrcEvent2 = new VRC_EventHandler.VrcEvent {
                EventType = VRC_EventHandler.VrcEventType.SetGameObjectActive,
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.False,
                ParameterObjects = new GameObject[codeArray.Length]
            };
            vrcEvent2.ParameterObjects[0] = dummyButtons[codeArray[0]];
            for (int i = 1; i < vrcEvent2.ParameterObjects.Length; i++) {
                vrcEvent2.ParameterObjects[i] = pinButtons[i];
            }

            trigEvent.Events.Add(vrcEvent);
            trigEvent.Events.Add(vrcEvent2);
            ResetEvent = trigEvent;
        }
        pinButtons.Last().GetComponent<VRC_Trigger>().Triggers.Add(ResetEvent);
    }

    private GameObject createNewButton(int index,bool isTemp) {
        GameObject button = new GameObject {
            tag = isTemp ? "dummyButton" : "pinButton",
            layer = LayerMask.NameToLayer("Walkthrough")
        };
        button.transform.SetParent(buttons[index].transform);
        button.transform.localPosition = Vector3.zero;
        button.transform.rotation = new Quaternion();
        button.transform.localScale = Vector3.one;
        BoxCollider boxCollider = button.AddComponent<BoxCollider>();
        boxCollider.center = Vector3.zero;
        boxCollider.size = Vector3.one;
        button.AddComponent<GU_LockButton>();
        return button;
    }
}
