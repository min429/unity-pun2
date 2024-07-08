using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public InputField userId;
    public InputField userPwd;
    public Button loginButton;

    private const string baseUrl = "http://localhost:8080/api/users/";

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string id = userId.text;
        string pwd = userPwd.text;

        StartCoroutine(LoginCoroutine(id, pwd));
    }

    private IEnumerator LoginCoroutine(string id, string pwd)
    {
        string url = baseUrl + "login";
        LoginRequest loginRequest = new LoginRequest(id, pwd);
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
                User userData = JsonUtility.FromJson<User>(userDataJson); // 로그인 성공, 유저 데이터를 싱글톤으로 관리된 User 클래스에 할당

                // 로그인 성공, 유저 데이터를 싱글톤으로 관리된 User 클래스에 할당
                User.Instance.id = userData.id;
                User.Instance.username = userData.username;
                User.Instance.level = userData.level;

                if (User.Instance != null)
                {                
                    // 로그인 후 다음 로직 처리 (예: 다음 씬으로 이동)
                    Debug.Log($"Logged in as {User.Instance.username}, Level: {User.Instance.level}");
                }
                else
                {
                    Debug.Log("Login failed: Invalid id or pwd.");
                }
            }
        }
    }
}

[System.Serializable]
public class LoginRequest
{
    public string id;
    public string pwd;

    public LoginRequest(string id, string pwd)
    {
        this.id = id;
        this.pwd = pwd;
    }
}
