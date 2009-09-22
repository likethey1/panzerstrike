using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.Web.Security;

/// <summary>
/// Summary description for DatabaseManager
/// </summary>
public class DatabaseManager
{
    private SqlConnection connection;
    private string connectionString;
	public DatabaseManager()
    {
        connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString();
        connection = new SqlConnection(connectionString);
	}
    public bool ValidateUser(string username, string password)
    {
        return Membership.ValidateUser(username, password);
    }

    public string[] getUsers() { return Roles.GetUsersInRole("players"); }
    
    public Dictionary<string,int> getPlayerStats(string username)
    {
        Dictionary<string,int> playerStats = new Dictionary<string,int>();
        PlayerStatsDataSet stats = new PlayerStatsDataSet();
        Guid playerID;

        connection.Open();

        PlayerStatsDataSetTableAdapters.StatsTableAdapter statsTableAdapter = new PlayerStatsDataSetTableAdapters.StatsTableAdapter();
        PlayerStatsDataSetTableAdapters.PlayerStatsTableAdapter playerStatsTableAdapter = new PlayerStatsDataSetTableAdapters.PlayerStatsTableAdapter();
        PlayerStatsDataSetTableAdapters.aspnet_UsersTableAdapter usersTableAdapter = new PlayerStatsDataSetTableAdapters.aspnet_UsersTableAdapter();

        statsTableAdapter.Connection = connection;
        playerStatsTableAdapter.Connection = connection;
        usersTableAdapter.Connection = connection;
        
        
        statsTableAdapter.Fill(stats.Stats);
        playerStatsTableAdapter.Fill(stats.PlayerStats);
        usersTableAdapter.Fill(stats.aspnet_Users);

        /*
         * cauta in tabela aspnet_Users Guid-ul pt username
         **/
        int i, statsID;
        bool exists = false;

        for (i = 0; i < stats.aspnet_Users.Rows.Count; i++)
            if (username.Equals(stats.aspnet_Users[i].UserName))
            {
                exists = true;
                break;
            }

        if (exists == false)
        {
            connection.Close();
            return null;
        }

        exists = false;
        playerID = stats.aspnet_Users[i].UserId;
        
        /*
         * cauta in tabela PlayerStats statsID-ul playerului curent (dat de playerID obtinut mai sus)
         **/
        for (i = 0; i < stats.PlayerStats.Rows.Count; i++)
            if (playerID.ToString().Equals(stats.PlayerStats[i].PlayerID.ToString())){
                exists = true;
                break;
            }

        if (exists == false)
        {
            connection.Close();
            return null;
        }

        statsID = stats.PlayerStats[i].StatsID;

        /*
         * adauga statsurile playerului curent la dicitionar
         **/
        PlayerStatsDataSet.StatsRow sr = stats.Stats.FindByStatsID(statsID);

        playerStats.Add("deaths", sr.Deaths);
        playerStats.Add("frags", sr.Frags);
        playerStats.Add("money", sr.Money);
        playerStats.Add("points", sr.Points);

        connection.Close();
        
        return playerStats;
    }

    public Dictionary<string, LinkedList<string>> getPlayerPowerUps(string username)
    {
        Dictionary<string, LinkedList<string>> pUps = new Dictionary<string, LinkedList<string>>();
        Guid playerID;
        PlayerStatsDataSet stats = new PlayerStatsDataSet();

        PlayerStatsDataSetTableAdapters.aspnet_UsersTableAdapter usersTableAdapter = new PlayerStatsDataSetTableAdapters.aspnet_UsersTableAdapter();
        PlayerStatsDataSetTableAdapters.PlayerPUpTableAdapter playerPUpTableAdapter = new PlayerStatsDataSetTableAdapters.PlayerPUpTableAdapter();
        PlayerStatsDataSetTableAdapters.PUpTableTableAdapter pUpTableAdapter = new PlayerStatsDataSetTableAdapters.PUpTableTableAdapter();

        connection.Open();

        playerPUpTableAdapter.Connection = connection;
        pUpTableAdapter.Connection = connection;
        usersTableAdapter.Connection = connection;

        usersTableAdapter.Fill(stats.aspnet_Users);
        playerPUpTableAdapter.Fill(stats.PlayerPUp);
        pUpTableAdapter.Fill(stats.PUpTable);

        /*
         * cauta in tabela aspnet_Users Guid-ul pt username
         **/
        int i;
        bool exists = false;

        for (i = 0; i < stats.aspnet_Users.Rows.Count; i++)
            if (username.Equals(stats.aspnet_Users[i].UserName))
            {
                exists = true;
                break;
            }

        if (exists == false)
        {
            connection.Close();
            return null;
        }

        exists = false;
        playerID = stats.aspnet_Users[i].UserId;


        /* Urmatoarele 2 secvente de cod comentate sunt doar pt debugging !! Nu le decomenta!!*/

        /*object[] paramets = {1, "pUp1", "descriere pUp1", null, 2, 1, 4};
        stats.PUpTable.Rows.Add(paramets);
        pUpTableAdapter.Update(stats.PUpTable);*/

        /*object[] parames = {playerID,2,1};
        stats.PlayerPUp.Rows.Add(parames);
        playerPUpTableAdapter.Update(stats.PlayerPUp);*/


        /*
         * Construieste o lista cu toate pUp-urile playerului curent
         **/
        LinkedList<int> pUpsList = new LinkedList<int>();
        exists = false;

        for (i = 0; i < stats.PlayerPUp.Rows.Count; i++)
            if (playerID.ToString().Equals(stats.PlayerPUp[i].PlayerID.ToString()))
                pUpsList.AddLast(stats.PlayerPUp[i].PUpID);

        /*
         * Adauga pUp-urile la dictionar
         **/

        LinkedList<string> pUpAttributes;
        PlayerStatsDataSet.PUpTableRow rand;
        for (i = 0; i < pUpsList.Count; i++)
        {
            pUpAttributes = new LinkedList<string>();
            
            /*cauta powerup*/

            rand = stats.PUpTable.FindByPUpID(pUpsList.ElementAt(i));
            pUpAttributes.AddLast(rand.Name);
            pUpAttributes.AddLast(rand.Description);
            /*if (rand.Image != null)
                pUpAttributes.AddLast(rand.Image);
            else
                pUpAttributes.AddLast("no image available");*/
            pUpAttributes.AddLast(rand.NextLevelCost.ToString());
            pUpAttributes.AddLast(rand.NextLevelBenefit.ToString());
            pUpAttributes.AddLast(rand.Effects.ToString());

            /*adauga stats-urile powerupului la dictionar*/
            pUps.Add(pUpsList.ElementAt(i).ToString(), pUpAttributes);
        }
        

        connection.Close();

        return pUps;
    }
}
