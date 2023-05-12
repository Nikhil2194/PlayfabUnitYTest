using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabLoginManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject MatchMakerPanel;

    [Header("Login Screen")]
    public TMP_InputField LoginEmailField;
    public TMP_InputField LoginPasswordField;
    public Button LoginBtn;
    public Button RegisterBtn;

    [Header("Register Screen")]
    public TMP_InputField RegisterEmailField;
    public TMP_InputField RegisterDisplayNameField;
    public TMP_InputField RegisterPasswordwordField;
    public Button RegisterAccountBtn;
    public Button BackBtn;


    public static string SessionTicket;
    public static string EntityId;


    public void OpenLoginPanel()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void OpenRegistrationPanel()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }


    public void OnTryLogin()
    {
        string email = LoginEmailField.text;
        string password = LoginPasswordField.text;

        LoginBtn.interactable = false;

        LoginWithEmailAddressRequest req = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(req,
        res =>
        {
            SessionTicket = res.SessionTicket;
            EntityId = res.EntityToken.Entity.Id;
            Debug.Log("Login Success");
            LoginPanel.SetActive(false);
            MatchMakerPanel.SetActive(true);
        },
        err =>
        {
            Debug.Log("Error: " + err.ErrorMessage);
            LoginBtn.interactable = true;
        });
    }



    public void OnTryRegisterNewAccount()
    {
        //BackBtn.interactable = false;
        //RegisterAccountBtn.interactable = false;

        string email = RegisterEmailField.text;
        string displayName = RegisterDisplayNameField.text;
        string password = RegisterPasswordwordField.text;

        RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest
        {
            Email = email,
            DisplayName = displayName,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(req,
        res =>
        {
            SessionTicket = res.SessionTicket;
            EntityId = res.EntityToken.Entity.Id;
            BackBtn.interactable = true;
            RegisterAccountBtn.interactable = true;
            OpenLoginPanel();
            Debug.Log(res.PlayFabId);
        },
        err =>
        {
            BackBtn.interactable = true;
            RegisterAccountBtn.interactable = true;
            Debug.Log("Error: " + err.ErrorMessage + "Email-" + email + "username-" + displayName + "password - " + password);
        });
    }
}
