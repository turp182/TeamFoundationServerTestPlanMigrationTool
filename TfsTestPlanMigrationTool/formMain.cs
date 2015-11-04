//The MIT License (MIT)

//Copyright (c) 20015 Jason Wilbur Turpin

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the Software), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TFSTestMigrationTool
{
    public partial class formMain : Form
    {
        private ITestManagementTeamProject _sourceTestManagementTeamProject;
        private ITestManagementTeamProject _destinationTestManagementTeamProject;

        public formMain()
        {
            InitializeComponent();
        }
            
        #region Private Form Events

        private void btnSelectSourceTfsProject_Click(object sender, EventArgs e)
        {
            _sourceTestManagementTeamProject = TfsOperations.SelectTfsProject(false);

            if (_sourceTestManagementTeamProject != null)
            {            
                lblSourceTfsProject.Text = _sourceTestManagementTeamProject.ToString();

                treeTestPlans.Nodes.Clear();
                
                this.Refresh();
                LoadTestPlansToForm(treeTestPlans);

                myStatus.Text = string.Empty;            
            }
        }

        private void btnSelectDestinationTfsProject_Click(object sender, EventArgs e)
        {
            _destinationTestManagementTeamProject = TfsOperations.SelectTfsProject(true);

            if (_destinationTestManagementTeamProject != null)
            {
                lblDestinationTfsProject.Text = _destinationTestManagementTeamProject.ToString();
            }
        }

        private void btnCopyTestItems_Click(object sender, EventArgs e)
        {
            string results = string.Empty;

            List<TreeNode> checkedPlanNodes = ValidateFormAndReturnSelectedTestPlanNodes();

            if (checkedPlanNodes == null)
            {
                return;
            }
            
            int testPlanIndex = 1;

            foreach (TreeNode planNode in checkedPlanNodes)
            {
                myStatus.Text = string.Format("Processing Test Plan '{0}' ({1}/{2})", planNode.Text, testPlanIndex, checkedPlanNodes.Count);
                this.Refresh();

                results += TfsOperations.CopyTestPlan(_destinationTestManagementTeamProject, planNode.Tag as ITestPlan);

                testPlanIndex++;
            }

            #endregion

            ResultsForm resultsForm = new ResultsForm(results);
            resultsForm.Show();

            myStatus.Text = string.Empty;
            MessageBox.Show("Finished...", "Finished");
        }

        #region Private Methods

        /// <summary>
        /// Loads the test plans for the selected Source TFS project to the treeView control.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        private void LoadTestPlansToForm(TreeView treeView)
        {
            ITestPlanCollection sourceTestPlans = TfsOperations.GetTestPlansForTfsProject(_sourceTestManagementTeamProject);

            int planIndex = 1;

            foreach (ITestPlan plan in sourceTestPlans)
            {
                myStatus.Text = string.Format("Loading Test Plan '{0}' ({1}/{2})", plan.Name, planIndex, sourceTestPlans.Count);
                this.Refresh();

                TreeNode planNode = new TreeNode(plan.Name, 1, 1);
                planNode.Tag = plan;

                treeView.Nodes.Add(planNode);

                planIndex++;
            }

            myStatus.Text = string.Empty;
        }

        /// <summary>
        /// Validates the form and returns the selected test plan nodes.
        /// </summary>
        /// <returns></returns>
        private List<TreeNode> ValidateFormAndReturnSelectedTestPlanNodes()
        {
            if (_sourceTestManagementTeamProject == null)
            {
                MessageBox.Show("Please select a Source TFS Project.", "No Source TFS Project Selected");
                return null;
            }

            if (_destinationTestManagementTeamProject == null)
            {
                MessageBox.Show("Please select a Destination TFS Project.", "No Destination TFS Project Selected");
                return null;
            }

            if (_sourceTestManagementTeamProject.Equals(_destinationTestManagementTeamProject))
            {
                MessageBox.Show("The Source and Destination projects cannot be the same.", "Select TFS Projects Must Be Different");
                return null;
            }

            List<TreeNode> checkedPlanNodes = treeTestPlans.Nodes.Cast<TreeNode>().Where(node => node.Checked).ToList();

            if (checkedPlanNodes.Count == 0)
            {
                MessageBox.Show("Please select at least one Test Plan to copy.", "No Test Plans Selected");
                return null;
            }

            return checkedPlanNodes;
        }

        #endregion
      
    }
}
