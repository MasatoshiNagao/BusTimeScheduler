using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class QuestManagement : MonoBehaviour
{

    private Quest[] questList;
    public int totalQuest;
    private bool[] questFlag; //クエストが終了しているかどうか

    private GameObject questUI;
    //private GameObject background;
    public ShowQuest showQuest; //クエストを表示するスクリプト

    //private int page = 1;

    // Use this for initialization
    void Start()
    {

        //　クエスト関連のオブジェクト領域確保
        questList = new Quest[totalQuest];
        questFlag = new bool[totalQuest];

        //　クエストUIのゲームオブジェクトを変数に入れる
        questUI = transform.Find("QuestUI").gameObject; //子クラスを探すならtransform(自分のコメント)
        //background = transform.Find("QuestUI/Background").gameObject;

        //　サンプルの説明文を設定しクエストインスタンスを大量生産
        //　本来は一つ一つクエストの説明文を記述して作成する
        for (int i = 0; i < totalQuest; i++)
        {
            //questList[i] = new Quest("タイトル" + i, "テストの説明" + i);
            questList[i] = new Quest("", "表示すべきデータがありません");
        }
        //　それぞれのクエスト情報を表示（確認の為）
        /*for (int i = 0; i < totalQuest; i++)
        {
            Debug.Log(questList[i].GetTitle() + ":" + questList[i].GetInformation());
        }*/

        //questUI.transform.Find("Background/ButtonPanel/Page/Number").GetComponent<Text>().text = "Page  " + page + "/" + (totalQuest / (background.transform.childCount - 2));

        //StartCoroutine(receiver.GET());
    }

    // Update is called once per frame
    void Update()
    {

        //　Qキーを押したらクエスト画面を開く
        if (Input.GetKeyDown("q"))
        {
            //　ShowQuestスクリプトのShow関数を呼び出し情報を書き換える
            showQuest.Show(0);
            questUI.SetActive(!questUI.activeSelf);
        }

        /*if (transform.Find("Cube").gameObject != null)
        {
            //　ShowQuestスクリプトのShow関数を呼び出し情報を書き換える
            showQuest.Show(0);
            questUI.SetActive(!questUI.activeSelf);
        }*/
    }

    //　クエスト終了をセット
    public void SetQuestFlag(int num)
    {
        questFlag[num] = true;
    }

    //　クエストが終了しているかどうか
    public bool IsQuestFlag(int num)
    {
        return questFlag[num];
    }

    //　クエストを返す
    public Quest GetQuest(int num)
    {
        return questList[num];
    }

    //　トータルクエスト数を返す
    public int GetTotalQuest()
    {
        return totalQuest;
    }

    public void SetQuest(IList<TimeData2> data/*List<TimeData> data*/, int hour, int minute, int[] number, int number_h, bool exist)
    { //左から{時刻表の全体のリスト,表示すべきhour,表示すべきmin,number[時間帯]でminの数を表す,時刻表のhourの数}

        if (exist == false) //表示するべきデータがないとき
        {
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "表示すべきデータがありません";
        }
        else{
            //表示させるべきデータの総数を計算
            int amount = 0; //表示するデータの総量
            amount = number[hour] - minute; //要計算 hourの時間帯の表示すべきminの数
            for (int i = hour + 1; i < number_h; i++) //それ以降の時間帯を全部計算
            {
                amount = amount + number[i];
            }

            /*Debug.Log(data);
            Debug.Log("表示すべきhour" + hour);
            Debug.Log("表示すべきminute" + minute);
            foreach (int i in number) {
                Debug.Log(i);
            }
            Debug.Log("時刻表のhourの数" + number_h);
            Debug.Log("表示すべきデータの総量" + amount);*/

            if (amount > totalQuest) //表示できるデータの限界
            {
                amount = totalQuest;
            }

            //データの格納
            for (int i = 0; i < amount/*表示すべきデータの総量*/; i++)
            {
                string time_minute = data[hour].min[minute].ToString();
                if(time_minute == "0") //minuteが0の時は00にする(見やすさ)
                {
                    time_minute = "00";
                }
                questList[i] = new Quest("未通過", data[hour].hour.ToString() + ":" + time_minute); //ステータスと時刻を表示
                questFlag[i] = false;
                minute++;
                if (minute == number[hour])
                {
                    hour++;
                    minute = 0;
                }
            }

        }


    }

    public void SetStatus(int number)
    { //未通過を通過済みに
        questFlag[number] = true;
        questList[number].title = "通過済み";
    }
}
