using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;

    private const string baseUrl = "http://localhost:8080/api/users/";

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        string url = baseUrl + "login";
        LoginRequest loginRequest = new LoginRequest(username, password);
        string jsonRequestBody = JsonUtility.ToJson(loginRequest);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonRequestBody))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to login: {www.error}");
            }
            else
            {
                string userDataJson = www.downloadHandler.text;
                User userData = JsonUtility.FromJson<User>(userDataJson);

                if (userData != null)
                {
                    // 로그인 성공, 유저 데이터를 싱글톤으로 관리
                    UserDataManager.Instance.currentUser = userData;

                    // 로그인 후 다음 로직 처리 (예: 다음 씬으로 이동)
                    Debug.Log($"Logged in as {userData.username}, Level: {userData.level}");
                }
                else
                {
                    Debug.Log("Login failed: Invalid username or password.");
                }
            }
        }
    }
}

[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;

    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
