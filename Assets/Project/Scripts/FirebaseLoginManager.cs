using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
    //Dki
    [Header("Register")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;

    public Button buttonRegister;

    //Dang nhap
    [Header("Login")]
    public InputField ipLoginEmail;
    public InputField ipLoginPassword;

    public Button buttonLogin;

    //Firebase Authentication
    private FirebaseAuth auth;

    //Chuyển đổi qua lại đky , đn
    [Header("Switch form")]
    public Button buttonMoveToSignIn;
    public Button buttonMoveToRegister;

    public GameObject loginForm;
    public GameObject registerForm;

    private FirebaseDatabaseManager databaseManager;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        buttonRegister.onClick.AddListener(RegisterAccountWithFirebase);
        buttonLogin.onClick.AddListener(SignInAccountWithFirebase);

        buttonMoveToRegister.onClick.AddListener(SwitchForm);
        buttonMoveToSignIn.onClick.AddListener(SwitchForm);

        databaseManager =GetComponent<FirebaseDatabaseManager>();
    }

    public void RegisterAccountWithFirebase()
    {
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.Log("Dang ki bi huy");
                return;
            }

            if (Task.IsFaulted)
            {
                Debug.Log("Dang ki that bai");
                return;
            }

            if (Task.IsCompleted)
            {
                Debug.Log("Dang ki thanh cong");

                Map mapInGame = new Map();
                User userInGame = new User("", 100, 50, new Map());

                LoadDataManager.firebaseUser = Task.Result.User;

                databaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId, userInGame.ToString());

                SceneManager.LoadScene(sceneName: "Playscene");
            }
        });
    }

    public void SignInAccountWithFirebase()
    {
        string email = ipLoginEmail.text;
        string password = ipLoginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.Log("Dang nhap bi huy");
                return;
            }

            if (Task.IsFaulted)
            {
                Debug.Log("Dang nhap that bai");
                return;
            }

            if (Task.IsCompleted)
            {
                Debug.Log("Dang nhap thanh cong");
                LoadDataManager.firebaseUser = Task.Result.User;

                //chuyen scene sau khi dang nhap
                SceneManager.LoadScene(sceneName: "Playscene");
                return;
            }
        });
    }

    public void SwitchForm()
    {
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);

    }
}
