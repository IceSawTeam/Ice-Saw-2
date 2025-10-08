using Raylib_cs;


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