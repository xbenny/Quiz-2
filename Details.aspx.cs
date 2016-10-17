using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public partial class Details : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["ID"] != null)
        {
            int productID = 0;
            bool validID = int.TryParse(Request.QueryString["ID"].ToString(),
                            out productID);

            if (validID)
            {
                if (!IsPostBack)
                {
                    GetCategories();
                    GetData(productID);
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
        else
        {
            Response.Redirect("Default.aspx");
        }
    }
    void GetCategories()
    {
        using (SqlConnection con = new SqlConnection(Util.GetConnection()))
        {
            con.Open();
            string SQL = @"SELECT CatID, Category FROM Categories";

            using (SqlCommand cmd = new SqlCommand(SQL, con))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    ddlCategories.DataSource = dr;
                    ddlCategories.DataTextField = "Category";
                    ddlCategories.DataValueField = "CatID";
                    ddlCategories.DataBind();

                    ddlCategories.Items.Insert(0, new ListItem("Select one...", ""));
                }
            }
        }
    }
    void GetData(int ID)
    {
        using (SqlConnection con = new SqlConnection(Util.GetConnection()))
        {
            con.Open();
            string SQL = @"SELECT ProductID, Name, CatID, Code, Description, Image, " +
                "Price, IsFeatured, CriticalLevel, Maximum FROM Products " +
                "WHERE ProductID=@ProductID";

            using (SqlCommand cmd = new SqlCommand(SQL, con))
            {
                cmd.Parameters.AddWithValue("@ProductID", ID);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows) //record is existing
                    {
                        while (dr.Read())
                        {
                            ltID.Text = dr["ProductID"].ToString();
                            ltID2.Text = dr["ProductID"].ToString();
                            txtName.Text = dr["Name"].ToString();
                            ddlCategories.SelectedValue = dr["CatID"].ToString();
                            txtCode.Text = dr["Code"].ToString();
                            txtDescription.Text = dr["Description"].ToString();
                            Session["image"] = dr["Image"].ToString();
                            txtPrice.Text = dr["Price"].ToString();
                            ddlFeatured.SelectedValue = dr["IsFeatured"].ToString();
                            txtCritical.Text = dr["CriticalLevel"].ToString();
                            txtMaximum.Text = dr["Maximum"].ToString();
                        }
                        imgProduct.ImageUrl = "~/Upload/" + Session["image"].ToString();
                    }
                    else //record is not existing
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
            }
        }
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {

        using (SqlConnection con = new SqlConnection(Util.GetConnection()))
        {
            con.Open();
            string SQL = @"UPDATE Products SET Name=@Name, CatID=@CatID, Code=@Code, " +
                "Description=@Description, Image=@Image, Price=@Price, IsFeatured=@IsFeatured, " +
                "CriticalLevel=@CriticalLevel, Maximum=@Maximum, DateModified=@DateModified " +
                "WHERE ProductID=@ProductID";
            //parameterized query

            using (SqlCommand cmd = new SqlCommand(SQL, con))
            {
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@CatID", ddlCategories.SelectedValue);
                cmd.Parameters.AddWithValue("@Code", txtCode.Text);
                cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                if (fuImage.HasFile)
                {
                    string fileExt = Path.GetExtension(fuImage.FileName);
                    string id = Guid.NewGuid().ToString();
                    cmd.Parameters.AddWithValue("@Image", id + fileExt);
                    fuImage.SaveAs(Server.MapPath("~/Upload/" + id + fileExt));
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Image", Session["image"].ToString());
                }
                cmd.Parameters.AddWithValue("@Price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@IsFeatured", ddlFeatured.SelectedValue);
                cmd.Parameters.AddWithValue("@Available", 0);
                cmd.Parameters.AddWithValue("@CriticalLevel", txtCritical.Text);
                cmd.Parameters.AddWithValue("@Maximum", txtMaximum.Text);
                cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                cmd.Parameters.AddWithValue("@ProductID",
                    Request.QueryString["ID"].ToString());

                cmd.ExecuteNonQuery();

                Response.Redirect("Default.aspx");
            }

        }
    }
}