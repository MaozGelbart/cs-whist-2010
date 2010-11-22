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
        void Register(string name);

        [OperationContract]
        void StartGame(string name, int numberOfAIPlayers, string[] player_AI);

        [OperationContract]
        void StartGameView(string[] player_AI);

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
    }

    public class PlayerPlugin
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
