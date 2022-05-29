# Unity Simple Audio Fade
A simple unity audio fader component that lerps the volume of an Audio Source


* To make setup easier, this script must be added to the same gameobject as an audiosource - no nullreference errors that way if one loses its reference or I forget to set it lol :)
* You can call this in code as myAudioFaderReference.FadeIn() or .FadeTo(float, float) if you want a custom number and fade time. You can also assign it using unity UI buttons if you've got a skip button or whatever.
* I've also added a context menu attribute, so if you right click the script title in inspector, there's 2 methods just to play around and test the basic fading in and out. :)
