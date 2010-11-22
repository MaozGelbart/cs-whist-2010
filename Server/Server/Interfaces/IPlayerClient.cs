using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Server.API;

namespace WCFServer
{
    public interface IPlayerClient
    {
        [OperationContract(IsOneWay = true)]
        void RecieveRoundStatusUpdate(RoundStatus status);

        [OperationContract(IsOneWay = true)]
        void RecieveGameStatusUpdate(GameStatus status);

        [OperationContract(IsOneWay = true)]
        void RecieveCards(Card[] cards);

        [OperationContract(IsOneWay = true)]
        void RecieveExchangedCards(Card[] cards);

        [OperationContract(IsOneWay = true)]
        void RequestExchangeCards();

        [OperationContract(IsOneWay = true)]
        void RequestBid();

        [OperationContract(IsOneWay = true)]
        void ReqeustContract();

        [OperationContract(IsOneWay = true)]
        void RequestPlay();

        [OperationContract(IsOneWay = true)]
        void RecieveStatusCards(RoundStatus status, Card[][] allCards);
    }
}
