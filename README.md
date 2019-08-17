# SimpleVRCPin

Just a super simple pin editor for VRChat.
1. Drag the provided scripts into the project.

2. Apply the GU_Lock component to any game object. It's best to put it on an object that is the parent of the entire pin/unlocked menu for organizational reasons, but any empty object will work as long as it isn't a child of the pin or unlockable menu.

3. Set up the component.

(Main Menu) is the object to by unlocked after correct entry.

(Pin Menu) is the object that should be the parent of all the pin buttons.

(Buttons) is an array on buttons. It should be set to size 10 and contain inputs for digits 0-9.
Once set to size 10, apply your buttons to the slots accordingly.
I haven't tested it, but you should be able to also create pin pads less than 10 digits but just decreasing the size of the buttons array.

(Global Update) This is a flag that will determine if the pin will be updated with all the other pins. When disabled, the key pad will not be affected by future pin changes.
This property can also be toggled from the editor window, by clicking the checkbox to the left of the key pads name.




To change the pin, select "Simple Keypad" from the top of unity, type your pin in the box at the bottom, and select "Set Password".
