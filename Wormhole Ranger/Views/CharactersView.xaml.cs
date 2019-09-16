using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Wormhole_Ranger.BusinessLogic;

namespace Wormhole_Ranger.Views
{
    /// <summary>
    /// Interaction logic for CharactersView.xaml
    /// </summary>
    public partial class CharactersView : UserControl
    {
        private delegate void SetTextCallback(string text);

        public CharactersView()
        {
            InitializeComponent();

            lblAuthorizationInfo.Text = Localization.Messages.Get("TextAuthorizationInfo");
            lblAction.Text = Localization.Messages.Get("LoadAllPilotesFromStorage");
            lblUpdateLog.Text = "";

            if (ApplicationSettings.Characters.Count > 0)
            {
                lblAction.Visibility = System.Windows.Visibility.Visible;
                lblAuthorizationInfo.Visibility = System.Windows.Visibility.Hidden;
                lblUpdateLog.Visibility = System.Windows.Visibility.Visible;
                LoadPilots();
            }
            else
            {
                lblAction.Visibility = System.Windows.Visibility.Hidden;
                lblAuthorizationInfo.Visibility = System.Windows.Visibility.Visible;
                lblUpdateLog.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private async void LoadPilots()
        {
            List<Character> pilots = await System.Threading.Tasks.Task.Run(() => AuthorizePilots());
        }

        private List<Character> AuthorizePilots()
        {
            var pilots = new List<Character>();

            foreach (var pilot in ApplicationSettings.Characters.List)
            {
                try
                {
                    SetText(string.Format(Localization.Messages.Get("StartAuthorizePilot"), pilot.Name));

                    var currentPilot = new Character {Id = pilot.Id, Name = pilot.Name, Token = pilot.Token };

                    pilots.Add(currentPilot);
                }
                catch (Exception ex)
                {
                    //Log.ErrorFormat("[whlAuthorization.LoadAllPilotesFromStorage] Critical error. Exception {0}", ex);
                }
            }

            return pilots;
        }

        private void SetText(string text)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new SetTextCallback(SetText), text);
                return;
            }

            lblUpdateLog.Text = text;
        }

        private void ImgLoginToEveAccount_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
