// ﻿using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// // Include Facebook namespace
// using Facebook.Unity;
//
// public class testLoginScreen : MonoBehaviour {
// 	// Awake function from Unity's MonoBehavior
// 	public void Awake ()
// 	{
// 	    if (!FB.IsInitialized) {
// 	        // Initialize the Facebook SDK
// 	        FB.Init(InitCallback, OnHideUnity);
// 	    } else {
// 	        // Already initialized, signal an app activation App Event
// 	        FB.ActivateApp();
// 	    }
// 	}
//
// 	private void InitCallback ()
// 	{
// 	    if (FB.IsInitialized) {
// 	        // Signal an app activation App Event
// 	        FB.ActivateApp();
// 	        // Continue with Facebook SDK
// 	        // ...
// 	    } else {
// 	        Debug.Log("Failed to Initialize the Facebook SDK");
// 	    }
// 	}
//
// 	private void OnHideUnity (bool isGameShown)
// 	{
// 	    if (!isGameShown) {
// 	        // Pause the game - we will need to hide
// 	        Time.timeScale = 0;
// 	    } else {
// 	        // Resume the game - we're getting focus again
// 	        Time.timeScale = 1;
// 	    }
// 	}
//
// 	var perms = new List<string>(){"public_profile", "email", "user_friends"};
//
// 	FB.LogInWithReadPermissions(perms, AuthCallback);
//
// 	private void AuthCallback (ILoginResult result) {
// 	    if (FB.IsLoggedIn) {
// 	        // AccessToken class will have session details
// 	        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
// 	        // Print current access token's User ID
// 	        Debug.Log(aToken.UserId);
// 	        // Print current access token's granted permissions
// 	        foreach (string perm in aToken.Permissions) {
// 	            Debug.Log(perm);
// 	        }
// 	    } else {
// 	        Debug.Log("User cancelled login");
// 	    }
// 	}
//
// 	var tutParams = new Dictionary<string, object>();
// 	tutParams[AppEventParameterName.ContentID] = "tutorial_step_1";
// 	tutParams[AppEventParameterName.Description] = "First step in the tutorial, clicking the first button!";
// 	tutParams[AppEventParameterName.Success] = "1";
//
// 	FB.LogAppEvent (
// 	    AppEventName.CompletedTutorial,
// 	    parameters: tutParams
// 	);
// }
