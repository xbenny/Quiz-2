﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetProducts();
        }
    }

    /// <summary>
    /// Allows the user to display list of product from
    /// a listview control.
    /// 
    /// Write something here.
    /// </summary>
    /// <param name="keyword>Any keyword</param>
    void GetProducts()
    {
        using (SqlConnection con = new SqlConnection(Util.GetConnection()))
        {
            con.Open();
            string query = @"SELECT p.ProductID, p.Name, c.Category, " +
               "p.Code, p.Description, p.Image, p.Price, " +
               "p.IsFeatured, p.DateAdded, p.DateModified, p.Status " +
               "FROM Products p INNER JOIN Categories c ON p.CatID = c.CatID";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Products");
                    lvProducts.DataSource = ds;
                    lvProducts.DataBind();
                }
            }
        }
    }
    void GetProducts(string keyword)
    {
        using (SqlConnection con = new SqlConnection(Util.GetConnection()))
        {
            con.Open();
            string SQL = @"SELECT p.ProductID, p.Name, c.Category,
                p.Description, p.Image, p.Price, p.IsFeatured,
                p.DateAdded, p.DateModified FROM Products p
                INNER JOIN Categories c ON p.CatID = c.CatID WHERE
                p.ProductID LIKE @keyword OR
                p.Name LIKE @keyword OR
                c.Category LIKE @keyword OR
                p.Description LIKE @keyword OR
                p.Image LIKE @keyword OR
                p.Price LIKE @keyword OR
                p.IsFeatured LIKE @keyword";
            using (SqlCommand cmd = new SqlCommand(SQL, con))
            {
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Products");
                    lvProducts.DataSource = ds;
                    lvProducts.DataBind();
                }
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtKeyword.Text == "")
        {
            GetProducts();
        }
        else
        {
            GetProducts(txtKeyword.Text);
        }
    }
    protected void lvProducts_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpProducts.SetPageProperties(e.StartRowIndex,
            e.MaximumRows, false);
        if (txtKeyword.Text == "")
        {
            GetProducts();
        }
        else
        {
            GetProducts(txtKeyword.Text);
        }
    }
}