﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoStar.app
{
    public partial class GT_Departamento : System.Web.UI.Page
    {
        protected void BindData()
        {
            if (!Page.IsPostBack)
            {
                DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["idUsuario"] == null))
            {
                string connectionString = "Data Source=.;Initial Catalog=GT_AutoStar;Integrated Security=True";
                string queryString = "SELECT * FROM GT_Opcion_Menu JOIN GT_Acceso_Menu AS temp ON GT_Opcion_Menu.idOpcionMenu = temp.idOpcion  JOIN GT_Usuarios ON GT_Usuarios.idRol = temp.idRol WHERE idUsuario = " + Session["idUsuario"];

                DataSet dataset = new DataSet();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(queryString, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    bool tieneAcceso = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        String opcion = dt.Rows[i]["descripcion"].ToString();

                        if (opcion == "Configuracion General")
                        {
                            tieneAcceso = true;
                        }

                    }

                    if (tieneAcceso == false)
                    {
                        Response.Redirect("default.aspx");
                    }

                    connection.Close();
                    connection.Dispose();



                }


            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Para ingresar debe iniciar session');", true);
                Response.Redirect("Default.aspx");
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connect"]);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            Label deleteId = (Label)GridView1.Rows[e.RowIndex].FindControl("idEmpleado");
            cmd.CommandText = "Delete from Empleado where idEmpleado='" + deleteId.Text + "'";
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            DataBind();
        }

        //This event is used to show a row in editable mode.

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            DataBind();
        }

        //This event will update information in database.

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connect"]);
            //Accessing Edited values from the GridView
            string str_id = GridView1.Rows[e.RowIndex].Cells[0].Text; //ID
            string str_nombre = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text; //Company
            string str_apellido1 = ((TextBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0]).Text; //Name
            string str_apellido2 = ((TextBox)GridView1.Rows[e.RowIndex].Cells[3].Controls[0]).Text; //Title
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "Update Empleados set nombre='" + str_nombre + "',apellido1='" + str_apellido1 + "',apellido2='" + str_apellido2 + "' where idEmpleado='" + str_id + "'";
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            GridView1.EditIndex = -1;
            DataBind();
            con.Close();
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCancelEditEventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            
            GridViewRow row = GridView1.Rows[e.NewSelectedIndex];
            //GridView1.SelectRow(e.NewSelectedIndex);


        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {

                // when mouse is over the row, save original color to new attribute, and change it to highlight color
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#EEFFAA'");

                // when mouse leaves the row, change the bg color to its original value  
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");


            }
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowIndex == GridView1.SelectedIndex)
                {
                    GridView1.EditIndex = -1;
                    row.BackColor = ColorTranslator.FromHtml("#8E7070");
                    row.ToolTip = string.Empty;
                }
                else
                {
                    row.BackColor = GridView1.BackColor;
                }
            }
        }

        protected void GridView1_RowCreated(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
                e.Row.ToolTip = "Click to select row";
                if (!(e.Row.RowIndex == GridView1.SelectedIndex))
                {

                    e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);

                }


            }
        }

        protected void btn_eliminarClick(object sender, EventArgs e)     // Panel Eliminar
        {
            try 
            {
                foreach (GridViewRow row in GridView1.Rows)
                {
                    if (row.RowIndex == GridView1.SelectedIndex)
                    {
                        //GridView1.SelectedRow.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Delete$" + GridView1.SelectedRow.RowIndex);
                        GridView1.DeleteRow(row.RowIndex);
                        //GridView1.SetEditRow(row.RowIndex);
                        //GridView1.SelectRow(GridView1.SelectedIndex);
                        row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                        //GridView1.SelectedRow.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                        row.ToolTip = string.Empty;
                    }
                    else
                    {
                        row.BackColor = GridView1.BackColor;
                        //row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                        //row.ToolTip = "Click to select this row.";
                    }
                }
            }
            catch(Exception ex)
            {

            }
            

        }
        
        protected virtual void btn_crearclick(object sender, EventArgs e)
        {
            try 
            {
                SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["connect"]);
                //Accessing Edited values from the GridView


                string descripcion = ((TextBox)GridView1.FooterRow.FindControl("TextBox4")).Text;
                string comentarios = ((TextBox)GridView1.FooterRow.FindControl("TextBox3")).Text;
                bool status = ((CheckBox)GridView1.FooterRow.FindControl("CheckBox3")).Checked;
                SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=GT_AutoStar;Integrated Security=True");
                conn.Open();
                SqlCommand cmd = new SqlCommand("insertArea", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;
                cmd.Parameters.Add("@comentarios", SqlDbType.NVarChar).Value = comentarios;
                cmd.Parameters.Add("@status", SqlDbType.Bit).Value = status;
                cmd.ExecuteReader();
                DataBind();
                con.Close();
            }
            catch(Exception ex)
            {

            }

            

        }        
        

        protected void btn_buscar_click(object sender, ImageClickEventArgs e)
        {
            try 
            {
                SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["connect"]);
                //Accessing Edited values from the GridView

                string valor = TextBox1.Text;  //Company
                string campo = DropDownList1.SelectedItem.Text; //Name


                SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=GT_AutoStar;Integrated Security=True");
                conn.Open();
                SqlCommand cmd = new SqlCommand("areasBusqueda", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@valor", SqlDbType.NVarChar).Value = valor;
                cmd.Parameters.Add("@campo", SqlDbType.NVarChar).Value = campo;
                cmd.ExecuteReader();
                DataBind();
                con.Close();
            }
            catch(Exception ex)
            {

            }
            
        }

        protected void btn_editar(object sender, ImageClickEventArgs e)
        {
            
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowIndex == GridView1.SelectedIndex)
                {

                    GridView1.SetEditRow(row.RowIndex);
                    row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                    row.ToolTip = string.Empty;
                }
                else
                {
                    row.BackColor = GridView1.BackColor;
                }
            }

        
        }

        protected void btn_guardarClick(object sender, ImageClickEventArgs e) 
        {

            try 
            {
                SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["connect"]);
                //Accessing Edited values from the GridView


                int idArea = int.Parse(((Label)GridView1.SelectedRow.FindControl("Label1")).Text);
                string descripcion = ((TextBox)GridView1.SelectedRow.FindControl("TextBox2")).Text;
                string comentarios = ((TextBox)GridView1.SelectedRow.FindControl("TextBox1")).Text;
                bool status = ((CheckBox)GridView1.SelectedRow.FindControl("CheckBox1")).Checked;

                SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=GT_AutoStar;Integrated Security=True");
                conn.Open();
                SqlCommand cmd = new SqlCommand("updateArea", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idArea", SqlDbType.Int).Value = idArea;
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;
                cmd.Parameters.Add("@comentarios", SqlDbType.NVarChar).Value = comentarios;
                cmd.Parameters.Add("@status", SqlDbType.Bit).Value = status;
                cmd.ExecuteReader();
                GridView1.EditIndex = -1;
                DataBind();
                con.Close();
            }
            catch(Exception ex)
            {

            }

            


        }
        
    }
}