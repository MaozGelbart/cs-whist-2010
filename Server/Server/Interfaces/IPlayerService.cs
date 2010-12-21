using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Server.API;
using System.ServiceModel.Activation;

namespace WCFServer
{
    [ServiceContract(Namespace = "Silverlight", CallbackContract = typeof(IPlayerClient))]
    public interface IPlayerService
    {
        [OperationContract]
        void Register(string name, string game_name);

        [OperationContract]
        void StartGame(string name, int number_Of_AIPlayers, string[] player_AI, int num_of_rounds, int milliseconds_between_turns, string game_name);

        [OperationContract]
        void StartGameView(string[] player_AI, int num_of_rounds, int milliseconds_between_turns);

        [OperationContract]
        void RequestUpdate();

        [OperationContract]
        void SwitchCards(Card[] cards);

        [OperationContract]
        void MakeBid(Bid? bid);

        [OperationContract]
        void MakeContract(int amount);

        [OperationContract]
        void PlayCard(Card play);

        [OperationContract]
        PlayerPlugin[] GetPlayerPlugIns();

        [OperationContract]
        void FinishGame();

        [OperationContract]
        void SendChatMessage(string msg);
    }

}
