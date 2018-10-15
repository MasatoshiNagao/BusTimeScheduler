using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowQuest : MonoBehaviour
{

    public QuestManagement questManagement;	//　QuestManagementスクリプト
    private GameObject questUI;
    private int num;		           		//　表示する個数
    private Transform[] list;				//　クエストインスタンスを入れる変数
    private int nowPage;                    //　現在開いているページ

    // Use this for initialization
    void Start()
    {
        nowPage = 0;
        transform.Find("ButtonPanel/Page/Number").GetComponent<Text>().text = "Page  " + (nowPage + 1) + "/" + (questManagement.totalQuest / (transform.childCount - 2));
        //　クエストを表示する個数を子要素の数から2引いて（InformationPanel,ButtonPanel分）計算(下部のShow関数で行っている)
        //num = transform.childCount - 1;
    }

    // Update is called once per frame
    void Update()
    {
        num = transform.childCount - 2;
        // 現在のページに受け取った引数を設定

        //　表示する分のインスタンスを確保
        list = new Transform[num];

        for (int i = 0; i < num; i++)
        {

            //　クエスト表示パネルのTransform情報を探す
            list[i] = transform.Find("Quest" + i);

            //　クエスト番号を変数に入れる
            int questNum = (nowPage * num) + i;

            //　クエストがある場合
            if (questNum < questManagement.GetTotalQuest())
            {

                //　クエストの情報を表示ページと表示数を使って計算し取得する
                bool check = questManagement.IsQuestFlag(questNum);
                string title = questManagement.GetQuest(questNum).GetTitle();
                string info = questManagement.GetQuest(questNum).GetInformation(); //追加12/28

                //　取得した情報をUIに反映させる
                list[i].Find("TitlePanel/Toggle").GetComponent<Toggle>().isOn = check;
                list[i].Find("TitlePanel/Toggle/Label").GetComponent<Text>().text = title;
                list[i].Find("InformationPanel/Information").GetComponent<Text>().text = info;//
            }
            //　クエストがない場合
            else
            {
                //　クエスト情報がない為、空文字をセット
                list[i].Find("TitlePanel/Toggle").GetComponent<Toggle>().isOn = false;
                list[i].Find("TitlePanel/Toggle/Label").GetComponent<Text>().text = "";
                list[i].Find("InformationPanel/Information").GetComponent<Text>().text = "";//
            }
        }
    }

    //　アクティブになったら現在のページを初期化
    void OnEnable()
    {
        nowPage = 0;
    }

    public void Show(int pageNum)
    {

        num = transform.childCount - 2;
        // 現在のページに受け取った引数を設定
        nowPage = pageNum;

        //　表示する分のインスタンスを確保
        list = new Transform[num];

        for (int i = 0; i < num; i++)
        {

            //　クエスト表示パネルのTransform情報を探す
            list[i] = transform.Find("Quest" + i);

            //　クエスト番号を変数に入れる
            int questNum = (pageNum * num) + i;

            //　クエストがある場合
            if (questNum < questManagement.GetTotalQuest())
            {

                //　クエストの情報を表示ページと表示数を使って計算し取得する
                bool check = questManagement.IsQuestFlag(questNum);
                string title = questManagement.GetQuest(questNum).GetTitle();
                string info = questManagement.GetQuest(questNum).GetInformation();

                //　取得した情報をUIに反映させる
                list[i].Find("TitlePanel/Toggle").GetComponent<Toggle>().isOn = check;
                list[i].Find("TitlePanel/Toggle/Label").GetComponent<Text>().text = title;
                list[i].Find("InformationPanel/Information").GetComponent<Text>().text = info;
            }
            //　クエストがない場合
            else
            {
                //　クエスト情報がない為、空文字をセット
                list[i].Find("TitlePanel/Toggle").GetComponent<Toggle>().isOn = false;
                list[i].Find("TitlePanel/Toggle/Label").GetComponent<Text>().text = "";
                list[i].Find("InformationPanel/Information").GetComponent<Text>().text = "";
            }
        }
    }
    //　クエスト数とページ数から最大のページを計算しそれより下の場合は次のページへ
    public void NextPage()
    {
        //　表示すべきクエストがまだある時
        if (questManagement.GetTotalQuest() - ((nowPage + 1) * num) > 0)
        {
            Show(nowPage + 1);
            transform.Find("ButtonPanel/Page/Number").GetComponent<Text>().text = "Page  " + (nowPage + 1) + "/" + (questManagement.totalQuest / (transform.childCount - 2));
        }
    }
    //　現在のページが0ページ目でなければ前のページを表示
    public void PrevPage()
    {
        if (nowPage != 0)
        {
            Show(nowPage - 1);
            transform.Find("ButtonPanel/Page/Number").GetComponent<Text>().text = "Page  " + (nowPage + 1) + "/" + (questManagement.totalQuest / (transform.childCount - 2));
        }
    }

    public void Close()
    {
        questUI = GameObject.Find("QuestManagement/QuestUI").gameObject;
        questUI.SetActive(!questUI.activeSelf); //上記の1行でquestUIがエディタ上に存在しないとエラーを吐くため、場所を設定してからUIを消すようにする
    }
}
