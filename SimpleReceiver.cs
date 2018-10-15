using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SimpleJSON;
//using System.IO;
using System.Text;

//[System.Serializable]
public class TimeData2
{
    public int hour;
    public IList<int> min;

    public TimeData2(int hour, IList<int> min)
    {
        this.hour = hour;
        this.min = min;
    }

    public override string ToString()
    {
        string minStr = "";
        foreach (int element in min)
        {
            if (minStr == "")
                minStr = element.ToString();
            else
                minStr = minStr + ", " + element.ToString();

        }

        return "hour : " + hour + ", min : [" + minStr + "]";
    }
}

public class PlaceData
{
    public int stop_id;
    public string stop_name;

    public PlaceData(int stop_id, string stop_name)
    {
        this.stop_id = stop_id;
        this.stop_name = stop_name;
    }

    public override string ToString()
    {
        /*string minStr = "";
        foreach (int element in min)
        {
            if (minStr == "")
                minStr = element.ToString();
            else
                minStr = minStr + ", " + element.ToString();

        }*/

        return "stop_id : " + stop_id + ", stop_name : " + stop_name;
    }
}

public class SimpleReceiver : MonoBehaviour
{

    public QuestManagement questManagement;
    //private int number_h = 0; //受け取ったTimeDataのhourの数
    //private int number_m = 0; //受け取ったTimeDataのminの数
    private int[] number = new int[24]; //number[7] だと7時台のminの数を返す

    //private TimeData[] dataArray = new TimeData[24];
    private JSONNode json; //SimpleJsonを使うなら

    Coroutine coroutine;

    public string url; //時刻表用
    public string url2; //場所用


    // Use this for initialization
    void Start()
    {
        // IEnumeratorインターフェースを継承したメソッドは、StartCoroutineでコールする
        coroutine = StartCoroutine(GET(true)); //最終的にはTrackableEventHandler2でターゲットを読んだ際に起動させるのがいいかも(引数としてマーカーの種類をとり、urlを変更させる)
    }

