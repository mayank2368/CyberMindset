using System.Collections;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class RegisterPanel : MonoBehaviour
{

    public TMP_InputField username_input, email_input, password_input, confirm_password_input;

    bool passwords = false, email = false, username = false;
    bool registerInProgress = false;
    string errorMsg;

    // email match pattern
    const string MatchEmailPattern =
    @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
    + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
    + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
    + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    void Start() 
    {

    }

    void ResetData()
    {
        passwords = false;
        email = false;
        username = false;
        registerInProgress = false;
        username_input.text = "";
        email_input.text = "";
        password_input.text = "";
        confirm_password_input.text = "";
    }

    void ValidateUser()
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
            
            MenuUi.instance.BackHomeClick();
            ResetData();
        }
    }

    public void RegisterClick()
    {
        validateEmail();
        validateUsername();
        validatePasswords();

        if (email && username && passwords && !registerInProgress)
        {
            Debug.Log("validateFields validated");
            StartCoroutine(RegisterUser(email_input.text, password_input.text, username_input.text));
        }
        else
        {
            MenuUi.instance.ShowErrorPanel(errorMsg);
        }
    }

    public void LoginPanelClick()
    {
        if (registerInProgress)
        {
            return;
        }
        ResetData();
        MenuUi.instance.ShowLoginPanel();
    }

    public void validateUsername()
    {
        if (string.IsNullOrEmpty(username_input.text))
        {
            username_input.GetComponent<Image>().color = Color.red;
            username = false;
            errorMsg = "Username cannot be empty !";
        }
        else
        {
            username_input.GetComponent<Image>().color = Color.white;
            username = true;
        }
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

    public void validatePasswords()
    {
        if (string.IsNullOrEmpty(password_input.text) || password_input.text.Length  < 6 || password_input.text != confirm_password_input.text)
        {
            password_input.GetComponent<Image>().color = Color.red;
            confirm_password_input.GetComponent<Image>().color = Color.red;
            passwords = false;
            if (password_input.text != confirm_password_input.text)
            {
                errorMsg = "Passwords Dont Match !";
            }
            else
            {
                errorMsg = "Password is too short, minimum 6 characters required !";
            }
        }
        else
        {
            password_input.GetComponent<Image>().color = Color.white;
            confirm_password_input.GetComponent<Image>().color = Color.white;
            passwords = true;
        }
    }

    IEnumerator RegisterUser(string _email, string _password, string _username)
    {
        registerInProgress = true;
        MenuUi.instance.ShowLoadingPanel();

        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        //auth.CurrentUser if it doesnt work
        Debug.Log(registerTask.Status);
        if (registerTask.Exception != null)
        {
            Debug.Log(registerTask.Exception);
            MenuUi.instance.ShowErrorPanel(registerTask.Exception.InnerException.InnerException.Message);
        }
        else
        {
            UserProfile userData = new UserProfile();
            userData.DisplayName = _username;
            var update = registerTask.Result.UpdateUserProfileAsync(userData);
            yield return new WaitUntil(() => update.IsCompleted);
        }

        MenuUi.instance.HideLoadingPanel();
        ValidateUser();
        registerInProgress = false;
    }
}
