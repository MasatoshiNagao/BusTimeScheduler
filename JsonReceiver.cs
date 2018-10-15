using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//using MiniJSON;

[System.Serializable]
public class TimeData
{//最終的には名前もあった方がいいかも？
    public int hour;
    public int[] min; //60個の要素を-1で初期化

    //ここにSimpleJSONでParseした情報を引数にこの構造体の形に当てはめる関数があるといいかも
    /*public TimeData(int hour, int[] min)
    {
        this.hour = hour;
        this.min = min;
    }*/
}
public class JsonReceiver : MonoBehaviour
{

    public QuestManagement questManagement;
    //private int number_h = 0; //受け取ったTimeDataのhourの数
    //private int number_m = 0; //受け取ったTimeDataのminの数
    private int[] number = new int[24]; //number[7] だと7時台のminの数を返す

    //private TimeData[] dataArray = new TimeData[24];
    //private JSONNode json; //SimpleJsonを使うなら

    // Use this for initialization
    void Start()
    {
        // IEnumeratorインターフェースを継承したメソッドは、StartCoroutineでコールする
        StartCoroutine(GET()); //最終的にはTrackableEventHandler2でターゲットを読んだ際に起動させるのがいいかも(引数としてマーカーの種類をとり、urlを変更させる)
    }

    //MiniJson+JsonNodeを用いたバージョン
    /*public IEnumerator GET()
    {//全体をwhileで回す可能性あり(データを受け取るのは1回でよさげ　現在時刻の表示をwhileループで)

        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata?hour=9");
        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata"); //時刻表は全部持ってくることを想定
        WWW www = new WWW("http://172.17.1.9:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://192.168.38.39:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://localhost:3000/test");

        yield return www; //wwwを返した後、1フレーム処理を中断し、再開する

        if (!string.IsNullOrEmpty(www.error))
        {//読み込み失敗時の処理
            Debug.LogError("www Error:" + www.error);
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "urlエラー";
            yield break;
        }
        else
        {
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "jsonデータ受け取り完了";
            Debug.Log("jsonデータ受け取り完了");
            Debug.Log(www.text);
        }

        //TimeData[] dataArray = (TimeData[])JsonHelper.FromJson<TimeData>(www.text);

        //SimpleJson ReadFile()
        //json = JSONNode.Parse(www.text);
        //Debug.Log(json["hour"]);

        //MiniJson
        //JsonデータをIList化


        IList json = (IList)Json.Deserialize(www.text);

        Debug.Log(json);

        TimeData[] dataArray = new TimeData[24];

        List<Dictionary<int, int[]>> dic = new List<Dictionary<int, int[]>>();//もともとは<string, object>だった
        foreach (IDictionary item in json)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            foreach(int key in item.Keys)
            {
                d.Add(key, (int[])item[key]);
                number_h = number_h + 1;
            }

            dic.Add(d);
        } // この操作の完了で、dic内にdic[0]には一つ目の{hour, min}のデータが、dic[1]には二つ目の…と続く

        List<TimeData> dataArray = new List<TimeData>();

        foreach(TimeData data in json)
        {
            dataArray.Add(data);
        }

        Debug.Log(dataArray[0].hour);

       　//TimeData[]にデータを当てはめる作業
        for(int i = 0; i < number_h; i++)
        {
            dataArray[i].hour = (int)(long)dic[i]["hour"];
            Debug.Log(dataArray[i].hour);
            dataArray[i].min = (int[])dic[i]["min"];
        }

        TimeData a = new TimeData((int)dic[0]["hour"], (int[])dic[0]["min"]);

        Debug.Log(a.hour);

        //DataContractJsonSerializer(typeof(Person));
        //dataArray = TimeData[].Parse(www.text);

        //www.txtファイルを読み込む



        //for (int i = 0; i < dataArray.Length; i++)
        //{
        //    Debug.Log("hour: " + dataArray[i].hour);
        //    
        //    for(int j = 0; j < dataArray[i].min.Length; j++)
        //    {
        //        Debug.Log("    min :" + dataArray[i].min[j]);
        //    }
        //}

        for (int i = 0; i < dataArray.Count; i++)
        {
            number[i] = dataArray[i].min.Length;
        }

        transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "jsonデータ解析完了";

        //データの簡易化
        //int now = System.DateTime.Now.Hour;
        int data_h = 0; //最初のデータから指していくインデックス的なもの
        int data_m = 0; //同様
        while (dataArray[data_h].hour < System.DateTime.Now.Hour) //時刻表によっては中途半端に何時台が抜けているとかあるかも
        {
            data_h++;
        }
        while (dataArray[data_h].min[data_m] < System.DateTime.Now.Minute)
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

        questManagement.SetQuest(dataArray, data_h, data_m, number, dataArray.Count); //questManagementが持つquestListに情報を反映させる(引数はdataArray[],data_h,data_m, number[], hourの数)

        //以下、周期的にアップデートする内容
        // string time = transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text; //下のwhile内では直接その場所をもってこないと反映されないらしい(左辺がtimeだと表示されなかった)
        string prace = transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text; //目的地を表示させる予定
        //time = System.DateTime.Now.ToString();

        //int data_h = 0; //最初のデータから指していくインデックス的なもの
        //同様
        int data_number = 0; //全体のデータの数(クエストの数に合わせるため)
        //System.DateTime time = System.DateTime.Now; //なぜか代入を使うとリアルタイムで時刻を変更しなくなる

        while (true) //現在時刻の表示　時間が過ぎたらチェックボックスにチェックなど
        {
            transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text = "" + System.DateTime.Now.ToString();
            //transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text = time.Year.ToString() +":" + time.Month.ToString() + ":" + time.Second.ToString() 
            //    + "   " + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString();
            //クエストの数(持ってきたTimeDataの数)だけforで回してToggleの内容を"通過済み"or"未通過"にしてチェックを入れる + ":" + time.Day.ToString()
            //データを取ってきたその時点での時刻を基にしたデータを取ってくる想定のため、その時刻になったら"通過済み"に変更するようにする
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

            //Debug.Log(time);
            yield return null; //毎フレーム処理(これやらないとエディタが死ぬ)
        }



    }*/

