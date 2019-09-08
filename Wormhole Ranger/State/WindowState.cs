using System.Windows;

namespace Wormhole_Ranger.State
{
    public class WindowCurrentStatus
    {
        public int Height { get; set; } = 420;

        public int Width { get; set; } = 640;

        public bool IsPinned { get; set; } = false;

        public Point Location { get; private set; } = new Point(0,0);

        public States State { get; private set; } = States.Maximated;

        private int PreviousHeight { get; set; } 

        private int PreviousWidth { get; set; }

        public WindowCurrentStatus()
        {
            PreviousHeight = Height;
            PreviousWidth = Width;
        }

        public void ChangePinState(bool isPinned)
        {
            IsPinned = isPinned;
        }

        public void ChangeState(States newState)
        {
            switch (newState)
            {
                case States.Maximated:
                    Height = PreviousHeight;
                    Width = PreviousWidth;
                    break;

                case States.Minimizated:
                    PreviousHeight = Height;
                    PreviousWidth = Width;

                    Height = 25;
                    break;

                case States.Hidden:
                    break;
                default:
                    break;
            }

            State = newState;
        }

        public void ChangeLocation(Point newLocation)
        {
            Location = newLocation;
        }
    }

    public enum States
    {
        Maximated,
        Minimizated,
        Hidden
    }
}