    //JsonUtilityを用いたものだとHololens上で動かない(FromJsonを使った関数でNULLが帰っているらしい)
    public IEnumerator GET(bool loop)
    {//全体をwhileで回す可能性あり(データを受け取るのは1回でよさげ　現在時刻の表示をwhileループで)

        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata?hour=9");
        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata"); //時刻表は全部持ってくることを想定
        //WWW www = new WWW("http://172.17.1.99:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://192.168.38.39:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://localhost:3000/test");
        WWW www = new WWW(url);

        yield return www; //wwwを返した後、1フレーム処理を中断し、再開する

        IList<TimeData2> dataArray = new List<TimeData2>();
        int data_h = 0; //最初のデータから指していくインデックス的なもの
        int data_m = 0; //同様
        bool data_exist = true;

        if (!string.IsNullOrEmpty(www.error))
        {//読み込み失敗時の処理
            //Debug.LogError("www Error:" + www.error);
            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "urlエラー";
            transform.Find("QuestUI/Background/Quest0/InformationPanel/Information").GetComponent<Text>().text = "情報を取得出来ません";
            //yield break;
        }
        else
        {
            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "データ受信完了";
            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = www.text;
            //本来はこの場所に何処発のバス停の情報なのかを記載したい(jsonデータで受け取ってきて)


            //textファイルの最初と最後にある"["と"]"をどうにかする必要がある(jsonの形式になっていないみたい)ため
            //JsonHelperを作成した(内容はスクリプトを確認)

            //TimeData data = JsonUtility.FromJson<TimeData>(www.text);

            //TimeData[] dataArray = (TimeData[])JsonHelper.FromJson<TimeData>(www.text);

            JSONNode timeTable = JSONNode.Parse(www.text); //ここまでが時刻表データの解析

            //IList<TimeData2> dataArray = new List<TimeData2>();

            for (int i = 0; i < timeTable.Count; i++)
            {
                int hour = Convert.ToInt32(((string)timeTable[i]["hour"]));
                IList<int> min = new List<int>();

                for (int j = 0; j < timeTable[i]["min"].Count; j++)
                {
                    min.Add(Convert.ToInt32((string)timeTable[i]["min"][j]));
                }

                TimeData2 td = new TimeData2(hour, min);
                dataArray.Add(td);
            }

            /*foreach (var item in timeTable)
            {
                int hour = (int)item.hour;
                IList<int> min = new List<int>();

                foreach (var data in item.min)
                {
                    min.Add((int)data);
                }

                TimeData2 td = new TimeData2(hour, min);
                dataArray.Add(td);
            }*/

            /*foreach(TimeData2 td in dataArray)
            {
                Debug.Log("TimeData2.hour : " + td.hour);
                Debug.Log("TiimeData2.min : " + td.min[0]);
            }*/

            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = dataArray[0].hour.ToString();

            //TimeData data = ((TimeData[])JsonHelper.FromJson<TimeData>(www.text))[0];
            //Debug.Log(JsonHelper.FromJson<TimeData>(www.text));

            //Debug.Log(string.Format("{0} : {1}", dataArray[0].hour, dataArray[0].min));
            //Debug.Log(data.min); //配列及びリストの場合はその型しか返ってこないのは仕様(data.min)
            //foreach(int n in dataArray[0].min){ //なのでこんな感じに1つ1つ表示
            //  Debug.Log(n);
            //}
            //Debug.Log("読み込み完了" + www.text);

            //Debug.Log("Arrayの長さ" + dataArray.Length);
            //for (int i = 0; i < dataArray.Length; i++)
            //{
            //    Debug.Log("Array[" + i +  "]のminの長さ" + dataArray[i].min.Length);
            //}

            //for (int i = 0; i < dataArray.Length; i++)
            //{
            //    Debug.Log("hour: " + dataArray[i].hour);
            //    
            //    for(int j = 0; j < dataArray[i].min.Length; j++)
            //    {
            //        Debug.Log("    min :" + dataArray[i].min[j]);
            //        number_m = number_m + 1;
            //    }

            //    number[number_h] = number_m;
            //    number_m = 0;
            //    number_h = number_h + 1;
            //}

            for (int i = 0; i < dataArray.Count; i++)
            {
                number[i] = dataArray[i].min.Count;
            }

            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "jsonデータ解析完了";
            //Debug.Log("Jsonデータ受け取り完了");

            //データの簡易化
            //int now = System.DateTime.Now.Hour;
            //int data_h = 0; //最初のデータから指していくインデックス的なもの
            //int data_m = 0; //同様
            //bool data_exist = true;

            if (dataArray[dataArray.Count - 1].hour < System.DateTime.Now.Hour) //そもそも時刻表に現在の時刻以降のデータがなかった時用
            {
                data_exist = false;
            }
            else if (dataArray[dataArray.Count - 1].hour == System.DateTime.Now.Hour)
            {
                if (dataArray[dataArray.Count - 1].min[dataArray[dataArray.Count - 1].min.Count - 1] <= System.DateTime.Now.Minute)
                {
                    data_exist = false;
                }
                else //残りのデータを表示する(追加11/28)
                {
                    data_h = dataArray.Count - 1;
                    while (dataArray[data_h].min[data_m] <= System.DateTime.Now.Minute)
                    {
                        data_m++;
                    }
                }
            }
            else
            { //表示すべき時刻表のデータが1個以上あるとき
                while (dataArray[data_h].hour < System.DateTime.Now.Hour) //時刻表によっては中途半端に何時台が抜けているとかあるかも
                {
                    data_h++;
                }

                while (dataArray[data_h].min[data_m] <= System.DateTime.Now.Minute)
                {
                    if (dataArray[data_h].hour > System.DateTime.Now.Hour) //そもそものhourが違う際には最初のデータから表示できる(例、現在が12時台で12時台のデータがない場合は13時の始めのデータから表示することになる)
                    {
                        break;
                    }
                    else
                    {
                        data_m++;
                        if (data_m == number[data_h])
                        {
                            data_h++;
                            data_m = 0;
                            break;
                        }
                    }
                } //これでdataArray[data_h].min[data_m]が次のバスの時刻のリストの中で最適な時間を指していることになる
            }

            questManagement.SetQuest(dataArray, data_h, data_m, number, dataArray.Count, data_exist); //questManagementが持つquestListに情報を反映させる(引数はdataArray[],data_h,data_m, number[], hourの数, 表示の可否)

        }

        /***************ここからバス停情報の解析***************/
        //WWW www2 = new WWW("http://172.17.1.44:3000/nishitetsu"); //Hololens上ではlocalhostは機能しない
        //WWW www2 = new WWW("http://192.168.38.45:3000/nishitetsu"); //Hololens上ではlocalhostは機能しない
        WWW www2 = new WWW(url2); //Hololens上ではlocalhostは機能しない

        yield return www2;

        if (!string.IsNullOrEmpty(www2.error))
        {
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "情報を取得できません";
            //yield break;
        }
        else
        {
            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "データ受信完了";
            //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = www.text;
            //本来はこの場所に何処発のバス停の情報なのかを記載したい(jsonデータで受け取ってきて)


            JSONNode place_info = JSONNode.Parse(www2.text); //ここまでが時刻表データの解析
            IList<PlaceData> placeArray = new List<PlaceData>();

            for (int i = 0; i < place_info.Count; i++)
            {
                int stop_id = Convert.ToInt32(((string)place_info[i]["stop_id"]));
                /*IList<int> min = new List<int>();

                for (int j = 0; j < timeTable[i]["min"].Count; j++)
                {
                    min.Add(Convert.ToInt32((string)timeTable[i]["min"][j]));
                }*/
                string stop_name = (string)place_info[i]["stop_name"];

                PlaceData pd = new PlaceData(stop_id, stop_name);
                placeArray.Add(pd);
            }

            string departure = placeArray[3].stop_name;
            string destination = placeArray[0].stop_name;

            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = departure + "  ===>  " + destination;
        }
        /******************************************************/

        //以下、周期的にアップデートする内容
        // string time = transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text; //下のwhile内では直接その場所をもってこないと反映されないらしい(左辺がtimeだと表示されなかった)
        //string prace = transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text; //目的地を表示させる予定
        //time = System.DateTime.Now.ToString();

        //int data_h = 0; //最初のデータから指していくインデックス的なもの
        //同様
        int data_number = 0; //全体のデータの数(クエストの数に合わせるため)
        //System.DateTime time = System.DateTime.Now; //なぜか代入を使うとリアルタイムで時刻を変更しなくなる

        string month, day, hour2, minute, second;

        while (loop) //現在時刻の表示　時間が過ぎたらチェックボックスにチェックなど
        {
            //現在時刻の表示(見栄えをよくするために1桁のときは0をつけるようにする(例19:0じゃなくて19:00にしたり))
            //transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text = "" + System.DateTime.Now.ToString();
            if (System.DateTime.Now.Month <= 9) { month =  "0" + System.DateTime.Now.Month.ToString();} else { month = System.DateTime.Now.Month.ToString();}
            if (System.DateTime.Now.Day <= 9) { day = "0" + System.DateTime.Now.Day.ToString(); } else { day = System.DateTime.Now.Day.ToString(); }
            if (System.DateTime.Now.Hour <= 9) { hour2 = "0" + System.DateTime.Now.Hour.ToString(); } else { hour2 = System.DateTime.Now.Hour.ToString(); }
            if (System.DateTime.Now.Minute <= 9) { minute = "0" + System.DateTime.Now.Minute.ToString(); } else { minute = System.DateTime.Now.Minute.ToString(); }
            if (System.DateTime.Now.Second <= 9) { second = "0" + System.DateTime.Now.Second.ToString(); } else { second = System.DateTime.Now.Second.ToString(); }

            transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text =
                System.DateTime.Now.Year + "/" + month + "/" + day + "  " + hour2 + " :" + minute + ":" + second;

            //    + "   " + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString();
            //クエストの数(持ってきたTimeDataの数)だけforで回してToggleの内容を"通過済み"or"未通過"にしてチェックを入れる + ":" + time.Day.ToString()
            //データを取ってきたその時点での時刻を基にしたデータを取ってくる想定のため、その時刻になったら"通過済み"に変更するようにする
            if (string.IsNullOrEmpty(www.error))
            {
                if (dataArray[data_h].hour == System.DateTime.Now.Hour)
                {
                    if (dataArray[data_h].min[data_m] == System.DateTime.Now.Minute)
                    {
                        questManagement.SetStatus(data_number);
                        data_number++;
                        data_m++;
                        if (data_m == number[data_h])
                        {
                            data_h++;
                            data_m = 0;
                        }
                    }
                }
            }

            //Debug.Log(time);
            yield return null; //毎フレーム処理(これやらないとエディタが死ぬ)
        }

        yield break;

    }

    public void UpdateStatus()
    {
        StopCoroutine(coroutine);
        coroutine = StartCoroutine(GET(true));
    }

}