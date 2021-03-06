﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFServer.Clients;
using Brain;
using Server.Clients;
using Server.API;
using AccountCenter;

namespace WCFServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" in code, svc and config file together.
    public class PlayerService : IPlayerService
    {
        private static Dictionary<string, PlayerSLAdaptor> Adaptors = new Dictionary<string, PlayerSLAdaptor>();
        private static Dictionary<string, ViewerSLAdaptor> ViewerAdaptors = new Dictionary<string, ViewerSLAdaptor>();
        private static PlayerSLAdaptor GetAdaptorBySessionID(string sessionID)
        {
            return Adaptors[sessionID];
        }
        private static ViewerSLAdaptor GetViewerAdaptorBySessionID(string sessionID)
        {
            return ViewerAdaptors[sessionID];
        }

        #region IPlayerService Members

        public void Register(string name, string photoUrl, string game_name)
        {
            if (!Adaptors.ContainsKey(OperationContext.Current.SessionId))
            {
                PlayerSLAdaptor adaptor = new PlayerSLAdaptor(name,
                    OperationContext.Current.SessionId, photoUrl,
                    OperationContext.Current.GetCallbackChannel<IPlayerClient>());
                PlayerService.Adaptors.Add(adaptor.SessionID, adaptor);
                if (!Brain.GameFactory.AddPlayer(adaptor, game_name))
                    PlayerService.Adaptors.Remove(adaptor.SessionID);
            }
        }

        public void StartGame(string name, string photoUrl, int numberOfAIPlayers, string[] player_AI, int num_of_rounds, int milliseconds_between_turns, string game_name)
        {
            if (Adaptors.ContainsKey(OperationContext.Current.SessionId))
            {
                GameFactory.Games.Clear();
                Adaptors.Remove(OperationContext.Current.SessionId);
            }
            PlayerSLAdaptor adaptor = new PlayerSLAdaptor(name,
                OperationContext.Current.SessionId, photoUrl,
                OperationContext.Current.GetCallbackChannel<IPlayerClient>());
            PlayerService.Adaptors.Add(adaptor.SessionID, adaptor);
            Brain.GameFactory.CreateGame(adaptor, numberOfAIPlayers, player_AI,  num_of_rounds,  milliseconds_between_turns, game_name);
        }

        public void RequestUpdate()
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            adaptor.InvokeOnUpdateStatusRequested();
        }

        public void SwitchCards(Server.API.Card[] cards)
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            adaptor.InvokeOnGetExchangedCardsCompleted(cards);
        }

        public void MakeBid(Server.API.Bid? bid)
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            adaptor.InvokeGetBidCompleted(bid);
        }

        public void MakeContract(int amount)
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            adaptor.InvokeOnGetContractCompleted(amount);
        }

        public void PlayCard(Server.API.Card play)
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            adaptor.InvokeOnGetPlayCompleted(play);
        }

        PlayerPlugin[] IPlayerService.GetPlayerPlugIns()
        {
            return PlayerFactory.GetPlayerPlugIns();
        }

        public void StartGameView(string[] player_AI, int num_of_rounds, int milliseconds_between_turns)
        {
            ViewerSLAdaptor adaptor = new ViewerSLAdaptor(OperationContext.Current.SessionId,
                OperationContext.Current.GetCallbackChannel<IPlayerClient>());
            PlayerService.ViewerAdaptors.Add(adaptor.SessionID, adaptor);
            Brain.GameFactory.CreateGame(adaptor, player_AI, num_of_rounds, milliseconds_between_turns, null);
        }

        public void FinishGame()
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            if (adaptor != null)
            {
                adaptor.KillGame();
                Adaptors.Remove(OperationContext.Current.SessionId);
            }
            else
            {
                ViewerSLAdaptor viewer = PlayerService.GetViewerAdaptorBySessionID(OperationContext.Current.SessionId);
                if (viewer != null)
                {
                    viewer.KillGame();
                    ViewerAdaptors.Remove(OperationContext.Current.SessionId);
                }
            }
        }

        public void SendChatMessage(string msg)
        {
            PlayerSLAdaptor adaptor = PlayerService.GetAdaptorBySessionID(OperationContext.Current.SessionId);
            if (adaptor != null)
            {
                adaptor.SendMessage(msg);
            }
        }

        #endregion

        internal static void KillPlayer(PlayerSLAdaptor playerSLAdaptor)
        {
            // get rid of this player
            Adaptors.Remove(playerSLAdaptor.SessionID);
        }

        #region Facebook interface Members


        public AccountCenter.Account RegisterFacebookAccount(string id)
        {
            return AccountDataProvider.Login(id);
        }

        public void UpdateAccount(AccountCenter.Account account)
        {
            AccountDataProvider.UpdateAccount(account);
        }

        #endregion
    }
}