    //JsonUtilityを用いたものだとHololens上で動かない(FromJsonを使った関数でNULLが帰っているらしい)
    public IEnumerator GET()
    {//全体をwhileで回す可能性あり(データを受け取るのは1回でよさげ　現在時刻の表示をwhileループで)

        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata?hour=9");
        //WWW www = new WWW("http://localhost:3000/bus/nishitetsu/hakata"); //時刻表は全部持ってくることを想定
        WWW www = new WWW("http://172.17.1.91:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://192.168.38.39:3000/bus/nishitetsu/hakata"); //Hololens上ではlocalhostは機能しない
        //WWW www = new WWW("http://localhost:3000/test");

        yield return www; //wwwを返した後、1フレーム処理を中断し、再開する

        if (!string.IsNullOrEmpty(www.error))
        {//読み込み失敗時の処理
            Debug.LogError("www Error:" + www.error);
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "urlエラー";
            yield break;
        }
        else
        {
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "jsonデータ受け取り完了";
            transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = www.text;
        }

        //textファイルの最初と最後にある"["と"]"を削除する必要がある(jsonの形式になっていないみたい)ため
        //JsonHelperを作成した(内容はスクリプトを確認)

        //TimeData data = JsonUtility.FromJson<TimeData>(www.text);

        TimeData[] dataArray = (TimeData[])JsonHelper.FromJson<TimeData>(www.text);

        transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = dataArray[0].hour.ToString();

        //TimeData data = ((TimeData[])JsonHelper.FromJson<TimeData>(www.text))[0];
        //Debug.Log(JsonHelper.FromJson<TimeData>(www.text));

        //Debug.Log(string.Format("{0} : {1}", dataArray[0].hour, dataArray[0].min));
        //Debug.Log(data.min); //配列及びリストの場合はその型しか返ってこないのは仕様(data.min)
        //foreach(int n in dataArray[0].min){ //なのでこんな感じに1つ1つ表示
        //  Debug.Log(n);
        //}
        //Debug.Log("読み込み完了" + www.text);

        //注意 Hololens上でのforeach文はタブー 演算量が増え、その先に進まなくなることがある(以下の文では動かなくなった)
        //foreach (TimeData data in dataArray)
        //{
        //    Debug.Log("hour: " + data.hour);
        //    foreach (int min in data.min)
        //    {
        //        Debug.Log("    min :" + min);
        //        number_m = number_m + 1;
        //    }
        //    number[number_h] = number_m;
        //    number_m = 0;
        //    number_h = number_h + 1;
        //}

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

        for (int i = 0; i < dataArray.Length; i++)
        {
            number[i] = dataArray[i].min.Length;
        }

        //transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text = "jsonデータ解析完了";
        //Debug.Log("Jsonデータ受け取り完了");

        //データの簡易化
        //int now = System.DateTime.Now.Hour;
        int data_h = 0; //最初のデータから指していくインデックス的なもの
        int data_m = 0; //同様
        while (dataArray[data_h].hour < System.DateTime.Now.Hour) //時刻表によっては中途半端に何時台が抜けているとかあるかも
        {
            data_h++;
        }
        while (dataArray[data_h].min[data_m] < System.DateTime.Now.Minute)
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

        //questManagement.SetQuest(dataArray, data_h, data_m, number, dataArray.Length); //questManagementが持つquestListに情報を反映させる(引数はdataArray[],data_h,data_m, number[], hourの数)

        //以下、周期的にアップデートする内容
        // string time = transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text; //下のwhile内では直接その場所をもってこないと反映されないらしい(左辺がtimeだと表示されなかった)
        string prace = transform.Find("QuestUI/Background/InformationPanel/Destination/Place").GetComponent<Text>().text; //目的地を表示させる予定
        //time = System.DateTime.Now.ToString();

        //int data_h = 0; //最初のデータから指していくインデックス的なもの
        //同様
        int data_number = 0; //全体のデータの数(クエストの数に合わせるため)
        //System.DateTime time = System.DateTime.Now; //なぜか代入を使うとリアルタイムで時刻を変更しなくなる

        while (true) //現在時刻の表示　時間が過ぎたらチェックボックスにチェックなど
        {
            transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text = "" + System.DateTime.Now.ToString();
            //transform.Find("QuestUI/Background/InformationPanel/NowTime/Time").GetComponent<Text>().text = time.Year.ToString() +":" + time.Month.ToString() + ":" + time.Second.ToString() 
            //    + "   " + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString();
            //クエストの数(持ってきたTimeDataの数)だけforで回してToggleの内容を"通過済み"or"未通過"にしてチェックを入れる + ":" + time.Day.ToString()
            //データを取ってきたその時点での時刻を基にしたデータを取ってくる想定のため、その時刻になったら"通過済み"に変更するようにする
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

            //Debug.Log(time);
            yield return null; //毎フレーム処理(これやらないとエディタが死ぬ)
        }



    }

    // Update is called once per frame
    void Update()
    {

    }

}