using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UsernameWizard : MonoBehaviour
{
    public Text username;
    public Text gold;
    public Text diamond;

    public GameObject usernameWizard;
    public Button ConfirmButton;
    public InputField ipUsername;

    private FirebaseDatabaseManager databaseManager;

    private void Start()
    {
        Debug.Log("UsernameWizard.Start() bắt đầu.");

        GameObject databaseObject = GameObject.Find("DatabaseManager");

        if (databaseObject == null)
        {
            Debug.LogError("Không tìm thấy GameObject 'DatabaseManager'.");
            return;
        }

        databaseManager = databaseObject.GetComponent<FirebaseDatabaseManager>();

        if (databaseManager == null)
        {
            Debug.LogError(
                "GameObject 'DatabaseManager' không có component FirebaseDatabaseManager.");
            return;
        }

        if (ConfirmButton == null)
        {
            Debug.LogError("ConfirmButton chưa được gán trong Inspector.");
            return;
        }

        if (usernameWizard == null)
        {
            Debug.LogError("usernameWizard chưa được gán trong Inspector.");
            return;
        }

        ConfirmButton.onClick.AddListener(SetNewUsername);

        StartCoroutine(CheckUsername());
    }

    private IEnumerator CheckUsername()
    {
        Debug.Log("Đang chờ dữ liệu UserData từ Firebase...");

        while (LoadDataManager.UserData == null)
        {
            yield return null;
        }

        User userData = LoadDataManager.UserData;

        Debug.Log(
            "Dữ liệu hiện tại - Name: " + userData.Name
            + " | Gold: " + userData.Gold
            + " | Diamond: " + userData.Diamond);

        if (username != null)
        {
            username.text = userData.Name ?? string.Empty;
        }
        else
        {
            Debug.LogError("Text username chưa được gán trong Inspector.");
        }

        if (gold != null)
        {
            gold.text = "Gold: " + userData.Gold;
        }
        else
        {
            Debug.LogError("Text gold chưa được gán trong Inspector.");
        }

        if (diamond != null)
        {
            diamond.text = "Diamond: " + userData.Diamond;
        }
        else
        {
            Debug.LogError("Text diamond chưa được gán trong Inspector.");
        }

        bool hasUsername = !string.IsNullOrWhiteSpace(userData.Name);

        // Chỉ bật wizard khi user chưa có username.
        usernameWizard.SetActive(!hasUsername);

        Debug.Log("Đã cập nhật giao diện username, gold và diamond.");
    }

    private void Update()
    {
    }

    public void SetNewUsername()
    {
        string username = ipUsername.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username không được để trống.");
            return;
        }

        if (databaseManager == null)
        {
            Debug.LogError(
                "DatabaseManager đang bị null. Hãy kiểm tra GameObject 'DatabaseManager' trong Scene.");
            return;
        }

        if (LoadDataManager.firebaseUser == null)
        {
            Debug.LogError(
                "FirebaseUser đang bị null. Người dùng chưa đăng nhập.");
            return;
        }

        if (LoadDataManager.UserData == null)
        {
            Debug.LogError("UserData đang bị null.");
            return;
        }

        LoadDataManager.UserData.Name = username;

        databaseManager.UpdateUsername(
            LoadDataManager.firebaseUser.UserId,
            username);

        usernameWizard.SetActive(false);
    }
}