using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using PlayFab.AuthenticationModels;
using System;

public class Matchmaker : MonoBehaviour
{
    public GameObject playButton;
    public GameObject leaveQueueButton;
    public TMP_Text queuStatus;
    private const string QUEUENAME = "DefaultQueue";

    private string ticketId;
    private Coroutine pollticketCoroutine;
    


    public void StartMatchMaking()
    {
        playButton.SetActive(false);
        queuStatus.text = "Submitting Ticket";
        queuStatus.gameObject.SetActive(true);

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest
            {
                Creator = new MatchmakingPlayer
                {
                    Entity = new PlayFab.MultiplayerModels.EntityKey
                    {
                        Id = PlayFabLoginManager.EntityId,
                        Type = "title_player_account"
                    },
                    Attributes = new MatchmakingPlayerAttributes
                    {
                        DataObject = new { }
                    }
                },
                GiveUpAfterSeconds = 120,
                QueueName = QUEUENAME
            },
            OnMatchMakingTicketCreated,
            OnMatchMakingError
            );
    }

    private void OnMatchMakingTicketCreated(CreateMatchmakingTicketResult obj)
    {
        ticketId = obj.TicketId;
        pollticketCoroutine = StartCoroutine(PollTicket());
        leaveQueueButton.SetActive(true);
        queuStatus.text = "Ticket Creation Succesful";
    }


    private void OnMatchMakingError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }


    private IEnumerator PollTicket()
    {
        while (true)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket(
               new GetMatchmakingTicketRequest
               {
                   TicketId = ticketId,
                   QueueName = QUEUENAME
               },
               OnGetMatchmakingTicket,
               OnMatchMakingError

               );
            yield return new WaitForSeconds(6);
     
        }
    }

    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult obj)
    {
       queuStatus.text = $"Status :{obj.Status}";

        switch (obj.Status)
        {
            case "Matched":
                StopCoroutine(PollTicket());
                StartMatch(obj.MatchId);
                break;

            case "Canceled":
                break;

        }
    }

    private void StartMatch(string _matchId)
    {
        queuStatus.text = $"StartingMatch";
        PlayFabMultiplayerAPI.GetMatch(
            new GetMatchRequest
            {
                MatchId = _matchId,
                QueueName = QUEUENAME
            },
            OngetMatch,
            OnMatchMakingError
            );
    }

    private void OngetMatch(GetMatchResult obj)
    {
        queuStatus.text = $"{obj.Members[0].Entity.Id} - VX - {obj.Members[1]}";
    }
}
