using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static OLMapAPI_Core_PoC.Models.BasicItem;

namespace OLMapAPI_Core_PoC.Infrastructure.BasicInfo
{
    public class BasicInfoFunc
    {
        private readonly IConfiguration config;
        public BasicInfoFunc(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<List<LayerResourceList>> getLayerResourceList()
        {
            SqlDataReader dr = null;
            SqlConnection myConnection = new SqlConnection();
            string Constr = config.GetConnectionString("OLDemo_DB");
            myConnection.ConnectionString = Constr;

            SqlCommand sqlCmd = new SqlCommand();

            string sqlStr;
            sqlStr = "SELECT [ID] ,[GroupID] ,[GroupName] ,[LayerID] ,[LayerOrder] ,[LayerQueryable] ,[LayerTitle] ,[LayerType],[DataType] ,[DataURL] ,[LayerVisibleCode] ,[OpenOpacity]  FROM [OLDemo].[dbo].[LayerResource]  order by [GroupID], [LayerOrder], [LayerType]";

            sqlCmd.CommandText = sqlStr;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Connection = myConnection;
            List<LayerResourceList> arrList = new List<LayerResourceList>();

            try
            {
                myConnection.Open();
                dr = sqlCmd.ExecuteReader();

                while (dr.Read())
                {
                    arrList.Add(new LayerResourceList()
                    {
                        ID = dr["ID"].ToString(),
                        GroupID = dr["GroupID"].ToString(),
                        GroupName = dr["GroupName"].ToString(),
                        LayerID = dr["LayerID"].ToString(),
                        LayerOrder = dr["LayerOrder"].ToString(),
                        LayerQueryable = dr["LayerQueryable"].ToString(),
                        LayerTitle = dr["LayerTitle"].ToString(),
                        LayerType = dr["LayerType"].ToString(),
                        DataType = dr["DataType"].ToString(),
                        DataURL = dr["DataURL"].ToString(),
                        LayerVisibleCode = dr["LayerVisibleCode"].ToString(),
                        OpenOpacity = dr["OpenOpacity"].ToString()
                    });
                }
                myConnection.Close();
                myConnection.Dispose();
                return arrList;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
