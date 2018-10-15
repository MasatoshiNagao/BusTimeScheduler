
public class Quest
{//クエストクラスの定義

    public string title;
    public string information;

    public Quest()
    {
        title = "タイトルなし";
        information = "内容なし";
    }

    public Quest(string title, string info)
    {
        this.title = title;
        information = info;
    }

    public string GetTitle()
    {
        return title;
    }

    public string GetInformation()
    {
        return information;
    }
}
