using Raylib_cs;


/*
    Wraps raylib's input with an Input Action system.

    An action can have multiple events, each with different combinations of keys/mouse buttons,
    Allowing you to bind multiple keys to a single action.
    For example you can use camera boost with either the left click or left shift.

    InputAction names can be found and edited in KeyBindings.cs

    Usable methods:
    - IsActionPressed()
    - IsActionReleased()
    - IsActionDown()

    It's very unlikely that you'll ever use IsActionPressed/Released on an input action's event
    that requires multiple key or mouse button presses. This is because pressing multiple keys in a
    single frame is very unlikely and have no real use.
    If you want to check a key combination then check if the Modifier is held down, and then check for the
    action key being pressed.

    DEVNOTE:
    This system might be scaled and specialized as the project grows, for now its barebones.
    Scroll wheel, mouse motion, and checking for multiple action presses in one frame is not supported.
*/


namespace IceSaw2
{
    public static class Input
    {
        public static bool IsActionPressed(string actionName)
        {
            var action = Settings.KeyBinding.Instance.GetInputActionByName(actionName);
            foreach (var inputEvent in action.InputEvents)
            {
                bool allPressed = true;
                foreach (var input in inputEvent)
                {
                    if (input.GetType() == typeof(KeyboardKey) && !Raylib.IsKeyPressed((KeyboardKey)input))
                    {
                        allPressed = false;
                        break;
                    }
                    if (input.GetType() == typeof(MouseButton) && !Raylib.IsMouseButtonPressed((MouseButton)input))
                    {
                        allPressed = false;
                        break;
                    }
                }
                if (allPressed) return true;
            }
            return false;
        }


        public static bool IsActionReleased(string actionName)
        {
            var action = Settings.KeyBinding.Instance.GetInputActionByName(actionName);
            foreach (var inputEvent in action.InputEvents)
            {
                bool allReleased = true;
                foreach (var input in inputEvent)
                {
                    if (input.GetType() == typeof(KeyboardKey) && !Raylib.IsKeyReleased((KeyboardKey)input))
                    {
                        allReleased = false;
                        break;
                    }
                    if (input.GetType() == typeof(MouseButton) && !Raylib.IsMouseButtonReleased((MouseButton)input))
                    {
                        allReleased = false;
                        break;
                    }
                }
                if (allReleased) return true;
            }
            return false;
        }


        public static bool IsActionDown(string actionName)
        {
            var action = Settings.KeyBinding.Instance.GetInputActionByName(actionName);
            foreach (var inputEvent in action.InputEvents)
            {
                bool allDown = true;
                foreach (var input in inputEvent)
                {
                    if (input.GetType() == typeof(KeyboardKey) && !Raylib.IsKeyDown((KeyboardKey)input))
                    {
                        allDown = false;
                        break;
                    }
                    if (input.GetType() == typeof(MouseButton) && !Raylib.IsMouseButtonDown((MouseButton)input))
                    {
                        allDown = false;
                        break;
                    }
                }
                if (allDown) return true;
            }
            return false;
        }
    }
}