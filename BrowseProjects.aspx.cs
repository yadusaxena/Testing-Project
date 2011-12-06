using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BetaJobJarDAL;
namespace Beta_Job_Jar
{
    public partial class BrowseProjects : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user has logged in using his/her Accenture credentials
            //if (Session["uname"] == null)
            //{
            //    Response.Redirect("Logout.aspx");
            //}

            if (!IsPostBack)
            {
                ProjectData pd = new ProjectData();
                hidUserId.Value = Common.GetUser().ToString(); 
                dgTop20Projects.DataSource = pd.FetchLatest20Projects(Convert.ToInt32(hidUserId.Value));
                dgTop20Projects.DataBind();
                
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string status;
            if (Convert.ToInt32(ddlStatus.SelectedValue) == 0)
                status = "1,2,3";
            else
                status = ddlStatus.SelectedValue;

            string category;
            if (Convert.ToInt32(ddlCategory.SelectedValue) == 0)
                category = "1,2,3";
            else
                category = ddlCategory.SelectedValue;
            Response.Redirect("Search.aspx?title=" + txtTitle.Text + "&createdby=" + txtCreatedBy.Text + "&categoryID=" + category + "&status=" + status + "&description=" + TextBox1.Text);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ProjectTeamData ptd = new ProjectTeamData();
            Common common = new Common();
            string message;
            try
            {
                for (int i = 0; i < dgTop20Projects.Rows.Count; i++)
                {
                    CheckBox joinCheckBox = checked((CheckBox)dgTop20Projects.Rows[i].FindControl("JoinProject"));
                    HyperLink titleHyperlink = checked((HyperLink)dgTop20Projects.Rows[i].FindControl("hlProjectTitle"));
                    if (joinCheckBox.Checked)
                    {

                        string subject = "A user has shown interest in your Project: " + titleHyperlink.Text + ", posted on Beta Job Jar website";

                        //Extract the project Id from the title hyperlink
                        string projectID = titleHyperlink.NavigateUrl.ToString();
                        projectID = projectID.Substring(31);
                        int length = projectID.Length;
                        length = length - 1;
                        projectID = projectID.Substring(0, length);

                        ptd.AddTeamMember(Convert.ToInt32(projectID), titleHyperlink.Text, dgTop20Projects.Rows[i].Cells[2].Text, Convert.ToInt32(hidUserId.Value), ((TextBox)dgTop20Projects.Rows[i].FindControl("Comments")).Text);
                        string body = "Hi, \n The below user has shown interest in your Project:" + titleHyperlink.Text + " (https://betasandbox.accenture.com/BetaJobJar/Post%20Project.aspx?mode=edit&projectid=" + projectID + ") , posted on Beta Job Jar website. \n\n Name :" + Common.GetUserName() + "\n View People Page Profile (https://people.accenture.com/personal/MySite/Person.aspx?accountname=DIR\\" + Common.GetUserName() + ") \n\n Please visit the website to add the member to your team. \n \n Thanks \n Beta Job Jar Team";


                        common.SendMail(dgTop20Projects.Rows[i].Cells[2].Text + "@accenture.com", subject, body);
                    }
                }
                Response.Redirect(Request.Url.ToString());
            }

            catch (Exception exc)
            {
                message = exc.Message.ToString();
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + message + "');", true);
            }
        }

        protected void Top20ProjectsRowBind(object sender, GridViewRowEventArgs e)
        {
            string message;
            try
            {
                if (Convert.ToDateTime(e.Row.Cells[3].Text) >= DateTime.Today)
                {
                }
                else
                {
                    HyperLink hl;
                    
                    //Disable joing project and comments for a project
                    ((CheckBox)(e.Row.FindControl("JoinProject"))).Enabled = false;
                    ((TextBox)(e.Row.FindControl("Comments"))).ReadOnly = true;
                    
                    //Pass a parameter to disable the Join Project button on Project Details page

                    hl = (HyperLink)(e.Row.FindControl("hlProjectTitle"));
                    hl.NavigateUrl = hl.NavigateUrl.Replace(")", "");
                    hl.NavigateUrl.Trim();
                    hl.NavigateUrl = hl.NavigateUrl + ",true" + ')';
                }
            }
            catch (Exception exc)
            {
                message = exc.Message.ToString();
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('" + message + "');", true);
            }
        }
}
}