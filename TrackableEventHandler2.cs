using UnityEngine;
using Vuforia;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class TrackableEventHandler2 : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    public ShowQuest showQuest; //クエストを表示するスクリプト
    public ShowQuest showQuest2;
    private GameObject questUI;
    private GameObject questUI2;
    //private int quest_on = 0; //ImageTargetの子には設置しないので必要ない

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        questUI = /*transform*/GameObject.Find("QuestManagement/QuestUI").gameObject; //子クラスを探すならtransform(自分のコメント)
        questUI.SetActive(!questUI.activeSelf); //上記の1行でquestUIがエディタ上に存在しないとエラーを吐くため、場所を設定してからUIを消すようにする
        questUI2 = /*transform*/GameObject.Find("QuestManagement2/QuestUI").gameObject;
        questUI2.SetActive(!questUI2.activeSelf);
        //questManagement2 = /*transform*/GameObject.Find("QuestManagement2").gameObject;
        //questManagement2.SetActive(!questManagement2.activeSelf);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        showQuest.Show(0);
        showQuest2.Show(0);
        if (questUI.activeSelf == false)//物体が見つかって失う動作を2回してしまった時にそのままUIが表示されるようにする
        {
            //if (quest_on == 0)
            //{
            questUI.SetActive(!questUI.activeSelf);
                //quest_on = 1;
            //}
        }
        if (questUI2.activeSelf == false)//物体が見つかって失う動作を2回してしまった時にそのままUIが表示されるようにする
        {
            //if (quest_on == 0)
            //{
            questUI2.SetActive(!questUI2.activeSelf);
            //quest_on = 1;
            //}
        }
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    #endregion // PRIVATE_METHODS
}
