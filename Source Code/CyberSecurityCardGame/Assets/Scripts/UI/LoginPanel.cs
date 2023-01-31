using System.Collections;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Google;
// using System.Threading.Tasks;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField email_input, password_input;
    bool password = false, email = false;
    bool loginInProgress = false;
    string errorMsg;

    const string MatchEmailPattern =
    @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
    + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
    + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
    + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    void Start() 
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        // if (FirebaseAuth.DefaultInstance.CurrentUser == null && GoogleSignIn.DefaultInstance != null)
        // {
        //     GoogleSignIn.DefaultInstance.SignOut();
        // }
    }

    void ResetData()
    {
        email_input.text = "";
        password_input.text = "";
        password= false;
        email = false;
        loginInProgress = false;
    }

    public void ValidateUser()
    {
        var user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                MenuUi.instance.setUsernameText(user.DisplayName);
                GameManager.instance.Username = user.DisplayName;
            }
            else
            {
                MenuUi.instance.setUsernameText(user.Email);
                GameManager.instance.Username = user.Email;
            }
            
            GameManager.instance.UserLoggedIn();
            MenuUi.instance.BackHomeClick();
            ResetData();
        }
    }

    public void RegisterPanelClick()
    {
        if (loginInProgress)
        {
            return;
        }
        ResetData();
        MenuUi.instance.ShowRegisterPanel();
    }

    public void LoginClick()
    {
        validateEmail();
        validatePassword();

        if (email && password && !loginInProgress)
        {
            Debug.Log("validateFields validated");
            StartCoroutine(LoginUser(email_input.text, password_input.text));
        }
        else
        {
            MenuUi.instance.ShowErrorPanel(errorMsg);
        }
    }

    public void SignInWithGoogle()
    {
        if (GoogleSignIn.Configuration == null)
        {
            Debug.Log("new GoogleSignInConfiguration");
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                // Copy this value from the google-service.json file.
                // oauth_client with type == 3
                WebClientId = "329159289075-hrcqnpkhdsrirrbn81mvlr3jk9qlnii0.apps.googleusercontent.com"
            }; 
            GoogleSignIn.Configuration.UseGameSignIn = false;
        }

        StartCoroutine(GoogleSignInProcess());
        // Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        // TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        // signIn.ContinueWith(task =>
        // {
        //     if (task.IsCanceled)
        //     {
        //         signInCompleted.SetCanceled();
        //         Debug.Log("IsCanceled ");
        //     }
        //     else if (task.IsFaulted)
        //     {
        //         signInCompleted.SetException(task.Exception);
        //         Debug.Log("IsFaulted ");
        //     }
        //     else
        //     {
        //         Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
        //         StartCoroutine(SignInWithCredential(credential));
        //         Debug.Log("SignInWithCredential ");
        //     }
        // });
    }

    public void validateEmail()
	{
		if (Regex.IsMatch(email_input.text, MatchEmailPattern))
        {
            email_input.GetComponent<Image>().color = Color.white;
            email = true;
        }
        else
        {
            email_input.GetComponent<Image>().color = Color.red;
            email = false;
            errorMsg = "Invalid Email format !";
        }
	}

    public void validatePassword()
    {
        if (string.IsNullOrEmpty(password_input.text) || password_input.text.Length  < 6)
        {
            password_input.GetComponent<Image>().color = Color.red;
            password = false;
            errorMsg = "Password need to be minimum 6 characters long";
        }
        else
        {
            password_input.GetComponent<Image>().color = Color.white;
            password = true;
        }
    }

    IEnumerator LoginUser(string _email, string _password)
    {
        loginInProgress = true;
        MenuUi.instance.ShowLoadingPanel();

        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => loginTask.IsCompleted);
        if (loginTask.Exception != null)
        {
            Debug.Log(loginTask.Exception.InnerException.InnerException);
            MenuUi.instance.ShowErrorPanel(loginTask.Exception.InnerException.InnerException.Message);
        }

        MenuUi.instance.HideLoadingPanel();
        ValidateUser();
        loginInProgress = false;
    }

    IEnumerator GoogleSignInProcess()
    {
        var gSignIn = GoogleSignIn.DefaultInstance.SignIn();
        // var signInCompleted = new TaskCompletionSource<FirebaseUser>();

        yield return new WaitUntil(()=> gSignIn.IsCompleted);
        if (gSignIn.Exception != null)
        {
            MenuUi.instance.ShowErrorPanel(gSignIn.Exception.InnerException.InnerException.Message);
        }
        else
        {
            Debug.Log(gSignIn.Status);
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(gSignIn.Result.IdToken, null);
            StartCoroutine(SignInWithCredential(credential));
        }
    }

    IEnumerator SignInWithCredential(Credential credential) // google login creds
    {
        loginInProgress = true;
        MenuUi.instance.ShowLoadingPanel();

        var mAuth = FirebaseAuth.DefaultInstance;
        if (mAuth == null)
        {
            Debug.Log("mAuth null");
            loginInProgress = false;
            yield break;
        }
        var gSignInTask = mAuth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(()=> gSignInTask.IsCompleted);
        if (gSignInTask.Exception != null)
        {
            Debug.Log(gSignInTask.Exception);
            MenuUi.instance.ShowErrorPanel(gSignInTask.Exception.InnerException.InnerException.Message);
        }

        MenuUi.instance.HideLoadingPanel();
        ValidateUser();
        loginInProgress = false;
    }
}
